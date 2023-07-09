using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public static SceneLoader Instance { get; private set; }
    public void Awake()
    {
        if (Instance != null) Destroy(this.gameObject);
        DontDestroyOnLoad(this);
        Instance = this;
    }
    public void GoToScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
    }
}
