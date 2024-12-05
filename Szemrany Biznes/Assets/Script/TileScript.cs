using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using TMPro;
using Unity.Collections.LowLevel.Unsafe;
public class TileScript : NetworkBehaviour
{
    public byte tileType = 0;


    const byte TOWN_TILE = 0;
    const byte START_TILE = 1;
    const byte CHANCE_TILE1 = 2;
    const byte CHANCE_TILE2 = 3;
    const byte TRAIN_TILE = 4;
    const byte PARKING_TILE = 5;
    const byte LIGHBULB_TILE = 6;
    const byte WATERPIEPES_TILE = 7;
    const byte RING_TILE = 8;
    const byte CUSTODY_TILE = 9;
    const byte PATROL_TILE = 10;
    const byte PARTY_TILE = 11;

    public List<int> townCostToBuy = new List<int>();
    public List<int> townCostToPay = new List<int>();

    public NetworkVariable<int> ownerId = new NetworkVariable<int>(-1); // -1 means that town has no owner
    public NetworkVariable<int> townLevel = new NetworkVariable<int>(-1);
    public int amountMoneyOnPlayerStep = 0;
    public DisplayPropertyUI displayPropertyUI;

    public bool monopol = false;

    public List<TileScript> AllTownsToGetMonopol = new List<TileScript>();

    private Material startMaterial;

    private void Start()
    {
        displayPropertyUI = transform.GetComponentInChildren<DisplayPropertyUI>();
        if (tileType != 0) return;

        if (townCostToBuy.Count == 0) return;
        startMaterial = transform.GetComponent<MeshRenderer>().material;
        townCostToBuy.Add((int)(townCostToBuy[0] * 1.5f));
        townCostToBuy.Add(townCostToBuy[0] * 3);
        townCostToBuy.Add((int)(townCostToBuy[0] * 4.5f));
        townCostToBuy.Add((int)(townCostToBuy[0] * 6));
        townCostToPay.Add(townCostToBuy[0] / 2);
        townCostToPay.Add(townCostToBuy[0]);
        townCostToPay.Add((int)(townCostToBuy[0] * 1.5f));
        townCostToPay.Add(townCostToBuy[0] * 3);
        townCostToPay.Add((int)(townCostToBuy[0] * 4.5f));
        townCostToPay.Add((int)(townCostToBuy[0] * 6.5f));

    }

    public override void OnNetworkSpawn()
    {
        ownerId.OnValueChanged += OnOwnerValueChanged;
    }

    public void OnOwnerValueChanged(int prevValue, int newValue)
    {
        if (tileType != 0) return;
        

        if (prevValue == -1)
        {
            if(CheckOtherTownsOwner())
            {
                SetMonopolyServerRpc(true);
            }
            return;
        }
        if (newValue == -1)
        {
            if (monopol)
            {
                SetMonopolyServerRpc(false);
            }
            return;
        }
        
    }

    private bool CheckOtherTownsOwner()
    {
        foreach (TileScript town in AllTownsToGetMonopol)
        {
            if (town.ownerId.Value != this.ownerId.Value)
            {
                return false;
            }
        }
        return true;
    }

    [ServerRpc]
    public void SetMonopolyServerRpc(bool Value)
    {
        SetMonopolyClientRpc(Value);
    }
    [ClientRpc]
    public void SetMonopolyClientRpc(bool Value)
    {
        foreach (TileScript town in AllTownsToGetMonopol)
        {
            town.monopol = Value;
            for (int i = 0; i < town.townCostToPay.Count; i++)
            {
                if (Value) town.townCostToPay[i] *= 2;
                else town.townCostToPay[i] /= 2;
            }
            town.UpdateOwnerTextServerRpc(true);
        }
        
    }


