using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public GameObject winPanel;
    public GameObject losePanel;
    
    private bool isGameOver = false;

    private void Start()
    {
        if (winPanel != null) winPanel.SetActive(false);
        if (losePanel != null) losePanel.SetActive(false);
        
        // 초기 맵 배치 시간 확보를 위해 2초 후부터 감시 시작
        StartCoroutine(CheckGameStateRoutine());
    }

    private IEnumerator CheckGameStateRoutine()
    {
        yield return new WaitForSeconds(2f);

        while (!isGameOver)
        {
            yield return new WaitForSeconds(1f); // 1초 주기로 연산 최적화

            int playerTowers = 0;
            int enemyTowers = 0;

            foreach (Tower tower in Tower.AllTowers)
            {
                if (tower.currentTeam == Team.Player) playerTowers++;
                else if (tower.currentTeam == Team.EnemyBlue || tower.currentTeam == Team.EnemyYellow) enemyTowers++;
            }

            // 아군 타워가 하나도 없으면 패배
            if (playerTowers == 0)
            {
                GameOver(false);
            }
            // 적군 타워가 하나도 없으면 승리
            else if (enemyTowers == 0)
            {
                GameOver(true);
            }
        }
    }

    private void GameOver(bool isWin)
    {
        isGameOver = true;
        Time.timeScale = 0f; // 모든 게임 내 물리 및 로직 일시 정지
        
        if (isWin && winPanel != null) winPanel.SetActive(true);
        else if (!isWin && losePanel != null) losePanel.SetActive(true);
    }
}