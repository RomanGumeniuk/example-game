using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

public class BooksUI : MonoBehaviour
{
    private RaycastHit2D raycastHit2D;
    [SerializeField] private RawImage Circle;
    [SerializeField] private Transform[] bookImages; // Lista obrazków
    [SerializeField] private Texture BlackSlide;
    [SerializeField] private Texture RedSlide;
    [SerializeField] private Texture BlackSlideBig;
    [SerializeField] private Texture RedSlideBig;

    [Header("Guide UI")]
    [SerializeField] private Image GuideUIBackground;
    [SerializeField] private RawImage GuideUIBook;
    [SerializeField] private Image[] guideUIBookmarks;
    [SerializeField] private Image GuideUIClose;

    [Header("Game UI View")]
    [SerializeField] private Button RollDiceButton;
    [SerializeField] private Button GuideBookButton;

    public void Start()
    {
        StartCoroutine(CheckMouseOverUI());
    }

    public void Update()
    {
        if(Input.GetMouseButtonDown(0) && Circle.IsActive())
        {
            foreach (Transform child in bookImages)
            {
                if (child.GetComponent<RawImage>().color == Color.red) {
                    Debug.Log("Clicked" + child.name);
                    switch (int.Parse(child.name))
                    {
                        case 0: //Wstep - Info o grze

                            OnBookClicked(0);
                            break;
                        case 1: //Zasady ogolne: Z1 - Start Gry, Z2 - Trwanie Gry, Z3 - Warunki Wygrania Gry, Z4 - Terminologia ogolna.

                            OnBookClicked(1);
                            break;
                        case 2: //Mapa: Z1 - Pola Zwyczajne, Z2 - Pola Specjalne, Z3 - Pola Naroznikowe (+ DZIALANIA)

                            OnBookClicked(2);
                            break;
                        case 3: //Postacie - Info ogolne

                            OnBookClicked(3);
                            break;
                        case 4: //Dzialki: Z1 - Info ogolne (Koszty i sciezki rozowju) , Z2 - Narkotyki, Z3 - Panienki, Z4 - Alkohol, Z5 - Hazard (+ DZIALANIA)

                            OnBookClicked(4);
                            break;
                        case 56: // Wyjscie z BooksUI
                           OnClose();
                            break;
                        case 7: // Przedmioty: Z1 - Informacje ogolne, Z2 - Przedmioty Klasa SMIECI, Z2 - Przedmioty Klasa ZWYKLE, Z3 - Przedmioty Klasa EKSKLUZYWNE, Z4 - Przedmioty Klasa SUPER TAJNE, Z5 - RELIKWIE

                            OnBookClicked(5);
                            break;

                    }
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
                    if (child.name == "56")
                    {
                        child.GetComponent<RawImage>().color = Color.red;
                        child.GetComponent<RawImage>().texture = RedSlideBig;
                    }
                    else
                    {
                        child.GetComponent<RawImage>().color = Color.red;
                        child.GetComponent<RawImage>().texture = RedSlide;
                    }

                }
                else
                {
                    if (child.name == "56")
                    {
                        child.GetComponent<RawImage>().color = Color.red;
                        child.GetComponent<RawImage>().texture = BlackSlideBig;
                    }
                    else
                    {
                        child.GetComponent<RawImage>().color = Color.black;
                        child.GetComponent<RawImage>().texture = BlackSlide;
                    }
                }
            }
        }
    }
    public void OnBookClicked(int guideUiBookmark)
    {
        GuideUIBackground.gameObject.SetActive(true);
        GuideUIBook.gameObject.SetActive(true);
        GuideUIClose.gameObject.SetActive(true);
        guideUIBookmarks[guideUiBookmark].gameObject.SetActive(true);

        Circle.gameObject.SetActive(false);
    }

    public void OnClose()
    {
        Circle.gameObject.SetActive(false);
        RollDiceButton.gameObject.SetActive(true);
        GuideBookButton.gameObject.SetActive(true);
    }
}
