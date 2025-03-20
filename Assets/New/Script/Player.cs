using UnityEngine;

public class Player : MonoBehaviour
{

    public float speed = 5;
    public float jumpUp = 1;
    public Vector3 direction;

    Animator ani;
    Rigidbody2D rigid;
    SpriteRenderer sprit;

    void Start()
    {
        ani = GetComponent<Animator>();
        rigid = GetComponent<Rigidbody2D>();
        sprit = GetComponent<SpriteRenderer>();
        direction = Vector2.zero;
    }

    void Update()
    {
        KeyInput();
        Move();
    }

    void KeyInput()
    {
        direction.x = Input.GetAxisRaw("Horizontal");

        if (direction.x < 0)
        {
            sprit.flipX = true;
            ani.SetBool("Run", true);
        }
        else if(direction.x > 0)
        {
            sprit.flipX = false;
            ani.SetBool("Run", true);
        }
        else if (direction.x == 0)
        {
            ani.SetBool("Run", false);
        }
    }

    public void Move()
    {
        transform.position += direction * speed * Time.deltaTime;
    }
}
