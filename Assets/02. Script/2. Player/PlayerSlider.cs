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
    private void StartScroll()
    {
        if (isGrounded || isWall)
        {
            alpaValue(1);
            float sliderPos = spriteRenderer.flipX == true ? 1.5f : -1.5f;
            Vector3 _playerScrollPos =
                Camera.main.WorldToScreenPoint(new Vector3(transform.position.x + sliderPos, transform.position.y + 1, 0));
            GameManager.Instance.Pool.position = _playerScrollPos;
            playerScrollbar.value = 0;
            isScrollStart = true;
        }
    }

    private void StartScrolling()
    {
        if (isGrounded && isScrollStart || isWall)
        {
            playerScrollbar.value += sliderpos * Time.deltaTime;
            if (thunderNum >= 2)
            {
                thunder = ObjectPool.Instance.GetObject(PoolObjectType.THUNDER);
                thunder.transform.position = new Vector2(transform.position.x + 1.5f, transform.position.y + 1);
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
