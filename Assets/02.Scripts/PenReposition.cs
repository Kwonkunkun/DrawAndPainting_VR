using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PenReposition : MonoBehaviour
{
    public GameObject reposition;


    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("Environment"))
        {
            print("Pen rePosition");
            transform.position = reposition.transform.position;
            transform.rotation = reposition.transform.rotation;
        }
    }
}
