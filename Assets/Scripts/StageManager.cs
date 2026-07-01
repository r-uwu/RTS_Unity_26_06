using UnityEngine;

public class StageManager : MonoBehaviour
{
    // 로비와 동일한 ScriptableObject 에셋 파일을 인스펙터에서 할당합니다.
    [SerializeField] private CurrentDeckData deckData;
    [SerializeField] private UserProfileData profileData;

    void Start()
    {
        InitializeStage();
    }

    private void InitializeStage()
    {
        // 로비에서 수정한 데이터가 그대로 로드됩니다.
        Debug.Log($"스테이지 시작 - 플레이어 이름: {profileData.userName}");

        // 선택된 유닛 덱 정보를 기반으로 실제 인게임 오브젝트를 배치합니다.
        for (int i = 0; i < deckData.selectedUnitIds.Length; i++)
        {
            int unitId = deckData.selectedUnitIds[i];
            if (unitId != -1)
            {
                SpawnUnit(i, unitId);
            }
            else
            {
                Debug.Log($"{i}번 슬롯은 비어있습니다.");
            }
        }
    }

    private void SpawnUnit(int slotIndex, int unitId)
    {
        // 프리팹 딕셔너리나 배열에서 unitId에 해당하는 유닛을 찾아 생성하는 로직을 수행합니다.
        Debug.Log($"{slotIndex}번 위치에 {unitId}번 유닛 오브젝트를 동적 생성합니다.");
    }
}