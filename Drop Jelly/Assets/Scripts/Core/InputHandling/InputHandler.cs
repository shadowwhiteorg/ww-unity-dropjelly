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

        private Vector2 initialMousePosition;
        public Vector2 currentMousePosition;

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
                initialMousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            }
            if (Input.GetMouseButton(0))
            {
                
                currentMousePosition = new Vector2( Mathf.Clamp(Camera.main.ScreenToWorldPoint(Input.mousePosition).x, BoardManager.Instance.BoardBorders().x, BoardManager.Instance.BoardBorders().y),0);

                if( Mathf.Abs(currentMousePosition.x - initialMousePosition.x) > minInputDistance)
                {
                    _isActive = true;
                    SetActiveParentTilePosition(true);
                }
            }
            if (Input.GetMouseButtonUp(0))
            {
                _isActive = false;
                SetActiveParentTilePosition(false);
            }
        }

        private void SetActiveParentTilePosition(bool mouseDown)
        {
            if (mouseDown) 
            _activeParentTile.transform.position = 
                new Vector2(Mathf.Clamp(currentMousePosition.x, BoardManager.Instance.BoardBorders().x,BoardManager.Instance.BoardBorders().y),
                            _activeParentTile.transform.position.y);
            else if (_activeParentTile)
                TileHandler.Instance.SendParentTileToTarget(_activeParentTile, TileHandler.Instance.TargetTile().transform.position,true);
        }

        private void DeactvateInput()
        {
            _isActive = false;
        }

    }
}
