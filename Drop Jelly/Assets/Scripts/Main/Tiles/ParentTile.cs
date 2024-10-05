using System.Collections;
using UnityEngine;
using System.Collections.Generic;

namespace ww.DropJelly
{
    internal class ParentTile : MonoBehaviour, ITile
    {
        private float _parentTileWidth;
        private float _parentTileHeight;
        private float _firstColumnPosition;
        private float _firstRowPosition;

        public List<SubTile> subTiles = new List<SubTile>();

        [SerializeField]
        private SubTile _subTilePrefab;
        [SerializeField]
        private float _subTileOffset;
        [SerializeField]
        private int _numberOfSubColumns = 2;
        [SerializeField]
        private int _numberOfSubRows = 2;

        private int _column;
        public int Column => _column;
        private int _row;
        public int Row => _row;

        public void SetGridParams(int newColumn, int newRow)
        {
            TileHandler.Instance.RemoveParentTileOnBoard(_column, _row);
            _column = newColumn;
            _row = newRow;
            TileHandler.Instance.AddParentTileOnBoard(this, _column, _row);

            subTiles[0]?.SetGridParams(newColumn * 2, newRow * 2);
            subTiles[1]?.SetGridParams(newColumn * 2 + 1, newRow * 2);
            subTiles[2]?.SetGridParams(newColumn * 2, newRow * 2 + 1);
            subTiles[3]?.SetGridParams(newColumn * 2 + 1, newRow * 2 + 1);

        }

        public void SetGridParams(int column, int row, int[] types)
        {
            _column = column;
            _row = row;
            TileHandler.Instance.AddParentTileOnBoard(this, _column, _row);

            InitializeSubtiles();

            subTiles[0]?.SetGridParams(column * 2, row * 2, types[0]);// left down - 0
            subTiles[1]?.SetGridParams(column * 2 + 1, row * 2, types[1]);// right down - 1
            subTiles[2]?.SetGridParams(column * 2, row * 2 + 1, types[2]);// left up - 2
            subTiles[3]?.SetGridParams(column * 2 + 1, row * 2 + 1, types[3]);// right up - 3

        }

        private void InitializeSubtiles()
        {
            for (int i = 0; i < _numberOfSubColumns; i++)
            {
                for (int j = 0; j < _numberOfSubRows; j++)
                {
                    SubTile m_subtileToInit = Instantiate(_subTilePrefab);
                    m_subtileToInit.transform.SetParent(transform);
                    m_subtileToInit.transform.localPosition = SubTilePosition(new Vector2(j, i));
                    subTiles.Add(m_subtileToInit);
                    m_subtileToInit.ParentTile = this;
                    m_subtileToInit.name = "parent " + _column + _row + " SubTile" + i + j;
                }
            }
        }

        private Vector2 SubTilePosition(Vector2 loopVector)
        {
            _parentTileWidth = _numberOfSubColumns * _subTileOffset;
            _parentTileWidth = _numberOfSubRows * _subTileOffset;
            _firstColumnPosition = (_subTileOffset - _parentTileWidth) / 2;
            _firstRowPosition = (_subTileOffset - _parentTileHeight) / 2;
            return new Vector2(_firstColumnPosition, _firstRowPosition) + _subTileOffset * loopVector - new Vector2(0, _subTileOffset);
        }

        public void ControlMatchesInOrder()
        {
            if(this.isActiveAndEnabled)
                StartCoroutine(CheckMatchesWithDelay(0.2f));
        }

        private IEnumerator CheckMatchesWithDelay(float delay)
        {
            for (int i = 0; i < subTiles.Count; i++)
            {
                if (subTiles[i] != null)
                {
                    if (MatchHandler.Instance.HasMatchWithNeighbors(subTiles[i]))
                        MatchHandler.Instance.CheckMatch(subTiles[i]);
                    yield return new WaitForSeconds(delay);
                    //yield return new WaitForEndOfFrame();
                }
            }
        }

        public void RemoveSubtileFromArray(SubTile subTile)
        {

            int m_index = subTiles.IndexOf(subTile);
            if (m_index > 0)
            {
                subTiles[m_index] = null;
            }
            for (int i = 0; i < subTiles.Count; i++)
            {
                if (subTiles[i] != null)
                    return;
            }
            TileHandler.Instance.RemoveParentTileOnBoard(_column, _row);
            gameObject.SetActive(false);
        }

        public bool HasSubtile()
        {

            for (int i = 0; i < subTiles.Count; i++)
            {
                if (subTiles[i] != null)
                    return true;
            }
            return false;
        }

        public void CheckSubTileNeigborsToFill(SubTile tile)
        {
            FillTheEmptyTile(0, 2, new Vector2(0, 1), 0, 1, tile);
            FillTheEmptyTile(1, 3, new Vector2(1, 1), 1, 1, tile);
            FillTheEmptyTile(2, 0, new Vector2(0, 0), 0, 0, tile);
            FillTheEmptyTile(3, 1, new Vector2(1, 0), 1, 0, tile);

            FillTheEmptyTile(0, 1, new Vector2(1, 0), 1, 0, tile);
            FillTheEmptyTile(2, 3, new Vector2(1, 1), 1, 1, tile);
            FillTheEmptyTile(1, 0, new Vector2(0, 0), 0, 0, tile);
            FillTheEmptyTile(3, 2, new Vector2(0, 1), 0, 1, tile);
        }

        private void FillTheEmptyTile(int sourceIndex, int targetIndex, Vector2 position, int subTileColumn, int subTileRow, SubTile centerTile)
        {
            if (subTiles[sourceIndex] && subTiles[targetIndex] == null)
            {
                SubTile subTileToInit = Instantiate(_subTilePrefab);
                subTileToInit.transform.SetParent(transform);
                subTileToInit.transform.localPosition = SubTilePosition(position);
                subTiles[targetIndex] = subTileToInit;
                subTileToInit.ParentTile = this;
                subTileToInit.name = $"parent {_column}{_row} SubTile{subTileColumn}{subTileRow}";
                subTileToInit.SetGridParams(_column * 2 + subTileColumn, _row * 2 + subTileRow, subTiles[sourceIndex].Type);
                if (MatchHandler.Instance.HasMatchWithNeighbors(subTileToInit))
                {
                    MatchHandler.Instance.CheckMatch(subTileToInit);
                }
            }
        }

        public ParentTile LowestEmtyParentTile()
        {
            ParentTile m_lowestParentTile = null;
            if (_row == 0)
            {
                return null;
            }
            for (int i = 0; i <= _row; i++)
            {
                if (TileHandler.Instance.GetParentTileOnBoard(_column, _row - i) && !TileHandler.Instance.GetParentTileOnBoard(_column, _row - i).HasSubtile())
                {
                    m_lowestParentTile = TileHandler.Instance.GetParentTileOnBoard(_column, _row - i);
                }
            }
            return m_lowestParentTile;
        }

    }
}
