using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using System;
using UnityEditor.AI;
using Unity.AI.Navigation;
using Unity.VisualScripting;

[CreateAssetMenu(fileName = "MapGenerator", menuName = "Scriptable Objects/MapGenerator")]
public class MapGenerator : ScriptableObject
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
    private List<Tile> tiles;
    [SerializeField]
    private GameObject tilePrefab;
    [SerializeField]
    private GameObject otherTilesCanvas;
    [SerializeField]
    private GameObject townTilesCanvas;
    public void Generate()
    {
        FindAnyObjectByType<GameLogic>().SpawnPoints = new List<Transform>();
        GameObject board = new GameObject("Board");
        board.transform.position = new Vector3(0,0,0);
        //Fist corner
        Vector3 startPosition = new Vector3((tileSize.x / 10) * (width - 2) + cornerTileSize.x/10, 0, (tileSize.x / 10) * (height - 2) + cornerTileSize.y/10);
        CreateCornerTileInWorld(0, startPosition, ref board,0);
        //Second corner
        startPosition = new Vector3((tileSize.x / 10) * (height - 2) + cornerTileSize.y / 10, 0, 0);
        CreateCornerTileInWorld(1, startPosition, ref board,90);
        //Third corner
        startPosition = new Vector3(0, 0, 0);
        CreateCornerTileInWorld(2, startPosition, ref board,180);
        //Fourth corner
        startPosition = new Vector3(0, 0, (tileSize.x / 10) * (height-2) + cornerTileSize.y / 10);
        CreateCornerTileInWorld(3, startPosition, ref board,-90);
        int index = 4;
        startPosition = new Vector3(tileSize.x / 10 * (width - 2) + cornerTileSize.x / 10, 0, tileSize.x / 10 * (width - 2) + cornerTileSize.x / 10 - ((cornerTileSize.y / 20) + (tileSize.x / 20)));
        for (int i = 0; i < height - 2; i++)
        {

            CreateTileInWorld(90, startPosition, tiles[index], ref board);
            startPosition -= new Vector3(0, 0, tileSize.x / 10);
            index++;
        }
        startPosition = new Vector3((tileSize.x / 10 * (width - 2) + cornerTileSize.x / 10) - ((cornerTileSize.x / 20) + (tileSize.x / 20)), 0, 0);
        for (int i = 0; i < width - 2; i++)
        {

            CreateTileInWorld(0, startPosition, tiles[index], ref board);
            startPosition -= new Vector3(tileSize.x / 10, 0, 0);
            index++;
        }
        startPosition = new Vector3(0, 0, (cornerTileSize.y / 20) + (tileSize.x / 20));
        for (int i = 0; i < height - 2; i++)
        {

            CreateTileInWorld(-90, startPosition, tiles[index], ref board);
            startPosition += new Vector3(0, 0, tileSize.x / 10);
            index++;
        }
        startPosition = new Vector3((cornerTileSize.y / 20) + (tileSize.x / 20), 0, tileSize.x / 10 * (width - 2) + cornerTileSize.x / 10);
        for (int i = 0; i < height - 2; i++)
        {

            CreateTileInWorld(0, startPosition, tiles[index],ref board);
            startPosition += new Vector3(tileSize.x / 10, 0,0) ;
            index++;
        }

        SetMonopolsForAllTiles(board);

        board.AddComponent<NavMeshSurface>().BuildNavMesh();
    }

    private void CreateTileInWorld(float rotation, Vector3 position,Tile tile,ref GameObject board)
    {
        GameObject tileObject = Instantiate(tilePrefab, position, Quaternion.Euler(new Vector3(0, rotation, 0)), board.transform);
        tileObject.name = tile.GetName();
        tileObject.transform.localScale = new Vector3(tileSize.x / 100, 1, tileSize.y / 100);
        tileObject.GetComponent<MeshRenderer>().material = tile.GetMaterial();
        switch (tile.GetTileType())
        {
            case TileScript.TileType.TrainTile:
            case TileScript.TileType.LightbulbTile:
            case TileScript.TileType.WaterPiepesTile:
            case TileScript.TileType.TownTile:
                Instantiate(townTilesCanvas, tileObject.transform);
                break;
            default:
                Instantiate(otherTilesCanvas, tileObject.transform);
                break;

        }
        TileScript tileScript = tileObject.GetComponent<TileScript>();
        tileScript.amountMoneyOnPlayerStep = tile.GetAmountOfMoneyOnStep();
        tileScript.tileType = tile.GetTileType();
        tileScript.index = tile.GetIndex();
        tileScript.townCostToBuy = tile.GetTownCostToBuy();
        tileScript.townCostToPay = tile.GetTownCostToPay();

    }

    private void CreateCornerTileInWorld(int index, Vector3 startPosition, ref GameObject board,float rotation)
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
        Instantiate(otherTilesCanvas, tile.transform);

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

    private void SetMonopolsForAllTiles(GameObject board)
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

}

[Serializable]
public class Tile
{
    [SerializeField] private int index;
    [SerializeField] private string name;
    [SerializeField] private TileScript.TileType type;
    [SerializeField] private List<int> townCostToBuy;
    [SerializeField] private List<int> townCostToPay;
    [SerializeField] private int amountOfMoneyOnStep;
    [SerializeField] private List<int> indexOfTileWhichMakesMonopol;
    [SerializeField] private Material material;


    public TileScript.TileType GetTileType()
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

}
