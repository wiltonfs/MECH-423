using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO.Ports;
using System.IO;
using System.Collections.Concurrent;
using System.Linq;
using System.Drawing;
enum ExpectedNextRead
{
    LEAD, X, Y, Z
}

public class SerialScanner : MonoBehaviour
{
    [Header("Serial Parameters")]
    public int baudRate = 9600;
    public string portName = "COM3";

    [Header("Serial Status")]
    [SerializeField] private bool openPort = false;
    [SerializeField] private int readAttempts = 0;
    [SerializeField] private int bytesRead = 0;


    // Serial reading
    SerialPort data_stream;
    ExpectedNextRead expectedNextRead = ExpectedNextRead.LEAD;
    Vector3 bias = new Vector3(-125f, -126f, -128f);
    Vector3 scale = new Vector3(0.3849f, 0.3773f, 0.3924f);

    Vector3 mostRecentAccelBuffer0 = new Vector3();
    Vector3 mostRecentAccelBuffer1 = new Vector3();
    int currentBuffer = -1;


    // Start is called before the first frame update
    void Start()
    {
        data_stream = new SerialPort(portName, baudRate);
        data_stream.DataBits = 8;
	    //data_stream.DiscardNull = false;
        data_stream.Handshake = Handshake.None;
        data_stream.Parity = Parity.None;
	    data_stream.ReadTimeout = -1;
        data_stream.StopBits = StopBits.One;
        data_stream.Open();
        Debug.Log("Starting serial scanner");
    }

    // FixedUpdate is called 50 times per second
    void FixedUpdate()
    {
        ReadSerial();
    }

    private void ReadSerial()
    {
        if (data_stream.IsOpen)
        {
            openPort = true;
            int newByte = 0;
            int bytesToRead;
            bytesToRead = data_stream.BytesToRead;
            readAttempts++;
            while (bytesToRead != 0)
            {
                bytesRead++;

                newByte = data_stream.ReadByte();
                Debug.Log(newByte);
                ReadByte(newByte);

                bytesToRead = data_stream.BytesToRead;
            }
        }
        else
        {
            openPort = false;
            data_stream.Open();
        }
    }

    private void ReadByte(int newByte)
    {
        if (newByte == 255)
        {
            // Indicates new data frame
            expectedNextRead = ExpectedNextRead.X;
        }
        else
        {
            float correctedX = (newByte + bias.x) * scale.x;
            float correctedY = (newByte + bias.y) * scale.y;
            float correctedZ = (newByte + bias.z) * scale.z;
            switch (expectedNextRead)
            {
                case ExpectedNextRead.LEAD:
                    break;
                case ExpectedNextRead.X:
                    WriteToAccelBuffer(0, correctedX);
                    expectedNextRead++;
                    break;
                case ExpectedNextRead.Y:
                    WriteToAccelBuffer(1, correctedY);
                    expectedNextRead++;
                    break;
                case ExpectedNextRead.Z:
                    WriteToAccelBuffer(2, correctedZ);
                    SwapAccelBuffers();
                    expectedNextRead = ExpectedNextRead.LEAD;
                    break;
                default:
                    break;
            }
        }
    }

    private void WriteToAccelBuffer(int coord, float value)
    {
        if (currentBuffer == -1 || currentBuffer == 1)
        {
            // Write to Buffer0
            if (coord == 0)
            {
                // Write X
                mostRecentAccelBuffer0 = new Vector3 (value, mostRecentAccelBuffer0.y, mostRecentAccelBuffer0.z);
            }
            else if (coord == 1)
            {
                // Write Y
                mostRecentAccelBuffer0 = new Vector3(mostRecentAccelBuffer0.x, value, mostRecentAccelBuffer0.z);
            } 
            else
            {
                // Write Z
                mostRecentAccelBuffer0 = new Vector3(mostRecentAccelBuffer0.x, mostRecentAccelBuffer0.y, value);
            }
        }
        else
        {
            // Write to Buffer1
            if (coord == 0)
            {
                // Write X
                mostRecentAccelBuffer1 = new Vector3(value, mostRecentAccelBuffer1.y, mostRecentAccelBuffer1.z);
            }
            else if (coord == 1)
            {
                // Write Y
                mostRecentAccelBuffer1 = new Vector3(mostRecentAccelBuffer1.x, value, mostRecentAccelBuffer1.z);
            }
            else
            {
                // Write Z
                mostRecentAccelBuffer1 = new Vector3(mostRecentAccelBuffer1.x, mostRecentAccelBuffer1.y, value);
            }
        }
    }

    private void SwapAccelBuffers()
    {
        if (currentBuffer == -1)
        {
            currentBuffer = 0;
        }
        else if (currentBuffer == 0)
        {
            currentBuffer = 1;
            mostRecentAccelBuffer0 = new Vector3();
        } else
        {
            currentBuffer = 0;
            mostRecentAccelBuffer1 = new Vector3();
        }
    }

    public Vector3 GetAccelData()
    {
        if (currentBuffer == -1)
        {
            Debug.Log("Tried to read AccelBuffer before any has been filled");
            return new Vector3();
        }
        else if (currentBuffer == 0)
        {
            return mostRecentAccelBuffer0;
        }
        else
        {
            return mostRecentAccelBuffer1;
        }
    }

    // Returns the boards current up vector, with Y up (Unity standard)
    public Vector3 GetUpVectorUnity()
    {
        Vector3 v = GetAccelData();
        return new Vector3(v.x, v.z, v.y);
    }

    public bool HasAccelData() { return  currentBuffer != -1; }
}
