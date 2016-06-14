using Microsoft.Xna.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HESOYAM_Production.App
{
    public class Movement
    {
        private bool[,] obstacleMap;
        private float wallShift;
        private Dictionary<Tuple<int, int>, LinkedList<Tuple<int, int>>> recentPaths;
        private const int maxIterations = 3000;

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

        public LinkedList<Tuple<int, int>> getPathToTarget(Vector3 sourcePosition, Vector3 targetPosition)
        {
            Tuple<int, int> sourceCoords = positionToCoords(sourcePosition);
            Tuple<int, int> targetCoords = positionToCoords(targetPosition);

            LinkedList<Tuple<int, int>> path = new LinkedList<Tuple<int, int>>();
            bool aStarize = false;

            if(recentPaths.ContainsKey(targetCoords))
            {
                if(recentPaths[targetCoords] == null) return null;
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
                else aStarize = true;
            }
            else aStarize = true;

            if(aStarize)
            {
                path = aStar(sourceCoords, targetCoords);
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
            Tuple<int, int> result = new Tuple<int, int>((int)((position.X + (wallShift / 2.0)) / wallShift), (int)((position.Z + (wallShift / 2.0)) / wallShift));
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
            if(direction == 0) return new Tuple<int, int>(source.Item1 - 1, source.Item2);
            if(direction == 1) return new Tuple<int, int>(source.Item1 + 1, source.Item2);
            if(direction == 2) return new Tuple<int, int>(source.Item1, source.Item2 - 1);
            if(direction == 3) return new Tuple<int, int>(source.Item1, source.Item2 + 1);
            return null;
        }

        private LinkedList<Tuple<int, int>> aStar(Tuple<int, int> sourceCoords, Tuple<int, int> targetCoords)
        {
            if(sourceCoords.Equals(targetCoords)) return null;
            if(isObstacleAt(targetCoords)) return null;
            bool[,] visited = new bool[obstacleMap.GetLength(0), obstacleMap.GetLength(1)];
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
                visited[currentCoords.Item1, currentCoords.Item2] = true;
                if(currentCoords.Equals(targetCoords))
                {
                    return currentPath;
                }
                //System.Console.Write(currentCoords.Item1);
                //System.Console.Write(' ');
                //System.Console.WriteLine(currentCoords.Item2);
                for(int i = 0; i < 4; i++)
                {
                    Tuple<int, int> currentStep = step(currentCoords, i);
                    if(!isObstacleAt(currentStep))
                    {
                        if(!visited[currentStep.Item1, currentStep.Item2])
                        {
                            LinkedList<Tuple<int, int>> newPath = new LinkedList<Tuple<int, int>>(currentPath);
                            currentDistance = distance(currentStep, targetCoords);
                            newPath.AddLast(currentStep);
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
