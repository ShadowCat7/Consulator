using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Consulator
{
    public static class Error
    {
        private static string _message;
        private static int _location;
        private static bool _isRaised;
        public static bool isRaised { get { return _isRaised; } }
        //TODO What about an option to list all possible errors? (Could get messy).

        public static void raise(string message)
        {
            if (!_isRaised)
            {
                _message = message;
                _location = -1;
                _isRaised = true;
            }
        }
        public static void raise(int location, string message)
        {
            if (!_isRaised)
            {
                _location = location;
                _message = message;
                _isRaised = true;
            }
        }

        public static string get()
        {
            string output = "";

            for (int i = 0; i < _location; ++i)
            { output += " "; }

            if (_location != -1)
            { output = "    " + output + '^'; }

            output += '\n' + _message;

            _isRaised = false;

            return output;
        }
    }
}
