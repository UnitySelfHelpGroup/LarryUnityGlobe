using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraGimbal : MonoBehaviour
{
    [Header("Gimbal Empties")]
    public GameObject horizontalGimbal;
    public GameObject verticalGimbal;

    [Header("Camera Controls")]
    public bool controlsInverted = false;
    [Range(1, 5)]
    public float moveSensitivity = 1;
    [Range(0.01f, 10f)]
    public float moveSpeed = 1;
    [Range(0, 2)]
    public int mouseButton = 0;
    [Range(0, 100)]
    public float smooth = 50.0f;

    private Vector3 lastPosition;


    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(mouseButton))
        {
            lastPosition = Input.mousePosition;
        }
        if (Input.GetMouseButton(mouseButton))
        {
            Vector3 delta = Input.mousePosition - lastPosition;

            if (controlsInverted == false)
            {
                delta = -delta;
            }

            Quaternion horizontal = Quaternion.Euler(delta.x, 0, 0);
            horizontalGimbal.transform.rotation = Quaternion.Lerp(transform.rotation, horizontal, Time.deltaTime * smooth);

            Quaternion vertical = Quaternion.Euler(0, delta.y, 0);
            verticalGimbal.transform.rotation = Quaternion.Lerp(transform.rotation, vertical, Time.deltaTime * smooth);


            lastPosition = Input.mousePosition;


            //FOR LATER (MAYBE)
            //horizontalGimbal.transform.Rotate(delta.x, 0, 0);
            //verticalGimbal.transform.Rotate(0, delta.y, 0);
        }
    }
}
    