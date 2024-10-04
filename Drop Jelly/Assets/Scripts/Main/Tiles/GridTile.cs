using System.Collections.Generic;
using UnityEngine;

namespace ww.DropJelly
{
    internal class GridTile : MonoBehaviour, ITile
    {
        [SerializeField]
        Color idleColor = Color.grey;
        [SerializeField]
        Color activeColor = Color.white;

        private List<GridTile> columnNeighbors = new List<GridTile>();

        private bool onInputRange;
        public bool OnInputRange { get => onInputRange; set { onInputRange = value; } }

        private bool _hasParentTile;
        public bool HasParentTile { get => _hasParentTile; set => _hasParentTile = value; }

        private int _column;
        public int Column => _column;
        private int _row;
        public int Row => _row;

        private void Start()
        {
            SetColor(false);
        }

        private void Update()
        {
            CheckActiveStatus();
        }

        public void Initialize(int column, int row)
        {
            _column = column;
            _row = row;
            TileHandler.Instance.AddTileToDictionary(this, _row, _column);
        }

        public void SetColor(bool enable)
        {
            GetComponentInChildren<SpriteRenderer>().color = enable ? activeColor : idleColor;
        }

        public void EnableDisableRowNeighbors(bool enabled)
        {
            if (enabled)
            {
                for (int i = 0; i < columnNeighbors.Count; i++)
                {
                    columnNeighbors[i].SetColor(enabled);
                }
            }
        }

        public void AddToColumnNeighbors(GridTile tileBackground)
        {
            columnNeighbors.Add(tileBackground);
        }

        public void CheckActiveStatus()
        {
            if (Mathf.Abs(this.transform.position.x - InputHandler.Instance.currentMousePosition.x) < BoardManager.Instance.TileOffset / 2)
            {
                SetColor(true);
                onInputRange = true;
            }
            else
            {
                SetColor(false);
                onInputRange = false;
            }
        }


    }
}
