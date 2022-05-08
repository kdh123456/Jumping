using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnakeBlock : MonoBehaviour
{
    public GameObject snake;
    public GameObject player;
    public float force = 10f;
    private bool snakeOn = false;

    public void Start()
    {
        snake.SetActive(false);
    }

    private void Update()
    {
        if (snake.transform.position.y < transform.position.y)
        {
            snake.SetActive(false);
            snakeOn = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player")&&snakeOn==false)
        {
            snake.SetActive(true);
            snakeOn = true;
            snake.transform.position = transform.position;
            snake.GetComponent<Rigidbody2D>().AddForce(Vector2.up * force, ForceMode2D.Impulse);
        }
    }
}
