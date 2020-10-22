using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorTableSize : MonoBehaviour
{
    public void UpSize()
    {
        print("UpSize");
        if (transform.localScale.x >= 1.5f)
            return;
        transform.localScale += new Vector3(0.1f, 0.1f, 0.1f);
    }
    public void OriginalSize()
    {
        print("OriginalSize");
        transform.localScale = new Vector3(1, 1, 1);
    }
    public void DownSize()
    {
        print("DownSize");
        if (transform.localScale.x <= 0.5f)
            return;
        transform.localScale -= new Vector3(0.1f, 0.1f, 0.1f);
    }
}
