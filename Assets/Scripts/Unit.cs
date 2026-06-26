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

        if (Vector3.Distance(transform.position, targetTower.transform.position) < 0.5f)
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