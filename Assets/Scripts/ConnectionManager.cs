using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class ConnectionManager : MonoBehaviour
{
    public static ConnectionManager Instance { get; private set; }

    private class Link
    {
        public Tower source;
        public Tower target;
        public GameObject lineObject;
    }

    public GameObject linePrefab;
    public LineRenderer tempLineRenderer;
    public float lineHeight = 1.0f; 

    private List<Link> activeLinks = new List<Link>();
    private Camera mainCam;
    private Tower selectedTower;

    private bool isConnecting = false;
    private bool isCutting = false;
    private Vector3 lastMouseWorldPos;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        mainCam = Camera.main;
        
        if (tempLineRenderer != null) 
        {
            tempLineRenderer.enabled = false;
        }
    }

    private void Update()
    {
        if (Mouse.current == null) return;

        Vector2 screenPos = Mouse.current.position.ReadValue();
        Vector3 worldPos = GetMouseWorldPosition(screenPos);

        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            HandlePointerDown(worldPos, screenPos);
        }
        else if (Mouse.current.leftButton.isPressed)
        {
            HandlePointerDrag(worldPos);
        }
        else if (Mouse.current.leftButton.wasReleasedThisFrame)
        {
            HandlePointerUp(worldPos, screenPos);
        }
    }

    private Vector3 GetMouseWorldPosition(Vector2 screenPos)
    {
        Ray ray = mainCam.ScreenPointToRay(screenPos);
        Plane hitPlane = new Plane(Vector3.up, new Vector3(0, lineHeight, 0));
        
        if (hitPlane.Raycast(ray, out float distance))
        {
            return ray.GetPoint(distance);
        }
        return Vector3.zero;
    }

    private void HandlePointerDown(Vector3 worldPos, Vector2 screenPos)
    {
        Ray ray = mainCam.ScreenPointToRay(screenPos);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            Tower tower = hit.collider.GetComponent<Tower>();
            
            // 내 타워를 정확히 클릭했을 때만 연결 모드 활성화
            if (tower != null && tower.currentTeam == Team.Player)
            {
                isConnecting = true;
                selectedTower = tower;
                
                // LineRenderer 정점 개수를 2개로 강제 확보
                tempLineRenderer.positionCount = 2; 
                tempLineRenderer.enabled = true;
                
                Vector3 startPos = selectedTower.transform.position;
                startPos.y = lineHeight;
                
                // 시작점과 끝점을 모두 타워 위치로 초기화
                tempLineRenderer.SetPosition(0, startPos);
                tempLineRenderer.SetPosition(1, startPos);
                return;
            }
        }
        
        // 타워가 아닌 빈 공간을 클릭했을 경우 절단 모드 진입
        isCutting = true;
        lastMouseWorldPos = worldPos;
    }

    private void HandlePointerDrag(Vector3 worldPos)
    {
        // 좌클릭을 유지한 상태로 드래그 중일 때
        if (isConnecting && selectedTower != null)
        {
            // 임시 선의 끝점(인덱스 1)을 현재 마우스 월드 좌표로 실시간 갱신
            tempLineRenderer.SetPosition(1, worldPos);
        }
        else if (isCutting)
        {
            if (Vector3.Distance(lastMouseWorldPos, worldPos) > 0.1f)
            {
                CheckIntersections(lastMouseWorldPos, worldPos);
                lastMouseWorldPos = worldPos;
            }
        }
    }

    private void HandlePointerUp(Vector3 worldPos, Vector2 screenPos)
    {
        // 좌클릭을 떼었을 때 (드롭)
        if (isConnecting)
        {
            Ray ray = mainCam.ScreenPointToRay(screenPos);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                Tower targetTower = hit.collider.GetComponent<Tower>();
                
                // 마우스를 뗀 위치가 내 타워가 아닌 다른 타워일 때만 연결 성사
                if (targetTower != null && targetTower != selectedTower)
                {
                    CreateLink(selectedTower, targetTower);
                }
            }
            
            // 연결 성공/실패 여부와 관계없이 임시 선 초기화
            tempLineRenderer.enabled = false;
            isConnecting = false;
            selectedTower = null;
        }
        
        isCutting = false;
    }

    private void CreateLink(Tower source, Tower target)
    {
        foreach (Link link in activeLinks)
        {
            if (link.source == source && link.target == target) return;
        }

        source.ConnectTo(target);

        GameObject lineObj = Instantiate(linePrefab);
        LineRenderer lr = lineObj.GetComponent<LineRenderer>();
        
        // 생성된 연결선의 정점 개수를 2개로 보장
        lr.positionCount = 2; 
        
        Vector3 p1 = source.transform.position;
        Vector3 p2 = target.transform.position;
        p1.y = lineHeight;
        p2.y = lineHeight;
        
        lr.SetPosition(0, p1);
        lr.SetPosition(1, p2);

        activeLinks.Add(new Link { source = source, target = target, lineObject = lineObj });
    }

    private void CheckIntersections(Vector3 p1, Vector3 p2)
    {
        for (int i = activeLinks.Count - 1; i >= 0; i--)
        {
            Link link = activeLinks[i];
            Vector3 p3 = link.source.transform.position;
            Vector3 p4 = link.target.transform.position;

            Vector2 lineA_Start = new Vector2(p1.x, p1.z);
            Vector2 lineA_End = new Vector2(p2.x, p2.z);
            Vector2 lineB_Start = new Vector2(p3.x, p3.z);
            Vector2 lineB_End = new Vector2(p4.x, p4.z);

            if (IsIntersecting(lineA_Start, lineA_End, lineB_Start, lineB_End))
            {
                BreakLink(link);
            }
        }
    }

    private bool IsIntersecting(Vector2 p1, Vector2 p2, Vector2 p3, Vector2 p4)
    {
        bool CCW(Vector2 A, Vector2 B, Vector2 C)
        {
            return (C.y - A.y) * (B.x - A.x) > (B.y - A.y) * (C.x - A.x);
        }
        return CCW(p1, p3, p4) != CCW(p2, p3, p4) && CCW(p1, p2, p3) != CCW(p1, p2, p4);
    }

    private void BreakLink(Link link)
    {
        link.source.DisconnectFrom(link.target);
        Destroy(link.lineObject);
        activeLinks.Remove(link);
    }

    public void RemoveAllLinksFrom(Tower tower)
    {
        for (int i = activeLinks.Count - 1; i >= 0; i--)
        {
            if (activeLinks[i].source == tower || activeLinks[i].target == tower)
            {
                BreakLink(activeLinks[i]);
            }
        }
    }
}