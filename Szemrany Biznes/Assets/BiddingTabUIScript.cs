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

    public float timeLeft = 15;

    private void Awake()
    {
        PassButton.onClick.AddListener(() =>
        {
            //Pass

        });

        Bid10Button.onClick.AddListener(() =>
        {
            //Bid 10
            timeLeft = 15;
        });
        Bid50Button.onClick.AddListener(() =>
        {
            //Bid 10
            timeLeft = 15;
        });
        Bid100Button.onClick.AddListener(() =>
        {
            //Bid 10
            timeLeft = 15;
        });
        Bid200Button.onClick.AddListener(() =>
        {
            //Bid 10
            timeLeft = 15;
        });
    }



    [ClientRpc]
    public void StartAuctionClientRpc(int currentCostOfProperty,string currentPropertiName, int playerIndexThatNotBuyProperti)
    {
        NameOfProperti.text = currentPropertiName;
        RegularCost.text = currentCostOfProperty.ToString() + "PLN";
        CurrentBid.text = "Start bid: " + (currentCostOfProperty - 10).ToString() + "PLN";
        TimerImage.fillAmount = 1;
        TimeLeft.text = "15s";
        TextLabel.text = "Wait for all Players to end auction";
        currentBidWinnerPlayerIndex.Value= -1;
        timeLeft = 15;
        transform.GetChild(0).gameObject.SetActive(true);
        if(playerIndexThatNotBuyProperti != PlayerScript.LocalInstance.playerIndex && PlayerScript.LocalInstance.amountOfMoney.Value >=currentCostOfProperty)
        {
            TextLabel.text = "How much do you wanna bid?";
            foreach(Transform child in transform)
            {
                child.gameObject.SetActive(true);
            }
            RefreshOnNewBidClientRpc();
        }

    }


    [ClientRpc]
    public void RefreshOnNewBidClientRpc()
    {

        if (!transform.GetChild(1).gameObject.activeSelf || PlayerScript.LocalInstance.playerIndex == currentBidWinnerPlayerIndex.Value) return;
        if (PlayerScript.LocalInstance.amountOfMoney.Value < currentBid.Value + 10)
        {
            transform.GetChild(1).GetComponent<Button>().interactable = false;
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


    private void Start()
    {
        StartCoroutine(Timer());
    }

    IEnumerator Timer()
    {
        while(true)
        {
            if (timeLeft <= 0)
            {
                Debug.Log("end");
                yield break;
            }
            yield return new WaitForFixedUpdate();
            timeLeft -= 0.02f;
            TimeLeft.text = Mathf.Ceil(timeLeft) + "s";
            TimerImage.fillAmount = timeLeft/15;
        }
        
    }

    [ClientRpc]
    public void EndOfAuctionClientRpc()
    {
        if(currentBidWinnerPlayerIndex.Value != -1)
        {
            //someone buy
            return;
        }
        // no one buy
    }
}
