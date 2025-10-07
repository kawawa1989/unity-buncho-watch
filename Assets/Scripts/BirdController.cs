using System.Collections;
using BunchoWatch;
using UnityEngine;
using Tree = BunchoWatch.Tree;

public class BirdController : MonoBehaviour
{
    [SerializeField] 
    private Tree[] trees;
    [SerializeField]
    private Transform positionA;
    [SerializeField]
    private Transform positionB;
    [SerializeField] 
    private float height = 400;
    [SerializeField] 
    private float duration = 0.3f;

    
    private const int IdleTime = 3;
    private float _time;
    private BirdAction _currentAction;
    private int _currentTreeIndex;
    private int _positionIndex;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SetTree(0, 0);
    }

    void SetTree(int treeIndex, int position)
    {
        var tree = trees[treeIndex];
        transform.position = tree.GetPosition(position);
        _currentTreeIndex = treeIndex;
        _positionIndex = position;
    }

    // Update is called once per frame
    void Update()
    {
        if (_time > IdleTime && _currentAction == null)
        {
            _currentAction = DecideNextAction();
            StartCoroutine(_currentAction.Action());
            _time = 0;
        }

        if (_currentAction is { IsRunning: false })
        {
            _currentAction = null;
        }
        
        _time += Time.deltaTime;
    }

    BirdAction DecideNextAction()
    {
        int action = 0;
        int rnd = Random.Range(0, 100);
        if (rnd is >= 0 and < 75)
        {
            action = 0;
        }
        else if (rnd is >= 75 and < 85)
        {
            action = 1;
        }
        else
        {
            action = 2;
        }

        switch (action)
        {
            case 0:
            {
                var nextPosition = trees[_currentTreeIndex].Pick(_positionIndex);
                _positionIndex = nextPosition;
                return new Walk(transform, trees[_currentTreeIndex].GetPosition(nextPosition));
            }
            case 1:
            {
                var nextTreeIndex = _currentTreeIndex == 0 ? 1 : 0;
                var nextTree = trees[nextTreeIndex];
                var jumpTo = nextTree.PickNearest(_positionIndex);
                _currentTreeIndex = nextTreeIndex;
                _positionIndex = jumpTo;
                return new Jump(transform, nextTree.GetPosition(jumpTo));
            }
            case 2:
                return new Sleep();
        }

        return new Idle();
    }
}
