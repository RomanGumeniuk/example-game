using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EndResultsTabUI : MonoBehaviour
{
    public static EndResultsTabUI Instance { get; private set; }

    public GameObject content;
    public GameObject contentPrefab;

    [SerializeField] Button mainMenu;
    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        mainMenu.onClick.AddListener(() =>
        {
            var go = new GameObject("Sacrificial Lamb");
            DontDestroyOnLoad(go);

            foreach (var root in go.scene.GetRootGameObjects())
                Destroy(root);
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        });
    }

    public void Show()
    {
        List<NetworkClient> playerOrder = GameLogic.Instance.PlayersOrder;
        for(int i=0;i<playerOrder.Count;i++)
        {
            PlayerResultsPrefab playerResultsPrefab = Instantiate(contentPrefab, content.transform).GetComponent<PlayerResultsPrefab>();
            PlayerScript playerScript = playerOrder[i].PlayerObject.GetComponent<PlayerScript>();
            playerResultsPrefab.playerName.text = playerScript.character.name + "#" + i;
            playerResultsPrefab.playerMoney.text = playerScript.totalAmountOfMoney.Value + "PLN";
        }
        foreach(Transform child in transform)
        {
            child.gameObject.SetActive(true);
        }
    }
}
