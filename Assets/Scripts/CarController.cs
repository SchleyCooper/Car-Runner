using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarController : MonoBehaviour
{
    public enum MoveType
    {
        SLIDING,
        LANE_SWITCHING,
    }

    public float forwardSpeed = 3.0f;
    public float sideSpeed = 2.0f;
    public float switchSpeed = 5.0f;
    public float turnSpeed = 3.0f;
    public float boostSpeed = 6.0f;

    [Header("Lane Switching")]
    public float laneDistance = 5.0f;
    public float gyroFlickRotThreshold = 10.0f;

    [Header("Damping")]
    public float posDamping = 4.0f;
    public float rotDamping = 4.0f;

    public MoveType moveType = MoveType.SLIDING;
    public bool     waitForLaneSwitch;
    private bool    isChangingLanes;

    private Vector3         smoothPosition;
    private Quaternion      smoothRotation;

    private Vector3         laneOffsetDest;

    private InputManager    inputManager;
    private Rigidbody       rb;

    private GameObject go;

    private void Awake()
    {
        inputManager = GetComponent<InputManager>();
    }

    // Start is called before the first frame update
    void Start()
    {
        //Input.simulateMouseWithTouches = true;
        //Input.gyro.enabled = true;
        go = new GameObject("LANE_DESTINATION");
    }

    // Update is called once per frame
    void Update()
    {
        // Set smoothed position according to translation type setting
        if (moveType == MoveType.SLIDING)
        {
            // allow car to move freely across lanes
            smoothPosition = transform.position + transform.forward * /*inputManager.vertical * */forwardSpeed + transform.right * inputManager.horizontal * sideSpeed;
        }
        else if (moveType == MoveType.LANE_SWITCHING)
        {
            if (true)   // --- FIX: STILL ALLOW FORWARD MOVEMENT? --- //
            {
                // Must stay in defined lanes, set smoothedPosition to adjacent lane on button press/screen tilt
                smoothPosition = transform.position + transform.forward * /*inputManager.vertical * */forwardSpeed;

                if (isChangingLanes)
                    smoothPosition += smoothPosition - transform.position;
                else
                {
                    if (Input.gyro.enabled && Mathf.Abs(Input.gyro.rotationRate.y) > gyroFlickRotThreshold)
                    {
                        // use gyro 'flick' for lane switching
                        laneOffsetDest = transform.right * (Input.gyro.rotationRate.y > 0 ? 1 : -1) * laneDistance;
                        smoothPosition += laneOffsetDest;
                        isChangingLanes = true;

                        go.transform.position = transform.position + laneOffsetDest;
                    }
                    else
                    {
                        // use regular button trigger to change lanes
                        if (Input.GetKey(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow)/*inputManager.horizontal >= 0.1f*/)
                        {
                            smoothPosition += transform.right * -laneDistance;
                            isChangingLanes = true;
                        }
                        else if (Input.GetKey(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow)/*inputManager.horizontal <= -0.1f*/)
                        {
                            smoothPosition += transform.right * laneDistance;
                            isChangingLanes = true;
                        }
                    }
                }
            }
            else if (!waitForLaneSwitch)
            {
                // still smoothing lane switch, need to maintain forward movement
                Vector3 dist = smoothPosition - transform.position;
                smoothPosition = transform.position + transform.forward * forwardSpeed + dist;


                //Vector3 dist = smoothPosition - transform.position;
                //smoothPosition = transform.position + transform.forward * /*inputManager.vertical * */forwardSpeed + dist;
            }
        }

        // Process car rotation via swipe or button input
        if (inputManager.IsDragging)
        {
            // Swipe is in progress, check if swipe is completed
            if (inputManager.SwipeDir.magnitude > inputManager.swipeDeadzoneRadius)
            {
                // rotate swipe direction input onto X-Z plane and add local object rotation
                inputManager.worldDir = transform.rotation * Quaternion.Euler(90, 0, 0) * inputManager.SwipeDir;

                // Adjust object's rotation to face according swipe direction
                //transform.rotation = Quaternion.LookRotation(inputManager.worldDir.normalized, transform.up);
                smoothRotation = Quaternion.LookRotation(inputManager.worldDir.normalized, transform.up);

                // terminate swipe
                inputManager.ResetSwipeData();
            }
        }
        else
        {
            // Set smoothed rotation according to button press/screen swipe
            if (Input.GetKeyDown(KeyCode.E)/*inputManager.mouseX > 0.8f*/)
            {
                smoothRotation = transform.rotation * Quaternion.Euler(0, 90, 0);
            }
            else if (Input.GetKeyDown(KeyCode.Q)/*inputManager.mouseX < -0.8f*/)
            {
                smoothRotation = transform.rotation * Quaternion.Euler(0, -90, 0);
            }
            //else if (Input.touchCount > 0)
            //{
            //    smoothRotation = transform.rotation * Quaternion.Euler(0,
            //        (Input.GetTouch(0).deltaPosition.magnitude > 0.5f ? 1 : Input.GetTouch(0).deltaPosition.magnitude < -0.5f ? -1 : 0) * 90, 0);
            //}
        }

        // Apply smooth translation & rotation
        SmoothTransform();
    }

    void SmoothTransform()
    {
        transform.position = Vector3.Lerp(transform.position, smoothPosition, Time.deltaTime * posDamping);
        if ((smoothPosition - transform.position).magnitude < 0.001f && moveType == MoveType.LANE_SWITCHING)
            isChangingLanes = false;

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
