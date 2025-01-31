using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class BooksUI : MonoBehaviour
{
    [SerializeField] private RawImage[] bookImages; // Lista obrazków

    [Header("Guide UI")]
    [SerializeField] private Image GuideUIBackground;
    [SerializeField] private RawImage GuideUIBook;
    [SerializeField] private Image GuideUIBookmarks;
    [SerializeField] private Image GuideUIClose;

    private void Start()
    {
        foreach (RawImage img in bookImages)
        {
            img.gameObject.AddComponent<ImageSegment>(); // Dodanie skryptu do ka¿dego obrazka
        }
    }

    public void OnBookClicked()
    {
        GuideUIBackground.gameObject.SetActive(true);
        GuideUIBook.gameObject.SetActive(true);
        GuideUIBookmarks.gameObject.SetActive(true);
        GuideUIClose.gameObject.SetActive(true);
    }
}
