using UnityEngine;

namespace Farm.AStar
{
    /// <summary>
    /// 建立节点和数组的映射
    /// </summary>
    public class GridNodes
    {
        private int width;
        private int height;
        private Node[,] gridNode;

        /// <summary>
        /// 构造函数初始化节点范围数组
        /// </summary>
        /// <param name="width">地图宽度</param>
        /// <param name="height">地图高度</param>
        public GridNodes(int width, int height)
        {
            this.width = width;
            this.height = height;

            gridNode = new Node[width, height];

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    gridNode[x, y] = new Node(new Vector2Int(x, y));
                }
            }
        }

        /// <summary>
        /// 通过x和y得到当前坐标的节点数据
        /// </summary>
        /// <param name="xPos">x轴坐标</param>
        /// <param name="yPos">y轴坐标</param>
        /// <returns></returns>
        public Node GetGridNode(int xPos, int yPos)
        {
            if (xPos < width && yPos < height)
            {
                return gridNode[xPos, yPos];
            }
            return null;
        }
    }
}