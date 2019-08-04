using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;
using Microsoft.Extensions.DependencyModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace ant.core.csscript.test
{
    class Program
    {
        static void Main(string[] args)
        {

            //int result =  CSharpScript.Evaluate <int>("1 + 2");
            //Console.WriteLine(result);

            foreach (var item in DependencyContext.Default.CompileLibraries)
            {
                Console.WriteLine(item.Name);
                Console.WriteLine(item.Path);
                Console.WriteLine(item.Assemblies.FirstOrDefault());
                Console.WriteLine(item.ResolveReferencePaths().Count());
                Console.WriteLine(item.ResolveReferencePaths().FirstOrDefault());
                Console.WriteLine("----------------------");
            }


            Console.ReadLine();

            MetadataReference[] _ref =
             DependencyContext.Default.CompileLibraries
                  //.All(cl=>!string.IsNullOrEmpty(cl.Name))
                  .First(cl => cl.Name == "Microsoft.NETCore.App")
                  .ResolveReferencePaths()
                  .Select(asm => MetadataReference.CreateFromFile(asm))
                  .ToArray();
            string testClass = @"using System; 
              namespace test{
               public class tes
               {
                 public string unescape(string Text)
                    { 
                      return Uri.UnescapeDataString(Text);
                    } 
               }
              }";

            var compilation = CSharpCompilation.Create(Guid.NewGuid().ToString() + ".dll")
                .WithOptions(new CSharpCompilationOptions(
                    Microsoft.CodeAnalysis.OutputKind.DynamicallyLinkedLibrary,
                    usings: null,
                    optimizationLevel: OptimizationLevel.Debug, // TODO
                    checkOverflow: false,                       // TODO
                    allowUnsafe: true,                          // TODO
                    platform: Platform.AnyCpu,
                    warningLevel: 4,
                    xmlReferenceResolver: null // don't support XML file references in interactive (permissions & doc comment includes)
                    ))
                .AddReferences(_ref)
                .AddSyntaxTrees(CSharpSyntaxTree.ParseText(testClass)
              );

            var eResult = compilation.Emit("test.dll");




            //        SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(@"
            //using System;

            //namespace RoslynCompileSample
            //{
            //    public class Writer
            //    {
            //        public void Write(string message)
            //        {
            //            Console.WriteLine(message);
            //        }
            //    }
            //}");

            //        string assemblyName = Path.GetRandomFileName();
            //        //MetadataReference[] references = new MetadataReference[]
            //        //{
            //        //    MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
            //        //    MetadataReference.CreateFromFile(typeof(Object).Assembly.Location),
            //        //    MetadataReference.CreateFromFile(typeof(Console).Assembly.Location),
            //        //    MetadataReference.CreateFromFile(typeof(AppDomain).Assembly.Location),
            //        //    MetadataReference.CreateFromFile(typeof(Enumerable).Assembly.Location)
            //        //};
            //        MetadataReference[] references = DependencyContext.Default.CompileLibraries
            //            .First(cl => cl.Name == "Microsoft.NETCore.App")
            //            .ResolveReferencePaths()
            //            .Select(asm => MetadataReference.CreateFromFile(asm))
            //            .ToArray();

            //        CSharpCompilation compilation = CSharpCompilation.Create(
            //            null,
            //            syntaxTrees: new[] { syntaxTree },
            //            references: references,
            //            //options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));
            //            options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

            //        using (var ms = new MemoryStream())
            //        {
            //            EmitResult result = compilation.Emit(ms);

            //            if (!result.Success)
            //            {
            //                IEnumerable<Diagnostic> failures = result.Diagnostics.Where(diagnostic =>
            //                    diagnostic.IsWarningAsError ||
            //                    diagnostic.Severity == DiagnosticSeverity.Error);

            //                foreach (Diagnostic diagnostic in failures)
            //                {
            //                    Console.Error.WriteLine("{0}: {1}", diagnostic.Id, diagnostic.GetMessage());
            //                }
            //            }
            //            else
            //            {
            //                ms.Seek(0, SeekOrigin.Begin);
            //                Assembly assembly = Assembly.Load(ms.ToArray());

            //                Type type = assembly.GetType("RoslynCompileSample.Writer");
            //                object obj = Activator.CreateInstance(type);
            //                type.InvokeMember("Write",
            //                    BindingFlags.Default | BindingFlags.InvokeMethod,
            //                    null,
            //                    obj,
            //                    new object[] { "Hello World" });

            //            }
            //        }


            Console.WriteLine("***** END *****");

            Console.ReadLine();
        }
    }
}
