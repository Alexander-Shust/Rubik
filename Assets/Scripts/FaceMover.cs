using UnityEngine;

public class FaceMover : MonoBehaviour
{
    [SerializeField]
    private Transform[] _children;

    private bool _isAcquired;
    
    public void Move(Vector3 axis)
    {
        AcquireChildren();
        RubikManager.Moves.Add(new Move
        {
            Movable = transform,
            Speed = 1.0f,
            Axis = axis
        });
    }

    public void DoubleMove()
    {
        AcquireChildren();
        RubikManager.Moves.Add(new Move
        {
            Movable = transform,
            Speed = 2.0f,
        });
    }

    public void MoveBack()
    {
        AcquireChildren();
        RubikManager.Moves.Add(new Move
        {
            Movable = transform,
            Speed = -1.0f,
        });
    }

    private void AcquireChildren()
    {
        if (_isAcquired) return;

        foreach (var child in _children)
        {
            child.SetParent(transform, true);
        }

        _isAcquired = true;
    }
}