using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnakeBlock : MonoBehaviour
{
    private GameObject snake;
    public GameObject player;
    public float force = 10f;
    private bool snakeOn = false;

    private Animator animator;

    private readonly int hashAttack = Animator.StringToHash("attack");
    public void Start()
    {
        animator = GetComponent<Animator>();

        //snake.SetActive(false);
    }

    private void Update()
    {
        //if (snake.transform.position.y < transform.position.y)
        //{
        //    snake.SetActive(false);
        //    snakeOn = false;
        //}
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //if (collision.CompareTag("Player") && snakeOn == false)
        //{
        //    snake = ObjectPool.Instance.GetObject(PoolObjectType.SNAKE);
        //    //snake.SetActive(true);
        //    snake.transform.position = transform.position + Vector3.up * 1.1f;
        //    snake.GetComponent<Rigidbody2D>().AddForce(Vector2.up * force, ForceMode2D.Impulse);
        //    snakeOn = true;
        //    DebuffManager.Instance.UpdateDown(true);
        //}
        //else if (collision.CompareTag("Snake") && snakeOn == true)
        //{
        //    ObjectPool.Instance.ReturnObject(PoolObjectType.SNAKE, snake);
        //    snakeOn = false;
        //}

        if (collision.CompareTag("Player"))
        {
            animator.SetTrigger(hashAttack);
            DebuffManager.Instance.UpdateDown(true);
        }
    }
}
