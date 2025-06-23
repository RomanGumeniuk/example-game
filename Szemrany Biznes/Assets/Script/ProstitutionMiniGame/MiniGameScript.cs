using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MiniGameScript : MonoBehaviour
{

    [SerializeField] private GameObject basicSkillCheck;
    [SerializeField] private GameObject clickSkillCheck;
    [SerializeField] private GameObject rectangleSkillCheck;
    [SerializeField] private GameObject expandingSkillCheck;

    [SerializeField] private TextMeshProUGUI instrictionText;
    string instruction = "Musisz trafiæ 5 skill checków pod rz¹d ¿eby zdobyæ nagrode. \nPrzygotuj siê!!!";
    public float radius = 5f;
    private int hitMarkMinRotation = -90;
    private int hitMarkMaxRotation = -250;
    bool spacePressed = false;
    bool isHit = false;
    public Vector2 GetRandomPointInCircle()
    {
        return (Vector2)transform.position + Random.insideUnitCircle * radius;
    }
    public async Task ShowBasicSkillCheck(float speed)
    {
        GameObject basicSkillCheck = Instantiate(this.basicSkillCheck, GetRandomPointInCircle(),Quaternion.identity,transform);
        float rotation = Random.Range(hitMarkMaxRotation, hitMarkMinRotation);
        basicSkillCheck.transform.GetChild(1).eulerAngles = new Vector3(0, 0, rotation);
        Transform arrow = basicSkillCheck.transform.GetChild(3);
        Transform hitMark = arrow.parent.GetChild(1);
        for (int i = 0; i < 360 / speed; i++)
        {
            await Awaitable.FixedUpdateAsync();
            arrow.transform.rotation = Quaternion.Euler(0, 0, i * speed * -1);
            if (!spacePressed) continue;

            spacePressed = false;
            if (arrow.eulerAngles.z <= hitMark.eulerAngles.z && arrow.eulerAngles.z >= hitMark.eulerAngles.z + (360 * hitMark.GetComponent<Image>().fillAmount * -1))
            {
                Debug.Log("hit");
                Destroy(arrow.parent.gameObject);
                isHit = true;
                await Awaitable.FixedUpdateAsync();
            }
            else
            {
                Destroy(basicSkillCheck, 1);
                Debug.Log("miss");
            }
            return;
        }
        Destroy(basicSkillCheck, 1);
    }
    

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            ShowRectangleSkillCheck(4);
        }
        if (Input.GetKeyDown(KeyCode.Y))
        {
            ShowBasicSkillCheck(5);
        }
        if (Input.GetKeyDown(KeyCode.U))
        {
            ShowExpandingSkillCheck(2.5f,1.1f);
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            spacePressed = true;
        }
    }

    public async void ShowClickSkillCheck()
    {
        GameObject clickSkillCheck = Instantiate(this.clickSkillCheck, GetRandomPointInCircle(), Quaternion.identity, transform);
        bool isClicked = false;
        clickSkillCheck.GetComponentInChildren<Button>().onClick.AddListener(() =>
        {
            isClicked = true;
        });
        for (int i = 0; i < 100; i++)
        {
            await Awaitable.FixedUpdateAsync();
            clickSkillCheck.transform.localScale = new Vector3(clickSkillCheck.transform.localScale.x - 0.1f, clickSkillCheck.transform.localScale.y - 0.1f, clickSkillCheck.transform.localScale.z);
            if(isClicked)
            {
                Debug.Log("hit");
                Destroy(clickSkillCheck);
                isHit = true;
                break;
            }
        }
        if(!isClicked)
        {
            Destroy(clickSkillCheck, 1);
            Debug.Log("miss");
        }

        
    }


    public async Task ShowRectangleSkillCheck(float speed)
    {
        GameObject rectangleSkillCheck = Instantiate(this.rectangleSkillCheck, GetRandomPointInCircle(), Quaternion.identity, transform);
        Transform hitMark = rectangleSkillCheck.transform.GetChild(2);
        hitMark.localPosition = new Vector3( Random.value * 10,0,0);
        Transform arrow = rectangleSkillCheck.transform.GetChild(3);
        for (int i=0;i<245/ speed; i++)
        {
            await Awaitable.FixedUpdateAsync();
            arrow.transform.localPosition -= new Vector3(0.1f* speed, 0,0);
            if (!spacePressed) continue;

            spacePressed = false;
            if (arrow.localPosition.x >= hitMark.localPosition.x && arrow.localPosition.x <= hitMark.localPosition.x + (25 * hitMark.GetComponent<Image>().fillAmount))
            {
                Debug.Log("hit");
                Destroy(rectangleSkillCheck);
                isHit = true;
            }
            else
            {
                Debug.Log("miss");
                Destroy(rectangleSkillCheck, 1);
            }
            return;
        }
        Destroy(rectangleSkillCheck,1);
        Debug.Log("miss");
    }


    public async Task ShowExpandingSkillCheck(float speed, float hitMarkArea)
    {
        GameObject expandingSkillCheck = Instantiate(this.expandingSkillCheck, GetRandomPointInCircle(), Quaternion.identity, transform);
        Transform hitMark = expandingSkillCheck.transform.GetChild(0);
        Transform innerCircle = expandingSkillCheck.transform.GetChild(1);
        hitMark.localScale = new Vector3(hitMarkArea, hitMarkArea, hitMarkArea);
        Transform arrow = expandingSkillCheck.transform.GetChild(2);
        for (int i = 0; i < hitMark.localScale.x*100 / speed; i++)
        {
            await Awaitable.FixedUpdateAsync();
            arrow.transform.localScale += new Vector3(0.01f * speed, 0.01f * speed, 0);
            if (!spacePressed) continue;

            spacePressed = false;
            if (arrow.localScale.x <= hitMark.localScale.x && arrow.localScale.x >= innerCircle.localScale.x)
            {
                Debug.Log("hit");
                Destroy(expandingSkillCheck);
                isHit = true;
            }
            else
            {
                Debug.Log("miss");
                Destroy(expandingSkillCheck, 1);
            }
            return;
        }
        Destroy(expandingSkillCheck,1);
        Debug.Log("miss");
    }




    public async void ShowMiniGame(int tier)
    {
        Debug.Log("aaa");
        foreach(Transform child in transform)
        {
            child.gameObject.SetActive(true);
        }
        for(int i =0;i<3; i++)
        {
            instrictionText.text = instruction + $"\n{3-i}"; 
            await Awaitable.WaitForSecondsAsync(1);
            Debug.Log("a");
        }
        instrictionText.gameObject.SetActive(false);
        spacePressed = false;
        switch (tier)
        {
            case 1:
                for(int i =0;i<5;i++)
                {
                    isHit = false;
                    await ShowBasicSkillCheck(4);
                    if(!isHit)
                    {
                        await Awaitable.WaitForSecondsAsync(1);
                        ShowFailiure();
                        return;
                    }
                }
                break;
            case 2:
                for (int i = 0; i < 3; i++)
                {
                    isHit = false;
                    await ShowBasicSkillCheck(5);
                    if (!isHit)
                    {
                        await Awaitable.WaitForSecondsAsync(1);
                        ShowFailiure();
                        return;
                    }
                }
                for (int i = 0; i < 2; i++)
                {
                    isHit = false;
                    await ShowRectangleSkillCheck(4);
                    if (!isHit)
                    {
                        await Awaitable.WaitForSecondsAsync(1);
                        ShowFailiure();
                        return;
                    }
                }
                break;
            case 3:
                isHit = false;
                await ShowBasicSkillCheck(5.5f);
                if (!isHit)
                {
                    await Awaitable.WaitForSecondsAsync(1);
                    ShowFailiure();
                    return;
                }
                for (int i = 0; i < 3; i++)
                {
                    isHit = false;
                    await ShowRectangleSkillCheck(5);
                    if (!isHit)
                    {
                        await Awaitable.WaitForSecondsAsync(1);
                        ShowFailiure();
                        return;
                    }
                }
                isHit = false;
                await ShowBasicSkillCheck(5.5f);
                if (!isHit)
                {
                    await Awaitable.WaitForSecondsAsync(1);
                    ShowFailiure();
                    return;
                }
                break;
            case 4:
                isHit = false;
                await ShowBasicSkillCheck(6);
                if (!isHit)
                {
                    await Awaitable.WaitForSecondsAsync(1);
                    ShowFailiure();
                    return;
                }
                for (int i = 0; i < 3; i++)
                {
                    isHit = false;
                    await ShowRectangleSkillCheck(5.5f);
                    if (!isHit)
                    {
                        await Awaitable.WaitForSecondsAsync(1);
                        ShowFailiure();
                        return;
                    }
                }
                isHit = false;
                await ShowExpandingSkillCheck(2.5f,1.1f);
                if (!isHit)
                {
                    await Awaitable.WaitForSecondsAsync(1);
                    ShowFailiure();
                    return;
                }
                break;
            case 5:
                isHit = false;
                await ShowExpandingSkillCheck(2.75f,1.1f);
                if (!isHit)
                {
                    await Awaitable.WaitForSecondsAsync(1);
                    ShowFailiure();
                    return;
                }
                isHit = false;
                await ShowRectangleSkillCheck(6);
                if (!isHit)
                {
                    await Awaitable.WaitForSecondsAsync(1);
                    ShowFailiure();
                    return;
                }
                await ShowBasicSkillCheck(6.5f);
                if (!isHit)
                {
                    await Awaitable.WaitForSecondsAsync(1);
                    ShowFailiure();
                    return;
                }
                isHit = false;
                await ShowExpandingSkillCheck(2.75f, 1.1f);
                if (!isHit)
                {
                    await Awaitable.WaitForSecondsAsync(1);
                    ShowFailiure();
                    return;
                }
                await ShowExpandingSkillCheck(2.75f, 1.1f);
                if (!isHit)
                {
                    await Awaitable.WaitForSecondsAsync(1);
                    ShowFailiure();
                    return;
                }
                break;
            case 6:
                isHit = false;
                await ShowExpandingSkillCheck(3f, 1.1f);
                if (!isHit)
                {
                    await Awaitable.WaitForSecondsAsync(1);
                    ShowFailiure();
                    return;
                }
                isHit = false;
                await ShowRectangleSkillCheck(6.5f);
                if (!isHit)
                {
                    await Awaitable.WaitForSecondsAsync(1);
                    ShowFailiure();
                    return;
                }
                await ShowBasicSkillCheck(7f);
                if (!isHit)
                {
                    await Awaitable.WaitForSecondsAsync(1);
                    ShowFailiure();
                    return;
                }
                isHit = false;
                await ShowExpandingSkillCheck(3f, 1.1f);
                if (!isHit)
                {
                    await Awaitable.WaitForSecondsAsync(1);
                    ShowFailiure();
                    return;
                }
                isHit = false;
                await ShowRectangleSkillCheck(6.5f);
                if (!isHit)
                {
                    await Awaitable.WaitForSecondsAsync(1);
                    ShowFailiure();
                    return;
                }
                break;
        }
        ShowSuccess(tier);
    }

    private async void ShowFailiure()
    {
        PlayerScript.LocalInstance.ChangeCantMoveValueServerRpc(1);
        Hide();
        await AlertTabForPlayerUI.Instance.ShowTab("Przegra³eœ!!\nCzekasz jedn¹ turê",2,false);
    }

    private async void ShowSuccess(int tier)
    {
        Hide();
        await AlertTabForPlayerUI.Instance.ShowTab("Wygra³eœ!!!", 2,false);
        switch (tier)
        {
            case 1:
                PlayerScript.LocalInstance.character.characterAdvantagesAndDisadvantages.Add(new Modifiers("Happines after sex",5,TypeOfModificator.CharacterAdvantages,ModifiersType.Precentage,2,0,true, TypeOfCharacterAdvantageOrDisadvantage.HappinesAfterSex));
                break;
            case 2:
                PlayerScript.LocalInstance.character.characterAdvantagesAndDisadvantages.Add(new Modifiers("Happines after sex", 10, TypeOfModificator.CharacterAdvantages, ModifiersType.Precentage, 2, 0, true, TypeOfCharacterAdvantageOrDisadvantage.HappinesAfterSex));
                break;
            case 3:
                PlayerScript.LocalInstance.character.characterAdvantagesAndDisadvantages.Add(new Modifiers("Happines after sex", 20, TypeOfModificator.CharacterAdvantages, ModifiersType.Precentage, 2, 0, true, TypeOfCharacterAdvantageOrDisadvantage.HappinesAfterSex));
                break;
            case 4:
                PlayerScript.LocalInstance.character.characterAdvantagesAndDisadvantages.Add(new Modifiers("Happines after sex", 30, TypeOfModificator.CharacterAdvantages, ModifiersType.Precentage, 2, 0, true, TypeOfCharacterAdvantageOrDisadvantage.HappinesAfterSex));
                break;
            case 5:
                PlayerScript.LocalInstance.character.characterAdvantagesAndDisadvantages.Add(new Modifiers("Happines after sex", 40, TypeOfModificator.CharacterAdvantages, ModifiersType.Precentage, 3, 0, true, TypeOfCharacterAdvantageOrDisadvantage.HappinesAfterSex));
                break;
            case 6:
                PlayerScript.LocalInstance.character.characterAdvantagesAndDisadvantages.Add(new Modifiers("Happines after sex", 50, TypeOfModificator.CharacterAdvantages, ModifiersType.Precentage, 3, 0, true, TypeOfCharacterAdvantageOrDisadvantage.HappinesAfterSex));
                break;
        }

        
    }


    private void Hide()
    {
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(false);
        }
        ProstitutionUITab.Instance.Hide();
    }
}
