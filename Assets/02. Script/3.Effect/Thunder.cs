using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Thunder : MonoBehaviour
{
    public float fieldofImpact;

    public float force;

    public LayerMask LayerToHit;

    private Collider2D objects;

    private void OnEnable()
    {
        Explode();
    }

    private void Update()
    {
        objects = Physics2D.OverlapCircle(transform.position, fieldofImpact, LayerToHit);

    }

    public void ThunderEnd()
    {
        //this.gameObject.SetActive(false);
        ObjectPool.Instance.ReturnObject(PoolObjectType.THUNDER, this.gameObject);
    }

    public void Explode()
    {
        float random = Random.value;
        Vector2 direction = random > .5f ? Vector2.left : Vector2.right;
        if (objects != null)
        {
            EventManager.TriggerEvent("ThunderExplode");
            Debug.Log(objects.name);
            objects.GetComponent<Rigidbody2D>().AddForce(direction * force, ForceMode2D.Impulse);
            if (objects.GetComponent<Rigidbody2D>() != null)
                objects.GetComponent<Rigidbody2D>().AddForce(direction * force, ForceMode2D.Impulse);
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
