using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxScript : MonoBehaviour
{

    public HeroKnight player;
    private SpriteRenderer renderer;
    public Sprite sprite;


    // Start is called before the first frame update
    void Awake()
    {
        renderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void open()
    {
        renderer.sprite = sprite;
        Invoke("Destroy", 1f);
    }
    void Destroy()
    {
        Destroy(gameObject);
    }
}
