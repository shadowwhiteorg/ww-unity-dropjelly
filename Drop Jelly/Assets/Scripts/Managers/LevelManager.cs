using System;
using System.Collections.Generic;
using UnityEngine;
using ww.Utilities.Singleton;

namespace ww.DropJelly
{
    internal class LevelManager : Singleton<LevelManager>
    {
        private int _currentLevel = 0;
        public int CurrentLevel => _currentLevel%_levelDatas.Count;
        private int _currentStep;
        public int CurrentStep { get => _currentStep; set => _currentStep = value; }
        [SerializeField]
        private List<LevelData> _levelDatas = new List<LevelData>();
        private LevelData _currentLevelData;
        public LevelData CurrentLevelData { get => _levelDatas[_currentLevel]; }

        public void Init()
        {
            PlayerPrefs.GetInt("CurentLevel", _currentLevel);
        }
        public void NextLevel()
        {
            _currentLevel++;
            PlayerPrefs.SetInt("CurentLevel", _currentLevel);
        }
    }

    [Serializable]
    public class ParentTileData
    {
        public bool isEmpty;
        public int[] types = new int[4];
    }
}
