using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckZone : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] public GameObject GameLogic;
    public int diceNumber;
    public bool isNotMoving;
    Vector3 diceVelocity;
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        diceVelocity = GameLogic.diceVelocity;
    }
    void OnTriggerStay(Collider col)
    {
        if (diceVelocity.x == 0f && diceVelocity.y == 0f && diceVelocity.z == 0f)
        {
            isNotMoving = true;
            switch (col.gameObject.name)
            {
                case "Number1":
                    diceNumber = 6;
                    Debug.Log("Wylosowano liczb�: " + diceNumber);
                    break;
                case "Number2":
                    diceNumber = 5;
                    Debug.Log("Wylosowano liczb�: " + diceNumber);
                    break;
                case "Number3":
                    diceNumber = 4;
                    Debug.Log("Wylosowano liczb�: " + diceNumber);
                    break;
                case "Number4":
                    diceNumber = 3;
                    Debug.Log("Wylosowano liczb�: " + diceNumber);
                    break;
                case "Number5":
                    diceNumber = 2;
                    Debug.Log("Wylosowano liczb�: " + diceNumber);
                    break;
                case "Number6":
                    diceNumber = 1;
                    Debug.Log("Wylosowano liczb�: " + diceNumber);
                    break;
            }


        }
    }
}
    
    
