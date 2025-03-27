using System;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{

    public float speed = 5;
    public float jumpUp = 5;
    public float power = 5;
    public Vector3 direction;
    public GameObject slash;

    public GameObject Shadow1;
    List<GameObject> sh = new List<GameObject>();
    public GameObject hit_lazer;
    public GameObject Jdust;

    bool bJump = false;
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

            //Shadowflip
            for(int i =0; i<sh.Count; i++)
            {
                sh[i].GetComponent<SpriteRenderer>().flipX = sprit.flipX;
            }
        }
        else if(direction.x > 0)
        {
            sprit.flipX = false;
            ani.SetBool("Run", true);
            //Shadowflip
            for (int i = 0; i < sh.Count; i++)
            {
                sh[i].GetComponent<SpriteRenderer>().flipX = sprit.flipX;
            }

        }
        else if (direction.x == 0)
        {
            ani.SetBool("Run", false);

            for (int i = 0; i < sh.Count; i++)
            {
                Destroy(sh[i]); //게임오브젝트지우기
                sh.RemoveAt(i); //게임오브젝트 관리하는 리스트지우기
            }

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
                JumpDust();
            }
        }
    }

    void Attack()
    {
        if (Input.GetMouseButtonDown(0))
        {
            ani.SetTrigger("Attack");
            Instantiate(hit_lazer, transform.position, Quaternion.identity);

        }
    }
 
    public void AttSlash()
    {
        //플레이어 오른쪽
        if(sprit.flipX == false)
        {
            rigid.AddForce(Vector2.right * power, ForceMode2D.Impulse);
            //플레이어 오른쪽
            GameObject go = Instantiate(slash, transform.position, Quaternion.identity);
            // go.GetComponent<SpriteRenderer>().flipX = sprit.flipX;
        }
        else
        {

            rigid.AddForce(Vector2.left * power, ForceMode2D.Impulse);
            //왼쪽
            GameObject go = Instantiate(slash, transform.position, Quaternion.identity);
            // go.GetComponent<SpriteRenderer>().flipX = sprit.flipX;
        }   

    }

    public void RunShadow()
    {
        if (sh.Count < 6)
        {
           GameObject go = Instantiate(Shadow1, transform.position, Quaternion.identity);
            go.GetComponent<Shadow>().TwSpeed = 10 - sh.Count;
            sh.Add(go);
        }
    }
    
    public void RandDust(GameObject dust)
    {
        Instantiate(dust, transform.position, Quaternion.identity);
    }

    public void JumpDust()
    {
     Instantiate(Jdust, transform.position, Quaternion.identity);
    }
}
