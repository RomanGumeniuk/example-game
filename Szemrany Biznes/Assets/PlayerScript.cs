using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class PlayerScript : NetworkBehaviour
{
    private void Update()
    {
        
        if(Input.GetKey(KeyCode.A))
        {
            transform.position = new Vector3(transform.position.x - (1 * Time.deltaTime), transform.position.y, transform.position.z);
        }
    }
}
