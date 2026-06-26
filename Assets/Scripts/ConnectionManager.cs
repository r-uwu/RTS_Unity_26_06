using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class ConnectionManager : MonoBehaviour
{
    public static ConnectionManager Instance { get; private set; }

    public class Link
    {
        public Tower source;
        public Tower target;
        public GameObject lineObject;
        public LineRenderer lineRenderer;
        public bool isMutual; 
    }

    public GameObject linePrefab;
    public float lineHeight = 1.0f; 

    private LineRenderer tempLineRenderer;
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
        
        GameObject tempLineObj = new GameObject("TempLineRenderer");
        tempLineObj.transform.SetParent(this.transform);
        tempLineRenderer = tempLineObj.AddComponent<LineRenderer>();
        tempLineRenderer.positionCount = 2;
        tempLineRenderer.startWidth = 0.3f;
        tempLineRenderer.endWidth = 0.3f;
        tempLineRenderer.enabled = false;
        
        if (linePrefab != null)
        {
            tempLineRenderer.material = linePrefab.GetComponent<LineRenderer>().sharedMaterial;
        }
    }

    private void Update()
    {
        if (Mouse.current == null) return;
        Vector2 screenPos = Mouse.current.position.ReadValue();
        Vector3 worldPos = GetMouseWorldPosition(screenPos);

        if (Mouse.current.leftButton.wasPressedThisFrame) HandlePointerDown(worldPos, screenPos);
        else if (Mouse.current.leftButton.isPressed) HandlePointerDrag(worldPos);
        else if (Mouse.current.leftButton.wasReleasedThisFrame) HandlePointerUp(worldPos, screenPos);
    }

    private Vector3 GetMouseWorldPosition(Vector2 screenPos)
    {
        Ray ray = mainCam.ScreenPointToRay(screenPos);
        Plane hitPlane = new Plane(Vector3.up, new Vector3(0, lineHeight, 0));
        if (hitPlane.Raycast(ray, out float distance)) return ray.GetPoint(distance);
        return Vector3.zero;
    }

    public Color GetTeamColor(Team team)
    {
        switch (team)
        {
            case Team.Player: return Color.red;
            case Team.EnemyBlue: return Color.blue;
            case Team.EnemyYellow: return Color.yellow;
            default: return Color.gray;
        }
    }

    private Tower FindTowerUnderMouse(Vector2 screenPos)
    {
        Ray ray = mainCam.ScreenPointToRay(screenPos);
        RaycastHit[] hits = Physics.RaycastAll(ray, 100f);
        
        foreach (RaycastHit hit in hits)
        {
            Tower tower = hit.collider.GetComponent<Tower>();
            if (tower != null) return tower;
        }
        return null;
    }

    private void HandlePointerDown(Vector3 worldPos, Vector2 screenPos)
    {
        Tower tower = FindTowerUnderMouse(screenPos);
        
        if (tower != null && tower.currentTeam == Team.Player)
        {
            isConnecting = true;
            selectedTower = tower;
            
            tempLineRenderer.enabled = true;
            Color myColor = GetTeamColor(selectedTower.currentTeam);
            tempLineRenderer.startColor = myColor;
            tempLineRenderer.endColor = new Color(myColor.r, myColor.g, myColor.b, 0.5f);
            
            Vector3 startPos = selectedTower.transform.position;
            startPos.y = lineHeight;
            tempLineRenderer.SetPosition(0, startPos);
            tempLineRenderer.SetPosition(1, worldPos);
            return;
        }
        
        isCutting = true;
        lastMouseWorldPos = worldPos;
    }

    private void HandlePointerDrag(Vector3 worldPos)
    {
        if (isConnecting && selectedTower != null)
        {
            tempLineRenderer.SetPosition(1, worldPos);
        }
        else if (isCutting && Vector3.Distance(lastMouseWorldPos, worldPos) > 0.1f)
        {
            CheckIntersections(lastMouseWorldPos, worldPos);
            lastMouseWorldPos = worldPos;
        }
    }

    private void HandlePointerUp(Vector3 worldPos, Vector2 screenPos)
    {
        if (isConnecting)
        {
            Tower targetTower = FindTowerUnderMouse(screenPos);
            if (targetTower != null && targetTower != selectedTower)
            {
                CreateLink(selectedTower, targetTower);
            }
            tempLineRenderer.enabled = false;
            isConnecting = false;
            selectedTower = null;
        }
        isCutting = false;
    }

    public void CreateLinkByAI(Tower source, Tower target)
    {
        CreateLink(source, target);
    }

    private void CreateLink(Tower source, Tower target)
    {
        foreach (Link link in activeLinks)
        {
            if (link.source == source && link.target == target) return;
        }

        foreach (Link link in activeLinks)
        {
            if (link.source == target && link.target == source)
            {
                if (source.currentTeam == target.currentTeam) return; 
                else
                {
                    link.isMutual = true;
                    UpdateLineGradient(link);
                    source.ConnectTo(target);
                    return;
                }
            }
        }

        source.ConnectTo(target);
        GameObject lineObj = Instantiate(linePrefab);
        LineRenderer lr = lineObj.GetComponent<LineRenderer>();
        
        lr.positionCount = 2; 
        lr.startWidth = 0.3f;
        lr.endWidth = 0.3f;
        lr.useWorldSpace = true;

        Vector3 p1 = source.transform.position;
        Vector3 p2 = target.transform.position;
        p1.y = lineHeight;
        p2.y = lineHeight;
        
        lr.SetPosition(0, p1);
        lr.SetPosition(1, p2);

        Link newLink = new Link { source = source, target = target, lineObject = lineObj, lineRenderer = lr, isMutual = false };
        activeLinks.Add(newLink);
        UpdateLineGradient(newLink);
    }

    private void UpdateLineGradient(Link link)
    {
        Gradient gradient = new Gradient();
        Color sourceColor = GetTeamColor(link.source.currentTeam);
        
        if (link.isMutual)
        {
            Color targetColor = GetTeamColor(link.target.currentTeam);
            gradient.SetKeys(
                new GradientColorKey[] { new GradientColorKey(sourceColor, 0.0f), new GradientColorKey(sourceColor, 0.499f), new GradientColorKey(targetColor, 0.501f), new GradientColorKey(targetColor, 1.0f) },
                new GradientAlphaKey[] { new GradientAlphaKey(1f, 0f), new GradientAlphaKey(1f, 1f) }
            );
        }
        else
        {
            gradient.SetKeys(
                new GradientColorKey[] { new GradientColorKey(sourceColor, 0.0f), new GradientColorKey(sourceColor, 1.0f) },
                new GradientAlphaKey[] { new GradientAlphaKey(1f, 0f), new GradientAlphaKey(1f, 1f) }
            );
        }
        link.lineRenderer.colorGradient = gradient;
    }

    private void CheckIntersections(Vector3 p1, Vector3 p2)
    {
        for (int i = activeLinks.Count - 1; i >= 0; i--)
        {
            Link link = activeLinks[i];
            Vector3 p3 = link.source.transform.position;
            Vector3 p4 = link.target.transform.position;
            
            if (IsIntersecting(new Vector2(p1.x, p1.z), new Vector2(p2.x, p2.z), new Vector2(p3.x, p3.z), new Vector2(p4.x, p4.z)))
            {
                // 적군의 단일 라인은 절단 무시
                if (!link.isMutual && link.source.currentTeam != Team.Player) continue;

                if (link.isMutual)
                {
                    // 상호 공격 중일 때, 플레이어의 연결만 해제하고 적의 연결은 단일 라인으로 전환
                    if (link.source.currentTeam == Team.Player)
                    {
                        link.source.DisconnectFrom(link.target);
                        Tower temp = link.source;
                        link.source = link.target;
                        link.target = temp;
                        link.isMutual = false;
                        UpdateLineGradient(link);
                    }
                    else if (link.target.currentTeam == Team.Player)
                    {
                        link.target.DisconnectFrom(link.source);
                        link.isMutual = false;
                        UpdateLineGradient(link);
                    }
                }
                else
                {
                    BreakLink(link);
                }
            }
        }
    }

    private bool IsIntersecting(Vector2 p1, Vector2 p2, Vector2 p3, Vector2 p4)
    {
        bool CCW(Vector2 A, Vector2 B, Vector2 C) => (C.y - A.y) * (B.x - A.x) > (B.y - A.y) * (C.x - A.x);
        return CCW(p1, p3, p4) != CCW(p2, p3, p4) && CCW(p1, p2, p3) != CCW(p1, p2, p4);
    }

    private void BreakLink(Link link)
    {
        link.source.DisconnectFrom(link.target);
        if (link.isMutual)
        {
            link.target.DisconnectFrom(link.source); 
        }
        Destroy(link.lineObject);
        activeLinks.Remove(link);
    }

    public void RemoveOutgoingLinksFrom(Tower tower)
    {
        for (int i = activeLinks.Count - 1; i >= 0; i--)
        {
            Link link = activeLinks[i];
            if (link.source == tower)
            {
                if (link.isMutual)
                {
                    link.source.DisconnectFrom(link.target);
                    Tower temp = link.source;
                    link.source = link.target;
                    link.target = temp;
                    link.isMutual = false;
                    UpdateLineGradient(link);
                }
                else
                {
                    BreakLink(link);
                }
            }
            else if (link.target == tower && link.isMutual)
            {
                link.target.DisconnectFrom(link.source);
                link.isMutual = false;
                UpdateLineGradient(link);
            }
        }
    }
}