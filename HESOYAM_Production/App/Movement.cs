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

        public Vector3? getMovementDirection(Vector3 sourcePosition, Vector3 targetPosition)
        {
            /*System.Console.Write(positionToCoords(sourcePosition).Item1);
            System.Console.Write(' ');
            System.Console.WriteLine(positionToCoords(sourcePosition).Item2);*/
            Tuple<int, int> sourceCoords = positionToCoords(sourcePosition);
            Tuple<int, int> targetCoords = positionToCoords(targetPosition);
            


            return null;
        }

        private Tuple<int, int> positionToCoords(Vector3 position)
        {
            Tuple<int, int> result = new Tuple<int, int>((int)(position.X / wallShift), (int)(position.Z / wallShift));
            return result;
        }

        private bool isObstacleAt(int x, int y)
        {
            if(x < 0 || x > obstacleMap.GetUpperBound(0) || y < 0 || y > obstacleMap.GetUpperBound(1))
                return true;
            else
                return obstacleMap[x, y];
        }
    }
}
