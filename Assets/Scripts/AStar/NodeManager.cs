using UnityEngine;

namespace Farm.AStar
{
    //节点管理类
    public class NodeManager
    {
        public int mapWidth;
        public int mapHeight;
        public Node[,] nodes;

        public NodeManager(int mapWidth,int mapHeight)
        {
            this.mapWidth = mapWidth;
            this.mapHeight = mapHeight;

            nodes = new Node[mapWidth, mapHeight];

            for(int x = 0; x < mapWidth;x++)
            {
                for(int y = 0;y < mapHeight;y++)
                {
                    nodes[x, y] = new Node(x, y);
                    Debug.Log("Node" + x.ToString() + ":" + y.ToString());
                }
            }
            RefeshNode();
        }

        public void RefeshNode()
        {
            for (int x = 0; x < mapWidth; x++)
            {
                for (int y = 0; y < mapHeight; y++)
                {
                    AddNeighborNode(x, y);
                }
            }
        }

        public void AddNeighborNode(int x,int y)
        {
            Node rootNode = GetNode(x, y);
            for(int i = x-1; i<= x+1;i++ )
            {
                for(int j = y-1;j<=j+1;j++)
                {
                    if(i != x && j != y && GetNode(i,j)!=null)
                    {
                        rootNode.AddNeighborNode(GetNode(i, j));
                    }
                }
            }
        }

        public Node GetNode(int x,int y)
        {
            if (x < 0 || x >= mapWidth || y < 0 || y >= mapHeight)
                return null;
            return nodes[x, y];
        }
    }
}