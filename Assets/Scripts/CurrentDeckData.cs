using UnityEngine;

[CreateAssetMenu(fileName = "CurrentDeckData", menuName = "ScriptableObjects/CurrentDeckData")]
public class CurrentDeckData : ScriptableObject
{
    // 출전 슬롯 배열 (예: 3개 슬롯). -1은 빈 슬롯을 의미합니다.
    public int[] selectedUnitIds = new int[3] { -1, -1, -1 };

    // 특정 슬롯의 유닛을 교체하는 메서드
    public void SetUnitInSlot(int slotIndex, int unitId)
    {
        if (slotIndex >= 0 && slotIndex < selectedUnitIds.Length)
        {
            selectedUnitIds[slotIndex] = unitId;
        }
    }

    // 데이터 초기화 메서드 (게임 시작 시 또는 테스트 시 호출)
    public void ResetData()
    {
        for (int i = 0; i < selectedUnitIds.Length; i++)
        {
            selectedUnitIds[i] = -1;
        }
    }
}