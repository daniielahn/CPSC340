using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class PauseSystem : MonoBehaviour
{
    [Header("Setup")]
    [SerializeField] GameObject pauseMenu;      // drag the Pause Canvas here
    [SerializeField] string startScene = "Start";

    bool isPaused = false;

    void Start()
    {
        if (pauseMenu)
            pauseMenu.SetActive(false);
        else
            Debug.LogError("PauseSystem: pauseMenu reference not set in Inspector.");
    }
    
    void Update()
    {
        if (Keyboard.current == null)
        {
            return;
        }

        // Specifically Esc:
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            TogglePause();
        }
    }


    public void TogglePause()
    {
        // flip pause
        isPaused = !isPaused;
        Time.timeScale = isPaused ? 0f : 1f;
        AudioListener.pause = isPaused;

        if (pauseMenu)
            pauseMenu.SetActive(isPaused);
    }

    // UI button hooks
    public void UI_OnResume()
    {
        if (isPaused) TogglePause();
    }

    public void UI_OnMainMenu()
    {
        // unpause before leaving
        Time.timeScale = 1f;
        AudioListener.pause = false;
        SceneManager.LoadScene(startScene, LoadSceneMode.Single);
    }

    public void UI_OnQuit()
    {
        Time.timeScale = 1f;
        AudioListener.pause = false;
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}