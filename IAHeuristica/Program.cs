using System;
using System.Collections.Generic;

class PuzzleNode : IComparable<PuzzleNode>
{
    public int[,] State { get; set; } // Estado do tabuleiro
    public int Moves { get; set; } // Número de movimentos feitos até este ponto

    public PuzzleNode(int[,] state, int moves)
    {
        State = state;
        Moves = moves;
    }

    public int CompareTo(PuzzleNode other)
    {
        // Compare pelo custo total (heurística + movimentos)
        return (Moves + Heuristic()).CompareTo(other.Moves + other.Heuristic());
    }

    public int Heuristic()
    {
        int sum = 0;
        int[,] goalState = {
            { 1, 2, 3 },
            { 8, 0, 4 },
            { 7, 6, 5 }
        };

        for (int i = 0; i < State.GetLength(0); i++)
        {
            for (int j = 0; j < State.GetLength(1); j++)
            {
                if (State[i, j] != 0)
                {
                    int[] goalPos = FindPosition(goalState, State[i, j]);
                    sum += Math.Abs(i - goalPos[0]) + Math.Abs(j - goalPos[1]);
                }
            }
        }

        return sum;
    }

    private int[] FindPosition(int[,] arr, int value)
    {
        int[] pos = new int[2];

        for (int i = 0; i < arr.GetLength(0); i++)
        {
            for (int j = 0; j < arr.GetLength(1); j++)
            {
                if (arr[i, j] == value)
                {
                    pos[0] = i;
                    pos[1] = j;
                    return pos;
                }
            }
        }

        return pos;
    }

    public bool IsGoalState()
    {
        // Verifique se o estado atual é o estado objetivo
        int[,] goalState = {
            { 1, 2, 3 },
            { 8, 0, 4 },
            { 7, 6, 5 }
        };

        return State.Rank == goalState.Rank &&
               System.Linq.Enumerable.Range(0, State.Rank).All(dimension =>
                 State.GetLength(dimension) == goalState.GetLength(dimension)) &&
               State.Cast<int>().SequenceEqual(goalState.Cast<int>());
    }

    public override bool Equals(object obj)
    {
        if (!(obj is PuzzleNode))
            return false;

        PuzzleNode other = (PuzzleNode)obj;

        return State.Rank == other.State.Rank &&
               System.Linq.Enumerable.Range(0, State.Rank).All(dimension =>
                 State.GetLength(dimension) == other.State.GetLength(dimension)) &&
               State.Cast<int>().SequenceEqual(other.State.Cast<int>());
    }

    public override int GetHashCode()
    {
        return State.Cast<int>().Aggregate(17, (hash, value) => hash * 31 + value);
    }
}

class PuzzleSolver
{
    private int[] dx = { 0, 1, 0, -1 }; // Movimentos possíveis em x
    private int[] dy = { -1, 0, 1, 0 }; // Movimentos possíveis em y

    // Implementação da busca gulosa
    public void GreedySearch(int[,] initialState)
    {
        HashSet<int[,]> visited = new HashSet<int[,]>();
        List<PuzzleNode> priorityQueue = new List<PuzzleNode>();

        PuzzleNode initialNode = new PuzzleNode(initialState, 0);
        priorityQueue.Add(initialNode);

        while (priorityQueue.Count > 0)
        {
            priorityQueue.Sort(); // Ordena a fila de prioridade

            PuzzleNode current = priorityQueue[0];
            priorityQueue.RemoveAt(0);

            if (current.IsGoalState())
            {
                Console.WriteLine("Estado objetivo alcançado em " + current.Moves + " movimentos.");
                return;
            }

            visited.Add(current.State);

            int zeroX = 0, zeroY = 0;
            for (int i = 0; i < 3; ++i)
            {
                for (int j = 0; j < 3; ++j)
                {
                    if (current.State[i, j] == 0)
                    {
                        zeroX = i;
                        zeroY = j;
                        break;
                    }
                }
            }

            for (int i = 0; i < 4; ++i)
            {
                int newX = zeroX + dx[i];
                int newY = zeroY + dy[i];

                if (newX >= 0 && newX < 3 && newY >= 0 && newY < 3)
                {
                    int[,] newState = (int[,])current.State.Clone();
                    newState[zeroX, zeroY] = newState[newX, newY];
                    newState[newX, newY] = 0;

                    PuzzleNode newNode = new PuzzleNode(newState, current.Moves + 1);

                    if (!visited.Contains(newState) && !priorityQueue.Contains(newNode))
                    {
                        priorityQueue.Add(newNode);
                    }
                }
            }
        }

        Console.WriteLine("Não foi possível alcançar o estado objetivo.");
    }
}

class Program
{
    static void Main(string[] args)
    {
        int[,] initialState = { { 2, 8, 3 }, { 1, 6, 0 }, { 4, 7, 5 } };

        PuzzleSolver solver = new PuzzleSolver();
        solver.GreedySearch(initialState);
    }
}