using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class AlertTabForPlayerUI : MonoBehaviour
{
    public static AlertTabForPlayerUI Instance { get; private set; }

    public TextMeshProUGUI alertText;

    private void Awake()
    {
        Instance = this;
    }

    public void ShowTab(string alertText, float secondsIsVisible, bool invokeNextPlayer = true)
    {
        StartCoroutine(Show(alertText,secondsIsVisible,invokeNextPlayer));
    }

    private IEnumerator Show(string alertText, float secondsIsVisible,bool invokeNextPlayer=true)
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
        yield return new WaitForSeconds(secondsIsVisible);
        foreach (Transform child in transform) child.gameObject.SetActive(false);
    }
}
