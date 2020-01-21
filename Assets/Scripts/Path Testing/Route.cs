using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Route : MonoBehaviour
{
    [SerializeField]
    private List<Transform> waypoints;

    private Vector3 gizmosPosition;

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
        for (float t = 0; t <= 1; t += 0.05f)
        {
            gizmosPosition = Mathf.Pow(1 - t, 3) * waypoints[0].position
                + 3 * Mathf.Pow(1 - t, 2) * t * waypoints[1].position
                + 3 * (1 - t) * Mathf.Pow(t, 2) * waypoints[2].position
                + Mathf.Pow(t, 3) * waypoints[3].position;

            Gizmos.color = Color.white;
            Gizmos.DrawSphere(gizmosPosition, 0.25f);
        }

        for (int i = 0; i < waypoints.Count; i += 4)
        {
            if (i + 1 < waypoints.Count)
                Gizmos.DrawLine(waypoints[i].position, waypoints[i + 1].position);
            if (i + 2 < waypoints.Count && i + 3 < waypoints.Count)
                Gizmos.DrawLine(waypoints[i + 3].position, waypoints[i + 2].position);
        }
    }
}
