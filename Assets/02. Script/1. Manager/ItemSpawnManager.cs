using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ItemSpawnManager : MonoSingleton<ItemSpawnManager>
{
    public List<GameObject> ropes = new List<GameObject>();

    public IEnumerator ItmeSpawn(PoolObjectType poolObject, Transform transform)
    {
        yield return new WaitForSeconds(5);
        GameObject item = ObjectPool.Instance.GetObject(poolObject);
        item.transform.position = transform.position;
    }

    public IEnumerator WaterSpawn(GameObject animator){
        yield return new WaitForSeconds(5);
        animator.GetComponent<Animator>().SetBool("isEat", false);
    }

    public void RockAndRopeRespawn()
    {
        foreach (var item in ropes)
        {
            item.GetComponent<RopeManager>().CreateRope();
        }
    }
}
