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
    START, COMM, DATA1, DATA2, ESCAPE
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
    ExpectedNextRead expectedNextRead = ExpectedNextRead.START;


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

    // Update is called once per frame
    void Update()
    {
        ReadSerial();
    }

    void ReadSerial()
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

    void ReadByte(int newByte)
    {
        if (newByte == 255)
        {
            // Indicates new data frame
            expectedNextRead = ExpectedNextRead.COMM;
        }
        else
        {
            switch (expectedNextRead)
            {
                case ExpectedNextRead.START:
                    break;
                case ExpectedNextRead.COMM:
                    
                    expectedNextRead++;
                    break;
                case ExpectedNextRead.DATA1:
                    
                    expectedNextRead++;
                    break;
                case ExpectedNextRead.DATA2:

                    expectedNextRead++;
                    break;
                case ExpectedNextRead.ESCAPE:
                    
                    expectedNextRead = ExpectedNextRead.START;
                    break;
                default:
                    break;
            }
        }
    }
}
