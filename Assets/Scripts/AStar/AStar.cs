using System;
using System.Collections.Generic;
using Farm.Map;
using UnityEngine;

namespace Farm.AStar
{
    public class AStar : MonoSingleton<AStar>
    {
        private NodeManager nodeManager;
        private Node startNode;
        private Node endNode;
        private Node tempNode;
        private Node tempOpenListMinNode;
        private List<Node> openList;
        private HashSet<Node> closeSet;
        private Stack<Node> shortestPathStack;
        private Dictionary<string, Node> mappingMapToNodeDict;

        private bool hasFindPath;
        private int minNode;
        private int originX;
        private int originY;

        public Stack<Node> FindShortestPath(GameScene targetScene,Vector2Int startNodePos,Vector2Int endNodePos)
        {
            BuildSceneMap(targetScene);
            startNode = nodeManager.GetNode(startNodePos.x, startNodePos.y);
            endNode = nodeManager.GetNode(endNodePos.x, endNodePos.y);

            if (BuildPath())
            {
                return NodeIntoStack();
            }
                     
            return null;
        }

        private void BuildSceneMap(GameScene targetScene)
        {
            Vector2Int mapSize = GridMapManager.Instance.GetMapSize(targetScene);
            Vector2Int originPosition = GridMapManager.Instance.GetMapOriginPosition(targetScene);

            originX = mapSize.x;
            originY = mapSize.y;

            openList ??= new List<Node>();
            closeSet ??= new HashSet<Node>();

            nodeManager = new NodeManager(mapSize.x,mapSize.y);
            mappingMapToNodeDict = new Dictionary<string, Node>();

            for(int i= 0;i<mapSize.x;i++)
            {
                for(int j = 0;j<mapSize.y;j++)
                {
                    Node node = nodeManager.GetNode(i, j);
                    mappingMapToNodeDict.Add(GetKey(targetScene, i, j), node);
                    TileDetails tileDetails = GridMapManager.Instance.GetTileDetails(new Vector2Int(i - originPosition.x, j - originPosition.y), targetScene);
                    node.walkable = !tileDetails.cantWalk;
                }
            }
        }

        private bool BuildPath()
        {
            openList.Add(startNode);

            while (openList.Count > 0)
            {
                tempOpenListMinNode = FindOpenListLeastNode();

                if(tempOpenListMinNode == endNode)
                {
                    return true;
                }

                if (tempOpenListMinNode != null)
                {
                    closeSet.Add(tempOpenListMinNode);
                    for (int i = 0; i < tempOpenListMinNode.neighborNodes.Count; i++)
                    {
                        tempNode = tempOpenListMinNode.neighborNodes[i];
                        if (!openList.Contains(tempNode) || !closeSet.Contains(tempNode))
                        {
                            openList.Add(tempNode);
                        }
                    }
                }
            }
            return false;
        }

        public Node FindOpenListLeastNode()
        {
            if (openList.Count <= 0)
                return null;
            minNode = 0;
            for(int i= 1;i<openList.Count;i++)
            {
                if(openList[i].fCost < openList[minNode].fCost)
                {
                    minNode = i;
                }
            }
            return openList[minNode];
        }

        private Stack<Node> NodeIntoStack()
        {
            if (shortestPathStack == null)
                shortestPathStack = new Stack<Node>();
            else
                shortestPathStack.Clear();

            while(endNode != startNode)
            {
                shortestPathStack.Push(endNode);
                endNode = endNode.parentNode;
            }
            return shortestPathStack;
        }

        private string GetKey(GameScene targetScene,int i,int j)
        {
            return targetScene + (i + originX).ToString() + (j + originY).ToString();
        }

        /*     private AStarNodes nodes;
             private AStarNode startNode;
             private AStarNode endNode;
             private int gridWidth;
             private int gridHeight;
             private int originX;
             private int originY;


             private List<Node> openNodeList;
             private HashSet<AStarNode> closedNodeList;

             private bool hasShorterPath;

     */

