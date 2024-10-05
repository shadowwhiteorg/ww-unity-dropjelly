using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using ww.Utilities.Singleton;

namespace ww.DropJelly
{
    internal class TileHandler : Singleton<TileHandler>
    {
        #region Grid Tile Related
        private Dictionary<GridTile, (int row, int column)> tileBackgroundDictionary =
           new Dictionary<GridTile, (int row, int column)>();

        public void AddTileToDictionary(GridTile tile, int row, int column)
        {
            tileBackgroundDictionary[tile] = (row, column);
        }
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
        #endregion
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

        public GridTile TargetTile()
        {

            for (int i = 0; i < BoardManager.Instance.NumberOfRows; i++)
            {
                var m_targetTileBackground = GetTilesOnColumn(ActiveTileColumn().Column)[i];
                if (!GetTilesOnColumn(ActiveTileColumn().Column)[i].HasParentTile)
                {
                    m_targetTileBackground = GetTilesOnColumn(ActiveTileColumn().Column)[i];
                    return m_targetTileBackground;
                }
            }
            return null;

        }

        private ParentTile[,] _parentTilesOnBoard;
        public ParentTile[,] ParentTilesOnBoard => _parentTilesOnBoard;
        public void InitializeParentTilesOnBoard()
        {
            _parentTilesOnBoard = new ParentTile[BoardManager.Instance.NumberOfColumns, BoardManager.Instance.NumberOfRows];
        }

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

        private SubTile[,] _subtilesOnBoard;
        public SubTile[,] SubtilesOnBoard => _subtilesOnBoard;

        public void InitializeSubtilesOnBoard()
        {
            _subtilesOnBoard = new SubTile[BoardManager.Instance.NumberOfColumns * 2, BoardManager.Instance.NumberOfRows * 2];
        }

        public void AddSubtileToBoard(SubTile subtile, int column, int row)
        {

            _subtilesOnBoard[column, row] = subtile;
        }

        public SubTile GetSubtileFromBoard(int column, int row)
        {
            return _subtilesOnBoard[column, row];
        }

        public void CheckedMatchOperation(SubTile matchedTile)
        {
            StartCoroutine(MatchedTileSequence(matchedTile));
        }

        private IEnumerator MatchedTileSequence(SubTile tile)
        {
            float duration = .5f;
            float elapsedTime = 0f;
            Vector3 startScale = tile.transform.localScale;
            Vector3 targetScale = new Vector3(2, 2, 2);
            while (elapsedTime < duration)
            {
                tile.transform.localScale = Vector3.Lerp(startScale, targetScale, (elapsedTime / duration));
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            tile.transform.localScale = targetScale;
            yield return new WaitForEndOfFrame();
            tile.ParentTile.RemoveSubtileFromArray(tile);
            Destroy(tile.gameObject);
            yield return new WaitForSeconds(0.5f);
            tile.ParentTile.CheckSubTileNeigbors(tile);
            yield return new WaitForSeconds(0.5f);
            ParentTile m_targetParentTile = null;
            m_targetParentTile = tile.ParentTile.LowestEmtyParentTile();
            if (m_targetParentTile != null)
            {
                InputHandler.Instance.SendParentTileToTarget(tile.ParentTile, m_targetParentTile.transform.position, false);
                tile.ParentTile.SetGridParams(m_targetParentTile.Column, m_targetParentTile.Row);
            }
        }

        private void OnBeforeTransformParentChanged()
        {
            foreach (var entry in tileBackgroundDictionary)
            {
                var tile = entry.Key;
                tileBackgroundDictionary[tile] = (tile.Row, tile.Column);
            }
        }

        public void CheckBackgroundTileHasParentStatus()
        {
            foreach (var entry in tileBackgroundDictionary)
            {
                var tile = entry.Key;
                var (row, column) = entry.Value;
                if (GetParentTileOnBoard(column, row) != null)
                {
                    tile.HasParentTile = true;
                }
                else
                {
                    tile.HasParentTile = false;
                }
            }
        }
    }
}
