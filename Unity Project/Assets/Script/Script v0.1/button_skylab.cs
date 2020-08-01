using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class button_skylab : MonoBehaviour
{
    public bool debug_var = false;

    public GameObject finger_indicator;

    // Start is called before the first frame update
    void Start()
    {

    }

    void OnMouseDown()
    {
        // Set the static variable relatives to engine to true
        Globals.action = true;
        Globals.select_skylab = true;
        if (debug_var) { print("Globals.select_skylab: " + Globals.select_skylab); }

        // For safety set the other static variables to false
        Globals.select_payload = false;
        Globals.select_engine = false;
        Globals.select_LEM = false;
    }

    // Update is called once per frame
    void Update()
    {
        Collider2D button_collider = this.GetComponent<Collider2D>();
        Collider2D finger_indicator_collider = finger_indicator.GetComponent<Collider2D>();

        if (button_collider.IsTouching(finger_indicator_collider))
        {
            // Set the static variable relatives to skylab to true
            Globals.select_skylab = true;
            if (debug_var) { print("Globals.select_skylab: " + Globals.select_skylab); }

            // For safety set the other static variables to false
            Globals.select_payload = false;
            Globals.select_engine = false;
            Globals.select_LEM = false;
            
        }
        else { Globals.select_skylab = false; }
    }
}
