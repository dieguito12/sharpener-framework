using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PHttp
{
    public class HttpSite
    {
        private string _name;

        private string _virtualPath;

        private string _physicalPath;

        private bool _directoryBrowsing;

        private List<string> _defaultDocuments;

        private Dictionary<int, string> _errorPages;

        public HttpSite(string name, string virtualPath, string physicalPath, bool directoryBrowsing, List<string> defaultDocuments, Dictionary<int, string> errorPages)
        {
            Name = name;
            VirtualPath = virtualPath;
            PhysicalPath = physicalPath;
            DirectoryBrowsing = directoryBrowsing;
            DefaultDocuments = defaultDocuments;
            ErrorPages = errorPages;
        }

        public string Name
        {
            get
            {
                return _name;
            }
            private set
            {
                _name = value;
            }
        }

        public string VirtualPath
        {
            get
            {
                return _virtualPath;
            }
            private set
            {
                _virtualPath = value;
            }
        }

        public string PhysicalPath
        {
            get
            {
                return _physicalPath;
            }
            private set
            {
                _physicalPath = value;
            }
        }

        public bool DirectoryBrowsing
        {
            get
            {
                return _directoryBrowsing;
            }
            private set
            {
                _directoryBrowsing = value;
            }
        }

        public List<string> DefaultDocuments
        {
            get
            {
                return _defaultDocuments;
            }
            private set
            {
                _defaultDocuments = value;
            }
        }

        public Dictionary<int, string> ErrorPages
        {
            get
            {
                return _errorPages;
            }
            private set
            {
                _errorPages = value;
            }
        }

        public void LoadApp()
        {
            
        }
    }
}
