using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace MShare_ASP.Data {
    /// <summary>
    /// Data Access Object for User
    /// </summary>
    public class DaoUser {
        /// <summary>
        /// Primary key of the user
        /// </summary>
        [Key]
        public long id { get; set; }
        /// <summary>
        /// Email of the user
        /// </summary>
        public String email { get; set; }
        /// <summary>
        /// Hashed password of the user
        /// </summary>
        public String password { get; set; }
        /// <summary>
        /// Displayname (not unique!)
        /// </summary>
        public String display_name { get; set; }
        /// <summary>
        /// Date and time when the user was registered
        /// </summary>
        public DateTime creation_date { get; set; }

        /// <summary>
        /// All email tokens associated with user
        /// </summary>
        public IEnumerable<DaoEmailToken> email_tokens { get; set; }
    }
}