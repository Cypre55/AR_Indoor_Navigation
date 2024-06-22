using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class MapNode : MonoBehaviour
{
    public Vector3 position;
    public bool isDestination;
    public bool isRoot;
    public ARAnchor anchor;

    public MapNode parent;
    public LineRenderer lineToParent;
    public GameObject anchorVisual;
    public GameObject branchVisual;
    public GameObject destinationVisual;
    public List<MapNode> children = new List<MapNode>();
    public bool DrawLine { get; set; }

    public void InitMapNode(Vector3 position_, bool isDestination_, MapNode parent_, bool isRoot_)
    {
        position = position_;
        isDestination = isDestination_;
        isRoot = isRoot_;

        DrawLine = true;

        if (!isRoot_)
        {
            parent_.children.Add(this);
            children.Add(parent_);

            List<Vector3> pos = new List<Vector3>();
            pos.Add(position_);
            pos.Add(parent_.position);

            GameObject temp = new GameObject("Temp");

            lineToParent = temp.AddComponent<LineRenderer>();
            lineToParent.material = new Material(Shader.Find("Legacy Shaders/Particles/Alpha Blended Premultiply"));
            
            lineToParent.startColor = Color.red;
            lineToParent.endColor = Color.red;
            lineToParent.startWidth = 0.01f;
            lineToParent.endWidth = 0.01f;
            lineToParent.SetPositions(pos.ToArray());
            lineToParent.useWorldSpace = true;

        }

        ARDebugManager.Instance.LogInfo("MapNode Created");
    }

    public void StopVisual()
    {
        if (anchorVisual != null)
            anchorVisual.gameObject.SetActive(false);
        if (destinationVisual != null)
            destinationVisual.gameObject.SetActive(false);
        if (branchVisual != null)
            branchVisual.gameObject.SetActive(false);
        if (lineToParent != null)
            lineToParent.gameObject.SetActive(false);
    }

    public void Update()
    {
    }
}
