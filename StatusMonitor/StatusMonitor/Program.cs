using System;
using System.Management;

namespace StatusMonitor
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length < 1)
            {
                Console.WriteLine("Usage: ServiceStatusMonitor  ");
                Environment.Exit(0);
            }
            string WMI_EVENT_QUERY = @"SELECT * FROM __InstanceModificationEvent
                WITHIN 1 WHERE TargetInstance ISA 'Win32_Service'";

            string WMI_EVENT_QUERY_WITH_SERVICE_NAME = WMI_EVENT_QUERY
                    + " and TargetInstance.Name = '{0}'";
            WqlEventQuery serviceModificationEvent =
                    new WqlEventQuery(string.Format(WMI_EVENT_QUERY_WITH_SERVICE_NAME, args[0]));
            ManagementEventWatcher eventWatcher =
                    new ManagementEventWatcher(serviceModificationEvent);
            eventWatcher.EventArrived +=
                    new EventArrivedEventHandler(Watcher_EventArrived);
            Console.WriteLine("Waiting for service status change events ...");
            eventWatcher.Start();
            Console.ReadLine();
        }

        static void Watcher_EventArrived(object sender, EventArrivedEventArgs e)
        {
            string eventType = e.NewEvent.ClassPath.ClassName;

            switch (eventType)
            {
                case "__InstanceCreationEvent":

                    Console.BackgroundColor = ConsoleColor.Blue;
                    Console.WriteLine("'{0}' Service created ....",
                            Environment.GetCommandLineArgs()[1]);
                    Console.ResetColor();
                    break;
                case "__InstanceDeletionEvent":

                    Console.BackgroundColor = ConsoleColor.Green;
                    Console.WriteLine("'{0}' Service deleted ....",
                        Environment.GetCommandLineArgs()[1]);
                    Console.ResetColor();
                    break;

                case "__InstanceModificationEvent":

                    Console.BackgroundColor = ConsoleColor.Blue;
                    ManagementBaseObject obj = (ManagementBaseObject)e.NewEvent["TargetInstance"];
                    Console.WriteLine("'{0}' Service Modified ( {1} )",
                        Environment.GetCommandLineArgs()[1], obj["State"]);
                    Console.ResetColor();
                    break;
            }

        }
    }
}
