using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

public class MelangeTile : Tile
{
    public MelangeTile(TileScript tile)
    {
        tileScript = tile;
    }

    public override void OnPlayerStepped()
    {
        MelangeTabUI.Instance.OnPlayerEnterServerRpc(PlayerScript.LocalInstance.playerIndex);
    }

}