    private void Buy(int playerAmountOfMoney,int Cost,bool isUpgrading=false,byte maxLevelThatPlayerCanAfford = 0,byte currentAvailableTownUpgrade=0) 
    {
        if(isUpgrading)
        {
            if (maxLevelThatPlayerCanAfford == 0)
            {
                AlertTabForPlayerUI.Instance.ShowTab("You can't aford any upgrade!", 3.5f);
                return;
            }
            if(currentAvailableTownUpgrade <= townLevel.Value)
            {
                AlertTabForPlayerUI.Instance.ShowTab($"To upgrade this town you need minimal town level {townLevel.Value} in your colection of all real estates.\nNow your minimal level town is on {currentAvailableTownUpgrade} level.", 6);
                return;
            }
            BuyingTabUIScript.Instance.ShowBuyingUI(townLevel.Value, maxLevelThatPlayerCanAfford + townLevel.Value, townCostToBuy, this, currentAvailableTownUpgrade);
            return;
        }
        if (playerAmountOfMoney >= Cost)
        {
            BuyingTabForOnePaymentUIScript.Instance.ShowBuyingUI(Cost, this);
            return;
        }
        AlertTabForPlayerUI.Instance.ShowTab($"You can't aford this property it costs: {townCostToBuy[0]}PLN!", 3.5f);
    }

    private void Pay(int playerAmountOfMoney,int amountOfMoneyToPay,bool payToPlayer = true)
    {
        if (playerAmountOfMoney >= amountOfMoneyToPay)
        {
            AlertTabForPlayerUI.Instance.ShowTab($"You paid {amountOfMoneyToPay}PLN", 2);
            GameLogic.Instance.UpdateMoneyForPlayerServerRpc(amountOfMoneyToPay, PlayerScript.LocalInstance.playerIndex, 1, true, true);
            if (payToPlayer) GiveMoney(amountOfMoneyToPay, ownerId.Value,false); 
            return;
        }
        if(payToPlayer) PlayerScript.LocalInstance.ShowSellingTab(amountOfMoneyToPay, ownerId.Value);
        else PlayerScript.LocalInstance.ShowSellingTab(amountOfMoneyToPay, -1);
    }

    private void UpgradeTown(int playerAmountOfMoney, byte currentAvailableTownUpgrade)
    {
        byte maxLevelThatPlayerCanAfford = 0;
        for (int i = townLevel.Value; i < townLevel.Value + 3; i++)
        {
            if (i > 4) continue;
            if (playerAmountOfMoney >= CaluculatePropertyValue(townLevel.Value,i+1)) maxLevelThatPlayerCanAfford++;
        }
        Buy(playerAmountOfMoney,0,true, maxLevelThatPlayerCanAfford, currentAvailableTownUpgrade);
    }

    private void OnTownEnter(int playerAmountOfMoney,byte currentAvailableTownUpgrade,bool isNonUpgradingTown = false)
    {
        if (townLevel.Value == -1)
        {
            Buy(playerAmountOfMoney, townCostToBuy[0]);
            return;
        }
        if (ownerId.Value != PlayerScript.LocalInstance.playerIndex)
        {
            Pay(playerAmountOfMoney, townCostToPay[townLevel.Value]);
            return;
        }
        if(isNonUpgradingTown) GameUIScript.OnNextPlayerTurn.Invoke();
        UpgradeTown(playerAmountOfMoney, currentAvailableTownUpgrade);
    }

    private void OnStartEnter(bool passTheStartTile)
    {
        GiveMoney(amountMoneyOnPlayerStep); 
        if (!passTheStartTile) GameUIScript.OnNextPlayerTurn.Invoke();
    }

    private void GiveMoney(int amount, int playerIndex = -1, bool hide = true,bool changeTotalAmountOfMoney=true)
    {
        if (playerIndex == -1) playerIndex = PlayerScript.LocalInstance.playerIndex;
        GameLogic.Instance.UpdateMoneyForPlayerServerRpc(amount, playerIndex, 2, hide, changeTotalAmountOfMoney);
    }

