using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Icicle : MonoBehaviour
{
    [SerializeField]
    private float spawnDelay = 3f;
    Vector2 basicTransform;
    Rigidbody2D rb;
    private Animator animator;

    private readonly int hashDestroy = Animator.StringToHash("destroy");

    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        //basicTransform = new Vector2(transform.position.x, transform.position.y);
        basicTransform = transform.position;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") == true || collision.CompareTag("BaseFloor") == true)
        {
            rb.velocity = new Vector2(0, 0);
            // 사운드 재생, 애니메이션 재생
            SoundManager.Instance.SetEffectSoundClip(EffectSoundState.Icicle);
            animator.SetTrigger(hashDestroy);
        }
    }

    void Spawn()
    {
        transform.position = basicTransform;
        gameObject.SetActive(true);
    }

    public void SetIcicleActive(int value)
    {
        gameObject.SetActive(value != 0);
    }

    private void OnDisable()
    {
        Invoke("Spawn", spawnDelay);
        //StartCoroutine(SpawnCoroutine());
    }
}
