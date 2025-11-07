using UnityEngine;
using UnityEngine.SceneManagement;

public class CasinoToBlackjack : MonoBehaviour
{
    [Header("Casino refs")]
    [SerializeField] MonoBehaviour playerMove;   // e.g., PlayerMovementNew
    [SerializeField] Camera casinoCamera;        // your main camera
    [SerializeField] Canvas[] casinoCanvases;    // HUD, prompts, etc.

    const string Blackjack = "Blackjack";        // exact scene name

    // call this when player presses E at the table
    public void EnterBlackjack()
    {
        // lock player + hide casino view
        if (playerMove) playerMove.enabled = false;
        if (casinoCamera) casinoCamera.enabled = false;
        foreach (var cv in casinoCanvases) if (cv) cv.enabled = false;

        var op = SceneManager.LoadSceneAsync(Blackjack, LoadSceneMode.Additive);
        op.completed += _ =>
        {
            var bj = SceneManager.GetSceneByName(Blackjack);
            SceneManager.SetActiveScene(bj);           // make Blackjack the active scene
            EnableFirstCameraInScene(bj);              // turn on its camera
        };
    }

    // call this from the Blackjack Exit button
    public void ExitBlackjack()
    {
        SceneManager.UnloadSceneAsync(Blackjack).completed += _ =>
        {
            // restore casino
            if (casinoCamera) casinoCamera.enabled = true;
            foreach (var cv in casinoCanvases) if (cv) cv.enabled = true;
            if (playerMove) playerMove.enabled = true;

            // (optional) set casino active again
            var casino = SceneManager.GetSceneByName("SampleScene");
            if (casino.IsValid()) SceneManager.SetActiveScene(casino);
        };
    }

    static void EnableFirstCameraInScene(Scene s)
    {
        foreach (var root in s.GetRootGameObjects())
        {
            var cam = root.GetComponentInChildren<Camera>(true);
            if (cam) { cam.enabled = true; return; }
        }
        Debug.LogWarning("No camera found in Blackjack scene.");
    }
}
