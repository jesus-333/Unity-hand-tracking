using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;

public class rotation_2_point : MonoBehaviour
{
    public GameObject point_1;
    public GameObject point_2;

    public GameObject object_to_rotate;

    public double max_distance = 10;
    private double x1, y1, x2, y2;
    
    private Vector3 old_coord_1, old_coord_2;
    private Quaternion original_rotation;

    public bool debug_var;

    // Start is called before the first frame update
    void Start()
    {
        old_coord_1 = new Vector3(9999, 9999, 9999);
        old_coord_2 = new Vector3(9999, 9999, 9999);

        original_rotation = object_to_rotate.transform.rotation;
    }

    // Update is called once per frame
    void Update()
    {
        //print(Globals_scene_2.rotate);

        if (Globals_scene_2.rotate)
        {
            // Take point coordinate
            Vector3 coord_1 = point_1.transform.position;
            Vector3 coord_2 = point_2.transform.position;

            //print("CHECK DISTANCE 1: " + checkIfPointDistanceIsOk(coord_1, old_coord_1, max_distance));
            
            if(checkIfPointDistanceIsOk(coord_1, old_coord_1, max_distance) && checkIfPointDistanceIsOk(coord_2, old_coord_2, max_distance))
            {
                // Reset to original rotation
                object_to_rotate.transform.rotation = original_rotation;

                // Retrieve x and y for both point
                x1 = coord_1[0];
                y1 = coord_1[1];
                x2 = coord_2[0];
                y2 = coord_2[1];

                // Evaluate angle (in radiants)
                double angle = Math.Atan2((y2 - y1), (x2 - x1));

                // Convert angle to degree
                angle = (angle / Math.PI) * 180;

                // Modify angle
                //Vector3 old_rotation = object_to_rotate.transform.rotation;
                //Vector3 new_rotation = Vector3(old_rotation[0], old_rotation[1], angle);
                object_to_rotate.transform.Rotate(0.0f, 0.0f, (float)angle, Space.Self);

                if (debug_var)
                {
                    print("x1: " + x1 + "   y1: " + y1 + "   x2: " + x2 + "   y2: " + y2);
                    print(angle);
                }
            }
        }
        else
        {
            object_to_rotate.transform.rotation = original_rotation;
        }
    }

    /* Check if the distance between two point is major of the min distance.
     * If minor return false. Otherwise return true.
     */
    bool checkIfPointDistanceIsOk(Vector3 p1, Vector3 p2, double min_distance)
    {
        if(min_distance <= 0) { min_distance = 10;  }

        double distance = Vector3.Distance(p1, p2);

        //if (debug_var) { print("distance = " + distance); }

        if (distance <= min_distance) { return false; }
        else { return true; }
    }
}
