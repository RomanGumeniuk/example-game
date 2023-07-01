using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarrotSpawnScript : MonoBehaviour
{
    public GameObject Carrot;
    public float spawnRate = 20;
    public float heightOffset;
    private float timer = 0;
    // Start is called before the first frame update
    void Start()
    {
        spawnCarrot();  
    }

    // Update is called once per frame
    void Update()
    {
        if (timer < spawnRate)
        {
            timer = timer + Time.deltaTime;
        }
        else {
            spawnCarrot();
            timer = 0;
    }
    }
    void spawnCarrot()
    {
        float lowestPoint = transform.position.y - heightOffset;
        float highestPoint = transform.position.y + heightOffset;
        Instantiate(Carrot, new Vector3(transform.position.x,Random.Range(lowestPoint,highestPoint),0), transform.rotation);
    }
}
