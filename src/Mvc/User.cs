using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mvc
{
    /// <summary>
    /// Representative class of an application basic user.
    /// </summary>
    public class User
    {
        /// <summary>
        /// Username of the user
        /// </summary>
        string _username;

        /// <summary>
        /// Password of the user
        /// </summary>
        string _password;

        /// <summary>
        /// Authentication token of the user.
        /// </summary>
        string _token;

        /// <summary>
        /// Constructor of the User class
        /// </summary>
        /// <param name="username">Username of the user</param>
        /// <param name="password">Password of the user</param>
        public User(string username, string password)
        {
            _username = username;
            _password = password;
        }

        /// <summary>
        /// Gets and sets the username of the user
        /// </summary>
        public string Username
        {
            get
            {
                return _username;
            }
            set
            {
                _username = value;
            }
        }

        /// <summary>
        /// Gets and sets the password of the user
        /// </summary>
        public string Password
        {
            get
            {
                return _password;
            }
            set
            {
                _password = value;
            }
        }

        /// <summary>
        /// Gets the authentication token of the user.
        /// </summary>
        public string Token
        {
            get
            {
                return _token;
            }
            internal set
            {
                _token = value;
            }
        }

    }
}
