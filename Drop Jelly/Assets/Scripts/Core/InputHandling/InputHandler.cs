using System.Collections;
using UnityEngine;
using ww.Utilities.Singleton;
using System.Collections.Generic;

namespace ww.DropJelly
{
    internal class InputHandler : Singleton<InputHandler>
    {
        [SerializeField]
        private float minInputDistance;
        private ParentTile _activeParentTile;
        public ParentTile ActiveParentTile { get => _activeParentTile; set => _activeParentTile = value; }

        private Vector2 _initialMousePosition;
        private Vector2 currentMousePosition;
        public Vector2 CurrentMousePosition => currentMousePosition;

        private bool _isActive = true;
        public  bool IsActive { get => _isActive; set => _isActive = value; }


        private void Update()
        {
            if(_isActive && GameManager.Instance.IsGameActive)
                GetInput();
        }

        private void OnEnable()
        {
            EventManager.OnLevelFailed += DeactvateInput;
        }

        private void OnDisable()
        {
            EventManager.OnLevelFailed -= DeactvateInput;
        }


        private void GetInput()
        {
            if(Input.GetKeyDown(KeyCode.Space))
            {
                SceneHandler.Instance.LoadNextLevel();
            }

            if (Input.GetMouseButtonDown(0))
            {
                _initialMousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            }
            if (Input.GetMouseButton(0))
            {
                currentMousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

                if ( Mathf.Abs(currentMousePosition.x - _initialMousePosition.x) > minInputDistance)
                {
                    _isActive = true;
                    SetActiveParentTilePosition();
                }
            }
            if (Input.GetMouseButtonUp(0))
            {
                _isActive = false;
                TileHandler.Instance.SendParentTileToTarget(_activeParentTile, TileHandler.Instance.TargetTile().transform.position, true);
                TileHandler.Instance.DisableAllGridTiles();
            }
        }

        private void SetActiveParentTilePosition()
        {
            _activeParentTile.transform.position = 
                new Vector2(Mathf.Clamp(currentMousePosition.x, BoardManager.Instance.BoardBorders().x,BoardManager.Instance.BoardBorders().y),
                            _activeParentTile.transform.position.y);
        }

        private void DeactvateInput()
        {
            _isActive = false;
        }

    }
}
