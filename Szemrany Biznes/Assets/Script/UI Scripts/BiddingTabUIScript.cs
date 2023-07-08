using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.Netcode;

public class BiddingTabUIScript : NetworkBehaviour
{
    public Button PassButton;
    public Button Bid10Button;
    public Button Bid50Button;
    public Button Bid100Button;
    public Button Bid200Button;

    public Image TimerImage;

    public TextMeshProUGUI TextLabel;
    public TextMeshProUGUI NameOfProperti;
    public TextMeshProUGUI RegularCost;
    public TextMeshProUGUI CurrentBid;
    public TextMeshProUGUI TimeLeft;

    public NetworkVariable<int> currentBidWinnerPlayerIndex = new NetworkVariable<int>(-1);
    public NetworkVariable<int> currentBid = new NetworkVariable<int>(0);
    public List<int> currentPlayersIndexInAuction = new List<int>();

    public NetworkVariable<float> timeLeft = new NetworkVariable<float>(15);

    public int playerIndexThatNotBuyProperti = -1;
    public int startBidValue = 0;

    private bool moneyForBank = true;

    public static BiddingTabUIScript Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
        PassButton.onClick.AddListener(() =>
        {
            //Pass
            RemovePlayerToListServerRpc(PlayerScript.LocalInstance.playerIndex);
            HideBiddingOption();
        });

