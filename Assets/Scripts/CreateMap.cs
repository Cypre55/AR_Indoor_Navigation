using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.ARFoundation;
using TMPro;

public class CreateMap : MonoBehaviour
{
    [SerializeField]
    private Camera arCamera = null;

    [SerializeField]
    private UnityEvent OnStart = null;

    [SerializeField]
    private UnityEvent OnStop = null;

    [SerializeField]
    private TextMeshProUGUI destinationTextInput = null;

    public bool CanCreateMap { get; set; }
    private Vector3 lastPosition;

    private MapManager mapManager;

    void Start()
    {
        ARDebugManager.Instance.LogInfo("Create Map Started");
        mapManager = GetComponent<MapManager>();
        CanCreateMap = false;
    }

    public void CanStart()
    {
        if (!mapManager.IsInitialized())
        {
            CanCreateMap = true;
            OnStart?.Invoke();
            return;
        }

        double distance;
        int closestIndex = mapManager.GetNearestNode(arCamera.transform.position, out distance);

        if (distance > 0.3)
        {
            ARDebugManager.Instance.LogInfo("[New Start] Too Far from Node");
            return;
        }

        mapManager.SetCurParent(closestIndex);

        CanCreateMap = true;
        OnStart?.Invoke();
    }

    public void AddDestination()
    {
        if (!mapManager.IsInitialized())
        {
            ARDebugManager.Instance.LogInfo("Map Manager not Initialized");
            return;
        }

        double distance;
        Vector3 centrePosition = arCamera.transform.position;

        int closestIndex = mapManager.GetNearestNode(centrePosition, out distance);

        if (distance > 0.8)
        {
            ARDebugManager.Instance.LogInfo("[Add Dest] Too Far from Node");
            return;
        }

        string name = destinationTextInput.text;

        if (name == "")
        {
            ARDebugManager.Instance.LogInfo("Please Enter a Destination Name");
            return;
        }

        if (mapManager.CheckIfDestinationExists(name))
        {
            ARDebugManager.Instance.LogInfo("Destination Name Already Used");
            return;
        }

        MapNode parent = mapManager.GetMapNode(closestIndex);
        mapManager.CreateNewNode(centrePosition, true, parent, name);
    }

    public void Stop()
    {
        CanCreateMap = false;
        OnStop?.Invoke();
    }

    void Update()
    {
        if (CanCreateMap)
        {
            if (!mapManager.IsInitialized())
            {
                mapManager.Initialize(arCamera.transform.position);
            }

            MapNode parent = mapManager.GetCurParent();

            if (Vector3.Distance(arCamera.transform.position, parent.position) >= 1)
            {
                mapManager.CreateNewNode(arCamera.transform.position, false, parent, "");
            }
        }
        
    }
}
