using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SourceRCode
{
    public static class CallRSource
    {
        public static void CallItem(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine(@"Usage:");
                Console.WriteLine(@"SourceRCode.exe c:/path/to/source.r");
                Console.WriteLine(@" (!) Do note that you should use forward slashes (simpler than backslashes here)");
                return;
            }
           
           // REngine.SetEnvironmentVariables();
            //REngine engine = REngine.GetInstance();

           // var abc = engine.Evaluate("source('C:/Users/jdmso/source/MatrixApp/wwwroot/lib/Rcode.R')").AsInteger();
           // System.Diagnostics.Trace.WriteLine(abc);
            /*
             * // Say your R file contains:
                sqr <- function(x) {
                return(x*x)
            }
             */
          //  Console.WriteLine("By default, autoprint on the console");
           // string answer = engine.Evaluate("sqr(0:5)").ToString();

           // Console.WriteLine("However, for manipulation of larger data, autoprint on the console is probably not a good idea");
            //engine.AutoPrint = false;
          //  var a = engine.Evaluate("sqr(0:1000)").AsNumeric().ToArray();

          //  Console.WriteLine("Length(a) is "+a.Length+", but the vector has not been written out to the console");

            /*
            // if testing for http://stackoverflow.com/questions/32236596/calling-user-defined-functions-inside-a-script-in-r-net
            double value1 = 1, value2 = 2;
            var dataframe = engine.Evaluate(string.Format(
                                                        CultureInfo.InvariantCulture, 
                                                        "dataframe <- userDefinedFunctionOne(parameter1 = {0}, parameter2 = {1})",
                                                        value1,
                                                        value2)).AsDataFrame();
             // with
             // userDefinedFunctionOne <- function(parameter1, parameter2) {
             //    return( data.frame(a=parameter1, b=parameter2) )
             // }
            */

            Console.WriteLine("Press any key to exit the program");
            Console.ReadKey();
           // engine.Dispose();
        }
    }
}