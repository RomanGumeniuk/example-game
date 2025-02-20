using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class ChooseingTileUI : MonoBehaviour, IQueueWindows
{
    public static ChooseingTileUI Instance { get; private set; }

    public void Awake()
    {
        Instance = this;
    }

    Image previousImage;
    TileScript choosenTile;
    [SerializeField] Color tileColor;
    [SerializeField] Color selecedTileColor;
    public async Task<TileScript> Show()
    {
        PlayerScript.LocalInstance.AddToQueueOfWindows(this);
        while(!returnTile)
        {
            await Awaitable.WaitForSecondsAsync(0.1f);
        }
        Hide();
        returnTile = false;
        return choosenTile;
    }

    void ShowWindow()
    {
        choosenTile = null;
        for (int i = 0; i < GameLogic.Instance.allTileScripts.Count; i++)
        {
            DisplayPropertyUI propertyUI = GameLogic.Instance.allTileScripts[i].gameObject.GetComponentInChildren<DisplayPropertyUI>();
            if(propertyUI != null)
            {
                propertyUI.backgroundImage.gameObject.SetActive(true);
                propertyUI.backgroundImage.color = tileColor;
                continue;
            }
            
            
            Image tileImage = GameLogic.Instance.allTileScripts[i].gameObject.GetComponentInChildren<Canvas>().GetComponentInChildren<Image>();
            tileImage.enabled = true;
            tileImage.color = tileColor;
        }
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(true);
        }
        stopCursorChecking = false;
        GetCursorTile();
    }
    bool stopCursorChecking = false;
    public async void GetCursorTile()
    {
        while (!stopCursorChecking)
        {
            await Awaitable.EndOfFrameAsync();
            
            if (!EventSystem.current.IsPointerOverGameObject()) continue;
            //Debug.Log("a");
            PointerEventData pointerEventData = new PointerEventData(EventSystem.current);
            pointerEventData.position = Input.mousePosition;

            List<RaycastResult> raycastResults = new List<RaycastResult>();
            EventSystem.current.RaycastAll(pointerEventData, raycastResults);
            bool foundTile = false;
            for (int i = 0; i < raycastResults.Count; i++)
            {
                //Debug.Log(raycastResults[i].gameObject.name);
                if (raycastResults[i].gameObject.name != "VisibleImage") continue;
                DisplayPropertyUI propertyUI = raycastResults[i].gameObject.transform.parent.parent.GetComponent<DisplayPropertyUI>();
                TileScript currentTileScript = null;
                if (propertyUI != null)
                {
                    currentTileScript = propertyUI.tileScript;
                }
                else
                {
                    currentTileScript = raycastResults[i].gameObject.transform.parent.parent.GetComponent<TileScript>();
                }
                foundTile = true;
                if (choosenTile == currentTileScript) continue;
                if (choosenTile != null)
                {
                    previousImage.color = tileColor;
                    previousImage.rectTransform.localScale = Vector3.one;
                }
                choosenTile = currentTileScript;
                previousImage = raycastResults[i].gameObject.GetComponent<Image>();
                previousImage.rectTransform.localScale = new Vector3(1.1f, 1.1f, 1.1f);
                previousImage.color = selecedTileColor;
            }
            if (!foundTile && choosenTile !=null)
            {
                choosenTile = null;
                previousImage.rectTransform.localScale = Vector3.one;
                previousImage.color = tileColor;
            }
            
        }
    }
    bool returnTile = false;

    private void Update()
    {
        if(Input.GetMouseButtonDown(0) && choosenTile!=null)
        {
            Debug.Log(choosenTile.name);
            returnTile = true;
        }
    }

    public void Hide()
    {
        for (int i = 0; i < GameLogic.Instance.allTileScripts.Count; i++)
        {
            DisplayPropertyUI propertyUI = GameLogic.Instance.allTileScripts[i].gameObject.GetComponentInChildren<DisplayPropertyUI>();
            if (propertyUI != null)
            {
                propertyUI.backgroundImage.gameObject.SetActive(false);
                continue;
            }
            GameLogic.Instance.allTileScripts[i].gameObject.GetComponentInChildren<Canvas>().GetComponentInChildren<Image>().enabled = false;
            
        }
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(false);
        }
        stopCursorChecking = true;
        PlayerScript.LocalInstance.GoToNextAction();
    }

    public void ResumeAction()
    {
        ShowWindow();
    }
}
