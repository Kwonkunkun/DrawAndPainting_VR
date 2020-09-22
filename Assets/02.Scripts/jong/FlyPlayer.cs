using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyPlayer : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Hashtable ht = new Hashtable();
        ht.Add("time",10.0f);
        ht.Add("path",iTweenPath.GetPath("Fly"));
        ht.Add("easetype",iTween.EaseType.linear);

        iTween.MoveTo(this.gameObject,ht);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
