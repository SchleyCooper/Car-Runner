using UnityEngine;
using System.Collections.Generic;
[RequireComponent(typeof(LineRenderer))]
public class BezierRenderer : MonoBehaviour
{
    public Transform[] controlPoints;
    public LineRenderer lineRenderer;

    private int curveCount = 0;
    private int layerOrder = 0;
    private int SEGMENT_COUNT = 50;


    void Start()
    {
        if (!lineRenderer)
        {
            lineRenderer = GetComponent<LineRenderer>();
        }
        lineRenderer.sortingLayerID = layerOrder;
        curveCount = (int)controlPoints.Length / 4;
    }

    void Update()
    {

        DrawCurve();

    }

    void DrawCurve()
    {
        lineRenderer.SetPosition(0, controlPoints[0].position);

        for (int j = 0; j < curveCount; j++)
        {
            for (int i = 1; i <= SEGMENT_COUNT; i++)
            {
                float t = i / (float)SEGMENT_COUNT;
                int nodeIndex = j * 4;
                Vector3 pixel = CalculateCubicBezierPoint(t, controlPoints[nodeIndex].position, controlPoints[nodeIndex + 1].position, 
                                                            controlPoints[nodeIndex + 2].position, controlPoints[nodeIndex + 3].position);
                lineRenderer.positionCount = j * SEGMENT_COUNT + i + 1;
                lineRenderer.SetPosition(j * SEGMENT_COUNT + i, pixel);
            }
        }
    }

    Vector3 CalculateCubicBezierPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
    {
        float u = 1 - t;
        float tt = t * t;
        float uu = u * u;
        float uuu = uu * u;
        float ttt = tt * t;

        Vector3 p = uuu * p0;
        p += 3 * uu * t * p1;
        p += 3 * u * tt * p2;
        p += ttt * p3;

        return p;
    }

    private void OnDrawGizmos()
    {
        for (int i = 0; i < controlPoints.Length; i += 4)
        {
            if (i + 1 < controlPoints.Length)
                Gizmos.DrawLine(controlPoints[i].position, controlPoints[i + 1].position);
            if (i + 2 < controlPoints.Length && i + 3 < controlPoints.Length)
                Gizmos.DrawLine(controlPoints[i+3].position, controlPoints[i + 2].position);
        }
    }
}