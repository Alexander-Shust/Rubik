using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class RubikManager : MonoBehaviour
{
    [SerializeField]
    private CentralManager _manager;

    [SerializeField]
    private CubeSolver _solver;
    
    [SerializeField]
    private TMP_InputField _commandField;

    [SerializeField]
    private TMP_InputField _mixField;
    
    [SerializeField]
    private float _turnTime = 1.0f;

    public static RubikManager Manager;
    public LayerMask Mask;
    public List<PhysicalMove> PhysicalMoves;
    public bool IsMoving => _isMoving;

    private string _inputCommand;
    private int _mixCount;
    private List<Moves> _inputMoves;
    private List<Moves> _outputMoves;

    private PhysicalMove _currentMove;
    private Quaternion _targetRotation;
    private bool _isMoving;
    private float _timeElapsed;

    private void Awake()
    {
        Manager = this;
        PhysicalMoves = new List<PhysicalMove>();
        _inputMoves = new List<Moves>();
        _mixCount = 5;
        _commandField.text = "Enter spin sequence here...";
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

    [UsedImplicitly]
    public void SetMixCount()
    {
        if (int.TryParse(_mixField.text, out var mixCount))
        {
            _mixCount = mixCount;
        }
    }
    
    [UsedImplicitly]
    public void Spin()
    {
        if (ParseCommand(_commandField.text, out var moves))
        {
            _inputCommand = _commandField.text;
            _inputMoves.AddRange(moves);
            ExecuteCommand(moves);
            _commandField.text = string.Empty;
        }
        else
        {
            _commandField.text = "Syntax error!";
        }
    }

    [UsedImplicitly]
    public void Solve()
    {
        if (_isMoving) return;
        
        _outputMoves = _solver.Solve(_inputMoves);
        ExecuteCommand(_outputMoves);
        _inputMoves.Clear();
    }

    [UsedImplicitly]
    public void Mix()
    {
        if (_isMoving) return;
        
        var mixMoves = new List<Moves>();
        for (var i = 0; i < _mixCount; ++i)
        {
            var randomMove = (Moves) Random.Range(0, 18); 
            mixMoves.Add(randomMove);
            _inputMoves.Add(randomMove);
        }
        ExecuteCommand(mixMoves);
    }

    public void SetSolution(string moves, int count)
    {
        _commandField.text = count + " moves: " + moves;
    }

    private void ExecuteCommand(List<Moves> moves)
    {
        foreach (var move in moves)
        {
            switch (move)
            {
                case Moves.U: _manager.U(); break;
                case Moves.U2: _manager.U2(); break;
                case Moves.Ub: _manager.Ub(); break;
                case Moves.D: _manager.D(); break;
                case Moves.D2: _manager.D2(); break;
                case Moves.Db: _manager.Db(); break;
                case Moves.F: _manager.F(); break;
                case Moves.F2: _manager.F2(); break;
                case Moves.Fb: _manager.Fb(); break;
                case Moves.B: _manager.B(); break;
                case Moves.B2: _manager.B2(); break;
                case Moves.Bb: _manager.Bb(); break;
                case Moves.L: _manager.L(); break;
                case Moves.L2: _manager.L2(); break;
                case Moves.Lb: _manager.Lb(); break;
                case Moves.R: _manager.R(); break;
                case Moves.R2: _manager.R2(); break;
                case Moves.Rb: _manager.Rb(); break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
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
    
    public void U()
    {
        _inputMoves.Add(Moves.U);
        _manager.U();
    }

    public void U2()
    {
        _inputMoves.Add(Moves.U2);
        _manager.U2();
    }

    public void Ub()
    {
        _inputMoves.Add(Moves.Ub);
        _manager.Ub();
    }

    public void D()
    {
        _inputMoves.Add(Moves.D);
        _manager.D();
    }

    public void D2()
    {
        _inputMoves.Add(Moves.D2);
        _manager.D2();
    }

    public void Db()
    {
        _inputMoves.Add(Moves.Db);
        _manager.Db();
    }

    public void L()
    {
        _inputMoves.Add(Moves.L);
        _manager.L();
    }

    public void L2()
    {
        _inputMoves.Add(Moves.L2);
        _manager.L2();
    }

    public void Lb()
    {
        _inputMoves.Add(Moves.Lb);
        _manager.Lb();
    }

    public void R()
    {
        _inputMoves.Add(Moves.R);
        _manager.R();
    }

    public void R2()
    {
        _inputMoves.Add(Moves.R2);
        _manager.R2();
    }

    public void Rb()
    {
        _inputMoves.Add(Moves.Rb);
        _manager.Rb();
    }

    public void F()
    {
        _inputMoves.Add(Moves.F);
        _manager.F();
    }

    public void F2()
    {
        _inputMoves.Add(Moves.F2);
        _manager.F2();
    }

    public void Fb()
    {
        _inputMoves.Add(Moves.Fb);
        _manager.Fb();
    }

    public void B()
    {
        _inputMoves.Add(Moves.B);
        _manager.B();
    }

    public void B2()
    {
        _inputMoves.Add(Moves.B2);
        _manager.B2();
    }

    public void Bb()
    {
        _inputMoves.Add(Moves.Bb);
        _manager.Bb();
    }
}
