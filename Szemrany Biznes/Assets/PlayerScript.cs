using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class PlayerScript : NetworkBehaviour
{
    public int directionX = 1;
    public int directionZ = 0;
    public int currentTileIndex = 0;

    public static PlayerScript LocalInstance { get; private set; }

    public override void OnNetworkSpawn()
    {
        if(IsOwner)
        {
            LocalInstance = this;
        }
    }


    private void Update()
    {
        if (!IsOwner) return;
        if(Input.GetKey(KeyCode.A))
        {
            transform.position = new Vector3(transform.position.x - (1 * Time.deltaTime), transform.position.y, transform.position.z);
        }
    }

    public void GoTo(Vector3 postition)
    {
        transform.position = postition;
    }

    public void Move(int diceValue)
    {
        //Debug.Log(diceValue);
        for(int i=0;i<diceValue;i++)
        {
            if(currentTileIndex%8==0)
            {
                //Debug.Log(currentTileIndex);
                switch (currentTileIndex)
                {
                    case 0:
                        directionX = 0;
                        directionZ = 1;
                        break;
                    case 8:
                        directionX = 1;
                        directionZ = 0;
                        break;
                    case 16:
                        directionX = 0;
                        directionZ = -1;
                        break;
                    case 24:
                        directionX = -1;
                        directionZ = 0;
                        break;
                }
            }
            transform.position = new Vector3(transform.position.x + 1.5f * directionX, transform.position.y, transform.position.z + 1.5f * directionZ);
            if (currentTileIndex != (7 * 4) + 3) currentTileIndex++;
            else currentTileIndex = 0;
        }
        
    }


}
