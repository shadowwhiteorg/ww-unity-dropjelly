using System.Collections.Generic;
using UnityEngine;
using ww.Utilities.Singleton;

namespace ww.DropJelly
{
    internal class BoardManager : Singleton<BoardManager>
    {
        [SerializeField]
        private int _numberOfRows;
        public int NumberOfRows => _numberOfRows;
        [SerializeField] 
        private int _numberOfColumns;
        public int NumberOfColumns => _numberOfColumns; 

        [SerializeField]
        private float _tileOffset;
        public float TileOffset => _tileOffset;

        [SerializeField]
        private GridTile _gridTilePrefab;
        [SerializeField]
        private ParentTile _parentTilePrefab;
        public ParentTile ParentTilePrefab => _parentTilePrefab;

        private float _boardWidth;
        private float _boardHeight;
        private float _firstColumnPosition;
        private float _firstRowPosition;


        public void InitGrid()
        {
            int m_currentTileNumber = 0;
            for (int x = 0; x < _numberOfColumns; x++)
            {
                for (int y = 0; y < _numberOfRows; y++)
                {
                    GridTile gridTile = Instantiate(_gridTilePrefab, TilePosition(new Vector2(y, x)), Quaternion.identity);
                    gridTile.transform.SetParent(transform);
                    gridTile.Initialize(y, x);
                    gridTile.gameObject.name = "tbg " + y + "-" + x;
                    gridTile.HasParentTile = false;

                    PlaceParentTiles(gridTile, x, y, m_currentTileNumber);

                    m_currentTileNumber++;
                }
            }
        }

        private void PlaceParentTiles(GridTile gridTile, int column, int row, int gridNr)
        {
            if (LevelManager.Instance.CurrentLevelData.levelTiles.Count > gridNr && !LevelManager.Instance.CurrentLevelData.levelTiles[gridNr].isEmpty)
            {
                gridTile.HasParentTile = true;
                ParentTile parentTile = Instantiate(_parentTilePrefab, TilePosition(new Vector2(row, column)), Quaternion.identity);
                parentTile.SetGridParams(row, column, LevelManager.Instance.CurrentLevelData.levelTiles[gridNr].types);
            }
        }
        private Vector2 TilePosition(Vector2 loopVector)
        {
            return FirstTilePosition() + loopVector*_tileOffset;
        }
        private Vector2 FirstTilePosition()
        {
            _boardWidth = _numberOfColumns * _tileOffset;
            _boardHeight = _numberOfRows * _tileOffset;
            _firstColumnPosition = (_tileOffset - _boardWidth) / 2;
            _firstRowPosition = (_tileOffset - _boardHeight) / 2;
            return new Vector2(_firstColumnPosition, _firstRowPosition);
        }
        public Vector2 BoardBorders()
        {
            return new Vector2(FirstTilePosition().x, FirstTilePosition().x + (_numberOfColumns-1)*_tileOffset);
        }

    }
}


