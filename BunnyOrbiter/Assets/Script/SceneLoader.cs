using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public void LoadScene(string sceneName)
    {
        // Always uncomment this after testing!
        // SceneManager.LoadScene(sceneName); 
        
        // TEMPORARY: Log to verify it works
        Debug.Log($"Attempting to load: {sceneName}");
    }

    public void QuitGame()
    {
        Application.Quit();
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
}