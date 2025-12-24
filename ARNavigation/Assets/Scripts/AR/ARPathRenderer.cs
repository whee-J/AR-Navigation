using UnityEngine;
using System.Collections.Generic;

public class ARPathRenderer : MonoBehaviour
{
    [Header("References")]
    [Tooltip("XR Origin (AR Session Origin) Transform")]
    public Transform xrOrigin;

    [Header("Prefabs")]
    public GameObject arrowPrefab;   // 화살표 프리팹
    public GameObject dotPrefab;     // (선택) 점선 프리팹

    [Header("Settings")]
    [Tooltip("화살표 간격 (미터)")]
    public float arrowSpacing = 1.0f;

    [Tooltip("바닥에서 살짝 띄우기")]
    public float heightOffset = 0.05f;

    private readonly List<GameObject> spawnedObjects = new();

    // ===============================
    // Public API
    // ===============================

    /// <summary>
    /// 전방 visibleDistance 만큼의 경로만 AR로 표시
    /// </summary>
    public void RenderForwardPath(
    List<RoutePoint> routePoints,
    int startIndex,
    Vector3 userWorldPos,
    float visibleDistance
)
    {
        Clear();

        float accumulated = 0f;
        Vector3 lastPos = userWorldPos;

        for (int i = startIndex; i < routePoints.Count; i++)
        {
            Vector3 p = routePoints[i].WorldPosition;
            float d = Vector3.Distance(lastPos, p);

            // 너무 먼 첫 포인트 방지
            if (i == startIndex && d > visibleDistance)
                continue;

            accumulated += d;

            if (accumulated > visibleDistance)
                break;

            SpawnArrow(p, lastPos);
            lastPos = p;
        }
    }


    /// <summary>
    /// 유턴 표시 (지금은 로그만, 나중에 UI/큰 화살표 가능)
    /// </summary>
    public void RenderUTurnIndicator()
    {
        Clear();
        Debug.Log("🔄 U-TURN REQUIRED");
    }

    /// <summary>
    /// 재탐색 중 표시
    /// </summary>
    public void RenderReroutingIndicator()
    {
        Clear();
        Debug.Log("🔁 REROUTING...");
    }

    /// <summary>
    /// 모든 AR 오브젝트 제거
    /// </summary>
    public void Clear()
    {
        foreach (var obj in spawnedObjects)
        {
            if (obj != null)
                Destroy(obj);
        }

        spawnedObjects.Clear();
    }

    // ===============================
    // Internals
    // ===============================

    void SpawnArrow(Vector3 worldPosition, Vector3 forwardDir)
    {
        // XR Origin 기준 좌표로 변환
        Vector3 localPos = xrOrigin.InverseTransformPoint(worldPosition);

        GameObject arrow = Instantiate(
            arrowPrefab,
            xrOrigin.TransformPoint(localPos),
            Quaternion.identity,
            xrOrigin
        );

        if (forwardDir != Vector3.zero)
        {
            arrow.transform.rotation = Quaternion.LookRotation(forwardDir);
        }

        spawnedObjects.Add(arrow);
    }
}
