using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : Player
{
    [Header("플레이어 왼쪽 오른쪽 움직일때 주는 힘")]
    [SerializeField]
    private float playerpos = 0;

    [Header("물에서 왼쪽 오른쪽 움직일때 주는 힘")]
    [SerializeField]
    private float waterplayerpos = 0;

    [Header("물에서 주는 스크롤 최대값")]
    [SerializeField]
    private float waterplayerMaxValue = 0;

    [SerializeField, Tooltip("PlayerState은 순으로 넣어라 반드시")]
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



    private bool isJumpStart = false;
    private bool isJump = false;
    private bool isMove = true;

    private bool isOneWall = false;

    public bool IsMove { get { return isMove; } }


    protected override void Start()
    {
        base.Start();

        UpdateAnimator();
        Init();
    }

    protected override void Update()
    {
        base.Update();
        MapExtent();
        DrowRay();

        PlayerAnimation();
        FrogColorChange();
        ChangeFacing();
        Addicted();

        Debug.Log(isWall);
    }

    void FixedUpdate()
    {
        isGround();
        Move();
    }

    #region 플레이어 에니메이션
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
    #endregion

    #region 점프 스크롤
    private void StartScroll()
    {
        if (isGrounded || isWall)
        {
            rigid.velocity = new Vector2(0.0f, rigid.velocity.y);
            isJumpStart = true;
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
    #endregion

    #region 애니메이터 바꾸기
    public void UpdateAnimator()
    {
        animator.runtimeAnimatorController = frogAnimators[(int)PlayerStateManager.Instance.PlayerState];
    }
    #endregion

    #region 개구리 색깔 바꾸기
    private void FrogColorChange()
    {
        float value = DebuffManager.Instance.Value;
        float maxValue = DebuffManager.Instance.MAXVALUE;
        SeasonState state = DebuffManager.Instance.State;
        if (state == SeasonState.SUMMER_0 || state == SeasonState.SUMMER_1)
        {
            spriteRenderer.color = new Color(255f, 255f - (255f * value / maxValue * 100), 255f - (255f * value / maxValue * 100));
        }
        else if (state == SeasonState.FALL)
        {
            spriteRenderer.color = new Color(255f - (255f * value / maxValue * 100) * .5f, 255f - (255f * value / maxValue * 100), 255f);
        }
    }
    #endregion

    #region 앞에 있는거 삼키기
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
            }
            UpdateAnimator();
        }
    }
    #endregion

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //왼쪽 벽
        if (collision.collider.CompareTag("LeftWall") && isOneWall)
        {
            transform.localEulerAngles = new Vector3(0, 0, 90);
            rigid.bodyType = RigidbodyType2D.Static;
            isWall = true;
            animator.Play("Idle");
        }
        //오른쪽 벽
        else if (collision.collider.CompareTag("RightWall") && isOneWall)
        {
            transform.localEulerAngles = new Vector3(0, 0, -90);
            rigid.bodyType = RigidbodyType2D.Static;
            isWall = true;
            animator.Play("Idle");
        }
        //물 블럭
        else if (collision.collider.CompareTag("Water"))
        {
            if (DebuffManager.Instance.State == SeasonState.SUMMER_1)
            {
                DebuffManager.Instance.UpdateDown(true);
            }
            else if (DebuffManager.Instance.State == SeasonState.SUMMER_1)
            {
                rPlayerpos = waterplayerpos;
                playerScrollbar.maxValue = waterplayerMaxValue;
            }
        }
        //구름 블럭
        else if (collision.collider.CompareTag("Cloud"))
        {
            StartCoroutine(ItemSpawnManager.Instance.ItmeSpawn(PoolObjectType.CLOUD, collision.transform));
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
        if (collision.collider.CompareTag("Water"))
        {
            rPlayerpos = playerpos;
            playerScrollbar.maxValue = rPlayerMaxValue;
        }
        else if (collision.collider.CompareTag("LeftWall") || collision.collider.CompareTag("RightWall"))
        {
            isOneWall = false;
        }
    }

    #region 플레이어 점프 이펙트 생성
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
    #endregion

    #region 플레이어 패치 변경(좌우 변경)
    /// <summary>
    /// 머리의 위치와 버튼 위치 조정
    /// </summary>
    private void ChangeFacing()
    {
        //버튼 위치 조정
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

        //머리 위치 조정
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

    #region 플레이어 기절&이펙트
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
        isMove = false;
        yield return new WaitForSeconds(3);
        isMove = true;
        ObjectPool.Instance.ReturnObject(PoolObjectType.FAINT_RING, faintRing);
        DebuffManager.Instance.IsDebuff = false;
    }
    #endregion


    /// <summary>
    /// 플레이어가 벽에 있을 때 벽에서 때어내주는 변수
    /// </summary>
    private void ChangeWallBool()
    {
        isScrollStart = false;
        isWall = false;
    }

    /// <summary>
    /// 땅에 떨어질때 충격을 완화해주는 함수
    /// </summary>
    private void ifFloor()
    {
        rigid.velocity = new Vector2(rigid.velocity.x / 3, rigid.velocity.y / 3);
    }

    /// <summary>
    /// 땅인지 아닌지 판단해주는 함수
    /// </summary>
    private void isGround()
    {
        if (isGrounded)
        {
            isOneWall = true;
            isMove = true;
        }
    }

    /// <summary>
    /// 움직임을 구현해주는 함수
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

    /// <summary>
    /// 중독상태일때 바꿔주기
    /// </summary>
    private void Addicted()
    {
        playerScrollbar.maxValue = rPlayerMaxValue - (int)DebuffManager.Instance.Value;

        Debug.Log(playerScrollbar.maxValue);
    }

    /// <summary>
    /// 레이캐스트를 그려주는 함수
    /// </summary>
    private void DrowRay()
    {
        direction = spriteRenderer.flipX == true ? Vector3.left : Vector3.right;
        position = new Vector2(transform.position.x, transform.position.y + .5f);

        hit = Physics2D.Raycast(position, direction, 3, LayerMask.GetMask("Ability"));
        Debug.DrawRay(position, direction * 3, Color.green);
    }

    /// <summary>
    /// 맵 나갔는지 아닌지 판단해주는 함수
    /// </summary>
    private void MapExtent()
    {
        //범위에 나갔는지 판단
        if (isGrounded == false)
        {
            //X의 맥스값
            if (this.transform.position.x > GameManager.Instance.mxX)
                this.transform.position = new Vector3(GameManager.Instance.mxX, transform.position.y, transform.position.z);

            //X의 민값
            if (this.transform.position.x < GameManager.Instance.mnX)
                this.transform.position = new Vector3(GameManager.Instance.mnX, transform.position.y, transform.position.z);

            //Y의 맥스값
            if (this.transform.position.y > GameManager.Instance.mxY)
                this.transform.position = new Vector3(transform.position.x, GameManager.Instance.mxY, transform.position.z);

            //Y의 민값
            if (this.transform.position.y < GameManager.Instance.mnY)
                this.transform.position = new Vector3(transform.position.x, 4, transform.position.z);
        }
    }

    /// <summary>
    /// 처음 시작할때 사용하는 함수
    /// </summary>
    private void Init()
    {
        rPlayerpos = playerpos;
        rPlayerMaxValue = playerScrollbar.maxValue;

        EventManager.StartListening("START", StartScroll);
        EventManager.StartListening("STOP", StopScrolling);
        EventManager.StartListening("Swallow", EnemySwallow);
        EventManager.StartListening("Tunder", ChangeWallBool);
        EventManager.StartListening("Faint", PlayerFaint);
        EventManager.StartListening("FloorCheck", ifFloor);
    }
}
