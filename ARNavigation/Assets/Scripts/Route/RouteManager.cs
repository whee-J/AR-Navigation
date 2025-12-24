using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;

public class RouteManager : MonoBehaviour
{
    [Header("Kakao API")]
    [SerializeField] private string kakaoRestApiKey = "b5b858b8e13c5926bd868aa63d7a5a12";

    [Header("Route")]
    public List<RoutePoint> RoutePoints { get; private set; } = new();

    [Header("Start / End (Lat,Lng)")]
    public Vector2 startLatLng;
    public Vector2 endLatLng;

    [Header("References")]
    public LocationServiceManager locationService;

    // ===============================
    // Public API
    // ===============================

    public void SetDestination(Vector2 latLng)
    {
        endLatLng = latLng;
    }

    public void RequestRoute()
    {
        if (!locationService.IsReady)
        {
            Debug.LogWarning("Location not ready");
            return;
        }

        startLatLng = locationService.GetCurrentLatLng();

        if (endLatLng == Vector2.zero)
        {
            Debug.LogError("Destination not set");
            return;
        }

        StartCoroutine(RequestKakaoRoute());
    }

    // ===============================
    // Kakao API
    // ===============================

    IEnumerator RequestKakaoRoute()
    {
        string url =
            $"https://apis-navi.kakaomobility.com/v1/directions/walk" +
            $"?origin={startLatLng.y},{startLatLng.x}" +
            $"&destination={endLatLng.y},{endLatLng.x}";

        UnityWebRequest req = UnityWebRequest.Get(url);
        req.SetRequestHeader("Authorization", $"KakaoAK {kakaoRestApiKey}");

        yield return req.SendWebRequest();

        if (req.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError($"Kakao Route Error: {req.error}");
            yield break;
        }

        BuildRoute(req.downloadHandler.text);
    }

    // ===============================
    // Route Build
    // ===============================

    void BuildRoute(string json)
    {
        RoutePoints.Clear();

        // 1️⃣ lat/lng 파싱
        List<Vector2> latLngs = KakaoRouteParser.Parse(json);

        if (latLngs.Count == 0)
        {
            Debug.LogWarning("No route points parsed");
            return;
        }

        // 2️⃣ 기준 GPS (경로 생성 시점에서 고정)
        Vector2 originLatLng = locationService.GetOriginLatLng();

        // 3️⃣ RoutePoint 생성
        foreach (var latLng in latLngs)
        {
            RoutePoint p = new RoutePoint(latLng);
            p.UpdateWorldPosition(originLatLng);   // ⭐ 핵심
            RoutePoints.Add(p);
        }

        Debug.Log($"Route built: {RoutePoints.Count} points");
    }

    // ===============================
    // Utilities
    // ===============================

    public float DistanceFromRoute(Vector3 userPos)
    {
        float min = float.MaxValue;

        foreach (var p in RoutePoints)
        {
            float d = Vector3.Distance(userPos, p.WorldPosition);
            if (d < min) min = d;
        }

        return min;
    }

    public int FindClosestPointIndex(Vector3 userWorldPos)
    {
        float minDist = float.MaxValue;
        int closestIndex = 0;

        for (int i = 0; i < RoutePoints.Count; i++)
        {
            float dist = Vector3.Distance(
                userWorldPos,
                RoutePoints[i].WorldPosition
            );

            if (dist < minDist)
            {
                minDist = dist;
                closestIndex = i;
            }
        }

        return closestIndex;
    }
}
