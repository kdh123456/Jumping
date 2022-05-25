using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PlayerMove : Player
{
    private RaycastHit2D hit;
    private Vector3 direction;
    private Vector2 position;
    public GameObject itemButton = null;
    private float moveInput;
    private bool isScrollStart;
    public enum Facing
    {
        LEFT,
        RIGHT
    }

    public Facing facing { get; private set; }

    [Header("??????????????롮쾸?椰???????????????????????????源녾텛???????濾?????????????嫄??????????????????ш끽維뽳쭩?뱀땡??????")]
    [SerializeField]
    private int playerpos = 0;


    //private bool thisWall = false;
    private bool isJumpStart = false;
    private bool isJump = false;
    private bool isFaint = false;
    public bool IsFaint { get => isFaint; }
    private bool isMove = true;

    private bool isOneWall = false;

    public bool IsMove { get { return isMove; } }

    [SerializeField, Tooltip("PlayerState?? ???????됰Ŧ????????????????????????????ш끽維뽳쭩?뱀땡???얩맪????????????????")]
    private List<AnimatorOverrideController> frogAnimators = new List<AnimatorOverrideController>();

    protected override void Start()
    {
        base.Start();
        //TryGetComponent(out seasonalDebuff);
        //spriteRenderer = GetComponent<SpriteRenderer>();

        EventManager.StartListening("START", StartScroll);
        EventManager.StartListening("STOP", StopScrolling);
        EventManager.StartListening("Swallow", EnemySwallow);
        EventManager.StartListening("Tunder", ChangeWallBool);
        EventManager.StartListening("Faint", PlayerFaint);
        EventManager.StartListening("FloorCheck", IfFloor);
        EventManager.StartListening("ColorChange", () => FrogColorChange(DebuffManager.Instance.IsDown));

        UpdateAnimator();
    }

    protected override void Update()
    {
        direction = spriteRenderer.flipX == true ? Vector3.left : Vector3.right;
        position = new Vector2(transform.position.x, transform.position.y + .5f);

        hit = Physics2D.Raycast(position, direction, 3, LayerMask.GetMask("Ability"));
        Debug.DrawRay(position, direction * 3, Color.green);
        base.Update();

        PlayerAnimation();
        //FrogColorChange(); // ??????????????????밸븶筌믩끃??獄???????멥렑??????????癲ル슢?싩땟?????????????됰Ŧ????????????????⑤슢堉??곕??????????

        if (Time.timeScale != 0)
        {
            ChangeFacing();
        }


        if (hit.collider != null)
        {
            if (hit.collider.TryGetComponent(out Ability_State ability_State))
            {
                itemButton.transform.position = this.transform.position + new Vector3((facing == Facing.LEFT ? -1.5f : 1.5f), .7f, 0);
                itemButton.SetActive(true);
            }
            else
            {
                itemButton.SetActive(false);
            }
        }
        else
        {
            itemButton.SetActive(false);
        }

        //???????????????????????紐???? ?????
        if (isGrounded == false)
        {
            //X????????嫄???????????????怨뺤떪????
            if (this.transform.position.x > GameManager.Instance.mxX)
                this.transform.position = new Vector3(GameManager.Instance.mxX, transform.position.y, transform.position.z);

            //X??????????????????????????쇄뵦????
            if (this.transform.position.x < GameManager.Instance.mnX)
                this.transform.position = new Vector3(GameManager.Instance.mnX, transform.position.y, transform.position.z);

            //Y????????嫄???????????????怨뺤떪????
            if (this.transform.position.y > GameManager.Instance.mxY)
                this.transform.position = new Vector3(transform.position.x, GameManager.Instance.mxY, transform.position.z);

            //Y??????????????????????????쇄뵦????
            if (this.transform.position.y < GameManager.Instance.mnY)
                this.transform.position = new Vector3(transform.position.x, 4, transform.position.z);
        }
    }

    void FixedUpdate()
    {
        IsGround();
        Move();
    }

    /// <summary>
    /// ??????????????롮쾸?椰???????????????룸챷援?????????????
    /// </summary>
    void PlayerAnimation()
    {
        animator.SetFloat("jump", rigid.velocity.y);

        isJump = ((isGrounded != true) || isScrollStart);

        animator.SetBool("isJump", isJump);
        animator.SetInteger("WalkPos", (int)(moveInput * playerpos));
        animator.SetBool("isWall", isWall);
        animator.SetBool("isStart", GameManager.Instance.IsGameStart);
        animator.SetInteger("timeScale", (int)Time.timeScale);
        animator.SetBool("isMove", IsMove);
    }

    /// <summary>
    /// ????????怨쀫엥????????????????ш끽維뽳쭩?뱀땡???얩맪??????????????????됰Ŧ????????????????
    /// </summary>
    private void StartScroll()
    {
        if (isGrounded || isWall)
        {
            rigid.velocity = new Vector2(0.0f, rigid.velocity.y);
            isJumpStart = true;
            isScrollStart = true;
        }
    }

    /// <summary>
    /// ????????怨쀫엥????????????????ш끽維뽳쭩?뱀땡???얩맪?????????????????????寃몃탿????????????????
    /// </summary>
    private void StopScrolling()
    {
        if ((isGrounded && isScrollStart) || isWall)
        {
            rigid.bodyType = RigidbodyType2D.Dynamic;

            StartCoroutine(CreateDust());

            float tempx = moveInput * playerpos;
            float tempy = playerScrollbar.value;

            rigid.velocity = new Vector2(tempx, tempy);

            isScrollStart = false;
            transform.localEulerAngles = Vector3.zero;
            isJumpStart = false;

            SoundManager.Instance.SetEffectSoundClip(EffectSoundState.Jump);

            isWall = false;
        }
    }

    /// <summary>
    /// ????????????????룸챷援????????????????????ш끽維뽳쭩?뱀땡???얩맪?????????????
    /// </summary>
    public void UpdateAnimator()
    {
        animator.runtimeAnimatorController = frogAnimators[(int)PlayerStateManager.Instance.PlayerState];
    }

    /// <summary>
    /// ??????????????????????????????ш끽維뽳쭩?뱀땡???얩맪?????????????
    /// </summary>
    public void FrogColorChange(bool isDown) // ??????????????????????...
    {
        //transform.DOKill();
        DOTween.Kill(transform);
        SeasonState state = DebuffManager.Instance.State;

        if (GameManager.Instance.IsGameStart)
        {
            if (!isFaint)
            {
                if (state == SeasonState.SUMMER_0 || state == SeasonState.SUMMER_0)
                {
                    if (isDown)
                    {
                        spriteRenderer.DOColor(Color.white, 20).SetAutoKill();
                    }
                    else
                    {
                        spriteRenderer.DOColor(new Color(255, 0, 0), 20).SetAutoKill();
                    }
                }
                else if (state == SeasonState.FALL)
                {
                    if (isDown)
                    {
                        spriteRenderer.DOColor(new Color(127, 0, 255), 20).SetAutoKill();
                    }
                    else
                    {
                        spriteRenderer.DOColor(Color.white, 1).SetAutoKill();
                    }
                }
            }
        }
    }

    /// <summary>
    /// isMove ?????????怨뺤떪??????????
    /// </summary>
    /// <param name="value"></param>
    public void ChangeIsMove(int value)
    {
        if (value == 0)
            isMove = false;
        else
            isMove = true;
    }

    /// <summary>
    /// ??????癲ル슢?싩땟???????????影??젘???????????????????????????????
    /// </summary>
    public void EnemySwallow()
    {
        animator.Play("Swallow");
        if (hit.collider != null)
        {
            switch (hit.collider.gameObject.GetComponent<Ability_State>().abilityState)
            {
                case AbilityState.FIREBALL:
                    ObjectPool.Instance.ReturnObject(PoolObjectType.FIREBALL_ITEM, hit.collider.gameObject);
                    PlayerStateManager.Instance.UpdateState(PlayerState.FIREBALL);

                    StartCoroutine(ItemSpawnManager.Instance.ItmeSpawn(PoolObjectType.FIREBALL_ITEM, hit.collider.transform));
                    break;
                case AbilityState.LADYBUG:
                    ObjectPool.Instance.ReturnObject(PoolObjectType.LADYBUG, hit.collider.gameObject);
                    PlayerStateManager.Instance.UpdateState(PlayerState.LADYBUG);

                    StartCoroutine(ItemSpawnManager.Instance.ItmeSpawn(PoolObjectType.LADYBUG, hit.collider.transform));
                    break;
                case AbilityState.SMALL:
                    ObjectPool.Instance.ReturnObject(PoolObjectType.MUSHROOM, hit.collider.gameObject);
                    PlayerStateManager.Instance.UpdateState(PlayerState.SMALL);

                    StartCoroutine(ItemSpawnManager.Instance.ItmeSpawn(PoolObjectType.MUSHROOM, hit.collider.transform));
                    break;
                case AbilityState.HERB:
                    ObjectPool.Instance.ReturnObject(PoolObjectType.HERB, hit.collider.gameObject);
                    EventManager.TriggerEvent("Herb");

                    StartCoroutine(ItemSpawnManager.Instance.ItmeSpawn(PoolObjectType.HERB, hit.collider.transform));
                    break;
            }
            UpdateAnimator();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //??????????????
        if (collision.collider.CompareTag("LeftWall") && isOneWall)
        {
            transform.localEulerAngles = new Vector3(0, 0, 90);
            rigid.bodyType = RigidbodyType2D.Static;
            isWall = true;
            animator.Play("Idle");
        }
        //???????????源녾텛???????濾???????
        else if (collision.collider.CompareTag("RightWall") && isOneWall)
        {
            transform.localEulerAngles = new Vector3(0, 0, -90);
            rigid.bodyType = RigidbodyType2D.Static;
            isWall = true;
            animator.Play("Idle");
        }
        //????????
        else if (collision.collider.CompareTag("Water"))
        {
            if (DebuffManager.Instance.State == SeasonState.SUMMER_0)
            {
                DebuffManager.Instance.UpdateDown(true);
            }
            else if(DebuffManager.Instance.State == SeasonState.SUMMER_1)
            {
                playerpos = 2;
                playerScrollbar.maxValue = 20;
            }
        }
        //?????????????
        else if (collision.collider.CompareTag("Cloud"))
        {
            StartCoroutine(ItemSpawnManager.Instance.ItmeSpawn(PoolObjectType.CLOUD, collision.transform));
        }
        else if (collision.collider.CompareTag("BaseFloor")&& !isGrounded)
        {
            isMove = false;
        }
        else if ((collision.collider.CompareTag("LeftWall") || collision.collider.CompareTag("RightWall")) && !isGrounded)
        {
            isMove = false;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Water"))
        {
            if (DebuffManager.Instance.State == SeasonState.SUMMER_0)
            {
                DebuffManager.Instance.UpdateDown(false);
            }
            else if (DebuffManager.Instance.State == SeasonState.SUMMER_1)
            {
                playerpos = 5;
                playerScrollbar.maxValue = 30;
            }
            
        }
        else if(collision.collider.CompareTag("LeftWall") || collision.collider.CompareTag("RightWall"))
        {
            isOneWall = false;
        }
    }

    /// <summary>
    /// ??????????????롮쾸?椰????????????怨쀫엥?????????????????썹땟戮녹??諭?????⑸㎦???????????袁⑸즴筌?씛彛???돗???????
    /// </summary>
    /// <returns></returns>
    IEnumerator CreateDust()
    {
        if (!isWall)
        {
            Vector2 dustPos = new Vector2(transform.position.x, transform.position.y);
            yield return new WaitForSeconds(.2f);
            GameObject dust = ObjectPool.Instance.GetObject(PoolObjectType.DUST);
            dust.transform.position = dustPos;
            yield return new WaitForSeconds(.2f);
            ObjectPool.Instance.ReturnObject(PoolObjectType.DUST, dust);
        }
    }

    /// <summary>
    /// ??????????????롮쾸?椰??????????????????怨뺤떪?????????????????????怨뺤떪?????
    /// </summary>
    private void ChangeFacing()
    {
        if (rigid.velocity.x > 1f)
        {
            facing = Facing.RIGHT;
        }
        else if (rigid.velocity.x < -1f)
        {
            facing = Facing.LEFT;
        }
        spriteRenderer.flipX = facing == Facing.RIGHT ? false : true;
    }

    /// <summary>
    /// ??????????????롮쾸?椰?????????????????
    /// </summary>
    public void PlayerFaint()
    {
        StartCoroutine(Faint());
    }

    /// <summary>
    /// ??????????????롮쾸?椰?????????????????????썹땟戮녹??諭?????⑸㎦?????
    /// </summary>
    /// <returns></returns>
    private IEnumerator Faint()
    {
        GameObject faintRing = ObjectPool.Instance.GetObject(PoolObjectType.FAINT_RING);
        float pos = facing == Facing.LEFT ? -.5f : .5f;
        faintRing.transform.position = transform.position + new Vector3(pos, 1, 0);
        faintRing.transform.SetParent(this.transform);
        isFaint = true;
        spriteRenderer.color = Color.white;
        ChangeIsMove(0);
        yield return new WaitForSeconds(3);
        isFaint = false;
        ChangeIsMove(1);
        ObjectPool.Instance.ReturnObject(PoolObjectType.FAINT_RING, faintRing);
        DebuffManager.Instance.IsDebuff = false;
    }


    /// <summary>
    /// ??????????????롮쾸?椰?????????????? ??????????????????????????????????????????????????釉먯뒠?????????????????怨뺤떪?????
    /// </summary>
    private void ChangeWallBool()
    {
        isScrollStart = false;
        isWall = false;
    }

    /// <summary>
    /// ???????????????????????????????????????留⑶뜮?????????붺몭??????????????????????????????釉먯뒠?????????????
    /// </summary>
    private void IfFloor()
    {
        rigid.velocity = new Vector2(rigid.velocity.x / 3, rigid.velocity.y / 3);
    }

    /// <summary>
    /// ????????關?쒎첎?嫄????????嫄?????????? ??????????밸븶筌믩끃??獄???????멥렑????????????濡?씀?濾??? ???????????釉먯뒠?????????????
    /// </summary>
    private void IsGround()
    {
        if(isGrounded)
        {
            isOneWall = true;
            if (isFaint)
                isMove = false;
            else
                isMove = true;
        }
    }

    /// <summary>
    /// ????????嫄???????????????????轅붽틓????곌퇈猶욘쾮???????釉먯뒠?????????????
    /// </summary>
    private void Move()
    {
        if (Time.timeScale != 0)
        {
            if (!isJumpStart && !isWall && GameManager.Instance.IsGameStart)
            {
                if (Input.GetKey(KeySetting.keys[KeyAction.LEFT]))
                    moveInput = -1f;
                else if (Input.GetKey(KeySetting.keys[KeyAction.RIGHT]))
                    moveInput = 1f;
                else
                    moveInput = 0;

                if (isMove)
                    rigid.velocity = new Vector3(moveInput * playerpos, rigid.velocity.y);
            }
        }
    }
}
