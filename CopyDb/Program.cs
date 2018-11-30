using System;
using System.Data;
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
        static async Task Main(string[] args)
        {
            Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;
            Dapper.SqlMapper.AddTypeMap(typeof(DateTime), DbType.DateTime2);
            NLog.LogManager.LoadConfiguration("nlog.config");
            var logger = NLog.LogManager.GetCurrentClassLogger();
            try
            {
                if (args?.Length != 1)
                {
                    logger.Error("Wrong number of parameters");
                    return;
                }
                if (!File.Exists(args[0]))
                {
                    logger.Error($"Can't find config file: '{args[0]}'");
                    return;
                }
                var configuration = new ConfigurationBuilder().AddJsonFile(args[0]).Build();
                var container = new ServiceCollection();
                container.AddTransient(typeof(ILoggerService<>), typeof(LoggerService<>));
                container.AddSingleton<IConfiguration>(configuration);
                container.AddTransient<ICopyService, CopyService>();
                var provider = container.BuildServiceProvider();
                var copyService = provider.GetService<ICopyService>();
                await copyService.Run();

            }
            catch (Exception e)
            {
                logger.Error(e);
                throw;
            }
        }
    }
}
