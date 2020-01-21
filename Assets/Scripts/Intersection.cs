using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class Intersection : MonoBehaviour
{
    [SerializeField]
    private List<Transform> availablePaths;
    
    // angles to each available path
    private Dictionary<Transform, Vector3>   directions = new Dictionary<Transform, Vector3>();

    private void OnValidate()
    {
        directions = new Dictionary<Transform, Vector3>();

        foreach (Transform t in availablePaths)
        {
            directions.Add(t, t.position - transform.position);
        }
    }

    public Dictionary<Transform, Vector3> GetDirections()
    {
        return directions;
    }

    private void OnDrawGizmos()
    {
        if (directions.Count > 0)
        {
            foreach (KeyValuePair<Transform, Vector3> d in directions)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawLine(transform.position, d.Key.position);
            }
        }
    }
}
