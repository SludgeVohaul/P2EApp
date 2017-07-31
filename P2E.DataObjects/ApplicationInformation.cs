using System.Reflection;
using P2E.Interfaces.DataObjects;

namespace P2E.DataObjects
{
    public class ApplicationInformation : IApplicationInformation
    {
        public string Name => Assembly.GetEntryAssembly().GetName().Name;
        public string Version => Assembly.GetEntryAssembly().GetName().Version.ToString();
    }
}