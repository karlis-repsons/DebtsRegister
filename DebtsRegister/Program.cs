using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace DebtsRegister
{
    using Startup = Initialization.Simplified.SimplifiedWebApiStartup;

    public class Program
    {
        public static void Main(string[] args) {
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .Build()
                .Run();
        }  
    }
}