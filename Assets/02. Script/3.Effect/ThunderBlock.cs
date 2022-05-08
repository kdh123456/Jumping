using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThunderBlock : MonoBehaviour
{
    public GameObject thunder;
    public Transform thunderLocation;
    public float thunderTime = 5f;
    // Start is called before the first frame update
    void Start()
    {
        //버그 땜에 잠시 끔 
        //StartCoroutine(GetThunder());
    }

    private IEnumerator GetThunder()
    {
        while (true)
        {
            yield return new WaitForSeconds(thunderTime);
            if (GameManager.Instance.IsGameStart)
            {
                thunder.transform.position = new Vector2(thunderLocation.position.x, thunderLocation.position.y);
                //EventManager.TriggerEvent("Tunder");
                //EventManager.TriggerEvent("ThunderExplode");
                thunder.SetActive(true);
            }
        }
    }
}
