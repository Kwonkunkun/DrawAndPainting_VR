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
        ht.Add("time",15.0f);
        ht.Add("path",iTweenPath.GetPath("GameOpen"));
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

    void Rotate()   // 게임 시작시 iTween이 끝나고 90도 맟추기
    {
	    iTween.RotateTo(gameObject, new Vector3(0, 92, 0), 6.0f);
    }
    void StartGame(){ // 게임 시작 버튼을 누를시에 실행

        Hashtable ht = new Hashtable();
        ht.Add("time",15.0f);
        ht.Add("path",iTweenPath.GetPath("GameStart"));
        ht.Add("easetype",iTween.EaseType.linear);
        ht.Add("orienttopath",true);
        ht.Add("oncomplete", "Rotate");
       // ht.Add("oncompletetarget", gameObject);

        iTween.MoveTo(this.gameObject,ht);
    }
}
