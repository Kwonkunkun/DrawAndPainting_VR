using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestLook : MonoBehaviour
{

    //public Transform transform;
    public GameObject target;
    // Start is called before the first frame update
    void Start()
    {
        //transform = gameObject.GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        transform.rotation = Quaternion.LookRotation(transform.position - target.transform.position);
    }
}
