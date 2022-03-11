using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnMob : MonoBehaviour
{

    public GameObject mob;
    public Vector2 location;
    public float period;
    public float delay;

    void Awake()
    {
        InvokeRepeating("SpawnMonster", delay, period);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void SpawnMonster()
    {
        if (GameObject.Find("MobManager").GetComponent<MobManager>().spawn)
        {
            GameObject g = Instantiate(mob, new Vector3(location.x, location.y, 0), Quaternion.identity);
            GameObject.Find("MobManager").GetComponent<MobManager>().mobList.Add(g);
        }
    }
}
