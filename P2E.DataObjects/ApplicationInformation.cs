using System.Reflection;
using P2E.Interfaces.DataObjects;

namespace P2E.DataObjects
{
    public class ApplicationInformation : IApplicationInformation
    {
        public string Name { get; }
        public string Version { get; }

        public ApplicationInformation()
        {
            Name = Assembly.GetEntryAssembly().GetName().Name;
            Version = Assembly.GetEntryAssembly().GetName().Version.ToString();
        }
    }
}