using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PotatoScript : MonoBehaviour
{
    public float flapStrength;
    public float deadZone;

    public Rigidbody2D myRigidbody;
    public SpriteRenderer myRenderer;
    public Sprite defaultSprite;
    public Sprite onclickSprite;
    


    

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        

        Movement();

        bool dead = Dying();
    }
    void Movement()
    {
        if (Input.GetKeyDown(KeyCode.Space)
           || Input.GetKeyDown(KeyCode.UpArrow))
        {
            myRigidbody.velocity = Vector2.up * flapStrength;
        }
    }

    bool Dying()
    {
        if (transform.position.x < deadZone)
        { 
            Destroy(gameObject); 
            return true;
        }
        else { return false; }
        
    }
}
