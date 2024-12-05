using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;


public class DiceSpawn : NetworkBehaviour
{
    public static DiceSpawn Instance { get; private set; }
    //Gameobjects
    public GameObject Dice;

    [SerializeField] Vector3 offsetPosition, offsetRotation;


    [SerializeField] int backOrForward;
    [SerializeField] private float rollForce = 20;
    public int diceNumber;
    public Vector3 diceVelocity;

    void Awake()
    {
        Instance = this;
    }
    public float RandomNumber(float from, float to)
    {
       return Random.Range(from, to);
    }

    public Vector3 RandomDirectionVertical()
    {
        if (Random.Range(1, 3) == 1) return Vector3.up;
        else return Vector3.down;
    }
    public Vector3 RandomDirectionHorizontal()
    {
        if (Random.Range(1, 3) == 1) return Vector3.forward;
        else return Vector3.back;
    }
    // Update is called once per frame
    IEnumerator CheckVelocity(Rigidbody rb,int playerIndex)
    {
        yield return new WaitForSeconds(2);
        while (true) 
        { 
            diceVelocity = rb.linearVelocity;
            yield return null;
            if (diceVelocity == new Vector3(0,0,0))
            {
                //Debug.Log("ZATRZYMANO KUROTINA KURWE");
                CheckZone.Instance.test(rb.gameObject, playerIndex);
                yield break;
            }
            if(rb.transform.position.y < -2 || rb.transform.position.x < -10 || rb.transform.position.x > 10 || rb.transform.position.z > 10 || rb.transform.position.z < -10)
            {
                Destroy(rb.gameObject);
                RollTheDiceServerRpc(playerIndex);
                yield break;
            }
        }
    }
    [ServerRpc(RequireOwnership = false)]
    public void RollTheDiceServerRpc(int playerIndex)
    {
        offsetPosition = new Vector3(RandomNumber(-3,3), RandomNumber(1,6),RandomNumber(-3,3));
        offsetRotation = new Vector3(RandomNumber(-180, 180), RandomNumber(-180, 180), RandomNumber(-180, 180));
        GameObject prefabInstance = Instantiate(Dice, offsetPosition, Quaternion.Euler(offsetRotation));
        Rigidbody rb = prefabInstance.GetComponent<Rigidbody>();
        prefabInstance.GetComponent<NetworkObject>().Spawn();
        rb.AddForce(RandomDirectionHorizontal() * RandomNumber(rollForce / 1.5f, rollForce * 1.5f), ForceMode.Impulse);
        rb.AddForce(RandomDirectionVertical() * RandomNumber(rollForce / 1.5f, rollForce * 1.5f), ForceMode.Impulse);
        rb.AddTorque(transform.up * RandomNumber(rollForce / 1.5f, rollForce * 1.5f), ForceMode.Impulse);
        StartCoroutine(CheckVelocity(rb,playerIndex));

    }

}


