using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.List
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MShare_ASP.API.Request;
using MShare_ASP.Data;
using MShare_ASP.Utils;

namespace MShare_ASP.Services {
    internal class AlgoService : IAlgoService {

        private Configurations.IJWTConfiguration _jwtConf;
        private Configurations.IURIConfiguration _uriConf;
        private IEmailService _emailService;
        private ITimeService _timeService;
        private MshareDbContext _context;
        private Random _random;
        private int[,] _owes;
        private int _groupcount;

        public AlgoService(MshareDbContext context, IEmailService emailService, ITimeService timeService, Configurations.IJWTConfiguration jwtConf, Configurations.IURIConfiguration uriConf) {
            _emailService = emailService;
            _timeService = timeService;
            _context = context;
            _jwtConf = jwtConf;
            _uriConf = uriConf;
            _random = new Random();
        }

        public bool Optimize()
        {
            ReadFromDB();
            RemoveCycle();
            ReduceTransfers();
            SaveResults();
            return true;
        }

        public bool ReadFromDB()
        {
            return true;
        }

        public bool RemoveCycle()
        {
            int n = _groupcount;
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
                Stack<int> Path;
                for(int i = 0; i < n && !foundcycle; i++)
                {
                    Path.Clear();
                    Path.Push(i);
                    int cu = CycleUtil(i,visited,stack);
                    if(cu > 0)
                    {
                        int[] cyclepath = Path.ToArray();
                        for(int j = 0; j < Path.Count - 1; j++)
                        {
                            _owes(cyclepath[j],cyclepath[j+1]) -= cu;
                        }
                        _owes(cyclepath[Path.Count-1],cyclepath[0]) -= cu;
                        foundcycle = true;
                    }
                }
            }
            return true;
        }

        private int CycleUtil(int v, bool[] visited, bool[] stack)
        {
            visited[v] = true;
            stack[v] = true;
            for(int i = 0; i < _groupcount; i++)
            {
                Path.Push(i);
                if(_owes(i,j)>0)
                {
                    int cu = 0;
                    if(visited[i] == false)
                    {
                         cu = CycleUtil(i,visited,stack);
                    }
                    if(visited[i] == false && cu > 0)
                    {
                        return Math.Min(cu,_owes(i,j));
                    }
                    else if (stack[i])
                    {
                        Path.Pop();
                        return _owes(i,j);
                    }
                }
                Path.Pop();
            }
            stack[v] = false;
            return 0;
        }

        public bool ReduceTransfers()
        {
            bool done = false;
            int n = _groupcount;
            while(!done)
            {
                Stack<int> Topological;
                int[] neighbors = new int[n];
                for(int i = 0; i < n; i++)
                {
                    neighbors[i] = 0;
                    for(int j = 0; j < n; j++)
                    {
                        if(_owes[i,j] > 0)
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
                int[] cost = new int[n];
                int[] len = new int[n];
                int[] parent = new int[n];
                for(int i = 0; i < n; i++)
                {
                    cost[i] = -1;
                    parent[i] = -1;
                    len[i] = 0;
                }
                cost[TopologicalArray[0]] = 0;
                for(int i = 0; i < TopologicalArray.Count; i++)
                {
                    for(int j = 0; j < n; j++)
                    {
                        if(_owes[TopologicalArray[i],j] > 0)
                        {
                            if(cost[j] * len[j] < Math.Min(cost[TopologicalArray[i]],_owes[TopologicalArray[i],j]) * (len[TopologicalArray[i]] + 1))
                            {
                                cost[j] = Math.Min(cost[TopologicalArray[i]],_owes[TopologicalArray[i],j]);
                                len[j] = len[TopologicalArray[i]] + 1;
                                parent[j] = TopologicalArray[i];
                            }
                        }
                    }
                }
                int max_save = 0;
                int max_save_per = 0;
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
                if(max_ind = -1)
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
            return true;
        }

        public bool SaveResults()
        {
            return true;
        }

    }
}
