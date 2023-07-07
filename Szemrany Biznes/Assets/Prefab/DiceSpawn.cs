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
    // Update is called once per frame
    IEnumerator CheckVelocity(Rigidbody rb,int playerIndex)
    {
        yield return new WaitForSeconds(2);
        while (true) 
        { 
            diceVelocity = rb.velocity;
            yield return null;
            if (diceVelocity == new Vector3(0,0,0))
            {
                //Debug.Log("ZATRZYMANO KUROTINA KURWE");
                CheckZone.Instance.test(rb.gameObject, playerIndex);
                yield break;
            }
        }
    }
    [ServerRpc(RequireOwnership =false)]
    public void RollTheDiceServerRpc(int playerIndex)
    {

        GameObject prefabInstance = Instantiate(Dice, offsetPosition, Quaternion.Euler(offsetRotation));
        Rigidbody rb = prefabInstance.GetComponent<Rigidbody>();
        prefabInstance.GetComponent<NetworkObject>().Spawn();
        rb.AddForce(Vector3.forward * Random.Range(rollForce,rollForce*5), ForceMode.Impulse);
        rb.AddForce(Vector3.down * Random.Range(rollForce, rollForce * 5), ForceMode.Impulse);
        StartCoroutine(CheckVelocity(rb,playerIndex));

    }
}


