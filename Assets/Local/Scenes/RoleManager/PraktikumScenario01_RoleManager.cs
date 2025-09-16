using Ubiq.Messaging;
using Ubiq.Samples;
using UnityEngine;
using VaSiLi.SceneManagement;
using VaSiLi.Logging;
using System;
using Random = UnityEngine.Random;
using VaSiLi.Object;
using TinyJson;
using System.Linq;
using System.Collections.Generic;
using API_3DTI;
using Ubiq.Rooms;

using static API_3DTI.HearingLoss;
using static API_3DTI.HearingLoss.Parameter;
using VisSim;
using UnityEngine.SceneManagement;
using VaSiLi.Networking;
using static VaSiLi.Networking.ResponsiveNetworking;


public class PraktikumScenario01_RoleManager : MonoBehaviour
{

    public GameObject templateObjects;
    public GameObject objectSpawns;
    public GameObject abstractObjectsSet1;
    public GameObject abstractObjectsSet2;

    public GameObject flatTeleporter;
    public Transform spawnP1;
    public Transform spawnP2;
    public Transform spawnP3;

    private GameObject player;
    private SocialMenu socialMenu;
    private LevelManager levelManager;
    private RoomClient roomClient;
    private GameObject voipManager;
    //private GameObject blurCanvas;
    private GameObject centerEyeAnchor;
    private myBlur blurShader;


    //private AudioLowPassFilter lowPassFilter;
    //private AudioHighPassFilter highPassFilter;
    //private AudioReverbFilter reverbFilter;
    //private AudioDistortionFilter distortionFilter;
    private HearingLoss hearingLoss;

    private bool round1check = false;
    private bool round2check = false;

    private NetworkContext context;


    private int playerIdx = -1;

    //private readonly float[] possibleBlurValues = { 0.5f, 0.75f, 1f, 1.5f };
    //private readonly float[] possibleBlurValues = { 0.5f, 1f, 1.5f };
    private readonly float[] possibleBlurValuesV2 = { 5f, 3f, 1f }; //The lower the stronger the blur


    //private RoleShuffle roleShuffleSecondRound;
    private RoleShuffleV2 experimentRoleShuffle;

    private struct RoleShuffleV2
    {
        public string[] roleRound1;
        public string[] roleRound2;

        public string[] disabilityRound1;
        public string[] disabilityRound2;

        public float blur_value;
        public int hearing_value;
    }


    private struct TemplateObjShuffleLog
    {
        public List<int> groupAOrder;  
        public List<int> groupBOrder;
        public List<int> groupCOrder;
        public List<int> groupDOrder;
    }


    // Start is called before the first frame update
    void Start()
    {
        context = NetworkScene.Register(this);
        round1check = false;
        round2check = false;

    }

    void OnEnable()
    {
        roomClient = GameObject.Find("/Social Network Scene").GetComponent<RoomClient>();

        player = GameObject.Find("/MetaPlayer");

        templateObjects = GameObject.Find("/TemplateObjects");
        objectSpawns = GameObject.Find("/ObjSpawns");
        abstractObjectsSet1 = FindInActiveObjectByName("AbstractObjsSet1");
        abstractObjectsSet2 = FindInActiveObjectByName("AbstractObjsSet2");

        flatTeleporter = GameObject.Find("/Teleportations/Teleportation-Flat-Hall");
        spawnP1 = GameObject.Find("/Spawns/P1").transform;
        spawnP2 = GameObject.Find("/Spawns/P2").transform;
        spawnP3 = GameObject.Find("/Spawns/P3").transform;

        centerEyeAnchor = GameObject.Find("/MetaPlayer/OVRCameraRig/TrackingSpace/CenterEyeAnchor");
        socialMenu = GameObject.Find("/Scene Selecter/Menu Manager/Menu").GetComponent<SocialMenu>();
        voipManager = GameObject.Find("/Social Network Scene/Voip Manager");
        levelManager = GameObject.Find("/Social Network Scene/Level Manager").GetComponent<LevelManager>();
        hearingLoss = centerEyeAnchor.GetComponent<HearingLoss>();
        blurShader = centerEyeAnchor.GetComponent<myBlur>();
        templateObjects = GameObject.Find("/TemplateObjects");

        LevelManager.levelStatus += LevelStatusChange;
        RoleManager.roleChanged += RoleChanges;
        roomClient.OnPeerAdded.AddListener(SynchronizeData);
        round1check = false;
        round2check = false;

    }

