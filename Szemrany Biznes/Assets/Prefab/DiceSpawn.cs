using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Unity.VisualScripting;

public class DiceSpawn : NetworkBehaviour
{
    
    //Gameobjects
    public GameObject Dice;
    public GameObject CheckZone;

    [SerializeField] Vector3 offsetPosition, offsetRotation;
    [SerializeField] int backOrForward;
    [SerializeField] private float rollForce = 20;
    Coroutine _velocityCoroutine;
    bool isNotMoving = false;
    public int diceNumber;
    public Vector3 diceVelocity;

    void Start()
    {
        Collider collider = CheckZone.GetComponent<Collider>();
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
    }
    void rollTheDice()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                GameObject prefabInstance = Instantiate(Dice, offsetPosition, Quaternion.Euler(offsetRotation));
                Rigidbody rb = prefabInstance.GetComponent<Rigidbody>();
                rb.AddForce(Vector3.forward * rollForce, ForceMode.Impulse);
                rb.AddForce(Vector3.down * rollForce, ForceMode.Impulse);
            if (isNotMoving == false)
            {
                _velocityCoroutine = StartCoroutine(CheckVelocity(rb));
            }
            else {
                StopCoroutine(_velocityCoroutine);
            }
            }           

        }
    
    }


