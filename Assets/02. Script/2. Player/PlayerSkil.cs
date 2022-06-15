using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PlayerSkil : Player
{
    private PlayerMove playerMove = null;
    private bool isFacing = false;
    private BoxCollider2D playerCollider = null;
    
    private bool isEmpty = false;
    private bool isCheck = false;

    private bool isCanMove = true;


    protected override void Start()
    {
        base.Start();

        TryGetComponent(out playerMove);
        TryGetComponent(out playerCollider);

        isFacing = (playerMove.facing == PlayerMove.Facing.LEFT) ? true : false;

        EventManager.StartListening("Fire", Fire);
        EventManager.StartListening("Umbrella", CreateUmbrella);
        //EventManager.StartListening("Small", GetSmaller);
        EventManager.StartListening("Fly", EatFly);
        EventManager.StartListening("EatWell", EatWell);
        EventManager.StartListening("Herb", UseMedicinalHerb);
    }

    protected override void Update()
    {
        base.Update();

        isFacing = (playerMove.facing == PlayerMove.Facing.LEFT) ? true : false;
    }

    #region fireball
    public void Fire()
    {
        GameObject fireBall = ObjectPool.Instance.GetObject(PoolObjectType.FIREBALL_OBJECT);
        fireBall.transform.position = this.transform.position;
        fireBall.GetComponent<SpriteRenderer>().flipX = isFacing;

        fireBall.transform.DOMove((isFacing ? Vector3.left : Vector3.right) * 10, 1) // ???????????????????????????????�곣뫖利?�㏄?? ??????????????????�곣뫖利?�㏄??
            .SetEase(Ease.Linear).SetRelative()
            .OnComplete(() => ObjectPool.Instance.ReturnObject(PoolObjectType.FIREBALL_OBJECT, fireBall));

        PlayerStateManager.Instance.UpdateState(PlayerState.BASIC);
        playerMove.UpdateAnimator();
    }
    #endregion

    #region umbrella
    private bool isUmbrella = false;
    public void CreateUmbrella()
    {
        if (isUmbrella) return;
        GameObject umbrella = ObjectPool.Instance.GetObject(PoolObjectType.UMBRELLA);
        umbrella.transform.position = transform.position + Vector3.up;
        umbrella.transform.SetParent(this.transform);
        umbrella.GetComponent<SpriteRenderer>().flipX = isFacing;
        isUmbrella = true;
        DebuffManager.Instance.UpdateDown(true);
        StartCoroutine(DeleteUmbrella(umbrella));
    }

    private IEnumerator DeleteUmbrella(GameObject gameObject)
    {
        yield return new WaitForSeconds(5);
        ObjectPool.Instance.ReturnObject(PoolObjectType.UMBRELLA, gameObject);
        DebuffManager.Instance.UpdateDown(false);
        isUmbrella = true;
        PlayerStateManager.Instance.UpdateState(PlayerState.BASIC);
        playerMove.UpdateAnimator();
    }
    #endregion

    #region Smaller And Bigger
    [System.Obsolete]
    private bool isSmall = false;

    [System.Obsolete]
    private void GetSmaller()
    {
        if (isSmall) return;
        // ???????????????????????????????�곣뫖利?�㏄????
        //playerCollider.size = new Vector2(playerCollider.size.x * .5f, playerCollider.size.y * .5f);
        //playerCollider.offset = new Vector2(0, -.47f);
        this.transform.localScale = Vector3.one * .5f;
        isSmall = true;
        StartCoroutine(GetBigger());
    }
    [System.Obsolete]
    private IEnumerator GetBigger()
    {
        yield return new WaitForSeconds(3);
        //playerCollider.size = new Vector2(playerCollider.size.x * 2, playerCollider.size.y * 2);
        //playerCollider.offset = Vector2.zero;
        this.transform.localScale = Vector3.one;
        isSmall = false;
        PlayerStateManager.Instance.UpdateState(PlayerState.BASIC);
        playerMove.UpdateAnimator();
    }
    #endregion

    private bool isFlyEat = false;
    string flyEmpty = "Fly_Empty";

    private void OnCollisionEnter2D(Collision2D other) {
        if(other.collider.CompareTag(flyEmpty))
        {
            isCanMove = false;
            isEmpty = true;
            if(!isCanMove)
            {
                rigid.constraints = RigidbodyConstraints2D.FreezePositionX;
            }
        }
    }
    private void OnCollisionExit2D(Collision2D other)
    {
        if (other.collider.CompareTag(flyEmpty) && !isCheck)
        {
            isEmpty = false;
            isCanMove = true;
            if (isCanMove)
            {
                rigid.constraints = RigidbodyConstraints2D.None;
                rigid.constraints = RigidbodyConstraints2D.FreezeRotation;
                PlayerStateManager.Instance.UpdateState(PlayerState.BASIC);
                playerMove.UpdateAnimator();
            }
        }
    }

    #region Herb
    public void UseMedicinalHerb()
    {
        StartCoroutine(UseMedicinalHerbCoroutine());
        //DebuffManager.Instance.Value = 0f;
        //DebuffManager.Instance.Reset();
    }

  
    IEnumerator UseMedicinalHerbCoroutine()
    {
        DebuffManager.Instance.UpdateDown(false);
        yield return new WaitForSeconds(1);
        DebuffManager.Instance.UpdateDown(true);
    }
    #endregion

    #region Fly
    public void EatFly()
    {
        isEmpty = true;
        Debug.Log("isflly");
        animator.Play("Idle");
        GameObject fly_empty = ObjectPool.Instance.GetObject(PoolObjectType.FLY_EMPTY);
        rigid.velocity = Vector2.zero;
        fly_empty.transform.position = transform.position + Vector3.down;
        PlayerStateManager.Instance.UpdateState(PlayerState.FLY);
        playerMove.UpdateAnimator();

        StartCoroutine(DeleteFly_Empty(fly_empty));
    }

    private IEnumerator DeleteFly_Empty(GameObject gameObject)
    {
        while(true)
        {
            yield return null;
            if (!isEmpty)
            {
                ObjectPool.Instance.ReturnObject(PoolObjectType.FLY_EMPTY, gameObject);
                //PlayerStateManager.Instance.UpdateState(PlayerState.BASIC);
            }
        }
        
        
    }
    #endregion

    #region Well
    public void EatWell()
    {
        //GameObject well = ObjectPool.Instance.GetObject(PoolObjectType.WELL);
        //wellAnimator.SetBool("isEat",true);
        GameObject waterball = ObjectPool.Instance.GetObject(PoolObjectType.WATERBALL);
        waterball.transform.position = this.transform.position;
        waterball.GetComponent<SpriteRenderer>().flipX = isFacing;
        waterball.transform.DOMove((isFacing ? Vector2.left : Vector2.right) * 5, .5f)
            .SetEase(Ease.Linear).SetRelative()
            .OnComplete(() => ObjectPool.Instance.ReturnObject(PoolObjectType.WATERBALL, waterball));
        EventManager.TriggerEvent("Thunder");
        EventManager.TriggerEvent("ThunderExplode");
        rigid.AddForce((isFacing ? Vector2.right : Vector2.left) * 200, ForceMode2D.Impulse);
        PlayerStateManager.Instance.UpdateState(PlayerState.BASIC);
    }
    #endregion
}