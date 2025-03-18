using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class ProstitutionMinigame : MonoBehaviour
{
    public static ProstitutionMinigame Instance { get; private set; }

    public void Awake()
    {
        Instance = this;
    }
    
    GameObject mainImage;
    





    Queue<SkillCheck> skillChecksQueue = new Queue<SkillCheck>();
    public async Task<bool> StartMiniGame()
    {
        startAmountOfSkillChecks = skillChecksQueue.Count;
        for(int i =0;i<skillChecksQueue.Count;i++)
        {
            SkillCheck skillCheck =  skillChecksQueue.Dequeue();
            skillCheck.OnSkillCheckCreate(mainImage.transform);
            amountOfActiveSkillChecks++;
            await Awaitable.WaitForSecondsAsync(skillCheck.delayTime);
        }
        while(amountOfActiveSkillChecks >0)
        {
            await Awaitable.WaitForSecondsAsync(0.1f);
        }
        return amountOfCompletedSkillChecks == startAmountOfSkillChecks;
    }
    int amountOfActiveSkillChecks = 0;
    int amountOfCompletedSkillChecks = 0;
    int startAmountOfSkillChecks = 0;
    public void SkillCheckReturn(bool isSkillCheckCompleted)
    {
        if(isSkillCheckCompleted)amountOfCompletedSkillChecks++;
        amountOfActiveSkillChecks--;
    }


}


public class SkillCheck
{
    public GameObject prefab;
    public float delayTime;
    public float durationTime;

    private float timeLeft;
    public async void OnSkillCheckCreate(Transform parent)
    {
        Vector3 spawnPosition = parent.position + (Vector3)Random.insideUnitCircle*10;
        GameObject skillCheck = GameObject.Instantiate(prefab,spawnPosition,Quaternion.identity,parent);
        timeLeft = durationTime;
        RawImage skillCheckImage = skillCheck.GetComponent<RawImage>();
        skillCheck.GetComponent<Button>().onClick.AddListener(() =>
        {
            ProstitutionMinigame.Instance.SkillCheckReturn(true);
        });
        while (timeLeft>0)
        {
            timeLeft -= 0.1f;
            await Awaitable.WaitForSecondsAsync(0.1f);
            skillCheck.GetComponent<RawImage>().color = new Color(skillCheckImage.color.r, skillCheckImage.color.g, skillCheckImage.color.b, (255 / durationTime) * timeLeft);
        }
        ProstitutionMinigame.Instance.SkillCheckReturn(false);
    }
}
