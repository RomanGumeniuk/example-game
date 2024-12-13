using UnityEngine;
using TMPro;
using UnityEngine.UI;
public class DevUI : MonoBehaviour
{
    public static DevUI Instance {  get; private set; }
    [SerializeField]
    private TMP_InputField inputField;
    [SerializeField]
    private Button button;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        button.onClick.AddListener(() =>
        {
            GameUIScript.Instance.OnDiceNumberReturn(int.Parse(inputField.text));
            PlayerScript.LocalInstance.Move(int.Parse(inputField.text));
            GameUIScript.Instance.HideButton();
        });
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.P))
        {
            inputField.gameObject.SetActive(!inputField.gameObject.activeSelf);
            button.gameObject.SetActive(!button.gameObject.activeSelf);
        }
    }
}
