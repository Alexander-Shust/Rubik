using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RubikManager : MonoBehaviour
{
    [SerializeField]
    private CentralManager _manager;

    [SerializeField]
    private CubeSolver _solver;
    
    [SerializeField]
    private TMP_InputField _command;
    
    [SerializeField]
    private float _turnTime = 1.0f;

    public static RubikManager Manager;
    public LayerMask Mask;
    public List<PhysicalMove> PhysicalMoves;
    public bool IsMoving => _isMoving;

    private PhysicalMove _currentMove;
    private Quaternion _targetRotation;
    private bool _isMoving;
    private float _timeElapsed;

    private void Awake()
    {
        Manager = this;
        PhysicalMoves = new List<PhysicalMove>();
        _command.text = "Enter spin sequence here...";
    }

    private void Update()
    {
        if (!_isMoving)
        {
            if (PhysicalMoves.Count == 0) return;
            
            _isMoving = true;
            _timeElapsed = 0.0f;
            _currentMove = PhysicalMoves[0];
            _targetRotation = _currentMove.Movable.rotation * Quaternion.AngleAxis(90.0f * _currentMove.Speed, Vector3.up);
            _currentMove.Movable.gameObject.GetComponent<FaceMover>().AcquireChildren();
            PhysicalMoves.RemoveAt(0);
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

    public void Spin()
    {
        if (ParseCommand(_command.text, out var moves))
        {
            _solver.Solve(moves);
            ExecuteCommand(_command.text);
            _command.text = string.Empty;
        }
        else
        {
            _command.text = "Syntax error!";
        }
    }

    private void ExecuteCommand(string command)
    {
        command = command.Replace(" ", "");
        while (FindNextToken(command, out var token))
        {
            if (token == string.Empty) return;
            switch (token)
            {
                case "U": _manager.U(); break;
                case "U2": _manager.U2(); break;
                case "U'": _manager.Ub(); break;
                case "D": _manager.D(); break;
                case "D2": _manager.D2(); break;
                case "D'": _manager.Db(); break;
                case "F": _manager.F(); break;
                case "F2": _manager.F2(); break;
                case "F'": _manager.Fb(); break;
                case "B": _manager.B(); break;
                case "B2": _manager.B2(); break;
                case "B'": _manager.Bb(); break;
                case "R": _manager.R(); break;
                case "R2": _manager.R2(); break;
                case "R'": _manager.Rb(); break;
                case "L": _manager.L(); break;
                case "L2": _manager.L2(); break;
                case "L'": _manager.Lb(); break;
            }

            command = command.Substring(token.Length);
        } 
    }

    private bool ParseCommand(string command, out List<Moves> moves)
    {
        moves = new List<Moves>();
        command = command.Replace(" ", "");
        while (FindNextToken(command, out var token))
        {
            if (token == string.Empty) return true;
            switch (token)
            {
                case "U": moves.Add(Moves.U); break;
                case "U2": moves.Add(Moves.U2); break;
                case "U'": moves.Add(Moves.Ub); break;
                case "D": moves.Add(Moves.D); break;
                case "D2": moves.Add(Moves.D2); break;
                case "D'": moves.Add(Moves.Db); break;
                case "F": moves.Add(Moves.F); break;
                case "F2": moves.Add(Moves.F2); break;
                case "F'": moves.Add(Moves.Fb); break;
                case "B": moves.Add(Moves.B); break;
                case "B2": moves.Add(Moves.B2); break;
                case "B'": moves.Add(Moves.Bb); break;
                case "R": moves.Add(Moves.R); break;
                case "R2": moves.Add(Moves.R2); break;
                case "R'": moves.Add(Moves.Rb); break;
                case "L": moves.Add(Moves.L); break;
                case "L2": moves.Add(Moves.L2); break;
                case "L'": moves.Add(Moves.Lb); break;
                
                default: return false;
            }

            command = command.Substring(token.Length);
        } 
        return false;
    }

    private bool FindNextToken(string command, out string token)
    {
        token = string.Empty;
        if (command == string.Empty) return true;
        if (!"UDFBRL".Contains(command[0].ToString())) return false;
        token = command[0].ToString();
        if (command.Length == 1) return true;
        if ("UDBFRL".Contains(command[1].ToString())) return true;
        switch (command[1])
        {
            case '\'':
                token = token + "'";
                break;
            case '2':
                token = token + "2";
                break;
            default:
                return false;
        }
        return true;
    }
}
