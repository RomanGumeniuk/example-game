using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class DiceSpawn : NetworkBehaviour
{
    // Start is called before the first frame update
    //Gameobjects
    public GameObject Dice;

    [SerializeField] Vector3 offset;
    [SerializeField] int backOrForward;
    [SerializeField] private float rollForce = 10;
    [SerializeField] private float speed = 150;

    // Tables declaration and 
    [SerializeField] Vector3 cords;
    [SerializeField] private Vector3[] rollNumberTable1;
    [SerializeField] private Vector3[] rollNumberTable2;
    [SerializeField] private Vector3[] rollNumberTable3;
    [SerializeField] private Vector3[] rollNumberTable4;
    [SerializeField] private Vector3[] rollNumberTable5;
    [SerializeField] private Vector3[] rollNumberTable6;

    
    void Start()
    {

        rollNumberTable1 = new Vector3[6]; // We need 6 elements for numbers 1 to 6
        rollNumberTable1[0] = new Vector3(4, 8, 7); // Number 1
        rollNumberTable1[1] = new Vector3(2, 8, 0); // Number 2
        rollNumberTable1[2] = new Vector3(-2, 8, 0); // Number 3

        rollNumberTable2 = new Vector3[6]; // We need 6 elements for numbers 1 to 6
        rollNumberTable2[0] = new Vector3(0, 8, 0); // Number 1
        rollNumberTable2[1] = new Vector3(2, 8, 0); // Number 2
        rollNumberTable2[2] = new Vector3(-2, 8, 0); // Number 3

        rollNumberTable3 = new Vector3[6];
        rollNumberTable3[0] = new Vector3(0, 8, 2);
        rollNumberTable3[1] = new Vector3(2, 8, 2);
        rollNumberTable3[2] = new Vector3(-2, 8, 2);

        rollNumberTable4 = new Vector3[6];
        rollNumberTable4[0] = new Vector3(-1.5f, 8f, -9f);
        rollNumberTable4[1] = new Vector3(2, 8, 2);
        rollNumberTable4[2] = new Vector3(-2, 8, 2);

        rollNumberTable5 = new Vector3[6];
        rollNumberTable5[0] = new Vector3(0, 8, 2);
        rollNumberTable5[1] = new Vector3(2, 8, 2);
        rollNumberTable5[2] = new Vector3(-2, 8, 2);

        rollNumberTable6 = new Vector3[6];
        rollNumberTable6[0] = new Vector3(0, 8, 2);
        rollNumberTable6[1] = new Vector3(2, 8, 2);
        rollNumberTable6[2] = new Vector3(-2, 8, 2);

    }

    // Update is called once per frame
    void Update()
    {
        rollTheDice();
        


    }
    void rollTheDice()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {

            int diceNumber = Random.Range(1, 7);
            int i = Random.Range(0, 3);
            switch (diceNumber)
            {
                case 1:
                    cords = rollNumberTable1[i];
                    Debug.Log("Wylosowa³eœ numer kostki " + diceNumber + " z animacj¹ nr. " + i + " a xyz to " + cords);
                    break;
                case 2:
                    cords = rollNumberTable2[i];
                    Debug.Log("Wylosowa³eœ numer kostki " + diceNumber + " z animacj¹ nr. " + i + " a xyz to " + cords);
                    break;
                case 3:
                    cords = rollNumberTable3[i];
                    Debug.Log("Wylosowa³eœ numer kostki " + diceNumber + " z animacj¹ nr. " + i + " a xyz to " + cords);
                    break;
                case 4:
                    cords = rollNumberTable4[i];
                    Debug.Log("Wylosowa³eœ numer kostki " + diceNumber + " z animacj¹ nr. " + i + " a xyz to " + cords);
                    break;
                case 5:
                    cords = rollNumberTable5[i];
                    Debug.Log("Wylosowa³eœ numer kostki " + diceNumber + " z animacj¹ nr. " + i + " a xyz to " + cords);
                    break;
                case 6:
                    cords = rollNumberTable6[i];
                    Debug.Log("Wylosowa³eœ numer kostki " + diceNumber + " z animacj¹ nr. " + i + " a xyz to " + cords);
                    break;
                default:
                    Debug.Log("Somethings wrong");
                    break;
            }

            GameObject prefabInstance = Instantiate(Dice, offset, Quaternion.Euler(0, 0, 30f));
            Rigidbody rb = prefabInstance.GetComponent<Rigidbody>();
            prefabInstance.transform.Rotate(Vector3.up, speed * Time.deltaTime);


            //if (diceNumber % 2 == 0)
            
            if (backOrForward == 1) //parzysta
            {
                
                rb.AddForce(Vector3.forward * rollForce, ForceMode.Impulse);
                rb.AddForce(Vector3.down * rollForce, ForceMode.Impulse);
               


            }
            else //nieparzysta 0
            {
                rb.AddForce(Vector3.back * rollForce, ForceMode.Impulse);
                rb.AddForce(Vector3.down * rollForce, ForceMode.Impulse);
                
            }
        }
    }
}
