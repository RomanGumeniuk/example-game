using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using System;
using Unity.AI.Navigation;
using TMPro;


[CreateAssetMenu(fileName = "MapGenerator", menuName = "Scriptable Objects/MapGenerator")]
public class MapGeneratorSO : ScriptableObject
{
    [SerializeField]
    private int height;

    [SerializeField]
    private int width;

    [SerializeField]
    private List<Vector3> spawnPoints;

    [SerializeField]
    private Vector2 tileSize;
    [SerializeField]
    private Vector2 cornerTileSize;
    [SerializeField]
    private List<TileProperties> tiles;
    [SerializeField]
    private GameObject tilePrefab;
    [SerializeField]
    private GameObject otherTilesCanvas;
    [SerializeField]
    private GameObject townTilesCanvas;
    [SerializeField] 
    private GameObject townButton;
    [SerializeField]
    private GameObject boardPrefab;

    private GameObject board;
    public void Generate()
    {
        FindAnyObjectByType<GameLogic>().SpawnPoints = new List<Transform>();
        GameObject boardMesh = Instantiate(boardPrefab,new Vector3(cornerTileSize.x/20+ tileSize.x / 10 * (width - 2)/2, 0, cornerTileSize.x / 20 + tileSize.x / 10 * (width - 2) / 2),Quaternion.identity);
        boardMesh.transform.localScale = new Vector3(tileSize.x/100*(width-2), tileSize.x/100 * (width - 2), tileSize.x/100 * (width - 2));
        boardMesh.name = "Board collider";
        board = new GameObject("Board");
        board.transform.position = Vector3.zero;
        //Fist corner
        Vector3 startPosition = new Vector3((tileSize.x / 10) * (width - 2) + cornerTileSize.x/10, 0, (tileSize.x / 10) * (height - 2) + cornerTileSize.y/10);
        CreateCornerTileInWorld(0, startPosition,0);
        //Second corner
        startPosition = new Vector3((tileSize.x / 10) * (height - 2) + cornerTileSize.y / 10, 0, 0);
        CreateCornerTileInWorld(1, startPosition,90);
        //Third corner
        startPosition = new Vector3(0, 0, 0);
        CreateCornerTileInWorld(2, startPosition,180);
        //Fourth corner
        startPosition = new Vector3(0, 0, (tileSize.x / 10) * (height-2) + cornerTileSize.y / 10);
        CreateCornerTileInWorld(3, startPosition,-90);
        int index = 4;
        startPosition = new Vector3(tileSize.x / 10 * (width - 2) + cornerTileSize.x / 10, 0, tileSize.x / 10 * (width - 2) + cornerTileSize.x / 10 - ((cornerTileSize.y / 20) + (tileSize.x / 20)));
        for (int i = 0; i < height - 2; i++)
        {

            CreateTileInWorld(-90, startPosition, tiles[index]);
            startPosition -= new Vector3(0, 0, tileSize.x / 10);
            index++;
        }
        startPosition = new Vector3((tileSize.x / 10 * (width - 2) + cornerTileSize.x / 10) - ((cornerTileSize.x / 20) + (tileSize.x / 20)), 0, 0);
        for (int i = 0; i < width - 2; i++)
        {

            CreateTileInWorld(0, startPosition, tiles[index]);
            startPosition -= new Vector3(tileSize.x / 10, 0, 0);
            index++;
        }
        startPosition = new Vector3(0, 0, (cornerTileSize.y / 20) + (tileSize.x / 20));
        for (int i = 0; i < height - 2; i++)
        {

            CreateTileInWorld(-90, startPosition, tiles[index]);
            startPosition += new Vector3(0, 0, tileSize.x / 10);
            index++;
        }
        startPosition = new Vector3((cornerTileSize.y / 20) + (tileSize.x / 20), 0, tileSize.x / 10 * (width - 2) + cornerTileSize.x / 10);
        for (int i = 0; i < height - 2; i++)
        {

            CreateTileInWorld(0, startPosition, tiles[index]);
            startPosition += new Vector3(tileSize.x / 10, 0,0) ;
            index++;
        }

        SetMonopolsForAllTiles();

        NavMeshSurface navMesh = board.AddComponent<NavMeshSurface>();
        navMesh.layerMask = 1 << 0;
        navMesh.BuildNavMesh();
        FindAnyObjectByType<GameLogic>().board = board;
    }

