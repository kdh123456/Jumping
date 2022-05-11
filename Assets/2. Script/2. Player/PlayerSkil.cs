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

    protected override void Start()
    {
        base.Start();

        TryGetComponent(out playerMove);
        TryGetComponent(out playerCollider);

        isFacing = (playerMove.facing == PlayerMove.Facing.LEFT) ? true : false;

        EventManager.StartListening("Fire", Fire);
        EventManager.StartListening("Umbrella", CreateUmbrella);
        EventManager.StartListening("Small", GetSmaller);
        EventManager.StartListening("Fly", EatFly);
        EventManager.StartListening("Well", EatWell);
        EventManager.StartListening("Water", Water);

    }

    private void OnCollisionEnter2D(Collision2D other) {
        if(other.collider.CompareTag("Fly_Empty"))
        {
            isEmpty = true;
            Debug.Log(1);
        }
    }

    protected override void Update()
    {
        base.Update();

        isFacing = (playerMove.facing == PlayerMove.Facing.LEFT) ? true : false;
    }

    #region ?��?��?�� �? ?���?
    public void Fire()
    {
        GameObject fireBall = ObjectPool.Instance.GetObject(PoolObjectType.FIREBALL_OBJECT);
        fireBall.transform.position = this.transform.position;
        fireBall.GetComponent<SpriteRenderer>().flipX = isFacing;

        fireBall.transform.DOMove((isFacing ? Vector3.left : Vector3.right) * 10, 1) // ?��?���? ?���?
            .SetEase(Ease.Linear).SetRelative()
            .OnComplete(() => ObjectPool.Instance.ReturnObject(PoolObjectType.FIREBALL_OBJECT, fireBall));

        PlayerStateManager.Instance.UpdateState(PlayerState.BASIC);
        playerMove.UpdateAnimator();
    }
    #endregion

    #region ?��?�� ?���?
    private bool isUmbrella = false;
    public void CreateUmbrella()
    {
        if (isUmbrella) return;
        GameObject umbrella = ObjectPool.Instance.GetObject(PoolObjectType.UMBRELLA);
        umbrella.transform.position = transform.position + Vector3.up;
        umbrella.transform.SetParent(this.transform);
        umbrella.GetComponent<SpriteRenderer>().flipX = isFacing;
        isUmbrella = true;
        playerMove.seasonalDebuff.UpdateDown(true);
        StartCoroutine(DeleteUmbrella(umbrella));
    }

    private IEnumerator DeleteUmbrella(GameObject gameObject)
    {
        yield return new WaitForSeconds(5);
        ObjectPool.Instance.ReturnObject(PoolObjectType.UMBRELLA, gameObject);
        playerMove.seasonalDebuff.UpdateDown(false);
        isUmbrella = true;
        PlayerStateManager.Instance.UpdateState(PlayerState.BASIC);
        playerMove.UpdateAnimator();
    }
    #endregion

    #region ?��?���?�?

    private bool isSmall = false;
    private void GetSmaller()
    {
        if (isSmall) return;
        // ?��?���?�?
        //playerCollider.size = new Vector2(playerCollider.size.x * .5f, playerCollider.size.y * .5f);
        //playerCollider.offset = new Vector2(0, -.47f);
        this.transform.localScale = Vector3.one * .5f;
        isSmall = true;
        StartCoroutine(GetBigger());
    }

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

    #region �ĸ��ɷ�!

    private bool isFlyEat = false;

    private void OnCollisionExit2D(Collision2D other) {
        if(other.collider.CompareTag("Fly_Empty"))
        {
            isEmpty = false;
        }
    }

    public void EatFly()
    {
        isFlyEat = true;
        if(isFlyEat)
        {
            rigid.velocity = Vector2.zero;
            animator.Play("Idle");
            GameObject fly_empty = ObjectPool.Instance.GetObject(PoolObjectType.FLY_EMPTY);
            fly_empty.transform.position = transform.position + Vector3.down;
            PlayerStateManager.Instance.UpdateState(PlayerState.BASIC);
            StartCoroutine(DeleteFly_Empty(fly_empty));
        }
        isFlyEat = false;
    }
    
    private IEnumerator DeleteFly_Empty(GameObject gameObject)
    {
        while(true)
        {
            yield return null;
            if(!isEmpty)
            {
                ObjectPool.Instance.ReturnObject(PoolObjectType.FLY_EMPTY, gameObject);
            }
        }
        
    }


    #endregion
    
    #region ? ??? ??

    private bool isWater = false;


    public void EatWell()
    {
        GameObject well = ObjectPool.Instance.GetObject(PoolObjectType.WELL);
        isWater = true;
        //StartCoroutine(DeleteWell(well));

    }

    /*private IEnumerator DeleteWell(GameObject gameObject)
    {
        yield return new WaitForSeconds(5);
        ObjectPool.Instance.ReturnObject(PoolObjectType.WELL, gameObject);
    }*/
    
    public void Water()
    {
        if(isWater)
        {
            GameObject waterball = ObjectPool.Instance.GetObject(PoolObjectType.WATERBALL);
            waterball.transform.position = this.transform.position;
            waterball.GetComponent<SpriteRenderer>().flipX = isFacing;

            waterball.transform.DOMove((isFacing ? Vector3.left : Vector3.right) * 10, 1) // ?��?���? ?���?
                .SetEase(Ease.Linear).SetRelative()
                .OnComplete(() => ObjectPool.Instance.ReturnObject(PoolObjectType.WATERBALL, waterball));
            GetComponent<Rigidbody2D>().AddForce((isFacing ? Vector3.right : Vector3.left)*10);

            PlayerStateManager.Instance.UpdateState(PlayerState.BASIC);

            StartCoroutine(DeleteWater(waterball));
        }
        

    }
    private IEnumerator DeleteWater(GameObject gameObject)
    {
        yield return new WaitForSeconds(5);
        ObjectPool.Instance.ReturnObject(PoolObjectType.WATERBALL, gameObject);
        isWater = false;
    }

    #endregion

}
