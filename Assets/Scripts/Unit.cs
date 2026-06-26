using UnityEngine;

[RequireComponent(typeof(SphereCollider), typeof(Rigidbody))]
public class Unit : MonoBehaviour
{
    public float speed = 5f;
    public Team currentTeam { get; private set; }
    private Tower targetTower;

    public void Initialize(Team team, Tower target)
    {
        this.currentTeam = team;
        this.targetTower = target;
    }

private void Update()
    {
        if (targetTower == null) return;
        transform.position = Vector3.MoveTowards(transform.position, targetTower.transform.position, speed * Time.deltaTime);

        // Y축 고도 차이를 무시하기 위해 X, Z 좌표만 추출하여 거리 계산
        Vector3 posA = new Vector3(transform.position.x, 0, transform.position.z);
        Vector3 posB = new Vector3(targetTower.transform.position.x, 0, targetTower.transform.position.z);

        if (Vector3.Distance(posA, posB) < 1.0f)
        {
            targetTower.ReceiveUnit(currentTeam);
            UnitPool.Instance.ReturnUnit(this);
            targetTower = null;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Unit otherUnit = other.GetComponent<Unit>();
        // 상대방 유닛이고, 두 유닛 모두 활성화된 상태일 때만 충돌 처리 (중복 반환 방지)
        if (otherUnit != null && otherUnit.currentTeam != this.currentTeam)
        {
            if (this.gameObject.activeInHierarchy && otherUnit.gameObject.activeInHierarchy)
            {
                UnitPool.Instance.ReturnUnit(this);
                UnitPool.Instance.ReturnUnit(otherUnit);
            }
        }
    }
}