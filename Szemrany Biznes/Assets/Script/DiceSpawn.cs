using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiceSpawn : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject Dice;
    
    public float rollForce = 10;
    private Vector3Int[] rollNumberTable1;
    void Start()
    {
        
        rollNumberTable1 = new Vector3Int[5];
        for (int i = 0; i < 5; i++)
        {
            rollNumberTable1[i] = new Vector3Int();
        }

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
            GameObject prefabInstance = Instantiate(Dice.transform.GetChild(0).gameObject, new Vector3(0, 8.59f, 0), Quaternion.Euler(0, 0, -18.48f));
            Rigidbody rb = prefabInstance.GetComponent<Rigidbody>();
            rb.AddForce(Vector3.right * rollForce, ForceMode.Impulse);
            rb.AddForce(Vector3.back * rollForce, ForceMode.Impulse);
            // diceNumber = Random.Range(1, numberOfSides + 1);
        }
    }
}
