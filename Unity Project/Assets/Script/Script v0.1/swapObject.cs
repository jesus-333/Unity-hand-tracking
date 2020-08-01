using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class swapObject : MonoBehaviour
{
    private Vector3 swap_vector = new Vector3(0, 0, 0);

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void swap(GameObject actual_object, GameObject target_object) {
        swap_vector = newVector3(actual_object.transform.position);

        actual_object.transform.position = newVector3(target_object.transform.position);

        target_object.transform.position = newVector3(swap_vector);
    }

    Vector3 newVector3(Vector3 old_vec) {
        return new Vector3(old_vec[0], old_vec[1], old_vec[2]);
    }
}
