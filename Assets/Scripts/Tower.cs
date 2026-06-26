using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// 파랑색, 노랑색 적군을 분리하기 위해 팀 Enum 확장
public enum Team { Neutral, Player, EnemyBlue, EnemyYellow }

public class Tower : MonoBehaviour
{
    public Team currentTeam = Team.Neutral;
    public int unitCount = 10;
    public int maxUnitCount = 50;
    public float generateInterval = 1f;
    public float sendInterval = 0.5f;

    private List<Tower> connectedTargets = new List<Tower>();
    private Coroutine sendRoutine;
    private Renderer towerRenderer;

    private void Awake()
    {
        towerRenderer = GetComponent<Renderer>();
    }

    private void Start()
    {
        UpdateColor();
        StartCoroutine(GenerateUnitsRoutine());
    }

    // 소유권에 따른 타워 색상 변경
    private void UpdateColor()
    {
        if (towerRenderer == null) return;

        switch (currentTeam)
        {
            case Team.Neutral:
                towerRenderer.material.color = Color.lightGray;
                break;
            case Team.Player:
                towerRenderer.material.color = Color.red;
                break;
            case Team.EnemyBlue:
                towerRenderer.material.color = Color.blue;
                break;
            case Team.EnemyYellow:
                towerRenderer.material.color = Color.yellow;
                break;
        }
    }

    private IEnumerator GenerateUnitsRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(generateInterval);
            if (currentTeam != Team.Neutral && unitCount < maxUnitCount)
            {
                unitCount++;
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
                UpdateColor(); // 소유권 변경 시 색상 즉시 업데이트
                DisconnectAll();
            }
        }
    }

    private void DisconnectAll()
    {
        connectedTargets.Clear();
        if (sendRoutine != null)
        {
            StopCoroutine(sendRoutine);
            sendRoutine = null;
        }
        ConnectionManager.Instance.RemoveAllLinksFrom(this);
    }
}