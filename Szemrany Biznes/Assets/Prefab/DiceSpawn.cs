using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Unity.VisualScripting;
using Mono.Cecil.Cil;

public class DiceSpawn : NetworkBehaviour
{
    
    //Gameobjects
    public GameObject Dice;
    CheckZone checkZone;
    [SerializeField] GameObject CheckZone;

    [SerializeField] Vector3 offsetPosition, offsetRotation;
    [SerializeField] int backOrForward;
    [SerializeField] private float rollForce = 20;
    Coroutine _velocityCoroutine;
    public int diceNumber;
    public Vector3 diceVelocity;

    void Start()
    {
        checkZone = CheckZone.GetComponent<CheckZone>();
        Debug.Log(checkZone.isNotMoving);
        
    }

    // Update is called once per frame
    void Update()
    {
        rollTheDice();
        

    }
    IEnumerator CheckVelocity(Rigidbody rb)
    {
        yield return null;
        diceVelocity = rb.velocity;
        yield return null;
        Debug.Log(diceVelocity);
        if (checkZone.isNotMoving == true)
        {
            Debug.Log("ZATRZYMANO KUROTINA KURWE");
            yield break;
            
        }
    }
    void rollTheDice()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                GameObject prefabInstance = Instantiate(Dice, offsetPosition, Quaternion.Euler(offsetRotation));
                Rigidbody rb = prefabInstance.GetComponent<Rigidbody>();
                rb.AddForce(Vector3.forward * rollForce, ForceMode.Impulse);
                rb.AddForce(Vector3.down * rollForce, ForceMode.Impulse);
            if (checkZone.isNotMoving == false)
            {
                _velocityCoroutine = StartCoroutine(CheckVelocity(rb));
            }
            else {
                Debug.Log("Stoisz chyba");
            }
            }           

        }
    
    }


