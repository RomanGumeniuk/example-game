using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.Netcode;

public class GameUIScript : NetworkBehaviour
{
    public static GameUIScript Instance { get; private set; }


    [SerializeField] private Button RollDiceButton;
    [SerializeField] private Button StartGameButton;
    [SerializeField] private TextMeshProUGUI TextAboutStateOfGame;
    

    public static UnityEvent OnStartGame;
    public static UnityEvent OnNextPlayerTurn;

    public void OnDiceNumberReturn(int diceNumber,bool isMultiplePlayerThrow = false)
    {
        TextAboutStateOfGame.text = "Wyrzuci³eœ: " + diceNumber;
        if(!isMultiplePlayerThrow)OnDiceNumberReturnForOtherPlayersServerRpc(diceNumber,PlayerScript.LocalInstance.character.GetName(),PlayerScript.LocalInstance.character.isWoman);
        //PlayerScript.LocalInstance.Move(diceNumber);
    }
    [ServerRpc(RequireOwnership =false)]
    private void OnDiceNumberReturnForOtherPlayersServerRpc(int diceNumber, string playerName,bool isWoman)
    {
        OnDiceNumberReturnForOtherPlayersClientRpc(diceNumber, playerName,isWoman);
    }


    [ClientRpc]
    private void OnDiceNumberReturnForOtherPlayersClientRpc(int diceNumber,string playerName,bool isWoman)
    {
        if (PlayerScript.LocalInstance.character.GetName() == playerName) return;
        TextAboutStateOfGame.gameObject.SetActive(true);
        TextAboutStateOfGame.text = playerName+" wyrzuci³"+(isWoman?"a":"")+": " + diceNumber;
        //PlayerScript.LocalInstance.Move(diceNumber);
    }

    public override void OnNetworkSpawn()
    {
        RollDiceButton.onClick.AddListener(() =>
        {
            GameLogic.Instance.DecreaseCallNextPlayerTurnServerRpc();
            RollDiceButton.gameObject.SetActive(false);
            DiceSpawn.Instance.RollTheDiceServerRpc(PlayerScript.LocalInstance.playerIndex,2);
        });
        if (!IsServer)return;
        
        StartGameButton.gameObject.SetActive( true);
        StartGameButton.onClick.AddListener(StartTheGameServerRpc);

        
    }
    [ServerRpc(RequireOwnership =false)]
    private void StartTheGameServerRpc()
    {
        OnStartGame.Invoke();
        StartGameButton.gameObject.SetActive(false);
        HideClientRpc();
    }
    [ClientRpc]
    public void HideClientRpc()
    {
        TestUIScript.Instance.gameObject.SetActive(false);
    }



    public void ShowUIForRollDice()
    {
        GameLogic.Instance.IncreasCallNextPlayerTurnServerRpc();
        RollDiceButton.gameObject.SetActive(true);
        TextAboutStateOfGame.text = "Rzuæ kostk¹!!!";
        TextAboutStateOfGame.gameObject.SetActive(true);
        CheckIfButtonShowed();
    }

    private async void CheckIfButtonShowed()
    {
        await Awaitable.WaitForSecondsAsync(0.1f);
        if (!RollDiceButton.gameObject.activeSelf)
        {
            Debug.Log("fixed");
            RollDiceButton.gameObject.SetActive(true);
        }
    }



        private void Awake()
    {
        Instance = this;
    }

    public void HideButton()
    {
        RollDiceButton.gameObject.SetActive(false);
        GameLogic.Instance.DecreaseCallNextPlayerTurnServerRpc();
    }


}
