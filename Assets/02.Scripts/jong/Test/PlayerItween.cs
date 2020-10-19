using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerItween : MonoBehaviour
{
    //public GameObject target;
    
    public GameObject menu;

    public GameObject LeftDoor;
    public GameObject RightDoor;

    // Start is called before the first frame update
    void Start()
    {
        Hashtable ht = new Hashtable();
        ht.Add("time",15.0f);
        ht.Add("path",iTweenPath.GetPath("GameOpen"));
        ht.Add("easetype",iTween.EaseType.linear);
        ht.Add("orienttopath",true);
        ht.Add("oncomplete", "Rotate_01");
       // ht.Add("oncompletetarget", gameObject);

        iTween.MoveTo(this.gameObject,ht);
    }

    void Rotate_01()   // 시작메뉴에서  iTween이 끝나고 92도 맟추기
    {
	    iTween.RotateTo(gameObject, new Vector3(0, 92, 0), 6.0f);
    }
    public void StartGame(){ // 게임시작 버튼을 누를 시에 실행

        menu.SetActive(false);

        Hashtable ht = new Hashtable();
        ht.Add("time",15.0f);
        ht.Add("path",iTweenPath.GetPath("GameStart"));
        ht.Add("easetype",iTween.EaseType.linear);
        ht.Add("orienttopath",true);
        ht.Add("oncomplete", "Rotate_02");
       // ht.Add("oncompletetarget", gameObject);

        iTween.MoveTo(this.gameObject,ht);
    }
    void Rotate_02()   // 시작게임에서 그림판 앞으로  iTween이 끝나고 270도 맟추기
    {

        LeftDoor.GetComponent<Animator>().SetBool("Open",false);
        RightDoor.GetComponent<Animator>().SetBool("Open",false);

	    iTween.RotateTo(gameObject, new Vector3(0, 270, 0), 6.0f);
    }

    public void ReturnMenu(){ // 메뉴로 돌아가기 버튼을 누를 시에 실행

        Hashtable ht = new Hashtable();
        ht.Add("time",15.0f);
        ht.Add("path",iTweenPath.GetPath("ReturnMenu"));
        ht.Add("easetype",iTween.EaseType.linear);
        ht.Add("orienttopath",true);
        ht.Add("oncomplete", "Rotate_03");
       // ht.Add("oncompletetarget", gameObject);

        iTween.MoveTo(this.gameObject,ht);
    }
    void Rotate_03()   // 시작게임에서 그림판 앞으로  iTween이 끝나고 270도 맟추기
    {
	    iTween.RotateTo(gameObject, new Vector3(0, 92, 0), 6.0f); 

        menu.SetActive(true);
        
        LeftDoor.GetComponent<Animator>().SetBool("Open",false);
        RightDoor.GetComponent<Animator>().SetBool("Open",false);
    }
}
