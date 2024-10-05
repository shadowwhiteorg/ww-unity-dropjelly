using ww.Utilities.Singleton;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace ww.DropJelly
{
    internal class UIManager : Singleton<UIManager>
    {
        [SerializeField] 
        private GameObject _inGameCanvas;
        [SerializeField]
        private GameObject _endGameCanvas;

        private TextMeshProUGUI _targetText;
        private TextMeshProUGUI _moveText;
        private GameObject _levelCompletedElement;
        private GameObject _levelFailedElement;
        private Button _nextLevelButton;
        private Button _restartButton;

        private void Start()
        {
            //Init();
        }
        private void OnEnable()
        {
            EventManager.OnLevelFailed += LevelFailedUI;
            EventManager.OnLevelCompleted += LevelCompletedUI;
        }

        private void OnDisable()
        {
            EventManager.OnLevelFailed -= LevelFailedUI;
            EventManager.OnLevelCompleted -= LevelCompletedUI;
        }

        public void Init()
        {
            _targetText = _inGameCanvas.transform.GetChild(1).GetChild(1).GetComponent<TextMeshProUGUI>();
            _moveText = _inGameCanvas.transform.GetChild(0).GetChild(1).GetComponent<TextMeshProUGUI>();
            _levelCompletedElement = _endGameCanvas.transform.transform.GetChild(0).GetChild(0).gameObject;
            _levelFailedElement = _endGameCanvas.transform.transform.GetChild(0).GetChild(1).gameObject;
            _nextLevelButton = _levelCompletedElement.transform.GetChild(1).GetComponent<Button>();
            _restartButton = _levelFailedElement.transform.GetChild(1).GetComponent<Button>();

            _nextLevelButton.onClick.AddListener(() =>
            {
                LevelManager.Instance.NextLevel();
                SceneHandler.Instance.LoadNextLevel();
            });
            _restartButton.onClick.AddListener(() =>
            {
                SceneHandler.Instance.ReloadCurrentLevel();
            });
        }

        private void LevelFailedUI()
        {
            _inGameCanvas.SetActive(false);
            _levelFailedElement.SetActive(true);
            _endGameCanvas.SetActive(true);
        }

        private void LevelCompletedUI()
        {
            _inGameCanvas.SetActive(false);
            _levelCompletedElement.SetActive(true);
            _endGameCanvas.SetActive(true);
        }

        public void UpdateTargetText(int target)
        {
            _targetText.text = target.ToString();
        }
        public void UpdateMoveText(int move)
        {
            _moveText.text = move.ToString();
        }



    }
}