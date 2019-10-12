using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MShare_ASP.Data
{
    /// <summary>Data Access Object for Languages</summary>
    public class DaoLangTypes
    {
        /// <summary>Primary key of this language</summary>
        [Key]
        [Column("id")]
        public short Id { get; set; }

        /// <summary>2 character long notation of the language (e.g.: en, hu, de)</summary>
        [Column("lang_name")]
        public string LangName { get; set; }

        /// <summary>All possible languages this app supports</summary>
        public enum Type
        {
            /// <summary>English</summary>
            EN = 1,

            /// <summary>Hungarian</summary>
            HU = 2
        }
    }
}