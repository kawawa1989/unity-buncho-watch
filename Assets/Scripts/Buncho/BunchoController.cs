using BunchoWatch;
using UnityEngine;

public class BunchoController : MonoBehaviour
{
    [SerializeField]
    private BunchoAnimator animator;
    [SerializeField]
    private FieldController  field;
    
    private const int IdleTime = 3;
    private float _time;
    private BunchoAction _currentAction;
    private int _currentPerchIndex;
    private int _positionIndex;
    private BunchoActionType _previousActionType;
    
    public void Initialize(FieldController fieldController)
    {
        field = fieldController;
        SetPerch(0, 5);
    }

    private void SetPerch(int perchIndex, int position)
    {
        var perch = field.GetPerchAt(perchIndex);
        perch.DumpPositions();
        transform.position = perch.GetPosition(position);
        _currentPerchIndex = perchIndex;
        _positionIndex = position;
    }

    // Update is called once per frame
    private void Update()
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

    BunchoActionType SelectNextAction()
    {
        var action = BunchoActionType.Mochi;
        int rnd = Random.Range(0, 100);
        if (rnd is >= 0 and < 75)
        {
            action = BunchoActionType.Mochi;
        }
        else if (rnd is >= 75 and < 85)
        {
            action = BunchoActionType.Walk;
        }
        else
        {
            action = BunchoActionType.Jump;
        }

        if (_previousActionType == BunchoActionType.Mochi)
        {
            action = BunchoActionType.Walk;
        }

        _previousActionType = action;
        return action;
    }

    BunchoAction DecideNextAction()
    {
        var action = SelectNextAction();
        Debug.Log($"DecideNextAction!! action: {action}");
        switch (action)
        {
            case BunchoActionType.Mochi:
            {
                return new Mochi(animator);
            }
            case BunchoActionType.Walk:
            {
                var perch = field.GetPerchAt(_currentPerchIndex);
                var nextPosition = perch.Pick(_positionIndex);
                perch.DumpPositions();
                _positionIndex = nextPosition;
                return new Walk(animator, transform, perch.GetPosition(nextPosition));
            }
            case BunchoActionType.Jump:
            {
                var nextPerchIndex = _currentPerchIndex == 0 ? 1 : 0;
                var nextPerch = field.GetPerchAt(nextPerchIndex);
                var jumpTo = nextPerch.PickNearest(_positionIndex);
                _currentPerchIndex = nextPerchIndex;
                _positionIndex = jumpTo;
                return new Jump(animator, transform, nextPerch.GetPosition(jumpTo));
            }
            case BunchoActionType.Sleep:
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
            var nearestPerchPosition = field.GetNearestPerchPosition(screenPosition);
            
            // デバッグ用ログ出力
            Debug.Log($"タッチされたスクリーン座標: {screenPosition}");
            Debug.Log($"最も近いPerchのPosition: {nearestPerchPosition.position}, PosIndex:{nearestPerchPosition.positionIndex}, Perch:{nearestPerchPosition.perchIndex}");
        }

        // モバイルデバイスでのタッチ入力
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                Vector2 screenPosition = touch.position;
                var nearestPerchPosition = field.GetNearestPerchPosition(screenPosition);
                
                // デバッグ用ログ出力
                Debug.Log($"タッチされたスクリーン座標: {screenPosition}");
                Debug.Log($"最も近いPerchのPosition: {nearestPerchPosition.position}, PosIndex:{nearestPerchPosition.positionIndex}, Perch:{nearestPerchPosition.perchIndex}");
            }
        }
    }
}
