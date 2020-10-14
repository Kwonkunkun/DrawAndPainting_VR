using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectRotator : MonoBehaviour
{
    [SerializeField] private Vector3 rotationPerSecond;

    private void Update()
    {
        transform.Rotate(rotationPerSecond * Time.deltaTime);
    }
}
