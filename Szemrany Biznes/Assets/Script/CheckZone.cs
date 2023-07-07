using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class CheckZone : NetworkBehaviour
{

    public int diceNumber = 0;
    public Collider diceSide;
    public static CheckZone Instance { get; private set; }

    void Awake()
    {
        Instance = this;
    }

    private void OnTriggerEnter(Collider other)
    {
        diceSide = other;
    }
    public void test(GameObject prefabGameObject,int playerIndex)
    {
        switch (diceSide.name)
        {
            case "Number1":
                diceNumber = 6;
                //Debug.Log("Wylosowano liczbê: " + diceNumber);
                break;
            case "Number2":
                diceNumber = 5;
                //Debug.Log("Wylosowano liczbê: " + diceNumber);
                break;
            case "Number3":
                diceNumber = 4;
                //Debug.Log("Wylosowano liczbê: " + diceNumber);
                break;
            case "Number4":
                diceNumber = 3;
                //Debug.Log("Wylosowano liczbê: " + diceNumber);
                break;
            case "Number5":
                diceNumber = 2;
                //Debug.Log("Wylosowano liczbê: " + diceNumber);
                break;
            case "Number6":
                diceNumber = 1;
                //Debug.Log("Wylosowano liczbê: " + diceNumber);
                break;
            default:
                diceNumber = 0;
                break;
        }
        ClientRpcParams clientRpcParams = new ClientRpcParams
        {
            Send = new ClientRpcSendParams
            {
                TargetClientIds = new ulong[] { (ulong)playerIndex }
            }
        };
        GameUIScript.Instance.OnDiceNumberReturnClientRpc(diceNumber,clientRpcParams);
        Destroy(prefabGameObject);
        

    }

}
