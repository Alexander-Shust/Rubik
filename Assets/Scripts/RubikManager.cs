using System.Collections.Generic;
using UnityEngine;

public class RubikManager : MonoBehaviour
{
    [SerializeField]
    private float _turnTime = 1.0f;

    public static List<Move> Moves;

    private Move _currentMove;
    private Quaternion _targetRotation;
    // private Quaternion _startRotation;
    private bool _isMoving;
    private float _timeElapsed;

    private void Awake()
    {
        Moves = new List<Move>();
    }

    private void Update()
    {
        if (!_isMoving)
        {
            if (Moves.Count == 0) return;
            
            _isMoving = true;
            _timeElapsed = 0.0f;
            _currentMove = Moves[0];
            // _startRotation = _currentMove.Movable.rotation;
            _targetRotation = _currentMove.Movable.rotation * Quaternion.AngleAxis(90.0f, _currentMove.Axis);
            Moves.RemoveAt(0);
        }
        
        var deltaTime = Time.deltaTime;
        var movable = _currentMove.Movable;
        _timeElapsed += deltaTime;
        if (_timeElapsed >= _turnTime)
        {
            _isMoving = false;
            movable.rotation = _targetRotation;
            return;
        }
        // var offset = Mathf.Lerp(0.0f, _currentMove.Speed * 90.0f, _timeElapsed / _turnTime);
        movable.Rotate(_currentMove.Axis, _currentMove.Speed * 90.0f / _turnTime * deltaTime);
        // movable.rotation *= Quaternion.Slerp(_startRotation, _targetRotation, _turnTime - _timeLeft);
    }
}
