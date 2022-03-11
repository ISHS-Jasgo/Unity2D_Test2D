using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.tag == "Player")
        {
            if (gameObject.tag == "heal")
            {
                col.transform.Find("Canvas").Find("HP").GetComponent<Image>().fillAmount = 1f;
                Destroy(gameObject);
            }
            else if (gameObject.tag == "coin")
            {
                int coin = int.Parse(GameObject.Find("CoinView").transform.Find("Text").GetComponent<Text>().text.Replace("코인: ", "").Trim());
                coin += 1;
                GameObject.Find("CoinView").transform.Find("Text").GetComponent<Text>().text = "코인: " + coin;
                col.GetComponent<StatScript>().atk = Mathf.FloorToInt(coin/5);
                col.GetComponent<StatScript>().def = Mathf.FloorToInt(coin / 5);
                Destroy(gameObject);
            }
        }
    }
}
