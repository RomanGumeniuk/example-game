using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using System;

[CreateAssetMenu(fileName = "MapGenerator", menuName = "Scriptable Objects/MapGenerator")]
public class MapGenerator : ScriptableObject
{
    [SerializeField]
    private int height;

    [SerializeField]
    private int width;

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
        Vector3 startPosition = new Vector3 (width/2 - cornerTileSize.x - tileSize.x/2, 0, (width / 2 - cornerTileSize.x - tileSize.x / 2)*-1);
        float rotation = 90;
        GameObject Board = Instantiate(new GameObject(),Vector3.zero,Quaternion.identity);
        Board.name = "Board";
        GameObject tile = Instantiate(tilePrefab,startPosition,Quaternion.identity, Board.transform);
        tile.transform.localScale = new Vector3(cornerTileSize.x / 100, 1, cornerTileSize.y / 100);
        TileScript tileScript = tile.GetComponent<TileScript>();
        switch (tiles[0].GetTileType())
        {
            case TileScript.TileType.TrainTile:
            case TileScript.TileType.LightbulbTile:
            case TileScript.TileType.WaterPiepesTile:
            case TileScript.TileType.TownTile:
                Instantiate(townTilesCanvas, tile.transform);
                break;
            default:
                Instantiate(otherTilesCanvas, tile.transform); 
                break;
        }
        tileScript.amountMoneyOnPlayerStep = tiles[0].GetAmountOfMoneyOnStep();
        tileScript.tileType = tiles[0].GetTileType();
        tileScript.townLevel.Value = 0;

        int sideCount = 0;
        for (int i=1;i<tiles.Count; i++)
        {
            sideCount++;
            if(sideCount==((rotation==0)?width:height))
            {
                sideCount = 0;
                if (rotation == -90)
                {
                    rotation = 0;
                }
                else rotation -= 90;
            }
            tile = Instantiate(tilePrefab, new Vector3(), Quaternion.identity, Board.transform);
            if(sideCount==0)tile.transform.localScale = new Vector3(cornerTileSize.x / 100, 1, cornerTileSize.y / 100);
            else tile.transform.localScale = new Vector3(tileSize.x / 100, 1, tileSize.y / 100);
            tileScript = tile.GetComponent<TileScript>();
            switch (tiles[i].GetTileType())
            {
                case TileScript.TileType.TrainTile:
                case TileScript.TileType.LightbulbTile:
                case TileScript.TileType.WaterPiepesTile:
                case TileScript.TileType.TownTile:
                    Instantiate(townTilesCanvas, tile.transform);
                    break;
                default:
                    Instantiate(otherTilesCanvas, tile.transform);
                    break;

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

}
