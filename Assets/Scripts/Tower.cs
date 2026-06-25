using UnityEngine;
using System.Collections;

public enum Team { Neutral, Player, Enemy }

public class Tower : MonoBehaviour
{
    public Team currentTeam = Team.Neutral;
    public int unitCount = 10;
    public int maxUnitCount = 50;
    public float generateInterval = 1f;

    private void Start()
    {
        StartCoroutine(GenerateUnitsRoutine());
    }

    private IEnumerator GenerateUnitsRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(generateInterval);
            // 중립 타워가 아니고, 최대치에 도달하지 않았을 때만 유닛 자동 생성
            if (currentTeam != Team.Neutral && unitCount < maxUnitCount)
            {
                unitCount++;
                UpdateUI(); 
            }
        }
    }

    public void ReceiveUnit(Team unitTeam)
    {
        if (unitTeam == currentTeam)
        {
            // 아군 유닛 도착 시 방어력(숫자) 증가
            if (unitCount < maxUnitCount) unitCount++;
        }
        else
        {
            // 적군 유닛 도착 시 숫자 감소
            unitCount--;
            if (unitCount < 0)
            {
                // 체력이 0 미만이 되면 소유권 이전 및 숫자 회복 시작
                currentTeam = unitTeam;
                unitCount = 1;
            }
        }
        UpdateUI();
    }

    public void SendUnitsTo(Tower target)
    {
        StartCoroutine(SendUnitsRoutine(target));
    }

    private IEnumerator SendUnitsRoutine(Tower target)
    {
        // 타워가 보유한 유닛의 절반을 출병
        int unitsToSend = unitCount / 2;
        for (int i = 0; i < unitsToSend; i++)
        {
            unitCount--;
            UpdateUI();

            Unit unit = UnitPool.Instance.GetUnit();
            unit.transform.position = transform.position;
            unit.Initialize(currentTeam, target);

            yield return new WaitForSeconds(0.2f); // 출병 간격
        }
    }

    private void UpdateUI()
    {
        // 향후 TextMeshPro를 연동하여 화면에 숫자 렌더링
    }
}