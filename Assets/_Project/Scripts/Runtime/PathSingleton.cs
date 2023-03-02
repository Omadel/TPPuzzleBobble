using Etienne;
using System;
using UnityEngine;

namespace PuzzleBobble
{
    [UnityEngine.DefaultExecutionOrder(-1)]
    public class PathSingleton : Singleton<PathSingleton>
    {
        Path[] path;
        private void Start()
        {
            path = GetComponents<Path>();
        }

        public Vector3[] GetClosestPath(Vector2 position)
        {
            var startIndex = 0;
            var closestPoint = 5000f;
            Path closestPath = null;
            for (int p = 0; p < path.Length; p++)
            {
                for (int i = 0; i < path[p].WorldWaypoints.Length; i++)
                {
                    var distance = Vector3.Distance(position, path[p].WorldWaypoints[i]);
                    if (distance <= closestPoint)
                    {
                        closestPoint = distance;
                        startIndex = i;
                        closestPath = path[p];
                    }
                }

            }
            var points = new Vector3[closestPath.WorldWaypoints.Length - startIndex];
            Array.Copy(closestPath.WorldWaypoints, startIndex, points, 0, points.Length);
            return points;
        }
    }
}
