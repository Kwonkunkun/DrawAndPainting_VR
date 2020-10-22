using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OnButton : MonoBehaviour
{
    public Button button;
    public bool isClicked;
    public Color newColor;
    
    private ColorBlock cb_default;
    private ColorBlock cb_pressed;

    private void Start()
    {
        isClicked = false;

        cb_default = button.colors;
        cb_pressed = button.colors;
        cb_pressed.normalColor = newColor;

    }

    IEnumerator ReturnIsClicked()
    {
        yield return new WaitForSeconds(0.5f);
        isClicked = false;
        button.colors = cb_default;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (button != null && isClicked == false)
            {
                print("Player button touch");
                button.onClick.Invoke();
                button.colors = cb_pressed;
                isClicked = true;
                StartCoroutine(ReturnIsClicked());
            }
        }
    }
}
