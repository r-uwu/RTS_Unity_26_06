using UnityEngine;
using UnityEngine.SceneManagement;

public class LobbyController : MonoBehaviour
{
    public void GoToStage()
    {
        // Execute transition to the designated stage scene
        SceneManager.LoadScene("Stage_Test_01");
    }
}