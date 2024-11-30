using ActiproSoftware.Text.Languages.DotNet.Reflection;

namespace WooW.SB.Helpers
{
    public class AssemblyHelper
    {
        private static volatile AssemblyHelper uniqueInstance = null;

        private static readonly object padlock = new object();

        private IProjectAssembly _projectAssembly = null;
        public IProjectAssembly projectAssembly
        {
            get => _projectAssembly;
            set => _projectAssembly = value;
        }

        public static AssemblyHelper getInstance()
        {
            if (uniqueInstance == null)
            {
                lock (padlock)
                {
                    if (uniqueInstance == null)
                    {
                        uniqueInstance = new AssemblyHelper();
                    }
                }
            }
            return uniqueInstance;
        }

        public AssemblyHelper() { }
    }
}
