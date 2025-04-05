using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "BlessingDataBaseSO", menuName = "Scriptable Objects/BlessingDataBaseSO")]
public class BlessingDataBaseSO : ScriptableObject
{
    [SerializeField]
    List<Blessing> listOfAllBlessings = new List<Blessing>();

    public List<Blessing> GetAllBlessings()
    {
        return listOfAllBlessings;
    }
}
