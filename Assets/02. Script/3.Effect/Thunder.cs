using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Thunder : MonoBehaviour
{
    public AudioSource audioSource;
    public float fieldofImpact;

    public float force;

    public LayerMask LayerToHit;

    private Collider2D objects;
    private void Awake()
    {
        //if (SoundManager.Instance != null)
            audioSource.volume = SoundManager.Instance.GetEffectVolume();
    }
    private void Start()
    {
        objects = GetComponent<Collider2D>();
        audioSource.Play();
    }

    public void ThunderEnd()
    {
        ObjectPool.Instance.ReturnObject(PoolObjectType.THUNDER, this.gameObject);
    }

    public void Explode(Rigidbody2D collider)
    {
        float random = Random.Range(0f, 1f);
        Vector2 direction = random > 0.5f ? new Vector2(1, 0) : new Vector2(-1, 0);

        EventManager.TriggerEvent("ThunderExplode");
        collider.AddForce(direction * force, ForceMode2D.Impulse);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Explode(collision.attachedRigidbody);
        }
    }

#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, fieldofImpact);
    }
#endif
}
