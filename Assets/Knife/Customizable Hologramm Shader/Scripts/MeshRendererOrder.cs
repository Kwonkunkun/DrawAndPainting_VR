using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class MeshRendererOrder : MonoBehaviour
{
    public int Order;

    void OnEnable()
    {
        GetComponent<Renderer>().sortingOrder = Order;
    }

    void OnValidate()
    {
        GetComponent<Renderer>().sortingOrder = Order;
    }
}
