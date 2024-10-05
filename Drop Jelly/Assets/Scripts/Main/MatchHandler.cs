using System.Collections.Generic;
using ww.Utilities.Singleton;
using UnityEngine;

namespace ww.DropJelly
{
    internal class MatchHandler : Singleton<MatchHandler>
    {

        public void CheckMatch(SubTile centerTile)
        {
            CheckMatchesInNeighbors(centerTile);
        }

        private void CheckMatchesInNeighbors(SubTile centerTile)
        {
            CheckMatchInDirection(centerTile, 1, 0); // Check right tile
            CheckMatchInDirection(centerTile, -1, 0); // Check left tile
            CheckMatchInDirection(centerTile, 0, 1); // Check up tile
            CheckMatchInDirection(centerTile, 0, -1);// Check down tile
        }

        private void CheckMatchInDirection(SubTile centerTile, int xOffset, int yOffset)
        {
            SubTile tileToCheck = GetTile(centerTile.Column + xOffset, centerTile.Row + yOffset);
            if (tileToCheck != null && tileToCheck.Type == centerTile.Type && !tileToCheck.IsMatched)
            {
                TileHandler.Instance.CheckedMatchOperation(centerTile);
                TileHandler.Instance.CheckedMatchOperation(tileToCheck);
                tileToCheck.IsMatched = true;
                centerTile.IsMatched = true;
                //GameManager.Instance.CurrentTarget--;
                CheckMatch(tileToCheck);
            }
        }

        public bool HasMatchWithNeighbors(SubTile centerTile)
        {
            bool m_hasMatchInParent = false;
            List<bool> m_checkedSubtiles = new List<bool>();
            m_checkedSubtiles.Add(CheckMatchesInParent(centerTile, 1, 0)); // Check right tile
            m_checkedSubtiles.Add(CheckMatchesInParent(centerTile, -1, 0)); // Check left tile
            m_checkedSubtiles.Add(CheckMatchesInParent(centerTile, 0, 1)); // Check up tile
            m_checkedSubtiles.Add(CheckMatchesInParent(centerTile, 0, -1));// Check down tile

            for (int i = 0; i < m_checkedSubtiles.Count; i++)
            {
                if (m_checkedSubtiles[i])
                {
                    m_hasMatchInParent = true;
                    return m_hasMatchInParent;
                }
            }
            return m_hasMatchInParent;
        }

        private bool CheckMatchesInParent(SubTile centerTile, int xOffset, int yOffset)
        {
            SubTile tileToCheck = GetTile(centerTile.Column + xOffset, centerTile.Row + yOffset);
            bool m_hasParentMatch = false;
            if (tileToCheck != null && tileToCheck.Type == centerTile.Type && !tileToCheck.IsMatched)
            {
                if (tileToCheck.ParentTile != centerTile.ParentTile)
                {
                    m_hasParentMatch = true;
                }
            }
            return m_hasParentMatch;
        }

        private SubTile GetTile(int column, int row)
        {
            if (column >= 0 && column < BoardManager.Instance.NumberOfColumns * 2 && row >= 0 && row < BoardManager.Instance.NumberOfRows * 2)
            {
                return (SubTile)TileHandler.Instance.SubtilesOnBoard.GetValue(column, row);
            }
            return null;
        }

    }
}
