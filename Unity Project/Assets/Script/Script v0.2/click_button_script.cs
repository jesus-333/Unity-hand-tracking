using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class click_button_script : MonoBehaviour
{
    public GameObject button;

    public bool debug_var = false;

    void Start()
    {

    }

    void OnMouseDown()
    {
        string s = button.name.ToLower();

        if (debug_var) { print(s); }

        if (s == "rotation button") {
            Globals_scene_2.rotate = !Globals_scene_2.rotate;
            if (debug_var) { print("Globals_scene_2.rotate = " + Globals_scene_2.rotate); }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
