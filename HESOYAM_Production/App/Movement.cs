using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace App
{

    public class Movement
    {
        private bool[,] obstacleMap;
        private float wallShift;
        private Dictionary<Tuple<int, int>, LinkedList<Tuple<int, int>>> recentPaths;
        private const int maxIterations = 1000;

        public Movement(int x, int y, float wallShift)
        {
            obstacleMap = new bool[x, y];
            this.wallShift = wallShift;
            recentPaths = new Dictionary<Tuple<int, int>, LinkedList<Tuple<int, int>>>();
        }

        public void addObstacle(int x, int y)
        {
            obstacleMap[x, y] = true;
        }

        public void removeObstacle(int x, int y)
        {
            obstacleMap[x, y] = false;
        }

        public LinkedList<Tuple<int, int>> getPathToTarget(Vector3 sourcePosition, Vector3 targetPosition)
        {
            Tuple<int, int> sourceCoords = positionToCoords(sourcePosition);
            Tuple<int, int> targetCoords = positionToCoords(targetPosition);

            LinkedList<Tuple<int, int>> path = new LinkedList<Tuple<int, int>>();
            bool aStarize = false;

            if(recentPaths.ContainsKey(targetCoords))
            {
                if(recentPaths[targetCoords] != null)
                {
                    LinkedListNode<Tuple<int, int>> sourceNode = recentPaths[targetCoords].Find(sourceCoords);
                    if(sourceNode != null)
                    {
                        path.AddLast(sourceNode.Value);
                        while(sourceNode.Next != null)
                        {
                            sourceNode = sourceNode.Next;
                            path.AddLast(sourceNode.Value);
                        }
                    }
                    else
                        aStarize = true;
                }
                else
                    aStarize = true;
            }
            else
                aStarize = true;

            if(aStarize)
            {
                path = aStar(sourceCoords, targetCoords, maxIterations);
                if(recentPaths.ContainsKey(targetCoords))
                {
                    recentPaths.Remove(targetCoords);
                }
                recentPaths.Add(targetCoords, path);
            }

            return path;
        }

        public Tuple<int, int> positionToCoords(Vector3 position)
        {
            Tuple<int, int> result = new Tuple<int, int>(
                                         (int)((position.X + (wallShift / 2.0)) / wallShift),
                                         (int)((position.Z + (wallShift / 2.0)) / wallShift));
            return result;
        }

        public Vector3 coordsToPosition(Tuple<int, int> coords)
        {
            Vector3 result = new Vector3(coords.Item1 * wallShift, 0, coords.Item2 * wallShift);
            return result;
        }

        private bool isObstacleAt(Tuple<int, int> coords)
        {
            int x = coords.Item1;
            int y = coords.Item2;
            if(x < 0 || x > obstacleMap.GetUpperBound(0) || y < 0 || y > obstacleMap.GetUpperBound(1))
                return true;
            else
                return obstacleMap[x, y];
        }

        private Tuple<int, int> step(Tuple<int, int> source, int direction)
        {
            if(direction == 0)
                return new Tuple<int, int>(source.Item1 - 1, source.Item2);
            if(direction == 1)
                return new Tuple<int, int>(source.Item1 + 1, source.Item2);
            if(direction == 2)
                return new Tuple<int, int>(source.Item1, source.Item2 - 1);
            if(direction == 3)
                return new Tuple<int, int>(source.Item1, source.Item2 + 1);
            return null;
        }

        private LinkedList<Tuple<int, int>> aStar(Tuple<int, int> sourceCoords, Tuple<int, int> targetCoords, int maxIterations)
        {
            if(sourceCoords.Equals(targetCoords))
                return null;
            //if(isObstacleAt(targetCoords))
                //return null;
            int[,] visited = new int[obstacleMap.GetLength(0), obstacleMap.GetLength(1)];
            for(int x = 0; x < visited.GetLength(0); x++)
            {
                for(int y = 0; y < visited.GetLength(1); y++)
                {
                    visited[x, y] = int.MaxValue;
                }
            }
            SortedDictionary<int, LinkedList<LinkedList<Tuple<int, int>>>> bestFound = new SortedDictionary<int, LinkedList<LinkedList<Tuple<int, int>>>>();

            LinkedList<Tuple<int, int>> currentPath = new LinkedList<Tuple<int, int>>();
            currentPath.AddLast(sourceCoords);
            int currentDistance = distance(sourceCoords, targetCoords);
            if(!bestFound.ContainsKey(currentDistance))
            {
                bestFound[currentDistance] = new LinkedList<LinkedList<Tuple<int, int>>>();
            }
            bestFound[currentDistance].AddLast(currentPath);
            int iterations = 0;
            while(bestFound.Count > 0 && iterations < maxIterations)
            {
                iterations++;
                currentPath = bestFound.First().Value.First();
                bestFound.First().Value.RemoveFirst();
                if(bestFound.First().Value.Count < 1)
                {
                    bestFound.Remove(bestFound.First().Key);
                }

                Tuple<int, int> currentCoords = currentPath.Last.Value;
                visited[currentCoords.Item1, currentCoords.Item2] = currentPath.Count;
                if(currentCoords.Equals(targetCoords))
                {
                    return currentPath;
                }
                for(int i = 0; i < 4; i++)
                {
                    Tuple<int, int> currentStep = step(currentCoords, i);
                    if(!isObstacleAt(currentStep) || currentStep.Equals(targetCoords))
                    {
                        LinkedList<Tuple<int, int>> newPath = new LinkedList<Tuple<int, int>>(currentPath);
                        newPath.AddLast(currentStep);
                        currentDistance = distance(currentStep, targetCoords);
                        if(visited[currentStep.Item1, currentStep.Item2] > newPath.Count)
                        {
                            if(!bestFound.ContainsKey(currentDistance))
                            {
                                bestFound[currentDistance] = new LinkedList<LinkedList<Tuple<int, int>>>();
                            }
                            bestFound[currentDistance].AddLast(newPath);
                        }
                    }
                }
            }
            return null;
        }

        private int distance(Tuple<int, int> sourceCoords, Tuple<int, int> targetCoords)
        {
            int dx = sourceCoords.Item1 - targetCoords.Item1;
            int dy = sourceCoords.Item2 - targetCoords.Item2;
            return (dx * dx) + (dy * dy);
        }
    }
}
