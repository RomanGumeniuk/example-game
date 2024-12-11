using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System.Threading.Tasks;

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
            diceVelocity = rb.angularVelocity;
            yield return new WaitForSeconds(0.1f);
            if (diceVelocity == Vector3.zero /*Mathf.Abs(diceVelocity.x)<0.001f && Mathf.Abs(diceVelocity.y)<0.001f && Mathf.Abs(diceVelocity.z) <0.001f*/)
            {
                Vector3 position = rb.transform.position;
                //Debug.Log("ZATRZYMANO KUROTINA KURWE");
                yield return new WaitForSeconds(0.1f);
                if (position != rb.transform.position) continue;
                CheckZone.Instance.test(rb.gameObject, playerIndex);
                yield break;
            }
            if(rb.transform.position.y < -2)
            {
                Destroy(rb.gameObject);
                RollTheDiceServerRpc(playerIndex,1,false);
                yield break;
            }
        }
    }
    [ServerRpc(RequireOwnership = false)]
    public void RollTheDiceServerRpc(int playerIndex,int diceAmount,bool addDiceLeft = true)
    {
        currentPlayerIndex = playerIndex;
        for(int i=0;i< diceAmount; i++)
        {
            StartCoroutine(CheckVelocity(SpawnDice(), playerIndex));
        }
        if(addDiceLeft) diceLeft += diceAmount;
        if(!isWaitingForAllDicesToRoll)WaitForAllDiceToRoll();
    }

    private Rigidbody SpawnDice()
    {
        offsetPosition = new Vector3(RandomNumber(-3, 3), RandomNumber(1, 6), RandomNumber(-3, 3));
        offsetRotation = new Vector3(RandomNumber(-180, 180), RandomNumber(-180, 180), RandomNumber(-180, 180));
        GameObject prefabInstance = Instantiate(Dice, offsetPosition, Quaternion.Euler(offsetRotation));
        Rigidbody rb = prefabInstance.GetComponent<Rigidbody>();
        prefabInstance.GetComponent<NetworkObject>().Spawn();
        rb.AddForce(RandomDirectionHorizontal() * RandomNumber(rollForce / 1.5f, rollForce * 1.5f), ForceMode.Impulse);
        rb.AddForce(RandomDirectionVertical() * RandomNumber(rollForce / 1.5f, rollForce * 1.5f), ForceMode.Impulse);
        rb.AddTorque(transform.up * RandomNumber(rollForce / 1.5f, rollForce * 1.5f), ForceMode.Impulse);
        return rb;
    }





    private int diceLeft = 0;
    private int combineDiceNumber = 0;
    private int currentPlayerIndex;
    bool isWaitingForAllDicesToRoll = false;
    public void DecreaseDiceLeft(int diceNumber)
    {
        if (diceLeft == 0) Debug.LogWarning("there are no more dices");
        combineDiceNumber += diceNumber;
        diceLeft--;
        //Debug.Log("Dice decrease" + " "+ diceLeft);
    }

    public async Task WaitForAllDiceToRoll()
    {
        isWaitingForAllDicesToRoll = true;
        while (true)
        {
            await Awaitable.WaitForSecondsAsync(0.1f);
            if (diceLeft == 0) break;
        }
        ClientRpcParams clientRpcParams = new ClientRpcParams
        {
            Send = new ClientRpcSendParams
            {
                TargetClientIds = new ulong[] { (ulong)currentPlayerIndex }
            }
        };
        PlayerScript.LocalInstance.OnDiceNumberReturnClientRpc(combineDiceNumber, clientRpcParams);
        combineDiceNumber = 0;
        CheckZone.Instance.DestroyAllDices();
        isWaitingForAllDicesToRoll = false;
    }

}


