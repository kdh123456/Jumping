using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThunderBlock : MonoBehaviour
{
    //public GameObject thunder;
    public Transform thunderLocation;
    public float thunderTime = 5f;
    void Start()
    {
        //?•ê³Œ?????ë½?“  ??«ë¡«ë»???
        StartCoroutine(GetThunder());
    }

    private IEnumerator GetThunder()
    {
        while (true)
        {
            yield return new WaitForSeconds(thunderTime);
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
