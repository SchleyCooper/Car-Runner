using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwipeTrail : MonoBehaviour
{
    [SerializeField]
    private Camera cam;

    // Update is called once per frame
    void Update()
    {
        Plane objPlane = new Plane(cam.transform.forward * -1, transform.position);
        if (((Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Moved) ||
            Input.GetMouseButton(0)))
        {

            Ray mRay = cam.ScreenPointToRay(Input.mousePosition);
            float rayDistance;
            if (objPlane.Raycast(mRay, out rayDistance))
                this.transform.position = mRay.GetPoint(rayDistance);
        }
    }
}
