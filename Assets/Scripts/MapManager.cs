using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class MapManager : MonoBehaviour
{

    private MapNode root;

    private List<MapNode> listNodes = new List<MapNode>();

    [SerializeField]
    private ARAnchorManager anchorManager;

    [SerializeField]
    private GameObject anchorPrefab;

    [SerializeField]
    private GameObject branchPrefab;

    [SerializeField]
    private GameObject destinationPrefab;

    public Dictionary<string, MapNode> destinations = new Dictionary<string, MapNode>();

    private int curParentIndex;

    public bool IsInitialized()
    {
        return listNodes.Count > 0;
    }

    public void Initialize(Vector3 position)
    {
        if (listNodes.Count != 0)
            return;
        ARDebugManager.Instance.LogInfo("Map Manager Initialized");
        
        GameObject temp = new GameObject("temp");
        root = temp.AddComponent<MapNode>();
        root.InitMapNode(position, false, null, true);
        listNodes.Add(root);
        root.anchor = anchorManager.AddAnchor(new Pose(position, Quaternion.identity));
        root.anchorVisual = Instantiate(anchorPrefab, position, Quaternion.identity);
        curParentIndex = 0;
    }

    public int GetNearestNode(Vector3 position, out double distance)
    {
        int closestIndex = -1;
        double closestDistance = Mathf.Infinity;
        for (int i = 0; i < listNodes.Count; i++)
        {
            if (Vector3.Distance(listNodes[i].position, position) < closestDistance)
            {
                closestIndex = i;
                closestDistance = Vector3.Distance(listNodes[i].position, position);
            }
        }
        distance = closestDistance;
        return closestIndex;
    }

    public MapNode GetCurParent()
    {
        return listNodes[curParentIndex];
    }

    public void SetCurParent(int index)
    {
        curParentIndex = index;
        listNodes[index].branchVisual = Instantiate(branchPrefab, listNodes[index].position, Quaternion.identity);
    }

    public void CreateNewNode(Vector3 position, bool isDestination, MapNode parent, string name)
    {
        GameObject temp = new GameObject("temp");
        MapNode newNode = temp.AddComponent<MapNode>();
        newNode.InitMapNode(position, isDestination, parent, false);
        listNodes.Add(newNode);
        newNode.anchor = anchorManager.AddAnchor(new Pose(position, Quaternion.identity));
        curParentIndex = listNodes.Count - 1;

        if (isDestination)
        {
            destinations.Add(name, newNode);
            newNode.destinationVisual = Instantiate(destinationPrefab, newNode.position, Quaternion.identity);
        }
        else
        {
            newNode.anchorVisual = Instantiate(anchorPrefab, newNode.position, Quaternion.identity);
        }
    }

    public bool CheckIfDestinationExists(string name)
    {
        foreach (var key in destinations.Keys)
        {
            if (name == key)
                return true;
        }
        return false;
    }

    public void StopVisual()
    {
        foreach(var node in listNodes)
        {
            node.StopVisual();
        }
    }

    public List<string> GetDestinations()
    {
        return new List<string>(destinations.Keys);
    }

    public MapNode GetMapNode(int index)
    {
        return listNodes[index];
    }

    public void Clear()
    {
        listNodes.Clear();
        curParentIndex = -1;
        ARDebugManager.Instance.LogInfo("Map Manager Cleared");
    }

}
