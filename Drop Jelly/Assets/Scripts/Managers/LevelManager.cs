using System;
using System.Collections.Generic;
using UnityEngine;
using ww.Utilities.Singleton;

namespace ww.DropJelly
{
    internal class LevelManager : Singleton<LevelManager>
    {
        public event Action<int> OnLevelChanged;
        public event Action OnLevelCompleted;

        private int _currentLevel = 0;
        public int CurrentLevel => _currentLevel;
        private int _currentStep;
        public int CurrentStep => _currentStep;

        [SerializeField]
        private List<LevelData> _levelDatas = new List<LevelData>();

        private LevelData _currentLevelData;
        public LevelData CurrentLevelData { get => _levelDatas[_currentLevel]; }

        private void Start()
        {
            //InitActiveParentTile();
        }

        public void InitActiveParentTile()
        {
            ParentTile m_activeParentTile = Instantiate(BoardManager.Instance.ParentTilePrefab);
            m_activeParentTile.SetGridParams(4, 4, CurrentLevelData.tilesToMove[_currentStep% CurrentLevelData.tilesToMove.Count].types);
            m_activeParentTile.transform.position = new Vector2(0, 15);
            InputHandler.Instance.ActiveParentTile = m_activeParentTile;
            _currentStep++;
        }

            public void NextLevel()
        {
            _currentLevel++;
            OnLevelChanged?.Invoke(_currentLevel);
        }

        public void CompleteLevel()
        {
            _currentStep = 0;
            OnLevelCompleted?.Invoke();
        }
    }

    [Serializable]
    public class ParentTileData
    {
        public bool isEmpty;
        public int[] types = new int[4];
    }
}
