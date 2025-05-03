using DbUp.Engine;
using DbUp.Helpers;
using DbUp;
using Microsoft.Extensions.Logging;
using System.Reflection;

namespace HrSystem.Shared.SQLScripts;


    public class Deploy
    {
        public static void Upgrade(string connectionString, ILogger log)
        {
            LogMessage($@"Start Deploying changes on {connectionString}", log);

            DeployEverytimeScripts(connectionString, log);
            DeployOnetimeScripts(connectionString, log);

        }
        public static void UpgradeFunctions(string connectionString, ILogger log)
        {
            DeployEverytimeFunctionsScripts(connectionString, log);
        }

        private static void DeployEverytimeFunctionsScripts(string connectionString, ILogger log)
        {
            LogMessage("Start Deploying every time functions scripts", log);

            var upgrader = DeployChanges.To
                           .SqlDatabase(connectionString)
                           .WithScriptsEmbeddedInAssembly(
                           Assembly.GetExecutingAssembly(),
                           s =>
                           {
                               return s.ToLower().Contains("01_Functions".ToLower());
                           })
                          .LogToConsole()
                          .JournalTo(new NullJournal())
                          .Build();
            RunUpgrader(upgrader, log);
        }

        private static void DeployOnetimeScripts(string connectionString, ILogger log)
        {
            LogMessage("Start Deploying one time scripts (Seeding Data And Others)", log);

            var upgrader =
                DeployChanges.To
                    .SqlDatabase(connectionString)
                    .WithScriptsEmbeddedInAssembly(Assembly.GetExecutingAssembly(),
                     s =>
                     {
                         return s.ToLower().Contains("OneTime".ToLower());
                     })
                    .LogToConsole()
                    .Build();
            RunUpgrader(upgrader, log);
        }

        private static void DeployEverytimeScripts(string connectionString,ILogger log)
        {
            LogMessage("Start Deploying every time scripts (BSFunctions, Views And SPs)", log);

            var upgrader = DeployChanges.To
                                      .SqlDatabase(connectionString)
                                      .WithScriptsEmbeddedInAssembly(
                                      Assembly.GetExecutingAssembly(),
                                      s =>
                                      {
                                          return s.ToLower().Contains("everytime".ToLower()) && !s.ToLower().Contains("01_Functions".ToLower());
                                      })
                                     .LogToConsole()
                                     .JournalTo(new NullJournal())
                                     .Build();
            RunUpgrader(upgrader, log);
        }

        private static void RunUpgrader(UpgradeEngine upgrader, ILogger log)
        {
            var result = upgrader.PerformUpgrade();

            if (!result.Successful)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                LogMessage(result.Error.Message, log);
                Console.ResetColor();
                // throw result.Error;                
            }

            Console.ForegroundColor = ConsoleColor.Green;
            LogMessage($@"Success ......", log);
            Console.ResetColor();
        }

    private static void LogMessage(string message, ILogger log)
        {
            Console.WriteLine(message);
            log.LogInformation(message);
        }
    }
