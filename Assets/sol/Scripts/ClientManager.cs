using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ClientManager : MonoBehaviour
{
    public static ClientManager Instance;
    public GameObject cameraObject;

    private Vector3 cameraDefaultPos;
    private List<GameObject> cameraTrackers = new List<GameObject>();
    public float trackingDistance;
    [Tooltip("Time it takes to snap to camera position (in seconds??)")] public float trackingSpeed;

    public bool gameRunning = false;

    // TODO - player cosmetics storage

    private void Start()
    {
        Instance = this;
        cameraDefaultPos = cameraObject.transform.position;
        LoadScene("testing menu");
    }

    private void Update()
    {
        if (gameRunning)
        {
            // Clear null trackers
            ClearEmptyTrackers();

            // Change tracking depending on tracker visibility
            if (TrackersVisible(PlayerMovement.Instance.gameObject, cameraTrackers))
            {
                // Follow all trackers
                GameObject[] trackers = cameraTrackers.ToArray();
                UpdateCamera(false, trackers);
            } else
            {
                // Follow player only
                UpdateCamera(false, PlayerMovement.Instance.gameObject.transform.position);
            }
        } else
        {
            // Return to default camera position (for menus)
            cameraObject.transform.position = Vector3.Lerp(cameraObject.transform.position, cameraDefaultPos, trackingSpeed * Time.fixedDeltaTime);
        }
    }

    // Load specific Scene
    public void LoadScene(string scene)
    {
        SceneManager.LoadScene(scene, LoadSceneMode.Additive);
    }
    // Load specific scene and close all specified scenes
    public void LoadScene(string scene, params string[] closeScenes)
    {
        LoadScene(scene);

        foreach (string sc in closeScenes)
        {
            CloseScene(sc);
        }
    }

    // Close specified scene
    public void CloseScene(string scene)
    {
        SceneManager.UnloadSceneAsync(scene);
    }

    // Close client
    public void CloseClient()
    {
        Application.Quit();
    }

    // Update camera position
    private void UpdateCamera(bool intense, params Vector3[] trackersPos)
    {
        // Get average tracker location
        Vector3 average = Vector3.zero;
        foreach (Vector3 player in trackersPos)
        {
            average += player;
        }

        average /= trackersPos.Length;
        average.z = cameraObject.transform.position.z; // dont need to change position.z from tracker positions

        // Lerp to new averaged location
        cameraObject.transform.position = Vector3.Lerp(cameraObject.transform.position, average, trackingSpeed * Time.fixedDeltaTime);

        // TODO - Update Camera Zoom
    }

    // Update camera position with GameObject[] input
    private void UpdateCamera(bool intense, params GameObject[] trackers)
    {
        // Convert GameObject array to transform.position array
        Vector3[] trackersPos = new Vector3[trackers.Length];

        int i = 0;
        foreach (GameObject tracker in trackers)
        {
            trackersPos[i] = tracker.transform.position;
            i++;
        }

        // Update camera with new transform.position array
        UpdateCamera(intense, trackersPos);
    }

    // Get trackers visibility
    // based off of trackingDistance
    private bool TrackersVisible(GameObject localTracker, List<GameObject> trackers)
    {
        bool playersVisible = false;
        foreach (GameObject tracker in trackers)
        {
            if (tracker != localTracker && Vector3.Distance(localTracker.transform.position, tracker.transform.position) < trackingDistance)
            {
                playersVisible = true;
            }
        }

        return playersVisible;
    }

    // Clear all instances of null in cameraTrackers
    private void ClearEmptyTrackers()
    {
        foreach (GameObject tracker in cameraTrackers)
        {
            if (tracker == null)
            {
                cameraTrackers.Remove(tracker);
            }
        }
    }

    public void GameStarted()
    {
        gameRunning = true;
        cameraTrackers = new List<GameObject>();
    }
    public void GameFinished()
    {
        gameRunning = false;
    }

    public void AddCameraTracker(GameObject tracker)
    {
        cameraTrackers.Add(tracker);
    }
}
