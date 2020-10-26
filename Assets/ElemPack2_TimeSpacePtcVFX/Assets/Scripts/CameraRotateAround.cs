using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRotateAround : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}

    // Update is called once per frame

    public float RotSpeed = 0.5f;
	void FixedUpdate () {

            this.transform.Rotate(0, RotSpeed, 0);

	}
}
