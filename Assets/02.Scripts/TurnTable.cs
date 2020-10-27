using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TurnTable : MonoBehaviour
{
    public Transform uiObj;
    public void Right60Rotate()
    {
        GameManager.instance.deleteIdx--;
        if (GameManager.instance.deleteIdx < 0)
            GameManager.instance.deleteIdx = 5;

        GameManager.instance.deleteText.GetComponent<Text>().text = "지워지는 번호 : "+ GameManager.instance.deleteIdx;

        transform.Rotate(new Vector3(0, 60, 0));
        uiObj.Rotate(new Vector3(0, 60, 0));
    }
    public void Left60Rotate()
    {
        GameManager.instance.deleteIdx++;
        if (GameManager.instance.deleteIdx > 5)
            GameManager.instance.deleteIdx = 0;

        GameManager.instance.deleteText.GetComponent<Text>().text = "지워지는 번호 : " + GameManager.instance.deleteIdx;

        transform.Rotate(new Vector3(0, -60, 0));
        uiObj.Rotate(new Vector3(0, -60, 0));
    }
}
