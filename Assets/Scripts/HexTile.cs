using UnityEngine;

public class HexTile
{
    public int Q { get; private set; }
    public int R { get; private set; }
    public GameObject TileObject { get; private set; }

    public HexTile(int q, int r, GameObject tileObject)
    {
        Q = q;
        R = r;
        TileObject = tileObject;
    }
}