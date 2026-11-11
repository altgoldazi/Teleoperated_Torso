using UnityEngine;

public class Webcam : MonoBehaviour
{

    [SerializeField] private Material webcamMaterial;
    [SerializeField] private string webcamName = "USB Camera";

    private WebCamTexture webcamTexture;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        WebCamDevice[] devices = WebCamTexture.devices;

        // Log out of each webcam
        Debug.Log("Available webcams:");
        foreach (WebCamDevice webcam in devices)
        {
            Debug.Log(webcam.name);
        }

        // We get the webcam name
        // Can call new() to get the default
        // REGULAR WEBCAM------------------------------------------------
        // WebCamTexture webcamTexture = new(webcamName, 1920, 1080, 30);
        //---------------------------------------------------------------

        //GXIVISION
        webcamTexture = new WebCamTexture(webcamName, 3840, 1080, 30);

        // Start the webcam
        Debug.Log("Starting Webcam");
        webcamTexture.Play();
    }

    void Update()
    {
        //push the material through the shader
        if (webcamTexture != null && webcamTexture.isPlaying)
        {
            webcamMaterial.SetTexture("_webcamTexture", webcamTexture);
        }
    }
    void OnDestroy()
    {
        // Clean up the camera stream when quitting or changing scenes
        if (webcamTexture != null)
        {
            webcamTexture.Stop();
        }
    }

}
