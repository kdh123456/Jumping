using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeafCreater : MonoBehaviour
{
    public Transform[] createPoints;

    public float delay = 5f;
    public float delayRandomness;

    private static WaitForSeconds waitForSeconds;

    private Coroutine leafCoroutine;

    private void Start()
    {
        waitForSeconds = new WaitForSeconds(delay);

        leafCoroutine = StartCoroutine(SpawnLeaf());
    }

    private IEnumerator SpawnLeaf()
    {
        yield return waitForSeconds;

        while (true)
        {
            foreach (Transform pos in createPoints)
            {
                GameObject leaf = ObjectPool.Instance.GetObject(PoolObjectType.LEAF); // KeyNotFoundException Bug
                leaf.transform.position = pos.position;
                leaf.GetComponent<ItemEffect>().CreateEffect();
            }

            yield return waitForSeconds;
        }
    }
}
