using System;
using UnityEngine;

namespace Farm.AStar
{
    /// <summary>
    /// 每一个节点
    /// </summary>
    public class Node : IComparable<Node>
    {

        #region Fields
        private Vector2Int gridPosition; 
        private int gCost = 0;   
        private int hCost = 0;   
        private int fCost => gCost + hCost;  
        private bool isObstacle = false; 
        private Node parentNode;
        #endregion


        public Node(Vector2Int pos)
        {
            gridPosition = pos;
            parentNode = null;
        }

        #region SetsAndGets

        public void SetGCost(int gCost) { this.gCost = gCost; }
        public void SetHCost(int hCost) { this.hCost = hCost; }
        public void SetObstacle(bool isObstacle) { this.isObstacle = isObstacle; }
        public void SetParentNode(Node parentNode) { this.parentNode = parentNode; }

        public Vector2Int GetGridPosition() => gridPosition;
        public int GetGCost() => gCost;
        public int GetHCost() => hCost;
        public int GetFCost() => fCost;
        public bool GetIsObstacle() => isObstacle;
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