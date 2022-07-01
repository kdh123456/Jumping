using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Random = UnityEngine.Random;

public class PlayerMove : Player
{
    [Header("??????????????????????????뷂┼?????????????????????????????????????????????????????????????????????????????쏙쭗????????????????????????????????????????????????")]
    [SerializeField]
    private float playerpos = 0;

    [Header("??????????????????????嫄??????????????????????????????????????????????????????????????????????쏙쭗????????????????????????????????????????????????")]
    [SerializeField]
    private float waterplayerpos = 0;

    [Header("??????????????????????嫄????????????????????????????????????????????????????????????????????????????")]
    [SerializeField]
    private float waterplayerMaxValue = 0;

    [SerializeField]
    private float hotpackPower = 10f;
    [SerializeField]
    private float thunderPower = 100f;

    [SerializeField, Tooltip("PlayerState?? ????????????????????????????????????????????????????????????????????????꾩룆梨띰쭕?뚢뵾??????????????嶺뚮죭?댁젘??????????????????????")]
    private List<AnimatorOverrideController> frogAnimators = new List<AnimatorOverrideController>();

    private float rPlayerpos = 0;
    private float rPlayerMaxValue = 0;

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

    private bool isFacing = false;

    private bool isFacingIce = false;

    private bool isJumpStart = false;
    private bool isJump = false;
    private bool isFaint = false;
    public bool IsFaint { get => isFaint; }
    private bool isMove = true;
    private bool isThunder = false;
    private bool isWater = false;

    private bool isOneWall = false;

    public bool IsMove { get { return isMove; } }

    private bool isStop = true;


