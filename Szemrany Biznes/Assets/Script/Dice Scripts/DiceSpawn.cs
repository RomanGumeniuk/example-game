using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System.Threading.Tasks;
using System.Linq;
using Unity.VisualScripting;

public class DiceSpawn : NetworkBehaviour
{
    public static DiceSpawn Instance { get; private set; }
    //Gameobjects
    public GameObject EightSideDice;
    public GameObject SixSideDice;
    public GameObject FourSideDice;

    public List<Material> fourSideDiceMaterials = new List<Material>();
    public List<Material> sixSideDiceMaterials = new List<Material>();
    public List<Material> eightSideDiceMaterials = new List<Material>();
    [SerializeField] Vector3 offsetPosition, offsetRotation;


    [SerializeField] int backOrForward;
    [SerializeField] private float rollForce = 20;
    public int diceNumber;
    public Vector3 diceVelocity;

    void Awake()
    {
        Instance = this;
    }

    public void StartGame()
    {
        foreach (NetworkClient client in GameLogic.Instance.PlayersOrder)
        {
            diceLeft.Add((int)client.ClientId, 0);
            combineDiceNumber.Add((int)client.ClientId, 0);
            //Debug.Log(client.ClientId);
        }
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
    IEnumerator CheckVelocity(Rigidbody rb,int playerIndex,DiceType diceType,bool movePlayer = true)
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
                CheckZone.Instance.CheckDiceNumber(rb.gameObject, playerIndex, diceType,movePlayer);
                yield break;
            }
            if(rb.transform.position.y < -2)
            {
                Destroy(rb.gameObject);
                RollTheDiceServerRpc(playerIndex,1,false, movePlayer,diceType);
                yield break;
            }
        }
    }
    [ServerRpc(RequireOwnership = false)]
    public void RollTheDiceServerRpc(int playerIndex,int diceAmount,bool addDiceLeft = true,bool movePlayer=true,DiceType type = DiceType.SixSide,float gameSpeed = 1.4f)
    {
        diceResults.Clear();
        for (int i=0;i< diceAmount; i++)
        {
            StartCoroutine(CheckVelocity(SpawnDice(type,playerIndex), playerIndex, type,movePlayer));
        }
        if(addDiceLeft) diceLeft[playerIndex] += diceAmount;
        if(!isWaitingForAllDicesToRoll) WaitForAllDiceToRoll(playerIndex, movePlayer, gameSpeed);
    }

    private Rigidbody SpawnDice(DiceType diceType, int playerIndex)
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
            case DiceType.EightSide:
                prefab = EightSideDice;
                break;
        }
        offsetPosition = new Vector3(RandomNumber(3, 17), RandomNumber(1, 6), RandomNumber(3, 17));
        offsetRotation = new Vector3(RandomNumber(-180, 180), RandomNumber(-180, 180), RandomNumber(-180, 180));
        GameObject prefabInstance = Instantiate(prefab, offsetPosition, Quaternion.Euler(offsetRotation));
        Rigidbody rb = prefabInstance.GetComponent<Rigidbody>();
        prefabInstance.GetComponent<NetworkObject>().Spawn();
        prefabInstance.GetComponent<DiceScript>().OnDiceCreatedClientRpc(diceType,playerIndex);
        rb.AddForce(RandomDirectionHorizontal() * RandomNumber(rollForce / 1.5f, rollForce * 1.5f), ForceMode.Impulse);
        rb.AddForce(RandomDirectionVertical() * RandomNumber(rollForce / 1.5f, rollForce * 1.5f), ForceMode.Impulse);
        rb.AddTorque(transform.up * RandomNumber(rollForce / 1.5f, rollForce * 1.5f), ForceMode.Impulse);
        return rb;
    }




    private Dictionary<int,int> diceLeft = new Dictionary<int, int>();
    private Dictionary<int, int> combineDiceNumber = new Dictionary<int, int>();
    bool isWaitingForAllDicesToRoll = false;
    List<Vector2Int> diceResults = new List<Vector2Int>();
    public void DecreaseDiceLeft(int diceNumber,int playerIndex)
    {
        if (diceLeft[playerIndex] == 0) Debug.LogWarning("there are no more dices");
        diceResults.Add(new Vector2Int(diceNumber, playerIndex));
        combineDiceNumber[playerIndex] += diceNumber;
        //Debug.Log(combineDiceNumber[playerIndex] + " " + diceNumber);
        diceLeft[playerIndex]--;
        //Debug.Log("Dice decrease" + " "+ diceLeft);
    }

    private async void WaitForAllDiceToRoll(int playerIndex,bool movePlayer = true,float gameSpeed = 1.4f)
    {
        GameLogic.Instance.IncreasCallNextPlayerTurnServerRpc();
        Time.timeScale = gameSpeed;
        isWaitingForAllDicesToRoll = true;
        combineDiceNumberCopy.Clear();
        while (true)
        {
            await Awaitable.WaitForSecondsAsync(0.1f);
            bool shouldBreake = true;
            for(int i=0;i<diceLeft.Count;i++)
            {
                if (diceLeft[i] != 0) shouldBreake = false;
            }
            if (shouldBreake) break;
        }
        bool isDoublet = true;
        for(int i=1;i<diceResults.Count;i++)
        {
            if ((diceResults[i].x != diceResults[0].x)) isDoublet = false; // not working for more than 1 person
        }
        GameLogic.Instance.SetIsDoubletServerRPC(isDoublet);
        combineDiceNumberCopy.AddRange(combineDiceNumber);
        GameLogic.Instance.DecreaseCallNextPlayerTurnServerRpc();
        int amountOfThrowers = 0;
        for (int i=0;i<combineDiceNumber.Count;i++)
        {
            if (combineDiceNumber.ElementAt(i).Value == 0) continue;
            amountOfThrowers++;
            ClientRpcParams clientRpcParams = new ClientRpcParams
            {
                Send = new ClientRpcSendParams
                {
                    TargetClientIds = new ulong[] { (ulong)combineDiceNumber.ElementAt(i).Key }
                }
            };
            PlayerScript.LocalInstance.OnDiceNumberReturnClientRpc(combineDiceNumber.ElementAt(i).Value, movePlayer, amountOfThrowers > 1, clientRpcParams);
            combineDiceNumber[combineDiceNumber.ElementAt(i).Key] = 0;
        }
        
        CheckZone.Instance.DestroyAllDices();
        isWaitingForAllDicesToRoll = false;
        Time.timeScale = 1f;
        
    }

    private Dictionary<int, int> combineDiceNumberCopy = new Dictionary<int, int>();

    public async Task<Dictionary<int,int>> GetAllDiceValues()
    {
        while(isWaitingForAllDicesToRoll)
        {
            await Awaitable.EndOfFrameAsync();
        }
        return combineDiceNumberCopy;

    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.T))
        {
            RollTheDiceServerRpc(0,1,true,false,DiceType.EightSide);
        }
        if(Input.GetKeyDown(KeyCode.R))
        {
            RollTheDiceServerRpc(0, 1, true, false, DiceType.FourSide);
        }
        if (Input.GetKeyDown(KeyCode.I))
        {
            RollTheDiceServerRpc(0, 1, true, false, DiceType.SixSide);
            RollTheDiceServerRpc(1, 1, true, false, DiceType.SixSide);
            RollTheDiceServerRpc(2, 1, true, false, DiceType.SixSide);
            RollTheDiceServerRpc(0, 1, true, false, DiceType.FourSide);
            RollTheDiceServerRpc(1, 1, true, false, DiceType.FourSide);
            RollTheDiceServerRpc(2, 1, true, false, DiceType.FourSide);
            RollTheDiceServerRpc(0, 1, true, false, DiceType.EightSide);
            RollTheDiceServerRpc(1, 1, true, false, DiceType.EightSide);
            RollTheDiceServerRpc(2, 1, true, false, DiceType.EightSide);
        }

    }


}

public enum DiceType
{
    FourSide,
    SixSide,
    EightSide
}
