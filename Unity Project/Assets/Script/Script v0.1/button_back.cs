using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class button_back : MonoBehaviour
{
    public GameObject finger_indicator;

    private GameObject Sat_V2;
    private GameObject other_obj;
    private swapObject sw_obj;

    // Start is called before the first frame update
    void Start()
    {
        Sat_V2 = GameObject.Find("Sat_V2");
        sw_obj = GameObject.Find("swap_object_support").GetComponent<swapObject>();
    }

    void OnMouseDown()
    {
        Globals.action = true;
        if (Globals.action)
        {
            if (Globals.inside_engine) // If I am inside the engine case
            {
                // Find and select the other object to swap (Engine)
                other_obj = GameObject.Find("Engine");

                // Swap object
                sw_obj.swap(Sat_V2, other_obj);
            }
            else if (Globals.inside_payload) // If I am inside the payload case
            {
                if (Globals.inside_LEM) // If I am inside the LEM case
                {
                    other_obj = GameObject.Find("LEM");
                    sw_obj.swap(Sat_V2, other_obj);
                }
                else if (Globals.inside_skylab) // If I am inside the skylab case
                {
                    other_obj = GameObject.Find("Skylab");
                    sw_obj.swap(Sat_V2, other_obj);
                }
                else // If I am in the payload menu section
                {
                    // Don't swap anything
                }
            }

            Globals.action = false;
            resetStaticVariables();
            resetButtonPosition();
        }
    }

    // Update is called once per frame
    void Update()
    {
        Collider2D button_collider = this.GetComponent<Collider2D>();
        Collider2D finger_indicator_collider = finger_indicator.GetComponent<Collider2D>();
        
        if (button_collider.IsTouching(finger_indicator_collider))
        {
            Globals.action = true;

            if (Globals.action)
            {
                //print("global.action = " + Globals.action);
                //print("global.inside_engine = " + Globals.inside_engine);
                //print("global.inside_payload = " + Globals.inside_payload);

                if (Globals.inside_engine) // If I am inside the engine case
                {
                    // Find and select the other object to swap (Engine)
                    other_obj = GameObject.Find("Engine");

                    // Swap object
                    sw_obj.swap(Sat_V2, other_obj);
                }
                else if (Globals.inside_payload) // If I am inside the payload case
                {
                    if (Globals.inside_LEM) // If I am inside the LEM case
                    { 
                        other_obj = GameObject.Find("LEM");
                        sw_obj.swap(Sat_V2, other_obj);
                    }
                    else if (Globals.inside_skylab) // If I am inside the skylab case
                    { 
                        other_obj = GameObject.Find("Skylab"); 
                        sw_obj.swap(Sat_V2, other_obj);
                    } 
                    else // If I am in the payload menu section
                    {
                        // Don't swap anything
                    }
                }

                Globals.action = false;
                resetStaticVariables();
                resetButtonPosition();
            }
        }
    }

    void resetStaticVariables()
    {
        Globals.select_engine = false;
        Globals.select_payload = false;
        Globals.select_LEM = false;
        Globals.select_skylab = false;

        Globals.inside_engine = false;
        Globals.inside_payload = false;
        Globals.inside_LEM = false;
        Globals.inside_skylab = false;
    }

    void resetButtonPosition()
    {
        print("RESET POSITION");

        GameObject.Find("Back_button").transform.position = Globals.back_button_position;
        GameObject.Find("Payload_button").transform.position = Globals.payload_button_position;
        GameObject.Find("Engine_button").transform.position = Globals.engine_button_position;
        GameObject.Find("LEM_button").transform.position = Globals.LEM_button_position;
        GameObject.Find("Skylab_button").transform.position = Globals.skylab_button_position;
    }
}
