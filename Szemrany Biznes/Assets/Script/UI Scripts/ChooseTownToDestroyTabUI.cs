using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChooseTownToDestroyTabUI : MonoBehaviour
{
    public static ChooseTownToDestroyTabUI Instance {  get; private set; }

    public Button DamageButton;

    public TileScript choosenTile;

    public int damageAmount = 10;

    public void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    private void Start()
    {
        DamageButton.onClick.AddListener(() =>
        {
            choosenTile.SetTownDamageServerRpc(damageAmount);
            GameUIScript.OnNextPlayerTurn.Invoke();
            foreach (TileScript tileScript in GameLogic.Instance.allTileScripts)
            {
                if(tileScript.displayPropertyUI == null) continue;
                if (tileScript.displayPropertyUI.toggle.enabled)
                {
                    tileScript.UpdateOwnerTextServerRpc();
                }                
            }
            Hide();
        });
    }

    public void Show(int damageAmount = 10)
    {
        this.damageAmount = damageAmount;
        choosenTile = null;
        DamageButton.interactable = false;
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(true);
        }
    }

    private void ChangeButtonInteraction()
    {
        
        DamageButton.interactable = (choosenTile == null) ? false : true;
        
        
    }


    public void UpdateSelectedTile(TileScript tile, bool value)
    {
        if (transform.GetChild(0).gameObject.activeSelf)
        {
            if (choosenTile != null && choosenTile != tile) choosenTile.displayPropertyUI.toggle.isOn = false;
            choosenTile = null;
            if (value) choosenTile = tile;
            ChangeButtonInteraction();
        }
    }

    public void Hide()
    {
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(false);
        }
    }
}
