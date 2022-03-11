using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AttackScript : MonoBehaviour
{

    public GameObject mob;
    private Animator animator;
    private GameObject col;
    public StatScript stat;

    void Awake()
    {
        animator = mob.GetComponent<Animator>();
        stat = GameObject.FindGameObjectWithTag("Player").GetComponent<StatScript>();
    }

    // Update is called once per frame
    void Update()
    {

    }
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            col = collision.gameObject;
            InvokeRepeating("Attack", 0.1f, 1f);
        }
    }
    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            CancelInvoke();
        }
    }
    void Attack()
    {
        if (col != null)
        {
            if (mob.GetComponent<MobAI>().stun <= 0 && !col.GetComponent<HeroKnight>().m_rolling)
            {
                float damage = 0;
                if (mob.tag == "Wizard")
                {
                    damage = 0.05f - (stat.def * 0.0005f);
                } else if (mob.tag == "Killer")
                {
                    damage = 0.2f - (stat.def * 0.0005f);
                }
                animator.SetTrigger("Attack");
                Invoke("Hurt", 0.25f);
                col.transform.Find("Canvas").Find("HP").GetComponent<Image>().fillAmount -= damage;
                if (col.transform.Find("Canvas").Find("HP").GetComponent<Image>().fillAmount <= 0.00001f)
                {
                    stat.coin = 0;
                    stat.atk = 0;
                    stat.def = 0;
                    GameObject.Find("CoinView").transform.Find("Text").GetComponent<Text>().text = "ÄÚÀÎ: " + stat.coin;
                    GameObject.Find("Panel").transform.Find("RespawnPanel").gameObject.SetActive(true);
                    GameObject.Find("MobManager").GetComponent<MobManager>().mobList.ForEach(m =>
                    {
                        Destroy(m);
                    });
                    GameObject.Find("MobManager").GetComponent<MobManager>().mobList.Clear();
                    GameObject.Find("MobManager").GetComponent<MobManager>().spawn = false;
                    Destroy(col);
                }
            }

        }
    }
    void Hurt()
    {
        if (col != null)
        {
            col.GetComponent<Animator>().SetTrigger("Hurt");
        } 
    }
}
