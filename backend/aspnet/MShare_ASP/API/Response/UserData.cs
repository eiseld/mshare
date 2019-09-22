using System;

namespace MShare_ASP.API.Response
{

    /// <summary>Describes the user' data</summary>
    public class UserData
    {

        /// <summary>Id of the user</summary>
        public long Id { get; set; }

        /// <summary>Max 32 length name of the user</summary>
        public String Name { get; set; }
    }
}
