using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

using TMPro;
using Unity.Collections.LowLevel.Unsafe;
using static UnityEngine.Rendering.DebugUI;
public class TileScript : NetworkBehaviour
{
    public TileType tileType = 0;
    public PropertyType propertyType = 0;

   

    

    public int index;
    public List<int> townCostToBuy = new List<int>();
    public List<int> townCostToPay = new List<int>();

    public NetworkVariable<int> ownerId = new NetworkVariable<int>(-1); // -1 means that town has no owner
    public NetworkVariable<int> townLevel = new NetworkVariable<int>(-1);
    public int amountMoneyOnPlayerStep = 0;
    public DisplayPropertyUI displayPropertyUI;

    public bool monopol = false;

    public List<TileScript> AllTownsToGetMonopol = new List<TileScript>();

    private Material startMaterial;

    public Tile specialTileScript;

    private void Start()
    {
        displayPropertyUI = transform.GetComponentInChildren<DisplayPropertyUI>();
        specialTileScript = SetSpecialTileScript();
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

    private Tile SetSpecialTileScript()
    {
        switch (tileType)
        {
            case TileType.GangTile:
                return new GangTile(this);
            case TileType.PatrolTile:
                return new PatrolTile(this);
            case TileType.BillTile:
                return new BillTile(this);
            case TileType.BonusTile:
                return new BonusTile(this);
            case TileType.StartTile:
                return new StartTile(this);
            case TileType.ChanceTile:
                return new ChanceTile(this);
            case TileType.SpecialTile:
                return new SpecialTile(this);
            default:
                return new TownTile(this);
        }

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


    public void Buy(int playerAmountOfMoney,int Cost,bool isUpgrading=false,byte maxLevelThatPlayerCanAfford = 0,byte currentAvailableTownUpgrade=0) 
    {
        if(isUpgrading)
        {
            if (maxLevelThatPlayerCanAfford == 0)
            {
                _ = AlertTabForPlayerUI.Instance.ShowTab("Nie staæ ciê na ¿adne ulepszenia!", 3.5f);
                return;
            }
            if(currentAvailableTownUpgrade <= townLevel.Value)
            {
                _ = AlertTabForPlayerUI.Instance.ShowTab($"To upgrade this town you need minimal town level {townLevel.Value} in your colection of all real estates.\nNow your minimal level town is on {currentAvailableTownUpgrade} level.", 6);
                return;
            }
            if (propertyType == PropertyType.None) ChoosingPropertyTypeUI.Instance.ShowChoosingUI(this);
            else BuyingTabUIScript.Instance.ShowBuyingUI(townLevel.Value, maxLevelThatPlayerCanAfford + townLevel.Value, townCostToBuy, this, currentAvailableTownUpgrade);
            return;
        }
        if (playerAmountOfMoney >= Cost)
        {
            BuyingTabForOnePaymentUIScript.Instance.ShowBuyingUI(Cost, this);
            return;
        }
        _ = AlertTabForPlayerUI.Instance.ShowTab($"Nie staæ ciê na t¹ posiad³oœæ: {townCostToBuy[0]}PLN!", 3.5f);
    }

    public async void Pay(int playerAmountOfMoney,int amountOfMoneyToPay,bool payToPlayer = true)
    {
        if (playerAmountOfMoney >= amountOfMoneyToPay)
        {
            GameLogic.Instance.UpdateMoneyForPlayerServerRpc(amountOfMoneyToPay, PlayerScript.LocalInstance.playerIndex, 1, true, true);
            if (payToPlayer) GiveMoney(amountOfMoneyToPay, ownerId.Value,false);
            int propertyValue = 0;
            bool isCapableOfBuyingProperty = false;
            if(payToPlayer)
            {
                propertyValue = (int)(specialTileScript.CaluculatePropertyValue() * 1.5f);
                isCapableOfBuyingProperty = PlayerScript.LocalInstance.amountOfMoney.Value > propertyValue;
            }
            await AlertTabForPlayerUI.Instance.ShowTab($"Zap³aci³eœ {amountOfMoneyToPay}PLN", 1.5f, !isCapableOfBuyingProperty);
            if (!isCapableOfBuyingProperty) return;
            //oferta wykupu
            BuyingTabForOnePaymentUIScript.Instance.ShowBuyingUI(propertyValue, this,"Do you want to buy this property from player "+ownerId.Value,true);

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
            if (playerAmountOfMoney >= specialTileScript.CaluculatePropertyValue(townLevel.Value,i+1)) maxLevelThatPlayerCanAfford++;
        }
        Buy(playerAmountOfMoney,0,true, maxLevelThatPlayerCanAfford, currentAvailableTownUpgrade);
    }

    public void OnTownEnter(int playerAmountOfMoney,byte currentAvailableTownUpgrade,bool isNonUpgradingTown = false)
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
        else UpgradeTown(playerAmountOfMoney, currentAvailableTownUpgrade);
    }

    

    public void GiveMoney(int amount, int playerIndex = -1, bool hide = true,bool changeTotalAmountOfMoney=true)
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
        _ = AlertTabForPlayerUI.Instance.ShowTab(chancePropertis[0], 5);
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

    public void OnPlayerEnter(byte currentAvailableTownUpgrade)
    {
        int playerAmountOfMoney = PlayerScript.LocalInstance.amountOfMoney.Value;
        switch (tileType)
        {
            case TileType.TownTile:
                OnTownEnter(playerAmountOfMoney,currentAvailableTownUpgrade);
                return;
            case TileType.PartyTile:
                _ = AlertTabForPlayerUI.Instance.ShowTab($"Wprosi³eœ siê na impreze z darmowym alkocholem, jesteœ 400 PLN do przodu", 3.5f);
                GiveMoney(amountMoneyOnPlayerStep);
                return;
            case TileType.PatrolTile:
                UpdatePlayerCantMoveVariableServerRpc(2, PlayerScript.LocalInstance.playerIndex);
                GameUIScript.OnNextPlayerTurn.Invoke();
                return;
            case TileType.CustodyTile:
                UpdatePlayerCantMoveVariableServerRpc(1, PlayerScript.LocalInstance.playerIndex);
                Pay(playerAmountOfMoney, amountMoneyOnPlayerStep, false);
                return;

            default:
                Debug.Log(specialTileScript);
                specialTileScript.OnPlayerStepped();
                return;
        }

    }

    [ServerRpc(RequireOwnership = false)]
    public void UpdatePlayerCantMoveVariableServerRpc(int value,int playerId)
    {
        NetworkManager.Singleton.ConnectedClientsList[playerId].PlayerObject.GetComponent<PlayerScript>().cantMoveFor.Value = value;
    }

    [ServerRpc(RequireOwnership = false)]
    public void SetPropertyTypeServerRpc(PropertyType choosenPropertyType)
    {
        propertyType = choosenPropertyType;
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
        int townTotalValue = specialTileScript.CaluculatePropertyValue();
        PlayerScript.LocalInstance.tilesThatPlayerOwnList.Remove(this);
        GameLogic.Instance.UpdateMoneyForPlayerServerRpc(townTotalValue, playerIndex, 2);
        ownerId.Value = -1;
        townLevel.Value = -1;
        UpdateOwnerTextClientRpc(-1, -1);
    }

    [ServerRpc(RequireOwnership = false)]
    public void TownBuyoutServerRpc(int playerThatBought,int costToBuy)
    {
        NetworkManager.Singleton.ConnectedClients[(ulong)ownerId.Value].PlayerObject.GetComponent<PlayerScript>().tilesThatPlayerOwnList.Remove(this);
        PlayerScript.LocalInstance.tilesThatPlayerOwnList.Add(this);
        GameLogic.Instance.UpdateMoneyForPlayerServerRpc(costToBuy, playerThatBought, 1);
        GameLogic.Instance.UpdateMoneyForPlayerServerRpc(costToBuy, ownerId.Value, 2,false);
        ownerId.Value = playerThatBought;
        UpdateOwnerTextClientRpc(ownerId.Value, townLevel.Value);
    }

    [ClientRpc]
    public void UpdateOwnerTextClientRpc(int ownerId, int townLevel,bool onlyChangeText=false)
    {
        if (ownerId != -1) GetComponent<MeshRenderer>().material = GameLogic.Instance.PlayerColors[ownerId];
        else GetComponent<MeshRenderer>().material = startMaterial;
        
        if (townLevel < 0) displayPropertyUI.ShowNormalView(ownerId, townLevel, 0, onlyChangeText);
        else displayPropertyUI.ShowNormalView(ownerId, townLevel, specialTileScript.GetPayAmount(), onlyChangeText);
    }

    public void ShowSellingView()
    {
        int totalPropertyValue = specialTileScript.CaluculatePropertyValue();
        displayPropertyUI.ShowSellingView(totalPropertyValue);
    }

}

public enum TileType
{
    TownTile,
    StartTile,
    CustodyTile,
    PatrolTile,
    PartyTile,
    ShopTile,
    PrisonTile,
    GangTile,
    ChanceTile,
    BonusTile,
    SpecialTile,
    BillTile,
    MelangeTile

}

public enum PropertyType
{
    None,
    Alcohol,
    Drugs,
    Prostitution,
    Gambling
}