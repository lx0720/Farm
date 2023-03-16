using System;
using UnityEngine;

namespace Farm.AStar
{
    /// <summary>
    /// ½ÚµãÀà
    /// </summary>
    public class Node : IComparable<Node>
    {
        #region Fields
        private Vector2Int nodePosition; 
        private int gCost = 0;   
        private int hCost = 0;   
        private int fCost =>  gCost + hCost;  
        private bool gridIsObstacle = false; 
        private Node parentNode;
        #endregion

        public Node(Vector2Int pos)
        {
            nodePosition = pos;
            parentNode = null;
        }

        #region SetsAndGets

        public void SetGCost(int gCost) { this.gCost = gCost; }
        public void SetHCost(int hCost) { this.hCost = hCost; }
        public void SetObstacle(bool gridIsObstacle) { this.gridIsObstacle = gridIsObstacle; }
        public void SetParentNode(Node parentNode) { this.parentNode = parentNode; }

        public Vector2Int GetGridPosition() => nodePosition;
        public int GetGCost() => gCost;
        public int GetHCost() => hCost;
        public int GetFCost() => fCost;
        public bool GetIsObstacle() => gridIsObstacle;
        public Node GetParentNode() => parentNode;
        #endregion

        public int CompareTo(Node other)
        {
            int result = fCost.CompareTo(other.fCost);
            if (result == 0)
            {
                result = hCost.CompareTo(other.hCost);
            }
            return result;
        }
    }
}