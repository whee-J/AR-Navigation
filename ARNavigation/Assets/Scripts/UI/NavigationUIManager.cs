using UnityEngine;

public class NavigationUIManager : MonoBehaviour
{
    [Header("UI Objects")]
    public GameObject reroutingUI;   // "경로 재탐색 중"
    public GameObject uTurnUI;       // "유턴하세요"

    void Start()
    {
        HideAll();
    }

    public void ShowRerouting()
    {
        HideAll();
        reroutingUI.SetActive(true);
    }

    public void ShowUTurn()
    {
        HideAll();
        uTurnUI.SetActive(true);
    }

    public void HideAll()
    {
        if (reroutingUI) reroutingUI.SetActive(false);
        if (uTurnUI) uTurnUI.SetActive(false);
    }
}
