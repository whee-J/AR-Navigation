using UnityEngine;

public class RerouteManager : MonoBehaviour
{
    public float rerouteCooldown = 5f;
    public float offRouteConfirmTime = 2f; // ⭐ 2초 지속 시만 인정

    private float offRouteTimer = 0f;
    private float lastRerouteTime = -999f;

    public bool CheckOffRoute(bool isOffRoute)
    {
        if (isOffRoute)
        {
            offRouteTimer += Time.deltaTime;
            return offRouteTimer >= offRouteConfirmTime;
        }
        else
        {
            offRouteTimer = 0f;
            return false;
        }
    }

    public bool CanReroute()
    {
        return Time.time - lastRerouteTime > rerouteCooldown;
    }

    public void RequestReroute(
        LocationServiceManager locationService,
        RouteManager routeManager
    )
    {
        lastRerouteTime = Time.time;

        routeManager.startLatLng = locationService.GetCurrentLatLng();
        routeManager.RequestRoute();

        offRouteTimer = 0f;

        Debug.Log("🔁 Reroute confirmed");
    }
}