﻿using UnityEngine;

namespace QuickMafs
{
    public delegate void TileViewEventHandler();

    public class TileView: MonoBehaviour
    {
        public SpriteRenderer Background;
        public SpriteRenderer Foreground;
        public SpriteRenderer Text;

        public event TileViewEventHandler Selected;
        public event TileViewEventHandler Unselected;

        public void SelectTile()
        {
            IssueEvent(Selected);
        }

        public void DeselectTile()
        {
            IssueEvent(Unselected);
        }

        private void IssueEvent(TileViewEventHandler eventToIssue)
        {
            if (eventToIssue != null)
            {
                eventToIssue();
            }
        }
    }
}
