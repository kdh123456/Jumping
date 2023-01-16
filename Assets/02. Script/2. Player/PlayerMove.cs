using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Random = UnityEngine.Random;

public class PlayerMove : Player
{
    public enum Facing
    {
        LEFT,
        RIGHT
    }

    #region 프로퍼티
    public Facing facing { get; private set; }
    public bool IsFaint { get => isFaint; }
    public bool IsMove { get { return isMove; } }

    private InteractionUI InteractionUI
    {
        get
        {
            interactionUI ??= FindObjectOfType<InteractionUI>();
            return interactionUI;
        }
        set
        {
            interactionUI = value;
        }
    }

    #endregion

    #region 인스펙터 조절 가능 변수

    [Header("Player Move Pos")]
    [SerializeField]
    private float playerpos = 0;

    [Header("Player Water Player Move Pos")]
    [SerializeField]
    private float waterplayerpos = 0;

    [Header("Player Water Jump Max Value")]
    [SerializeField]
    private float waterplayerMaxValue = 0;

    [SerializeField]
    private float hotpackPower = 10f;
    [SerializeField]
    private float thunderPower = 100f;
    [SerializeField]
    private float iceFloorPower = 10f;

    [SerializeField, Tooltip("Stay In Order")]
    private List<AnimatorOverrideController> frogAnimators = new List<AnimatorOverrideController>();

    #endregion

    #region 프라이빗 참조 변수
    private PlayerTrailEffect playerTrailEffect;
    private InteractionUI interactionUI;
    
    #endregion

    #region 속성

    private float rPlayerpos = 0;
    private float rPlayerMaxValue = 0;
    private Vector3 direction;
    private Vector2 position;
    private RaycastHit2D hit;
    private Transform effectTrm;
    private float moveInput;
    private bool isScrollStart;
    private bool isFacing = false;
    private bool isFacingIce = false;
    private bool isJumpStart = false;
    private bool isJump = false;
    private bool isFaint = false;
    private bool isMove = true;
    private bool isThunder = false;
    private bool isWater = false;
    private bool isOneWall = false;
    private bool isStop = true;


    #endregion

    protected override void Start()
    {
        base.Start();
        playerTrailEffect = GetComponentInChildren<PlayerTrailEffect>();
        UpdateAnimator();
        Init();
        isFacing = (facing == Facing.LEFT) ? true : false;
        effectTrm = transform.Find($"effectTrm");
    }

    protected override void Update()
    {
        base.Update();
        MapExtent();
        DrowRay();

        PlayerAnimation();
        ChangeFacing();
        Addicted();
        isFacing = (facing == Facing.LEFT) ? true : false;
    }

    void FixedUpdate()
    {
        IsGround();
        Move();
        //Debug.Log(isWall);
    }

    void PlayerAnimation()
    {
        if (isStop)
            return;
        animator.SetFloat("jump", rigid.velocity.y);

        //isJump = ((isGrounded != true) || isScrollStart);

        if(isJump)
        {
            playerTrailEffect.ActiveTrail();
        }
        animator.SetBool("isJump", isJump);
        animator.SetInteger("WalkPos", (int)(rigid.velocity.x));
        Debug.Log((int)(moveInput * playerpos));
        animator.SetBool("isWall", isWall);
        animator.SetBool("isStart", GameManager.Instance.IsGameStart);
        animator.SetInteger("timeScale", (int)Time.timeScale);
        animator.SetBool("isMove", IsMove);
    }

    private void StartScroll()
    {
        if ((isGrounded || isWall) && !isStop && !isFaint)
        {
            rigid.velocity = new Vector2(0.0f, rigid.velocity.y);
            isJumpStart = true;
            isJump = true;
            isScrollStart = true;
        }
    }

    private void StopScrolling()
    {
        if ((isGrounded && isScrollStart) || isWall)
        {
            rigid.bodyType = RigidbodyType2D.Dynamic;

            StartCoroutine(CreateDust());

            float tempx = moveInput * rPlayerpos;
            float tempy = playerScrollbar.value;

            rigid.velocity = new Vector2(tempx, tempy);

            isScrollStart = false;
            transform.localEulerAngles = Vector3.zero;
            isJumpStart = false;

            SoundManager.Instance.SetEffectSoundClip(EffectSoundState.Jump);

            isWall = false;
        }
    }

