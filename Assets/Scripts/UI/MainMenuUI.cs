using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuUI : MonoBehaviour
{
    public void StartGame(bool isAuto)
    {
        PlayerPrefs.SetInt("isAuto", isAuto?1:0);
        SceneManager.LoadScene(1);
    }
}
