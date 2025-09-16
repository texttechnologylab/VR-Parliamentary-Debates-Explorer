using Ubiq.Messaging;
using Ubiq.Samples;
using UnityEngine;
using UnityEngine.Video;
using VaSiLi.Movement;
using VaSiLi.SceneManagement;
using VaSiLi.Logging;
using System;
using Random = UnityEngine.Random;
using TinyJson;
using static VaSiLi.Networking.ResponsiveNetworking;
using VaSiLi.Networking;
using Oculus.Platform.Models;
using Ubiq.Rooms;


public class PraktikumScenario02_RoleManager : MonoBehaviour
{
    public static string[,] movies;

    public GameObject Player1_Cinema;
    public GameObject Player2_Cinema;

    //public TeleportOnTrigger Flat_Teleporter;
    public Transform Flat_POS;
    public Transform Cinema1_POS;
    public Transform Cinema2_POS;
    
    private GameObject player;
    private SocialMenu socialMenu;
    //private GameObject Voip_Manager;

    private VideoPlayer player1_player;
    private VideoPlayer player2_player;

    private NetworkContext context;
    //private LevelManager levelManager;


    private MovieSequence myMovieSequence;
    private MovieSequence otherMovieSequence;

    private bool isPlaying;


    // Start is called before the first frame update
    void Start()
    {
        context = NetworkScene.Register(this);

        movies = new string[4, 2];
        movies[0,0] = "Movies/For The Birds";
        movies[0,1] = "Movies/Snack Attack";
        movies[1,0] = "Movies/In The Green";
        movies[1,1] = "Movies/Mars";
        movies[2,0] = "Movies/The Mug";
        movies[2,1] = "Movies/The Black Hole";
        movies[3,0] = "Movies/Dji. Death Sails";
        movies[3,1] = "Movies/The Choice";

        player = GameObject.Find("/MetaPlayer");
        socialMenu = GameObject.Find("/Scene Selecter/Menu Manager/Menu").GetComponent<SocialMenu>();
        //levelManager = GameObject.Find("/Social Network Scene/Level Manager").GetComponent<LevelManager>();
        //Voip_Manager = GameObject.Find("/Social Network Scene/Voip Manager");

        Player1_Cinema = GameObject.Find("/Cinema_Player_1");

        Player2_Cinema = GameObject.Find("/Cinema_Player_2");

        //Flat_Teleporter = GameObject.Find("/Teleportations/Teleportation-Flat-Cinema").GetComponent<TeleportOnTrigger>();

        Flat_POS = GameObject.Find("/Teleportations/Teleportation-Flat-POS").transform;

        Cinema1_POS = GameObject.Find("/Teleportations/Teleportation-Cinema1-POS").transform;

        Cinema2_POS = GameObject.Find("/Teleportations/Teleportation-Cinema2-POS").transform;

        player1_player = Player1_Cinema.GetComponentInChildren<VideoPlayer>();
        player2_player = Player2_Cinema.GetComponentInChildren<VideoPlayer>();
        player1_player.Stop();
        player2_player.Stop();

        isPlaying = false;

        CheckPlayer(null);
    }


    void OnEnable()
    {
        RoleManager.roleChanged += CheckPlayer;
        LevelManager.levelStatus += LevelStatusChange;

        roomClient.OnPeerAdded.AddListener(SynchronizeData);
    }

    private void OnDisable()
    {
        RoleManager.roleChanged -= CheckPlayer;
        LevelManager.levelStatus -= LevelStatusChange;

        roomClient.OnPeerAdded.RemoveListener(SynchronizeData);
    }



    void Update()
    {
        if(isPlaying && !player1_player.isPlaying && !player2_player.isPlaying)
        {
            isPlaying = false;
            socialMenu.Request();
        }
    } 

    private void SynchronizeData(IPeer _peer)
    {
        SendMovieSequence(myMovieSequence);
        SendMovieSequence(otherMovieSequence);
    }

    private void CheckPlayer(ApiRole? role)
    {

        if (SceneManager.CurrentScene?.internalName == "PraktikumScenario02")
        {
            Player1_Cinema.SetActive(false);
            Player2_Cinema.SetActive(false);
            //Flat_Teleporter.teleportTargert = Flat_POS;

            if (RoleManager.CurrentRole?.name == "Person 1")
            {
                Player1_Cinema.SetActive(true);
                //Flat_Teleporter.teleportTargert = Cinema1_POS;
            }

            if (RoleManager.CurrentRole?.name == "Person 2")
            {
                Player2_Cinema.SetActive(true);
                //Flat_Teleporter.teleportTargert = Cinema2_POS;
            }
        }
    }


