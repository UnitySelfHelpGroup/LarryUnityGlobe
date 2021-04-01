using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraGimbal : MonoBehaviour
{
    // Target objects - these have raw, unsmoothed rotations applied
    [Header("Gimbal Target Objects")]
    public Transform horizontalTargetGimbal;
    public Transform verticalTargetGimbal;
    // Smooth objects - these gimbals follow the targets, with smoothing applied
    [Header("Gimbal Smooth Objects")]
    public Transform horizontalGimbal;
    public Transform verticalGimbal;

    [Header("Camera Controls")]
    // Split the inversion up - u never really want to invert both
    public bool xInvert = false;
    public bool yInvert = false;
    public bool zInvert = false;
    public float zoomSpeedFactor = 1;
    public float zoomUpperBound = 50;
    public float zoomLowerBound = -12;
    public GameObject gimbalCamera;

    // Not used in this code - kept in case your planning to do panning or something
    /*
    [Range(1, 5)]
    public float moveSensitivity = 1;
    [Range(0.01f, 10f)]
    public float moveSpeed = 1;
    */

    [Range(0, 100)]
    public float smooth = 50.0f;

    // Speed of camera rotation - aka sensitivity
    [Min(1)]
    public float rotSpeed = 10f;
    // When the mouse is released, how fast the motion slows
    [Min(0.1f)]
    public float friction;


    // Last position to calculate mouse delta with
    private Vector3 lastPosition;
    // Keep track of current vertical rotation - much easier to clamp than figuring it out on the fly
    private float verticalRot;
    // Keep track of rotation speed while the mouse is down, for continuing motion after
    private float lastFrameHorizontalRot;
    private float lastFrameVerticalRot;
    // Magnitude of rotation for after mouse release, decreases over time
    private float rotMagnitude;


    // Update is called once per frame
    void Update()
    {
        if (Input.GetAxisRaw("Mouse ScrollWheel") < 0)
        {
            CameraZoom(-1 * zoomSpeedFactor);
        }

        if (Input.GetAxisRaw("Mouse ScrollWheel") > 0)
        {
            CameraZoom(1 * zoomSpeedFactor);
        }

        if (Input.GetButtonDown("Fire1"))
        {
            lastPosition = Input.mousePosition;
        }
        if (Input.GetButton("Fire1"))
        {
            // Get mouse delta for rotation
            Vector3 delta = Input.mousePosition - lastPosition;
            lastPosition = Input.mousePosition;
            
            // Apply inversions
            if (xInvert) delta.x = -delta.x;
            if (yInvert) delta.y = -delta.y;

            // Whenever we make any translations or rotations in Update, we *must* multiply by Time.deltaTime - or suffer problems like FO76
            delta *= Time.deltaTime * rotSpeed;

            // Get the starting rotations horizontally and vertically so to calculate a frame delta later
            float startFrameHorizontalRot = horizontalGimbal.eulerAngles.y;
            float startFrameVerticalRot = verticalGimbal.localEulerAngles.x;

            // For the horizontal gimbal, just rotate it around the Y axis as needed - nothing fancy
            horizontalTargetGimbal.Rotate(new Vector3(0, delta.x, 0));
            // This does the same for the vertical axis but in a more roundabout way, seeing as we want to clamp the rotation vertically to +-90 degrees.
            // Trying to rotate the object then clamp it's rotation is mathematically fucking hard because Euler angles can get to a rotation in multiple ways, so theres no angle to reliably clamp
            // It is *much* easier to simply keep track of the current vertical rotation independently, and clamp that instead
            verticalRot += delta.y;
            verticalRot = Mathf.Clamp(verticalRot, -90, 90);
            // We set the vertical gimbal's local rotation specifically - we want to keep the rotation it inherets from the horizontal gimbal
            verticalTargetGimbal.localRotation = Quaternion.Euler(verticalRot, 0, 0);
            // Finally, we get the actual gimbals to follow the targets we have just rotated, using smooth Lerps
            horizontalGimbal.rotation = Quaternion.Lerp(horizontalGimbal.rotation, horizontalTargetGimbal.rotation, smooth * Time.deltaTime);
            verticalGimbal.localRotation = Quaternion.Lerp(verticalGimbal.localRotation, verticalTargetGimbal.localRotation, smooth * Time.deltaTime);

            // Get the rotations at the end of the frame and calculate the last frame rotation deltas
            float endFrameHorizontalRot = horizontalGimbal.eulerAngles.y;
            float endFrameVerticalRot = verticalGimbal.localEulerAngles.x;
            // When calculating the deltas themselves, we divide by the current frame Time.deltaTime - giving us a rotation independent of the current frame render time
            // This allows us to divide by other frame Time.deltaTimes later, ensuring a smooth motion
            lastFrameHorizontalRot = (endFrameHorizontalRot - startFrameHorizontalRot) / Time.deltaTime;
            lastFrameVerticalRot = (endFrameVerticalRot - startFrameVerticalRot) / Time.deltaTime;
            // Whenever the mouse is down we want the rotation magnitude to be maxed at 1, ready for use later
            rotMagnitude = 1;

        } else {
            // The mouse is released - begin the long rotation fade!
            // We only continue as long as we have momentum
            if (rotMagnitude > 0) {
                // This is very simple - rotate each gimble by the last frame rotation delta, times the current rotation magnitude, which reduces over time
                horizontalTargetGimbal.Rotate(new Vector3(0, lastFrameHorizontalRot * rotMagnitude * Time.deltaTime, 0));
                // The vertical gimbal has the same clamping code applied here as described above
                verticalRot += lastFrameVerticalRot * rotMagnitude * Time.deltaTime;
                verticalRot = Mathf.Clamp(verticalRot, -90, 90);
                verticalTargetGimbal.localRotation = Quaternion.Euler(verticalRot, 0, 0);
                // The target gimbals themselves are following a smooth motion now - so we don't apply any extra, just copy the rotations to the camera gimbles
                horizontalGimbal.rotation = horizontalTargetGimbal.rotation;
                verticalGimbal.rotation = verticalTargetGimbal.rotation;
                // Reduce the rotation magnitude by the friction value
                rotMagnitude -= friction * Time.deltaTime;
            }
        }

    }
    void CameraZoom(float zoomDir)
    {
        Vector3 cameraPosition = gimbalCamera.transform.position;
        Vector3 worldOrigin = new Vector3(0, 0, 0);
        float cameraDist = Vector3.Distance(cameraPosition, worldOrigin);

        if (zInvert)
        {
            zoomDir =  -zoomDir;
        }
        if ((cameraDist >= zoomUpperBound && zoomDir == -1) || (cameraDist <= zoomLowerBound && zoomDir == 1))
        {
            zoomDir = 0;
        }
        gimbalCamera.transform.Translate(0, 0, zoomDir, Space.Self);

        Debug.Log(("Camera at upper bound = ") + (cameraDist >= zoomUpperBound && zoomDir == -1));
        Debug.Log(("Camera at lower bound = ") + (cameraDist <= zoomLowerBound && zoomDir == 1));
    }
}
    