        Bid10Button.onClick.AddListener(() =>
        {
            //Bid 10
            ButtonBidLogic(10);
        });
        Bid50Button.onClick.AddListener(() =>
        {
            //Bid 50
            ButtonBidLogic(50);
        });
        Bid100Button.onClick.AddListener(() =>
        {
            //Bid 100
            ButtonBidLogic(100);
        });
        Bid200Button.onClick.AddListener(() =>
        {
            //Bid 200
            ButtonBidLogic(200);
        });
    }


    public void ButtonBidLogic(int bidValue)
    {
        ChangeTimeLeftServerRpc(15);
        ChangeCurrentBidServerRpc(currentBid.Value + bidValue);
        ChangeCurrentBidWinnerPlayerIndexServerRpc(PlayerScript.LocalInstance.playerIndex);
        Debug.Log("false interactable");
        PassButton.interactable = false;
        Bid10Button.interactable = false;
        Bid50Button.interactable = false;
        Bid100Button.interactable = false;
        Bid200Button.interactable = false;
    }

    public void HideBiddingOption()
    {
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(false);
        }
        transform.GetChild(0).gameObject.SetActive(true);
    }

    [ServerRpc(RequireOwnership = false)]
    public void ChangeCurrentBidServerRpc(int newBid)
    {
        currentBid.Value = newBid;
        OnCurrentBidChanged(0, currentBid.Value);
    }

    [ServerRpc(RequireOwnership = false)]
    public void ChangeCurrentBidWinnerPlayerIndexServerRpc(int newPlayerIndex)
    {
        currentBidWinnerPlayerIndex.Value = newPlayerIndex;
    }
    [ServerRpc(RequireOwnership = false)]
    public void ChangeTimeLeftServerRpc(float newTimeLeft)
    {
        timeLeft.Value = newTimeLeft;
    }

    [ServerRpc(RequireOwnership = false)]
    private void AddPlayerToListServerRpc(int playerIndex)
    {
        currentPlayersIndexInAuction.Add(playerIndex);
        Debug.Log("added " + currentPlayersIndexInAuction.Count);

    }

    [ServerRpc(RequireOwnership = false)]
    private void RemovePlayerToListServerRpc(int playerIndex)
    {
        currentPlayersIndexInAuction.Remove(playerIndex);
        Debug.Log("delete");

    }

    [ServerRpc(RequireOwnership = false)]
    public void StartAuctionServerRpc(int currentCostOfProperty, string currentPropertiName, int playerIndexThatNotBuyProperti,bool moneyForBank = true)
    {
        currentPlayersIndexInAuction.Clear();
        StartAuctionClientRpc(currentCostOfProperty, currentPropertiName, playerIndexThatNotBuyProperti,moneyForBank);
    }

    [ClientRpc]
    public void StartAuctionClientRpc(int currentCostOfProperty, string currentPropertiName, int playerIndexThatNotBuyProperti,bool moneyForBank)
    {
        this.moneyForBank = moneyForBank;
        NameOfProperti.text = currentPropertiName;
        RegularCost.text = currentCostOfProperty.ToString() + "PLN";
        ChangeCurrentBidServerRpc(currentCostOfProperty - 10);
        CurrentBid.text = "Start bid: " + (currentCostOfProperty - 10) + "PLN";
        TimerImage.fillAmount = 1;
        TimeLeft.text = "15s";
        TextLabel.text = "Wait for all Players to end auction";
        ChangeCurrentBidWinnerPlayerIndexServerRpc(-1);
        ChangeTimeLeftServerRpc(15);
        RefreshOnNewBid();
        startBidValue = currentCostOfProperty;
        transform.GetChild(0).gameObject.SetActive(true);
        this.playerIndexThatNotBuyProperti = playerIndexThatNotBuyProperti;
        if (playerIndexThatNotBuyProperti != PlayerScript.LocalInstance.playerIndex && PlayerScript.LocalInstance.amountOfMoney.Value >= currentCostOfProperty)
        {
            TextLabel.text = "How much do you wanna bid?";
            AddPlayerToListServerRpc(PlayerScript.LocalInstance.playerIndex);
            foreach (Transform child in transform)
            {
                child.gameObject.SetActive(true);
            }
        }
        for (int i = 0; i < currentPlayersIndexInAuction.Count; i++)
        {
            Debug.Log(i);
        }

        StartCoroutine(Timer(currentCostOfProperty));

    }



    public override void OnNetworkSpawn()
    {
        currentBidWinnerPlayerIndex.OnValueChanged += OnCurrentBidWinnerChanged;
        currentBid.OnValueChanged += OnCurrentBidChanged;
    }

    public void OnCurrentBidWinnerChanged(int prevValue,int currentValue)
    {
        RefreshOnNewBid();
    }

    public void OnCurrentBidChanged(int prevValue,int currentValue)
    {
        Debug.Log($"old: {prevValue} new:{currentValue}");
        Debug.Log($"player money: {PlayerScript.LocalInstance.amountOfMoney.Value} curentBid{currentValue}");
        if (PlayerScript.LocalInstance.amountOfMoney.Value < currentValue + 10)
        {
            Bid10Button.interactable = false;
            RemovePlayerToListServerRpc(PlayerScript.LocalInstance.playerIndex);
            HideBiddingOption();
            Debug.Log("hide10");
        }
        if (PlayerScript.LocalInstance.amountOfMoney.Value < currentValue + 50)
        {
            Bid50Button.interactable = false;
            Debug.Log("hide50");
        }
        if (PlayerScript.LocalInstance.amountOfMoney.Value < currentValue + 100)
        {
            Bid100Button.interactable = false;
            Debug.Log("hide100");
        }
        if (PlayerScript.LocalInstance.amountOfMoney.Value < currentValue + 200)
        {
            Bid200Button.interactable = false;
            Debug.Log("hide200");
        }
    }
    
    public void RefreshOnNewBid()
    {
        if (currentBidWinnerPlayerIndex.Value != PlayerScript.LocalInstance.playerIndex)
        {
            Debug.Log("true interactable |" + currentBidWinnerPlayerIndex.Value +"|"+ PlayerScript.LocalInstance.playerIndex);
            PassButton.interactable = true;
            if(PlayerScript.LocalInstance.amountOfMoney.Value >= currentBid.Value + 10)
                Bid10Button.interactable = true;
            if (PlayerScript.LocalInstance.amountOfMoney.Value >= currentBid.Value + 50)
                Bid50Button.interactable = true;
            if (PlayerScript.LocalInstance.amountOfMoney.Value >= currentBid.Value + 100)
                Bid100Button.interactable = true;
            if (PlayerScript.LocalInstance.amountOfMoney.Value >= currentBid.Value + 200)
                Bid200Button.interactable = true;
        }
    }

    IEnumerator Timer(int currentCostOfProperty)
    {
        yield return new WaitForSeconds(0.5f);
        while (true)
        {
            if ((timeLeft.Value <= 0 || currentPlayersIndexInAuction.Count  < 2) &&IsServer)
            {
                EndOfAuctionClientRpc();
                yield break;
            }
            yield return new WaitForFixedUpdate();
            if(IsServer)ChangeTimeLeftServerRpc( timeLeft.Value - 0.02f);
            TimeLeft.text = Mathf.Ceil(timeLeft.Value)+"s";
            TimerImage.fillAmount = timeLeft.Value/15;
            if (currentBidWinnerPlayerIndex.Value != -1) CurrentBid.text = "Player#" + currentBidWinnerPlayerIndex.Value + " bid: " + currentBid.Value + " PLN";
        }
        
    }

    [ClientRpc]
    public void EndOfAuctionClientRpc()
    {
        Debug.Log(currentPlayersIndexInAuction.Count);
        hide();
        if(currentBidWinnerPlayerIndex.Value==-1)
        {
            if (!moneyForBank) SellingTabUI.Instance.AuctionEnd(false);
            return;
        }
        if (PlayerScript.LocalInstance.playerIndex == playerIndexThatNotBuyProperti)
        {
            BuyingTabForOnePaymentUIScript.Instance.BuyTownForOtherPlayer(currentBidWinnerPlayerIndex.Value,currentBid.Value, startBidValue, moneyForBank);
            if (!moneyForBank) 
            {
                GameLogic.Instance.UpdateMoneyForPlayerServerRpc(startBidValue, playerIndexThatNotBuyProperti, 1,false,true);
                GameLogic.Instance.UpdateMoneyForPlayerServerRpc(currentBid.Value, playerIndexThatNotBuyProperti, 2, false, true);
                StartCoroutine(SellingTabUI.Instance.UpdatePayButton());
                SellingTabUI.Instance.AuctionEnd(true);
            }
            
        }
        if(currentBidWinnerPlayerIndex.Value == PlayerScript.LocalInstance.playerIndex) StartCoroutine(WinnerOfAuction());
        
    }

    IEnumerator WinnerOfAuction()
    {
        transform.GetChild(0).gameObject.SetActive(true);
        TextLabel.text = "You win Auction and paid " + currentBid.Value + " PLN for " + NameOfProperti.text;
        yield return new WaitForSeconds(3);
        hide();
    }

    public void hide()
    {
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(false);
        }
    }

}
