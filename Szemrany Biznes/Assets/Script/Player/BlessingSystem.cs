using System.Collections.Generic;
using Unity.Netcode;
using System;
using UnityEngine;
using System.Threading.Tasks;

public class BlessingSystem : NetworkBehaviour
{
    List<Blessing> blessings = new List<Blessing>();

    
    public void OnPlayerTurnEnd()
    {
        foreach(Blessing b in blessings)
        {
            b.DecreasAmountOfTurnLeft();
            if (b.GetAmountOfTurnLeft() < 1)
            {
                blessings.Remove(b);
            }
        }
    }



    public void AddBlessing(Blessing blessing)
    {
        bool found = false;
        foreach (Blessing b in blessings)
        {
            if (b.GetName() == blessing.GetName())
            {
                found = true;
                b.SetAmountOfTurnLeft(blessing.GetAmountOfTurnLeft());
            }
        }
        DoSthIfNesesery(blessing);
        if (found) return;

    }

    private void DoSthIfNesesery(Blessing blessing)
    {
        switch(blessing.GetName())
        {
            case "":

                break;
        }
    }



    public bool CheckBlessing(string blessingName)
    {
        
        bool found = false;
        foreach (Blessing b in blessings)
        {
            if (b.GetName() == blessingName) found = true;
        }
        return found;

    }

}
[Serializable]
public class Blessing
{
    [SerializeField]
    string name;
    [SerializeField]
    int amountOfTurnLeft;


    public string GetName()
    {
        return name;
    }

    public void SetAmountOfTurnLeft(int amountOfTurnLeft)
    {
        this.amountOfTurnLeft = amountOfTurnLeft;
    }

    public int GetAmountOfTurnLeft()
    {
        return amountOfTurnLeft;
    }

    public void DecreasAmountOfTurnLeft()
    {
        amountOfTurnLeft--;
    }
}

