using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;

public class TestUIScript : MonoBehaviour
{
    [SerializeField] private Button statHostButton;
    [SerializeField] private Button statClientButton;

    private void Awake()
    {
        statHostButton.onClick.AddListener(() =>
        {
            NetworkManager.Singleton.StartHost();
            Hide();
        });

        statClientButton.onClick.AddListener(() =>
        {
            NetworkManager.Singleton.StartClient();
            Hide();
        });
    }


    private void Hide()
    {
        gameObject.SetActive(false);
    }
}
