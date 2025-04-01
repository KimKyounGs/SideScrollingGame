using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    [Header("플레이어 속성")]
    public float speed = 5;
    public float jumpUp = 1;
    public float power = 5;
    public Vector3 direction;
    public GameObject slash;

    // 그림자
    public GameObject Shadow1;
    List<GameObject> sh = new List<GameObject>();

    // 히트 이팩트
    public GameObject hit_lazer;

    public GameObject Jdust;

    bool bJump = false;
    Animator ani;
    Rigidbody2D rigid;
    SpriteRenderer sprit;

    public float GROUND_CHECK_DISTANCE = 0.1f;
    //벽점프
    public Transform wallChk;
    public float wallchkDistance;
    public LayerMask wLayer;
    bool isWall;
    public float slidingSpeed;
    public float wallJumpPower;
    public bool isWallJump;
    float isRight = 1;
       

    void Start()
    {
        ani = GetComponent<Animator>();
        rigid = GetComponent<Rigidbody2D>();
        sprit = GetComponent<SpriteRenderer>();
        direction = Vector2.zero;
    }

    void Update()
    {
        if (!isWallJump)
        {
            KeyInput();
            Move();
            Attack();
        }
         //벽인지 체크
        isWall = Physics2D.Raycast(wallChk.position, Vector2.right * isRight, wallchkDistance, wLayer);
        ani.SetBool("Grab", isWall);
        if(isWall)
        {
            isWallJump = false;
            //벽점프상태
            rigid.linearVelocity = new Vector2(rigid.linearVelocityX, rigid.linearVelocityY * slidingSpeed);
            //벽을 잡고있는 상태에서 점프
            if(Input.GetKeyDown(KeyCode.W))
            {
                isWallJump = true;
                //벽점프 먼지

                Invoke("FreezeX", 0.3f);
                //물리
                rigid.linearVelocity = new Vector2(-isRight * wallJumpPower, 0.9f * wallJumpPower);

                sprit.flipX = sprit.flipX == false ? true : false;
                isRight = -isRight;
            }

        }
        Jump();
        
        // 시간 조절 입력 체크 (완쪽 시프트 키를 누르면 슬로우 모션 시작)
        if(Input.GetKeyDown(KeyCode.LeftShift))
        {
            // 포스트프로세싱 화면효과
            TimeController.Instance.SetSlowMotion(true);
        }
    }

    void FixedUpdate()
    {
        Debug.DrawRay(rigid.position, Vector3.down, new Color(0, GROUND_CHECK_DISTANCE, 0));

        //레이캐스트로 땅체크 
        RaycastHit2D rayHit = Physics2D.Raycast(rigid.position, Vector3.down, GROUND_CHECK_DISTANCE, LayerMask.GetMask("Ground"));

        CheckGroundedState(rayHit);
    }

    void CheckGroundedState(RaycastHit2D rayHit)
    {
        bool isGrounded = rayHit.collider != null && rayHit.distance < GROUND_CHECK_DISTANCE;
        
        if (isGrounded)
        {
            ani.SetBool("Jump", false);                
        }
        else
        {
            //떨어지고 있다
            if (!isWall)
            {
                //그냥 떨어지는중
                ani.SetBool("Jump", true);
            }
            else
            {
                //벽타기
                ani.SetBool("Grab", true);
            }
        }   
    }

    void FreezeX()
    {
        isWallJump = false;
    }


    void KeyInput()
    {
        direction.x = Input.GetAxisRaw("Horizontal");

        if (direction.x < 0)
        {
            sprit.flipX = true;
            ani.SetBool("Run", true);

            isRight = -1;

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

            isRight = 1;
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

    void OnTriggerEnter2D(Collider2D other)
    {
        // 보스 씬 진입 포털과 충돌 체크
        if (other.CompareTag("BossScene"))
        {
            SceneManager.LoadScene("BossScene");
        }
    }

}
