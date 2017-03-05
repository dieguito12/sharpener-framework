using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PHttp.Application;

/// <summary>
/// Mvc namespace of the Sharpener Framework.
/// </summary>
namespace Mvc
{
    /// <summary>
    /// Representative class of a web application.</summary>
    /// <remarks>
    /// Implements the IPHttpApplication interface.</remarks>
    public class Application : IPHttpApplication
    {
        /// <summary>
        /// Name of the application.
        /// </summary>
        private string name;

        /// <summary>
        /// Gets or sets the name of the application.</summary>
        /// <value>
        /// String value.</value>
        public string Name
        {
            get
            {
                return name;
            }

            set
            {
                name = value;
            }
        }

        /// <summary>
        /// Event fired before the application starts.
        /// </summary>
        public event PreApplicationStartMethod PreApplicationStart;

        /// <summary>
        /// Event fired when the application starts.
        /// </summary>
        public event ApplicationStartMethod ApplicationStart;

        /// <summary>
        /// Launches the application.
        /// </summary>
        public void Start()
        {
            Console.WriteLine("Start Method");
        }

        /// <summary>
        /// Execute some action.
        /// </summary>
        public void ExecuteAction()
        {
            Console.WriteLine("ExecuteAction Method");
        }

        /// <summary>
        /// Test method of the application.
        /// </summary>
        public void Test()
        {
            Console.WriteLine("This is a Test Method");
        }
    }
}
