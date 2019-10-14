using System;
using System.Collections.Generic;

namespace MShare_ASP.Utils
{
    internal class SpendingOptimizer
    {
        private long[][] balanceMatrix;
        private int usercount;

        public SpendingOptimizer(long[][] balanceMatrix)
        {
            this.balanceMatrix = balanceMatrix;
            this.usercount = balanceMatrix.Length;
        }

        public void Optimize()
        {
            RemoveCycle();
            ReduceTransfers();
        }

        public long[][] GetResult()
        {
            return balanceMatrix;
        }

        private void RemoveCycle()
        {
            bool[] visited = new bool[usercount];
            bool[] stack = new bool[usercount];
            bool foundcycle = true;
            while (foundcycle)
            {
                foundcycle = false;
                for (int i = 0; i < usercount; i++)
                {
                    visited[i] = false;
                    stack[i] = false;
                }
                Stack<int> Path = new Stack<int>();
                for (int i = 0; i < usercount && !foundcycle; i++)
                {
                    Path.Clear();
                    Path.Push(i);
                    long cu = CycleUtil(i, visited, stack, Path);
                    if (cu > 0)
                    {
                        int[] cyclepath = Path.ToArray();
                        for (int j = 0; j < Path.Count - 1; j++)
                        {
                            balanceMatrix[cyclepath[j + 1]][cyclepath[j]] -= cu;
                        }
                        balanceMatrix[cyclepath[0]][cyclepath[Path.Count - 1]] -= cu;
                        foundcycle = true;
                    }
                }
            }
        }

        private long CycleUtil(int v, bool[] visited, bool[] stack, Stack<int> Path)
        {
            visited[v] = true;
            stack[v] = true;
            for (int i = 0; i < usercount; i++)
            {
                Path.Push(i);
                if (balanceMatrix[v][i] > 0)
                {
                    long cu = 0;
                    bool visitedi = visited[i];
                    if (visitedi == false)
                    {
                        cu = CycleUtil(i, visited, stack, Path);
                    }
                    if (visitedi == false && cu > 0)
                    {
                        return Math.Min(cu, balanceMatrix[v][i]);
                    } else if (stack[i])
                    {
                        Path.Pop();
                        {
                            Stack<int> tmp = new Stack<int>();
                            int j = Path.Pop();
                            while (j != i)
                            {
                                tmp.Push(j);
                                j = Path.Pop();
                            }
                            Path.Clear();
                            Path.Push(j);
                            while (tmp.Count != 0)
                                Path.Push(tmp.Pop());
                        }
                        return balanceMatrix[v][i];
                    }
                }
                Path.Pop();
            }
            stack[v] = false;

            return 0;
        }

        private void ReduceTransfers()
        {
            bool done = false;
            int n = usercount;
            while (!done)
            {
                Stack<int> Topological = new Stack<int>();
                int[] neighbors = new int[n];
                for (int i = 0; i < n; i++)
                {
                    neighbors[i] = 0;
                    for (int j = 0; j < n; j++)
                    {
                        if (balanceMatrix[j][i] > 0)
                        {
                            neighbors[i] += 1;
                        }
                    }
                }
                while (Topological.Count != n)
                {
                    for (int i = 0; i < n; i++)
                    {
                        if (neighbors[i] == 0)
                        {
                            neighbors[i] = -1;
                            Topological.Push(i);
                            for (int j = 0; j < n; j++)
                            {
                                if (balanceMatrix[i][j] > 0)
                                {
                                    neighbors[j] -= 1;
                                }
                            }
                        }
                    }
                }
                int[] TopologicalArray = new int[Topological.Count];
                {
                    int[] tmp = Topological.ToArray();
                    for (int i = 0; i < Topological.Count; i++)
                    {
                        TopologicalArray[i] = tmp[Topological.Count - i - 1];
                    }
                }
                long[] cost = new long[n];
                int[] len = new int[n];
                int[] parent = new int[n];
                for (int i = 0; i < n; i++)
                {
                    cost[i] = -1;
                    parent[i] = -1;
                    len[i] = 0;
                }
                for (int i = 0; i < TopologicalArray.Length; i++)
                {
                    for (int j = 0; j < n; j++)
                    {
                        if (balanceMatrix[TopologicalArray[i]][j] > 0)
                        {
                            if (len[j] < len[TopologicalArray[i]] + 1)
                            {
                                if (cost[TopologicalArray[i]] == -1)
                                {
                                    cost[j] = balanceMatrix[TopologicalArray[i]][j];
                                } else
                                    cost[j] = Math.Min(cost[TopologicalArray[i]], balanceMatrix[TopologicalArray[i]][j]);
                                len[j] = len[TopologicalArray[i]] + 1;
                                parent[j] = TopologicalArray[i];
                            }
                        }
                    }
                }
                long max_save = 0;
                long max_save_per = 0;
                int max_ind = -1;
                for (int i = 0; i < n; i++)
                {
                    if (len[i] > 1 && max_save < cost[i] * len[i])
                    {
                        max_save = cost[i] * len[i];
                        max_save_per = cost[i];
                        max_ind = i;
                    }
                }
                if (max_ind == -1)
                {
                    done = true;
                } else
                {
                    int curr = max_ind;
                    while (parent[curr] != -1)
                    {
                        balanceMatrix[parent[curr]][curr] -= max_save_per;
                        curr = parent[curr];
                    }
                    balanceMatrix[curr][max_ind] += max_save_per;
                }
            }
        }
    }
}