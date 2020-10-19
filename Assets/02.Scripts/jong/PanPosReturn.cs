using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanPosReturn : MonoBehaviour
{
    /* 펜이 떨어졌을때 다시 책상 위로 올리는 코드 */

    public GameObject returnPos;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position.y<=0.3){ // 펜이 떨어졌을때 위로 올리기
            transform.position = returnPos.GetComponent<Transform>().position;
        }
    }
}
