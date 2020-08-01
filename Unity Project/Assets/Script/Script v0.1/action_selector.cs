using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class action_selector : MonoBehaviour
{
    public bool debug_var = false;

    public int current_action = 0;

    // (OLD IDEA) - Pressing space I change the modality of interation. Now use only gesture command
    // current_action = 0 ----> Change Scenario
    // current_action = 1 ----> Change Rotation speed
    // current_action = 2 ----> Selector
    // current_action = 3 ----> Nothing (Idle)



    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            current_action += 1;

            if (debug_var) { print("CHANGE ACTION - Current action: " + current_action); }

            if(current_action > 3) { current_action = 0; }
        }
    }
}