using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

public class action_executer : MonoBehaviour
{
    public bool debug_var = false;
    public GameObject action_selector_object;
    public GameObject AR_camera;

    private swapObject sw_obj;

    private int action_type;
    private string command = "";

    private float x_coord;
    private float y_coord;

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

                if (command == "ACTION") 
                {
                    Globals.action = true;
                    if (debug_var) { print("Globals.action (action executer): " + Globals.action);  }
                } 
                else
                {
                    String[] spearator = new String[1];
                    spearator[0] = " ";
                    String[] commnad_vet = command.Split(spearator, 3, StringSplitOptions.None);

                    if (commnad_vet[0] == "l")
                    {
                        x_coord = float.Parse(commnad_vet[1]);
                        y_coord = float.Parse(commnad_vet[2]);
                        //if (debug_var) { print(" Highest Point " + x_coord + " " + y_coord); }
                    }
                    else { x_coord = y_coord = 0.0f; }
                }

                

                //if (debug_var) { print(">> " + command); }
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
        // Move pointer (Finger indicator)

        GameObject Test_button = GameObject.Find("Test_button");
        test_camera_image_V2 test_camera_image_V2_script = Test_button.GetComponent<test_camera_image_V2>();

        Vector3 coord = test_camera_image_V2_script.cameraToWorldCoord(x_coord + 30, y_coord - 40);

        GameObject finger_indicator = GameObject.Find("Finger Indicator");
        finger_indicator.transform.position = coord;

        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
        // Detect action

        if (Globals.action)
        {
            sw_obj = GameObject.Find("swap_object_support").GetComponent<swapObject>();

            if (Globals.select_engine && Globals.inside_engine == false)
            {
                // Select the object to swap (Saturn V and F1 Engine)
                GameObject Sat_V = GameObject.Find("Sat_V2");
                GameObject F1_engine = GameObject.Find("Engine");

                // Swap object
                sw_obj.swap(Sat_V, F1_engine);

                Globals.inside_engine = true;
                removeButton();
            } 
            else if (Globals.select_payload && Globals.inside_payload == false)
            {
                Globals.inside_payload = true;
                GameObject.Find("Back_button").transform.position = new Vector3(-718, 345, 1977.315f);

                sw_obj.swap(GameObject.Find("Payload_button"), GameObject.Find("LEM_button"));
                sw_obj.swap(GameObject.Find("Engine_button"), GameObject.Find("Skylab_button"));
            } 
            else if (Globals.select_LEM && Globals.inside_payload)
            {
                // Select the object to swap (Saturn V and LEM)
                GameObject Sat_V = GameObject.Find("Sat_V2");
                GameObject LEM = GameObject.Find("LEM");

                // Swap object
                sw_obj.swap(Sat_V, LEM);

                Globals.inside_LEM = true;
                removeButton();
            } 
            else if (Globals.select_skylab && Globals.inside_payload)
            {
                // Select the object to swap (Saturn V and Skylab)
                GameObject Sat_V = GameObject.Find("Sat_V2");
                GameObject Skylab = GameObject.Find("Skylab");

                // Swap object
                sw_obj.swap(Sat_V, Skylab);

                Globals.inside_skylab = true;
                removeButton();
            }

            Globals.action = false;
        }
    }

    void removeButton()
    {
        GameObject.Find("Back_button").transform.position = new Vector3(-718, 345, 1977.315f);
        GameObject.Find("Payload_button").transform.position = new Vector3(9000, 9000, 9000);
        GameObject.Find("Engine_button").transform.position = new Vector3(9000, 9000, 9000);
        GameObject.Find("LEM_button").transform.position = new Vector3(9000, 9000, 9000);
        GameObject.Find("Skylab_button").transform.position = new Vector3(9000, 9000, 9000);

    }

    bool stabilizeIndicator(Vector3 new_coord, Vector3 old_coord)
    {
        float tollerance = 0.05f;

        return true;
    }
}

public static class Globals
{
    public static bool select_engine = false;
    public static bool select_payload = false;
    public static bool select_LEM = false;
    public static bool select_skylab = false;

    public static bool inside_engine = false;
    public static bool inside_payload = false;
    public static bool inside_LEM = false;
    public static bool inside_skylab = false;

    public static bool action = false;

    // Original position of the button
    // public static Vector3 back_button_position = new Vector3(-718, 345, 1977.315f); // Old position
    public static Vector3 back_button_position = new Vector3(-12036, 277, 1977.315f);
    public static Vector3 payload_button_position = new Vector3(-712, 125, 1977.315f);
    public static Vector3 engine_button_position = new Vector3(-712, -86, 1977.315f);
    public static Vector3 LEM_button_position = new Vector3(-11983, -6, 1977.315f);
    public static Vector3 skylab_button_position = new Vector3(-11973, -234, 1977.315f);

}

