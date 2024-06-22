using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.ARFoundation;

public class PlaneDetector : MonoBehaviour
{
    [SerializeField]
    private UnityEvent OnInitialized = null;

    private ARPlaneManager arPlaneManager = null;

    private bool Initialized { get; set; }


    void Awake()
    {
        arPlaneManager = GetComponent<ARPlaneManager>();
        arPlaneManager.planesChanged += PlanesChanged;
    }

    void PlanesChanged(ARPlanesChangedEventArgs args)
    {
        if (!Initialized)
        {
            Activate();
        }
    }

    private void Activate()
    {
        ARDebugManager.Instance.LogInfo("Activate Experience");
        Initialized = true;
        arPlaneManager.enabled = false;
        OnInitialized?.Invoke();
    }

    public void Restart()
    {
        ARDebugManager.Instance.LogInfo("Activate Experience");
        Initialized = false;
        arPlaneManager.enabled = true;
    }
}
