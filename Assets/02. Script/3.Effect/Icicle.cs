using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Icicle : MonoBehaviour
{
    Vector2 basicTransform;
    Rigidbody2D rb;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        basicTransform = new Vector2(transform.position.x, transform.position.y);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")==true||collision.CompareTag("Floor")==true)
        {
            rb.velocity = new Vector2(0, 0);
            transform.position = basicTransform;
            gameObject.SetActive(false);
        }
    }

    void Spawn()
    {
        gameObject.SetActive(true);
    }

    private void OnDisable()
    {
        Invoke("Spawn", 3f);
    }
}
