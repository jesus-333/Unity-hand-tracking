using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

public class test_UDP : MonoBehaviour
{

    public bool debug_var = false;

    // 1. Declare Variables
    Thread receiveThread; //1
    UdpClient client; //2
    int port; //3

    // Start is called before the first frame update
    void Start()
    {
        port = 5065; //1 
       
        InitUDP(); //4
    }

    // 3. InitUDP
    private void InitUDP()
    {
        print("UDP Initialized");

        receiveThread = new Thread(new ThreadStart(ReceiveData)); //1 
        receiveThread.IsBackground = true; //2
        receiveThread.Start(); //3

    }

    // 4. Receive Data
    private void ReceiveData()
    {
        client = new UdpClient(port); //1
        while (true) //2
        {
            try
            {
                IPEndPoint anyIP = new IPEndPoint(IPAddress.Parse("0.0.0.0"), port); //3
                byte[] data = client.Receive(ref anyIP); //4

                string text = Encoding.UTF8.GetString(data); //5
                if (debug_var) { print(">> " + text); }

                if(text == "Back")
                {
                    anyIP = new IPEndPoint(IPAddress.Parse("0.0.0.0"), port); //3
                    data = client.Receive(ref anyIP); //4
                    string coordinate = Encoding.UTF8.GetString(data);
                    if (debug_var) { print("coordinate = " + coordinate); }
                }
            }
            catch (Exception e)
            {
                print(e.ToString()); //7
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
