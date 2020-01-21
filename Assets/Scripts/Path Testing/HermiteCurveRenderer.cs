using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HermiteCurveRenderer : MonoBehaviour
{
    public List<Transform> waypoints = new List<Transform>();

    [SerializeField]
    [Range(2, 100)]
    private int steps = 20;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnDrawGizmos()
    {
        float t = 0;
        Vector3 newPos, oldPos;

        Gizmos.color = Color.white;

        // need at least 2 control points
        if (waypoints.Count > 1)
        {
            newPos = waypoints[0].position;
            for (int i = 0; i < waypoints.Count - 1; i++)
            {
                Vector3 p0 = waypoints[i].position;
                Vector3 m0 = (waypoints[i].GetChild(0).position - waypoints[i].position).magnitude * waypoints[i].forward;
                Vector3 p1 = waypoints[i + 1].position;
                Vector3 m1 = (waypoints[i + 1].GetChild(0).position - waypoints[i + 1].position).magnitude * waypoints[i + 1].forward;

                for (int j = 0; j < steps; j++)
                {
                    t = j / (steps - 1.0f);

                    oldPos = newPos;
                    newPos = (2.0f * t * t * t - 3.0f * t * t + 1.0f) * p0
                    + (t * t * t - 2.0f * t * t + t) * m0
                    + (-2.0f * t * t * t + 3.0f * t * t) * p1
                    + (t * t * t - t * t) * m1;


                    Gizmos.DrawLine(oldPos, newPos);
                }
            }
        }

        foreach(Transform w in waypoints)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(w.position, w.GetChild(0).position);
        }
    }
}
