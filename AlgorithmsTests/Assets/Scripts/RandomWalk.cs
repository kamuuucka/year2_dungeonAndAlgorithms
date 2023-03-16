using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Self-Avoiding Random Walk algorithm
public class RandomWalk : MonoBehaviour 
{
    //How many steps do we want to take before we stop?
    public int stepsToTake;
    private int _stepsTaken;
    private int _randomDirPos;
    
    [SerializeField]
    private List<Vector3> _visitedNodes = new List<Vector3>();
    [SerializeField]
    private List<Vector3> _randomWalkPositions =  new List<Vector3>();
    
    readonly List<Vector3> _allPossibleDirections = new List<Vector3> {
            new Vector3(0f, 1f, 0f),
            new Vector3(0f, -1f, 0f),
            new Vector3(-1f, 0f, 0f),
            new Vector3(1f, 0f, 0f)
            };

    void Update() 
	{
        if (Input.GetKeyDown(KeyCode.Return))
        {
            _randomWalkPositions.Clear();
            _visitedNodes.Clear();
            _randomWalkPositions = GenerateSelfAvoidingRandomWalk();

            //Debug.Log(randomWalkPositions.Count);
        }

        //Display the path with lines
        if (_randomWalkPositions != null && _randomWalkPositions.Count > 1)
        {
            for (int i = 1; i < _randomWalkPositions.Count; i++)
            {
                //Only visible in editor
                Debug.DrawLine(_randomWalkPositions[i - 1], _randomWalkPositions[i]);
            }
        }
    }

    public List<Vector3> GenerateSelfAvoidingRandomWalk()
    {
        Vector3 startPos = Vector3.zero;

        WalkNode currentNode = new WalkNode(startPos, null, new List<Vector3>(_allPossibleDirections));
        
        _stepsTaken = 0;

        //If we don't want to visid the same node twice
        _visitedNodes.Add(startPos);

        while (true)
        {
            if (_stepsTaken == stepsToTake)
            {
                break;
            }

            currentNode = CurrentNode(currentNode);

            _randomDirPos = Random.Range(0, currentNode.possibleDirections.Count);  //Both values are inclusive
            Debug.Log($"Direction: {currentNode.possibleDirections.Count}");
            Vector3 randomDir = currentNode.possibleDirections[_randomDirPos];

            //Remove this direction from the list of possible directions we can take from this node - TODO: ASK
            currentNode.possibleDirections.RemoveAt(_randomDirPos);

            //Whats the position after we take a step in this direction
            Vector3 nextNodePos = currentNode.pos + randomDir;

            //Have we visited this position before?
            if (!HasVisitedNode(nextNodePos, _visitedNodes))
            {
                //Walk to this node
                currentNode = new WalkNode(nextNodePos, currentNode, new List<Vector3>(_allPossibleDirections));

                _visitedNodes.Add(nextNodePos);

                _stepsTaken += 1;
           }
        }

        //Generate the final path
        List<Vector3> randomWalkPositions = new List<Vector3>();

        while (currentNode.previousNode != null)
        {
            randomWalkPositions.Add(currentNode.pos);

            currentNode = currentNode.previousNode;
        }

        randomWalkPositions.Add(currentNode.pos);

        //Reverse the list so it begins at the step we started from 
        randomWalkPositions.Reverse();

        return randomWalkPositions;
    }

    private WalkNode CurrentNode(WalkNode currentNode)
    {
        //Need to backtrack if we cant move in any direction from the current node - TODO: ASK because idk exactly why
        while (currentNode.possibleDirections.Count == 0)
        {
            currentNode = currentNode.previousNode;
            _stepsTaken -= 1;
        }

        return currentNode;
    }

    //Checks if a position is in a list of positions
    private bool HasVisitedNode(Vector3 pos, List<Vector3> listPos)
    {
        bool hasVisited = false;

        for (int i = 0; i < listPos.Count; i++)
        {
            float distSqr = Vector3.SqrMagnitude(pos - listPos[i]);

            //Cant compare exactly because of floating point precisions
            if (distSqr < 0.001f)
            {
                hasVisited = true;

                break;
            }
        }

        return hasVisited;
    }

    //Help class to keep track of the steps
    private class WalkNode
    {
        //The position of this node in the world
        public Vector3 pos;

        public WalkNode previousNode;

        //Which steps can we take from this node?
        public List<Vector3> possibleDirections;

        public WalkNode(Vector3 pos, WalkNode previousNode, List<Vector3> possibleDirections)
        {
            this.pos = pos;
            Debug.Log(pos.ToString());
            this.previousNode = previousNode;

            this.possibleDirections = possibleDirections;
        }
    }
    
}