    void OnDisable()
    {
        hearingLoss.SetParameter(HearingLoss.Parameter.HLOn, false, T_ear.BOTH);
        blurShader.enabled = false;

        LevelManager.levelStatus -= LevelStatusChange;
        RoleManager.roleChanged -= RoleChanges;
        roomClient.OnPeerAdded.RemoveListener(SynchronizeData);
    }

    
    private void SynchronizeData(IPeer _peer)
    {
        Debug.Log("!!!Sync:" + experimentRoleShuffle.ToJson());
        //Back up, wehn disconnect. resend just in case ....
        if (experimentRoleShuffle.disabilityRound1 != null && experimentRoleShuffle.disabilityRound1.Length > 0)
        {
            //ResponsiveNetworking.SendJson(context.Id, experimentRoleShuffle, SimpleStatusMessageHandler);
            Debug.Log("!!!Sync");
            SendRoleShuffleSequence(experimentRoleShuffle);

            //context.SendJson<RoleShuffleV2>(experimentRoleShuffle);
        }
    }

    private void DisableAll()
    {
        Debug.Log("!!! :) Disable all");
        
        hearingLoss.SetParameter(HearingLoss.Parameter.HLOn, false, T_ear.BOTH);

        //blurCanvas.SetActive(false);
        blurShader.enabled = false;

        templateObjects.SetActive(false);
        abstractObjectsSet1.SetActive(false);
        abstractObjectsSet2.SetActive(false);
        flatTeleporter.SetActive(false);
    }