    public void ChooseChanceCard(bool chanceTile1,List<string> chanceList)
    {
        int index = 0;
        string[] chancePropertis;
        if (chanceTile1) index = Random.Range(0, (int)chanceList.Count / 2);
        else index = Random.Range((int)chanceList.Count / 2, chanceList.Count);
        chancePropertis = chanceList[index].Split(";"[0]);
        AlertTabForPlayerUI.Instance.ShowTab(chancePropertis[0], 5);
        switch(index)
        {
            case 0:
            case 1:
            case 2:
            case 3:
            case 4:
            case 5:
                if (int.Parse(chancePropertis[1]) > 0)
                {
                    GiveMoney(int.Parse(chancePropertis[1]));
                }
                else
                {
                    Pay(PlayerScript.LocalInstance.amountOfMoney.Value, int.Parse(chancePropertis[1]) * -1, false);
                }
                PlayerScript.LocalInstance.cantMoveFor.Value += int.Parse(chancePropertis[2]);
                break;
            

        }
        
    }

    public void OnPlayerEnter(byte currentAvailableTownUpgrade, bool passTheStartTile = false)
    {
        int playerAmountOfMoney = PlayerScript.LocalInstance.amountOfMoney.Value;
        switch (tileType)
        {
            case START_TILE:
                OnStartEnter(passTheStartTile);
                return;
            case TOWN_TILE:
                OnTownEnter(playerAmountOfMoney,currentAvailableTownUpgrade);
                return;
            case PARKING_TILE:
            case RING_TILE:
                Pay(playerAmountOfMoney, amountMoneyOnPlayerStep, false);
                return;
            case TRAIN_TILE:
            case LIGHBULB_TILE:
            case WATERPIEPES_TILE:
                OnTownEnter(playerAmountOfMoney, 0, true);
                return;
            default:
                GameUIScript.OnNextPlayerTurn.Invoke();
                return;
        }

    }

    [ServerRpc(RequireOwnership = false)]
    public void UpgradeTownServerRpc(int currentNewLevel,int ownerId)
    {
        this.ownerId.Value = ownerId;

        townLevel.Value = currentNewLevel;
        UpdateOwnerTextClientRpc(ownerId, townLevel.Value);
    }

    [ServerRpc(RequireOwnership =false)]
    public void UpdateOwnerTextServerRpc(bool onlyChangeText=false)
    {
        UpdateOwnerTextClientRpc(ownerId.Value, townLevel.Value,onlyChangeText);
    }


    [ServerRpc(RequireOwnership =false)]
    public void SellingTownServerRpc(int playerIndex)
    {
        int townTotalValue = CaluculatePropertyValue();
        GameLogic.Instance.UpdateMoneyForPlayerServerRpc(townTotalValue, playerIndex, 2);
        ownerId.Value = -1;
        townLevel.Value = -1;
        UpdateOwnerTextClientRpc(-1, -1);
    }

    [ClientRpc]
    public void UpdateOwnerTextClientRpc(int ownerId, int townLevel,bool onlyChangeText=false)
    {
        if (ownerId != -1) GetComponent<MeshRenderer>().material = GameLogic.Instance.PlayerColors[ownerId];
        else GetComponent<MeshRenderer>().material = startMaterial;
        if (townLevel < 0) displayPropertyUI.ShowNormalView(ownerId, townLevel, 0, onlyChangeText);
        else displayPropertyUI.ShowNormalView(ownerId, townLevel, townCostToPay[townLevel], onlyChangeText);
    }

    public void ShowSellingView()
    {
        int totalPropertyValue = CaluculatePropertyValue();
        displayPropertyUI.ShowSellingView(totalPropertyValue);
    }

    public int CaluculatePropertyValue(int start =0,int stop= -1)
    {
        int totalPropertyValue = 0;
        if (stop == -1)
        {
            totalPropertyValue += townCostToBuy[0];
            stop = townLevel.Value;
        }
        for (int i = start; i < stop; i++)
        {
            totalPropertyValue += townCostToBuy[i];
        }
        return totalPropertyValue;
    }
}
