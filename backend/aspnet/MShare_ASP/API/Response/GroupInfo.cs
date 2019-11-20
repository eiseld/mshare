using System;

namespace MShare_ASP.API.Response
{
    /// <summary>Describes a group's minimal data</summary>
    public class GroupInfo
    {
        /// <summary>Id of the group</summary>
        public long Id { get; set; }

        /// <summary>Name of the group</summary>
        public string Name { get; set; }

        /// <summary>Name of the creator</summary>
        public string Creator { get; set; }

        /// <summary>Number of members</summary>
        public int MemberCount { get; set; }

        /// <summary>User balance in the view of the group</summary>
        public long MyCurrentBalance { get; set; }

        /// <summary>Last group change that affacted the user</summary>
        public DateTime LastModified { get; set; }
    }
}