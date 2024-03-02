using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class MapNode
{
    public MapNode left;
    public MapNode right;
    public MapNode parent;
    public RectInt rect;
    public RectInt roomRect;
    public MapNode(RectInt _rect)
    {
        this.rect = _rect;
    }
}
