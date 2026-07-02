using UnityEngine;
using UnityEngine.UI;
using TMPro; // TextMeshPro 사용을 위한 네임스페이스
using UnityEngine.SceneManagement;

public class LobbyUIManager : MonoBehaviour
{
    [Header("Data Profile")]
    public UserProfileData profileData; // 생성해둔 ScriptableObject 에셋 할당

    [Header("UI Elements - Top Panel")]
    public Image portraitImage;
    public TMP_Text nameText;
    public TMP_Text levelText;

    void Start()
    {
        // 씬이 시작될 때 ScriptableObject의 데이터를 UI에 반영합니다.
        UpdateProfileUI();
    }

    private void UpdateProfileUI()
    {
        if (profileData != null)
        {
            nameText.text = profileData.userName;
            levelText.text = $"Lv. {profileData.userLevel}";
            
            if (profileData.userPortrait != null)
            {
                portraitImage.sprite = profileData.userPortrait;
            }
        }
    }

    // [유닛 정비하기] 버튼의 OnClick 이벤트에 연결할 메서드
    public void OnUnitSetupButtonClicked()
    {
        Debug.Log("유닛 정비 패널을 엽니다.");
        // 예: unitSetupPanel.SetActive(true);
    }

    // [스테이지 진입] 버튼의 OnClick 이벤트에 연결할 메서드
    public void OnEnterStageButtonClicked()
    {
        Debug.Log("스테이지 씬으로 전환합니다.");
        SceneManager.LoadScene("Stage_Test_01"); // 실제 인게임 씬 이름으로 변경
    }
}