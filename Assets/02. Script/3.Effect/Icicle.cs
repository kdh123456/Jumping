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
    public AudioSource audioSource;
    public AudioClip clip;

    private readonly int hashDestroy = Animator.StringToHash("destroy");

    private void Awake()
    {
        audioSource.clip = clip;
    }

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
            audioSource.volume = SoundManager.Instance.GetEffectVolume();
            if (collision.CompareTag("Player"))
            {
                EventManager.TriggerEvent("ThunderExplode");
            }

            rb.velocity = new Vector2(0, 0);
            audioSource.Play();
            Debug.Log(audioSource.clip);
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
