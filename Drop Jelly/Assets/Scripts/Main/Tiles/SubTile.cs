using System;
using UnityEngine;

namespace ww.DropJelly
{
    internal class SubTile : MonoBehaviour, ITile
    {
        [SerializeField]
        Color[] typeColors = new Color[4];


        private int _type;
        public int Type => _type;

        private int _column;
        public int Column => _column;
        private int _row;
        public int Row => _row;

        private bool _bIsMatched;
        public bool IsMatched { get => _bIsMatched; set => _bIsMatched = value; }

        private ParentTile parentTile;
        public ParentTile ParentTile { get => parentTile; set => parentTile = value; }

        public void SetGridParams(int column, int row, int type)
        {
            _row = row;
            _column = column;
            _type = type;

            TileHandler.Instance.AddSubtileToBoard(this, _column, _row);

            Color color = typeColors[type];
            color.a = 1;
            transform.GetComponentInChildren<SpriteRenderer>().color = color;
        }

        public void SetGridParams(int newColumn, int newRow)
        {
            _column = newColumn;
            _row = newRow;
            TileHandler.Instance.AddSubtileToBoard(this, _column, _row);
        }

        private int _inParentColumn;
        public int InParentColumn { get => _inParentColumn; set => _inParentColumn = value; }

        private int _inParentRow;
        public int InParentRow { get => _inParentRow; set => _inParentRow = value; }
    }
}
