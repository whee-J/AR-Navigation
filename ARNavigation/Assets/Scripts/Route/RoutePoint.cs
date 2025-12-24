using UnityEngine;

/// <summary>
/// 하나의 경로 포인트 (위도/경도 + Unity 월드 좌표)
/// </summary>
public class RoutePoint
{
    public Vector2 LatLng { get; private set; }   // (lat, lng)
    public Vector3 WorldPosition { get; private set; }

    // 생성자
    public RoutePoint(Vector2 latLng)
    {
        LatLng = latLng;
        WorldPosition = Vector3.zero;
    }

    /// <summary>
    /// 기준 GPS 위치를 기준으로 Unity 월드 좌표 계산
    /// </summary>
    public void UpdateWorldPosition(Vector2 originLatLng)
    {
        WorldPosition = GeoUtils.LatLngToUnityWorld(
            originLatLng,
            LatLng
        );
    }
}
