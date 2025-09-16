using PathCreation;
using PathCreation.Examples;
using Ubiq.Avatars;
using Ubiq.Samples;
using Ubiq.XR;
using UnityEngine;
using VaSiLi.SceneManagement;


public class PraktikumScenario03_RoleManager : MonoBehaviour
{
    //public TeleportOnTrigger Flat_Teleporter;
    public Transform Flat_POS;
    public PathCreator PathCreator;
    

    private GameObject player;
    private AvatarManager avatarManager;
    private GameObject centerEyeAnchor;
    private SocialMenu socialMenu;
    private PathFolowerRoundTrip trip;
    private LevelManager lvlManager;

    // Experiment Settings
    private float trip_acceleration = 4f;  
    private float trip_maxSpeed = 10f;
    private float walking_speed = 10f;


    // Start is called before the first frame update
    void Start()
    {
        Flat_POS = GameObject.Find("/Spawns/P1").transform;
        PathCreator = GameObject.Find("/RundTrip/SplinePath").GetComponent<PathCreator>();

        player = GameObject.Find("/MetaPlayer");
        socialMenu = GameObject.Find("/Scene Selecter/Menu Manager/Menu").GetComponent<SocialMenu>();
        lvlManager = GameObject.Find("/Social Network Scene/Level Manager").GetComponent<LevelManager>();
        avatarManager = GameObject.Find("/Social Network Scene/Avatar Manager").GetComponent<AvatarManager>();
        centerEyeAnchor = GameObject.Find("/MetaPlayer/OVRCameraRig/TrackingSpace/CenterEyeAnchor");
        centerEyeAnchor.GetComponent<Camera>().farClipPlane = 2000;
    }


    void OnEnable()
    {
        LevelManager.levelStatus += LevelStatusChange;
    }

    void OnDisable()
    {
        LevelManager.levelStatus -= LevelStatusChange;
    }

    private void LevelStatusChange(int lvl, LevelManager.Status status)
    {
        Debug.Log("!!! - " +  lvl + " - " +  status);
        avatarManager.LocalAvatar.gameObject.SetActive(true);
        if (lvl == 0 && status == LevelManager.Status.READY && RoleManager.CurrentRole?.name == "Person 1")
        {
            if(trip == null)
            {
                trip = player.AddComponent<PathFolowerRoundTrip>();
                trip.pathCreator = PathCreator;
                trip.endOfPathInstruction = EndOfPathInstruction.Stop;
                trip.EndOfPathFunction += PathEnded;
                trip.acceleration = trip_acceleration;
                trip.maxSpeed = trip_maxSpeed;

                player.transform.position = trip.pathCreator.path.GetPoint(0);
                socialMenu.Request();
            }
        }

        if (lvl == 0 && status == LevelManager.Status.RUNNING && RoleManager.CurrentRole?.name == "Person 1")
        {
            trip.StartRoundtrip();
            avatarManager.LocalAvatar.gameObject.SetActive(false);

        }

        if (lvl == 1 && status == LevelManager.Status.READY && RoleManager.CurrentRole?.name == "Person 1")
        {
            Debug.Log("!!! Player position:" + player.transform.position);
            Debug.Log("!!! Flat Position:" + Flat_POS.position);

            player.transform.position = Flat_POS.position;


            //player.transform.position = Flat_POS.position;
            //player.transform.rotation = Flat_POS.rotation;

            Debug.Log("!!! Player position2:" + player.transform.position);
            socialMenu.Request();
        }

        if (lvl == 2 && status == LevelManager.Status.READY && RoleManager.CurrentRole?.name == "Person 2")
        {
            Debug.Log("!!! Player position:" + player.transform.position);
            Debug.Log("!!! Flat Position:" + PathCreator.path.GetPoint(0));
            player.transform.position = PathCreator.path.GetPoint(0);
            player.GetComponent<XRPlayerController>().joystickFlySpeed = walking_speed;

            Debug.Log("!!! Player position2:" + player.transform.position);
            socialMenu.Request();
        }
    }


    private void PathEnded(bool ended)
    {
        if (ended)
        {
            Debug.Log("!!! Path Ended");
            lvlManager.TryCompleteLevel();
            trip.EndOfPathFunction -= PathEnded;
            Destroy(trip);
        }
    }
}
