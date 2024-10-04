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

        private bool _isActive = false;
        public  bool IsActive { get { return _isActive; } }

        

        private void Update()
        {
            //if(_isActive)
                GetInput();
        }

        private void GetInput()
        {
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
            else
                SendParentTileToTarget(_activeParentTile, TileHandler.Instance.TargetTile().transform.position,true);
        }

        public void SendParentTileToTarget(ParentTile parentTile, Vector2 targetPosition, bool fromInput)
        {
            Vector2 m_targetPosition = targetPosition;
            if(fromInput)
                StartCoroutine(SetTileToTargetPositionFromInput(parentTile, m_targetPosition));
            else
                StartCoroutine(SetTileToTargetPosition(parentTile, m_targetPosition));
        }

        private IEnumerator SetTileToTargetPositionFromInput(ParentTile parentTile, Vector2 targetPosition)
        {
            
            parentTile.transform.position = new Vector2(targetPosition.x, parentTile.transform.position.y);
            while (Vector2.Distance(parentTile.transform.position, targetPosition) > 0.1f)
            {
                parentTile.transform.position = Vector2.Lerp(parentTile.transform.position, targetPosition, 0.5f);
                yield return null;
            }
            parentTile.transform.position = targetPosition;
            parentTile.SetGridParams(TileHandler.Instance.TargetTile().Column, TileHandler.Instance.TargetTile().Row);
            yield return new WaitForEndOfFrame();
            parentTile.ControlMatchesInOrder();
            yield return new WaitForSeconds(11);
            TileHandler.Instance.CheckAndRemoveEmptyParentTiles();
            TileHandler.Instance.CheckBackgroundTileHasParentStatus();
            yield return new WaitForEndOfFrame();
            LevelManager.Instance.InitActiveParentTile();

        }

        private IEnumerator SetTileToTargetPosition(ParentTile parentTile, Vector2 targetPosition)
        {

            parentTile.transform.position = new Vector2(targetPosition.x, parentTile.transform.position.y);
            while (Vector2.Distance(parentTile.transform.position, targetPosition) > 0.1f)
            {
                parentTile.transform.position = Vector2.Lerp(parentTile.transform.position, targetPosition, 0.5f);
                yield return null;
            }
            parentTile.transform.position = targetPosition;
            yield return new WaitForEndOfFrame();
            parentTile.ControlMatchesInOrder();
            //parentTile.CheckMatchesFromNeighborParentTiles();
        }

    }
}