    protected override void Start()
    {
        base.Start();

        UpdateAnimator();
        Init();
        isFacing = (facing == Facing.LEFT) ? true : false;

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

    /// <summary>
    /// ?????????????????????????????????????????????룸챷援???????????????????????
    /// </summary>
    void PlayerAnimation()
    {
        if (isStop)
            return;
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
    /// ????????????????????????????????????????????獄쏅챶留덌┼??????????????筌롈살젔?????????????????????????????????????????????????????????????????????????????????????????????????????????????????
    /// </summary>
    private void StartScroll()
    {
        if ((isGrounded || isWall) && !isStop)
        {
            rigid.velocity = new Vector2(0.0f, rigid.velocity.y);
            isJumpStart = true;
            isScrollStart = true;
        }
    }

    /// <summary>
    /// ????????????????????????????????????????????獄쏅챶留덌┼??????????????筌롈살젔???????????????????????????????????????????????????????????????????????????????????????????????????????????븐뼐???????癲ル슢??蹂잜맪???????????????????????????룸㎗??琉우º??????????????????????
    /// </summary>
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

    /// <summary>
    /// ??????????????????????????룸챷援???????????????????????????????????????????獄쏅챶留덌┼??????????????筌롈살젔??????????????????????????????????????????????????????????????????????????????????????????
    /// </summary>
    public void UpdateAnimator()
    {
        animator.runtimeAnimatorController = frogAnimators[(int)PlayerStateManager.Instance.PlayerState];
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

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.collider.gameObject.layer == LayerMask.NameToLayer("ground"))
        {
            animator.Play("Idle");
        }
        else if (collision.collider.CompareTag("IceFloor"))
        {
            Debug.Log("iceice");
            isFacingIce = true;
            rigid.AddForce((isFacing ? Vector2.left : Vector2.right) * 180, ForceMode2D.Impulse);
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
            isFacingIce = false;
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

    #region ??????????????????????????뷂┼??????????????????????????????????????????????????????????????????????
    /// <summary>
    /// ???????????????????????????????????????? ?????????????????????????????????????
    /// </summary>
    private void ChangeFacing()
    {
        //?????????????????????????????????????
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

        //?????????????????????????????????????????????????????
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
    /// <summary>
    /// ?????????????????????????????????????
    /// </summary>
    public void PlayerFaint()
    {
        StartCoroutine(Faint());
    }

    /// <summary>
    /// ???????????????????????????????????????????????????????????꾩룆梨띰쭕?뚢뵾??????????????嶺뚮죭?댁젘?????????????????????????????????????????????????????????????????????????????????
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
    /// ?????????????????????????????????? ?????????????????????????????????????????????????????????????????????????????????????????????
    /// </summary>
    private void ChangeWallBool()
    {
        isScrollStart = false;
        isWall = false;
        StartCoroutine(Thunder());
    }

    /// <summary>
    /// ????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????
    /// </summary>
    private void IfFloor()
    {
        if (rigid.velocity.y < 0)
            rigid.velocity = new Vector2(rigid.velocity.x / 3, rigid.velocity.y / 3);
    }
    /// <summary>
    /// ???????????????????????????????????? ????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????????? ????????????????????????????
    /// </summary>
    private void IsGround()
    {
        if (isGrounded)
        {
            isOneWall = true;
            if (isFaint)
                isMove = false;
            else
                isMove = true;
        }
    }

    /// <summary>
    /// ???????????????????????????????????????????????????????????????????????????????????????????????????
    /// </summary>
    private void Move()
    {
        if (Time.timeScale != 0)
        {
            if (!isJumpStart && GameManager.Instance.IsGameStart)
            {
                if (Input.GetKey(KeySetting.keys[KeyAction.LEFT]))
                    moveInput = -1f;
                else if (Input.GetKey(KeySetting.keys[KeyAction.RIGHT]))
                    moveInput = 1f;
                else
                    moveInput = 0;

                if (isMove && !isThunder && !isFacingIce && !isStop)
                    rigid.velocity = new Vector3(moveInput * playerpos, rigid.velocity.y);
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

    /// <summary>
    /// ???????????????????????ㅼ뒧???怨???????????????????????????????????????????????????????????꾩룆梨띰쭕?뚢뵾??????????????嶺뚮죭?댁젘??????????????????????????????????뷂┼????????????????
    /// </summary>
    private void Addicted()
    {
        //if (DebuffManager.Instance.State == SeasonState.FALL)
        //    playerScrollbar.maxValue = rPlayerMaxValue - (int)DebuffManager.Instance.Value/2;
    }

    /// <summary>
    /// ??????????????????????????????????????????뷂┼????? ????????????????????????????????????????????????뷂┼?????????????????????????????????
    /// </summary>
    private void DrowRay()
    {
        direction = spriteRenderer.flipX == true ? Vector3.left : Vector3.right;
        position = new Vector2(transform.position.x, transform.position.y + .5f);

        hit = Physics2D.Raycast(position, direction, 3, LayerMask.GetMask("Ability"));
        Debug.DrawRay(position, direction * 3, Color.green);
    }

    /// <summary>
    /// ???????????????????????????? ????????????????????????????????????怨뺤꽢?? ??????????????????????????뷂┼??????????????????????????뷂┼???????????????????
    /// </summary>
    private void MapExtent()
    {
        //?????????????????????????????????? ??????????????????????????뷂┼??
        if (isGrounded == false)
        {
            //X???????????????????????????????欲꼲??????
            if (this.transform.position.x > GameManager.Instance.mxX)
                this.transform.position = new Vector3(GameManager.Instance.mxX, transform.position.y, transform.position.z);

            //X??????????????????????????????
            if (this.transform.position.x < GameManager.Instance.mnX)
                this.transform.position = new Vector3(GameManager.Instance.mnX, transform.position.y, transform.position.z);

            //Y???????????????????????????????欲꼲??????
            if (this.transform.position.y > GameManager.Instance.mxY)
                this.transform.position = new Vector3(transform.position.x, GameManager.Instance.mxY, transform.position.z);

            //Y??????????????????????????????
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

    /// <summary>
    /// 
    /// </summary>
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
        //????????????????????????
        //????????????????????????嫄?????????????
    }
}
