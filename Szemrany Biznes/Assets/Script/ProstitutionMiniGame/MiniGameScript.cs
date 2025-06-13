using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class MiniGameScript : MonoBehaviour
{

    [SerializeField] private GameObject basicSkillCheck;
    [SerializeField] private GameObject clickSkillCheck;
    [SerializeField] private GameObject rectangleSkillCheck;
    public float radius = 5f;
    private int hitMarkMinRotation = -90;
    private int hitMarkMaxRotation = -250;
    public Vector2 GetRandomPointInCircle()
    {
        return (Vector2)transform.position + Random.insideUnitCircle * radius;
    }
    public async void ShowBasicSkillCheck()
    {
        GameObject basicSkillCheck = Instantiate(this.basicSkillCheck, GetRandomPointInCircle(),Quaternion.identity,transform);
        float rotation = Random.Range(hitMarkMaxRotation, hitMarkMinRotation);
        basicSkillCheck.transform.GetChild(1).eulerAngles = new Vector3(0, 0, rotation);
        await RotateArrow(basicSkillCheck.transform.GetChild(3),5f);
        try
        {
            Destroy(basicSkillCheck);
        }
        catch { }
    }
    bool spacePressed = false;
    private async Task RotateArrow(Transform arrow,float speed)
    {
        Transform hitMark = arrow.parent.GetChild(1);
        for(int i =0;i <360/speed; i++)
        {
            await Awaitable.FixedUpdateAsync();
            arrow.transform.rotation = Quaternion.Euler(0, 0, i * speed *-1);
            if (!spacePressed) continue;

            spacePressed = false;
            if (arrow.eulerAngles.z <= hitMark.eulerAngles.z && arrow.eulerAngles.z >= hitMark.eulerAngles.z + (360 * hitMark.GetComponent<Image>().fillAmount*-1))
            {
                Debug.Log("hit");
                Destroy(arrow.parent.gameObject);
                ShowBasicSkillCheck();
            }
            else
            {
                Debug.Log("miss");
                
            }
            return;
        }
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.T))
        {
            ShowRectangleSkillCheck(2);
        }
        if(Input.GetKeyDown(KeyCode.Space))
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
                break;
            }
        }
        if(!isClicked)
        {
            Debug.Log("miss");
        }
        Destroy(clickSkillCheck,1);
    }


    public async void ShowRectangleSkillCheck(float speed)
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
            }
            else
            {
                Debug.Log("miss");

            }
            return;
        }
        Debug.Log("miss");
    }

}
