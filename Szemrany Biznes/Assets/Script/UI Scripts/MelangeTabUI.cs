using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using System.Linq;

public class MelangeTabUI : NetworkBehaviour
{
    public static MelangeTabUI Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }
    [ServerRpc(RequireOwnership = false)]
    public void OnPlayerEnterServerRpc(int startingPlayerIndex)
    {
        foreach (NetworkClient client in GameLogic.Instance.PlayersOrder)
        {
            if((int)client.ClientId  == startingPlayerIndex)
            {
                continue;
            }
            _ = AlertTabForPlayerUI.Instance.ShowTabForOtherPlayer($"Zosta³eœ zaproszony na melan¿.\nRzucasz trzema ró¿nymi kostkami", 3.5f, (int)client.ClientId);
            
        }
        WaitForAlert(startingPlayerIndex);
        
    }

    private async void WaitForAlert(int startingPlayerIndex)
    {
        await AlertTabForPlayerUI.Instance.ShowTabForOtherPlayer($"Zapraszasz wszystkich graczy na melan¿.\nRzucasz trzema ró¿nymi kostkami", 3.5f, startingPlayerIndex);
        DiceResultsForAllPlayers();
    }

    private async void DiceResultsForAllPlayers()
    {
        GameLogic gameLogic = GameLogic.Instance;
        DiceSpawn diceSpawn = DiceSpawn.Instance;
        for (int i = 0; i < gameLogic.allPlayerAmount; i++)
        {
            int playerId = (int)gameLogic.PlayersOrder[i].ClientId;
            diceSpawn.RollTheDiceServerRpc(playerId, 1, true, false, DiceType.EightSide,1);
            diceSpawn.RollTheDiceServerRpc(playerId, 1, true, false, DiceType.SixSide,1);
            diceSpawn.RollTheDiceServerRpc(playerId, 1, true, false, DiceType.FourSide,1);
        }
        
        GameLogic.Instance.IncreasCallNextPlayerTurnServerRpc();
        GameUIScript.OnNextPlayerTurn.Invoke();
        Dictionary<int, int> allDiceResults = await DiceSpawn.Instance.GetAllDiceValues();
        for (int i = 0; i < allDiceResults.Count; i++)
        {
            Debug.Log(allDiceResults.ElementAt(i).Key + " " + allDiceResults.ElementAt(i).Value);
            switch (allDiceResults.ElementAt(i).Value)
            {
                case 3:
                    Debug.Log($"Party {allDiceResults.ElementAt(i).Key}");
                    OnPartyArivesMilioner(allDiceResults.ElementAt(i).Key);
                    break;
                case 4:
                case 5:
                case 6:
                    Debug.Log($"EveryoneBuy {allDiceResults.ElementAt(i).Key}");
                    BuyEveryoneSomething(allDiceResults.ElementAt(i).Key);
                    break;
                case 7:
                case 8:
                case 9:
                case 10:
                    Debug.Log($"Magic menel {allDiceResults.ElementAt(i).Key}");
                    MagicMenel(allDiceResults.ElementAt(i).Key);
                    break;
                case 11:
                case 12:
                case 13:
                case 14:
                    Debug.Log($"Trip {allDiceResults.ElementAt(i).Key}");
                    Trip(allDiceResults.ElementAt(i).Key);
                    break;
                case 15:
                case 16:
                case 17:
                    Debug.Log($"Attacked {allDiceResults.ElementAt(i).Key}");
                    YouAreAttacked(allDiceResults.ElementAt(i).Key);
                    break;
                case 18:
                    Debug.Log($"Jackpot {allDiceResults.ElementAt(i).Key}");
                    Jackpot(allDiceResults.ElementAt(i).Key);
                    break;
            }
        }



        GameLogic.Instance.DecreaseCallNextPlayerTurnServerRpc();
    }


    void OnPartyArivesMilioner(int playerIndex)
    {
        int amountOfMoney = 1000;
        foreach (NetworkClient client in GameLogic.Instance.PlayersOrder)
        {
            _ =AlertTabForPlayerUI.Instance.ShowTabForOtherPlayer($"Na impreze wbija milioner i daje ka¿demu kase. Dostajerz {amountOfMoney} PLN" + (playerIndex == (int)client.ClientId ? " Dostajesz jeszcze dragi" : ""), 2, (int)client.ClientId);
            GameLogic.Instance.UpdateMoneyForPlayerServerRpc(amountOfMoney, (int)client.ClientId, 2, false, true);
        }
        ClientRpcParams clientRpcParams = new ClientRpcParams
        {
            Send = new ClientRpcSendParams
            {
                TargetClientIds = new ulong[] { (ulong)playerIndex }
            }
        };
        Item item = GameLogic.Instance.itemDataBase.GetRandomNumberOfItems(1, new ItemType[] { ItemType.Drug }, new ItemTier[] { ItemTier.Exclusive })[0];
        PlayerScript.LocalInstance.AddItemToInventoryClientRpc(GameLogic.Instance.itemDataBase.GetIndexOfItem(item), clientRpcParams);
        Debug.Log(item.GetName());
    }

    void BuyEveryoneSomething(int playerIndex)
    {
        int cost = 200 * GameLogic.Instance.allPlayerAmount;
        foreach (NetworkClient client in GameLogic.Instance.PlayersOrder)
        {
            if (client.ClientId == (ulong)playerIndex)
            {
                if (client.PlayerObject.GetComponent<PlayerScript>().amountOfMoney.Value >= cost)
                {
                    GameLogic.Instance.UpdateMoneyForPlayerServerRpc(cost, playerIndex, 1, false, true);
                    _ = AlertTabForPlayerUI.Instance.ShowTabForOtherPlayer("Zap³aci³eœ za shoty dla ka¿dego. -" + cost + "PLN", 2, playerIndex);
                    continue;
                }
                client.PlayerObject.GetComponent<PlayerScript>().ShowSellingTab(cost, -1);
                continue;
            }
            ClientRpcParams clientRpcParams = new ClientRpcParams
            {
                Send = new ClientRpcSendParams
                {
                    TargetClientIds = new ulong[] { client.ClientId }
                }
            };
            _ = AlertTabForPlayerUI.Instance.ShowTabForOtherPlayer("Dostajesz wódkê za darmo", 2, (int)client.ClientId);
            Item item = GameLogic.Instance.itemDataBase.GetRandomNumberOfItems(1, new ItemType[] { ItemType.Alcohol }, new ItemTier[] { ItemTier.Decent })[0];
            PlayerScript.LocalInstance.AddItemToInventoryClientRpc(GameLogic.Instance.itemDataBase.GetIndexOfItem(item), clientRpcParams);
            Debug.Log(item.GetName());
        }
    }

    void Jackpot(int playerIndex)
    {
        int win = 10000;
        _ = AlertTabForPlayerUI.Instance.ShowTabForOtherPlayer($"Ziomek mówi ze wygra³ w Lotto. Dostajesz {win}PLN, dostajesz równie¿ jeden alkohol AMBROSSIA", 4, playerIndex);
        GameLogic.Instance.UpdateMoneyForPlayerServerRpc(win, playerIndex, 2, false, true);
        Item item = GameLogic.Instance.itemDataBase.GetRandomNumberOfItems(1, new ItemType[] { ItemType.Alcohol }, new ItemTier[] { ItemTier.Exclusive })[0];
        ClientRpcParams clientRpcParams = new ClientRpcParams
        {
            Send = new ClientRpcSendParams
            {
                TargetClientIds = new ulong[] { (ulong)playerIndex }
            }
        };
        PlayerScript.LocalInstance.AddItemToInventoryClientRpc(GameLogic.Instance.itemDataBase.GetIndexOfItem(item), clientRpcParams);
        Debug.Log(item.GetName());
    }


    void YouAreAttacked(int playerIndex)
    {
        int lost = 400;
        _ = AlertTabForPlayerUI.Instance.ShowTabForOtherPlayer($"Ziomala mówi ¿e chce ci coœ pokazaæ w kiblu. Pokazuje ci gnata i mówi ze to napad. Tracisz wszystkie kontrabandy i {lost} PLN", 4, playerIndex);
        GameLogic.Instance.UpdateMoneyForPlayerServerRpc(lost, playerIndex, 1, false, true);
        ClientRpcParams clientRpcParams = new ClientRpcParams
        {
            Send = new ClientRpcSendParams
            {
                TargetClientIds = new ulong[] { (ulong)playerIndex }
            }
        };
        PlayerScript.LocalInstance.ClearAllIllegalItemsClientRpc(clientRpcParams);
    }

    void Trip(int playerIndex)
    {
        int found = 2000;
        _ = AlertTabForPlayerUI.Instance.ShowTabForOtherPlayer($"Idziesz na tripa po okolicy - teleport na losowe miejsce na mapie i znalaz³eœ {found} PLN i przedmiot klasy zwykle.", 4, playerIndex);
        GameLogic.Instance.UpdateMoneyForPlayerServerRpc(found, playerIndex, 2, false, true);
        Item item = GameLogic.Instance.itemDataBase.GetRandomNumberOfItems(1, null, new ItemTier[] { ItemTier.Normal })[0];
        ClientRpcParams clientRpcParams = new ClientRpcParams
        {
            Send = new ClientRpcSendParams
            {
                TargetClientIds = new ulong[] { (ulong)playerIndex }
            }
        };
        int randomIndex = UnityEngine.Random.Range(0, GameLogic.Instance.mapGenerator.GetSize());
        Debug.Log(playerIndex);
        PlayerScript.LocalInstance.TeleportToTileClientRpc(randomIndex, clientRpcParams);
        PlayerScript.LocalInstance.AddItemToInventoryClientRpc(GameLogic.Instance.itemDataBase.GetIndexOfItem(item), clientRpcParams);
        Debug.Log(item.GetName());
    }

    void MagicMenel(int playerIndex)
    {
        int paid = 1500;
        _ = AlertTabForPlayerUI.Instance.ShowTabForOtherPlayer($"Poszed³eœ na petka a tu przyszed³ czarodziej (menel na cracku) i mówi ze za {paid} PLN da ci b³ogos³awieñstwo. Tracisz kase a menel znika", 4, playerIndex);
        GameLogic.Instance.UpdateMoneyForPlayerServerRpc(paid, playerIndex, 1, false, true);
    }
}

