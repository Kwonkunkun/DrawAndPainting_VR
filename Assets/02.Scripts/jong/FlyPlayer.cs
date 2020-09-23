using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyPlayer : MonoBehaviour
{
    public GameObject target;

    // Start is called before the first frame update
    void Start()
    {
        Hashtable ht = new Hashtable();
        ht.Add("time",25.0f);
        ht.Add("path",iTweenPath.GetPath("Fly"));
        ht.Add("easetype",iTween.EaseType.linear);
        ht.Add("orienttopath",true);
        ht.Add("oncomplete", "Rotate");
       // ht.Add("oncompletetarget", gameObject);

        iTween.MoveTo(this.gameObject,ht);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void Rotate()
    {

	    iTween.RotateTo(gameObject, new Vector3(0, 92, 0), 6.0f);
    }
}
