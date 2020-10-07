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

    IEnumerator canvasClearCo(){
        
        canvasClearBtn.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        canvasClearBtn.SetActive(false);

    }
}
