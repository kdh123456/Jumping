using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudBlock : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            StartCoroutine(DestroyBlock());
        }
    }

    IEnumerator DestroyBlock()
    {
        yield return new WaitForSeconds(2f);
        ObjectPool.Instance.ReturnObject(PoolObjectType.CLOUD, this.transform.parent.gameObject);
    }
}
