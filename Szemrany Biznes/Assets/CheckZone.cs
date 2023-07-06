using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckZone : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] public GameObject GameLogic;
    DiceSpawn diceSpawn;
    public int diceNumber;
    public bool isNotMoving = false;
    Vector3 diceVelocity;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        diceSpawn = GameLogic.GetComponent<DiceSpawn>();
        diceVelocity = diceSpawn.diceVelocity;
        
    }
    void OnTriggerStay(Collider col)
    {
        if (isNotMoving==true)
        {
            StopCoroutine(test(col));
        }
        StartCoroutine(test(col));
    }
    IEnumerator test(Collider col)
    {
        yield return new WaitForSeconds(1);
        if (diceVelocity.x == 0f && diceVelocity.y == 0f && diceVelocity.z == 0f)
        {
            isNotMoving = true;
            
            switch (col.gameObject.name)
            {
                case "Number1":
                    diceNumber = 6;
                    Debug.Log("Wylosowano liczbê: " + diceNumber);
                    break;
                case "Number2":
                    diceNumber = 5;
                    Debug.Log("Wylosowano liczbê: " + diceNumber);
                    break;
                case "Number3":
                    diceNumber = 4;
                    Debug.Log("Wylosowano liczbê: " + diceNumber);
                    break;
                case "Number4":
                    diceNumber = 3;
                    Debug.Log("Wylosowano liczbê: " + diceNumber);
                    break;
                case "Number5":
                    diceNumber = 2;
                    Debug.Log("Wylosowano liczbê: " + diceNumber);
                    break;
                case "Number6":
                    diceNumber = 1;
                    Debug.Log("Wylosowano liczbê: " + diceNumber);
                    break;
                default:
                    diceNumber = 0;
                    break;
            }


        }
        else if (isNotMoving == true)
        {
            yield break;
        }
    }
}
    
    
