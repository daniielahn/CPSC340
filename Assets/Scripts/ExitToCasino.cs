using UnityEngine;
using UnityEngine.SceneManagement;

public class ExitToCasino : MonoBehaviour
{
    [SerializeField] string casinoScene = "SampleScene";

    public void ExitGame()
    {
        SceneManager.LoadScene(casinoScene, LoadSceneMode.Single);
    }
}