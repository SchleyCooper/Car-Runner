using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GyroTest : MonoBehaviour
{
    private Quaternion calibrateOffset;

    // Start is called before the first frame update
    void Start()
    {
        Input.gyro.enabled = true;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 v = Input.gyro.attitude.eulerAngles - calibrateOffset.eulerAngles;
        transform.rotation = Quaternion.Euler(v);

        if (Input.anyKeyDown)
            CalibrateGyro();
    }

    void CalibrateGyro()
    {
        calibrateOffset = Input.gyro.attitude;
    }
}
