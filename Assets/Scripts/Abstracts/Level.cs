using UnityEngine;

[CreateAssetMenu(fileName = "Level Asset", menuName = "Content/Peak Game Sample Level", order = 1)]

public class Level : ScriptableObject
{
    public int maxMoves, goal1, goal2, levelID;
    public TileType goal1Type, goal2Type;

    public string dataOfTileOrder;
    public int width, height;

    public Level(int maxMoves, int goal1, int goal2, int levelID, TileType goal1Type, TileType goal2Type, string dataOfTileOrder, int width, int height)
    {
        this.maxMoves = maxMoves;
        this.goal1 = goal1;
        this.goal2 = goal2;
        this.levelID = levelID;
        this.goal1Type = goal1Type;
        this.goal2Type = goal2Type;
        this.dataOfTileOrder = dataOfTileOrder;
        this.width = width;
        this.height = height;
    }
}
