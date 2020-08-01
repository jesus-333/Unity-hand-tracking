using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class camera_coord_converter : MonoBehaviour
{
    public bool debug_var = false;
    public GameObject AR_camera_object;
    public GameObject finger_indicator;

    private int i = 0;

    private float v_fov = 0;
    private float h_fov = 0;
    private float z_background = 0;
    private float max_x = 0;
    private float max_y = 0;
    private float camera_width = 0;
    private float camera_height = 0;
    
    public float x_tick = 0;
    public float y_tick = 0;


    // Start is called before the first frame update
    void Start()
    {


    }

    // Update is called once per frame
    void Update()
    {
        if (i == 5)
        {
            GameObject BackgroundPlane = GameObject.Find("BackgroundPlane");
            //Camera AR_camera = GameObject.Find("ARCamera").GetComponent<Camera>();
            Camera AR_camera = AR_camera_object.GetComponent<Camera>();

            z_background = BackgroundPlane.GetComponent<Transform>().position[2];
            print(BackgroundPlane.GetComponent<Transform>().position);

            v_fov = AR_camera.fieldOfView;
            h_fov = Camera.VerticalToHorizontalFieldOfView(v_fov, AR_camera.aspect);

            max_x = coordBackgroundEvaluation(z_background, h_fov/2.0f);
            max_y = coordBackgroundEvaluation(z_background, v_fov/2.0f);

            camera_width = AR_camera_object.GetComponent<Camera>().pixelWidth;
            camera_height = AR_camera_object.GetComponent<Camera>().pixelHeight;

            x_tick = (2.0f * max_x) / camera_width;
            y_tick = (2.0f * max_y) / camera_height;
        }

        //if(i <= 5)
        //{
        //    finger_indicator.transform.position = cameraToWorldCoord(0, 0);
        //}

        if (debug_var && i == 5)
        {
            print("z_background = " + z_background);
            print("v_fov = " + v_fov);
            print("h_fov = " + h_fov);
            print("max_x = " + max_x);
            print("max_y = " + max_y);
            print("pixelWidth = " + camera_width);
            print("pixelHeight = " + camera_height);
        }

        i++;
    }

    float coordBackgroundEvaluation(float z_background, float fov) {
        float coord = 0f;

        coord = z_background * Mathf.Tan(degreeToRadiant(fov));

        return coord;
    }

    float degreeToRadiant(float angle)
    {
        return Mathf.PI * (angle / 180f);
    }

    public Vector3 cameraToWorldCoord(float x_camera_coord, float y_camera_coord)
    {
        float x_world_coord = (-max_x) + x_camera_coord * x_tick;
        float y_world_coord = max_y - y_camera_coord * y_tick;

        Vector3 world_coord = new Vector3(x_world_coord, y_world_coord, z_background);

        return world_coord;
    }
}
