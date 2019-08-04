//using Microsoft.CodeAnalysis;
//using Microsoft.CodeAnalysis.CSharp;
//using Microsoft.CodeAnalysis.Emit;
//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Linq;
//using System.Reflection;

//namespace ant.core.csscript.test
//{ 

//class Program
//    {
//        static void Main(string[] args)
//        {
//            var source = @"
//                    using System;
//                    namespace Sample 
//                    { 
//                        public class Program 
//                        { 
//                           public static void Main()
//                            { 
//                                //Console.WriteLine(""hello, world""); 
//                            } 
//                        } 
//                    }";

//            //var a = GetAssemblyFromSourceByCodeDom(source);
//            var b = GetAssemblyFromSourceByRoslyn(source);

//            //var methodA = a.CreateInstance("Sample.Program").GetType().GetMethod("Main");
//            var methodB = b.CreateInstance("Sample.Program").GetType().GetMethod("Main");
//            //methodA.Invoke(null, null);
//            methodB.Invoke(null, null);
//            //RunStringCodeByRoslyn(@"Console.WriteLine(""hello, world"");");
//            Console.ReadKey();
//        }

//        //static Assembly GetAssemblyFromSourceByCodeDom(params string[] source)
//        //{
//        //    using var provider = CodeDomProvider.CreateProvider(
//        //                                                "CSharp",
//        //                                                new Dictionary<string, string> {
//        //                                                    { "CompilerVersion", "v4.0" }});

//        //    var compileResult = provider.CompileAssemblyFromSource(
//        //                                                new CompilerParameters()
//        //                                                {
//        //                                                    IncludeDebugInformation = false,
//        //                                                    TreatWarningsAsErrors = true,
//        //                                                    WarningLevel = 4,
//        //                                                    GenerateExecutable = false,
//        //                                                    GenerateInMemory = true
//        //                                                },
//        //                                                source);

//        //    if (compileResult.Errors.Count > 0)
//        //    {
//        //        throw new ArgumentException();
//        //    }

//        //    return compileResult.CompiledAssembly;
//        //}

//        static Assembly GetAssemblyFromSourceByRoslyn(string source)
//        {
//            var compilation = CSharpCompilation.Create(
//                                                    null,
//                                                    new[] { CSharpSyntaxTree.ParseText(source) },
//                                                    new[] { MetadataReference.CreateFromFile(typeof(object).Assembly.Location) },
//                                                    new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

//            using (var memSteam = new MemoryStream())
//            {
//                var emitResult = compilation.Emit(memSteam);

//                if (!emitResult.Success || emitResult.Diagnostics.FirstOrDefault(x => x.Severity > 0) != null)
//                {
//                    throw new ArgumentException();
//                }

//                memSteam.Seek(0, SeekOrigin.Begin);

//                return Assembly.Load(memSteam.ToArray());
//            };
//        }

//        //static void RunStringCodeByRoslyn(string source)
//        //{
//        //    _ = CSharpScript.RunAsync(source,
//        //                                 ScriptOptions.Default
//        //                                         .AddReferences(new[] { MetadataReference.CreateFromFile(typeof(object).Assembly.Location) })
//        //                                         .AddImports("System")
//        //                                         .WithLanguageVersion(LanguageVersion.CSharp8)
//        //                                         .WithOptimizationLevel(OptimizationLevel.Release)
//        //                                         .WithEmitDebugInformation(false)
//        //                                         .WithWarningLevel(4)
//        //                                         ).Result;
//        //}
//    }
//}
