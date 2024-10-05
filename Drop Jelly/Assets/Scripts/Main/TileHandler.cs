using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ww.Utilities.Singleton;

namespace ww.DropJelly
{
    internal class TileHandler : Singleton<TileHandler>
    {
        #region Grid Tile Dict Operations
        private Dictionary<GridTile, (int row, int column)> tileBackgroundDictionary =
           new Dictionary<GridTile, (int row, int column)>();

        public Dictionary<int, List<GridTile>> GetTilesGroupedByColumn()
        {
            Dictionary<int, List<GridTile>> columnToTiles = new Dictionary<int, List<GridTile>>();

            foreach (var entry in tileBackgroundDictionary)
            {
                int column = entry.Value.column;
                GridTile tile = entry.Key;

                if (!columnToTiles.ContainsKey(column))
                {
                    columnToTiles[column] = new List<GridTile>();
                }

                columnToTiles[column].Add(tile);
            }

            return columnToTiles;
        }
        public List<GridTile> GetTilesOnColumn(int column)
        {
            var tilesGroupedByRow = GetTilesGroupedByColumn();
            if (tilesGroupedByRow.ContainsKey(column))
            {
                return tilesGroupedByRow[column];
            }
            return new List<GridTile>();
        } 


        public Vector2 ActiveColumnPosition()
        {
            for (int i = 0; i < BoardManager.Instance.NumberOfColumns; i++)
            {
                var tilesOnColumn = GetTilesOnColumn(i);
                if (tilesOnColumn.Count > 0)
                {
                    if (tilesOnColumn[0].OnInputRange)
                        return tilesOnColumn[0].transform.position;
                }
            }
            return Vector2.zero;
        }

        public GridTile ActiveTileColumn()
        {
            for (int i = 0; i < BoardManager.Instance.NumberOfColumns; i++)
            {
                var tilesOnColumn = GetTilesOnColumn(i);
                if (tilesOnColumn.Count > 0)
                {
                    if (tilesOnColumn[0].OnInputRange)
                        return tilesOnColumn[0];
                }
            }
            return null;
        }
        public void AddTileToDictionary(GridTile tile, int row, int column)
        {
            tileBackgroundDictionary[tile] = (row, column);
        }
        public GridTile TargetTile()
        {

            for (int i = 0; i < BoardManager.Instance.NumberOfRows; i++)
            {
                var m_targetGridTile = GetTilesOnColumn(ActiveTileColumn().Column)[i];
                if (!GetTilesOnColumn(ActiveTileColumn().Column)[i].HasParentTile)
                {
                    m_targetGridTile = GetTilesOnColumn(ActiveTileColumn().Column)[i];
                    return m_targetGridTile;
                }
            }
            return null;

        }
        #endregion

        #region Parent Tile Dict Operations
        private ParentTile[,] _parentTilesOnBoard;
        public ParentTile[,] ParentTilesOnBoard => _parentTilesOnBoard;

        public void AddParentTileOnBoard(ParentTile parentTile, int column, int row)
        {
            _parentTilesOnBoard[column, row] = parentTile;
        }

        public void RemoveParentTileOnBoard(int column, int row)
        {
            _parentTilesOnBoard[column, row] = null;
        }

        public void CheckAndRemoveEmptyParentTiles()
        {
            for (int i = 0; i < BoardManager.Instance.NumberOfColumns; i++)
            {
                for (int j = 0; j < BoardManager.Instance.NumberOfRows; j++)
                {
                    if (_parentTilesOnBoard[i, j] != null)
                    {
                        if (_parentTilesOnBoard[i, j].HasSubtile() == false)
                        {
                            _parentTilesOnBoard[i, j] = null;
                        }
                    }
                }
            }
        }

        public ParentTile GetParentTileOnBoard(int column, int row)
        {
            return _parentTilesOnBoard[column, row];
        }
        #endregion

        #region Sub Tile Dict Operations
        private SubTile[,] _subtilesOnBoard;
        public SubTile[,] SubtilesOnBoard => _subtilesOnBoard;
        public void AddSubtileToBoard(SubTile subtile, int column, int row)
        {

            _subtilesOnBoard[column, row] = subtile;
        }

        public SubTile GetSubtileFromBoard(int column, int row)
        {
            return _subtilesOnBoard[column, row];
        } 
        #endregion

        public void Init()
        {
            InitializeParentTilesOnBoard();
            InitializeSubtilesOnBoard();
            InitializeActiveParentTile();
        }
        public void InitializeParentTilesOnBoard()
        {
            _parentTilesOnBoard = new ParentTile[BoardManager.Instance.NumberOfColumns, BoardManager.Instance.NumberOfRows];
        }
        public void InitializeSubtilesOnBoard()
        {
            _subtilesOnBoard = new SubTile[BoardManager.Instance.NumberOfColumns * 2, BoardManager.Instance.NumberOfRows * 2];
        }
        public void InitializeActiveParentTile()
        {
            if (GameManager.Instance.IsGameActive)
                GameManager.Instance.CheckLevelEndCondition();
            int m_currentStep = LevelManager.Instance.CurrentStep % LevelManager.Instance.CurrentLevelData.tilesToMove.Count;
            ParentTile m_activeParentTile = Instantiate(BoardManager.Instance.ParentTilePrefab);
            m_activeParentTile.SetGridParams(4, 4, LevelManager.Instance.CurrentLevelData.tilesToMove[m_currentStep].types);
            m_activeParentTile.transform.position = new Vector2(0, 15);
            InputHandler.Instance.ActiveParentTile = m_activeParentTile;
            LevelManager.Instance.CurrentStep++;
            GameManager.Instance.CurrentMove--;
        }


