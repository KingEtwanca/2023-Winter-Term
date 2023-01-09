using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;
using Unity.Collections;
using Unity.Burst;


//this code is adapted from  youtube channel Code Monkey
//https://www.youtube.com/watch?v=1bO1FdEThnU
public class PathFinding : MonoBehaviour
{
    //via trig
    private const int MOVE_STRAIGHT_COST = 10;
    private const int MOVE_DIAGONAL_COST = 14;

    //Test function
   


    private void FindPath(int2 startPosition, int2 endPosition) {
        int2 gridSize = new int2(4, 4);

        NativeArray<PathNode> pathNodeArray = new NativeArray<PathNode>(gridSize.x*gridSize.y, Allocator.Temp);
        

        //initialize nodes
        for(int x = 0; x < gridSize.x; x++) {
            for(int y = 0; y<gridSize.y; y++) {
                PathNode pathNode = new PathNode();
                pathNode.x = x;
                pathNode.y = y;
                pathNode.index = CalculateIndex(x, y, gridSize.x);

                //initialize costs
                pathNode.gCost = int.MaxValue;
                pathNode.hCost = CalculateDistanceCost(new int2(x,y),endPosition);
                pathNode.CalculateFCost();

                pathNode.isWalkable = true;
                pathNode.cameFromNodeIndex = -1;       //-1 is the invalid value 

                pathNodeArray[pathNode.index] = pathNode;
            }
        }

        NativeArray<int2> neighbourOffsetArray = new NativeArray<int2>(8, Unity.Collections.Allocator.Temp);
        neighbourOffsetArray[0] = new int2(-1, 0);     //Left
        neighbourOffsetArray[1] = new int2(+1, 0);    //Right
        neighbourOffsetArray[2] = new int2(0, +1);    //Up
        neighbourOffsetArray[3] = new int2(0, -1);    //Down
        neighbourOffsetArray[4] = new int2(-1, -1);     //Left Down
        neighbourOffsetArray[5] = new int2(-1, +1);    //Left Up
        neighbourOffsetArray[6] = new int2(+1, -1);    //Right Down
        neighbourOffsetArray[7] = new int2(+1, +1);    //Right Up

        int endNodeIndex = CalculateIndex(endPosition.x, endPosition.y, gridSize.x);
        
        PathNode startNode = pathNodeArray[CalculateIndex(startPosition.x,startPosition.y,gridSize.x)];
        startNode.gCost = 0;
        startNode.CalculateFCost();
        pathNodeArray[startNode.index] = startNode;     //update array

        //for algorithm
        NativeList<int> openList = new NativeList<int>(Allocator.Temp);
        NativeList<int> closedList = new NativeList<int>(Allocator.Temp);

        //here is the setup for A* pathfinding Algorithm
        openList.Add(startNode.index);

        while(openList.Length > 0) {
            int currentNodeIndex = GetLowestCostFNodeIndex(openList,pathNodeArray);
            PathNode currentNode = pathNodeArray[currentNodeIndex];

            if(currentNodeIndex == endNodeIndex) {
                //Reached destination
                break;
            }

            //Remove Current Node from OpenList
            for(int i=0; i<openList.Length; i++) { 
                if(openList[i] == currentNodeIndex)  {
                    openList.RemoveAtSwapBack(i);
                    break;
                }
            }

            closedList.Add(currentNodeIndex);

            for(int i=0; i<neighbourOffsetArray.Length; i++) {
                int2 neighbourOffset = neighbourOffsetArray[i];
                int2 neighbourPosition = new int2(currentNode.x+neighbourOffset.x, currentNode.y+neighbourOffset.y);

                //must check if node is valid
                if (!IsPositionInsideGrid(neighbourPosition, gridSize)) {
                    //Neighbour not a valid position
                    continue;
                }

                int neighbourNodeIndex = CalculateIndex(neighbourPosition.x, neighbourPosition.y, gridSize.x);
                
                if(closedList.Contains(neighbourNodeIndex)) {
                    //Already searched this node
                    continue;
                }

                PathNode neighbourNode = pathNodeArray[neighbourNodeIndex];
                if (!neighbourNode.isWalkable) {
                    //Not walkable
                    continue;
                }

                //if this point is reached, neighbourNode is valid
                //run A* pathfinding algorithm
                int2 currentNodePosition = new int2(currentNode.x, currentNode.y);

                int tentativeGCost = currentNode.gCost + CalculateDistanceCost(currentNodePosition, neighbourPosition);
                if(tentativeGCost < neighbourNode.gCost) {
                    neighbourNode.cameFromNodeIndex = currentNodeIndex;
                    neighbourNode.gCost = tentativeGCost;
                    neighbourNode.CalculateFCost();
                    pathNodeArray[neighbourNodeIndex] = neighbourNode;

                    if (!openList.Contains(neighbourNode.index)) {
                        openList.Add(neighbourNode.index);
                    }
                }

            }

        }
        //end A* pathfinding Algorithm

        PathNode endNode = pathNodeArray[endNodeIndex];
        if(endNode.cameFromNodeIndex == -1) {
            //Didn't find a path
        }
        else {
            //Found a path
            NativeList<int2> path = CalculatePath(pathNodeArray, endNode);

            path.Dispose();
        }

        pathNodeArray.Dispose();
        neighbourOffsetArray.Dispose();
        openList.Dispose();
        closedList.Dispose();
    }

