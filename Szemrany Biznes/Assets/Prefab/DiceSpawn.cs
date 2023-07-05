using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class DiceSpawn : NetworkBehaviour
{
    // Start is called before the first frame update
    //Gameobjects
    public GameObject Dice;

    [SerializeField] Vector3 offsetPosition,offsetRotation;
    [SerializeField] int backOrForward;
    [SerializeField] private float rollForce = 10;
    [SerializeField] private float speed = 150;

    // Tables declaration and 
    /*[SerializeField]*/ Vector3 cords;
    /*[SerializeField]*/ private Vector3[] rollNumberTable1;
   /* [SerializeField]*/ private Vector3[] rollNumberTable2;
    /*[SerializeField]*/ private Vector3[] rollNumberTable3;
    /*[SerializeField]*/ private Vector3[] rollNumberTable4;
    /*[SerializeField]*/ private Vector3[] rollNumberTable5;
    /*[SerializeField]*/ private Vector3[] rollNumberTable6;

    
    void Start()
    {
        /*-------------------------TABLES FOR ODD NUMBERS--------------------------------*/

        //Table number 1 Status : Done
        rollNumberTable1 = new Vector3[6]; // We need 6 elements for animation 1 to 6
        rollNumberTable1[0] = new Vector3(-3, 12, -8); // Animation 1.1 Status: Done
        rollNumberTable1[1] = new Vector3(0, 16, 11); // Animation 1.2 Status: Done
        rollNumberTable1[2] = new Vector3(-0.5f, 12.5f, -10); // Animation 1.3 Status: Done
        //add more...


        //Table number 3 Status : Done
        rollNumberTable3 = new Vector3[6];
        rollNumberTable3[0] = new Vector3(0, 11, -8);// Animation 3.1 Status: Done
        rollNumberTable3[1] = new Vector3(3, 13, -9);// Animation 3.2 Status: Done
        rollNumberTable3[2] = new Vector3(0, 14, -9);// Animation 3.3 Status: Done
        rollNumberTable3[3] = new Vector3(1, 16, -10);// Animation 3.4 Status: Done
        //add more...

        //Table number 5 Status : Done
        rollNumberTable5 = new Vector3[6];
        rollNumberTable5[0] = new Vector3(-1, 12, -11);// Animation 5.1 Status: False
        rollNumberTable5[1] = new Vector3(2, 8, 2);// Animation 5.2 Status: Done
        rollNumberTable5[2] = new Vector3(2, 21, -15);// Animation 5.3 Status: Done
        //add more...

        /*-------------------------TABLES FOR EVEN NUMBERS--------------------------------*/
        //Table number 2 Status : In progress
        rollNumberTable2 = new Vector3[6]; // We need 6 elements for animations 1 to 6
        rollNumberTable2[0] = new Vector3(-0.5f, 33, 20); // Animation 2.1 Status: Done
        rollNumberTable2[1] = new Vector3(2, 8, 0); // Animation 2.2 Status: False
        rollNumberTable2[2] = new Vector3(-2, 8, 0); // Animation 2.3 Status: False
        //add more...


        //Table number 4 Status : Done
        rollNumberTable4 = new Vector3[6];
        rollNumberTable4[0] = new Vector3(2, 19, 14); //Animation 4.1 Status: Done
        rollNumberTable4[1] = new Vector3(1, 15, 12); //Animation 4.2 Status: Done
        rollNumberTable4[2] = new Vector3(-0.5f, 17, 12); //Animation 4.3 Status: Done
        //add more...


        //Table number 6
        rollNumberTable6 = new Vector3[6];
        rollNumberTable6[0] = new Vector3(0, 8, 2); //Animation 6.1 Status: False
        rollNumberTable6[1] = new Vector3(2, 8, 2); //Animation 6.1 Status: False
        rollNumberTable6[2] = new Vector3(-2, 8, 2);//Animation 6.1 Status: False
        //add more...
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

            GameObject prefabInstance = Instantiate(Dice, cords, Quaternion.Euler(offsetRotation));
            Rigidbody rb = prefabInstance.GetComponent<Rigidbody>();
            prefabInstance.transform.Rotate(Vector3.up, speed * Time.deltaTime);


            //if (backOrForward == 1) /jesli jest nieparzysta

            if (diceNumber % 2 == 1) //nieparzysta
            {
                
                rb.AddForce(Vector3.forward * rollForce, ForceMode.Impulse);
                rb.AddForce(Vector3.down * rollForce, ForceMode.Impulse);
               


            }
            else //parzysta 0
            {
                rb.AddForce(Vector3.back * rollForce, ForceMode.Impulse);
                rb.AddForce(Vector3.down * rollForce, ForceMode.Impulse);
                
            }
        }
    }
}
