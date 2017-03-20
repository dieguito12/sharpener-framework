using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PHttp
{
    class HttpEventArgs : EventArgs
    {
        private HttpServerState _previousState;

        private HttpServerState _newState;

        public HttpServerState PreviousState
        {
            get
            {
                return _previousState;
            }
            set
            {
                _previousState = value;
            }
        }

        public HttpServerState NextState
        {
            get
            {
                return _newState;
            }
            set
            {
                _newState = value;
            }
        }

        public HttpEventArgs(HttpServerState previous, HttpServerState next) : base()
        {
            this._newState = next;
            this._previousState = previous;
        }
    }
}
