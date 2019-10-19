namespace MShare_ASP.API.Response
{
    /// <summary>The user data that can be displayed to anybody</summary>
    public class FilteredUserData
    {
        /// <summary>Id of the user</summary>
        public long Id { get; set; }

        /// <summary>DisplayName of the user</summary>
        public string DisplayName { get; set; }

        /// <summary>Email of the user</summary>
        public string Email { get; set; }
    }
}