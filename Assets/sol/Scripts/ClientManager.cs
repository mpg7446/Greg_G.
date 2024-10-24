using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ClientManager : MonoBehaviour
{
    public static ClientManager Instance;
    public GameObject cameraObject;
    private float defaultFOV;

    [SerializeField] private bool limitFramerate = false;

    // Camera tracking
    private Vector3 cameraDefaultPos;
    private List<GameObject> cameraTrackers = new List<GameObject>();
    private List<Vector3> visibleTrackers = new List<Vector3>();
    public float trackingDistance;
    public int thisPlayerWeight = 2;
    [Tooltip("Time it takes to snap to camera position (not in seconds, im not too sure why)")] public float trackingSpeed;

    public bool gameRunning = false;

    // Player visuals
    public PlayerVisual playerVisual;
    public int playerVariation = -1;
    public bool wonLastRound = false;
    public enum PlayerVisual
    {
        None,
        Racoon,
        Brown,
        Albino,
        Cat,
        PNG,
        Isopod,
        Gnarpy
    }
    public List<PlayerModel> playerModels = new List<PlayerModel>();

    private void Start()
    {
        if (Instance != null)
            Destroy(this);
        Instance = this;
        LoadScene("Menu");

        cameraDefaultPos = cameraObject.transform.position;
        defaultFOV = cameraObject.GetComponent<Camera>().fieldOfView;

        if (limitFramerate)
        {
            QualitySettings.vSyncCount = 0;
            Application.targetFrameRate = 50;
        }
    }

    private void Update()
    {
        if (gameRunning)
        {
            // Clear null trackers
            ClearEmptyTrackers();

            // Change tracking depending on tracker visibility
            if (TrackersVisible(PlayerManager.Instance.gameObject, cameraTrackers, thisPlayerWeight))
            {
                // Follow all trackers
                GameObject[] trackers = cameraTrackers.ToArray();
                UpdateCamera(false, trackers);
            } else
            {
                // Follow player only
                UpdateCamera(false, PlayerManager.Instance.gameObject.transform.position);
            }
        } else
        {
            // Return to default camera position (for menus)
            cameraObject.transform.position = Vector3.Lerp(cameraObject.transform.position, cameraDefaultPos, trackingSpeed * Time.deltaTime);
        }
    }

    #region Visuals
    public void ResetVisuals()
    {
        playerVisual = PlayerVisual.None;
        playerVariation = -1;
    }
    public void SetRandomPlayerVisual(int rolls = 3)
    {
        if (playerVisual == PlayerVisual.None)
        {
            // Roll x amount of dice and select lowest number to choose which 
            System.Random rnd = new System.Random();
            int id = rnd.Next(1, Enum.GetValues(typeof(PlayerVisual)).Length);

            for (int i = 1; i < rolls; i++)
            {
                int compare = rnd.Next(1, Enum.GetValues(typeof(PlayerVisual)).Length);
                if (compare < id)
                {
                    id = compare;
                }
            }

            playerVisual = (PlayerVisual)id;
            Debug.Log($"ClientManager: Set Random Player Visual ({playerVisual})");
        }
        if (playerVariation <= 0)
        {
            SetRandomPlayerVariation();
        }

    }

    public void SetRandomPlayerVariation()
    {
        List<PlayerModel> matches = new List<PlayerModel>();

        // Get all matching PlayerModels
        foreach (PlayerModel model in playerModels)
        {
            if (model.playerVisual == playerVisual)
            {
                matches.Add(model);
            }
        }

        playerVariation = UnityEngine.Random.Range(0, matches.Count - 1);
        Debug.Log($"ClientManager: Set Random Player Varation ({playerVariation})");
    }

    public void SetNextPlayerVariation()
    {

    }
    #endregion

    #region Scene Management
    // Load specific Scene
    public void LoadScene(string scene)
    {
        if (SceneManager.GetSceneByName(scene).name == null)
            SceneManager.LoadScene(scene, LoadSceneMode.Additive);
        else
            Debug.LogWarning($"ClientManaher.LoadScene: Unable to find scene by name \"{scene}\"");
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
    #endregion

    #region Camera Position
    // Update camera position
    private void UpdateCamera(bool intense, params Vector3[] trackersPos)
    {
        // Get average tracker location
        Vector3 average = Vector3.zero;
        foreach (Vector3 player in trackersPos)
        {
            average += player;
            //Debug.Log($"UpdateCamera: added {player} to average ({average})");
        }

        average /= trackersPos.Length;
        //Debug.Log($"UpdateCamera: average averaged | {average}");
        average.z = cameraObject.transform.position.z; // dont need to change position.z from tracker positions

        // Lerp to new averaged location
        cameraObject.transform.position = Vector3.Lerp(cameraObject.transform.position, average, trackingSpeed * Time.deltaTime);

        // TODO - Update Camera Zoom

        //float newFOV = defaultFOV;

        //if (intense)
        //{
        //    newFOV *= 1.1f;
        //}

        //cameraObject.GetComponent<Camera>().fieldOfView = newFOV;
    }

    // Update camera position with GameObject[] input
    private void UpdateCamera(bool intense, params GameObject[] trackers)
    {
        // Convert GameObject array to transform.position array
        //Vector3[] trackersPos = new Vector3[trackers.Length];

        //int i = 0;
        //foreach (GameObject tracker in trackers)
        //{
        //    trackersPos[i] = tracker.transform.position;
        //    i++;
        //}

        // Update camera with new transform.position array
        UpdateCamera(intense, visibleTrackers.ToArray());
    }
#endregion

    #region Camera Trackers
    // Get trackers visibility
    // based off of trackingDistance
    private bool TrackersVisible(GameObject localTracker, List<GameObject> trackers, int localWeight = 2)
    {
        bool visible = false;
        for (visibleTrackers.Clear(); visibleTrackers.Count < localWeight; visibleTrackers.Add(localTracker.transform.position))

        foreach (GameObject tracker in trackers)
        {
            if (tracker != localTracker && Vector3.Distance(localTracker.transform.position, tracker.transform.position) < trackingDistance)
            {
                visible = true;
                visibleTrackers.Add(tracker.transform.position);
            }
        }

        return visible;
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

    public void AddCameraTracker(GameObject tracker)
    {
        cameraTrackers.Add(tracker);
        Debug.Log($"ClientManager: successfully added tracker (Player {tracker.GetComponent<PlayerID>().GetID()})");
    }
#endregion

    public void GameStarted()
    {
        gameRunning = true;
        cameraTrackers = new List<GameObject>();
    }
    public void GameFinished()
    {
        gameRunning = false;
    }

    // Close client
    public void CloseClient()
    {
        Application.Quit();
    }
}
