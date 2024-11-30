using System.Collections.Generic;
using System.Linq;
using WooW.Core.Common.Observer.LogModel;

namespace WooW.SB.AlertsRepository
{
    public class FakeRepository
    {
        private List<WoLog> woLogsCol = new List<WoLog>();

        public FakeRepository()
        {
            woLogsCol.Add(_noLogsResourse1);
            woLogsCol.Add(_noLogsResourse2);
            woLogsCol.Add(_noLogsResourse3);
        }

        public WoLog SearchLog(string logCode)
        {
            return woLogsCol.Where(x => x.CodeLog == logCode).FirstOrDefault();
        }

        private WoLog _noLogsResourse1 = new WoLog()
        {
            CodeLog = "000001",
            Title = "Log1 Prueba Desde repo",
            Details = "dsa",
            UserMessage = "dsaf",
            PossibleSolution = $@"dsafadfs",
            LogType = eLogType.Error,
            FileDetails = new WoFileDetails()
            {
                Path = $@"C:\Frog\WooW.SB\WooW.SB\WooW.SB\Observer\LogObserver\WoLogObserver.cs",
                Class = "WoLogObserver",
                MethodOrContext = "SetCommonLog",
                LineAprox = "39-40, 56"
            }
        };

        private WoLog _noLogsResourse2 = new WoLog()
        {
            CodeLog = "000002",
            Title = "Log2 Prueba Desde repo",
            Details = "dasf",
            UserMessage = "dsafdsa",
            PossibleSolution = $@"dasfds",
            LogType = eLogType.Error,
            FileDetails = new WoFileDetails()
            {
                Path = $@"C:\Frog\WooW.SB\WooW.SB\WooW.SB\Observer\LogObserver\WoLogObserver.cs",
                Class = "WoLogObserver",
                MethodOrContext = "SetCommonLog",
                LineAprox = "39-40, 56"
            }
        };

        private WoLog _noLogsResourse3 = new WoLog()
        {
            CodeLog = "000003",
            Title = "Log3 Prueba Desde repo",
            Details = "dasfsa",
            UserMessage = "dfsadsadf",
            PossibleSolution = $@"adsfdsa",
            LogType = eLogType.Error,
            FileDetails = new WoFileDetails()
            {
                Path = $@"C:\Frog\WooW.SB\WooW.SB\WooW.SB\Observer\LogObserver\WoLogObserver.cs",
                Class = "WoLogObserver",
                MethodOrContext = "SetCommonLog",
                LineAprox = "39-40, 56"
            }
        };
    }
}
