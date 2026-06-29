using System.Collections.Generic;
using UnityEngine;

public static class PathRegistry
{
    private static readonly List<Path> paths = new List<Path>();

    /// <summary>
    /// 注册场景中的路径，供实体按位置查找最近路径。
    /// </summary>
    public static void Register(Path path)
    {
        if (path == null || paths.Contains(path)) return;

        paths.Add(path);
    }

    /// <summary>
    /// 注销已销毁或离开场景的路径。
    /// </summary>
    public static void Unregister(Path path)
    {
        if (path == null) return;

        paths.Remove(path);
    }

    /// <summary>
    /// 查找距离指定世界坐标最近的路径，并输出路径上的最近点。
    /// </summary>
    public static Path GetNearestPath(Vector3 position, out Vector3 nearestPoint)
    {
        return GetNearestPath(position, out nearestPoint, out _);
    }

    /// <summary>
    /// 查找距离指定世界坐标最近的路径，并输出最近点所在的线段索引。
    /// </summary>
    public static Path GetNearestPath(Vector3 position, out Vector3 nearestPoint, out int segmentIndex)
    {
        nearestPoint = position;
        segmentIndex = -1;

        Path nearestPath = null;
        float nearestDistanceSqr = float.MaxValue;

        for (int i = paths.Count - 1; i >= 0; i--)
        {
            Path path = paths[i];
            if (path == null)
            {
                paths.RemoveAt(i);
                continue;
            }

            int candidateSegmentIndex = path.GetClosestSegmentIndex(position, out Vector3 candidatePoint);
            if (candidateSegmentIndex < 0) continue;

            float distanceSqr = (position - candidatePoint).sqrMagnitude;
            if (distanceSqr >= nearestDistanceSqr) continue;

            nearestPath = path;
            nearestPoint = candidatePoint;
            segmentIndex = candidateSegmentIndex;
            nearestDistanceSqr = distanceSqr;
        }

        return nearestPath;
    }
}
