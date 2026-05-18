using System.Collections.Generic;
using UnityEngine;

public class PathNode : MonoBehaviour
{
    public List<PathNode> incoming = new();
    public List<PathNode> outgoing = new();
    
    public PathNodeCost costData;
    public PlayerBase attachedBase;

    public Vector3 Position => transform.position;
}
