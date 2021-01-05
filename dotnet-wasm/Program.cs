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

        #region Test code

        static string _watdir = Path.GetFullPath("../../../wat/");

        static void MemoryExample()
        {
            using (var engine = new Engine())
            {
                using (var module = Module.FromTextFile(engine, Path.Combine(_watdir, "memory.wat")))
                {
                    using (var host = new Host(engine))
                    {
                        using (var function = host.DefineFunction("", "log", (Caller caller, int address, int length) =>
                            {
                                var message = caller.GetMemory("mem").ReadString(address, length);
                                Console.WriteLine($"Message from WebAssembly: {message}");
                            }
                        ))
                        {
                            using (dynamic instance = host.Instantiate(module))
                            {
                                instance.run();
                            }
                        }
                    }
                }
            }
        }

        static void WasmTest()
        {
            using var engine = new EngineBuilder()
                .WithReferenceTypes(true)
                .Build();

            using (var module = Module.FromTextFile(engine, Path.Combine(_watdir, "externref.wat")))
            {
                using( var host = new Host(engine))
                {
                    using (var function = host.DefineFunction("", "concat", (string a, string b) => $"{a} {b}"))
                    {
                        using dynamic instance = host.Instantiate(module);
                        Console.WriteLine(instance.run("Hello", "world!"));
                    }
                }
            }
        }

        static void WasmHello()
        {
            string wat = "(module (func $hello (import \"\" \"hello\")) (func (export \"run\") (call $hello)))";

            using (var engine = new Engine())
            {
                using (var module = Module.FromText(engine, "hello", wat))
                {
                    using (var host = new Host(engine))
                    {
                        using (var function = host.DefineFunction("", "hello", () => Console.WriteLine("Hello from C#!")))
                        {
                            using (dynamic instance = host.Instantiate(module))
                            {
                                instance.run();
                            }
                        }
                    }
                }
            }
        }

        static void TestGreet()
        {

            using (var engine = new EngineBuilder().WithReferenceTypes(true).Build())
            {
                using (var module = Module.FromTextFile(engine, Path.Combine(_watdir, "main.wat")))
                {
                    var mod = Module.FromBytes(engine, "jktest", System.IO.File.ReadAllBytes(Path.Combine(_wasmdir, "main.wasm")));

                    using (var host = new Host(engine))
                    {

                        //using dynamic inst = host.Instantiate(mod);
                        //var res = inst.greet();
                        //Console.WriteLine(res);
                        //var memory = inst.Externs.Memories[0];
                        //var r = memory.ReadInt32(0);

                        //var test1 = memory.ReadInt32(1024);
                        //var test11 = Encoding.UTF8.GetString(memory.Span.ToArray());

                        //var slice = Span<byte>.Slice(1024);
                        //var terminator = slice.IndexOf((byte)0);
                        //if (terminator == -1)
                        //{
                        //    throw new InvalidOperationException("string is not null terminated");
                        //}

                        //var text = Encoding.UTF8.GetString(slice.Slice(0, terminator));

                        //var test2 = memory.ReadNullTerminatedString(memory.ReadInt32(1024));

                        using (var function = host.DefineFunction("", "add", (Caller caller, int address) =>
                        {
                            var message = caller.GetMemory("memory").ReadString(address, 10);
                            Console.WriteLine($"Message from WebAssembly: {message}");
                        }))
                        {
                            var memCount = module.Exports.Memories.Count;
                            using dynamic instance = host.Instantiate(module);
                            Console.WriteLine("Calling WebAssembly greet method...");
                            var result = instance.greet();

                            Console.WriteLine(result);
                        }
                    }
                }
            }
        }

        #endregion
    }
}
