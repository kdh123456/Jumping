using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class MapleBlock : MonoBehaviour
{
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            this.GetComponent<SpriteRenderer>().DOFade(0, .1f);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            this.GetComponent<SpriteRenderer>().DOFade(1, .1f);
        }
    }
}
