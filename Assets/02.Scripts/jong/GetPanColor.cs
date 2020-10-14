using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PaintIn3D;

public class GetPanColor : MonoBehaviour
{
    P3dPaintSphere penColor;
    
    // Start is called before the first frame update
    void Start()
    {
        penColor = gameObject.GetComponent<P3dPaintSphere>();   
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter(Collider other){
        if(other.transform.gameObject.tag=="Color"){
            transform.GetComponentInParent<MeshRenderer>().material.color = other.transform.GetComponent<MeshRenderer>().material.color;
            
            penColor.Color =  other.transform.GetComponent<MeshRenderer>().material.color;
        }
    }
    

    
}