    private void CheckDisabilitesRound1()
    {
        Debug.Log("!!!! - " + round1check + " - " + playerIdx);
        if (round1check || experimentRoleShuffle.disabilityRound1 == null || experimentRoleShuffle.disabilityRound1.Length == 0)
            return;
        if (playerIdx < 0)
            RoleChanges(null);

        Debug.Log("!!!!:) - " + round1check + " - " + playerIdx);
        Debug.Log("!!!" + experimentRoleShuffle.ToJson());
        round1check = true;

        DisableAll();


        abstractObjectsSet1.SetActive(true);
        flatTeleporter.SetActive(true);

        AddHearingLossMixerToPeers();

        
        if (experimentRoleShuffle.disabilityRound1[playerIdx] == "blur")
        {
            //MeshRenderer blurShader = blurCanvas.transform.GetComponentInChildren<MeshRenderer>();
            //Material blurmaterial = blurShader.material;
            //blurmaterial.EnableKeyword("_Radius");
            //int randomNum = Random.Range(0, possibleBlurValues.Length);
            //blurmaterial.SetFloat("_Radius", experimentRoleShuffle.blur_value);
            //blurCanvas.SetActive(true);
            blurShader.maxCPD = experimentRoleShuffle.blur_value;
            blurShader.enabled = true;
            flatTeleporter.SetActive(true);
        }

        
        if (experimentRoleShuffle.disabilityRound1[playerIdx] == "hearing")
        {
            int downSize, upSize;
            float downHz, upHz;
            hearingLoss.SetParameter(Parameter.FrequencySmearingApproach, T_HLFrequencySmearingApproach.Graf, T_ear.BOTH);

            if (experimentRoleShuffle.hearing_value == 0)
            {
                hearingLoss.SetTemporalDistortionFromPreset(T_ear.BOTH, T_HLPreset.HL_PRESET_MILD);

                GetFrequencySmearingGrafPresetValues(T_HLPreset.HL_PRESET_NORMAL, out downSize, out upSize, out downHz, out upHz);

                hearingLoss.SetParameter(FrequencySmearingDownSize, downSize, T_ear.BOTH);
                hearingLoss.SetParameter(FrequencySmearingDownHz, downHz, T_ear.BOTH);
                hearingLoss.SetParameter(FrequencySmearingUpSize, upSize, T_ear.BOTH);
                hearingLoss.SetParameter(FrequencySmearingUpHz, upHz, T_ear.BOTH);

                hearingLoss.SetAudiometryFromClassificationScale(T_ear.BOTH, T_HLClassificationScaleCurve.HL_CS_K, 2, T_HLClassificationScaleSeverity.HL_CS_SEVERITY_MILD);
            }

            if (experimentRoleShuffle.hearing_value == 1)
            {
                hearingLoss.SetTemporalDistortionFromPreset(T_ear.BOTH, T_HLPreset.HL_PRESET_MILD);

                GetFrequencySmearingGrafPresetValues(T_HLPreset.HL_PRESET_MILD, out downSize, out upSize, out downHz, out upHz);

                hearingLoss.SetParameter(FrequencySmearingDownSize, downSize, T_ear.BOTH);
                hearingLoss.SetParameter(FrequencySmearingDownHz, downHz, T_ear.BOTH);
                hearingLoss.SetParameter(FrequencySmearingUpSize, upSize, T_ear.BOTH);
                hearingLoss.SetParameter(FrequencySmearingUpHz, upHz, T_ear.BOTH);

                hearingLoss.SetAudiometryFromClassificationScale(T_ear.BOTH, T_HLClassificationScaleCurve.HL_CS_K, 2, T_HLClassificationScaleSeverity.HL_CS_SEVERITY_MILD);
            }

            if (experimentRoleShuffle.hearing_value == 2)
            {
                hearingLoss.SetTemporalDistortionFromPreset(T_ear.BOTH, T_HLPreset.HL_PRESET_MILD);

                GetFrequencySmearingGrafPresetValues(T_HLPreset.HL_PRESET_MODERATE, out downSize, out upSize, out downHz, out upHz);

                hearingLoss.SetParameter(FrequencySmearingDownSize, downSize, T_ear.BOTH);
                hearingLoss.SetParameter(FrequencySmearingDownHz, downHz, T_ear.BOTH);
                hearingLoss.SetParameter(FrequencySmearingUpSize, upSize, T_ear.BOTH);
                hearingLoss.SetParameter(FrequencySmearingUpHz, upHz, T_ear.BOTH);

                hearingLoss.SetAudiometryFromClassificationScale(T_ear.BOTH, T_HLClassificationScaleCurve.HL_CS_K, 2, T_HLClassificationScaleSeverity.HL_CS_SEVERITY_MILD);
            }

            hearingLoss.SetParameter(Parameter.HLOn, true, T_ear.BOTH);
            hearingLoss.SetParameter(Parameter.TemporalDistortionOn, true, T_ear.BOTH);
            hearingLoss.SetParameter(Parameter.FrequencySmearingOn, true, T_ear.BOTH);
            hearingLoss.SetParameter(Parameter.MultibandExpansionOn, false, T_ear.BOTH);

        }
        Debug.Log("!!!..........Speaker :)");

        if (experimentRoleShuffle.roleRound1[playerIdx] == "speaker")
        {
            Debug.Log("!!!Speaker :)");
            templateObjects.SetActive(true);
            flatTeleporter.SetActive(false);
            GrabbableObject(false, abstractObjectsSet1);
            TemplateObjPlacer();
        }
    }

    private void RoleChanges(ApiRole? role)
    {
        Debug.Log("!!!!RoleManager.CurrentRole?.name:" + RoleManager.CurrentRole?.name);
        if (role?.name == "Person 1")
            playerIdx = 0;
        if (role?.name == "Person 2")
            playerIdx = 1;
        if (role?.name == "Person 3")
            playerIdx = 2;
            
        if(role == null)
        {
            if(RoleManager.CurrentRole?.name == "Person 1")
                playerIdx = 0;
            if (RoleManager.CurrentRole?.name == "Person 2")
                playerIdx = 1;
            if (RoleManager.CurrentRole?.name == "Person 3")
                playerIdx = 2;
        }

        levelManager.CheckinStatus();

        round1check = false;
        round2check = false;
    }

