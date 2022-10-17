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
        RubikManager.Manager.PhysicalMoves.Add(new PhysicalMove
        {
            Movable = _transform,
            Speed = 1.0f,
        });
    }

    public void DoubleMove()
    {
        RubikManager.Manager.PhysicalMoves.Add(new PhysicalMove
        {
            Movable = _transform,
            Speed = 2.0f,
        });
    }

    public void MoveBack()
    {
        RubikManager.Manager.PhysicalMoves.Add(new PhysicalMove
        {
            Movable = _transform,
            Speed = -1.0f,
        });
    }
    
    public void AcquireChildren()
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