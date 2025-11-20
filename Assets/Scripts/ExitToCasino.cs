using UnityEngine;
using UnityEngine.SceneManagement;

public class ExitToCasino : MonoBehaviour
{
    [SerializeField] string casinoScene = "Casino";

    public void ExitGame()
    {
        SceneManager.LoadScene(casinoScene, LoadSceneMode.Single);
    }
}