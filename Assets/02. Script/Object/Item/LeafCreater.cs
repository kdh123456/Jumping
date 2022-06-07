using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeafCreater : MonoBehaviour
{
    [HideInInspector]
    public List<Transform> createPoints;

    public float delay = 5f;
    public float delayRandomness;

    private Coroutine leafCoroutine;

    private void Start()
    {
        //GetComponentsInChildren<Transform>(createPoints);
        for(int i = 0; i < transform.childCount; i++)
        {
            createPoints.Add(transform.GetChild(i).transform);
        }

        leafCoroutine = StartCoroutine(SpawnLeaf());
    }

    private IEnumerator SpawnLeaf()
    {
        yield return new WaitForSeconds(delay);

        while (true)
        {
            if (GameManager.Instance.IsGameStart)
            {
                foreach (Transform pos in createPoints)
                {
                    GameObject leaf = ObjectPool.Instance.GetObject(PoolObjectType.LEAF);
                    Vector3 position = pos.position;
                    position.z = 0f;
                    leaf.transform.position = position;
                }
            }

            yield return new WaitForSeconds(delay + Random.Range(-delayRandomness, delayRandomness));
        }
    }
}
