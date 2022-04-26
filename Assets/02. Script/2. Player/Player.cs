using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Rigidbody2D))]
public class Player : MonoBehaviour
{
    [SerializeField]
    protected Slider playerScrollbar;
    protected Rigidbody2D rigid;
    protected bool isGrounded;
    protected Animator animator;
    protected SpriteRenderer spriteRenderer;
    protected BoxCollider2D col;

    protected static bool isWall = false;
    protected static bool isOneWall = false;

    protected virtual void Start()
    {
        TryGetComponent(out rigid);
        TryGetComponent(out animator);
        TryGetComponent(out spriteRenderer);
        TryGetComponent(out col);
    }
    protected virtual void Update()
    {
        isGrounded = Physics2D.OverlapBox(new Vector2(gameObject.transform.position.x + col.offset.x, gameObject.transform.position.y + col.offset.y), new Vector2(col.size.x, col.size.y), 180f, LayerMask.GetMask("ground"));
    }
}
