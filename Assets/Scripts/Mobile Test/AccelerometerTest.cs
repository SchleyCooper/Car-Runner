using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AccelerometerTest : MonoBehaviour
{
    public float rollSpeed = 5.0f;

    private Vector3 calibrateOffset;
    private Rigidbody rb;

    private Vector3 accel;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Start is called before the first frame update
    void Start()
    {
        calibrateOffset = new Vector3(Input.acceleration.x, Input.acceleration.z, Input.acceleration.y);
    }

    // Update is called once per frame
    void Update()
    {
        accel = Input.acceleration;
        float t = accel.z;
        accel.z = accel.y;
        accel.y = t;
        accel -= calibrateOffset;
        //accel.z = 0;
        rb.AddForce(accel * rollSpeed);
        
        if (Input.anyKeyDown)
            CalibrateAccelerometer();
    }

    void CalibrateAccelerometer()
    {
        calibrateOffset = new Vector3(Input.acceleration.x, Input.acceleration.z, Input.acceleration.y);
    }

    private void OnGUI()
    {
        GUIStyle style = new GUIStyle();
        style.fontSize = 50;
        GUI.TextArea(new Rect(20, 20, Screen.width / 2, Screen.height / 2), accel.ToString(), style);
    }
}
