using System;
using System.Collections.Generic;
using UnityEngine;
using ww.Utilities.Singleton;

namespace ww.DropJelly
{
    internal class LevelManager : Singleton<LevelManager>
    {
        [SerializeField]
        private List<LevelData> _levelDatas = new List<LevelData>();

        private int _currentLevel = 1;
        public int CurrentLevel => _currentLevel%_levelDatas.Count;

        private int _currentStep;
        public int CurrentStep { get => _currentStep; set => _currentStep = value; }


        private LevelData _currentLevelData;
        public LevelData CurrentLevelData { get => _levelDatas[CurrentLevel]; }
        public void Init()
        {
            _currentLevel = PlayerPrefs.GetInt("CurrentLevel");
        }
        public void NextLevel()
        {
            _currentLevel++;
            PlayerPrefs.SetInt("CurrentLevel", _currentLevel);
        }
    }

    [Serializable]
    public class ParentTileData
    {
        public bool isEmpty;
        public int[] types = new int[4];
    }
}
