using UnityEngine;
using UnityEngine.Events;

#if ENABLE_VR || ENABLE_AR
using Ubiq.XR;
#endif

namespace PathCreation.Examples
{
    public class PathFolowerRoundTrip : MonoBehaviour
    {
        public PathCreator pathCreator;
        public EndOfPathInstruction endOfPathInstruction;
        public UnityAction<bool> EndOfPathFunction = delegate { };

        public float maxSpeed = 5f;
        public float acceleration = 0.5f;
        public Quaternion rotationOffset = Quaternion.Euler(0, 90, 0);
        [SerializeField]
        private GameObject _pathHint;
        private bool _pathHintVisibility = true;
        float speed = 0f;
        float distanceTravelled = 0f;
        bool accelerate = false;
        public HandController[] handControllers;

        void Start() {
            if (pathCreator != null)
            {
                // Subscribed to the pathUpdated event so that we're notified if the path changes during the game
                pathCreator.pathUpdated += OnPathChanged;
            }
        }

        public void StartRoundtrip()
        {
            accelerate = true;
        }

        void Update()
        {
            /*
            foreach (var item in handControllers)
            {
                if (item.PrimaryButtonPress.previousvalue)
                {
                    accelerate = !accelerate;
                }
            }
            if (Input.GetKeyDown("w"))
            {
                accelerate = !accelerate;
            }
            if (Input.GetKeyDown("h"))
            {
                _pathHintVisibility = !_pathHintVisibility;
                _pathHint.SetActive(_pathHintVisibility);
            }*/
            if (pathCreator != null)
            {
                accelerateSpeed();
                distanceTravelled += speed * Time.deltaTime;
                Vector3 new_position = pathCreator.path.GetPointAtDistance(distanceTravelled, endOfPathInstruction);
                transform.position = new_position;


                if (distanceTravelled >= pathCreator.path.length)
                {
                    EndOfPathFunction.Invoke(true);
                }


                /*
                #if !(ENABLE_VR || ENABLE_AR)
                    #transform.rotation = pathCreator.path.GetRotationAtDistance(distanceTravelled, endOfPathInstruction);
                #endif
                #if ENABLE_VR || ENABLE_AR
                    transform.rotation = rotationOffset;
                #endif
                */
            }
        }

        // If the path changes during the game, update the distance travelled so that the follower's position on the new path
        // is as close as possible to its position on the old path
        void OnPathChanged() {
            distanceTravelled = pathCreator.path.GetClosestDistanceAlongPath(transform.position);
        }
    
        void accelerateSpeed()
        {
            if (accelerate && speed < maxSpeed)
            {
                speed = speed + acceleration * Time.deltaTime;
            }
            else if (!accelerate && speed > 0f)
            {
                speed = speed - acceleration * Time.deltaTime;
            }
            else if (speed > maxSpeed)
            {
                speed = maxSpeed;
            }
            else if (speed < 0f)
            {
                speed = 0f;
            }
        }
    }
}

