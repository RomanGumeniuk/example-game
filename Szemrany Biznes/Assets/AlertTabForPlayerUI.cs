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

    public IEnumerator ShowTab(string alertText, float secondsIsVisible)
    {
        this.alertText.text = alertText;
        foreach (Transform child in transform) child.gameObject.SetActive(true);
        yield return new WaitForSeconds(secondsIsVisible);
        foreach (Transform child in transform) child.gameObject.SetActive(false);
    }
}
