using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ClientManager : MonoBehaviour
{
    public static ClientManager Instance;

    private void Start()
    {
        Instance = this;
        LoadScene("testing menu");
    }

    public void LoadScene(string scene)
    {
        SceneManager.LoadScene(scene, LoadSceneMode.Additive);
    }
    public void LoadScene(string scene, params string[] closeScenes)
    {
        LoadScene(scene);

        foreach (string sc in closeScenes)
        {
            CloseScene(sc);
        }
    }

    public void CloseScene(string scene)
    {
        SceneManager.UnloadSceneAsync(scene);
    }

    public void CloseGame()
    {
        Application.Quit();
    }
}
