using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PathTesting
{
    public class DirectFollow : MonoBehaviour
    {
        [SerializeField]
        private List<Transform> waypoints;

        [SerializeField]
        private float followSpeed = 10f;
        [SerializeField]
        private float turnSpeed = 10f;
        [SerializeField]
        private float distanceThreshold = 2.0f;
        [SerializeField]
        private float turnThreshold = 2.0f;

        [SerializeField]
        private Transform targetWaypoint;
        private int waypointIdx = 0;

        // Start is called before the first frame update
        void Start()
        {
            if (waypoints.Count > 0)
                targetWaypoint = waypoints[0];   // set starting target, if exists
        }

        // Update is called once per frame
        void Update()
        {
            if (targetWaypoint)
                Movement();
        }

        void Movement()
        {
            Vector3 dir = targetWaypoint.position - transform.position;
            Vector3 angle = targetWaypoint.rotation.eulerAngles - transform.rotation.eulerAngles;

            transform.Translate(dir.normalized * followSpeed * Time.deltaTime, Space.World);
            if (Vector3.Distance(transform.rotation.eulerAngles, targetWaypoint.rotation.eulerAngles) > turnThreshold)
                transform.Rotate(angle.normalized * turnSpeed * Time.deltaTime, Space.World);   // rotate to match waypoint direction
            if (Vector3.Distance(transform.position, targetWaypoint.position) <= distanceThreshold)
            {
                GetNextWaypoint();
            }
        }

        void GetNextWaypoint()
        {
            if (waypoints != null)
            {
                if (waypointIdx >= waypoints.Count - 1)
                {
                    waypointIdx = 0;
                }
                else
                {
                    waypointIdx++;
                }
                targetWaypoint = waypoints[waypointIdx];
            }
            else
                targetWaypoint = null;
        }
    } 
}
