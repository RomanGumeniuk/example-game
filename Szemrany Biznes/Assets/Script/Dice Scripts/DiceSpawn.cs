using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System.Threading.Tasks;

public class DiceSpawn : NetworkBehaviour
{
    public static DiceSpawn Instance { get; private set; }
    //Gameobjects
    public GameObject SixSideDice;
    public GameObject FourSideDice;

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
    IEnumerator CheckVelocity(Rigidbody rb,int playerIndex,bool movePlayer = true)
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
                CheckZone.Instance.CheckDiceNumber(rb.gameObject, playerIndex);
                yield break;
            }
            if(rb.transform.position.y < -2)
            {
                Destroy(rb.gameObject);
                RollTheDiceServerRpc(playerIndex,1,false, movePlayer);
                yield break;
            }
        }
    }
    [ServerRpc(RequireOwnership = false)]
    public void RollTheDiceServerRpc(int playerIndex,int diceAmount,bool addDiceLeft = true,bool movePlayer=true,DiceType type = DiceType.SixSide)
    {
        currentPlayerIndex = playerIndex;
        diceResults.Clear();
        for (int i=0;i< diceAmount; i++)
        {
            StartCoroutine(CheckVelocity(SpawnDice(type), playerIndex, movePlayer));
        }
        if(addDiceLeft) diceLeft += diceAmount;
        if(!isWaitingForAllDicesToRoll) WaitForAllDiceToRoll(movePlayer);
    }

    private Rigidbody SpawnDice(DiceType diceType)
    {
        GameObject prefab = null;
        switch (diceType)
        { 
            case DiceType.SixSide:
                prefab = SixSideDice; 
                break;
            case DiceType.FourSide:
                prefab = FourSideDice;
                break;
        }

        offsetPosition = new Vector3(RandomNumber(3, 17), RandomNumber(1, 6), RandomNumber(3, 17));
        offsetRotation = new Vector3(RandomNumber(-180, 180), RandomNumber(-180, 180), RandomNumber(-180, 180));
        GameObject prefabInstance = Instantiate(prefab, offsetPosition, Quaternion.Euler(offsetRotation));
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
    List<int> diceResults = new List<int>();
    public void DecreaseDiceLeft(int diceNumber)
    {
        if (diceLeft == 0) Debug.LogWarning("there are no more dices");
        diceResults.Add(diceNumber);
        combineDiceNumber += diceNumber;
        diceLeft--;
        //Debug.Log("Dice decrease" + " "+ diceLeft);
    }

    public async void WaitForAllDiceToRoll(bool movePlayer = true)
    {
        isWaitingForAllDicesToRoll = true;
        while (true)
        {
            await Awaitable.WaitForSecondsAsync(0.1f);
            if (diceLeft == 0) break;
        }
        bool isDoublet = true;
        for(int i=1;i<diceResults.Count;i++)
        {
            if (diceResults[i] != diceResults[0]) isDoublet = false;
        }
        GameLogic.Instance.SetIsDoubletServerRPC(isDoublet);
        ClientRpcParams clientRpcParams = new ClientRpcParams
        {
            Send = new ClientRpcSendParams
            {
                TargetClientIds = new ulong[] { (ulong)currentPlayerIndex }
            }
        };
        PlayerScript.LocalInstance.OnDiceNumberReturnClientRpc(combineDiceNumber,  movePlayer, clientRpcParams);
        combineDiceNumber = 0;
        CheckZone.Instance.DestroyAllDices();
        isWaitingForAllDicesToRoll = false;
    }

}

public enum DiceType
{
    FourSide,
    SixSide
}