        /*        /// <summary>
                /// 构建最短路径
                /// </summary>
                /// <param name="sceneName"></param>
                /// <param name="startPos"></param>
                /// <param name="endPos"></param>
                /// <param name="npcMovementStack"></param>
                public void BuildShortestPath(GameScene targetScene, Vector2Int startPos, Vector2Int endPos, Stack<MovementStep> npcMovementStack)
                {
                    hasShorterPath = false;

                    if (GenerateGridNodes(targetScene, startPos, endPos))
                    {
                        if (FindShortestPath())
                        {
                            UpdatePathOnMovementStepStack(targetScene, npcMovementStack);
                        }
                    }
                }



                /// <summary>
                /// 构建网格节点信息，初始化两个列表
                /// </summary>
                /// <param name="sceneName">场景名字</param>
                /// <param name="startPos">起点</param>
                /// <param name="endPos">终点</param>
                /// <returns></returns>
                private bool GenerateGridNodes(GameScene targetScene, Vector2Int startPos, Vector2Int endPos)
                {
                    if (GridMapManager.Instance.GetGridSize(targetScene, out Vector2Int gridDimensions, out Vector2Int gridOrigin))
                    {
                        gridNodes = new GridNodes(gridDimensions.x, gridDimensions.y);
                        gridWidth = gridDimensions.x;
                        gridHeight = gridDimensions.y;
                        originX = gridOrigin.x;
                        originY = gridOrigin.y;

                        openNodeList ??= new List<Node>();
                        closedNodeList ??= new HashSet<Node>();
                    }
                    else
                        return false;
                    startNode = gridNodes.GetGridNode(startPos.x - originX, startPos.y - originY);
                    targetNode = gridNodes.GetGridNode(endPos.x - originX, endPos.y - originY);

                    for (int x = 0; x < gridWidth; x++)
                    {
                        for (int y = 0; y < gridHeight; y++)
                        {
                            Vector3Int tilePos = new Vector3Int(x + originX, y + originY, 0);
                            var key = tilePos.x + "x" + tilePos.y + "y" + targetScene.ToString();

                            TileDetails tile = GridMapManager.Instance.GetTileDetails(key);

                            if (tile != null)
                            {
                                Node node = gridNodes.GetGridNode(x, y);

                                if (tile.isNPCObstacle)
                                {
                                    node.SetObstacle(true);
                                }
                            }
                        }
                    }

                    return true;
                }

                /// <summary>
                /// 找到最短路径所有node添加到 closedNodeList
                /// </summary>
                /// <returns></returns>
                private bool FindShortestPath()
                {
                    openNodeList.Add(startNode);
                    while (openNodeList.Count > 0)
                    {
                        openNodeList.Sort();

                        Node closeNode = openNodeList[0];

                        openNodeList.RemoveAt(0);
                        closedNodeList.Add(closeNode);

                        if (closeNode == targetNode)
                        {
                            hasShorterPath = true;
                            break;
                        }
                        EvaluateNeighbourNodes(closeNode);
                    }

                    return hasShorterPath;
                }

                /// <summary>
                /// 评估周围8个点，并生成对应消耗值
                /// </summary>
                /// <param name="currentNode"></param>
                private void EvaluateNeighbourNodes(Node currentNode)
                {
                    Vector2Int currentNodePos = currentNode.GetGridPosition();
                    Node validNeighbourNode;

                    for (int x = -1; x <= 1; x++)
                    {
                        for (int y = -1; y <= 1; y++)
                        {
                            if (x == 0 && y == 0)
                                continue;

                            validNeighbourNode = GetValidNeighbourNode(currentNodePos.x + x, currentNodePos.y + y);

                            if (validNeighbourNode != null)
                            {
                                if (!openNodeList.Contains(validNeighbourNode) && !closedNodeList.Contains(validNeighbourNode))
                                {
                                    validNeighbourNode.SetGCost(currentNode.GetGCost() + GetDistance(currentNode, validNeighbourNode));
                                    validNeighbourNode.SetHCost(GetManhattanDistance(validNeighbourNode,targetNode));
                                    validNeighbourNode.SetParentNode(currentNode);
                                    openNodeList.Add(validNeighbourNode);
                                }
                            }
                        }
                    }
                }


                /// <summary>
                /// 找到有效的Node,非障碍，非已选择
                /// </summary>
                /// <param name="x"></param>
                /// <param name="y"></param>
                /// <returns></returns>
                private Node GetValidNeighbourNode(int x, int y)
                {
                    if (x >= gridWidth || y >= gridHeight || x < 0 || y < 0)
                        return null;

                    Node neighbourNode = gridNodes.GetGridNode(x, y);

                    if (neighbourNode.GetIsObstacle() || closedNodeList.Contains(neighbourNode))
                        return null;
                    else
                        return neighbourNode;
                }


                /// <summary>
                /// 返回两点距离值
                /// </summary>
                /// <param name="nodeA"></param>
                /// <param name="nodeB"></param>
                /// <returns>14的倍数+10的倍数</returns>
                private int GetDistance(Node nodeA, Node nodeB)
                {
                    int xDistance = Mathf.Abs(nodeA.GetGridPosition().x - nodeB.GetGridPosition().x);
                    int yDistance = Mathf.Abs(nodeA.GetGridPosition().y - nodeB.GetGridPosition().y);
                    return xDistance > yDistance ? 
                        14 * yDistance + 10 * (xDistance - yDistance): 
                        14 * xDistance + 10 * (yDistance - xDistance);
                }
                /// <summary>
                /// 得到两点间的曼哈顿距离
                /// </summary>
                /// <param name="nodeA"></param>
                /// <param name="nodeB"></param>
                /// <returns></returns>
                public int GetManhattanDistance(Node nodeA,Node nodeB)
                {
                    return 10 * (Mathf.Abs(nodeB.GetGridPosition().y - nodeA.GetGridPosition().y) 
                        + 10 * Mathf.Abs(nodeB.GetGridPosition().x - nodeA.GetGridPosition().x));
                }


                /// <summary>
                /// 更新路径每一步的坐标和场景名字
                /// </summary>
                /// <param name="sceneName"></param>
                /// <param name="npcMovementStep"></param>
                private void UpdatePathOnMovementStepStack(GameScene targetScene, Stack<MovementStep> npcMovementStep)
                {
                    Node nextNode = targetNode;

                    while (nextNode != null)
                    {
                        MovementStep newStep = new MovementStep();
                        newStep.sceneName = targetScene;
                        newStep.gridCoordinate = new Vector2Int(nextNode.GetGridPosition().x + originX, nextNode.GetGridPosition().y + originY);
                        npcMovementStep.Push(newStep);
                        nextNode = nextNode.GetParentNode();
                    }
                }
            }*/
    }
}
