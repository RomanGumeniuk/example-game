using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;

public class TestUIScript : MonoBehaviour
{
    public static TestUIScript Instance {  get; private set; }
    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        foreach(Transform child in transform)
        {
            child.gameObject.SetActive(true);
        }
        
    }
    
}
