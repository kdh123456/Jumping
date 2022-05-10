using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Thunder : MonoBehaviour
{
    public float fieldofImpact;

    public float force;

    public LayerMask LayerToHit;

    private void Start()
    {
        EventManager.StartListening("ThunderExplode", Explode);
    }

    private void OnEnable()
    {
        Explode();
    }

    public void ThunderEnd()
    {
        //this.gameObject.SetActive(false);
        ObjectPool.Instance.ReturnObject(PoolObjectType.THUNDER, this.gameObject);
    }

    public void Explode()
    {
        Collider2D objects = Physics2D.OverlapCircle(transform.position, fieldofImpact, LayerToHit);

        float random = Random.value;
        Vector2 direction = random > .5f ? Vector2.left : Vector2.right;
        if (objects != null)
        {
            if (objects.GetComponent<Rigidbody2D>() != null)
                objects.GetComponent<Rigidbody2D>().AddForce(direction * force, ForceMode2D.Impulse);
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, fieldofImpact);
    }

}
