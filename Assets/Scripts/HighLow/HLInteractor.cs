using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class HLInteractor : MonoBehaviour
{
    [SerializeField] string highLowSceneName = "HighLow";
    [SerializeField] GameObject promptUI;                     // assign your “Press E” UI
    [SerializeField] MonoBehaviour playerMoveScriptToDisable; // e.g., PlayerMovementNew
    [SerializeField] string playerTag = "Player";

    bool playerInRange = false;
    GameObject playerRef;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag(playerTag)) return;
        playerInRange = true;
        playerRef = other.gameObject;
        if (promptUI) promptUI.SetActive(true);
        Debug.Log("Entered High-Low interact tile");
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag(playerTag)) return;
        playerInRange = false;
        playerRef = null;
        if (promptUI) promptUI.SetActive(false);
        Debug.Log("Exited High-Low interact tile");
    }

    void Update()
    {
        if (!playerInRange) return;

        if (Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
        {
            if (playerMoveScriptToDisable) playerMoveScriptToDisable.enabled = false;

            // optional guard to avoid build-profile mistakes
            if (!Application.CanStreamedLevelBeLoaded(highLowSceneName))
            {
                Debug.LogError($"Scene '{highLowSceneName}' is not in the active Build Profile.");
                return;
            }

            SceneManager.LoadScene(highLowSceneName, LoadSceneMode.Single);
        }
    }
}