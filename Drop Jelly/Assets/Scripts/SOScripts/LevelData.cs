using System.Collections.Generic;
using UnityEngine;
using ww.DropJelly;

[CreateAssetMenu(fileName = "NewLevelData", menuName = "Level Data", order = 0)]
public class LevelData : ScriptableObject
{
    public int levelNumber;
    public int targetScore;
    public int maxMoves;
    public List<ParentTileData> levelTiles = new List<ParentTileData>();
    public List<ParentTileData> tilesToMove = new List<ParentTileData>();

}
