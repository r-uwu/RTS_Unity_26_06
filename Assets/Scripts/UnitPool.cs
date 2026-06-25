using UnityEngine;
using UnityEngine.Pool;

public class UnitPool : MonoBehaviour
{
    public static UnitPool Instance { get; private set; }
    public Unit unitPrefab;

    private IObjectPool<Unit> pool;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        // 기본 용량 20, 최대 100의 오브젝트 풀 생성
        pool = new ObjectPool<Unit>(CreateUnit, OnTakeFromPool, OnReturnedToPool, OnDestroyUnit, true, 20, 100);
    }

    private Unit CreateUnit() => Instantiate(unitPrefab);
    private void OnTakeFromPool(Unit unit) => unit.gameObject.SetActive(true);
    private void OnReturnedToPool(Unit unit) => unit.gameObject.SetActive(false);
    private void OnDestroyUnit(Unit unit) => Destroy(unit.gameObject);

    public Unit GetUnit() => pool.Get();
    public void ReturnUnit(Unit unit) => pool.Release(unit);
}