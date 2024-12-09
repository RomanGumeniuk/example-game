using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class CheckZone : NetworkBehaviour
{

    public int diceNumber = 0;
    public static CheckZone Instance { get; private set; }

    void Awake()
    {
        Instance = this;
    }


    public void test(GameObject prefabGameObject,int playerIndex)
    {
        string name="";
        float min = 1;
        foreach(Transform child in prefabGameObject.transform)
        {
            float y = child.GetComponent<SphereCollider>().ClosestPoint(new Vector3(prefabGameObject.transform.position.x, 0, prefabGameObject.transform.position.z)).y;
            if (y < min)
            {
                min = y;
                name = child.name;
                //Debug.Log(child.GetComponent<SphereCollider>().ClosestPoint(new Vector3(prefabGameObject.transform.position.x,0, prefabGameObject.transform.position.z)) + " " +child.name);
            }
        }

        if (name == "")
        {
            Destroy(prefabGameObject);
            DiceSpawn.Instance.RollTheDiceServerRpc(playerIndex,1,false);
            return;
        }
        switch (name)
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
                Destroy(prefabGameObject);
                DiceSpawn.Instance.RollTheDiceServerRpc(playerIndex,1,false);

                return;
        }
        dicesToDestroy.Add(prefabGameObject);
        DiceSpawn.Instance.DecreaseDiceLeft(diceNumber);
    }

    private List<GameObject> dicesToDestroy = new List<GameObject>();

    public void DestroyAllDices()
    {
        foreach(GameObject dice in dicesToDestroy)
        { Destroy(dice); }
    }

}
