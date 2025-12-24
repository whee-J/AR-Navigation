using UnityEngine;
using System.Collections;

public class ARNavigationManager : MonoBehaviour
{
    [Header("Managers")]
    public LocationServiceManager locationService;
    public RouteManager routeManager;
    public ARPathRenderer pathRenderer;
    public DirectionAnalyzer directionAnalyzer;
    public RerouteManager rerouteManager;
    public NavigationUIManager uiManager;

    [Header("Navigation Settings")]
    public float visibleDistance = 5f;
    public float arriveThreshold = 3f;
    public float offRouteThreshold = 6f;
    public float uTurnAngleThreshold = 150f;

    [Header("Debug")]
    public NavigationState currentState = NavigationState.None;

    private int currentRouteIndex = 0;
    private Vector3 userWorldPos;
    private float userHeading;

    void Start()
    {
        StartCoroutine(WaitForLocationAndStart());
    }

    IEnumerator WaitForLocationAndStart()
    {
        // GPS 준비 대기
        while (!locationService.IsReady)
            yield return null;

        // 출발 좌표 설정
        routeManager.startLatLng = locationService.GetOriginLatLng();

        // ⭐ 목적지는 RouteManager Inspector에서 설정
        routeManager.RequestRoute();

        currentState = NavigationState.Navigating;

        Debug.Log("AR Navigation Started");
    }

    void Update()
    {
        if (!locationService.IsReady) return;
        if (routeManager.RoutePoints.Count == 0) return;

        UpdateUserPose();

        // 항상 가장 가까운 포인트 기준
        currentRouteIndex =
            routeManager.FindClosestPointIndex(userWorldPos);

        EvaluateNavigationState();
        UpdateARPath();
    }

    void UpdateUserPose()
    {
        userWorldPos = locationService.GetWorldPosition();
        userHeading = locationService.GetHeading();
    }

    void EvaluateNavigationState()
    {
        RoutePoint target = routeManager.RoutePoints[currentRouteIndex];
        float distanceToTarget = Vector3.Distance(userWorldPos, target.WorldPosition);

        // 도착
        if (distanceToTarget < arriveThreshold)
        {
            AdvanceRoutePoint();
            return;
        }

        // 경로 이탈 + 가짜 이탈 필터
        bool isOffRoute =
            routeManager.DistanceFromRoute(userWorldPos) > offRouteThreshold;

        if (rerouteManager.CheckOffRoute(isOffRoute))
        {
            if (rerouteManager.CanReroute())
            {
                currentState = NavigationState.Rerouting;
                rerouteManager.RequestReroute(locationService, routeManager);
            }
            return;
        }

        // 방향 체크
        float angle = directionAnalyzer.AngleToTarget(
            userWorldPos,
            userHeading,
            target.WorldPosition
        );

        currentState = Mathf.Abs(angle) > uTurnAngleThreshold
            ? NavigationState.UTurn
            : NavigationState.Navigating;
    }

    void UpdateARPath()
    {
        uiManager.HideAll();

        switch (currentState)
        {
            case NavigationState.Navigating:
                pathRenderer.RenderForwardPath(
                    routeManager.RoutePoints,
                    currentRouteIndex,
                    userWorldPos,
                    visibleDistance
                );
                break;

            case NavigationState.UTurn:
                pathRenderer.RenderUTurnIndicator();
                uiManager.ShowUTurn();
                break;

            case NavigationState.Rerouting:
                pathRenderer.Clear();
                uiManager.ShowRerouting();
                break;
        }
    }

    void AdvanceRoutePoint()
    {
        currentRouteIndex++;

        if (currentRouteIndex >= routeManager.RoutePoints.Count)
        {
            currentState = NavigationState.Arrived;
            pathRenderer.Clear();
            Debug.Log("Arrived at destination");
        }
    }
}
