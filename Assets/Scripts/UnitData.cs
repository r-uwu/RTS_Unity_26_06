using UnityEngine;

[CreateAssetMenu(fileName = "NewUnitData", menuName = "Game/Unit Data")]
public class UnitData : ScriptableObject
{
    public float moveSpeed = 3.5f;
    public float health = 100f;
    public float attackDamage = 10f;

    public Sprite unitIcon;
}