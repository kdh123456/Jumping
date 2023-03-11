using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerSlider : Player
{
    [SerializeField]
    private Image Bk;
    [SerializeField]
    private Image Fill;
    [SerializeField]
    private Image Handle;
    [SerializeField]
    private float sliderpos = 1f;

    public int thunderNum = 0;

    private GameObject thunder;
    private Animator effectAnimator;

    private bool isScrollStart;

    protected override void Start()
    {

        base.Start();
        //thunder.TryGetComponent(out effectAnimator);
        EventManager.StartListening("Tunder", ChangeBool);
        EventManager.StartListening("START", StartScroll);
        EventManager.StartListening("STARTING", StartScrolling);
        EventManager.StartListening("STOP", StopScrolling);

        rect = playerScrollbar.GetComponent<RectTransform>();
    }
    protected override void Update()
    {
        base.Update();
        //if (thunder.activeSelf == true)
        //{
        //    if (effectAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f)
        //    {
        //        thunder.SetActive(false);
        //    }
        //}
    }

    RectTransform rect;
    private void StartScroll()
    {
        if (isGrounded || isWall)
        {
            alpaValue(1);
            float sliderPos = spriteRenderer.flipX == true ? 1.5f : -1.5f;

            rect.anchoredPosition = new Vector3(sliderPos, rect.anchoredPosition.y, -1f);
            //Vector3 _playerScrollPos = new Vector3(transform.localPosition.x + sliderPos, transform.localPosition.y + 1, 0);

            //if (Screen.height - 130 < _playerScrollPos.y)
            //{
            //    _playerScrollPos.y += _playerScrollPos.y - Screen.height - 130;
            //}
            //else if (130 > _playerScrollPos.y)
            //{
            //    _playerScrollPos.y += 130 - _playerScrollPos.y;
            //}

            /*GameManager.Instance.Pool.position = _playerScrollPos*/;

            playerScrollbar.value = 0;
            isScrollStart = true;
        }
    }

    private void StartScrolling()
    {
        if(!isGrounded && !isWall)
        {
            alpaValue(0);
            playerScrollbar.value = 0;
        }

        if (isGrounded && isScrollStart || isWall)
        {
            playerScrollbar.value += sliderpos * Time.deltaTime;
            if (thunderNum >= 2)
            {
                thunder = null;
                thunder = ObjectPool.Instance.GetObject(PoolObjectType.THUNDER);
                Vector3 pos = transform.position;
                pos.y += 1.5f;
                thunder.transform.position = pos;
                transform.localEulerAngles = Vector3.zero;
                rigid.bodyType = RigidbodyType2D.Dynamic;
                EventManager.TriggerEvent("Tunder");
                //thunder.SetActive(true);
                thunderNum = 0;
            }
            if (playerScrollbar.value <= 0)
            {
                thunderNum++;
                sliderpos = -sliderpos;
            }
            else if (playerScrollbar.value >= playerScrollbar.maxValue)
            {
                sliderpos = -sliderpos;
            }
        }
    }

    private void StopScrolling()
    {
        if (isGrounded && isScrollStart || isWall)
        {
            thunderNum = 0;

            alpaValue(0);
            isScrollStart = false;
        }
    }

    private void alpaValue(int a)
    {
        Debug.Log(a);
        Color bk = Bk.color;
        bk.a = a;
        Bk.color = bk;
        Color fill = Fill.color;
        fill.a = a;
        Fill.color = fill;
        Color handle = Handle.color;
        handle.a = a;
        Handle.color = handle;
    }

    private void ChangeBool()
    {
        alpaValue(0);
        isScrollStart = false;
    }
}
