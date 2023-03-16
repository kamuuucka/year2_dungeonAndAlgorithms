using UnityEngine;

public class BspTree
{
    public RectInt container;
    public RectInt room;
    public BspTree left;
    public BspTree right;

    public BspTree(RectInt a)
    {
        container = a;
    }

    internal static BspTree Split(int numberOfOperations, RectInt container)
    {
        var node = new BspTree(container);

        if (numberOfOperations == 0)
        {
            return node;
        }

        var splitedContainer = SplitContainer(container);
        node.left = Split(numberOfOperations - 1, splitedContainer[0]);
        Debug.Log(numberOfOperations);

        node.right = Split(numberOfOperations - 1, splitedContainer[1]);
        Debug.Log(numberOfOperations);

        return node;
    }

    private static RectInt[] SplitContainer(RectInt container)
    {
        RectInt c1, c2;
        bool horizontal = Random.Range(0f, 1f) > 0.5f ? true : false;
        if (horizontal)
        {
            c1 = new RectInt(container.x, container.y, (int)(container.width * Random.Range(0.3f, 0.6f)),
                container.height);
            c2 = new RectInt(container.x + c1.width, container.y, container.width - c1.width, container.height);
        }
        else
        {
            c1 = new RectInt(container.x, container.y, container.width,
                (int)(container.height * Random.Range(0.3f, 0.6f)));
            c2 = new RectInt(container.x, container.y + c1.height, container.width, container.height - c1.height);
        }

        return new RectInt[] { c1, c2 };
    }
}