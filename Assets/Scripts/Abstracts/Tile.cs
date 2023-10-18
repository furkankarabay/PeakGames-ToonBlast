using UnityEngine;

public class Tile
{
    public Vector2Int coordinates;
    public TileType tileType;
    public int orderInLayer;

    public Tile(Vector2Int coordinates, TileType type, int orderInLayer)
    {
        this.coordinates = coordinates;
        this.tileType = type;
        this.orderInLayer = orderInLayer;
    }
}
