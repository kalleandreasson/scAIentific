using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Frontend.Services
{
    public class SessionService
    {
        public event Action OnLoginStatusChanged;
        private string _jwtToken;
        public void SetToken(string jwt)
        {
            _jwtToken = jwt;
            OnLoginStatusChanged?.Invoke();
        }

        public string GetToken()
        {
            return _jwtToken;
        }

        public void ClearToken()
        {
            _jwtToken = null;
            OnLoginStatusChanged?.Invoke();
        }
    }
}