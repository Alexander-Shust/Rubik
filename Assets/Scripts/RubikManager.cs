using System.Collections.Generic;
using UnityEngine;

public class RubikManager : MonoBehaviour
{
    [SerializeField]
    private float _turnTime = 1.0f;

    public static RubikManager Manager;
    public LayerMask Mask;
    public List<Move> Moves;

    private Move _currentMove;
    private Quaternion _targetRotation;
    private bool _isMoving;
    private float _timeElapsed;

    private void Awake()
    {
        Manager = this;
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
            _targetRotation = _currentMove.Movable.rotation * Quaternion.AngleAxis(90.0f * _currentMove.Speed, Vector3.up);
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
        movable.Rotate(Vector3.up, _currentMove.Speed * 90.0f / _turnTime * deltaTime);
    }
}
