using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CubeSolver : MonoBehaviour
{
    private int[] _state;

    private int[] GetNeighbours(Faces face)
    {
        return face switch
        {
            Faces.U => new[] {0, 1, 2, 3, 0, 1, 2, 3},
            Faces.D => new[] {4, 7, 6, 5, 4, 5, 6, 7},
            Faces.F => new[] {0, 9, 4, 8, 0, 3, 5, 4},
            Faces.B => new[] {2, 10, 6, 11, 2, 1, 7, 6},
            Faces.L => new[] {3, 11, 7, 9, 3, 2, 6, 5},
            Faces.R => new[] {1, 8, 5, 10, 1, 0, 4, 7},
            _ => throw new ArgumentOutOfRangeException(nameof(face), face, null)
        };
    }

    private Moves[] GetAllowedMoves(Phases phase)
    {
        return phase switch
        {
            Phases.Eo => new []
            {
	            Moves.U, Moves.U2, Moves.Ub,
	            Moves.D, Moves.D2, Moves.Db,
	            Moves.F, Moves.F2, Moves.Fb,
	            Moves.B, Moves.B2, Moves.Bb,
	            Moves.L, Moves.L2, Moves.Lb,
	            Moves.R, Moves.R2, Moves.Rb
            },
            Phases.Co =>  new []
            {
	            Moves.U, Moves.U2, Moves.Ub,
	            Moves.D, Moves.D2, Moves.Db,
	            Moves.F2,
	            Moves.B2,
	            Moves.L, Moves.L2, Moves.Lb,
	            Moves.R, Moves.R2, Moves.Rb
            },
            Phases.Htr => new []
            {
	            Moves.U, Moves.U2, Moves.Ub,
	            Moves.D, Moves.D2, Moves.Db,
	            Moves.F2,
	            Moves.B2,
	            Moves.L2,
	            Moves.R2
            },
            Phases.Final => new []
            {
	            Moves.U2,
	            Moves.D2,
	            Moves.F2,
	            Moves.B2,
	            Moves.L2,
	            Moves.R2
            },
            _ => throw new ArgumentOutOfRangeException(nameof(phase), phase, null)
        };
    }
    
    private void Reset()
    {
        _state = (int[]) State.ReadyState.Clone();
    }

    private void Start()
    {
	    Reset();
    }

    public List<Moves> Solve(List<Moves> moves)
    {
	    Reset();
	    foreach (var move in moves)
	    {
		    _state = Move(move, _state);
	    }

	    var result = new List<Moves>();
	    var moveText = string.Empty;
	    
	    var phase1 = SetEdgeOrientation();
        moveText += UpdateState(phase1);
        result.AddRange(phase1);
        moveText += "/";
        
        var phase2 = SetCornerOrientation();
        moveText += UpdateState(phase2);
        result.AddRange(phase2);
        moveText += "/";
        
        var phase3 = SetHtr();
        moveText += UpdateState(phase3);
        result.AddRange(phase3);
        moveText += "/";
        
        var phase4 = SetFinal();
        moveText += UpdateState(phase4);
        result.AddRange(phase4);

        Reset();
        Debug.LogError(moveText);
        Debug.LogError(result.Count);
        return result;
    }

    private string UpdateState(LinkedList<Moves> moves)
    {
	    var moveText = string.Empty;
	    foreach (var move in moves)
	    {
		    _state = Move(move, _state);
		    moveText += move switch
		    {
			    Moves.U => "U",
			    Moves.U2 => "U2",
			    Moves.Ub => "U'",
			    Moves.D => "D",
			    Moves.D2 => "D2",
			    Moves.Db => "D'",
			    Moves.F => "F",
			    Moves.F2 => "F2",
			    Moves.Fb => "F'",
			    Moves.B => "B",
			    Moves.B2 => "B2",
			    Moves.Bb => "B'",
			    Moves.L => "L",
			    Moves.L2 => "L2",
			    Moves.Lb => "L'",
			    Moves.R => "R",
			    Moves.R2 => "R2",
			    Moves.Rb => "R'",
			    _ => throw new ArgumentOutOfRangeException()
		    };
	    }
	    
	    return moveText;
    }

    private int[] Move(Moves move, int[] state)
    {
        var turnCount = (int) move % 3 + 1;
        var face = (Faces) ((int) move / 3);
        state = (int[]) state.Clone();

        for (var count = 0; count < turnCount; ++count)
        {
            var oldState = (int[]) state.Clone();
            for (var i = 0; i < 8; ++i)
            {
	            var victim = GetNeighbours(face)[i];
	            victim += i >= 4 ? 12 : 0;
                var affector = GetNeighbours(face)[(i & 3) == 3 ? i - 3 : i + 1];
                affector += i >= 4 ? 12 : 0;
                int rotationOffset;
                if (i <= 3)
                {
	                rotationOffset = face == Faces.F || face == Faces.B ? 1 : 0;
                }
                else
                {
	                rotationOffset = face == Faces.U || face == Faces.D ? 0 : 2 - i % 2;
                }
                state[victim] = oldState[affector];
                state[victim + 20] = oldState[affector + 20] + rotationOffset;
                if (count == turnCount - 1)
                {
                    state[victim + 20] %= i >= 4 ? 3 : 2;
                }
            }
        }
        return state;
    }

    private LinkedList<Moves> FindMoves(Phases phase, int[] currentPhaseState, int[] targetPhaseState)
    {
	    if (currentPhaseState.SequenceEqual(targetPhaseState))
		    return new LinkedList<Moves>();
	    
        var stateQueue = new Queue<int[]>();
		stateQueue.Enqueue(_state);
		stateQueue.Enqueue(State.ReadyState);

		Dictionary<int[], int[]> predecessor = new Dictionary<int[], int[]>(new ArrayComparer());
		var isForward = new Dictionary<int[], bool>(new ArrayComparer());
		Dictionary<int[], int> lastMove = new Dictionary<int[], int>(new ArrayComparer());
		isForward[currentPhaseState] = true;
		isForward[targetPhaseState] = false;

		while (true)
		{
			var oldState = stateQueue.Dequeue();
			var oldPhaseState = phase switch
			{
				Phases.Eo => GetEdgeOrientation(oldState),
				Phases.Co => GetCornerOrientation(oldState),
				Phases.Htr => GetHtr(oldState),
				Phases.Final => oldState,
				_ => throw new ArgumentOutOfRangeException(nameof(phase), phase, null)
			};
			var oldDir = isForward[oldPhaseState];

			for (var move = 0; move < 18; ++move)
			{
				if (!GetAllowedMoves(phase).Contains((Moves) move)) continue;
				
				int[] newState = Move((Moves) move, oldState);
				int[] newPhaseState = phase switch
				{
					Phases.Eo => GetEdgeOrientation(newState),
					Phases.Co => GetCornerOrientation(newState),
					Phases.Htr => GetHtr(newState),
					Phases.Final => newState,
					_ => throw new ArgumentOutOfRangeException(nameof(phase), phase, null)
				};
				if (isForward.TryGetValue(newPhaseState, out var newDir))
				{

					if (newDir != oldDir)
					{
						if (!oldDir)
						{
							(newPhaseState, oldPhaseState) = (oldPhaseState, newPhaseState);
							move = (int) AntiMove(move);
						}

						var result = new LinkedList<Moves>();
						result.AddFirst((Moves) move);

						while (!oldPhaseState.SequenceEqual(currentPhaseState))
						{
							result.AddFirst((Moves) lastMove[oldPhaseState]);
							oldPhaseState = predecessor[oldPhaseState];
						}
						
						while (!newPhaseState.SequenceEqual(targetPhaseState))
						{
							result.AddLast(AntiMove(lastMove[newPhaseState]));
							newPhaseState = predecessor[newPhaseState];
						}

						return result;
					}
				}

				else
				{
					stateQueue.Enqueue(newState);
					isForward[newPhaseState] = oldDir;
					lastMove[newPhaseState] = move;
					predecessor[newPhaseState] = oldPhaseState;
				}
			}
		}
    }

    private static Moves AntiMove(int move)
    {
	    return (Moves) move switch
	    {
		    Moves.U => Moves.Ub,
		    Moves.U2 => Moves.U2,
		    Moves.Ub => Moves.U,
		    Moves.D => Moves.Db,
		    Moves.D2 => Moves.D2,
		    Moves.Db => Moves.D,
		    Moves.F => Moves.Fb,
		    Moves.F2 => Moves.F2,
		    Moves.Fb => Moves.F,
		    Moves.B => Moves.Bb,
		    Moves.B2 => Moves.B2,
		    Moves.Bb => Moves.B,
		    Moves.L => Moves.Lb,
		    Moves.L2 => Moves.L2,
		    Moves.Lb => Moves.L,
		    Moves.R => Moves.Rb,
		    Moves.R2 => Moves.R2,
		    Moves.Rb => Moves.R,
		    _ => throw new ArgumentOutOfRangeException(nameof(move), move, null)
	    };
    }

    private int[] GetEdgeOrientation(int[] source)
    {
	    var target = new int[12];
	    Array.Copy(source, 20, target, 0, 12);
	    return target;
    }

    private int[] GetCornerOrientation(int[] source)
    {
	    var target = new int[8];
	    Array.Copy(source, 31, target, 0, 8);
	    for (int e = 0; e < 12; e++)
		    target[0] |= (source[e] / 8) << e;
	    return target;
    }

    private int[] GetHtr(int[] source)
    {
	    var target = new[] { 0, 0, 0 };
	    for (int e = 0; e < 12; e++)
		    target[0] |= ((source[e] > 7) ? 2 : (source[e] & 1)) << (2 * e);
	    for (int c = 0; c < 8; c++)
		    target[1] |= ((source[c + 12] - 12) & 5) << (3 * c);
	    for (int i = 12; i < 20; i++)
	    for (int j = i + 1; j < 20; j++)
		    target[2] ^= Convert.ToInt32(source[i] > source[j]);
	    return target;
    }

    private LinkedList<Moves> SetEdgeOrientation()
    {
	    return FindMoves(Phases.Eo, GetEdgeOrientation(_state), GetEdgeOrientation(State.ReadyState));
    }

    private LinkedList<Moves> SetCornerOrientation()
    {
	    return FindMoves(Phases.Co, GetCornerOrientation(_state), GetCornerOrientation(State.ReadyState));
    }

    private LinkedList<Moves> SetHtr()
    {
	    return FindMoves(Phases.Htr, GetHtr(_state), GetHtr(State.ReadyState));
    }

    private LinkedList<Moves> SetFinal()
    {
	    return FindMoves(Phases.Final, _state, State.ReadyState);
    }
    
    private class ArrayComparer : IEqualityComparer<int[]>
    {
	    public bool Equals(int[] x, int[] y)
	    {
		    if (x == null || y == null) return false;
		    
		    return x.SequenceEqual(y);
	    }

	    public int GetHashCode(int[] obj)
	    {
		    var hashCode = 17;
		    foreach (var t in obj)
		    {
			    unchecked { hashCode = hashCode * 23 + t; }
		    }
		    return hashCode;
	    }
    }
}