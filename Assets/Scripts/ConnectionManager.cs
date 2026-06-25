using UnityEngine;
using UnityEngine.InputSystem; // 새로운 Input System 네임스페이스 추가

[RequireComponent(typeof(LineRenderer))]
public class ConnectionManager : MonoBehaviour
{
    private LineRenderer lineRenderer;
    private Tower selectedTower;
    private Camera mainCam;

    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.enabled = false;
        mainCam = Camera.main;
    }

    private void Update()
    {
        // 마우스 장치 연결 상태 확인
        if (Mouse.current == null) return;

        // 기존 Input.GetMouseButtonDown(0) 대체
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            HandleSelection();
        }
        // 기존 Input.GetMouseButton(0) 대체
        else if (Mouse.current.leftButton.isPressed && selectedTower != null)
        {
            DrawLine();
        }
        // 기존 Input.GetMouseButtonUp(0) 대체
        else if (Mouse.current.leftButton.wasReleasedThisFrame && selectedTower != null)
        {
            HandleConnection();
        }
    }

    private void HandleSelection()
    {
        Vector2 mousePos = Mouse.current.position.ReadValue();
        Ray ray = mainCam.ScreenPointToRay(mousePos);
        
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            Tower tower = hit.collider.GetComponent<Tower>();
            if (tower != null && tower.currentTeam == Team.Player)
            {
                selectedTower = tower;
                lineRenderer.enabled = true;
                lineRenderer.SetPosition(0, selectedTower.transform.position);
            }
        }
    }

    private void DrawLine()
    {
        Vector2 mousePos = Mouse.current.position.ReadValue();
        Ray ray = mainCam.ScreenPointToRay(mousePos);
        Plane groundPlane = new Plane(Vector3.up, Vector3.zero); // Y축 기준 바닥 평면

        if (groundPlane.Raycast(ray, out float distance))
        {
            Vector3 mousePoint = ray.GetPoint(distance);
            lineRenderer.SetPosition(1, mousePoint);
        }
    }

    private void HandleConnection()
    {
        Vector2 mousePos = Mouse.current.position.ReadValue();
        Ray ray = mainCam.ScreenPointToRay(mousePos);
        
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            Tower targetTower = hit.collider.GetComponent<Tower>();
            if (targetTower != null && targetTower != selectedTower)
            {
                selectedTower.SendUnitsTo(targetTower);
            }
        }
        
        lineRenderer.enabled = false;
        selectedTower = null;
    }
}