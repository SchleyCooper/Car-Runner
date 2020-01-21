using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunnerController : MonoBehaviour
{
    [Header("Speed Settings")]
    [SerializeField]
    private float startingFwdSpeed = 3.0f;
    [SerializeField]
    private float sideSpeed = 2.0f;

    [Header("Damping")]
    public float posDamping = 4.0f;
    public float rotDamping = 4.0f;

    private Vector3     smoothPosition;
    private Quaternion  smoothRotation;


    private float currentFwdSpeed;
    [SerializeField] private bool inIntersection = false, hasTurned = false;

    private Intersection currentIntersection;

    private InputManager inputManager;

    private void Awake()
    {
        inputManager = GetComponent<InputManager>();
    }

    // Start is called before the first frame update
    void Start()
    {
        currentFwdSpeed = startingFwdSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        if (inIntersection && !hasTurned)
        {
            // Process car rotation via swipe or button input
            if (inputManager.IsDragging)
            {
                Dictionary<Transform, Vector3> directions = currentIntersection.GetDirections();

                // Swipe is in progress, check if swipe is completed
                if (inputManager.SwipeDir.magnitude > inputManager.swipeDeadzoneRadius && currentIntersection.GetDirections().Count > 0)
                {
                    float directionAccuracy = float.MaxValue;
                    Transform targetRoad = null;

                    // rotate swipe direction input onto X-Z plane and add local object rotation
                    inputManager.worldDir = transform.rotation * Quaternion.Euler(90, 0, 0) * inputManager.SwipeDir;

                    foreach (KeyValuePair<Transform, Vector3> d in directions)
                    {
                        float angle = Vector3.Angle(inputManager.worldDir.normalized, d.Value.normalized);
                        if (angle < directionAccuracy)
                        {
                            directionAccuracy = angle;
                            targetRoad = d.Key;
                        }
                    }

                    // Adjust object's rotation to face according swipe direction
                    smoothRotation = Quaternion.LookRotation(directions[targetRoad].normalized, transform.up);

                    // terminate swipe
                    inputManager.ResetSwipeData();

                    hasTurned = true;
                }
            }
            else
            {
                // Set smoothed rotation according to button press/screen swipe
                if (Input.GetKeyDown(KeyCode.E))
                {
                    smoothRotation = transform.rotation * Quaternion.Euler(0, 90, 0);
                    hasTurned = true;
                }
                else if (Input.GetKeyDown(KeyCode.Q))
                {
                    smoothRotation = transform.rotation * Quaternion.Euler(0, -90, 0);
                    hasTurned = true;
                }
            }
        }

        // move rig fwd at constant starting speed [allow car to move freely across lanes]
        smoothPosition = transform.position + transform.forward * /*inputManager.vertical * */currentFwdSpeed 
            + (inIntersection && !hasTurned ? Vector3.zero : transform.right * inputManager.horizontal * sideSpeed);

        // apply transform changes
        SmoothTransform();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "TurnTrigger")
        {
            inIntersection = true;
            currentIntersection = other.gameObject.GetComponent<Intersection>();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "TurnTrigger")
        {
            // make sure to reset necessary flags
            inIntersection = false;
            hasTurned = false;
            currentIntersection = null;
        }
    }

    void SmoothTransform()
    {
        transform.position = Vector3.Lerp(transform.position, smoothPosition, Time.deltaTime * posDamping);
        //if ((smoothPosition - transform.position).magnitude < 0.001f && moveType == MoveType.LANE_SWITCHING)
        //    isChangingLanes = false;

        Quaternion newRot = Quaternion.Slerp(transform.rotation, smoothRotation, Time.deltaTime * rotDamping);
        // make sure rotation is still multiple of 90 deg
        //if (newRot.y % 90 != 0)
        //{
        //    //Debug.Log(newRot + " -- " + newRot.y % 90);
        //    newRot.y -= newRot.y % 90;
        //}
        transform.rotation = newRot;
    }
}
