using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

public class AlertTabForPlayerUI : MonoBehaviour
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
}
