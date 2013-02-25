using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ares.GlobalDB.StandaloneHost
{
    public class AppHost : ServiceStack.WebHost.Endpoints.AppHostHttpListenerBase
    {
        public AppHost()
            : base("RPGMusicTags HttpListener", typeof(Ares.GlobalDB.Services.DownloadService).Assembly)
        {
        }

        public override void Configure(Funq.Container container)
        {
            Routes
                .Add<Ares.GlobalDB.Services.Download>("/Download")
                .Add<Ares.GlobalDB.Services.Upload>("/Upload");
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var listeningOn = args.Length == 0 ? "http://*:1337/" : args[0];
            var appHost = new AppHost();
            appHost.Init();
            appHost.Start(listeningOn);

            Console.WriteLine("AppHost created at {0}, listening on {1}", DateTime.Now, listeningOn);
            Console.ReadKey();
        }
    }
}
