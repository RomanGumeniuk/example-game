using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class PrisonTabUI : NetworkBehaviour,IQueueWindows
{
    public static PrisonTabUI Instance { get; private set; }

    private void Awake()
    {
        if(Instance == null)
            Instance = this;
    }
    [SerializeField]
    private Button option1;
    [SerializeField]
    private Button option2;
    [SerializeField] 
    private TextMeshProUGUI title;
    [SerializeField]
    private TextMeshProUGUI opiton1Title;
    [SerializeField]
    private TextMeshProUGUI opiton1Description;
    [SerializeField]
    private TextMeshProUGUI opiton2Title;
    [SerializeField]
    private TextMeshProUGUI opiton2Description;

    private bool option1Picked = false;
    private bool isWaitingForDiceRoll = false;
    private void Start()
    {
        option1.onClick.AddListener(() =>
        {

            option1Picked = true;
            DiceSpawn.Instance.RollTheDiceServerRpc(PlayerScript.LocalInstance.playerIndex,2,true,false);
            isWaitingForDiceRoll = true;
            Hide();
        });

        option2.onClick.AddListener(() =>
        {
            if(PlayerScript.LocalInstance.isInPrison.Value)
            {
                PlayerScript.LocalInstance.SetIsInPrisonServerRpc(false);
                PlayerScript.LocalInstance.UpdatePlayerCantMoveVariableServerRpc(0, PlayerScript.LocalInstance.playerIndex);
                GameLogic.Instance.UpdateMoneyForPlayerServerRpc(DEPOSIT, PlayerScript.LocalInstance.playerIndex, 1, true, true);
                _ = AlertTabForPlayerUI.Instance.ShowTab("Zap�aci�e� kaucje i jeste� ju� wolny!", 2f, false);
                GameUIScript.Instance.ShowUIForRollDice();
                Hide();
                return;
            }
            option1Picked = false;
            DiceSpawn.Instance.RollTheDiceServerRpc(PlayerScript.LocalInstance.playerIndex, 2, true, false);
            isWaitingForDiceRoll = true;
            Hide();
        });
    }

    const int MIN_DICE_VALUE_TO_HELP = 8;
    const int MIN_DICE_VALUE_TO_BETRAY = 10;

    public void OnDiceValueReturned(int diceValue)
    {
        if (!isWaitingForDiceRoll) return;
        isWaitingForDiceRoll = false;
        Debug.Log("On dice value returned: " + diceValue);
        if (PlayerScript.LocalInstance.isInPrison.Value)
        {
            if(diceValue == 3 || diceValue ==7)
            {
                PlayerScript.LocalInstance.SetIsInPrisonServerRpc(false);
                PlayerScript.LocalInstance.UpdatePlayerCantMoveVariableServerRpc(0, PlayerScript.LocalInstance.playerIndex);
                _ = AlertTabForPlayerUI.Instance.ShowTab("Uda�o ci si� uciec i jeste� ju� wolny!", 2f, false);
                GameUIScript.Instance.ShowUIForRollDice();
                return;
            }
            _ = AlertTabForPlayerUI.Instance.ShowTab("Nie uda�o ci si� uciec!", 2f);
            PlayerScript.LocalInstance.UpdatePlayerCantMoveVariableServerRpc(PlayerScript.LocalInstance.cantMoveFor.Value-1, PlayerScript.LocalInstance.playerIndex);
            return;
        }
        if (option1Picked)
        {
            if (diceValue < MIN_DICE_VALUE_TO_HELP)
            {
                _ = AlertTabForPlayerUI.Instance.ShowTab("Nie uda�o ci si� uwolni� gracza z wi�zienia!", 2.5f);
                return;
            }
            foreach (NetworkClient client in NetworkManager.Singleton.ConnectedClientsList)
            {
                if (!client.PlayerObject.GetComponent<PlayerScript>().isInPrison.Value) continue;

                client.PlayerObject.GetComponent<PlayerScript>().SetIsInPrisonServerRpc(false);
                client.PlayerObject.GetComponent<PlayerScript>().UpdatePlayerCantMoveVariableServerRpc(0, (int)client.ClientId);
                _ = AlertTabForPlayerUI.Instance.ShowTab("Uda�o ci si� uwolni� gracza z wi�zienia!", 2.5f);
                AlertTabForPlayerUI.Instance.ShowTabForOtherPlayer("Inny gracz pom�g� ci w ucieczce z wi�zienia, jeste� wolny", 2.5f, (int)client.ClientId);
                return;
            }
            Debug.LogWarning("Nie znaleziono gracza w wi�zieniu");
            GameUIScript.OnNextPlayerTurn.Invoke();
            return;
        }

        if (diceValue < MIN_DICE_VALUE_TO_BETRAY)
        {
            _ = AlertTabForPlayerUI.Instance.ShowTab("Nie uda�o ci si� podkapowa� gracza w wi�zieniu!", 2.5f);
            return;
        }
        foreach (NetworkClient client in NetworkManager.Singleton.ConnectedClientsList)
        {
            if (!client.PlayerObject.GetComponent<PlayerScript>().isInPrison.Value) continue;
            client.PlayerObject.GetComponent<PlayerScript>().SetWasBetrayedServerRpc(true);
            _ = AlertTabForPlayerUI.Instance.ShowTab("Uda�o ci si� odkapowa� gracza w wi�zieniu!", 2.5f);
            AlertTabForPlayerUI.Instance.ShowTabForOtherPlayer("Inny gracz podkapowa� ci�, w nast�pnej turze nie b�dziesz m�g� nic zrobi�", 2.5f, (int)client.ClientId);
            return;
        }
        Debug.LogWarning("Nie znaleziono gracza w wi�zieniu");
        GameUIScript.OnNextPlayerTurn.Invoke();
        return;

    }
    const int DEPOSIT = 1500;
    
    

    public void Show(int playerIndex=-1)
    {
        Debug.Log("a");
        if(playerIndex == -1) playerIndex = PlayerScript.LocalInstance.playerIndex;
        ShowServerRpc(playerIndex); 
    }


    [ServerRpc(RequireOwnership =false)]
    private void ShowServerRpc(int playerIndex)
    {
        ClientRpcParams clientRpcParams = new ClientRpcParams
        {
            Send = new ClientRpcSendParams
            {
                TargetClientIds = new ulong[] { (ulong)playerIndex }
            }
        };
        Debug.Log("b " + playerIndex);
        ShowClientRpc(clientRpcParams);
    }

    [ClientRpc]
    private void ShowClientRpc(ClientRpcParams clientRpcParams = default)
    {
        //Debug.Log("c");
        PlayerScript.LocalInstance.AddToQueueOfWindows(this);
    }

    void Show()
    {
        option2.interactable = true;
        if (PlayerScript.LocalInstance.isInPrison.Value)
        {
            title.text = "Wybierz co chesz zrobi�?";
            opiton1Title.text = "Spr�buj uciec";
            opiton1Description.text = "Wyrzu� sume 3 lub 7 aby uciec";
            option1.GetComponentInChildren<TextMeshProUGUI>().text = "Ucieczka";
            if (PlayerScript.LocalInstance.amountOfMoney.Value < DEPOSIT) option2.interactable = false;
            opiton2Title.text = "Zap�a� kaucje";
            opiton2Description.text = "Zap�a� kaucje w wysoko�ci " + DEPOSIT + " PLN";
            option2.GetComponentInChildren<TextMeshProUGUI>().text = "Zap�a�";
        }
        else
        {
            title.text = "Odwiedzi�e� odsiaduj�cego gracza w wi�zieniu";
            opiton1Title.text = "Pom� w ucieczce";
            opiton1Description.text = "Wyrzu� co najmniej 8 aby uwolni� gracza";
            option1.GetComponentInChildren<TextMeshProUGUI>().text = "Pom�";
            opiton2Title.text = "Podkapuj";
            opiton2Description.text = "Wyrzu� co najmniej 10 aby podkapowa� gracza";
            option2.GetComponentInChildren<TextMeshProUGUI>().text = "Podkapuj";

        }
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(true);
        }
    }
    public void Hide()
    {
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(false);
        }
        PlayerScript.LocalInstance.GoToNextAction();
    }

    public void ResumeAction()
    {
        Show();
    }
}
