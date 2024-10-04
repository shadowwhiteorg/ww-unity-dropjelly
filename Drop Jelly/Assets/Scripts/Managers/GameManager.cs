using UnityEngine;
using ww.Utilities.Singleton;
namespace ww.DropJelly
{
    internal class GameManager : Singleton<GameManager>
    {

        private int currentScore = 0;
        public int CurrentScore
        {
            get => currentScore;
            set
            {
                currentScore = value;
                //UIManager.Instance.UpdateScore(currentScore);
            }
        }

        private void Start()
        {
            //TileManager.Instance.InitializeParentTilesOnBoart();
            //TileManager.Instance.InitializeSubtilesOnBoard();
            BoardManager.Instance.InitBoard();
            LevelManager.Instance.InitActiveParentTile();
        }
    }
}
