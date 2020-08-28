using System.Collections;
using System.Collections.Generic;
using TreeEditor;
using UnityEngine;

public class EraseDraw : MonoBehaviour
{
    Transform tr;
    private void Start()
    {
        tr = GetComponent<Transform>();    
    }
    public void EraseAll()
    {
        for(int i = tr.childCount -1; i >= 0; i--)
        {
            Destroy(tr.GetChild(i).gameObject);
        }
    }
}
