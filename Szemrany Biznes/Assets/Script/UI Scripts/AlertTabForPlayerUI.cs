using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System;
using Unity.Netcode;

public class AlertTabForPlayerUI : NetworkBehaviour, IQueueWindows
{
    public static AlertTabForPlayerUI Instance { get; private set; }

    public TextMeshProUGUI alertText;

    private void Awake()
    {
        Instance = this;
    }


    public async Task ShowTab(string alertText, float secondsIsVisible, bool invokeNextPlayer = true)
    {
        currentInvokePlayer.Enqueue(invokeNextPlayer);
        currentAlertText.Enqueue(alertText);
        currentSecondsIsVisible.Enqueue(secondsIsVisible);
        PlayerScript.LocalInstance.AddToQueueOfWindows(this);
        //Debug.Log("a");
        while(true)
        {
            await Awaitable.WaitForSecondsAsync(0.1f);
            Debug.Log(alertText + " " + currentInvokePlayer.Count);
            if (currentInvokePlayer.Count == 0) break;
            if (currentAlertText.Peek() != alertText) continue;
            if (currentInvokePlayer.Peek() != invokeNextPlayer) continue;
            if (currentSecondsIsVisible.Peek() != secondsIsVisible) continue;
            break;
        }
        //Debug.Log("b");
        while(true)
        {
            await Awaitable.WaitForSecondsAsync(0.1f);
            if (currentAlertText.Count!=0 && currentAlertText.Peek() == alertText) continue;
            if (currentInvokePlayer.Count != 0 && currentInvokePlayer.Peek() == invokeNextPlayer) continue;
            if (currentSecondsIsVisible.Count != 0 && currentSecondsIsVisible.Peek() == secondsIsVisible) continue;
            //Debug.Log("c");
            if (isDone) break;
        }
        //Debug.Log("d");

    }
    Queue<bool> currentInvokePlayer = new Queue<bool>();
    Queue<string> currentAlertText = new Queue<string>();
    Queue<float> currentSecondsIsVisible = new Queue<float>();

    bool isDone = false;
    public async Task ShowTab()
    {
        isDone = false;
        if (currentInvokePlayer.Dequeue()) GameUIScript.OnNextPlayerTurn.Invoke();
        try
        {
            this.alertText.text = currentAlertText.Dequeue();
        }
        catch 
        {
            Debug.Log(this.alertText);
        }
        foreach (Transform child in transform) child.gameObject.SetActive(true);
        await Awaitable.WaitForSecondsAsync(currentSecondsIsVisible.Dequeue());
        foreach (Transform child in transform) child.gameObject.SetActive(false);
        PlayerScript.LocalInstance.GoToNextAction();
        isDone = true;
    }


    public async Task ShowTabForOtherPlayer(string alertText,float secondsIsVisible,int playerIndex)
    {
        ShowTabServerRpc(alertText, secondsIsVisible,playerIndex);
        await Awaitable.WaitForSecondsAsync(secondsIsVisible);
    }

    [ServerRpc(RequireOwnership = false)]
    private void ShowTabServerRpc(string alertText, float secondsIsVisible,int playerIndex)
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

    public void ResumeAction()
    {
        _ = ShowTab();
    }
}
