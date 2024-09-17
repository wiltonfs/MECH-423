using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UILoader : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // Load a new scene by name
    public void LoadScene(string target)
    {
        Debug.Log("Attempting to load: " +  target);
        // Check if the scene exists and load it
        SceneManager.LoadScene(target);
    }

    // Quit the application
    public void Quit()
    {
#if UNITY_EDITOR
        // If running in the Unity editor, stop playing
        UnityEditor.EditorApplication.isPlaying = false;
#else
            // If in a build, quit the application
            Application.Quit();
#endif
    }
}
