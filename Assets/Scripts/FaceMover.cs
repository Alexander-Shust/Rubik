using UnityEngine;

public class FaceMover : MonoBehaviour
{
    private Transform _transform;

    private void Awake()
    {
        _transform = transform;
    }

    public void Move()
    {
        AcquireChildren();
        RubikManager.Manager.Moves.Add(new Move
        {
            Movable = _transform,
            Speed = 1.0f,
        });
    }

    public void DoubleMove()
    {
        AcquireChildren();
        RubikManager.Manager.Moves.Add(new Move
        {
            Movable = _transform,
            Speed = 2.0f,
        });
    }

    public void MoveBack()
    {
        AcquireChildren();
        RubikManager.Manager.Moves.Add(new Move
        {
            Movable = _transform,
            Speed = -1.0f,
        });
    }
    
    private void AcquireChildren()
    {
        var forward = _transform.forward;
        var right = _transform.right;
        AcquireChild(forward);
        AcquireChild(-forward);
        AcquireChild(right);
        AcquireChild(-right);
        AcquireChild(forward + right);
        AcquireChild(-(forward + right));
        AcquireChild(forward - right);
        AcquireChild(right - forward);
    }

    private void AcquireChild(Vector3 direction)
    {
        var ray = new Ray(transform.position, _transform.position + direction * 2.0f);
        if (Physics.Raycast(ray, out var hit, float.MaxValue, RubikManager.Manager.Mask))
        {
            hit.collider.transform.SetParent(transform, true);
        }
    }
}