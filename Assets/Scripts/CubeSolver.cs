using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.VersionControl;
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
            Phases.EO => new []
            {
	            Moves.U, Moves.U2, Moves.Ub,
	            Moves.D, Moves.D2, Moves.Db,
	            Moves.F, Moves.F2, Moves.Fb,
	            Moves.B, Moves.B2, Moves.Bb,
	            Moves.L, Moves.L2, Moves.Lb,
	            Moves.R, Moves.R2, Moves.Rb
            },
            Phases.CO =>  new []
            {
	            Moves.U, Moves.U2, Moves.Ub,
	            Moves.D, Moves.D2, Moves.Db,
	            Moves.F2,
	            Moves.B2,
	            Moves.L, Moves.L2, Moves.Lb,
	            Moves.R, Moves.R2, Moves.Rb
            },
            Phases.HTR => new []
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

    public void Solve()
    {
        Debug.LogError("Trying to solve...");
        Reset();
        // _state = Move(Moves.U2, _state);
        _state = Move(Moves.L2, _state);
        var moves = SetEdgeOrientation();
        var moveText = new List<string>();
        foreach (var move in moves)
        {
            moveText.Add(move switch
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
            });
        }

        foreach (var text in moveText)
        {
	        Debug.LogError(text);
        }
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
                var victim = GetNeighbours(face)[i] + i >= 4 ? 12 : 0;
                var affector = GetNeighbours(face)[(i & 3) == 3 ? i - 3 : i + 1] + i >= 4 ? 12 : 0;
                int rotationOffset = 0;
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
                if (count == turnCount)
                {
                    state[victim + 20] %= 2 + i >= 4 ? 1 : 0;
                }
            }
        }
        return state;
    }

    private List<Moves> FindMoves(Phases phase, int[] currentPhaseState, int[] targetPhaseState)
    {
        var stateQueue = new Queue<int[]>();
		stateQueue.Enqueue(_state);
		stateQueue.Enqueue(State.ReadyState);

		Dictionary<int[], int[]> predecessor = new Dictionary<int[], int[]>(new ArrayComparer());
		var isForward = new Dictionary<int[], bool>(new ArrayComparer());
		Dictionary<int[], int> lastMove = new Dictionary<int[], int>(new ArrayComparer());
		isForward[currentPhaseState] = true;
		isForward[targetPhaseState] = false;

		// var insanity = 10000;
		// while (insanity-- > 0)
		while (true)
		{
			// Get next state from queue, find its ID and direction
			var oldState = stateQueue.Dequeue();
			var oldPhaseState = GetEdgeOrientations(oldState);
			var oldDir = isForward[oldPhaseState];

			for (var move = 0; move < 18; ++move)
			{
				// only try the allowed moves in the current phase
				if (!GetAllowedMoves(phase).Contains((Moves) move))
					continue;
				
				// generate a new state from the old state
				int[] newState = Move((Moves) move, oldState);
				int[] newPhaseState = GetEdgeOrientations(newState); 
				if (isForward.TryGetValue(newPhaseState, out var newDir))
				{

					// if we have already found this new state from the other direction, then we can construct a full path
					if (newDir != oldDir)
					{
						// swap directions if necessary
						if (!oldDir)
						{
							(newPhaseState, oldPhaseState) = (oldPhaseState, newPhaseState);
							move = (int) AntiMove(move);
						}

						// build a linked list for the moves found in this phase
						var result = new List<Moves>();
						result.Insert(0, (Moves) move);

						// traverse backward to beginning state
						while (!oldPhaseState.SequenceEqual(currentPhaseState))
						{
							result.Insert(0, (Moves) lastMove[oldPhaseState]);
							oldPhaseState = predecessor[oldPhaseState];
						}
						
						// traverse forward to goal state
						while (!newPhaseState.SequenceEqual(targetPhaseState))
						{
							result.Add(AntiMove(lastMove[newPhaseState]));
							newPhaseState = predecessor[newPhaseState];
						}

						return result;
					}
					// Debug.LogError("Cycling...");
				}

				// if we have not seen this new state before, add it to queue and dictionaries
				else
				{
					Debug.LogError("Adding...");
					stateQueue.Enqueue(newState);
					isForward[newPhaseState] = oldDir;
					lastMove[newPhaseState] = move;
					predecessor[newPhaseState] = oldPhaseState;
				}
			}
		}
    }

    private Moves AntiMove(int move)
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

    private int[] GetEdgeOrientations(int[] source)
    {
	    var target = new int[12];
	    Array.Copy(source, 20, target, 0, 12);
	    return target;
    }

    private List<Moves> SetEdgeOrientation()
    {
	    return FindMoves(Phases.EO, GetEdgeOrientations(_state), GetEdgeOrientations(State.ReadyState));
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