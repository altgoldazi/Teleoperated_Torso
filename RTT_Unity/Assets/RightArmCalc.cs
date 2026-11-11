using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.InputSystem;

public class RightArmCalc : MonoBehaviour
{
    private float forearmLength = 26;
    private float bicepLength = 34;

    private Vector3 shoulder;
    private Vector3 controller;
    private Vector3 relative;

    public int shoulderPitch;
    public int shoulderRoll;
    public int elbowAngle;

    private float dMax;
    private float dMin = 4;

    public InputActionProperty rightControllerPos;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
       dMax = forearmLength + bicepLength;
    }

    // Update is called once per frame
    void Update()
    {
        //update shoulder x y z and control z y z

        shoulder = GameObject.Find("RightShoulder").transform.position;
        controller = rightControllerPos.action.ReadValue<Vector3>();

        //Debug.Log($"ShoulderPos: {shoulder.x}, {shoulder.y}, {shoulder.z} ControllerPos: {controller.x}, {controller.y}, {controller.z}");

        //find coords of controller relative to shoulder
        relative.x = (controller.x - shoulder.x) * 100;
        relative.y = (controller.y - shoulder.y) * 100;
        relative.z = (controller.z - shoulder.z) * 100;

        //Debug.Log($"Distance: {relative.x}, {relative.y}, {relative.z}");

        //calculate elbow angle

        dMax = forearmLength + bicepLength;
        float D = Mathf.Clamp(Vector3.Distance(controller, shoulder) * 100, dMin, dMax); //calculate distance between shoulder and controller and adjust for out of reach

        //Debug.Log($"Distance: {D}");

        elbowAngle = (int)(Mathf.Acos(Mathf.Clamp((Mathf.Pow(forearmLength, 2) + Mathf.Pow(bicepLength, 2) - Mathf.Pow(D, 2)) / (2 * forearmLength * bicepLength), -1, 1)) * Mathf.Rad2Deg); //law of cosines to calculate interior angle in degrees
        Mathf.Clamp(elbowAngle, 0, 180);
        elbowAngle = 180 - elbowAngle;

        //Debug.Log($"Elbow: {elbowAngle}");

        //calculate shoulder roll

        shoulderRoll = (int)(Mathf.Asin(Mathf.Clamp((relative.x / D), -1, 1)) * Mathf.Rad2Deg);
        shoulderRoll = 90 - shoulderRoll;
        //saftey for testing
        if (shoulderRoll > 90)
        {
            shoulderRoll = 90;
        }
        //calculate shoulder pitch

        float phi = Mathf.Atan2(relative.y, relative.z) * Mathf.Rad2Deg;

        float ratio = Mathf.Clamp(forearmLength * Mathf.Sin(elbowAngle * Mathf.Deg2Rad) / D, -1, 1);
        float ph = Mathf.Asin(ratio) * Mathf.Rad2Deg;

        shoulderPitch = (int)(90 - ph + phi);
        if (shoulderPitch < 0)
        {
            shoulderPitch += 360;
        }
        if (shoulderPitch <=180)
        {
            shoulderPitch += 180;
        }
        else if (shoulderPitch > 180)
        {
            shoulderPitch -= 180;
        }
        shoulderPitch = 360 - shoulderPitch;

        Debug.Log($"Shoulder Pitch: {shoulderPitch}");
    }
}

