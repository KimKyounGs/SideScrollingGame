using System;
using UnityEngine;

public class Player : MonoBehaviour
{

    public float speed = 5;
    public float jumpUp = 5;
    public Vector3 direction;
    public GameObject slash;

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
        Jump();
        Attack();
    }

    void FixedUpdate()
    {
        Debug.DrawRay(rigid.position, Vector3.down, new Color(0,1,0)); // 캐릭터가 땅에 닿였는지 디버깅

        RaycastHit2D rayHit = Physics2D.Raycast(rigid.position, Vector3.down, 1, LayerMask.GetMask("Ground"));

        if (rigid.linearVelocityY < 0)
        {
            if (rayHit.collider != null)
            {
                if (rayHit.distance < 0.7f)
                {
                    ani.SetBool("Jump", false);
                }
            }
        }
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

    void Jump()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            if (ani.GetBool("Jump") == false)
            {
                rigid.linearVelocity = Vector2.zero;
                rigid.AddForce(new Vector2(0, jumpUp), ForceMode2D.Impulse);
                ani.SetBool("Jump", true);
            }

        }
    }

    void Attack()
    {
        if (Input.GetMouseButtonDown(0))
        {
            ani.SetTrigger("Attack");
        }
    }
 
    public void AttSlash() // 애니메이션 함수로 사용.
    {
        //플레이어 오른쪽
        Instantiate(slash, transform.position, Quaternion.identity);

    }
}
