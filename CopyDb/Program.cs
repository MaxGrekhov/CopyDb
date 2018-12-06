using System;
using System.IO;
using System.Threading.Tasks;
using CopyDb.Common;
using CopyDb.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CopyDb
{
    class Program
    {
        static async Task<int> Main(string[] args)
        {
            NLog.LogManager.LoadConfiguration("nlog.config");
            var logger = NLog.LogManager.GetCurrentClassLogger();
            try
            {
                if (args?.Length != 1)
                {
                    logger.Error("Wrong number of parameters");
                    return 1;
                }
                if (!File.Exists(args[0]))
                {
                    logger.Error($"Can't find config file: '{args[0]}'");
                    return 1;
                }
                var configuration = new ConfigurationBuilder().AddJsonFile(args[0]).Build();
                var container = new ServiceCollection();
                container.AddTransient(typeof(ILoggerService<>), typeof(LoggerService<>));
                container.AddSingleton<IConfiguration>(configuration);
                container.AddTransient<ICopyService, CopyService>();
                var provider = container.BuildServiceProvider();
                var copyService = provider.GetService<ICopyService>();
                if (!await copyService.Run())
                    return 1;

            }
            catch (Exception e)
            {
                logger.Error(e);
                return 1;
            }
            return 0;
        }
    }
}
