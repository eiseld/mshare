using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IdentityModel.Tokens.Jwt;

namespace MShare_ASP.Services{

    /// <summary>
    /// Authentication related services
    /// </summary>
    public interface IAlgoService {


        bool ReadFromDB();


        bool RemoveCycle();


        bool Optimize();

        bool SaveResults();

    }
}
