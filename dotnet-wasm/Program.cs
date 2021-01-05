using System;
using System.IO;
using System.Text;
using Wasmtime;

namespace DotnetWasm
{
    class Program
    {
        static string _wasmdir = Path.GetFullPath("../../../wasm/");

        static void Main(string[] args)
        {
            using (var engine = new Engine())
            {
                using (var module = Module.FromBytes(engine, "main", File.ReadAllBytes(Path.Combine(_wasmdir, "main.wasm"))))
                { 
                    using (var host = new Host(engine))
                    {
                        using (dynamic instance = host.Instantiate(module))
                        {
                            Console.WriteLine("Calling methods exported by WebAssembly...");

                            Console.WriteLine($"main: {instance.main()}");
                            Console.WriteLine($"add: {instance.add(7, 6)}");

                            var pointer = instance.greet();
                            var memory = instance.Externs.Memories[0];
                            var greetMessage = memory.ReadNullTerminatedString(pointer);
                            Console.WriteLine($"greet: {greetMessage}");
                        }
                    }
                }
            }
        }
    }
}
