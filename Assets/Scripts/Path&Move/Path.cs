using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class Path : MonoBehaviour
{
    public GameObject[] Waypoints;

    /// <summary>
    /// 路径创建时注册到全局路径表。
    /// </summary>
    private void Awake()
    {
        PathRegistry.Register(this);
    }

    /// <summary>
    /// 路径销毁时从全局路径表移除。
    /// </summary>
    private void OnDestroy()
    {
        PathRegistry.Unregister(this);
    }

    /// <summary>
    /// 根据路径点索引返回对应的世界坐标。
    /// </summary>
    public Vector3 get_position(int index)
    {
        return Waypoints[index].transform.position;
    }//获得下一个目标点方法

    /// <summary>
    /// 返回路径线段上距离指定世界坐标最近的点。
    /// </summary>
    public Vector3 GetClosestPoint(Vector3 position)
    {
        GetClosestSegmentIndex(position, out Vector3 closestPoint);
        return closestPoint;
    }

    /// <summary>
    /// 返回指定世界坐标到路径线段的最近距离。
    /// </summary>
    public float GetClosestDistance(Vector3 position)
    {
        return Vector3.Distance(position, GetClosestPoint(position));
    }

    /// <summary>
    /// 返回最近线段索引，并输出该线段上的最近点。
    /// </summary>
    public int GetClosestSegmentIndex(Vector3 position, out Vector3 closestPoint)
    {
        closestPoint = transform.position;

        if (Waypoints == null || Waypoints.Length == 0)
        {
            return -1;
        }

        if (Waypoints.Length == 1)
        {
            closestPoint = Waypoints[0].transform.position;
            return 0;
        }

        int closestSegmentIndex = 0;
        float closestDistanceSqr = float.MaxValue;

        for (int i = 0; i < Waypoints.Length - 1; i++)
        {
            Vector3 start = Waypoints[i].transform.position;
            Vector3 end = Waypoints[i + 1].transform.position;
            Vector3 pointOnSegment = GetClosestPointOnSegment(position, start, end);
            float distanceSqr = (position - pointOnSegment).sqrMagnitude;

            if (distanceSqr < closestDistanceSqr)
            {
                closestDistanceSqr = distanceSqr;
                closestSegmentIndex = i;
                closestPoint = pointOnSegment;
            }
        }

        return closestSegmentIndex;
    }

    /// <summary>
    /// 返回指定点到直线段的投影点，并限制在端点之间。
    /// </summary>
    private Vector3 GetClosestPointOnSegment(Vector3 position, Vector3 start, Vector3 end)
    {
        Vector3 segment = end - start;
        float segmentLengthSqr = segment.sqrMagnitude;

        if (segmentLengthSqr <= Mathf.Epsilon)
        {
            return start;
        }

        float t = Vector3.Dot(position - start, segment) / segmentLengthSqr;
        t = Mathf.Clamp01(t);
        return start + segment * t;
    }

    /// <summary>
    /// 在编辑器中绘制路径点名称和线段。
    /// </summary>
    private void OnDrawGizmos()
    {
#if UNITY_EDITOR
        if (Waypoints != null && Waypoints.Length > 0)
        {
            for (int i = 0; i < Waypoints.Length; i++)
            {
                GUIStyle style = new GUIStyle();
                style.normal.textColor = Color.white;
                style.alignment = TextAnchor.MiddleCenter;
                Handles.Label(Waypoints[i].transform.position + Vector3.up * 0.7f, Waypoints[i].name, style);
                if (i < Waypoints.Length - 1)
                {
                    Gizmos.color = Color.gray;
                    Gizmos.DrawLine(Waypoints[i].transform.position, Waypoints[i + 1].transform.position);
                }
            }
        }
#endif
    }
}
