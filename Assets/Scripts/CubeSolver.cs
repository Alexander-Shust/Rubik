using System;
using System.Collections.Generic;
using UnityEngine;

public class CubeSolver : MonoBehaviour
{
    private int[] _state;
    
    private static readonly int[,] AFFECTED_CUBIES =
    {
        {  0,  1,  2,  3,  0,  1,  2,  3 },   // U
        {  4,  7,  6,  5,  4,  5,  6,  7 },   // D
        {  0,  9,  4,  8,  0,  3,  5,  4 },   // F
        {  2, 10,  6, 11,  2,  1,  7,  6 },   // B
        {  3, 11,  7,  9,  3,  2,  6,  5 },   // L
        {  1,  8,  5, 10,  1,  0,  4,  7 },   // R
    };

    private int[] GetNeighbours(Faces face)
    {
        return face switch
        {
            Faces.U => new[] {0,  1,  2,  3,  0,  1,  2,  3},
            Faces.D => new[] {4,  7,  6,  5,  4,  5,  6,  7},
            Faces.F => new[] {0,  9,  4,  8,  0,  3,  5,  4},
            Faces.B => new[] {2, 10,  6, 11,  2,  1,  7,  6},
            Faces.L => new[] {3, 11,  7,  9,  3,  2,  6,  5},
            Faces.R => new[] {1,  8,  5, 10,  1,  0,  4,  7},
            _ => throw new ArgumentOutOfRangeException(nameof(face), face, null)
        };
    }
    
    private void Reset()
    {
        _state = (int[]) State.ReadyState.Clone();
    }

    private int[] GetEdgeOrientations(int[] source)
    {
        var target = new int[12];
        Array.Copy(source, 20, target, 0, 12);
        return target;
    }

    public void Solve()
    {
        Debug.LogError("Trying to solve...");
        Reset();
        var moves = SetEdgeOrientation();
        foreach (var move in moves)
        {
            _state = Move(move, _state);
        }
    }

    private int[] Move(Moves move, int[] state)
    {
        var turnCount = (int) move % 3 + 1;
        var face = (Faces) ((int) move / 3);
        // state = (int[]) state.Clone();

        for (var count = 0; count < turnCount; ++count)
        {
            int[] oldState = (int[]) state.Clone();
            for (var i = 0; i < 8; ++i)
            {
                int isCorner = Convert.ToInt32(i > 3);
                int target = AFFECTED_CUBIES[face, i] + i >= 4 ? 12 : 0;
                int killer = AFFECTED_CUBIES[face, (i & 3) == 3 ? i - 3 : i + 1] + i >= 4 ? 12 : 0;
                int orientationDelta = (i < 4) ? Convert.ToInt32(face > 1 && face < 4) :
                    (face < 2) ? 0 : 2 - (i & 1);
                state[target] = oldState[killer];
                state[target + 20] = oldState[killer + 20] + orientationDelta;
                if (count == turnCount)
                {
                    state[target + 20] %= 2 + isCorner;
                }
            }
        }
        return state;
    }

    private List<Moves> FindMoves(Phases phase, int[] currentState, int[] targetState)
    {
        var result = new List<Moves>();
        return result;
    }

    private List<Moves> SetEdgeOrientation()
    {
        return new List<Moves>();
    }
}