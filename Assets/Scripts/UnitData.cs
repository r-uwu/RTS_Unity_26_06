using UnityEngine;

[CreateAssetMenu(fileName = "NewUnitData", menuName = "ScriptableObjects/UnitData")]
public class UnitData : ScriptableObject
{
    public float moveSpeed = 3.5f;
    public float health = 100f;
    public float attackDamage = 10f;

    public int unitId;
    public Sprite unitIcon;
}