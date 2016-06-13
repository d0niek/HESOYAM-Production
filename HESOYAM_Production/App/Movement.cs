using Microsoft.Xna.Framework;
using System;
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

        public Movement(int x, int y, float wallShift)
        {
            obstacleMap = new bool[x, y];
            this.wallShift = wallShift;
        }

        public void addObstacle(int x, int y)
        {
            obstacleMap[x, y] = true;
        }

        public Vector3? getMovementTarget(Vector3 sourcePosition, Vector3 targetPosition)
        {
            /*System.Console.Write(positionToCoords(sourcePosition).Item1);
            System.Console.Write(' ');
            System.Console.WriteLine(positionToCoords(sourcePosition).Item2);*/
            Tuple<int, int> sourceCoords = positionToCoords(sourcePosition);
            Tuple<int, int> targetCoords = positionToCoords(targetPosition);

            LinkedList<Tuple<int, int>> path = aStar(sourceCoords, targetCoords);
            if(path == null) return null;
            if(path.First.Next == null) return null;
            if(path.First.Next.Next == null) return null;
            return coordsToPosition(path.First.Next.Next.Value);
            //return coordsToPosition(path.Last.Value);
        }

        public Tuple<int, int> positionToCoords(Vector3 position)
        {
            Tuple<int, int> result = new Tuple<int, int>((int)((position.X + (wallShift / 2.0)) / wallShift), (int)((position.Z + (wallShift / 2.0)) / wallShift));
            return result;
        }

        public Vector3 coordsToPosition(Tuple<int, int> coords)
        {
            Vector3 result = new Vector3((coords.Item1 * wallShift) + (wallShift / 2.0f), 0, (coords.Item2 * wallShift) + (wallShift / 2.0f));
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
            while(bestFound.Count > 0)
            {
                //if(bestFound.Count < 2)
                //{
                //    System.Console.Write('a');
                //}
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