    private NativeList<int2> CalculatePath(NativeArray<PathNode> pathNodeArray, PathNode endNode) { 
        if(endNode.cameFromNodeIndex == -1) {
            //Couldn't find a path
            return new NativeList<int2>(Allocator.Temp);
        }
        else {
            //Found a path
            NativeList<int2> path = new NativeList<int2>(Allocator.Temp);
            path.Add(new int2(endNode.x, endNode.y));

            PathNode currentNode = endNode;
            while(currentNode.cameFromNodeIndex != -1) {
                PathNode cameFormNode = pathNodeArray[currentNode.cameFromNodeIndex];
                path.Add(new int2(cameFormNode.x, cameFormNode.y));
                currentNode = cameFormNode;
            }
            return path;        //note this path is inverted
        }
    }

    private bool IsPositionInsideGrid(int2 gridPosition, int2 gridSize) {
        return
            gridPosition.x >= 0 &&
            gridPosition.y >= 0 &&
            gridPosition.x < gridSize.x &&
            gridPosition.y < gridSize.y;
    }

    private int CalculateIndex(int x, int y, int gridWidth) {
        return x + y * gridWidth;
    }

    private int CalculateDistanceCost(int2 aPosition, int2 bPosition) {
        int xDistance = math.abs(aPosition.x - bPosition.x);
        int yDistance = math.abs(aPosition.y - bPosition.y);
        int remaining = math.abs(xDistance - yDistance);
        return MOVE_DIAGONAL_COST * math.min(xDistance, yDistance) + MOVE_STRAIGHT_COST * remaining;
    }

    private int GetLowestCostFNodeIndex(NativeList<int> openList, NativeArray<PathNode> pathNodeArray) {
        PathNode lowestCostPathNode = pathNodeArray[openList[0]];
        for(int i = 1; i > openList.Length; i++) {
            PathNode testPathNode = pathNodeArray[openList[i]];
            if(testPathNode.fCost< lowestCostPathNode.fCost){
                lowestCostPathNode = testPathNode;
            }
        }
        return lowestCostPathNode.index;
    }

    //this is the dots pathfinding node
    private struct PathNode {
        public int x;
        public int y;

        public int index;       //since this is dots, an index is used instead of an actual object

        public int gCost;       //movement cost
        public int hCost;       //dist to target
        public int fCost;       //final cost

        public bool isWalkable;

        public int cameFromNodeIndex;

        public void CalculateFCost() {
            fCost = gCost + hCost;
        }
    }
}
