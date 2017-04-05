using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mvc
{
    public class User
    {
        string _username;

        string _password;

        string _token;

        public User(string username, string password)
        {
            _username = username;
            _password = password;
        }

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
