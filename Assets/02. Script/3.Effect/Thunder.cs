using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Thunder : MonoBehaviour
{
    public AudioSource audioSource;
    public float fieldofImpact = 4;

    public float force = 100;

    public LayerMask LayerToHit;

    private Collider2D objects;

    private void Start()
    {
        objects = GetComponent<Collider2D>();
    }

    private void OnEnable()
    {
        audioSource.volume = SoundManager.Instance.GetEffectVolume();
        audioSource.Play();
    }

    public void ThunderEnd()
    {
        ObjectPool.Instance.ReturnObject(PoolObjectType.THUNDER, this.gameObject);
    }

    public void Explode()
    {
        //float random = Random.Range(0f, 1f);
        //Vector2 direction = random > 0.5f ? new Vector2(1, 0) : new Vector2(-1, 0);
        EventManager.TriggerEvent("ThunderExplode");
        //collider.AddForce(direction * force, ForceMode2D.Impulse);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Explode();
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
