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

    public override void OnNetworkSpawn()
    {
        RollDiceButton.onClick.AddListener(() =>
        {
            Debug.Log("RollClicked");
            int diceValue = Random.Range(1, 7);
            TextAboutStateOfGame.text = "You rolled " + diceValue;
            PlayerScript.LocalInstance.Move(diceValue);
            RollDiceButton.gameObject.SetActive(false);
            
            //OnNextPlayerTurn.Invoke();
        });
        if (!IsServer)return;
        
        StartGameButton.gameObject.SetActive( true);
        StartGameButton.onClick.AddListener(() =>
        {
            OnStartGame.Invoke();
            StartGameButton.gameObject.SetActive(false);
        });

        
    }

    public void ShowUIForRollDice()
    {
        RollDiceButton.gameObject.SetActive(true);
        TextAboutStateOfGame.text = "Roll Dice!!!";
        TextAboutStateOfGame.gameObject.SetActive(true);
    }


    private void Awake()
    {
        Instance = this;
    }

    


}