    private void CheckDisabilitesRound2()
    {
        if (round2check || experimentRoleShuffle.disabilityRound1 == null || experimentRoleShuffle.disabilityRound1.Length == 0)
            return;
        round2check = true;

        if (playerIdx < 0)
            RoleChanges(null);
        Debug.Log("!!! - playerIdx: " + playerIdx);
        DisableAll();

        flatTeleporter.SetActive(true);
        abstractObjectsSet2.SetActive(true);

        AddHearingLossMixerToPeers();


        if (experimentRoleShuffle.disabilityRound2[playerIdx] == "blur")
        {
            //MeshRenderer blurShader = blurCanvas.transform.GetComponentInChildren<MeshRenderer>();
            //Material blurmaterial = blurShader.material;
            //blurmaterial.EnableKeyword("_Radius");
            //int randomNum = Random.Range(0, possibleBlurValues.Length);
            //blurmaterial.SetFloat("_Radius", experimentRoleShuffle.blur_value);
            //blurCanvas.SetActive(true);

            blurShader.maxCPD = experimentRoleShuffle.blur_value;
            blurShader.enabled = true;
            flatTeleporter.SetActive(true);
        }
        if (experimentRoleShuffle.disabilityRound2[playerIdx] == "hearing")
        {
            hearingLoss.SetParameter(Parameter.HLOn, true, T_ear.BOTH);
            hearingLoss.SetParameter(Parameter.FrequencySmearingApproach, T_HLFrequencySmearingApproach.Graf, T_ear.BOTH);


            int downSize, upSize;
            float downHz, upHz;
            hearingLoss.SetParameter(Parameter.FrequencySmearingApproach, T_HLFrequencySmearingApproach.Graf, T_ear.BOTH);

            if (experimentRoleShuffle.hearing_value == 0)
            {
                hearingLoss.SetTemporalDistortionFromPreset(T_ear.BOTH, T_HLPreset.HL_PRESET_MILD);

                GetFrequencySmearingGrafPresetValues(T_HLPreset.HL_PRESET_NORMAL, out downSize, out upSize, out downHz, out upHz);

                hearingLoss.SetParameter(FrequencySmearingDownSize, downSize, T_ear.BOTH);
                hearingLoss.SetParameter(FrequencySmearingDownHz, downHz, T_ear.BOTH);
                hearingLoss.SetParameter(FrequencySmearingUpSize, upSize, T_ear.BOTH);
                hearingLoss.SetParameter(FrequencySmearingUpHz, upHz, T_ear.BOTH);

                hearingLoss.SetAudiometryFromClassificationScale(T_ear.BOTH, T_HLClassificationScaleCurve.HL_CS_K, 2, T_HLClassificationScaleSeverity.HL_CS_SEVERITY_MILD);
            }

            if (experimentRoleShuffle.hearing_value == 1)
            {
                hearingLoss.SetTemporalDistortionFromPreset(T_ear.BOTH, T_HLPreset.HL_PRESET_MILD);

                GetFrequencySmearingGrafPresetValues(T_HLPreset.HL_PRESET_MILD, out downSize, out upSize, out downHz, out upHz);

                hearingLoss.SetParameter(FrequencySmearingDownSize, downSize, T_ear.BOTH);
                hearingLoss.SetParameter(FrequencySmearingDownHz, downHz, T_ear.BOTH);
                hearingLoss.SetParameter(FrequencySmearingUpSize, upSize, T_ear.BOTH);
                hearingLoss.SetParameter(FrequencySmearingUpHz, upHz, T_ear.BOTH);

                hearingLoss.SetAudiometryFromClassificationScale(T_ear.BOTH, T_HLClassificationScaleCurve.HL_CS_K, 2, T_HLClassificationScaleSeverity.HL_CS_SEVERITY_MILD);
            }

            if (experimentRoleShuffle.hearing_value == 2)
            {
                hearingLoss.SetTemporalDistortionFromPreset(T_ear.BOTH, T_HLPreset.HL_PRESET_MILD);

                GetFrequencySmearingGrafPresetValues(T_HLPreset.HL_PRESET_MODERATE, out downSize, out upSize, out downHz, out upHz);

                hearingLoss.SetParameter(FrequencySmearingDownSize, downSize, T_ear.BOTH);
                hearingLoss.SetParameter(FrequencySmearingDownHz, downHz, T_ear.BOTH);
                hearingLoss.SetParameter(FrequencySmearingUpSize, upSize, T_ear.BOTH);
                hearingLoss.SetParameter(FrequencySmearingUpHz, upHz, T_ear.BOTH);

                hearingLoss.SetAudiometryFromClassificationScale(T_ear.BOTH, T_HLClassificationScaleCurve.HL_CS_K, 2, T_HLClassificationScaleSeverity.HL_CS_SEVERITY_MILD);
            }

            hearingLoss.SetParameter(Parameter.HLOn, true, T_ear.BOTH);
            hearingLoss.SetParameter(Parameter.TemporalDistortionOn, true, T_ear.BOTH);
            hearingLoss.SetParameter(Parameter.FrequencySmearingOn, true, T_ear.BOTH);
            hearingLoss.SetParameter(Parameter.MultibandExpansionOn, false, T_ear.BOTH);
        }


        if (experimentRoleShuffle.roleRound2[playerIdx] == "speaker")
        {
            templateObjects.SetActive(true);
            flatTeleporter.SetActive(false);
            GrabbableObject(false, abstractObjectsSet2);
            TemplateObjPlacer();
        }
    }

