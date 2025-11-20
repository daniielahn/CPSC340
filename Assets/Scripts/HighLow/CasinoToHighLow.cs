using UnityEngine;
using UnityEngine.SceneManagement;

public class CasinoToHighLow : MonoBehaviour
{
    [Header("Scene Refs")]
    [SerializeField] MonoBehaviour playerMove;   // your player movement script
    [SerializeField] Camera casinoCamera;        // main casino camera
    [SerializeField] Canvas[] casinoCanvases;    // casino HUDs or prompts

    private const string HIGHLOW_SCENE = "HighLow";  // exact scene name in Build Settings

    // Called when player presses E at the High-Low table
    public void EnterHighLow()
    {
        // disable player + casino UI
        if (playerMove) playerMove.enabled = false;
        if (casinoCamera) casinoCamera.enabled = false;
        foreach (var cv in casinoCanvases)
            if (cv) cv.enabled = false;

        // load the HighLow scene additively
        var op = SceneManager.LoadSceneAsync(HIGHLOW_SCENE, LoadSceneMode.Additive);
        op.completed += _ =>
        {
            var hlScene = SceneManager.GetSceneByName(HIGHLOW_SCENE);
            SceneManager.SetActiveScene(hlScene);

            // enable first camera found in HighLow
            foreach (var root in hlScene.GetRootGameObjects())
            {
                var cam = root.GetComponentInChildren<Camera>(true);
                if (cam)
                {
                    cam.enabled = true;
                    break;
                }
            }
        };
    }

    // Called by Exit button inside High-Low scene
    public void ExitHighLow()
    {
        SceneManager.UnloadSceneAsync(HIGHLOW_SCENE).completed += _ =>
        {
            // re-enable casino environment
            if (casinoCamera) casinoCamera.enabled = true;
            foreach (var cv in casinoCanvases)
                if (cv) cv.enabled = true;
            if (playerMove) playerMove.enabled = true;

            // re-activate casino as main scene
            var casino = SceneManager.GetSceneByName("Casino"); // rename if your casino scene has a different name
            if (casino.IsValid())
                SceneManager.SetActiveScene(casino);
        };
    }
}
