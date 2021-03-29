using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NavCameraController : MonoBehaviour
{
    public float moveSensitivity = 1;
    private Vector3 lastPosition;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(2))
        {
            lastPosition = Input.mousePosition;
        }
        if (Input.GetMouseButton(2))
        {
            Vector3 delta = Input.mousePosition - lastPosition;
            transform.Rotate(delta.x * moveSensitivity, delta.y * moveSensitivity, 0);
            lastPosition = Input.mousePosition;
        }
    }
}
