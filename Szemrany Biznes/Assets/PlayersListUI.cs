using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using TMPro;

public class PlayersListUI : NetworkBehaviour
{
    /*
    public static PlayersListUI Instance { get; private set; }

    public List<Transform> AllPlayersPrefabList = new List<Transform>();

    private void Awake()
    {
        Instance = this;
    }
    [ServerRpc(RequireOwnership = false)]
    public void AddPlayerToListServerRpc(int index)
    {
        AllPlayersPrefabList.Add(GameLogic.Instance.allPlayersListPrefab[index]);
    }

    [ServerRpc(RequireOwnership =false)]
    public void RefreshListServerRpc()
    {
        for(int i=0;i<AllPlayersPrefabList.Count;i++)
        {
            AllPlayersPrefabList[i].GetComponentInChildren<TextMeshProUGUI>().text = GameLogic.Instance.PlayersOrder[i]+"PLN";
        }
    }
    */


}
