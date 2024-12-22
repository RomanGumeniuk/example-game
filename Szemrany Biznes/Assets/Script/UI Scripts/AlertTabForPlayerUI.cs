using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System;
using Unity.Netcode;

public class AlertTabForPlayerUI : NetworkBehaviour
{
    public static AlertTabForPlayerUI Instance { get; private set; }

    public TextMeshProUGUI alertText;

    private void Awake()
    {
        Instance = this;
    }


    public async Task ShowTab(string alertText, float secondsIsVisible,bool invokeNextPlayer=true)
    {
        if (invokeNextPlayer) GameUIScript.OnNextPlayerTurn.Invoke();
        try
        {
            this.alertText.text = alertText;
        }
        catch 
        {
            Debug.Log(this.alertText);
        }
        foreach (Transform child in transform) child.gameObject.SetActive(true);
        await Awaitable.WaitForSecondsAsync(secondsIsVisible);
        foreach (Transform child in transform) child.gameObject.SetActive(false);
    }


    public void ShowTabForOtherPlayer(string alertText,float secondsIsVisible,int playerIndex)
    {
        

        ShowTabServerRpc(alertText, secondsIsVisible,playerIndex);
    }

    [ServerRpc(RequireOwnership = false)]
    public void ShowTabServerRpc(string alertText, float secondsIsVisible,int playerIndex)
    {
        ClientRpcParams clientRpcParams = new ClientRpcParams
        {
            Send = new ClientRpcSendParams
            {
                TargetClientIds = new ulong[] { (ulong)playerIndex }
            }
        };
        ShowTabClientRpc(alertText, secondsIsVisible,clientRpcParams);
    }

    [ClientRpc]
    public void ShowTabClientRpc(string alertText, float secondsIsVisible,ClientRpcParams clientRpcParams = default)
    {
        _ = ShowTab(alertText, secondsIsVisible, false);
    }


}
