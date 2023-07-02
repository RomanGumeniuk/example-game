using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class PlayerScript : NetworkBehaviour
{
    private void Start()
    {
        transform.position = new Vector3(-4.25f, transform.position.y, -4.25f);
    }


    private void Update()
    {
        if (!IsOwner) return;
        if(Input.GetKey(KeyCode.A))
        {
            transform.position = new Vector3(transform.position.x - (1 * Time.deltaTime), transform.position.y, transform.position.z);
        }
    }
}
