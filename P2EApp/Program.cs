using System;
using CommandLine;
using Ninject;
using P2E.Interfaces.AppLogic;
using P2E.Interfaces.CommandLine;

namespace P2EApp
{
    class Program
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

                kernel.Get<ILogic>().Run();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            finally
            {
                // todo - remove the whole finally block
                Console.ReadLine();
            }

        }
    }
}
