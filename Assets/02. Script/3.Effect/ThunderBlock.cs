using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThunderBlock : MonoBehaviour
{
    //public GameObject thunder;
    public Transform thunderLocation;
    public float thunderTime = 5f;

    private static WaitForSeconds waitForSecondsThunderTime;
    void Start()
    {
        waitForSecondsThunderTime = new WaitForSeconds(thunderTime);

        //?ç­???????ï¦?????????”ï¦«???
        StartCoroutine(GetThunder());
    }

    private IEnumerator GetThunder()
    {
        while (true)
        {
            yield return waitForSecondsThunderTime;
            if (GameManager.Instance.IsGameStart)
            {
                GameObject thunder = ObjectPool.Instance.GetObject(PoolObjectType.THUNDER);
                thunder.transform.position = new Vector2(thunderLocation.position.x, thunderLocation.position.y);
                //EventManager.TriggerEvent("Tunder");
                //EventManager.TriggerEvent("ThunderExplode");
            }
        }
    }
}
