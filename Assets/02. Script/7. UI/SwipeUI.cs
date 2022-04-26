using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class SwipeUI : MonoBehaviour
{
    [SerializeField]
    private Scrollbar scrollbar; //Scrollbar의 위치를 바탕으로 현재 페이지 검사
    [SerializeField]
    private float swipeTime = 0.2f; //페이지가 swipe 되는 시간
    [SerializeField]
    private float swipeDistance = 50.0f;//페이지가 swipe되기 위해 움직이는 최소 거리

    private float[] scrollPageValues; //각 페이지의 위치 값[0.0 ~ 1.0]
    private float valueDistance = 0;//각 페이지 사이의 거리
    private int currentPage = 0;//현재 페이지
    private int maxPage = 0;//최대 페이지
    private float startTouchx;//터치 시작 위치
    private float endTouchx; // 터치 종료 위치
    private bool isSwipeMode = false;//현재 swipe가 되고 있는지 체크

    private void Awake()
    {
        //스크롤 되는 페이지의 각 value 값을 저장하는 배열 메모리 할당
        scrollPageValues = new float[transform.childCount];

        //스크롤 되는 페이지 사이의 거리
        valueDistance = 1f / (scrollPageValues.Length - 1f);

        //스크롤 되는 페이지의 각 value 위치 설정 [0 <= value <= 1]
        for(int i=0;i<scrollPageValues.Length;--i)
        {
            scrollPageValues[i] = valueDistance * i;
        }
        //최대 페이지의 수
        maxPage = transform.childCount;

    }

    private void Start()
    {
        //최초 시작할 때 0번 페이지를 볼 수 있도록 설정
        SetScrollBarValue(0);
    }

    public void SetScrollBarValue(int index)
    {
        currentPage = index;
        scrollbar.value = scrollPageValues[index];
    }

    private void UpdateInput()
    {
        //현재 swipe를 진행중이면 터치 불가
        if (isSwipeMode == true) return;
        
        if(Input.GetKeyDown(KeyCode.Mouse0))
        {
            //다시 시작 지점 (Swipe 방향 구분)
            startTouchx = Input.mousePosition.x;
        }
        else if(Input.GetMouseButtonDown(0))
        {
            //터치 중요 지점
            endTouchx = Input.mousePosition.x;

            UpdateInput();
        }
    }

    private void UpdateSwipe()
    {
        //너무 작은 거리를 움직였을 때는 Swpie X
        if(Mathf.Abs(startTouchx-endTouchx)<swipeDistance)
        {
            //원래 페이지로 Swipe해서 돌아간다
            StartCoroutine(OnSwipeOneStep(currentPage));
            return;
        }
        //swipe 방향
        bool isLeft = startTouchx < endTouchx ? true : false;

        //이동 방향이 왼쪽일 때
        if(isLeft==true)
        {
            //현재 페이지가 왼쪽 끝이면 종료
            if (currentPage == 0) return;
            //왼쪽으로 이동을 위해 현재 페이지를 1 감소
            currentPage--;
        }
        //이동 방향이 오른쪽일 때
        else
        {
            //현재 페이지가 오른쪽 끝이면 종료
            if (currentPage == maxPage - 1) return;
            //오른쪽으로 이동을 위해 현재 페이지를 1 증가
            currentPage++;
        }
        //currentIndex번째 페이지로 Swipe해서 이동
        StartCoroutine(OnSwipeOneStep(currentPage));
    }
    //페이지를 한 장 옆으로 넘기는 swipe 효과 재생
    private IEnumerator OnSwipeOneStep(int index)
    {
        float start = scrollbar.value;
        float current = 0;
        float percent = 0;
        isSwipeMode = true;
        while(percent<1)
        {
            current += Time.deltaTime;
            percent = current / swipeTime;
            scrollbar.value = Mathf.Lerp(start, scrollPageValues[index], percent);
            yield return null;
        }
        isSwipeMode = false;
    }
}
