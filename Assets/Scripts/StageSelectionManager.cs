using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;
using UnityEngine.SceneManagement;

public class StageSelectionManager : MonoBehaviour
{
    [Header("Data References")]
    public List<StageData> allStages; // 전체 스테이지 ScriptableObject 리스트

    [Header("UI References")]
    public Transform stageGridParent; // 세로 정렬을 위한 Grid/Vertical Layout Group 패널
    public GameObject stageButtonPrefab; // 동적 생성할 스테이지 버튼 프리팹

    private const string TargetStageKey = "HighestClearedStage";

    void Start()
    {
        InitializeStageGridUI();
    }

    private void InitializeStageGridUI()
    {
        // 기기에 저장된 플레이어의 최고 클리어 스테이지 획득 (기본값 0: 1스테이지가 열린 상태)
        int highestClearedStage = PlayerPrefs.GetInt(TargetStageKey, 0);

        foreach (StageData stage in allStages)
        {
            GameObject btnObj = Instantiate(stageButtonPrefab, stageGridParent);
            
            // 프리팹 내부의 컴포넌트 추출
            Button btn = btnObj.GetComponent<Button>();
            TMP_Text stageText = btnObj.GetComponentInChildren<TMP_Text>();
            
            // 텍스트 설정
            if (stageText != null)
            {
                stageText.text = $"{stage.stageIndex}. {stage.stageName}";
            }

            // 해금 조건 검증: 1스테이지(index 1)는 항상 열려있음 (highestClearedStage + 1 관리)
            bool isUnlocked = stage.stageIndex <= (highestClearedStage + 1);

            if (isUnlocked)
            {
                btn.interactable = true;
                string targetScene = stage.sceneName;
                
                // 람다식 캡처 이슈 방지를 위해 로컬 변수 사용 후 이벤트 바인딩
                btn.onClick.AddListener(() => OnStageButtonClicked(targetScene));
            }
            else
            {
                // 잠긴 스테이지 시각 처리
                btn.interactable = false;
                Image btnImage = btnObj.GetComponent<Image>();
                if (btnImage != null)
                {
                    btnImage.color = Color.gray; // 비활성화 시각 표기
                }
            }
        }
    }

    private void OnStageButtonClicked(string sceneName)
    {
        Debug.Log($"{sceneName} 스테이지로 진입합니다.");
        SceneManager.LoadScene(sceneName);
    }

    // 개발 중 테스트 및 데이터 초기화를 위한 치트 메서드
    public void ResetStageProgress()
    {
        PlayerPrefs.SetInt(TargetStageKey, 0);
        PlayerPrefs.Save();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name); // 씬 재로드로 UI 새로고침
    }
}