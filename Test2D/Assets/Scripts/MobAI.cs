using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MobAI : MonoBehaviour
{
    
    public GameObject follow;
    public Animator animator;
    private Rigidbody2D rigid;
    private SpriteRenderer renderer;
    public float knockback = 0;
    private GameObject col;
    public int stun = 0;
    // Start is called before the first frame update
    void Awake()
    {
        animator = GetComponent<Animator>();
        rigid = GetComponent<Rigidbody2D>();
        renderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (follow != null)
        {
            if (knockback <= 0)
            {
                
                Move();
                Vector2 vec = new Vector2();
                int x = rigid.position.x > follow.transform.position.x ? -1 : 1;
                vec.x = x;
                vec.y = rigid.velocity.y;
                rigid.velocity = vec;
                renderer.flipX = x == -1;
            } else
            {
                rigid.velocity = new Vector2(0, rigid.velocity.y);
                Vector2 vec = new Vector2();
                int x = rigid.position.x > follow.transform.position.x ? 10 : -10;
                vec.x = x;
                vec.y = rigid.velocity.y;
                rigid.velocity = vec;
                knockback--;
            }
        } else
        {
            Stop();
        }
    }

    void Move()
    {
        animator.SetBool("isWalking", true);
    }
    void Stop()
    {
        animator.SetBool("isWalking", false);
    }
}
