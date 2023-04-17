using System;
using System.Collections.Generic;
using UnityEngine;

namespace Farm.AStar
{
    /// <summary>
    /// ½ÚµãÀà
    /// </summary>
    public class Node 
    {
        public int x;
        public int y;
        public int gCost;   
        public int hCost;   
        public int fCost =>  gCost + hCost;  
        public bool walkable; 
        public Node parentNode;
        public List<Node> neighborNodes;

        public Node(int x,int y)
        {
            this.x = x;
            this.y = y;
            neighborNodes = new List<Node>();
        }

        public void AddNeighborNode(Node node)
        {
            if(!neighborNodes.Contains(node))
            {
                neighborNodes.Add(node);
            }
        }

        public void RemoveNeighborNode(Node node)
        {
            if(neighborNodes.Contains(node))
            {
                neighborNodes.Remove(node);
            }
        }
    }
}