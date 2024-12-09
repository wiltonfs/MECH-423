using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO.Ports;
using System.IO;
using System.Collections.Concurrent;
using System.Linq;
using System.Drawing;
using UnityEngine.SocialPlatforms.Impl;
public enum ExpectedNextRead
{
    START, COMM, DATA1, DATA2, ESCAPE
}

public enum CommBytes
{
    DEBUG_0 = 0, ROT_CW = 8, ROT_CCW = 9, SLIDERS = 10, BIN_INS = 11, LAUNCH = 12
}

public struct MessagePacket
{
    public CommBytes comm;
    public byte d1;
    public byte d2;

    public MessagePacket(CommBytes c)
    {
        comm = c;
        d1 = 0; d2 = 0;
    }

    public override string ToString()
    {
        return $"[{(byte)comm}, {d1}, {d2}] = [{comm}, {this.Combined()}]";
    }

    public void ApplyEscapeByte(byte esc)
    {
        if ((esc & 1) > 0)
            throw new System.NotImplementedException();
        if ((esc & 2) > 0)
            d1 = 255;
        if ((esc & 4) > 0)
            d2 = 255;
    }

    public uint Combined()
    {
        return (uint)(d1 << 8) | d2;
    }

    public void Split(uint combined)
    {
        d1 = (byte) (combined >> 8);
        d2 = (byte) (combined & 0xff);
    }
}

public class SerialScanner : MonoBehaviour
{
    [Header("Player Details")]
    public string PlayerName = "Lazar";


    [Header("Serial Parameters")]
    public int baudRate = 9600;
    public string portName = "COM6";
    public MessagePacket MostRecentRxPacket = new MessagePacket(CommBytes.DEBUG_0);

    [Header("Serial Status")]
    [SerializeField] private bool printMessages = false;
    [SerializeField] private bool openPort = false;
    [SerializeField] private uint readAttempts = 0;
    [SerializeField] private uint bytesRead = 0;

    [Header("Module 1")]
    public bool FineAdjustmentSwitch = false;
    public long CummulativeEncoderCounts = 0;
    public bool BatterySelect1 = false;
    public bool BatterySelect2 = false;
    public bool BatterySelect3 = false;
    public bool CruiseMissileSwitch = false;
    public uint RxdLaunchCommands = 0;

    [Header("Module 2")]
    public bool Module2Enabled = false;
    public byte Slider1 = 0;
    public byte Slider2 = 0;
    public bool ThermalCorrectionByte0 = false;
    public bool ThermalCorrectionByte1 = false;

    [Header("Module 3")]
    public bool Module3Enabled = false;
    public byte StateMachine = 0;

    [Header("High Scores")]
    public uint HighScore_Arcade = 100;
    public uint HighScore_Challenge = 250;
    public float BestTime_Challenge = Mathf.Infinity;


    // Serial reading
    SerialPort data_stream;
    ExpectedNextRead expectedNextRead = ExpectedNextRead.START;

