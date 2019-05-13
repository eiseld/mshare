using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MShare_ASP.Utils {
    internal class SpendingOptimizer{
        private long[,] _owes;
        private int _usercount;
        public SpendingOptimizer(long[,] owes, int usercount)
        {
            _owes = owes;
            _usercount = usercount;
        }
        public void Optimize()
        {
            RemoveCycle();
            ReduceTransfers();
        }

        public long[,] GetResult()
        {
            return _owes;
        }
        /// Old algorithm here
        /*
         Old algorithm
        private void RemoveCycle()
        {
            int n = _usercount;
            bool[] visited = new bool[n];
            bool[] stack = new bool[n];
            int[] parent = new int[n];
            bool foundcycle = true;
            while(foundcycle)
            {
                foundcycle = false;
                for(int i = 0; i < n; i++)
                {
                    visited[i] = false;
                    stack[i] = false;
                    parent[i] = -1;
                }
                Stack<int> Path = new Stack<int>();
                for(int i = 0; i < n && !foundcycle; i++)
                {
                    Path.Clear();
                    Path.Push(i);
                    long cu = CycleUtil(i,visited,stack, Path);
                   // if(cu == 0)
                   //     for (; ;) { };   
                    if(cu > 0)
                    {
                       int[] cyclepath = Path.ToArray();
                       for(int j = 0; j < Path.Count - 1; j++)
                        {
                            _owes[cyclepath[j],cyclepath[j+1]] = 0;
                        }
                        _owes[cyclepath[Path.Count-1],cyclepath[0]] = 0;

                        //foundcycle = true;
                    }
                }
            }
        }

        private long CycleUtil(int v, bool[] visited, bool[] stack, Stack<int> Path)
        {
            visited[v] = true;
            stack[v] = true;
            for(int i = 0; i < _usercount; i++)
            {
                Path.Push(i);
                if(_owes[v,i]>0)
                {
                    if (v == i)
                    { }
                    long cu = 0;
                    if(visited[i] == false)
                    {
                         cu = CycleUtil(i,visited,stack,Path);
                    }
                    if(visited[i] == false && cu > 0)
                    {
                        return Math.Min(cu,_owes[v,i]);
                    }
                    else if (stack[i])
                    {
                        Path.Pop();
                        return _owes[v,i];
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
            int n = _usercount;
            while(!done)
            {
                Stack<int> Topological = new Stack<int>();
                int[] neighbors = new int[n];
                for(int i = 0; i < n; i++)
                {
                    neighbors[i] = 0;
                    for(int j = 0; j < n; j++)
                    {
                        if(_owes[j,i] > 0)
                        {
                            neighbors[i] += 1;
                        }
                    }
                }
                while(Topological.Count != n)
                {
                    for(int i = 0; i < n; i++)
                    {
                        if(neighbors[i] == 0)
                        {
                            Topological.Push(i);
                            for(int j = 0; j < n; j++)
                            {
                                if(_owes[i,j] > 0)
                                {
                                    neighbors[j] -= 1;
                                }
                            }
                        }
                    }
                }
                int[] TopologicalArray = Topological.ToArray();
                long[] cost = new long[n];
                int[] len = new int[n];
                int[] parent = new int[n];
                for(int i = 0; i < n; i++)
                {
                    cost[i] = -1;
                    parent[i] = -1;
                    len[i] = 0;
                }
                cost[TopologicalArray[0]] = 0;
                for(int i = 0; i < TopologicalArray.Length; i++)
                {
                    for(int j = 0; j < n; j++)
                    {
                        if(_owes[TopologicalArray[i],j] > 0)
                        {
                            if(len[j] < len[TopologicalArray[i]] + 1)
                            {
                                cost[j] = Math.Min(cost[TopologicalArray[i]],_owes[TopologicalArray[i],j]);
                                len[j] = len[TopologicalArray[i]] + 1;
                                parent[j] = TopologicalArray[i];
                            }
                        }
                    }
                }
                long max_save = 0;
                long max_save_per = 0;
                int max_ind = -1;
                for(int i = 0; i < n; i++)
                {
                    if(len[i] > 1 && max_save < cost[i] * len[i])
                    {
                        max_save = cost[i] * len[i];
                        max_save_per = cost[i];
                        max_ind = i;
                    }
                }
                if(max_ind == -1)
                {
                    done = true;
                }
                else
                {
                    int curr = max_ind;
                    while(parent[curr] != -1)
                    {
                        _owes[parent[curr],curr] -= max_save_per;
                        curr = parent[curr];
                    }
                    _owes[curr,max_ind] += max_save_per;
                }
            }
        }
        */
        private void RemoveCycle()
        {
            int n = _usercount;
            bool[] visited = new bool[n];
            bool[] stack = new bool[n];
            int[] parent = new int[n];
            bool foundcycle = true;
            while (foundcycle)
            {
                foundcycle = false;
                for (int i = 0; i < n; i++)
                {
                    visited[i] = false;
                    stack[i] = false;
                    parent[i] = -1;
                }
                Stack<int> Path = new Stack<int>();
                for (int i = 0; i < n && !foundcycle; i++)
                {
                    Path.Clear();
                    Path.Push(i);
                    long cu = CycleUtil(i, visited, stack, Path);
                    // if(cu == 0)
                    //     for (; ;) { };   
                    if (cu > 0)
                    {
                        int[] cyclepath = Path.ToArray();
                        for (int j = 0; j < Path.Count - 1; j++)
                        {
                            _owes[cyclepath[j + 1], cyclepath[j]] -= cu;
                        }
                        _owes[cyclepath[0], cyclepath[Path.Count - 1]] -= cu;
                        Console.WriteLine("Middle owed: ");
                        for (int ii = 0; ii < _usercount; ii++)
                        {
                            for (int j = 0; j < _usercount; j++)
                                Console.Write(_owes[ii, j] + " ");
                            Console.WriteLine();
                        }
                        foundcycle = true;
                    }
                }
            }
        }

        private long CycleUtil(int v, bool[] visited, bool[] stack, Stack<int> Path)
        {
            visited[v] = true;
            stack[v] = true;
            for (int i = 0; i < _usercount; i++)
            {
                Path.Push(i);
                if (_owes[v, i] > 0)
                {
                    if (v == i)
                    { }
                    long cu = 0;
                    bool visitedi = visited[i];
                    if (visitedi == false)
                    {
                        cu = CycleUtil(i, visited, stack, Path);
                    }
                    if (visitedi == false && cu > 0)
                    {
                        return Math.Min(cu, _owes[v, i]);
                    }
                    else if (stack[i])
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
                        return _owes[v, i];
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
            int n = _usercount;
            while (!done)
            {
                Stack<int> Topological = new Stack<int>();
                int[] neighbors = new int[n];
                for (int i = 0; i < n; i++)
                {
                    neighbors[i] = 0;
                    for (int j = 0; j < n; j++)
                    {
                        if (_owes[j, i] > 0)
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
                                if (_owes[i, j] > 0)
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
                //cost[TopologicalArray[0]] = 0;
                for (int i = 0; i < TopologicalArray.Length; i++)
                {
                    for (int j = 0; j < n; j++)
                    {
                        if (_owes[TopologicalArray[i], j] > 0)
                        {
                            if (len[j] < len[TopologicalArray[i]] + 1)
                            {
                                if (cost[TopologicalArray[i]] == -1)
                                {
                                    cost[j] = _owes[TopologicalArray[i], j];
                                }
                                else
                                    cost[j] = Math.Min(cost[TopologicalArray[i]], _owes[TopologicalArray[i], j]);
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
                }
                else
                {
                    int curr = max_ind;
                    while (parent[curr] != -1)
                    {
                        _owes[parent[curr], curr] -= max_save_per;
                        curr = parent[curr];
                    }
                    _owes[curr, max_ind] += max_save_per;
                }
            }
        }
    }
}