    private void LevelStatusChange(int lvl, LevelManager.Status status)
    {
        Debug.Log("!!!" + lvl + " - " + status);
        if (lvl == 0 && status != LevelManager.Status.RUNNING && RoleManager.CurrentRole?.admin == true)
        {
            Debug.Log("!!!!:)" + experimentRoleShuffle.disabilityRound1);
            if (experimentRoleShuffle.disabilityRound1 == null || experimentRoleShuffle.disabilityRound1.Length == 0) {
                Debug.Log("!!!!Init shuffle .....");
                InitExperimentShuffle();
                MetaDataLogger.miscMessages.Enqueue(new MiscLogMessage()
                {
                    localTime = (int)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds,
                    jsonData = experimentRoleShuffle
                });
            }
            SendRoleShuffleSequence(experimentRoleShuffle);
        }


        if (lvl == 0 && status == LevelManager.Status.RUNNING)
        {
            CheckDisabilitesRound1();
        }

        if (lvl == 1)
            DisableAll();


        if (lvl == 1 && status == LevelManager.Status.READY)
        {
            DisableAll();
            if (RoleManager.CurrentRole?.name == "Person 1")
                player.transform.position = spawnP1.position;
            if (RoleManager.CurrentRole?.name == "Person 2")
                player.transform.position = spawnP2.position;
            if (RoleManager.CurrentRole?.name == "Person 3")
                player.transform.position = spawnP3.position;
            socialMenu.Request();

        }

        if (lvl == 2 && status == LevelManager.Status.RUNNING)
        {
            CheckDisabilitesRound2();
        }

    }

    private void InitExperimentShuffle()
    {
        Debug.Log("!!!!Init");
        string[] roleRound1 = new string[] {"worker", "worker", "speaker"};
        string[] roleRound2 = new string[] {"speaker", "worker", "worker"};
        RandomizeArray(roleRound1);
        RandomizeArray(roleRound2);

        string[] disabilityRound1 = new string[] {"blur", "hearing", "none"};
        string[] disabilityRound2 = new string[] {"none", "blur", "hearing"};
        //string[] disabilityRound1 = new string[] { "none", "none", "none" };
        //string[] disabilityRound2 = new string[] { "none", "none", "none" };
        RandomizeArray(disabilityRound1);
        RandomizeArray(disabilityRound2);

        int rng_blur = Random.Range(0, possibleBlurValuesV2.Length);
        int hearing_value = Random.Range(0, 3);
        //int hearing_value = 0;

        experimentRoleShuffle = new RoleShuffleV2()
        {
            roleRound1 = roleRound1,
            roleRound2 = roleRound2,
            disabilityRound1 = disabilityRound1,
            disabilityRound2 = disabilityRound2,
            blur_value = possibleBlurValuesV2[rng_blur],
            hearing_value = hearing_value
        };
        Debug.Log("!!!!!" + experimentRoleShuffle.ToJson());
    }


