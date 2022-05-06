using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : Player
{
    private RaycastHit2D hit;
    private Vector3 direction;
    private Vector2 position;
    [HideInInspector]
    public SeasonalDebuff seasonalDebuff = null;
    public GameObject itemButton = null;
    private float moveInput;
    private bool isScrollStart;
    public enum Facing
    {
        LEFT,
        RIGHT
    }

    public Facing facing { get; private set; }

    [Header("플레이어 왼쪽 오른쪽 움직일때 주는 힘")]
    [SerializeField]
    private int playerpos = 0;


    //private bool thisWall = false;
    private bool isJumpStart = false;
    private bool isJump = false;
    private bool isMove = true;
    public bool IsMove { get { return isMove; } }

    [SerializeField, Tooltip("PlayerState은 순으로 넣어라 반드시")]
    private List<AnimatorOverrideController> frogAnimators = new List<AnimatorOverrideController>();

    protected override void Start()
    {
        base.Start();

        TryGetComponent(out seasonalDebuff);

        EventManager.StartListening("START", StartScroll);
        EventManager.StartListening("STOP", StopScrolling);
        EventManager.StartListening("Swallow", EnemySwallow);
        EventManager.StartListening("Tunder", ChangeBool);
        EventManager.StartListening("Faint", PlayerFaint);

        UpdateAnimator();
    }
    protected override void Update()
    {
        direction = spriteRenderer.flipX == true ? Vector3.left : Vector3.right;
        position = new Vector2(transform.position.x, transform.position.y + .5f);

        hit = Physics2D.Raycast(position, direction, 3, LayerMask.GetMask("Ability"));
        Debug.DrawRay(position, direction * 3, Color.green);
        base.Update();
        //moveInput = Input.GetAxisRaw("Horizontal");

        PlayerAnimation();

        if (Time.timeScale != 0)
        {
            ChangeFacing();
        }

        if(isGrounded)
        {
            isOneWall = true;
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

    void FixedUpdate()
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
        if(isGrounded || isWall)
        {
            rigid.velocity = new Vector2(0.0f, rigid.velocity.y);
            isJumpStart = true;
            isScrollStart = true;
        }
    }

    private void StopScrolling()
    {
        if(isGrounded && isScrollStart || isWall)
        {
            rigid.bodyType = RigidbodyType2D.Dynamic;
            isWall = false;
            StartCoroutine(CreateDust());
            float tempx = moveInput * playerpos;
            float tempy = playerScrollbar.value;
            rigid.velocity = new Vector2(tempx, tempy);
            isScrollStart = false;
            transform.localEulerAngles = Vector3.zero;
            isJumpStart = false;
            SoundManager.Instance.SetEffectSoundClip(EffectSoundState.Jump);
        }
    }
    #endregion

    #region 에니메이터 바꾸기
    public void UpdateAnimator()
    {
        animator.runtimeAnimatorController = frogAnimators[(int)PlayerStateManager.Instance.PlayerState];
    }
    #endregion

    #region 앞에 있는거 삼키기
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
                case AbilityState.Fly:
                    EventManager.TriggerEvent("Fly");
                    ObjectPool.Instance.ReturnObject(PoolObjectType.FLY, hit.collider.gameObject);
                    //ObjectPool.Instance.ReturnObject(PoolObjectType.FLY_EMPTY,hit.collider.gameObject);
                    PlayerStateManager.Instance.UpdateState(PlayerState.FLY);
                    StartCoroutine(ItemSpawnManager.Instance.ItmeSpawn(PoolObjectType.FLY, hit.collider.transform));
                    break;
                case AbilityState.Water:
                    EventManager.TriggerEvent("Water");
                    ObjectPool.Instance.ReturnObject(PoolObjectType.FLY, hit.collider.gameObject);
                    PlayerStateManager.Instance.UpdateState(PlayerState.FLY);
                    StartCoroutine(ItemSpawnManager.Instance.ItmeSpawn(PoolObjectType.FLY, hit.collider.transform));
                    break;

            }
            UpdateAnimator();
        }
    }
    #endregion

    private float zrot;
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("LeftWall") && isOneWall)
        {
            transform.localEulerAngles = new Vector3(0, 0, 90);
            rigid.bodyType = RigidbodyType2D.Static;
            isWall = true;
            isOneWall = false;
            animator.Play("Idle");
        }
        else if(collision.collider.CompareTag("RightWall") && isOneWall)
        {
            transform.localEulerAngles = new Vector3(0, 0, -90);
            rigid.bodyType = RigidbodyType2D.Static;
            isWall = true;
            isOneWall = false;
            animator.Play("Idle");
        }
        else if (collision.collider.CompareTag("Floor"))
        {
            SoundManager.Instance.SetEffectSoundClip(EffectSoundState.Land);
        }
        else if(collision.collider.CompareTag("Water"))
        {
            playerpos = 2;
            playerScrollbar.maxValue = 20;
        }
        else if (collision.collider.CompareTag("Cloud"))
        {
            StartCoroutine(ItemSpawnManager.Instance.ItmeSpawn(PoolObjectType.CLOUD, collision.transform));
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Water"))
        {
            playerpos = 5;
            playerScrollbar.maxValue = 30;
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

    private void ChangeBool()
    {
        isWall = false;
    }

    #region 플레이어 패치 변경(좌우 변경)
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
        //transform.localScale = new Vector3(scaleX, 1f, 1f);
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
        seasonalDebuff.IsDebuff = false;
    }
    #endregion

    //?
}
