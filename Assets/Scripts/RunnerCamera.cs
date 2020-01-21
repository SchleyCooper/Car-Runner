using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunnerCamera : MonoBehaviour
{
    public float        posDamping = 1.0f;
    public float        rotDamping = 1.0f;
    public float        distAway = 5.0f;
    public float        distUp = 3.0f;
    public bool         lockRot = true;

    //public Vector3      offset;
    public Transform    target;

    private Vector3     smoothPosition;
    private Quaternion  smoothRotation;

    private void Awake()
    {
        // If no target set, try to find player car instance
        if (!target)
            target = GameObject.Find("PlayerCar").transform;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (!target)
            return;

        //smoothPosition = target.position + -target.forward * distAway + target.up * distUp;
        smoothPosition = target.position + -target.forward * distAway + target.up * distUp;
        smoothRotation = target.rotation;

        transform.position = Vector3.Lerp(transform.position, smoothPosition, posDamping * Time.deltaTime);
        if (!lockRot)
            transform.rotation = Quaternion.Slerp(transform.rotation, smoothRotation, rotDamping * Time.deltaTime);
    }
}
