using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LobbyUiManager : MonoBehaviour
{
    // 인스펙터창에서 위에서 만든 ScriptableObject 에셋을 각각 드래그 앤 드롭하여 할당합니다.
    [SerializeField] private CurrentDeckData deckData;
    [SerializeField] private UserProfileData profileData;

    [SerializeField] private InputField nameInputField;

    // 이름 수정 완료 버튼 이벤트 바인딩용 메서드
    public void OnSaveProfileButtonPressed()
    {
        if (nameInputField != null && !string.IsNullOrEmpty(nameInputField.text))
        {
            profileData.UpdateProfile(nameInputField.text);
            Debug.Log("프로필 데이터가 ScriptableObject에 저장되었습니다: " + profileData.userName);
        }
    }

    // 1번 슬롯에 105번 유닛을 장착하는 예시 메서드
    public void OnSelectUnitExample(int slotIndex, int unitId)
    {
        deckData.SetUnitInSlot(slotIndex, unitId);
        Debug.Log($"{slotIndex}번 슬롯에 {unitId}번 유닛 장착 완료.");
    }

    // 인게임 스테이지로 이동하는 버튼 이벤트
    public void OnEnterStageButtonPressed()
    {
        // 로비에서 저장된 ScriptableObject는 씬이 바뀌어도 파괴되지 않고 유지됩니다.
        SceneManager.LoadScene("Stage_Test_01");
    }
}