using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Frontend.Services
{
    public class SessionService
    {
        private string _jwtToken;
        public void SetToken(string jwt)
        {
            _jwtToken = jwt;
        }

        public string GetToken()
        {
            return _jwtToken;
        }
    }
}