    private void SendRoleShuffleSequence(RoleShuffleV2 roleShuffle)
    {
        if (RoleManager.CurrentRole.HasValue && RoleManager.CurrentRole?.mode != Mode.Player)
            return;

        ResponsiveNetworking.SendJson(context.Id, roleShuffle, SimpleStatusMessageHandler);

        //context.SendJson<RoleShuffleV2>(roleShuffle);
    }

    public void SimpleStatusMessageHandler(CallbackResult result)
    {
        if (!result.success)
        {
            var resultContext = result.context;
            if (resultContext == null)
            {
                resultContext = 1;
            }
            else
            {
                int count = (int)resultContext;
                if (count > 5)
                {
                    Debug.LogError("A client couldn't process the message after 5 tries");
                    return;
                }
                count = count + 1;
                resultContext = count;
            }

            ResponsiveNetworking.SendJson(context.Id, result.message, SimpleStatusMessageHandler, resultContext);
        }

    }


    public void ProcessMessage(ReferenceCountedSceneGraphMessage message)
    {
        var m = message.FromJson<RoleShuffleV2>();
        if(m.disabilityRound1 != null)
        {
            experimentRoleShuffle = m;
            Debug.Log("!!!!!" + experimentRoleShuffle.ToJson());


            MetaDataLogger.miscMessages.Enqueue(new MiscLogMessage()
            {
                localTime = (int)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds,
                jsonData = experimentRoleShuffle
            });
        }
    }



    private void AddHearingLossMixerToPeers()
    {
        if (voipManager)
        {
            foreach (AudioSource source in voipManager.GetComponentsInChildren<AudioSource>())
            {
                if(source.outputAudioMixerGroup == null)
                    source.outputAudioMixerGroup = hearingLoss.hlMixer.FindMatchingGroups("Master")[0];
            }
        }
    }
    
    void GrabbableObject(bool grabable, GameObject obj_set)
    {
        for(int i = 0; i < obj_set.transform.childCount; i++)
        {
            obj_set.transform.GetChild(i).GetComponent<GenericThrowable>().enable_grasp = grabable;
        }
    }

