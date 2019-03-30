using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MShare_ASP.API.Response {
    /// <summary>
    /// Describes a group's data
    /// </summary>
    public class GroupData {
        /// <summary>
        /// Id of the group
        /// </summary>
        public long Id { get; set; }
        /// <summary>
        /// Name of the group
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Creator of this group
        /// </summary>
        public UserData CreatorUser { get; set; }
        /// <summary>
        /// Members of this group
        /// </summary>
        public IList<UserData> Members { get; set; }
        /// <summary>
        /// Calculated membercount based on Members property
        /// </summary>
        public int? MemberCount { get { return Members?.Count; } }
        /// <summary>
        /// Balance of the group
        /// </summary>
        public int Balance { get; set; }
    }
}
