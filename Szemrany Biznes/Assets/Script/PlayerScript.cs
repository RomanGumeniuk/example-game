using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class PlayerScript : NetworkBehaviour
{
    public int directionX = 1;
    public int directionZ = 0;
    public int currentTileIndex = 0;
    public int playerIndex;
    public NetworkVariable<int> amountOfMoney = new NetworkVariable<int>( 3000);
    public int playerId;
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
        for(int i=0;i<diceValue;i++)
        {
            if(currentTileIndex%10==0)
            {
                switch (currentTileIndex)
                {
                    case 0:
                        directionX = -1;
                        directionZ = 0;
                        
                        transform.position = GameLogic.Instance.SpawnPoints[0].GetChild(playerIndex).transform.position;
                        if(i!=0)GameLogic.Instance.allTileScripts[currentTileIndex].OnPlayerEnter();
                        if(i+1>=diceValue)
                        {
                            currentTileIndex = 0;
                            return;
                        }
                        break;
                    case 10:
                        directionX = 0;
                        directionZ = 1;
                        transform.position = GameLogic.Instance.SpawnPoints[1].GetChild(playerIndex).transform.position;
                        break;
                    case 20:
                        directionX = 1;
                        directionZ = 0;
                        transform.position = GameLogic.Instance.SpawnPoints[2].GetChild(playerIndex).transform.position;
                        break;
                    case 30:
                        directionX = 0;
                        directionZ = -1;
                        transform.position = GameLogic.Instance.SpawnPoints[3].GetChild(playerIndex).transform.position;
                        break;
                }
            }
            transform.position = new Vector3(transform.position.x + 1.17f * directionX, transform.position.y, transform.position.z + 1.17f * directionZ);
            if (currentTileIndex != (9 * 4) + 3) currentTileIndex++;
            else currentTileIndex = 0;
        }
        GameLogic.Instance.allTileScripts[currentTileIndex].OnPlayerEnter();
        
    }


}
