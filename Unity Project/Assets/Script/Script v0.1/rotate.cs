using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rotate : MonoBehaviour
{
    public bool continue_rotation = false;
    public float rotation_speed = 1f;
    public float change_speed_factor = 1.02f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // Active/Deactive continue rotation
        if (Input.GetKey(KeyCode.C)) {
            continue_rotation = !continue_rotation;
        }

        // Increase/Decrease rotation speed
        if (Input.GetKey(KeyCode.Z)) { rotation_speed = rotation_speed * change_speed_factor; }
        if (Input.GetKey(KeyCode.X)) { rotation_speed = rotation_speed / change_speed_factor; }

        if(rotation_speed == 0) { rotation_speed = 1; }

        if (continue_rotation)
        {
            this.transform.Rotate(0.0f, rotation_speed, 0.0f, Space.Self);
        } else
        {
            if (Input.GetKey(KeyCode.RightArrow))
            {
                this.transform.Rotate(0.0f, rotation_speed, 0.0f, Space.Self);
            }

            if (Input.GetKey(KeyCode.LeftArrow))
            {
                this.transform.Rotate(0.0f, -rotation_speed, 0.0f, Space.Self);
            }
        } 
    }
}
