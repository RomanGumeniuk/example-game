using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GamblingTabUI : MonoBehaviour
{
    bool isDone;

    public static GamblingTabUI Instance { get; private set; }

    private TileScript currentGamblingTileScript;

    [Header("Start View")]
    [SerializeField] private TextMeshProUGUI mainTitle;

    [SerializeField] private TextMeshProUGUI gameTitle;
    [SerializeField] private RawImage gameImage;


    [SerializeField] private TextMeshProUGUI fieldTitle;
    [SerializeField] private TextMeshProUGUI maxWin;


    [SerializeField] private TextMeshProUGUI gameDescription;
    [SerializeField] private TextMeshProUGUI currentBetAmount;
    [SerializeField] private TextMeshProUGUI betCost;
    [SerializeField] private Slider betSlider;
    [SerializeField] private Button betSubstractButton;
    [SerializeField] private Button betAddButton;

    [SerializeField] private Button gamePlayButton;
    [SerializeField] private Button gameExitButton;
    private int startingBet = 0;
    private int additionalBet = 0;
    private int maximumBetAmount = 0;

    public async Task Show(TileScript currentGamblingTileScript)
    {
        Debug.Log("DejTeTuDbuga");
        isDone = false;

        foreach (Transform child in transform)
        {
            Debug.Log("Nyga: " + child.name);
            child.gameObject.SetActive(true);
        }
        this.currentGamblingTileScript = currentGamblingTileScript;
        maximumBetAmount = currentGamblingTileScript.specialTileScript.GetPayAmount();
        startingBet = Mathf.CeilToInt(currentGamblingTileScript.specialTileScript.GetPayAmount() * 0.2f);

        betSlider.minValue = startingBet;
        betSlider.maxValue = maximumBetAmount;
        betSlider.value = startingBet;
        ChangeBetValue(startingBet);
        ChangeBetCostValue(0);

        mainTitle.text = $"Witaj w {GameLogic.Instance.drugsTownNames[currentGamblingTileScript.townLevel.Value-1]} Sukinsynie";

        switch (currentGamblingTileScript.townLevel.Value)
        {
            case 1:
                HigherLower();
                break;
            case 2:
                OneHandBandit();
                break;
            case 3:
                Next();
                break;
            case 4:
                ScrapingGame();
                break;
            case 5:
                Roulette();
                break;
            case 6:
                Blackjack();
                break;
        }


        while (!isDone)
        {
            await Awaitable.WaitForSecondsAsync(0.2f);
        }
    }

    private void Awake()
    {
        if(Instance==null)
            Instance = this;
    }

    private void Start()
    {
        gameExitButton.onClick.AddListener(() => { 
            isDone = true;
            Hide();
        });
        gamePlayButton.onClick.AddListener(() => {
            // PLAY
        });
        betSlider.onValueChanged.AddListener((float value) => {
            OnBetChange((int)(startingBet+value));
            additionalBet = (int)value;
        });
        betSubstractButton.onClick.AddListener(() => {
            if(additionalBet==startingBet) return;
            additionalBet--;
            OnBetChange((int)(startingBet + additionalBet));
        });
        betAddButton.onClick.AddListener(() => {
            if (additionalBet == maximumBetAmount) return;
            additionalBet++;
            OnBetChange((int)(startingBet + additionalBet));
        });
    }

    public void Hide()
    {
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(false);
        }
    }

    private void HigherLower()
    {
        gameTitle.text = "Higher Lower";
        gameImage.texture = null; // TO DO
        gameDescription.text = "Cel gry:\r\nTwoim zadaniem jest zgadywanie, czy kolejna karta bêdzie wy¿sza czy ni¿sza od poprzedniej.\r\nGra zaczyna siê od pierwszej karty, któr¹ zobaczysz na ekranie\r\nPo zobaczeniu karty, musisz zdecydowaæ:\r\nCzy natêpna jest \"Higher\"?\r\nCzy jest \"Lower\"?\r\nJeœli odgadniesz prawid³owo, zdobywasz punkt i przechodzisz do nastêpnej rundy."; //TO DO
    }
    private void OneHandBandit()
    {

    }
    private void Next()
    {

    }
    private void ScrapingGame()
    {

    }
    private void Roulette()
    {

    }
    private void Blackjack()
    {

    }

    private void ChangeBetValue(int currentBet)
    {
        currentBetAmount.text = $"{currentBet} PLN";
    }
    private void ChangeBetCostValue(int currentCost)
    {
        betCost.text = $"{currentCost} PLN";
    }

    private void OnBetChange(int value)
    {
        ChangeBetValue(value);
        ChangeBetCostValue(value-startingBet);
    }
}
