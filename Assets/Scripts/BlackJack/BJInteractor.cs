using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;


public class BJInteractor : MonoBehaviour
{
    [SerializeField] string blackjackSceneName = "Blackjack";
    [SerializeField] GameObject promptUI;                   // assign your “Press E” UI
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
        Debug.Log("Entered interact tile");
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag(playerTag)) return;
        playerInRange = false;
        playerRef = null;
        if (promptUI) promptUI.SetActive(false);
        Debug.Log("Exited interact tile");
    }
    
    void Update() {
        if (!playerInRange) return;

        if (Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame) {
            // optional guard to avoid spammy errors
            if (!Application.CanStreamedLevelBeLoaded("Blackjack")) {
                Debug.LogError($"Scene '{blackjackSceneName}' is not in the active Build Profile.");
                return;
            }
            SceneManager.LoadScene("Blackjack", LoadSceneMode.Single);

        }
    }

}