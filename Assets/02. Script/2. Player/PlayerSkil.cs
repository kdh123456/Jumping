using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PlayerSkil : MonoBehaviour
{
    private PlayerMove playerMove = null;
    private bool isFacing = false;
    private BoxCollider2D playerCollider = null;

    protected void Start()
    {
        TryGetComponent(out playerMove);
        TryGetComponent(out playerCollider);

        isFacing = (playerMove.facing == PlayerMove.Facing.LEFT) ? true : false;

        EventManager.StartListening("Fire", Fire);
        EventManager.StartListening("Umbrella", CreateUmbrella);
        EventManager.StartListening("Small", GetSmaller);
        EventManager.StartListening("Herb", UseMedicinalHerb);
        EventManager.StartListening("Fly", EatFly);

    }

    protected void Update()
    {
        isFacing = (playerMove.facing == PlayerMove.Facing.LEFT) ? true : false;
    }

    #region 파이어볼 쏘기
    public void Fire()
    {
        GameObject fireBall = ObjectPool.Instance.GetObject(PoolObjectType.FIREBALL_OBJECT);
        fireBall.transform.position = this.transform.position;
        fireBall.GetComponent<SpriteRenderer>().flipX = isFacing;

        fireBall.transform.DOMove((isFacing ? Vector3.left : Vector3.right) * 10, 1) // ?占쏙옙?占쏙옙占? ?占쏙옙占?
            .SetEase(Ease.Linear).SetRelative()
            .OnComplete(() => ObjectPool.Instance.ReturnObject(PoolObjectType.FIREBALL_OBJECT, fireBall));

        PlayerStateManager.Instance.UpdateState(PlayerState.BASIC);
        playerMove.UpdateAnimator();
        Debug.Log("1");
    }
    #endregion

    #region 우산만들기
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

    #region 커지고 작아지기 // 사용하지 않음
    [System.Obsolete]
    private bool isSmall = false;

    [System.Obsolete]
    private void GetSmaller()
    {
        if (isSmall) return;
        // ?占쏙옙?占쏙옙占?占?
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

    #region 허브 먹기
    public void UseMedicinalHerb()
    {
        StartCoroutine(UseMedicinalHerbCoroutine());
    }

    IEnumerator UseMedicinalHerbCoroutine()
    {
        DebuffManager.Instance.UpdateDown(false);
        yield return new WaitForSeconds(0.01f);
        DebuffManager.Instance.UpdateDown(true);
    }
    #endregion

    #region 파리 먹기

    private bool isFlyEat = false;
    public void EatFly()
    {
        isFlyEat = true;
        if(isFlyEat)
        {
            GameObject fly_empty = ObjectPool.Instance.GetObject(PoolObjectType.FLY_EMPTY);
            fly_empty.transform.position = transform.position + Vector3.down;
            StartCoroutine(DeleteFly_Empty(fly_empty));
        }
    }
    private IEnumerator DeleteFly_Empty(GameObject gameObject)
    {
        yield return new WaitForSeconds(5);
        ObjectPool.Instance.ReturnObject(PoolObjectType.FLY_EMPTY, gameObject);
        //playerMove.seasonalDebuff.UpdateDown(false);
        isFlyEat = false;
        PlayerStateManager.Instance.UpdateState(PlayerState.BASIC);
        playerMove.UpdateAnimator();
    }


    #endregion
    
}
