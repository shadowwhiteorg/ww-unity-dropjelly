using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;

namespace ww.DropJelly
{
    internal class ParentTile : MonoBehaviour, ITile
    {
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
        //[SerializeField]
        public List<SubTile> _subTiles = new List<SubTile>();
        //public List<SubTile> SubTiles => _subTiles;


        public void SetGridParams(int column, int row, int[] types)
        {
            _column = column;
            _row = row;
            TileHandler.Instance.AddParentTileOnBoard(this, _column, _row);


            for (int i = 0; i < _numberOfSubColumns; i++)
            {
                for (int j = 0; j < _numberOfSubRows; j++)
                {
                    //int m_index = 0;
                    SubTile m_subtileToInit = Instantiate(_subTilePrefab);
                    m_subtileToInit.transform.SetParent(transform);
                    m_subtileToInit.transform.localPosition = SubTilePosition(new Vector2(j, i));
                    _subTiles.Add(m_subtileToInit);
                    //_subTiles[i+j] = m_subtileToInit;
                    m_subtileToInit.ParentTile = this;
                    m_subtileToInit.name = "parent " + column + row + " SubTile" + i + j;
                    //m_index++;
                }
            }

            _subTiles[0]?.SetGridParams(column * 2, row * 2, types[0]);// left down
            _subTiles[1]?.SetGridParams(column * 2 + 1, row * 2, types[1]);// right down
            _subTiles[2]?.SetGridParams(column * 2, row * 2 + 1, types[2]);// left up
            _subTiles[3]?.SetGridParams(column * 2 + 1, row * 2 + 1, types[3]);// right up
        }

        public void SetGridParams(int newColumn, int newRow)
        {
            _column = newColumn;
            _row = newRow;
            TileHandler.Instance.AddParentTileOnBoard(this, _column, _row);

            _subTiles[0]?.SetGridParams(newColumn * 2, newRow * 2);
            _subTiles[1]?.SetGridParams(newColumn * 2 + 1, newRow * 2);
            _subTiles[2]?.SetGridParams(newColumn * 2, newRow * 2 + 1);
            _subTiles[3]?.SetGridParams(newColumn * 2 + 1, newRow * 2 + 1);

        }

        //private void SetupSubtiles()


        private float _parentTileWidth;
        private float _parentTileHeight;
        private float _firstColumnPosition;
        private float _firstRowPosition;
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
            //if(HasMatchesInNeighbour())
                StartCoroutine(CheckMatchesWithDelay(.1f));
        }

        private bool HasMatchesInNeighbour()
        {
            bool m_HasMatch = false;
            for (int i = 0; i < _subTiles.Count; i++)
            {
                if (_subTiles[i] != null)
                {
                    //if (MatchHandler.Instance.HasMatchWithNeighborParentTiles(_subTiles[i]))
                    //{
                    //    m_HasMatch = true;
                    //    break;
                    //}
                }
            }

            return m_HasMatch;
        }

        private IEnumerator CheckMatchesWithDelay(float delay)
        {
            for (int i = 0; i < _subTiles.Count; i++)
            {
                if (_subTiles[i] != null)
                {
                    if (MatchHandler.Instance.HasMatchWithNeighbors(_subTiles[i]))
                        MatchHandler.Instance.CheckMatch(_subTiles[i]);
                    yield return new WaitForSeconds(delay);
                }
            }
        }

        public void RemoveSubtileFromArray(SubTile subTile)
        {

            int m_index = _subTiles.IndexOf(subTile);
            if (m_index > 0)
            {
                _subTiles[m_index] = null;
            }
            for (int i = 0; i < _subTiles.Count; i++)
            {
                if (_subTiles[i] != null)
                    return;
            }
            TileHandler.Instance.RemoveParentTileOnBoard(_column, _row);
            Destroy(gameObject);
            Debug.Log("test " + this.name);
        }

        public bool HasSubtile()
        {

            for (int i = 0; i < _subTiles.Count; i++)
            {
                if (_subTiles[i] != null)
                    return true;
            }
            return false;
        }

        public ParentTile UpperTile()
        {
            return TileHandler.Instance.GetParentTileOnBoard(_column, _row + 1);
        }



        public void CheckSubTileNeigbors(SubTile tile)
        {
            FillTheEmptyTile(0, 2, new Vector2(0, 1), 0, 1, tile);
            FillTheEmptyTile(1, 3, new Vector2(1, 1), 1, 1, tile);
            FillTheEmptyTile(2, 0, new Vector2(0, 0), 0, 0, tile);
            FillTheEmptyTile(3, 1, new Vector2(1, 0), 1, 0, tile);
                                                          
            FillTheEmptyTile(0, 1, new Vector2(1, 0), 0, 0, tile);
            FillTheEmptyTile(2, 3, new Vector2(1, 1), 1, 1, tile);
            FillTheEmptyTile(1, 0, new Vector2(0, 0), 0, 0, tile);
            FillTheEmptyTile(3, 2, new Vector2(0, 1), 0, 1, tile);
        }

        private void FillTheEmptyTile(int sourceIndex, int targetIndex, Vector2 position, int subTileColumn, int subTileRow, SubTile centerTile)
        {
            if (_subTiles[sourceIndex] && _subTiles[targetIndex] == null)
            {
                SubTile subTileToInit = Instantiate(_subTilePrefab);
                subTileToInit.transform.SetParent(transform);
                subTileToInit.transform.localPosition = SubTilePosition(position);
                _subTiles[targetIndex] = subTileToInit;
                subTileToInit.ParentTile = this;
                subTileToInit.name = $"parent {_column}{_row} SubTile{subTileColumn}{subTileRow}";
                subTileToInit.SetGridParams(_column * 2 + subTileColumn, _row * 2 + subTileRow, _subTiles[sourceIndex].Type);
                if (MatchHandler.Instance.HasMatchWithNeighbors(subTileToInit) /*|| MatchManager.Instance.HasMatchWithNeighborParentTiles(centerTile)*/)
                    MatchHandler.Instance.CheckMatch(subTileToInit);
            }
            //StartCoroutine(FillTheTilesCoroutine(sourceIndex, targetIndex, position, subTileColumn, subTileRow, centerTile));
        }

        private IEnumerator FillTheTilesCoroutine(int sourceIndex, int targetIndex, Vector2 position, int subTileColumn, int subTileRow, SubTile centerTile)
        {
            yield return null;
                //centerTile.ParentTile.ControlMatchesInOrder();
        }


        public ParentTile LowestEmtyParentTile()
        {
            ParentTile m_lowestParentTile = null;
            if(_row == 0)
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


        private List<ParentTile> ClosestParentTileNeighborst()
        {
            List<ParentTile> m_closestParentTiles = new List<ParentTile>();

            if (_column < BoardManager.Instance.NumberOfColumns - 1)
                m_closestParentTiles.Add(TileHandler.Instance.GetParentTileOnBoard(_column + 1, _row)); // Right
            if (_column > 0)
                m_closestParentTiles.Add(TileHandler.Instance.GetParentTileOnBoard(_column - 1, _row)); // Left
            if (_row < BoardManager.Instance.NumberOfRows - 1)
                m_closestParentTiles.Add(TileHandler.Instance.GetParentTileOnBoard(_column, _row + 1)); // Up
            if (_row > 0)
                m_closestParentTiles.Add(TileHandler.Instance.GetParentTileOnBoard(_column, _row - 1)); // Down

            return m_closestParentTiles;
        }

    }
}
