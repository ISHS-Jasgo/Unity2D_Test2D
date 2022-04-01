using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class HeroKnight : MonoBehaviour
{

    [SerializeField] float m_speed = 4.0f;
    [SerializeField] float m_jumpForce = 7.5f;
    [SerializeField] float m_rollForce = 6.0f;
    [SerializeField] bool m_noBlood = false;

    private Animator m_animator;
    private Rigidbody2D m_body2d;
    private Sensor_HeroKnight m_groundSensor;
    private SpriteRenderer renderer;
    private bool m_grounded = false;
    public bool m_rolling = false;
    private int m_facingDirection = 1;
    private int m_currentAttack = 0;
    private float m_timeSinceAttack = 0.0f;
    private float m_delayToIdle = 0.0f;
    private float m_rollDuration = 0.5f;
    private float m_rollCurrentTime;
    private GameObject col;
    public GameObject heal;
    public GameObject coin;
    public StatScript stat;
    public bool isRiding;
    public bool canOpen = true;
    public BoxScript box;

    // Use this for initialization
    void Start()
    {
        renderer = GetComponent<SpriteRenderer>();
        m_animator = GetComponent<Animator>();
        m_body2d = GetComponent<Rigidbody2D>();
        m_groundSensor = transform.Find("GroundSensor").GetComponent<Sensor_HeroKnight>();
        stat = GetComponent<StatScript>();
    }

    // Update is called once per frame
    void Update()
    {
        m_timeSinceAttack += Time.deltaTime;

        if (m_rolling)
            m_rollCurrentTime += Time.deltaTime;

        if (m_rollCurrentTime > m_rollDuration)
        {
            m_rollCurrentTime = 0f;
            m_rolling = false;
        }

        if (!m_grounded && m_groundSensor.State())
        {
            m_grounded = true;
            m_animator.SetBool("Grounded", m_grounded);
        }

        if (m_grounded && !m_groundSensor.State())
        {
            m_grounded = false;
            m_animator.SetBool("Grounded", m_grounded);
        }

        float inputX = Input.GetAxis("Horizontal");

        if (inputX > 0)
        {
            GetComponent<SpriteRenderer>().flipX = false;
            m_facingDirection = 1;
        }

        else if (inputX < 0)
        {
            GetComponent<SpriteRenderer>().flipX = true;
            m_facingDirection = -1;
        }

        if (!m_rolling)
            m_body2d.velocity = new Vector2(inputX * m_speed, m_body2d.velocity.y);

        m_animator.SetFloat("AirSpeedY", m_body2d.velocity.y);

        if (Input.GetKeyDown("e") && !m_rolling)
        {
            box.open();
        }

        else if (Input.GetKeyDown("q") && !m_rolling)
            m_animator.SetTrigger("Hurt");

        else if (Input.GetMouseButtonDown(0) && m_timeSinceAttack > 0.25f && !m_rolling)
        {
            m_currentAttack++;

            if (m_currentAttack > 3)
                m_currentAttack = 1;

            if (m_timeSinceAttack > 1.0f)
                m_currentAttack = 1;

            m_animator.SetTrigger("Attack" + m_currentAttack);
            Attack();
            m_timeSinceAttack = 0.0f;
        }

        else if (Input.GetMouseButtonDown(1) && !m_rolling)
        {
            m_animator.SetTrigger("Block");
            m_animator.SetBool("IdleBlock", true);
        }

        else if (Input.GetMouseButtonUp(1))
            m_animator.SetBool("IdleBlock", false);

        else if (Input.GetKeyDown("left shift") && !m_rolling)
        {
            m_rolling = true;
            m_animator.SetTrigger("Roll");
            m_body2d.velocity = new Vector2(m_facingDirection * m_rollForce, m_body2d.velocity.y);
        }


        else if (Input.GetKeyDown("space") && m_grounded && !m_rolling)
        {
            m_animator.SetTrigger("Jump");
            m_grounded = false;
            m_animator.SetBool("Grounded", m_grounded);
            m_body2d.velocity = new Vector2(m_body2d.velocity.x, m_jumpForce);
            m_groundSensor.Disable(0.2f);
        }
        else if (Input.GetAxis("Vertical") != 0)
        {
            if (isRiding)
            {
                float v = Input.GetAxis("Vertical");
                m_body2d.velocity = new Vector2(m_body2d.velocity.x, m_speed * v);
            }
        }

        else if (Mathf.Abs(inputX) > Mathf.Epsilon)
        {
            m_delayToIdle = 0.05f;
            m_animator.SetInteger("AnimState", 1);
        }

        else
        {
            m_delayToIdle -= Time.deltaTime;
            if (m_delayToIdle < 0)
                m_animator.SetInteger("AnimState", 0);
        }
    }
    void Attack()
    {
        int layerMask = 1 << 2;
        layerMask = ~layerMask;

        Vector2 pos = m_body2d.position;
        pos.Set(renderer.flipX ? -0.38f + pos.x : 0.38f + pos.x, pos.y + 0.5f);
        RaycastHit2D ray = Physics2D.Raycast(pos, new Vector3(renderer.flipX ? -1 : 1, 0, 0), 2, layerMask);
        if (ray.collider != null)
        {
            if (ray.collider.tag == "Slime")
            {
                CancelInvoke();
                col = ray.collider.gameObject;
                col.GetComponent<MobAI>().knockback = 3;
                ray.collider.gameObject.GetComponent<SpriteRenderer>().color = new Color(255, 0, 0);
                var img = col.transform.Find("Canvas").Find("HPBar").Find("HP").GetComponent<Image>();
                col.transform.Find("Canvas").Find("HPBar").gameObject.SetActive(true);
                img.fillAmount -= 0.2f + (stat.atk * 0.001f);
                Invoke("Damage", 0.2f);
                Invoke("HideHP", 3f);
            }
            else if (ray.collider.tag == "Wizard"){
                CancelInvoke();
                col = ray.collider.gameObject;
                col.GetComponent<MobAI>().knockback = 1;
                col.GetComponent<MobAI>().stun = 1;
                col.GetComponent<MobAI>().animator.SetTrigger("Hurt");
                var img = col.transform.Find("Canvas").Find("HPBar").Find("HP").GetComponent<Image>();
                col.transform.Find("Canvas").Find("HPBar").gameObject.SetActive(true);
                img.fillAmount -= 0.1f + (stat.atk * 0.001f);
                Invoke("HideHP", 3f);
                Invoke("resetStun", 0.25f);
                if (img.fillAmount <= 0.01f)
                {
                    Instantiate(coin, col.transform.position, Quaternion.identity);
                    GameObject.Find("MobManager").GetComponent<MobManager>().mobList.Remove(col);
                    Destroy(col);
                }
            }
            else if (ray.collider.tag == "Killer")
            {
                CancelInvoke();
                col = ray.collider.gameObject;
                var img = col.transform.Find("Canvas").Find("HPBar").Find("HP").GetComponent<Image>();
                col.transform.Find("Canvas").Find("HPBar").gameObject.SetActive(true);
                img.fillAmount -= 0.01f + (stat.atk * 0.001f);
                Invoke("HideHP", 3f);
                if (img.fillAmount <= 0.001f)
                {
                    Instantiate(coin, col.transform.position, Quaternion.identity);
                    GameObject.Find("MobManager").GetComponent<MobManager>().mobList.Remove(col);
                    Destroy(col);
                }
            }
        }
    }
    void Damage()
    {
        col.GetComponent<SpriteRenderer>().color = new Color(255, 255, 255);
        var img = col.transform.Find("Canvas").Find("HPBar").Find("HP").GetComponent<Image>();
        if (img.fillAmount <= 0.01f)
        {
            Instantiate(heal, col.transform.position, Quaternion.identity);
            GameObject.Find("MobManager").GetComponent<MobManager>().mobList.Remove(col);
            Destroy(col);
        }
    }
    void HideHP()
    {
        if (col != null)
        {
            col.transform.Find("Canvas").Find("HPBar").gameObject.SetActive(false);
        }
    }
    void resetStun()
    {
        if (col != null)
        {
            col.GetComponent<MobAI>().stun = 0;
        }
    }
    public void Respawn()
    {
        GameObject.Find("Panel").transform.Find("RespawnPanel").gameObject.SetActive(false);
        GameObject.Find("MobManager").GetComponent<MobManager>().spawn = true;
        Instantiate(gameObject, new Vector3(0, 0, 0), Quaternion.identity);
    }
}
