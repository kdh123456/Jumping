using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class SwipeUI : MonoBehaviour
{
    [SerializeField]
    private Scrollbar scrollbar; //Scrollbar???꾩튂瑜?諛뷀깢?쇰줈 ?꾩옱 ?섏씠吏 寃??
    [SerializeField]
    private float swipeTime = 0.2f; //?섏씠吏媛 swipe ?섎뒗 ?쒓컙
    [SerializeField]
    private float swipeDistance = 50.0f;//?섏씠吏媛 swipe?섍린 ?꾪빐 ?吏곸씠??理쒖냼 嫄곕━

    private float[] scrollPageValues; //媛??섏씠吏???꾩튂 媛?0.0 ~ 1.0]
    private float valueDistance = 0;//媛??섏씠吏 ?ъ씠??嫄곕━
    private int currentPage = 0;//?꾩옱 ?섏씠吏
    private int maxPage = 0;//理쒕? ?섏씠吏
    private float startTouchx;//?곗튂 ?쒖옉 ?꾩튂
    private float endTouchx; // ?곗튂 醫낅즺 ?꾩튂
    private bool isSwipeMode = false;//?꾩옱 swipe媛 ?섍퀬 ?덈뒗吏 泥댄겕

    private void Awake()
    {
        //?ㅽ겕濡??섎뒗 ?섏씠吏??媛?value 媛믪쓣 ??ν븯??諛곗뿴 硫붾え由??좊떦
        scrollPageValues = new float[transform.childCount];

        //?ㅽ겕濡??섎뒗 ?섏씠吏 ?ъ씠??嫄곕━
        valueDistance = 1f / (scrollPageValues.Length - 1f);

        //?ㅽ겕濡??섎뒗 ?섏씠吏??媛?value ?꾩튂 ?ㅼ젙 [0 <= value <= 1]
        for(int i=0;i<scrollPageValues.Length;--i)
        {
            scrollPageValues[i] = valueDistance * i;
        }
        //理쒕? ?섏씠吏????
        maxPage = transform.childCount;

    }

    private void Start()
    {
        //理쒖큹 ?쒖옉????0踰??섏씠吏瑜?蹂????덈룄濡??ㅼ젙
        SetScrollBarValue(0);
    }

    public void SetScrollBarValue(int index)
    {
        currentPage = index;
        scrollbar.value = scrollPageValues[index];
    }

    private void UpdateInput()
    {
        //?꾩옱 swipe瑜?吏꾪뻾以묒씠硫??곗튂 遺덇?
        if (isSwipeMode == true) return;
        
        if(Input.GetKeyDown(KeyCode.Mouse0))
        {
            //?ㅼ떆 ?쒖옉 吏??(Swipe 諛⑺뼢 援щ텇)
            startTouchx = Input.mousePosition.x;
        }
        else if(Input.GetMouseButtonDown(0))
        {
            //?곗튂 以묒슂 吏??
            endTouchx = Input.mousePosition.x;

            UpdateInput();
        }
    }

    private void UpdateSwipe()
    {
        //?덈Т ?묒? 嫄곕━瑜??吏곸????뚮뒗 Swpie X
        if(Mathf.Abs(startTouchx-endTouchx)<swipeDistance)
        {
            //?먮옒 ?섏씠吏濡?Swipe?댁꽌 ?뚯븘媛꾨떎
            StartCoroutine(OnSwipeOneStep(currentPage));
            return;
        }
        //swipe 諛⑺뼢
        bool isLeft = startTouchx < endTouchx ? true : false;

        //?대룞 諛⑺뼢???쇱そ????
        if(isLeft==true)
        {
            //?꾩옱 ?섏씠吏媛 ?쇱そ ?앹씠硫?醫낅즺
            if (currentPage == 0) return;
            //?쇱そ?쇰줈 ?대룞???꾪빐 ?꾩옱 ?섏씠吏瑜?1 媛먯냼
            currentPage--;
        }
        //?대룞 諛⑺뼢???ㅻⅨ履쎌씪 ??
        else
        {
            //?꾩옱 ?섏씠吏媛 ?ㅻⅨ履??앹씠硫?醫낅즺
            if (currentPage == maxPage - 1) return;
            //?ㅻⅨ履쎌쑝濡??대룞???꾪빐 ?꾩옱 ?섏씠吏瑜?1 利앷?
            currentPage++;
        }
        //currentIndex踰덉㎏ ?섏씠吏濡?Swipe?댁꽌 ?대룞
        StartCoroutine(OnSwipeOneStep(currentPage));
    }
    //?섏씠吏瑜??????놁쑝濡??섍린??swipe ?④낵 ?ъ깮
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
