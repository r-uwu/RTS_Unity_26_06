using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public enum Team { Neutral, Player, EnemyBlue, EnemyYellow }

public class Tower : MonoBehaviour
{
    public static List<Tower> AllTowers = new List<Tower>();

    public Team currentTeam = Team.Neutral;
    public int unitCount = 10;
    public int maxUnitCount = 50;
    
    [Header("Generation Settings")]
    public float generateInterval = 1f; // 생성 주기 (초)
    public int generateAmount = 1;      // 1틱당 생성량 (0으로 두면 자동회복 정지!)
    
    public float sendInterval = 0.5f;
    
    public TextMeshPro unitText;

    private List<Tower> connectedTargets = new List<Tower>();
    private Coroutine sendRoutine;
    private Renderer towerRenderer;

private void Awake()
    {
        towerRenderer = GetComponent<Renderer>();
        // 인스펙터 할당 실수나 프리팹 덮어쓰기로 인한 참조 증발을 방지하는 자동 할당
        unitText = GetComponentInChildren<TextMeshPro>(); 
        AllTowers.Add(this);
    }

    private void OnDestroy()
    {
        AllTowers.Remove(this);
    }

    private void Start()
    {
        UpdateColorAndUI();
        StartCoroutine(GenerateUnitsRoutine());
        
        if (currentTeam == Team.EnemyBlue || currentTeam == Team.EnemyYellow)
        {
            StartCoroutine(EnemyAIRoutine());
        }
    }

    public void UpdateColorAndUI()
    {
        if (unitText != null) unitText.text = unitCount.ToString();

        if (towerRenderer == null) return;
        switch (currentTeam)
        {
            case Team.Neutral: towerRenderer.material.color = Color.gray; break;
            case Team.Player: towerRenderer.material.color = Color.red; break;
            case Team.EnemyBlue: towerRenderer.material.color = Color.blue; break;
            case Team.EnemyYellow: towerRenderer.material.color = Color.yellow; break;
        }
    }

    private IEnumerator EnemyAIRoutine()
    {
        yield return new WaitForSeconds(2f);
        
        while (currentTeam == Team.EnemyBlue || currentTeam == Team.EnemyYellow)
        {
            if (AllTowers.Count > 1)
            {
                Tower target = null;
                while (target == null || target == this)
                {
                    target = AllTowers[Random.Range(0, AllTowers.Count)];
                }
                ConnectionManager.Instance.CreateLinkByAI(this, target);
            }
            yield return new WaitForSeconds(Random.Range(4f, 7f)); 
        }
    }

    private IEnumerator GenerateUnitsRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(generateInterval);
            // generateAmount가 0보다 클 때만 자동 회복 실행
            if (generateAmount > 0 && currentTeam != Team.Neutral && unitCount < maxUnitCount)
            {
                unitCount += generateAmount;
                if (unitCount > maxUnitCount) unitCount = maxUnitCount; // 최대치 초과 방지
                
                if (unitText != null) unitText.text = unitCount.ToString();
            }
        }
    }

    public void ConnectTo(Tower target)
    {
        if (!connectedTargets.Contains(target))
        {
            connectedTargets.Add(target);
            if (sendRoutine == null) sendRoutine = StartCoroutine(SendUnitsRoutine());
        }
    }

    public void DisconnectFrom(Tower target)
    {
        if (connectedTargets.Contains(target))
        {
            connectedTargets.Remove(target);
            if (connectedTargets.Count == 0 && sendRoutine != null)
            {
                StopCoroutine(sendRoutine);
                sendRoutine = null;
            }
        }
    }

    private IEnumerator SendUnitsRoutine()
    {
        while (connectedTargets.Count > 0)
        {
            yield return new WaitForSeconds(sendInterval);
            for (int i = connectedTargets.Count - 1; i >= 0; i--)
            {
                Unit unit = UnitPool.Instance.GetUnit();
                unit.transform.position = transform.position;
                unit.Initialize(currentTeam, connectedTargets[i]);
            }
        }
    }

    public void ReceiveUnit(Team unitTeam)
    {

    Debug.Log($"{gameObject.name}에 유닛 도착@@ 현재 팀: {currentTeam}, 도착 팀: {unitTeam}");
        if (unitTeam == currentTeam)
        {
            if (unitCount < maxUnitCount) unitCount++;
        }
        else
        {
            unitCount--;
            if (unitCount < 0)
            {
                currentTeam = unitTeam;
                unitCount = 1;
                
                DisconnectOutgoing();
                
                if (currentTeam == Team.EnemyBlue || currentTeam == Team.EnemyYellow)
                {
                    StartCoroutine(EnemyAIRoutine());
                }
            }
        }
        UpdateColorAndUI();
    }

    private void DisconnectOutgoing()
    {
        connectedTargets.Clear();
        if (sendRoutine != null)
        {
            StopCoroutine(sendRoutine);
            sendRoutine = null;
        }
        ConnectionManager.Instance.RemoveOutgoingLinksFrom(this);
    }
}