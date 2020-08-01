using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;

using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

public class command_receiver : MonoBehaviour
{
    public bool debug_var = false;

    private string command = "";

    private float x_coord_1;
    private float y_coord_1;
    private float x_coord_2;
    private float y_coord_2;

    // UDP Variables
    Thread receiveThread;
    UdpClient client;
    int port;

    // Start is called before the first frame update
    void Start()
    {
        port = 5065;
        InitUDP();
    }

    private void InitUDP()
    {
        print("UDP Initialized");

        receiveThread = new Thread(new ThreadStart(ReceiveData));
        receiveThread.IsBackground = true;
        receiveThread.Start();

    }

    // 4. Receive Data
    private void ReceiveData()
    {
        client = new UdpClient(port);
        while (true)
        {
            try
            {
                IPEndPoint anyIP = new IPEndPoint(IPAddress.Parse("0.0.0.0"), port);
                byte[] data = client.Receive(ref anyIP);

                command = Encoding.UTF8.GetString(data);

                if(command[0] == 'f')
                {

                } 
                else if (command[0] == 'h')
                {
                    String[] spearator = new String[1];
                    spearator[0] = " ";
                    String[] command_vet = command.Split(spearator, 4, StringSplitOptions.None);

                    if(command_vet[1] == "1")
                    {
                        x_coord_1 = float.Parse(command_vet[2]);
                        y_coord_1 = float.Parse(command_vet[3]);
                        if (debug_var) { print(" Position hand 1: " + x_coord_1 + " " + y_coord_1); }
                    } 
                    else if (command_vet[1] == "2")
                    {
                        x_coord_2 = float.Parse(command_vet[2]);
                        y_coord_2 = float.Parse(command_vet[3]);
                        if (debug_var) { print(" Position hand 2: " + x_coord_2 + " " + y_coord_2); }
                    }
                    else { x_coord_1 = y_coord_1 = x_coord_2 = y_coord_2 = 0.0f; }
                }

                if (debug_var) { print(">> " + command); }
            }
            catch (Exception e)
            {
                print(e.ToString()); 
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        GameObject controller = GameObject.Find("Controller");
        camera_coord_converter camera_coord_converter_script = controller.GetComponent<camera_coord_converter>();

        GameObject indicator_1 = GameObject.Find("Indicator_1");
        Vector3 coord = camera_coord_converter_script.cameraToWorldCoord(x_coord_1 + 25, y_coord_1 + 10);
        indicator_1.transform.position = coord;

        GameObject Indicator_2 = GameObject.Find("Indicator_2");
        coord = camera_coord_converter_script.cameraToWorldCoord(x_coord_2 + 60, y_coord_2 - 20);
        Indicator_2.transform.position = coord;
    }
}


public static class Globals_scene_2
{
    public static bool rotate = false;
}
