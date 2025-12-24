using UnityEngine;

public static class GeoUtils
{
    private const float EarthRadius = 6378137f; // meter

    /// <summary>
    /// 위도/경도 → Unity 월드 좌표 (미터 단위)
    /// X = East, Z = North
    /// </summary>
    public static Vector3 LatLngToUnityWorld(
        Vector2 originLatLng,
        Vector2 targetLatLng
    )
    {
        float dLat = Mathf.Deg2Rad * (targetLatLng.x - originLatLng.x);
        float dLon = Mathf.Deg2Rad * (targetLatLng.y - originLatLng.y);

        float latRad = Mathf.Deg2Rad * originLatLng.x;

        float x = EarthRadius * dLon * Mathf.Cos(latRad);
        float z = EarthRadius * dLat;

        return new Vector3(x, 0f, z);
    }

    public static Vector3 LatLngToWorld(
    Vector2 originLatLng,
    Vector2 targetLatLng,
    Vector3 originWorld
)
    {
        Vector3 offset = LatLngToUnityWorld(originLatLng, targetLatLng);
        return originWorld + offset;
    }

}
