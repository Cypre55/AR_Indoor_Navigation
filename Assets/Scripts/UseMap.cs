using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UseMap : MonoBehaviour
{
    [SerializeField]
    private Camera arCamera = null;

    private MapManager mapManager;

    [SerializeField]
    private TMP_Dropdown dropdown = null;

    [SerializeField]
    private TMP_Text instruction = null;

    [SerializeField]
    private TMP_Text message = null;

    [SerializeField]
    private GameObject arrowPrefab = null;

    [SerializeField]
    private GameObject destinationPrefab = null;

    private bool checkReachedNode = false;
    private bool searchInitialized = false;

    private MapNode currentNearestNode = null;
    private MapNode destinationNode = null;

    private List<MapNode> path = new List<MapNode>();
    private List<GameObject> arrows = new List<GameObject>();

    public void BeginUse()
    {
        foreach (var key in mapManager.GetDestinations())
        {
            ARDebugManager.Instance.LogInfo($"Destination: {key}");
        }

        dropdown.AddOptions(mapManager.GetDestinations());

        ARDebugManager.Instance.LogInfo("Destinations added");

        checkReachedNode = true;
    }

    void Start()
    {
        mapManager = GetComponent<MapManager>();
        dropdown.ClearOptions();

    }

    public void SetDestination()
    {
        ARDebugManager.Instance.LogInfo(dropdown.options[dropdown.value].text);
        destinationNode = mapManager.destinations[dropdown.options[dropdown.value].text];
        ComputeNewArrows();
    }

    void Update()
    {
        if (checkReachedNode)
        {
            double distance;
            int closestIndex = mapManager.GetNearestNode(arCamera.transform.position, out distance);

            if (distance < 0.3)
            {

                instruction.gameObject.SetActive(false);
                message.gameObject.SetActive(true);
                dropdown.gameObject.SetActive(true);

                // Turn off all Anchors and Lines
                mapManager.StopVisual();

                // Treat the first option as destination
                destinationNode = mapManager.destinations[dropdown.options[dropdown.value].text];

                currentNearestNode = mapManager.GetMapNode(closestIndex);

                searchInitialized = true;
                checkReachedNode = false;

                ComputeNewArrows();
            }
        }

        if (searchInitialized)
        {
            double distance;
            int closestIndex = mapManager.GetNearestNode(arCamera.transform.position, out distance);

            if (distance < Vector3.Distance(arCamera.transform.position, currentNearestNode.position))
            {
                currentNearestNode = mapManager.GetMapNode(closestIndex);
                ComputeNewArrows();
            }
        }


    }

    void ComputeNewArrows()
    {
        foreach (var arrow in arrows)
        {
            Destroy(arrow.gameObject);
        }
        arrows.Clear();

        path.Clear();
        DFS(currentNearestNode, null, destinationNode);

        int numArrows = 4;

        for (int i = 0; i < path.Count - 1 && i < numArrows; i++)
        {
            GameObject newArrow = Instantiate(arrowPrefab, path[i].position, Quaternion.Euler(0f, -90f + Vector3.Angle(path[i+1].position - path[i].position, Vector3.forward), 0f));
            arrows.Add(newArrow);
        }

        if (arrows.Count < numArrows) 
        {
            GameObject dest = Instantiate(destinationPrefab, destinationNode.position, Quaternion.identity);
            arrows.Add(dest);
        }
    }

    bool DFS(MapNode cur, MapNode parent, MapNode dest)
    {
        path.Add(cur);
        if (cur == dest)
        {
            return true;
        }

        foreach (var neigh in cur.children)
        {
            if (neigh != parent)
            {
                bool result = DFS(neigh, cur, dest);
                if (result)
                    return true;
            }
        }

        path.RemoveAt(path.Count - 1);
        return false;
    }
}
