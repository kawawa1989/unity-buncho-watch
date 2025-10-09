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
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Application.targetFrameRate = 30;
        SetPerch(0, 5);
    }

    void SetPerch(int perchIndex, int position)
    {
        var perch = field.GetPerchAt(perchIndex);
        transform.position = perch.GetPosition(position);
        _currentPerchIndex = perchIndex;
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

    BunchoAction DecideNextAction()
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
                var perch = field.GetPerchAt(_currentPerchIndex);
                var nextPosition = perch.Pick(_positionIndex);
                _positionIndex = nextPosition;
                return new Walk(animator, transform, perch.GetPosition(nextPosition));
            }
            case 1:
            {
                var nextPerchIndex = _currentPerchIndex == 0 ? 1 : 0;
                var nextPerch = field.GetPerchAt(nextPerchIndex);
                var jumpTo = nextPerch.PickNearest(_positionIndex);
                _currentPerchIndex = nextPerchIndex;
                _positionIndex = jumpTo;
                return new Jump(animator, transform, nextPerch.GetPosition(jumpTo));
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
