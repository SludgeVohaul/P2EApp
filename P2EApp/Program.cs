using System;
using System.Linq;
using CommandLine;
using Ninject;
using P2E.ExtensionMethods;
using P2E.Interfaces.AppLogic;
using P2E.Interfaces.CommandLine;

namespace P2EApp
{
    internal static class Program
    {
        static void Main(string[] args)
        {
            try
            {
                var kernel = new StandardKernel();
                Bootstrapper.ConfigureContainer(kernel);

                var options = kernel.Get<IConsoleOptions>();
                if (Parser.Default.ParseArguments(args, options) == false)
                {
                    Console.WriteLine(options.GetUsage());
                    return;
                }

                kernel.Get<IMainLogic>().RunAsync().Wait();
            }
            catch (AggregateException ae)
            {
                var messages = ae.Flatten().InnerExceptions
                    .ToList()
                    .Select(e => e.GetInnermostException())
                    .Select(e => (object)e.Message)
                    .ToArray();

                Console.WriteLine("Oops, you did something I didn't think of:\n{0}", messages);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Oops, you did something I didn't think of:\n{0}", ex.Message);
            }
            finally
            {
                // todo - remove the whole finally block
                Console.ReadLine();
            }
        }
    }
}
