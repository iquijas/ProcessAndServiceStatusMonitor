using System;
using System.Management;

namespace StatusMonitor
{
    class Program
    {
        static void Main(string[] args)
        {
            string WMI_EVENT_QUERY ="";

            if (args.Length < 1)
            {
                Console.WriteLine("Usage: StatusMonitor -s|-p servicename|processname");
                Environment.Exit(0);
            }
            switch (args[0])
            {
                case "-s": WMI_EVENT_QUERY = @"SELECT * FROM __InstanceModificationEvent WITHIN 1 WHERE TargetInstance ISA 'Win32_Service' and TargetInstance.Name = '{0}'";
                    break;
                case "-p": WMI_EVENT_QUERY = @"Select * FROM win32_ProcessTrace where processname = '{0}'";
                    break;
                default:
                    Console.WriteLine("Usage: StatusMonitor -s|-p servicename|processname");
                    Environment.Exit(0);
                    break;
            }
            
            WqlEventQuery serviceModificationEvent =
                    new WqlEventQuery(string.Format(WMI_EVENT_QUERY, args[1]));
            ManagementEventWatcher eventWatcher =
                    new ManagementEventWatcher(serviceModificationEvent);
            eventWatcher.EventArrived +=
                    new EventArrivedEventHandler(Watcher_EventArrived);
            Console.WriteLine("Waiting for status change events ...");
            eventWatcher.Start();
            Console.ReadLine();
        }

        static void Watcher_EventArrived(object sender, EventArrivedEventArgs e)
        {
            string eventType = e.NewEvent.ClassPath.ClassName;

            Console.BackgroundColor = ConsoleColor.Green;
            Console.WriteLine("{0} " + eventType,
                            Environment.GetCommandLineArgs()[2]);
            Console.ResetColor();
        }
    }
}
