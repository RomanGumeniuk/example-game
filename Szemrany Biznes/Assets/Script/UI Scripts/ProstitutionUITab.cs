using UnityEngine;
using UnityEngine.UI;

public class ProstitutionUITab : MonoBehaviour, IQueueWindows
{
    public static ProstitutionUITab Instance { get; private set; }

    void Awake()
    {
        Instance = this;
    }


    TileScript tile;
    public MiniGameScript minigame;
    [SerializeField] private Button take;
    [SerializeField] private Button exit;

    private void Start()
    {
        take.onClick.AddListener(() =>
        {
            minigame.ShowMiniGame(tile.townLevel.Value);
        });

        exit.onClick.AddListener(() =>
        {


        });
    }


    public void Show(TileScript tile)
    {
        this.tile = tile;
        PlayerScript.LocalInstance.AddToQueueOfWindows(this);
    }

    private void Show()
    {
        foreach (Transform child in transform)
        {
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
