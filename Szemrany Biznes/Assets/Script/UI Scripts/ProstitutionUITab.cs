using UnityEngine;

public class ProstitutionUITab : MonoBehaviour, IQueueWindows
{
    public static ProstitutionUITab Instance { get; private set; }

    void Awake()
    {
        Instance = this;
    }


    TileScript tile;
    public GameObject minigame;
    public void Show(TileScript tile)
    {
        this.tile = tile;
        PlayerScript.LocalInstance.AddToQueueOfWindows(this);
    }

    private void Show()
    {
        foreach (Transform child in transform)
        {
            if (child.name == minigame.name) continue;
            child.gameObject.SetActive(true);
        }
    }

    public void Hide()
    {
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(false);
        }
        PlayerScript.LocalInstance.GoToNextAction();
    }

    public void ResumeAction()
    {
        Show();
    }
}