        public void CheckedMatchedSubTiles(SubTile matchedTile)
        {
            StartCoroutine(MatchedTileSequence(matchedTile));
        }

        private IEnumerator MatchedTileSequence(SubTile tile)
        {
            #region Visual Feedback
            float duration = .35f;
            float elapsedTime = 0f;
            Vector3 startScale = tile.transform.localScale;
            Vector3 targetScale = Vector3.one * 2f;
            while (elapsedTime < duration)
            {
                tile.transform.localScale = Vector3.Lerp(startScale, targetScale, (elapsedTime / duration));
                elapsedTime += Time.deltaTime;
                yield return null;
            } 
            tile.transform.localScale = targetScale;
            #endregion

            //yield return new WaitForSeconds(0.5f);
            tile.ParentTile.RemoveSubtileFromArray(tile);
            Destroy(tile.gameObject);
            yield return new WaitForSeconds(0.5f);
            tile.ParentTile.CheckSubTileNeigborsToFill(tile);
            yield return new WaitForSeconds(0.1f);
            ParentTile m_targetParentTile = null;
            m_targetParentTile = tile.ParentTile.LowestEmtyParentTile();
            if (m_targetParentTile != null)
            {
                SendParentTileToTarget(tile.ParentTile, m_targetParentTile.transform.position, false);
                tile.ParentTile.SetGridParams(m_targetParentTile.Column, m_targetParentTile.Row);
            }
            yield return new WaitForSeconds(0.1f);
            CheckIfParentTileHasNoTileUnder();
        }
        private void CheckIfParentTileHasNoTileUnder()
        {
            ParentTile m_sourceParentTile;
            for (int i = 0; i < BoardManager.Instance.NumberOfColumns; i++)
            {
                for (int j = 0; j < BoardManager.Instance.NumberOfRows; j++)
                {
                    if (_parentTilesOnBoard[i, j] != null)
                    {
                        if(_parentTilesOnBoard[i, j].LowestEmtyParentTile() != null)
                        {
                            m_sourceParentTile = _parentTilesOnBoard[i, j];
                            SendParentTileToTarget(m_sourceParentTile, m_sourceParentTile.LowestEmtyParentTile().transform.position, false);
                        }
                    }
                }
            }
        }

        public void CheckBackgroundTileHasParent()
        {
            foreach (var entry in tileBackgroundDictionary)
            {
                var tile = entry.Key;
                var (row, column) = entry.Value;
                if (GetParentTileOnBoard(column, row) != null)
                    tile.HasParentTile = true;
                else
                    tile.HasParentTile = false;
            }
        }

        public void SendParentTileToTarget(ParentTile parentTile, Vector2 targetPosition, bool fromInput)
        {
            Vector2 m_targetPosition = targetPosition;
            if (fromInput)
                StartCoroutine(SetTilePosition(parentTile, m_targetPosition,true));
            else
                StartCoroutine(SetTilePosition(parentTile, m_targetPosition, false));
        }

        private IEnumerator SetTilePosition(ParentTile parentTile, Vector2 targetPosition, bool fromInput)
        {
            parentTile.transform.position = new Vector2(targetPosition.x, parentTile.transform.position.y);
            while (Vector2.Distance(parentTile.transform.position, targetPosition) > 0.1f)
            {
                parentTile.transform.position = Vector2.Lerp(parentTile.transform.position, targetPosition, 0.5f);
                yield return null;
            }
            parentTile.transform.position = targetPosition;

            StartCoroutine(TileMatchControl(parentTile, fromInput));
        }

        private IEnumerator TileMatchControl(ParentTile parentTile, bool fromInput)
        {
            if (!fromInput)
            {
                yield return new WaitForEndOfFrame();
                if (parentTile)
                {   
                    if(parentTile.LowestEmtyParentTile())
                        parentTile.SetGridParams(parentTile.LowestEmtyParentTile().Column, parentTile.LowestEmtyParentTile().Row);
                    parentTile.ControlMatchesInOrder();
                }
                yield break;
            }
            yield return new WaitForEndOfFrame();
            parentTile.SetGridParams(TargetTile().Column, TargetTile().Row);
            
            parentTile.ControlMatchesInOrder();
            yield return new WaitForSeconds(3);
            InputHandler.Instance.IsActive = true;
            CheckAndRemoveEmptyParentTiles();
            CheckBackgroundTileHasParent();
            yield return new WaitForEndOfFrame();
            InitializeActiveParentTile();
        }
        public void DisableAllGridTiles()
        {
            foreach (var entry in tileBackgroundDictionary)
            {
                entry.Key.SetColor(false);
            }
        }
    }
}
