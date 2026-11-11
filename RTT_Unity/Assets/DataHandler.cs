using NUnit.Framework;
using System;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class WifiController : MonoBehaviour
{
    [Header("Network Setup")]
    [Tooltip("Paster the IP printed in the Arduino Serial Monitor here")]
    public string arduinoIP = "---------------";
    public int arduinoPort = 5005;
    public int unityPort = 5006;

    [Header("Network Performance")]
    [Tooltip("How many times per second to send data")]
    public float sendRateHz = 20f;

    [Header("Servo Control Angles")]
    [UnityEngine.Range(0, 180)] public int servo3Angle = 90;

    //--------------------------------------------------------------------------------------------------------------------
    private int neckYawAngle;
    private int neckPitchAngle;
    private int elbowAngle;
    private int shoulderRoll;
    private int shoulderPitch;
    //--------------------------------------------------------------------------------------------------------------------

    private UdpClient _udpClient;
    private IPEndPoint _arduinoEndPoint;
    private Thread _receiveThread;
    private bool _isRunning = false;
    private string _incomingData = "";
    private readonly object _lock = new object();

    //checks if the angles are the same to not send unneccessary data
    //--------------------------------------------------------------------------------------------------------------------
    private int _lastNeckYawAngle = -1;
    private int _lastNeckPitchAngle = -1;
    private int _lastServo3Angle = -1;
    private int _lastElbowAngle = -1;
    private int _lastShoulderRoll = -1;
    private int _lastShoulderPitch = -1;
    //--------------------------------------------------------------------------------------------------------------------

    private float _sendTimer = 0f;


    void Start()
    {
        try
        {
            _arduinoEndPoint = new IPEndPoint(IPAddress.Parse(arduinoIP), arduinoPort);
            _udpClient = new UdpClient(unityPort);
            _isRunning = true;
            _receiveThread = new Thread(ReceivePacketsLoop) { IsBackground = true };
            _receiveThread.Start();

            Debug.Log($"UDP Network bound to port {unityPort}. Target: {arduinoIP}:{arduinoPort}");
        }
        catch (Exception e)
        {
            Debug.LogError($"Network initialization failed: {e.Message}");
        }
    }

    // Update is called once per frame
    void Update()
    {
        /* and update angles
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

        Debug.Log($"{neckYawAngle},{neckPitchAngle}");*/

        //Update values from outside scripts
        //--------------------------------------------------------------------------------------------------------------------
        //neck Angles
        neckYawAngle = GameObject.Find("NeckController").GetComponent<NeckCalc>().neckYawAngle;
        neckPitchAngle = GameObject.Find("NeckController").GetComponent<NeckCalc>().neckPitchAngle;
        //Debug.Log($"{neckYawAngle},{neckPitchAngle}");

        //Right Arm Angles
        elbowAngle = GameObject.Find("RightArmController").GetComponent<RightArmCalc>().elbowAngle;
        shoulderRoll = GameObject.Find("RightArmController").GetComponent<RightArmCalc>().shoulderRoll;
        shoulderPitch = GameObject.Find("RightArmController").GetComponent<RightArmCalc>().shoulderPitch;
        //--------------------------------------------------------------------------------------------------------------------


        HandleOutgoingData();
        HandleIncomingData();
    }

    private void HandleOutgoingData()
    {
        if (_udpClient == null) return;

        _sendTimer += Time.deltaTime;
        float interval = 1f / sendRateHz;

        //check if the angle of servos needs to be changed
        bool dataChanged = (neckYawAngle != _lastNeckYawAngle ||
                            neckPitchAngle != _lastNeckPitchAngle ||
                            servo3Angle != _lastServo3Angle || 
                            elbowAngle != _lastElbowAngle ||
                            shoulderRoll != _lastShoulderRoll ||
                            shoulderPitch != _lastShoulderPitch); 

        if (dataChanged || _sendTimer>= interval)
        {
            _sendTimer = 0f;
            //--------------------------------------------------------------------------------------------------------------------
            _lastNeckYawAngle = neckYawAngle;
            _lastNeckPitchAngle = neckPitchAngle;
            _lastServo3Angle = servo3Angle;
            _lastElbowAngle = elbowAngle;
            _lastShoulderRoll = shoulderRoll;
            _lastShoulderPitch = shoulderPitch;

            string command = $"{neckYawAngle}a{neckPitchAngle}b{servo3Angle}c{elbowAngle}d{shoulderRoll}e{shoulderPitch}";
            //--------------------------------------------------------------------------------------------------------------------

            byte[] bytes = Encoding.UTF8.GetBytes(command);

            try
            {
                _udpClient.Send(bytes, bytes.Length, _arduinoEndPoint);
            }
            catch (Exception e)
            {
                Debug.LogWarning($"Failed to send UDP packet: {e.Message}");
            }
        }
    }

    private void HandleIncomingData()
    {
        string dataToProcess = string.Empty;

        lock (_lock)
        {
            if (!string.IsNullOrEmpty(_incomingData))
            {
                dataToProcess = _incomingData;
                _incomingData = string.Empty;
            }
        }
        if (!string.IsNullOrEmpty(dataToProcess))
        {
            Debug.Log($"Wireless Data from Arduino: {dataToProcess}");
        }
    }

    private void ReceivePacketsLoop()
    {
        IPEndPoint remoteEndPoint = new IPEndPoint(IPAddress.Any, 0);

        while (_isRunning)
        {
            try
            {
                if (_udpClient != null)
                {
                    byte[] receivedBytes = _udpClient.Receive(ref remoteEndPoint);
                    string text = Encoding.UTF8.GetString(receivedBytes);

                    lock (_lock)
                    {
                        _incomingData = text;
                    }
                }
            }
            catch (SocketException se)
            {
                if (!_isRunning) break;
                Debug.LogWarning($"Socket read error: {se.Message}");
            }
            catch (Exception e)
            {
                Debug.LogWarning($"Network Read Error: {e.Message}");
            }
        }
    }

    void OnDisable() { CleanUpNetwork(); }
    void OnDestroy() { CleanUpNetwork(); }

    private void CleanUpNetwork()
    {
        _isRunning = false;

        if (_udpClient != null)
        {
            _udpClient.Close();
            _udpClient = null;
        }

        if (_receiveThread != null)
        {
            if (_receiveThread.IsAlive)
            {
                _receiveThread.Abort();
            }
            _receiveThread = null;
        }
        Debug.Log("Networkj Sockets Safely Closed.");
    }
}

