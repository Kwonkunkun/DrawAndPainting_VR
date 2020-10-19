using UnityEngine;
using Valve.VR;

public class PaletteMgr : MonoBehaviour
{
    /*  */

    //왼손 컨트롤러의 Input Source
    public SteamVR_Input_Sources leftHand = SteamVR_Input_Sources.LeftHand;
    //트랙패드의 클릭 여부
    public SteamVR_Action_Boolean trackPad = SteamVR_Actions.default_Teleport;

    //트랙패드의 터치 위치
    public SteamVR_Action_Vector2 trackPadPosition = SteamVR_Actions.default_TrackpadPosition;

    void Update()
    {
        //트랙패드를 클릭했을 경우
        if (trackPad.GetStateDown(leftHand))
        {
            Debug.Log("왼쪽 트랙패드 클릭");
            //트랙패드의 터치 위치값을 추출
            Vector2 touchPos = trackPadPosition.GetLastAxis(leftHand);
            Debug.Log(touchPos);

            if (touchPos.x >= 0.2f)
            {
                Debug.Log("왼쪽 트랙패드의 오른쪽 클릭");
                //트랙패드의 오른쪽 부분을 터치했을 경우
                //90도씩 회전값을 누적해 감소
                // iTween.RotateTo(this.gameObject, iTween.Hash("rotation",new Vector3(0,-90.0f,0), "time", 0.2f, "easetype", iTween.EaseType.easeOutBounce));
                iTween.RotateBy(this.gameObject, iTween.Hash("y", -0.25f, "time", 0.2f, "easetype", iTween.EaseType.easeOutBounce));
            }
            else if (touchPos.x <= -0.2f)
            {
                Debug.Log("왼쪽 트랙패드의 왼쪽 클릭");
                //트랙패드의 왼쪽 부분을 터치했을 경우
                //90도씩 회전값을 누적해 증가
                // iTween.RotateTo(this.gameObject, iTween.Hash("rotation", new Vector3(0,-90.0f,0), "time", 0.2f , "easetype" , iTween.EaseType.easeOutBounce));
                iTween.RotateBy(this.gameObject, iTween.Hash("y", 0.25f, "time", 0.2f, "easetype", iTween.EaseType.easeOutBounce));
            }
        }   
    }
}