    public void UpdateAnimator()
    {
        animator.runtimeAnimatorController = frogAnimators[(int)PlayerStateManager.Instance.PlayerState];
    }

    public void FrogColorChange(bool isDown)
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
    /// isMove ????????????????????????????????????
    /// </summary>
    /// <param name="value"></param>
    public void ChangeIsMove(int value)
    {
        if (value == 0)
            isMove = false;
        else
            isMove = true;
    }

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
                    UpdateAnimator();
                    break;
                case AbilityState.LADYBUG:
                    ObjectPool.Instance.ReturnObject(PoolObjectType.LADYBUG, hit.collider.transform.parent.gameObject);
                    PlayerStateManager.Instance.UpdateState(PlayerState.LADYBUG);

                    StartCoroutine(ItemSpawnManager.Instance.ItmeSpawn(PoolObjectType.LADYBUG, hit.collider.transform.parent.transform));
                    UpdateAnimator();
                    break;
                case AbilityState.SMALL:
                    ObjectPool.Instance.ReturnObject(PoolObjectType.MUSHROOM, hit.collider.gameObject);
                    PlayerStateManager.Instance.UpdateState(PlayerState.SMALL);

                    StartCoroutine(ItemSpawnManager.Instance.ItmeSpawn(PoolObjectType.MUSHROOM, hit.collider.transform));
                    UpdateAnimator();
                    break;
                case AbilityState.HERB:
                    ObjectPool.Instance.ReturnObject(PoolObjectType.HERB, hit.collider.gameObject);
                    EventManager.TriggerEvent("Herb");

                    StartCoroutine(ItemSpawnManager.Instance.ItmeSpawn(PoolObjectType.HERB, hit.collider.transform));
                    UpdateAnimator();
                    break;
                case AbilityState.FLY:
                    ObjectPool.Instance.ReturnObject(PoolObjectType.FLY, hit.collider.gameObject);
                    PlayerStateManager.Instance.UpdateState(PlayerState.FLY_READY);