    private void CreateTileInWorld(float rotation, Vector3 position, TileProperties tile)
    {
        GameObject tileObject = Instantiate(tilePrefab, position, Quaternion.Euler(new Vector3(0, rotation, 0)), board.transform);
        tileObject.name = tile.GetName();
        tileObject.transform.localScale = new Vector3(tileSize.x / 100, 1, tileSize.y / 100);
        switch (tile.GetTileType())
        {
            case TileType.TownTile:
            case TileType.GangTile:
            case TileType.SpecialTile:
                GameObject townPrefab = Instantiate(townTilesCanvas, tileObject.transform);
                GameObject prefabButton = Instantiate(townButton, Vector3.zero,Quaternion.identity, townPrefab.transform);
                prefabButton.transform.localPosition = new Vector3(0, (((GetSize()*4)-4) / 2 >= tile.GetIndex()-1?-6.5f:6.5f), -0.0001f);
                prefabButton.transform.localRotation = Quaternion.Euler(0, 0, 0);
                townPrefab.GetComponent<DisplayPropertyUI>().SetUpDisplay(tile.GetName(), tile.GetMaterial(), tile.GetTownCostToBuy()[0]);
                if(tile.GetMaterial().color == Color.white || tile.GetMaterial().name == "Yellow")
                {
                    townPrefab.GetComponent<DisplayPropertyUI>().townCostToPay.color = Color.black;
                    townPrefab.GetComponent<DisplayPropertyUI>().townName.color = Color.black;
                }
                break;
            default:
                tileObject.GetComponent<MeshRenderer>().material = tile.GetMaterial();
                TextMeshProUGUI ui = Instantiate(otherTilesCanvas, tileObject.transform).GetComponentInChildren<TextMeshProUGUI>();
                ui.text = tile.GetName().Split(" ")[0];
                if(ui.transform.parent.parent.GetComponent<MeshRenderer>().sharedMaterials[0].color == Color.black) ui.color = Color.white;
                else ui.color = Color.black;
                break;

        }
        TileScript tileScript = tileObject.GetComponent<TileScript>();
        tileScript.amountMoneyOnPlayerStep = tile.GetAmountOfMoneyOnStep();
        tileScript.tileType = tile.GetTileType();
        tileScript.index = tile.GetIndex();
        tileScript.SetTownCostToBuy(tile.GetTownCostToBuy());
        tileScript.SetTownCostToPay(tile.GetTownCostToPay());

    }

    private void CreateCornerTileInWorld(int index, Vector3 startPosition,float rotation)
    {
        GameObject tile = Instantiate(tilePrefab, startPosition, Quaternion.identity, board.transform);
        tile.GetComponent<MeshRenderer>().material = tiles[index].GetMaterial();
        tile.name = tiles[index].GetName();
        tile.transform.localScale = new Vector3(cornerTileSize.x / 100, 1, cornerTileSize.y / 100);
        TileScript tileScript = tile.GetComponent<TileScript>();
        tileScript.amountMoneyOnPlayerStep = tiles[index].GetAmountOfMoneyOnStep();
        tileScript.tileType = tiles[index].GetTileType();
        tileScript.townLevel.Value = 0;
        tileScript.index = tiles[index].GetIndex();
        TextMeshProUGUI ui = Instantiate(otherTilesCanvas, tile.transform).GetComponentInChildren<TextMeshProUGUI>();
        ui.text = tiles[index].GetName();
        if (ui.transform.parent.parent.GetComponent<MeshRenderer>().sharedMaterials[0].color == Color.black) ui.color = Color.white;
        else ui.color = Color.black;


        GameObject playerSpawnPoints = new GameObject("PlayerSpawnPoints");
        playerSpawnPoints.transform.SetParent(tile.transform, false);
        playerSpawnPoints.transform.rotation = Quaternion.Euler(0,rotation,0);
        for (int i = 0; i < 8; i++)
        {
            GameObject point = new GameObject("Point " + i);
            point.transform.SetParent(playerSpawnPoints.transform, false);
            point.transform.localPosition = spawnPoints[i];
        }
        FindAnyObjectByType<GameLogic>().SpawnPoints.Add(playerSpawnPoints.transform);
    }

    







    private void SetMonopolsForAllTiles()
    {
        foreach (Transform child in board.transform)
        {
            TileScript tile = child.GetComponent<TileScript>();
            foreach (int index in tiles[tile.index].GetMonopolList())
            {
                tile.AllTownsToGetMonopol.Add(board.transform.GetChild(index).GetComponent<TileScript>());
            }
            
        }

    }

    

    public int GetSize()
    {
        return width;
    }

    public Vector2 GetTileSize()
    {
        return tileSize;
    }

    public Vector2 GetCornerTileSize()
    {
        return cornerTileSize;
    }

    public void ClearLists()
    {
        foreach(TileProperties tile in tiles)
        {
            for(int i=2;i<tile.GetTownCostToBuy().Count;i++)
            {
                tile.RemoveBuyCost(i);
            }
            for (int i = 0; i < tile.GetTownCostToPay().Count; i++)
            {
                tile.RemovePayCost(i);
            }
        }
    }

}

[Serializable]
public class TileProperties
{
    [SerializeField] private int index;
    [SerializeField] private string name;
    [SerializeField] private TileType type;
    [SerializeField] private List<int> townCostToBuy;
    [SerializeField] private List<int> townCostToPay;
    [SerializeField] private int amountOfMoneyOnStep;
    [SerializeField] private List<int> indexOfTileWhichMakesMonopol;
    [SerializeField] private Material material;


    public TileType GetTileType()
    {
        return type;
    }

    public string GetName()
    { 
        return name; 
    }

    public int GetIndex()
    {
        return index;
    }

    public List<int> GetTownCostToBuy()
    {
        return townCostToBuy;
    }

    public List<int> GetTownCostToPay()
    {
        return townCostToPay;
    }

    public List<int> GetMonopolList()
    {
        return indexOfTileWhichMakesMonopol;
    }
    public int GetAmountOfMoneyOnStep()
    {
        return amountOfMoneyOnStep;
    }

    public Material GetMaterial()
    {
        return material;
    }

    public void RemoveBuyCost(int index)
    {
        townCostToBuy.RemoveAt(index);
    }

    public void RemovePayCost(int index)
    {
        townCostToPay.RemoveAt(index);
    }
}
