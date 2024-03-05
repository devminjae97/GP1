using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapNodeTest
{
    public bool isLinked;
    public RectInt rect;
    public RectInt roomRect;
    public MapNodeTest( RectInt _rect )
    {
        this.rect = _rect;
    }
}
