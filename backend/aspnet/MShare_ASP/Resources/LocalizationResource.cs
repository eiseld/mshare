using Microsoft.Extensions.Localization;
using MShare_ASP.Data;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

// IMPORTANT NOTE HERE: Every sharedresource should be in the root namespace of the project!!!
namespace MShare_ASP
{
    /// <summary>Contains language keys for the application</summary>
    public class LocalizationResource
    {
        /// <summary>Team name (similar to "MShare - Money share team")</summary>
        public static string MSHARE_TEAM { get; } = nameof(MSHARE_TEAM);

        /// <summary>Cheers for forgotten password email</summary>
        public static string EMAIL_CASUAL_BODY_CHEERS { get; } = nameof(EMAIL_CASUAL_BODY_CHEERS);
        /// <summary>Forgotten password email's footer, should contain the raw link</summary>
        public static string EMAIL_FOOTER_BADBUTTON { get; } = nameof(EMAIL_FOOTER_BADBUTTON);

        /// <summary>For the button of the forgotten password email</summary>
        public static string EMAIL_FORGOTPSW_BODY_BUTTON { get; } = nameof(EMAIL_FORGOTPSW_BODY_BUTTON);
        /// <summary>You requested the forgotten email or didn't you</summary>
        public static string EMAIL_FORGOTPSW_BODY_DISCLAIMER { get; } = nameof(EMAIL_FORGOTPSW_BODY_DISCLAIMER);
        /// <summary>Greetings for casual email (hello {0}, hi {0}, etc)</summary>
        public static string EMAIL_CASUAL_BODY_GREETING { get; } = nameof(EMAIL_CASUAL_BODY_GREETING);
        /// <summary>Introduction part of the forgotten password email</summary>
        public static string EMAIL_FORGOTPSW_BODY_INTRO { get; } = nameof(EMAIL_FORGOTPSW_BODY_INTRO);
        /// <summary>Subject of forgotten password email</summary>
        public static string EMAIL_FORGOTPSW_SUBJECT { get; } = nameof(EMAIL_FORGOTPSW_SUBJECT);
        /// <summary>PreHeader (after email subject) for forgotten password</summary>
        public static string EMAIL_FORGOTPSW_PREHEADER { get; } = nameof(EMAIL_FORGOTPSW_PREHEADER);
        /// <summary>Hero of forgotten password email</summary>
        public static string EMAIL_FORGOTPSW_HERO { get; } = nameof(EMAIL_FORGOTPSW_HERO);

        /// <summary>Subject of the register validation email</summary>
        public static string EMAIL_REGISTER_BODY_BUTTON { get; } = nameof(EMAIL_REGISTER_BODY_BUTTON);
        /// <summary>Disclaimer that you requested the password reset (else you should ignore it)</summary>
        public static string EMAIL_REGISTER_SUBJECT { get; } = nameof(EMAIL_REGISTER_SUBJECT);
        /// <summary>PreHeader (after email subject) for register validation</summary>
        public static string EMAIL_REGISTER_PREHEADER { get; } = nameof(EMAIL_REGISTER_PREHEADER);
        /// <summary>Hero of register validation password email</summary>
        public static string EMAIL_REGISTER_HERO { get; } = nameof(EMAIL_REGISTER_HERO);
        /// <summary>Introduction part of the register validation email</summary>
        public static string EMAIL_REGISTER_BODY_INTRO { get; } = nameof(EMAIL_REGISTER_BODY_INTRO);
        /// <summary>You requested the forgotten email or didn't you</summary>
        public static string EMAIL_REGISTER_BODY_DISCLAIMER { get; } = nameof(EMAIL_REGISTER_BODY_DISCLAIMER);
        /// <summary>Subject of the email one receives when they are added to a group</summary>
        public static string EMAIL_ADDEDTOGROUP_SUBJECT { get; } = nameof(EMAIL_ADDEDTOGROUP_SUBJECT);
        /// <summary>Preheader of the email one receives when they are added to a group</summary>
        public static string EMAIL_ADDEDTOGROUP_PREHEADER { get; } = nameof(EMAIL_ADDEDTOGROUP_PREHEADER);
        /// <summary>Hero of the email one receives when they are added to a group</summary>
        public static string EMAIL_ADDEDTOGROUP_HERO { get; } = nameof(EMAIL_ADDEDTOGROUP_HERO);
        /// <summary>
        /// Introduction of the email one receives when they are added to a group, has two parameters 
        /// {0}: inviter's name
        /// {1}: group name
        /// </summary>
        public static string EMAIL_ADDEDTOGROUP_BODY_INTRO { get; } = nameof(EMAIL_ADDEDTOGROUP_BODY_INTRO);
        /// <summary>Why one got the email (when they are added to a group)</summary>
        public static string EMAIL_ADDEDTOGROUP_BODY_DISCLAIMER { get; } = nameof(EMAIL_ADDEDTOGROUP_BODY_DISCLAIMER);
        /// <summary>Subject of the email that you get when your password has been changed</summary>
        public static string EMAIL_PASSWORDCHANGED_SUBJECT { get; } = nameof(EMAIL_PASSWORDCHANGED_SUBJECT);
        /// <summary>Preheader of the email that you get when your password has been changed</summary>
        public static string EMAIL_PASSWORDCHANGED_PREHEADER { get; } = nameof(EMAIL_PASSWORDCHANGED_PREHEADER);
        /// <summary>Hero of the email that you get when your password has been changed</summary>
        public static string EMAIL_PASSWORDCHANGED_HERO { get; } = nameof(EMAIL_PASSWORDCHANGED_HERO);
        /// <summary>Introduction of the email that you get when your password has been changed</summary>
        public static string EMAIL_PASSWORDCHANGED_BODY_INTRO { get; } = nameof(EMAIL_PASSWORDCHANGED_BODY_INTRO);
        /// <summary>Why one got the email (when their password has been changed)</summary>
        public static string EMAIL_PASSWORDCHANGED_BODY_DISCLAIMER { get; } = nameof(EMAIL_PASSWORDCHANGED_BODY_DISCLAIMER);
    }
    /// <summary>Extension class for IStringLocalizer</summary>
    public static class IStringLocalizerExtensions
    {
        /// <summary>Returns a localized string from a language</summary>
        /// <param name="localizer">Extension's localizer</param>
        /// <param name="language">Language in which to return the key in</param>
        /// <param name="name">Name of the key</param>
        /// <param name="arguments">Arguments for the value</param>
        public static string GetString(this IStringLocalizer localizer, DaoLangTypes.Type language, string name, params object[] arguments)
        {
            return localizer.WithCulture(new CultureInfo(language.ToString())).GetString(name, arguments);
        }
    }
}
