using UnityEngine;

public class NeckCalc : MonoBehaviour
{

    public int neckYawAngle;
    public int neckPitchAngle;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //determine and update angles
        Vector3 headVector = Camera.main.transform.localEulerAngles;
        int theta = (int)headVector[1];
        int phi = (int)headVector[0];
        if (theta <= 90)
        {
            neckYawAngle = 90 - theta;
        }
        else if (theta >= 270)
        {
            neckYawAngle = 450 - theta;
        }
        else if (theta > 90 && theta <= 180)
        {
            neckYawAngle = 0;
        }
        else if (theta < 270 && theta > 180)
        {
            neckYawAngle = 180;
        }

        if (phi >= 270)
        {
            neckPitchAngle = 450 - phi;
        }
        else if (phi <= 90)
        {
            neckPitchAngle = 90 - phi;
        }
        else
        {
            Debug.LogWarning("Pitch Angle Out Of Bounds");
        }

        //Debug.Log($"{neckYawAngle},{neckPitchAngle}");

    }
}
