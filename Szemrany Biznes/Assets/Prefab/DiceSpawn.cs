using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Unity.VisualScripting;
//using Mono.Cecil.Cil;

public class DiceSpawn : NetworkBehaviour
{
    public static DiceSpawn Instance { get; private set; }
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

    void Awake()
    {
        Instance = this;
    }
    void Start()
    {
        checkZone = CheckZone.GetComponent<CheckZone>();
        
    }

    // Update is called once per frame
    IEnumerator CheckVelocity(Rigidbody rb)
    {
        yield return new WaitForSeconds(2);
        while (true) { 
        diceVelocity = rb.velocity;
        yield return null;
        if (diceVelocity == new Vector3(0,0,0))
        {
            
            Debug.Log("ZATRZYMANO KUROTINA KURWE");
               StartCoroutine(checkZone.test(rb.transform));
            yield break;
        }
        }
    }
    public void RollTheDice()
        {
                checkZone.isNotMoving = false;
                GameObject prefabInstance = Instantiate(Dice, offsetPosition, Quaternion.Euler(offsetRotation));
                Rigidbody rb = prefabInstance.GetComponent<Rigidbody>();
                rb.AddForce(Vector3.forward * rollForce, ForceMode.Impulse);
                rb.AddForce(Vector3.down * rollForce, ForceMode.Impulse);
                if (checkZone.isNotMoving == false)
                {
                    _velocityCoroutine = StartCoroutine(CheckVelocity(rb));
                }
                else if(checkZone.isNotMoving==true){
                    Debug.Log("Stoisz chyba");
                    
            }
        }
}


