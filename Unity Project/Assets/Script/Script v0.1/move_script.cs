using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class move_script : MonoBehaviour
{   

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.RightArrow))
        {
            Vector3 temp = new Vector3(2.0f, 0, 0);
            this.transform.position += temp;
        }

        if (Input.GetKey(KeyCode.LeftArrow))
        {
            Vector3 temp = new Vector3(2.0f, 0, 0);
            this.transform.position -= temp;
        }

        if (Input.GetKey(KeyCode.UpArrow))
        {
            Vector3 temp = new Vector3(0, 2.0f, 0);
            this.transform.position += temp;
        }

        if (Input.GetKey(KeyCode.DownArrow))
        {
            Vector3 temp = new Vector3(0, 2.0f, 0);
            this.transform.position -= temp;
        }
    }
}
