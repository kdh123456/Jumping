using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody2D))]
public class Player : MonoBehaviour
{
    [SerializeField]
    protected Slider playerScrollbar;

    protected BoxCollider2D floorCol;

    protected Rigidbody2D rigid;
    protected bool isGrounded;
    protected Animator animator;
    protected SpriteRenderer spriteRenderer;
    protected BoxCollider2D col;
    protected bool isCanFly = false;


    protected static bool isWall = false;


    protected virtual void Start()
    {
        TryGetComponent(out rigid);
        TryGetComponent(out animator);
        TryGetComponent(out spriteRenderer);
        TryGetComponent(out col);
        floorCol = GetComponentInChildren(typeof(BoxCollider2D)) as BoxCollider2D;
    }
    protected virtual void Update()
    {
        isGrounded = Physics2D.OverlapBox(new Vector2(gameObject.transform.position.x, gameObject.transform.position.y - 1), new Vector2(floorCol.size.x - 0.3f, floorCol.size.y), 180f, LayerMask.GetMask("ground"));
    }

    protected virtual void Reset()
    {

    }
}
