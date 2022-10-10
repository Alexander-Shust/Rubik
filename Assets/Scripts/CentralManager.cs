using UnityEngine;

public class CentralManager : MonoBehaviour
{
    [SerializeField] private FaceMover _u;
    [SerializeField] private FaceMover _d;
    [SerializeField] private FaceMover _l;
    [SerializeField] private FaceMover _r;
    [SerializeField] private FaceMover _f;
    [SerializeField] private FaceMover _b;

    public void U()
    {
        _u.Move();
    }

    public void U2()
    {
        _u.DoubleMove();
    }

    public void Ub()
    {
        _u.MoveBack();
    }

    public void D()
    {
        _d.Move();
    }

    public void D2()
    {
        _d.DoubleMove();
    }

    public void Db()
    {
        _d.MoveBack();
    }

    public void L()
    {
        _l.Move();
    }

    public void L2()
    {
        _l.DoubleMove();
    }

    public void Lb()
    {
        _l.MoveBack();
    }

    public void R()
    {
        _r.Move();
    }

    public void R2()
    {
        _r.DoubleMove();
    }

    public void Rb()
    {
        _r.MoveBack();
    }

    public void F()
    {
        _f.Move();
    }

    public void F2()
    {
        _f.DoubleMove();
    }

    public void Fb()
    {
        _f.MoveBack();
    }

    public void B()
    {
        _b.Move();
    }

    public void B2()
    {
        _b.DoubleMove();
    }

    public void Bb()
    {
        _b.MoveBack();
    }
}