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
                _currentMove = value;
                if(_currentMove < 0)
                {
                    _currentMove = 0;
                }
                UIManager.Instance.UpdateMoveText(_currentMove);
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
                _currentTarget = value;
                if(_currentTarget < 0)
                    _currentTarget = 0;
                UIManager.Instance.UpdateTargetText(_currentTarget);
            }
        }


        private void Start()
        {
            UIManager.Instance.Init();
            Init();
            TileHandler.Instance.Init();
            BoardManager.Instance.InitGrid();
        }
        private void Init()
        {
            _isGameActive = true;
            CurrentMove = LevelManager.Instance.CurrentLevelData.maxMoves;
            CurrentTarget = LevelManager.Instance.CurrentLevelData.targetScore;
        }

        public void CheckLevelEndCondition()
        {
            if (_currentMove <= 0 && _isGameActive)
            {
                _isGameActive = false;
                EventManager.LevelFailed();
            }
            if (_currentTarget <= 0 && _isGameActive)
            {
                _isGameActive = false;
                EventManager.LevelComplete();
            }
        }


    }
}
