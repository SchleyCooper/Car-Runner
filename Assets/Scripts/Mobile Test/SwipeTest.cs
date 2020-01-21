using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class SwipeTest : MonoBehaviour
{
    public float swipeDeadzoneRadius = 125;
    public Vector3 v;

    private bool isDragging;
    private Vector2 startPos;
    private Vector2 swipeDir;
    private Vector3 worldDir;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        #region Standalone Input

        if (Input.GetMouseButtonDown(0))
        {
            startPos = Input.mousePosition;
            isDragging = true;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            Reset();
        }

        #endregion

        #region Mobile Input

        if (Input.touchCount > 0)
        {
            Touch t = Input.GetTouch(0);
            if (t.phase == TouchPhase.Began)
            {
                startPos = t.position;
                isDragging = true;
            }
            if (t.phase == TouchPhase.Ended || t.phase == TouchPhase.Canceled)
            {
                Reset();
            }
        }

        #endregion

        // Calculate swipe direction
        swipeDir = Vector2.zero;
        if (isDragging)
        {
            if (Input.GetMouseButton(0))
            {
                swipeDir = ((Vector2)Input.mousePosition - startPos);
            }
            else if (Input.touchCount > 0)
            {
                swipeDir = (Input.GetTouch(0).position - startPos);
            }
        }

        // set world direction if full swipe processed & turn object
        if (swipeDir.magnitude > swipeDeadzoneRadius)
        {
            // rotate swipe direction input onto X-Z plane and add local object rotation
            worldDir = transform.rotation * Quaternion.Euler(90, 0, 0) * swipeDir;
            
            // Adjust object's rotation to face according swipe direction
            transform.rotation = Quaternion.LookRotation(worldDir.normalized, transform.up);

            // terminate swipe
            Reset();
        }

        // constantly move object forward
        transform.position = Vector3.MoveTowards(transform.position, transform.position + transform.forward, Time.deltaTime);
        //transform.Translate(transform.forward / 100, Space.Self);

        Debug.DrawRay(transform.position, transform.forward * 5, Color.green);
        Debug.DrawRay(transform.position, transform.rotation * Quaternion.Euler(90,0,0) * swipeDir * 5, Color.red);
    }

    private void Reset()
    {
        isDragging = false;
        startPos = swipeDir = Vector2.zero;
    }

    private void OnDrawGizmos()
    {
        if (isDragging)
            Handles.DrawSolidArc(transform.position, transform.forward, v,
                Vector3.Angle(transform.forward, worldDir), 5);
    }
}
