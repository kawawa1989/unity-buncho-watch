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
    [SerializeField]
    private Camera mainCamera;

    
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
        // タッチ入力の検出
        HandleTouchInput();
        
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

    /// <summary>
    /// タッチ入力を処理するメソッド
    /// </summary>
    void HandleTouchInput()
    {
        // マウス/タッチ入力の検出
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 screenPosition = Input.mousePosition;
            var nearestTreePosition = GetNearestTreePosition(screenPosition);
            
            // デバッグ用ログ出力
            Debug.Log($"タッチされたスクリーン座標: {screenPosition}");
            Debug.Log($"最も近いTreeのPosition: {nearestTreePosition.position}, PosIndex:{nearestTreePosition.positionIndex}, Tree:{nearestTreePosition.treeIndex}");
        }

        // モバイルデバイスでのタッチ入力
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                Vector2 screenPosition = touch.position;
                var nearestTreePosition = GetNearestTreePosition(screenPosition);
                
                // デバッグ用ログ出力
                Debug.Log($"タッチされたスクリーン座標: {screenPosition}");
                Debug.Log($"最も近いTreeのPosition: {nearestTreePosition.position}, PosIndex:{nearestTreePosition.positionIndex}, Tree:{nearestTreePosition.treeIndex}");
            }
        }
    }

    /// <summary>
    /// スクリーン座標から最も近いTreeのPositionを取得するメソッド
    /// </summary>
    /// <param name="screenPosition">タッチされたスクリーン座標</param>
    /// <returns>最も近いTreeのPosition</returns>
    (Vector3 position, int treeIndex, int positionIndex) GetNearestTreePosition(Vector2 screenPosition)
    {
        if (mainCamera == null)
        {
            Debug.LogError("MainCameraが設定されていません");
            return (Vector3.zero, 0, 0);
        }

        if (trees == null || trees.Length == 0)
        {
            Debug.LogError("Treesが設定されていません");
            return (Vector3.zero, 0, 0);
        }

        // スクリーン座標をワールド座標に変換
        Vector3 nearestPosition = Vector3.zero;
        float nearestDistance = float.MaxValue;
        int resultTreeIndex = 0;
        int resultPositionIndex = 0;
        
        // 全てのTreeの全てのPositionをチェック
        for (int treeIndex = 0; treeIndex < trees.Length; treeIndex++)
        {
            // Treeの全てのポジションをチェック
            for (int position = 0; position < trees[treeIndex].GetPositionCount(); position++)
            {
                Vector3 treePosition = trees[treeIndex].GetPosition(position);
                float distance = Vector2.Distance(screenPosition,
                    new Vector2(treePosition.x, treePosition.y));
                
                if (distance < nearestDistance)
                {
                    nearestDistance = distance;
                    nearestPosition = treePosition;
                    resultTreeIndex = treeIndex;
                    resultPositionIndex = position;
                }
            }
        }

        return (nearestPosition, resultTreeIndex, resultPositionIndex);
    }
}
