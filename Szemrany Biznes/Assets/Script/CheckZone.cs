using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckZone : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] public GameObject GameLogic;
    DiceSpawn diceSpawn;
    public int diceNumber = 0;
    public bool isNotMoving;
    public Collider diceSide;
    void Start()
    {
        diceSpawn = GameLogic.GetComponent<DiceSpawn>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter(Collider other)
    {
        diceSide = other;
    }
    public IEnumerator test(Transform prefab)
    {
        
        if (diceSpawn.diceVelocity.x == 0f && diceSpawn.diceVelocity.y == 0f && diceSpawn.diceVelocity.z == 0f)
        {
            if (isNotMoving == true)
            {
                yield break;
            }
                isNotMoving = true;
            
                switch (diceSide.name)
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
            GameUIScript.Instance.OnDiceNumberReturn(diceNumber);
            }
            


        }
        
        
    }

    
    