    private void TemplateObjPlacer()
    {
        int countAObjs = templateObjects.transform.GetChild(0).childCount;
        List<int> groupAidxList = Enumerable.Range(0, countAObjs).ToList();
        RandomizeList(groupAidxList);
        Debug.Log("!!!Rng listA: " + groupAidxList.ToJson());

        int countBObjs = templateObjects.transform.GetChild(1).childCount;
        List<int> groupBidxList = Enumerable.Range(0, countBObjs).ToList();
        RandomizeList(groupBidxList);
        Debug.Log("!!!Rng listB: " + groupBidxList.ToJson());


        int countCObjs = templateObjects.transform.GetChild(2).childCount;
        List<int> groupCidxList = Enumerable.Range(0, countCObjs).ToList();
        RandomizeList(groupCidxList);
        Debug.Log("!!!Rng listC: " + groupCidxList.ToJson());


        int countDObjs = templateObjects.transform.GetChild(3).childCount;
        List<int> groupDidxList = Enumerable.Range(0, countDObjs).ToList();
        RandomizeList(groupDidxList);
        Debug.Log("!!!Rng listD: " + groupDidxList.ToJson());



        TemplateObjShuffleLog m = new TemplateObjShuffleLog()
        {
            groupAOrder = groupAidxList,
            groupBOrder = groupBidxList,
            groupCOrder = groupCidxList,
            groupDOrder = groupDidxList
        };
        MetaDataLogger.miscMessages.Enqueue(new MiscLogMessage()
        {
            localTime = (int)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds,
            jsonData = m
        });


        int i_a = 0;
        int i_b = 0;
        int i_c = 0;
        int i_d = 0;
        for (int i = 0; i < objectSpawns.transform.childCount; i++)
        {
            Transform room = objectSpawns.transform.GetChild(i);
            for (int j =  0; j < room.childCount; j++)
            {
                Transform spawn = room.GetChild(j);
                Debug.Log("!!! - " + spawn.name + " - " + spawn.position + " - " + spawn.rotation);
                if (spawn.name[0] == 'A')
                {
                    Vector3 spawn_pos = spawn.position;
                    Transform temp_obj = templateObjects.transform.GetChild(0).GetChild(groupAidxList[i_a]);
                    temp_obj.gameObject.SetActive(true);
                    spawn_pos.y = temp_obj.position.y;
                    temp_obj.position = spawn_pos;
                    i_a++;
                }
                
                else if (spawn.name[0] == 'B')
                {
                    Vector3 spawn_pos = spawn.position;
                    Transform temp_obj = templateObjects.transform.GetChild(1).GetChild(groupBidxList[i_b]);
                    temp_obj.gameObject.SetActive(true);
                    spawn_pos.y = temp_obj.position.y;
                    temp_obj.position = spawn_pos;
                    i_b++;
                }
                else if (spawn.name[0] == 'C')
                {
                    Vector3 spawn_pos = spawn.position;
                    Transform temp_obj = templateObjects.transform.GetChild(2).GetChild(groupCidxList[i_c]);
                    temp_obj.gameObject.SetActive(true);
                    spawn_pos.y = temp_obj.position.y;
                    temp_obj.position = spawn_pos;
                    i_c++;
                }
                else if (spawn.name[0] == 'D')
                {
                    Vector3 spawn_pos = spawn.position;
                    Transform temp_obj = templateObjects.transform.GetChild(3).GetChild(groupDidxList[i_d]);
                    temp_obj.gameObject.SetActive(true);
                    spawn_pos.y = temp_obj.position.y;
                    temp_obj.position = spawn_pos;
                    i_d++;
                }
            }

        }

        for(int i=i_a; i < groupAidxList.Capacity; i++) {
            Transform temp_obj = templateObjects.transform.GetChild(0).GetChild(groupAidxList[i]);
            temp_obj.gameObject.SetActive(false);
        }
        for (int i = i_b; i < groupBidxList.Capacity; i++)
        {
            Transform temp_obj = templateObjects.transform.GetChild(1).GetChild(groupBidxList[i]);
            temp_obj.gameObject.SetActive(false);
        }
        for (int i = i_c; i < groupCidxList.Capacity; i++)
        {
            Transform temp_obj = templateObjects.transform.GetChild(2).GetChild(groupCidxList[i]);
            temp_obj.gameObject.SetActive(false);
        }
        for (int i = i_d; i < groupDidxList.Capacity; i++)
        {
            Transform temp_obj = templateObjects.transform.GetChild(3).GetChild(groupDidxList[i]);
            temp_obj.gameObject.SetActive(false);
        }

    }


    void RandomizeList<T>(List<T> list)
    {
        for (int i = 0; i < list.Count - 1; i++)
        {
            T temp = list[i];
            int rand = Random.Range(i, list.Count);
            list[i] = list[rand];
            list[rand] = temp;
        }
    }


    void RandomizeArray<T>(T[] array)
    {
        for (int i = 0; i < array.Length - 1; i++)
        {
            T temp = array[i];
            int rand = Random.Range(i, array.Length);
            array[i] = array[rand];
            array[rand] = temp;
        }
    }


    GameObject FindInActiveObjectByName(string name)
    {
        Transform[] objs = Resources.FindObjectsOfTypeAll<Transform>() as Transform[];
        for (int i = 0; i < objs.Length; i++)
        {
            if (objs[i].hideFlags == HideFlags.None)
            {
                if (objs[i].name == name)
                {
                    return objs[i].gameObject;
                }
            }
        }
        return null;
    }
}

