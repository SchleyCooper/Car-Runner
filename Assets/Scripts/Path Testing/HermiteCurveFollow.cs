using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HermiteCurveFollow : MonoBehaviour
{
    [SerializeField]
    private List<Transform> waypoints;
    [SerializeField]
    private float speedModifier = 0.5f;

    private float tParam;
    private Vector3 newPos, oldPos;
    private bool coroutineAllowed;


    // Start is called before the first frame update
    void Start()
    {
        tParam = 0;
        coroutineAllowed = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (coroutineAllowed && waypoints.Count > 1)
            StartCoroutine(FollowCurve());
    }

    private IEnumerator FollowCurve()
    {
        coroutineAllowed = false;
        Vector3 p0, m0, p1, m1;

        

        while (waypoints.Count > 1)
        {
            p0 = waypoints[0].position;
            m0 = (waypoints[0].GetChild(0).position - waypoints[0].position).magnitude * waypoints[0].forward;
            p1 = waypoints[1].position;
            m1 = (waypoints[1].GetChild(0).position - waypoints[1].position).magnitude * waypoints[1].forward;

            tParam = 0;
            while (tParam < 1)
            {
                tParam += Time.deltaTime * speedModifier;

                oldPos = transform.position
                    ;
                newPos = (2.0f * tParam * tParam * tParam - 3.0f * tParam * tParam + 1.0f) * p0
                + (tParam * tParam * tParam - 2.0f * tParam * tParam + tParam) * m0
                + (-2.0f * tParam * tParam * tParam + 3.0f * tParam * tParam) * p1
                + (tParam * tParam * tParam - tParam * tParam) * m1;

                transform.position = newPos;
                transform.rotation = Quaternion.LookRotation(newPos - oldPos);
                yield return new WaitForEndOfFrame();
            }
            waypoints.RemoveAt(0);
        }

        coroutineAllowed = true;
    }
}
