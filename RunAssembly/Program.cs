using System;
using System.CodeDom.Compiler;
using Microsoft.CSharp;

namespace RunAssembly
{
    public class Program
    {
        static int Main(string[] args)
        {
            if (args.Length != 2)
            {
                Console.WriteLine("Usage: RunAssembly <assemblyname> <statement>");
                Console.WriteLine("    enclose statement in quotes, use fully qualified names for all types");
                return 0;
            }
            CSharpCodeProvider provider = new CSharpCodeProvider();

            var fixquotes = args[1].Replace("\\\"", "\"");

            var strings = new[]
                {   
                    "public class Program { public void Main() { ", 
                    fixquotes, 
                    "; } }"                  
                };

            var assembly = provider.CompileAssemblyFromSource(new CompilerParameters(new string[] { args[0]}, "RunDllAssembly"), strings);

            if (assembly.Errors.HasErrors)
            {
                foreach (var line in assembly.Output)
                {
                    Console.WriteLine(line);
                }
                return -2;
            }

            try
            {
                var instance = assembly.CompiledAssembly.CreateInstance("Program");
                var type = instance.GetType();
                var method = type.GetMethod("Main");

                var retval = method.Invoke(instance, new object[0]);
                return 0;
            }
            catch
            {
                return -1;
            }
        }
    }
}
