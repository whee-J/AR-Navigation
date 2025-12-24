using UnityEngine;
using System.Collections;

public class LocationServiceManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform gpsOriginTransform;

    [Header("Debug")]
    [SerializeField] private bool isReady = false;

    [SerializeField] private float gpsSmoothFactor = 0.2f;

    public bool IsReady => isReady;

    private Vector2 gpsOriginLatLng;     // 최초 GPS 기준점
    private Vector2 currentLatLng;       // 현재 위치
    private float heading;               // 나침반 각도

    private Vector3 worldOrigin;         // Unity 기준점

    // ===============================
    // Life Cycle
    // ===============================

    void Start()
    {
    #if UNITY_ANDROID
        if (!UnityEngine.Android.Permission.HasUserAuthorizedPermission(
            UnityEngine.Android.Permission.FineLocation))
        {
            UnityEngine.Android.Permission.RequestUserPermission(
                UnityEngine.Android.Permission.FineLocation);
            Debug.Log("Waiting for location permission...");
            return;
        }
    #endif

        StartCoroutine(StartLocationService());
    }


    IEnumerator StartLocationService()
    {
        // ✅ 권한 대기
        if (!Input.location.isEnabledByUser)
        {
            Debug.Log("Waiting for location permission...");
            while (!Input.location.isEnabledByUser)
                yield return null;
        }

        Input.location.Start(1f, 0.5f);
        Input.compass.enabled = true;

        int wait = 20;
        while (Input.location.status == LocationServiceStatus.Initializing && wait > 0)
        {
            yield return new WaitForSeconds(1);
            wait--;
        }

        if (Input.location.status != LocationServiceStatus.Running)
        {
            Debug.LogError("Location service failed");
            yield break;
        }

        // ✅ GPS 기준점 (한 번만)
        gpsOriginLatLng = new Vector2(
            Input.location.lastData.latitude,
            Input.location.lastData.longitude
        );

        // ✅ Unity 기준점 (고정)
        worldOrigin = gpsOriginTransform.position;

        isReady = true;

        Debug.Log($"LocationService ready | GPS:{gpsOriginLatLng} World:{worldOrigin}");
    }

    void Update()
    {
        if (!isReady) return;

        currentLatLng = Vector2.Lerp(
            currentLatLng,
            new Vector2(
                Input.location.lastData.latitude,
                Input.location.lastData.longitude
            ),
            gpsSmoothFactor
        );

        heading = Input.compass.trueHeading > 0
            ? Input.compass.trueHeading
            : Input.compass.magneticHeading;
        if (heading <= 0f)
            heading = Camera.main.transform.eulerAngles.y;
    }

    // ===============================
    // Public API
    // ===============================

    /// <summary>
    /// 최초 GPS 기준점 (Route 생성용)
    /// </summary>
    public Vector2 GetOriginLatLng()
    {
        return gpsOriginLatLng;
    }

    /// <summary>
    /// 현재 GPS 위치
    /// </summary>
    public Vector2 GetCurrentLatLng()
    {
        return currentLatLng;
    }

    /// <summary>
    /// 현재 Unity World 위치
    /// </summary>
    public Vector3 GetWorldPosition()
    {
        return GeoUtils.LatLngToWorld(
            gpsOriginLatLng,
            currentLatLng,
            worldOrigin
        );
    }

    /// <summary>
    /// 현재 바라보는 방향 (0~360)
    /// </summary>
    public float GetHeading()
    {
        return heading;
    }
}
