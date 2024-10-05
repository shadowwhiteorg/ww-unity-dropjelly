using UnityEngine;
using ww.Utilities.Singleton;
namespace ww.DropJelly
{
    internal class GameManager : Singleton<GameManager>
    {

        private bool _isGameActive = true;
        public bool IsGameActive => _isGameActive;

        [SerializeField]
        private int _currentMove;
        public int CurrentMove
        {
            get
            {
                return _currentMove;
            }
            set
            {
                // Add your logic here
                _currentMove = value;
                UIManager.Instance.UpdateMoveText(_currentMove);
                Debug.Log("Current Move: " + _currentMove);
                if (_currentMove <= 0 && _isGameActive)
                {
                    _isGameActive = false;
                    EventManager.LevelFailed();
                }
            }
        }

        private int _currentTarget = 0;
        public int CurrentTarget
        {
            get
            {
                return _currentTarget;
            }
            set
            {
                // Add your logic here
                _currentTarget = value;
                UIManager.Instance.UpdateTargetText(_currentTarget);
                if (_currentTarget <= 0 && _isGameActive)
                {
                    _isGameActive = false;
                    EventManager.LevelComplete();
                }
            }
        }

        private void Start()
        {
            UIManager.Instance.Init();
            Init();
            TileHandler.Instance.Init();
            BoardManager.Instance.InitBoard();
        }
        private void Init()
        {
            _isGameActive = true;
            CurrentMove = LevelManager.Instance.CurrentLevelData.maxMoves;
            CurrentTarget = LevelManager.Instance.CurrentLevelData.targetScore;
        }

    }
}
