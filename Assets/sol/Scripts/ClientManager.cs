using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ClientManager : MonoBehaviour
{
    public static ClientManager Instance;
    public GameObject cameraObject;
    private Vector3 cameraDefaultPos;

    private bool gameRunning = false;

    private void Start()
    {
        Instance = this;
        cameraDefaultPos = cameraObject.transform.position;
        LoadScene("testing menu");
    }

    private void Update()
    {
        if (gameRunning && PlayerMovement.Instance != null)
        {
            UpdateCamera(false, PlayerMovement.Instance.transform.position);
        } else
        {
            cameraObject.transform.position = cameraDefaultPos;
        }
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

    public void CloseClient()
    {
        Application.Quit();
    }

    public void UpdateCamera(bool intense, params Vector3[] playersPos)
    {
        // Update Camera Position
        Vector3 average = Vector3.zero;

        foreach (Vector3 player in playersPos)
        {
            average += player;
        }

        average /= playersPos.Length;
        average.z = cameraObject.transform.position.z;

        cameraObject.transform.position = average;

        // Update Camera Zoom
    }

    public void GameStarted()
    {
        gameRunning = true;
    }
    public void GameFinished()
    {
        gameRunning = false;
    }
}
