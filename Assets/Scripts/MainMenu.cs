using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] string casinoScene = "SampleScene"; // your hub

    public void Play()
    {
        // fresh start – you can also reset Bankroll here if you want
        PlayerPrefs.SetInt("bankroll_cash", 1000);
        PlayerPrefs.Save();
        SceneManager.LoadScene(casinoScene, LoadSceneMode.Single);
    }

    public void ContinueGame()
    {
        // if you save last scene name, load it; otherwise just load casino
        SceneManager.LoadScene(casinoScene, LoadSceneMode.Single);
    }

    public void Quit()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}