    private void LevelStatusChange(int lvl, LevelManager.Status status)
    {
        Debug.Log($"!!!!{lvl} : {status}");


        if(RoleManager.CurrentRole?.name == "Person 1" && myMovieSequence.player == 2)
        {
            var tmp = myMovieSequence;
            myMovieSequence = otherMovieSequence;
            otherMovieSequence = tmp;
        }

        if (RoleManager.CurrentRole?.name == "Person 2" && myMovieSequence.player == 1)
        {
            var tmp = myMovieSequence;
            myMovieSequence = otherMovieSequence;
            otherMovieSequence = tmp;
        }


            // Befor first movie, rng the movie seq and send to other player
        if (lvl == 0 && status == LevelManager.Status.WAITING && RoleManager.CurrentRole?.admin == true)
        {
            if(myMovieSequence.movie_sequence == null)
                InitMovieSync();
            return;
        }

        // Every even lvl is a movie watch lvl
        if(lvl % 2 == 0)
        {
            //Voip_Manager.SetActive(false);
            if (status == LevelManager.Status.READY && RoleManager.CurrentRole?.name == "Person 1")
            {
                player.transform.position = Cinema1_POS.position;
                socialMenu.Request();
            }

            if (status == LevelManager.Status.RUNNING && RoleManager.CurrentRole?.name == "Person 1")
            {
                if (!player1_player.isPlaying)
                {
                    int seq = myMovieSequence.movie_sequence[lvl / 2];
                    int pair = myMovieSequence.movie_pair[lvl / 2];
                    player1_player.clip = Resources.Load<VideoClip>(movies[seq, pair]);
                    player1_player.Play();
                    isPlaying = true;
                }
            }

            if (status == LevelManager.Status.READY && RoleManager.CurrentRole?.name == "Person 2")
            {
                player.transform.position = Cinema2_POS.position;
                socialMenu.Request();
            }

            if (status == LevelManager.Status.RUNNING && RoleManager.CurrentRole?.name == "Person 2")
            {
                if (!player2_player.isPlaying)
                {
                    int seq = myMovieSequence.movie_sequence[lvl / 2];
                    int pair = myMovieSequence.movie_pair[lvl / 2];
                    player2_player.clip = Resources.Load<VideoClip>(movies[seq, pair]);
                    player2_player.Play();
                    isPlaying = true;
                }
            }
        }
        else
        {
            Debug.Log("!!! :)");
            // Back to flat and talk!
            if (status == LevelManager.Status.READY)
            {
                player1_player.Stop();
                player2_player.Stop();
                Debug.Log("!!! ------");
                player.transform.position = Flat_POS.position;
                socialMenu.Request();
            }
        }
    }

    private void InitMovieSync()
    {
        int[] seq = new int[4] {0, 1, 2, 3};
        RandomizeArray(seq);

        int[] my_pair = new int[4];
        int[] other_pair = new int[4];
        for (int i = 0; i < seq.Length; i++)
        {
            
            int rng = Random.Range(0, 2);
            my_pair[i] = rng;
            other_pair[i] = 1 - rng;
        }

        myMovieSequence = new MovieSequence()
        {
            player = 1,
            movie_sequence = seq,
            movie_pair = my_pair,
        };
        SendMovieSequence(myMovieSequence);
        Debug.Log("MySequence: " + myMovieSequence.ToJson());

        otherMovieSequence = new MovieSequence()
        {
            player = 2,
            movie_sequence = seq,
            movie_pair = other_pair,
        };
        SendMovieSequence(otherMovieSequence);
        Debug.Log("OtherSequence: " + myMovieSequence.ToJson());

        
        MetaDataLogger.miscMessages.Enqueue(new MiscLogMessage()
        {
            localTime = (int)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds,
            jsonData = myMovieSequence
        });
    }

    private struct MovieSequence
    {
        public int player;
        public int[] movie_sequence;
        public int[] movie_pair;
    }

    private void SendMovieSequence(MovieSequence movieSequence)
    {
        if (RoleManager.CurrentRole.HasValue && RoleManager.CurrentRole?.mode != Mode.Player)
            return;

        ResponsiveNetworking.SendJson(context.Id, movieSequence, SimpleStatusMessageHandler);

        //context.SendJson<MovieSequence>(movieSequence);
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
        var m = message.FromJson<MovieSequence>();
        if (m.player == 2)
            myMovieSequence = m;
        if (m.player == 1)
            otherMovieSequence = m;

        MetaDataLogger.miscMessages.Enqueue(new MiscLogMessage()
        {
            localTime = (int)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds,
            jsonData = myMovieSequence
        });
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
}