                    StartCoroutine(ItemSpawnManager.Instance.ItmeSpawn(PoolObjectType.FLY, hit.collider.transform));
                    //UpdateAnimator();
                    break;
                case AbilityState.WATER:
                    //StartCoroutine(ItemSpawnManager.Instance.WaterSpawn(hit.collider.gameObject));
                    Animator wellAnim = hit.collider.GetComponent<Animator>();
                    if (!wellAnim.GetBool("isEat"))
                    {
                        wellAnim.SetBool("isEat", true);
                        StartCoroutine(ItemSpawnManager.Instance.WaterSpawn(wellAnim));
                        PlayerStateManager.Instance.UpdateState(PlayerState.WATER);
                    }
                    UpdateAnimator();
                    break;
            }
        }
    }

    private IEnumerator IceFloorCoroutine()
    {
        isFacingIce = true;
        rigid.velocity = new Vector2(iceFloorPower, 0);
        yield return new WaitForSeconds(.8f);
        rigid.velocity = Vector2.zero;
        isFacingIce = false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.collider.gameObject.layer == LayerMask.NameToLayer("ground"))
        {
            animator.Play("Idle");
            playerTrailEffect.InActiveTrail();
        }
        if (collision.collider.CompareTag("IceFloor"))
        {
            Debug.Log("iceice");
            //isFacingIce = true;
            //rigid.AddForce((isFacing ? Vector2.left : Vector2.right) * iceFloorPower, ForceMode2D.Impulse);
            StartCoroutine(IceFloorCoroutine());
        }

        //????????
        if (collision.collider.CompareTag("Water"))
        {
            if (DebuffManager.Instance.State == SeasonState.SUMMER_0)
            {
                isWater = true;
                DebuffManager.Instance.UpdateDown(true);
            }
            else if (DebuffManager.Instance.State == SeasonState.SUMMER_1)
            {
                rPlayerpos = waterplayerpos;
                playerScrollbar.maxValue = waterplayerMaxValue;
            }
        }
        else if (collision.collider.CompareTag("BaseFloor") && !isGrounded)
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
        if (collision.collider.CompareTag("IceFloor"))
        {
            Debug.Log("iceice");
            //isFacingIce = false;
        }

        if (collision.collider.CompareTag("Water"))
        {
            rPlayerpos = playerpos;
            playerScrollbar.maxValue = rPlayerMaxValue;
            DebuffManager.Instance.UpdateDown(false);
            isWater = false;
        }
        else if (collision.collider.CompareTag("LeftWall") || collision.collider.CompareTag("RightWall"))
        {
            isOneWall = false;
            isMove = true;
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("LeftWall") && isOneWall)
        {
            transform.localEulerAngles = new Vector3(0, 0, 90);
            rigid.bodyType = RigidbodyType2D.Static;
            isWall = true;
            animator.Play("Idle");
        }
        else if (collision.collider.CompareTag("RightWall") && isOneWall && !isScrollStart)
        {
            transform.localEulerAngles = new Vector3(0, 0, -90);
            rigid.bodyType = RigidbodyType2D.Static;
            isWall = true;
            animator.Play("Idle");
        }
        else if (collision.collider.CompareTag("Cloud") && !isGrounded)
        {
            Debug.Log("?");
            isMove = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Water"))
        {
            if (DebuffManager.Instance.State == SeasonState.SUMMER_0)
            {
                isWater = true;
                DebuffManager.Instance.UpdateDown(true);
            }
            else if (DebuffManager.Instance.State == SeasonState.SUMMER_1)
            {
                rPlayerpos = waterplayerpos;
                playerScrollbar.maxValue = waterplayerMaxValue;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Water"))
        {
            rPlayerpos = playerpos;
            playerScrollbar.maxValue = rPlayerMaxValue;
            DebuffManager.Instance.UpdateDown(false);
            isWater = false;
        }
    }

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

    #region ?????????????????????????????????????????????????????????????????????????????????????????????????????
    /// <summary>
    /// ???????????????????????????????????????? ?????????????????????????????????????
    /// </summary>
    private void ChangeFacing()
    {
        //?????????????????????????????????????
        if (hit.collider != null)
        {
            if (hit.collider.TryGetComponent(out IInteraction interaction))
            {
                InteractionUI?.ActiveUI(interaction);
            }
            else
            {
                InteractionUI?.InActiveUI();
            }
        }
        else
        {
            InteractionUI?.InActiveUI();
        }

        if (Time.timeScale != 0)
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

    }
    #endregion

    public void PlayerFaint()
    {
        StartCoroutine(Faint());
    }

    private IEnumerator Faint()
    {
        GameObject faintRing = ObjectPool.Instance.GetObject(PoolObjectType.FAINT_RING);
        float pos = facing == Facing.LEFT ? -.5f : .5f;
        faintRing.transform.position = transform.position + new Vector3(pos, 1, 0);
        faintRing.transform.SetParent(this.transform);
        isFaint = true;
        spriteRenderer.color = Color.white;
        ChangeIsMove(0);
        rigid.velocity = new Vector2(0, 0);
        yield return new WaitForSeconds(3);
        isFaint = false;
        ChangeIsMove(1);
        ObjectPool.Instance.ReturnObject(PoolObjectType.FAINT_RING, faintRing);
        DebuffManager.Instance.IsDebuff = false;
    }

    private void ChangeWallBool()
    {
        isScrollStart = false;
        isWall = false;
        StartCoroutine(Thunder());
    }
    private void IfFloor()
    {
        if (rigid.velocity.y < 0)
            rigid.velocity = new Vector2(rigid.velocity.x / 3, rigid.velocity.y / 3);
    }
    private void IsGround()
    {
        if (isGrounded)
        {
            isOneWall = true;

            if (isJump && rigid.velocity.y < 0)
                isJump = false;
            if (isFaint)
                isMove = false;
            else
                isMove = true;
        }
    }
    private void Move()
    {
        if (Time.timeScale != 0)
        {
            if (!isJumpStart && GameManager.Instance.IsGameStart)
            {
                if (Input.GetKey(KeySetting.keys[KeyAction.LEFT]))
                {
                    moveInput = -1f;
                    effectTrm.transform.localScale = new Vector3(-1, 1, 1);
                }
                else if (Input.GetKey(KeySetting.keys[KeyAction.RIGHT]))
                {
                    moveInput = 1f;
                    effectTrm.transform.localScale = new Vector3(1, 1, 1);
                }
                else
                {
                    moveInput = 0;
                }

                if (isMove && !isThunder && !isFacingIce && !isStop && !isJumpStart)
                    rigid.velocity = new Vector3(moveInput * playerpos, rigid.velocity.y);

                //if (isJumpStart)
                //    rigid.velocity = Vector2.zero;
            }
        }
    }

    private void Stop()
    {
        isStop = true;
        rigid.velocity = new Vector2(0, 0);
        animator.SetBool("isMove", false);
        animator.Play("Idle");
    }

    public void MoveOn()
    {
        isStop = false;
    }

    private void Addicted()
    {
        //if (DebuffManager.Instance.State == SeasonState.FALL)
        //    playerScrollbar.maxValue = rPlayerMaxValue - (int)DebuffManager.Instance.Value/2;
    }

    private void DrowRay()
    {
        direction = spriteRenderer.flipX == true ? Vector3.left : Vector3.right;
        position = new Vector2(transform.position.x, transform.position.y + .5f);

        hit = Physics2D.Raycast(position, direction, 3, LayerMask.GetMask("Ability"));
        Debug.DrawRay(position, direction * 3, Color.green);
    }

    private void MapExtent()
    {
        if (isGrounded == false)
        {
            if (this.transform.position.x > GameManager.Instance.mxX)
                this.transform.position = new Vector3(GameManager.Instance.mxX, transform.position.y, transform.position.z);

            if (this.transform.position.x < GameManager.Instance.mnX)
                this.transform.position = new Vector3(GameManager.Instance.mnX, transform.position.y, transform.position.z);

            if (this.transform.position.y > GameManager.Instance.mxY)
                this.transform.position = new Vector3(transform.position.x, GameManager.Instance.mxY, transform.position.z);

            if (this.transform.position.y < GameManager.Instance.mnY)
                this.transform.position = new Vector3(transform.position.x, 4, transform.position.z);
        }
    }

    private IEnumerator Thunder()
    {
        isThunder = true;
        float random = Random.Range(0f, 1f);
        Vector2 direction = random > 0.5f ? new Vector2(1, 0) : new Vector2(-1, 0);
        rigid.AddForce(direction * thunderPower, ForceMode2D.Impulse);
        yield return new WaitForSeconds(1f);
        rigid.velocity = new Vector2(0.2f, 0.2f);
        yield return new WaitForSeconds(.1f);
        rigid.velocity = Vector2.zero;
        isThunder = false;
    }

    private void PlayerFly()
    {
        rigid.AddForce(Vector2.up * hotpackPower, ForceMode2D.Impulse);
    }

    private void Init()
    {
        rPlayerpos = playerpos;
        rPlayerMaxValue = playerScrollbar.maxValue;

        EventManager.StartListening("START", StartScroll);
        EventManager.StartListening("STOP", StopScrolling);
        EventManager.StartListening("Swallow", EnemySwallow);
        EventManager.StartListening("Thunder", ChangeWallBool);
        EventManager.StartListening("ThunderExplode", ChangeWallBool);
        EventManager.StartListening("Faint", PlayerFaint);
        EventManager.StartListening("FloorCheck", IfFloor);
        EventManager.StartListening("RESET", Reset);
        EventManager.StartListening("Stop", Stop);
        EventManager.StartListening("Hotpack", PlayerFly);
    }

    public override void Reset()
    {
        base.Reset();
        playerpos = rPlayerpos;
        isJumpStart = false;
        isJump = false;
        isFaint = false;
        isMove = true;
        isThunder = false;
        isOneWall = false;
        isScrollStart = false;
        transform.position = new Vector2(-4f, 9f);
        isStop = false;
        rigid.bodyType = RigidbodyType2D.Dynamic;
        DebuffManager.Instance.Reset();
    }
}
