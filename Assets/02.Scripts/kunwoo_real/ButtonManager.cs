using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonManager : MonoBehaviour
{

    public GameObject canvasClearBtn;

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public void canvasClear(){
        
        StartCoroutine(canvasClearCo());

    }

    IEnumerator canvasClearCo(){    // 그림판 지우기 버튼 이벤트
        
        canvasClearBtn.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        canvasClearBtn.SetActive(false);

    }
}
