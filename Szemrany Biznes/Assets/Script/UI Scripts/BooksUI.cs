using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

public class BooksUI : MonoBehaviour
{
    private RaycastHit2D raycastHit2D;
    [SerializeField] private Transform[] bookImages; // Lista obrazków
    [SerializeField] private Texture BlackSlide;
    [SerializeField] private Texture RedSlide;

    [Header("Guide UI")]
    [SerializeField] private Image GuideUIBackground;
    [SerializeField] private RawImage GuideUIBook;
    [SerializeField] private Image GuideUIBookmarks;
    [SerializeField] private Image GuideUIClose;

    public void Start()
    {
        StartCoroutine(CheckMouseOverUI());
    }

    public void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            foreach (Transform child in bookImages)
            {
                if (child.GetComponent<RawImage>().color == Color.red) {
                    Debug.Log("Clicked" + child.name);
                    //TUTAJ WPEIRDOL DOJENAY KOD 
                };
            }
        }
    }

    IEnumerator CheckMouseOverUI()
    {
        while (true)
        {
            yield return 0;
            raycastHit2D = Physics2D.Raycast(Input.mousePosition, Vector2.zero);
            foreach (Transform child in bookImages)
            {
                if (raycastHit2D.collider?.name == child.name)
                {
                    child.GetComponent<RawImage>().color = Color.red;
                    child.GetComponent<RawImage>().texture = RedSlide;
                    Debug.Log(child.name);

                }
                else
                {
                    child.GetComponent<RawImage>().color = Color.black;
                    child.GetComponent<RawImage>().texture = BlackSlide;
                }
            }
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
