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

    public int playerIndexThatNotBuyProperti=-1;

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
        RefreshOnNewBidServerRpc();
        PassButton.interactable = false;
    }

    public void HideBiddingOption()
    {
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(false);
        }
        transform.GetChild(0).gameObject.SetActive(true);
    }

    [ServerRpc(RequireOwnership =false)]
    public void ChangeCurrentBidServerRpc(int newBid)
    {
        currentBid.Value = newBid;
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
        Debug.Log("added "+ currentPlayersIndexInAuction.Count);

    }

    [ServerRpc(RequireOwnership = false)]
    private void RemovePlayerToListServerRpc(int playerIndex)
    {
        currentPlayersIndexInAuction.Remove(playerIndex);
        Debug.Log("delete");

    }

    [ServerRpc(RequireOwnership = false)]
    public void StartServerRpc(int currentCostOfProperty, string currentPropertiName, int playerIndexThatNotBuyProperti)
    {
        currentPlayersIndexInAuction.Clear();
        StartAuctionClientRpc(currentCostOfProperty, currentPropertiName, playerIndexThatNotBuyProperti);
    }

    [ClientRpc]
    public void StartAuctionClientRpc(int currentCostOfProperty,string currentPropertiName, int playerIndexThatNotBuyProperti)
    {
        Debug.Log("startAukcji");
        NameOfProperti.text = currentPropertiName;
        RegularCost.text = currentCostOfProperty.ToString() + "PLN";
        ChangeCurrentBidServerRpc(currentCostOfProperty - 10);
        CurrentBid.text = "Start bid: " + currentBid.Value + "PLN";
        TimerImage.fillAmount = 1;
        TimeLeft.text = "15s";
        TextLabel.text = "Wait for all Players to end auction";
        ChangeCurrentBidWinnerPlayerIndexServerRpc(-1);
        ChangeTimeLeftServerRpc(15);
        
        transform.GetChild(0).gameObject.SetActive(true);
        this.playerIndexThatNotBuyProperti = playerIndexThatNotBuyProperti;
        if (playerIndexThatNotBuyProperti != PlayerScript.LocalInstance.playerIndex && PlayerScript.LocalInstance.amountOfMoney.Value >=currentCostOfProperty)
        {
            TextLabel.text = "How much do you wanna bid?";
            AddPlayerToListServerRpc(PlayerScript.LocalInstance.playerIndex);
            foreach (Transform child in transform)
            {
                child.gameObject.SetActive(true);
            }
            RefreshOnNewBidServerRpc();
            
        }
        for(int i=0;i<currentPlayersIndexInAuction.Count;i++)
        {
            Debug.Log(i);
        }
        
        StartCoroutine(Timer());

    }
    [ServerRpc (RequireOwnership =false)]
    public void RefreshOnNewBidServerRpc()
    {
        RefreshOnNewBidClientRpc();
    }    


    [ClientRpc]
    public void RefreshOnNewBidClientRpc()
    {
        if(currentBidWinnerPlayerIndex.Value == PlayerScript.LocalInstance.playerIndex) PassButton.interactable = true;
        CurrentBid.text = "Player#"+currentBidWinnerPlayerIndex.Value+" bid: " + currentBid.Value + " PLN";
        if(!currentPlayersIndexInAuction.Contains(PlayerScript.LocalInstance.playerIndex)) return;
        if (PlayerScript.LocalInstance.amountOfMoney.Value < currentBid.Value + 10)
        {
            transform.GetChild(1).GetComponent<Button>().interactable = false;
            RemovePlayerToListServerRpc(PlayerScript.LocalInstance.playerIndex);
            HideBiddingOption();
        }
        if (PlayerScript.LocalInstance.amountOfMoney.Value < currentBid.Value + 50)
        {
            transform.GetChild(2).GetComponent<Button>().interactable = false;
        }
        if (PlayerScript.LocalInstance.amountOfMoney.Value < currentBid.Value + 100)
        {
            transform.GetChild(3).GetComponent<Button>().interactable = false;
        }
        if (PlayerScript.LocalInstance.amountOfMoney.Value < currentBid.Value + 200)
        {
            transform.GetChild(4).GetComponent<Button>().interactable = false;
        }
    }

    IEnumerator Timer()
    {
        yield return new WaitForSeconds(0.5f);
        while(true)
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
        }
        
    }

    [ClientRpc]
    public void EndOfAuctionClientRpc()
    {
        Debug.Log(currentPlayersIndexInAuction.Count);
        hide();
        if (currentBidWinnerPlayerIndex.Value != -1 && PlayerScript.LocalInstance.playerIndex == playerIndexThatNotBuyProperti)
        {
            BuyingTabForOnePaymentUIScript.Instance.BuyTownForOtherPlayer(currentBidWinnerPlayerIndex.Value,currentBid.Value);
        }
        
    }

    public void hide()
    {
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(false);
        }
    }

}
