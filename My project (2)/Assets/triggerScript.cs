using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerScript : MonoBehaviour
{
    public ScoreManagerScript scoreManager;
    // Start is called before the first frame update
    void Start()
    {
        scoreManager = GameObject.FindGameObjectWithTag("Logic").GetComponent<ScoreManagerScript>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 3) { 
        scoreManager.addScore(1);
    }
    }
}