    void Awake()
    {
        // Ensures the object persists across scenes
        DontDestroyOnLoad(gameObject); 
    }

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
        if (printMessages)
            Debug.Log("Starting serial scanner");
    }

    // Update is called once per frame
    void Update()
    {
        ReadSerial();
    }

    public void HorizontalLine()
    {
        if (data_stream.IsOpen)
        {
            data_stream.WriteLine("--------------------------------");
        }
    }

    public string ReadName()
    {
        string filePath = "C:/Users/fsant/Desktop/PlayerName.txt";

        if (File.Exists(filePath))
        {
            string content = File.ReadAllText(filePath);
            return content;
        }
        else
        {
            return PlayerName;
        }

    }

    public void TransmitScoreToThermalPrinter(int score)
    {
        if (data_stream.IsOpen)
        {
            HorizontalLine();
            data_stream.WriteLine($"Arcade Gamemode");
            data_stream.WriteLine($"Operator: {ReadName()}");
            data_stream.WriteLine($"Final Score: {score}");
            data_stream.WriteLine("Thanks for playing!");
            HorizontalLine();
            ThermalPrinter_FinishParagraph();
        }
    }

    public void StartChallengeReceipt()
    {
        if (data_stream.IsOpen)
        {
            HorizontalLine();
            data_stream.WriteLine($"Challenge Gamemode");
            data_stream.WriteLine($"Operator: {ReadName()}");
            ThermalPrinter_FinishParagraph(2);
        }
    }

    public void FinishChallengeReceipt()
    {
        if (data_stream.IsOpen)
        {
            data_stream.WriteLine($"Operator: {ReadName()}");
            data_stream.WriteLine("Thanks for playing!");
            HorizontalLine();
            ThermalPrinter_FinishParagraph();
        }
    }

    public void ThermalPrinter_FinishParagraph(int spacerLines = 3)
    {
        if (data_stream.IsOpen)
        {
            for (int i = 0; i < spacerLines; i++) { data_stream.WriteLine(""); }
        }
    }

    public void ThermalPrinter_WriteLine(string line)
    {
        Debug.Log("Thermal Printer: " + line);
        if (data_stream.IsOpen)
        {
            data_stream.WriteLine(line);
        }
    }

    public bool IsBoardConnected()
    {
        return bytesRead > 0;
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
                ReadByte((byte)newByte);

                bytesToRead = data_stream.BytesToRead;
            }
        }
        else
        {
            openPort = false;
            data_stream.Open();
        }
    }

    void ReadByte(byte newByte)
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
                    MostRecentRxPacket.comm = (CommBytes)newByte;
                    expectedNextRead++;
                    break;
                case ExpectedNextRead.DATA1:
                    MostRecentRxPacket.d1 = newByte;
                    expectedNextRead++;
                    break;
                case ExpectedNextRead.DATA2:
                    MostRecentRxPacket.d2 = newByte;
                    expectedNextRead++;
                    break;
                case ExpectedNextRead.ESCAPE:
                    MostRecentRxPacket.ApplyEscapeByte(newByte);
                    ProcessCompletePacket();
                    expectedNextRead = ExpectedNextRead.START;
                    break;
                default:
                    break;
            }
        }
    }

    void ProcessCompletePacket()
    {
        if (printMessages)
            Debug.Log($"Rx'd a new Packet: {MostRecentRxPacket.ToString()}");

        switch (MostRecentRxPacket.comm)
        {
            case CommBytes.ROT_CW:
                CummulativeEncoderCounts += MostRecentRxPacket.Combined();
                break;
            case CommBytes.ROT_CCW:
                CummulativeEncoderCounts -= MostRecentRxPacket.Combined();
                break;
            case CommBytes.SLIDERS:
                Slider1 = MostRecentRxPacket.d1;
                Slider2 = MostRecentRxPacket.d2;
                break;
            case CommBytes.BIN_INS:
                // Data Byte 1
                BatterySelect1 = (MostRecentRxPacket.d1 & 1) > 0;
                BatterySelect2 = (MostRecentRxPacket.d1 & 2) > 0;
                BatterySelect3 = (MostRecentRxPacket.d1 & 4) > 0;
                CruiseMissileSwitch = (MostRecentRxPacket.d1 & 8) > 0;
                FineAdjustmentSwitch = (MostRecentRxPacket.d1 & 16) > 0;
                Module2Enabled = (MostRecentRxPacket.d1 & 32) > 0;
                ThermalCorrectionByte0 = (MostRecentRxPacket.d1 & 64) > 0;
                ThermalCorrectionByte1 = (MostRecentRxPacket.d1 & 128) > 0;
                // Data Byte 2
                Module3Enabled = (MostRecentRxPacket.d2 & 1) > 0;
                StateMachine = (byte)((MostRecentRxPacket.d2 >> 1) & 0b111111);
                break;
            case CommBytes.LAUNCH:
                RxdLaunchCommands++;
                MissileLauncher missileLauncher = FindObjectOfType<MissileLauncher>();
                if (missileLauncher)
                {
                    missileLauncher.LaunchMissile();
                }   
                break;
            default:
                break;
        }

    }
}
