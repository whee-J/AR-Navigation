using UnityEngine;

public class DirectionAnalyzer : MonoBehaviour
{
    /// <summary>
    /// 현재 사용자 방향과 목표 지점 사이의 각도 반환
    /// </summary>
    public float AngleToTarget(
        Vector3 userPosition,
        float userHeading,
        Vector3 targetPosition
    )
    {
        Vector3 toTarget = targetPosition - userPosition;
        toTarget.y = 0;

        Vector3 forward = Quaternion.Euler(0, userHeading, 0) * Vector3.forward;

        return Vector3.SignedAngle(forward, toTarget, Vector3.up);
    }
}
