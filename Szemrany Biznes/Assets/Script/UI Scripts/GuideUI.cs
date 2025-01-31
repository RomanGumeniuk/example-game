using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.UI;

public class GuideUI : MonoBehaviour
{
    [Header("Content")]
    [TextArea]
    [SerializeField] private string[] chaptersContent;
    [Space]

    [Header("Guide Book View")]
    [SerializeField] private TextMeshProUGUI leftPage;
    [SerializeField] private TextMeshProUGUI rightPage;
    [Space]
    [SerializeField] private TextMeshProUGUI leftPageNum;
    [SerializeField] private TextMeshProUGUI rightPageNum;
    [SerializeField] private EventTrigger[] chapterTabs;


    [Header("Game UI View")]
    [SerializeField] private Button RollDiceButton;
    [SerializeField] private Button GuideBookButton;

    private int currentChapterIndex = 0;

    private void Awake()
    {
        SetupEventTriggers();
        LoadChapter(currentChapterIndex);
    }

    public void Hide()
    {
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(false);
        }

        RollDiceButton.gameObject.SetActive(true);
        GuideBookButton.gameObject.SetActive(true);
    }

    private void SetupEventTriggers()
    {
        for (int i = 0; i < chapterTabs.Length; i++)
        {
            int chapterIndex = i;
            EventTrigger.Entry entry = new EventTrigger.Entry
            {
                eventID = EventTriggerType.PointerClick
            };
            entry.callback.AddListener((data) => { LoadChapter(chapterIndex); });
            chapterTabs[i].triggers.Add(entry);
        }
    }


    private void LoadChapter(int chapterIndex)
    {
        if (chapterIndex < 0 || chapterIndex >= chaptersContent.Length)
        {
            return;
        }

        currentChapterIndex = chapterIndex;
        SetPagesContent(chaptersContent[chapterIndex]);
        ResetPageNumbers();
    }

    private void SetPagesContent(string content)
    {
        leftPage.text = content;
        rightPage.text = content;
    }

    private void ResetPageNumbers()
    {
        leftPage.pageToDisplay = 1;
        rightPage.pageToDisplay = 2;
        SetPagesNumbers();
    }

    private void SetPagesNumbers()
    {
        leftPageNum.text = leftPage.pageToDisplay.ToString();
        rightPageNum.text = rightPage.pageToDisplay.ToString();
    }

    public void GoPreviousPage()
    {
        if (leftPage.pageToDisplay < 1)
        {
            leftPage.pageToDisplay = 1;
            return;
        }
        if (leftPage.pageToDisplay - 2 > 1)
        {
            leftPage.pageToDisplay -= 2;
        }
        else
        {
            leftPage.pageToDisplay = 1;
        }
        rightPage.pageToDisplay = leftPage.pageToDisplay + 1;

        SetPagesNumbers();
    }

    public void GoNextPage()
    {
        if (rightPage.pageToDisplay >= rightPage.textInfo.pageCount)
        {
            return;
        }
        else
        {
            leftPage.pageToDisplay += 2;
            rightPage.pageToDisplay = leftPage.pageToDisplay + 1;
        }
        SetPagesNumbers();
    }
}
