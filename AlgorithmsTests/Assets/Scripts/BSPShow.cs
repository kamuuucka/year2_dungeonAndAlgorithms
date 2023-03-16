using System;
using UnityEngine;

public class BSPShow : MonoBehaviour
{
    [Range (1, 400)]
    public int widthRange;
    public bool shouldDebugDrawBsp;

    private BspTree tree;

    void Start()
    {
        RectInt rect = new RectInt(0, 0, widthRange, widthRange);
        tree = BspTree.Split(4, rect);
    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.Return))
        {
            RectInt rect = new RectInt(0, 0, widthRange, widthRange);
            tree = BspTree.Split(4, rect);
        }
    }

    public void DebugDrawBsp()
    {
        if (tree == null) return; 

        DebugDrawBspNode(tree); 
    }

    public void DebugDrawBspNode(BspTree node)
    {
        // Container
        Gizmos.color = Color.green;
        // top      
        Gizmos.DrawLine(new Vector3(node.container.x, node.container.y, 0), new Vector3Int(node.container.xMax, node.container.y, 0));
        // right
        Gizmos.DrawLine(new Vector3(node.container.xMax, node.container.y, 0), new Vector3Int(node.container.xMax, node.container.yMax, 0));
        // bottom
        Gizmos.DrawLine(new Vector3(node.container.x, node.container.yMax, 0), new Vector3Int(node.container.xMax, node.container.yMax, 0));
        // left
        Gizmos.DrawLine(new Vector3(node.container.x, node.container.y, 0), new Vector3Int(node.container.x, node.container.yMax, 0));

        // children
        if (node.left != null) DebugDrawBspNode(node.left);
        if (node.right != null) DebugDrawBspNode(node.right);
    }

    private void OnDrawGizmos()
    {
        if (shouldDebugDrawBsp)
        {
            DebugDrawBsp();
        }
    }
}