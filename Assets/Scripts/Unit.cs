using UnityEngine;

public class Unit : MonoBehaviour
{
    public float speed = 5f;
    private Team team;
    private Tower targetTower;

    public void Initialize(Team team, Tower target)
    {
        this.team = team;
        this.targetTower = target;
    }

    private void Update()
    {
        if (targetTower == null) return;

        // 타겟 타워를 향해 직선 이동
        transform.position = Vector3.MoveTowards(transform.position, targetTower.transform.position, speed * Time.deltaTime);

        // 타워 도착 판정 (거리 0.5f 이내)
        if (Vector3.Distance(transform.position, targetTower.transform.position) < 0.5f)
        {
            targetTower.ReceiveUnit(team);
            UnitPool.Instance.ReturnUnit(this);
            targetTower = null; // 초기화
        }
    }
}