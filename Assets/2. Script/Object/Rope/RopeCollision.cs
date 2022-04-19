using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class RopeCollision : MonoBehaviour
{
    private int index = 0;
    
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("FireBall"))
        {
            collision.transform.DOKill();
            ObjectPool.Instance.ReturnObject(PoolObjectType.FIREBALL_OBJECT, collision.gameObject);

            // ropemanager에 ropefire호출
            if (!GetComponentInParent<RopeManager>().isFire)
            {
                this.GetComponentInParent<RopeManager>().SetDeleteIndex(index);
            }
        }
    }

    public void SetIndex(int i)
    {
        index = i;
    }
}
