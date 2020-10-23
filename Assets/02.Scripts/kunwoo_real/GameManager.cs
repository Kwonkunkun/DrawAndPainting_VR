using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//게임 시작, 종료, 진행 시간
public class GameManager : MonoBehaviour
{
    public bool isGameStart;
    public float gameTime;
    public float currentTime;
    public int maxIdx = 6;
    public int currentIdx = 0;

    //위치들
    public Transform[] pos;

    #region 싱글톤

    private static GameManager m_instance; // 싱글톤이 할당될 static 변수
    public static GameManager instance
    {
        get
        {
            // 만약 싱글톤 변수에 아직 오브젝트가 할당되지 않았다면
            if (m_instance == null)
            {
                // 씬에서 GameManager 오브젝트를 찾아 할당
                m_instance = FindObjectOfType<GameManager>();
            }

            // 싱글톤 오브젝트를 반환
            return m_instance;
        }
    }
    private void Awake()
    {
        // 씬에 싱글톤 오브젝트가 된 다른 GameManager 오브젝트가 있다면
        if (instance != this)
        {
            // 자신을 파괴
            Destroy(gameObject);
        }
    }
    #endregion

    void Start()
    {
        isGameStart = false;
    }

    void Update()
    {
        //게임 시간
        if(isGameStart == true)
        {
            if(currentTime > gameTime)
            {
                //여기다가 게임 init넣기
                GameInit();
            }
            currentTime += Time.deltaTime;
        }
    }

    //게임 초기화 부분!! 나중에 추가
    public void GameInit()
    {
        //그 파이썬에 보내는 함수도 넣으면 될듯

        isGameStart = false;
        currentTime = 0;

        //추가로 초기화할것 넣기
    }

    public void OnTable(GameObject obj)
    {
        if (obj.transform.childCount == 0)
            return;

        GameObject gobj = obj.transform.GetChild(0).gameObject;
        
        //이미 차있을 경우
        if (pos[currentIdx].childCount >= 1)
            Destroy(pos[currentIdx].GetChild(0).gameObject);

        gobj.transform.position = pos[currentIdx].position;
        gobj.transform.SetParent(pos[currentIdx]);
        currentIdx++;
        currentIdx %= 6;
    }
}
