using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Newtonsoft.Json;

namespace CSharpFileParser
{
    class Program
    {
        static async Task Main(string[] args)
        {
            if (args.Length != 2)
            {
                Console.WriteLine("Usage: CSharpFileParser <input_file_path> <output_file_path>");
                return;
            }

            var inputFilePath = args[0];
            var outputFilePath = args[1];

            if (!File.Exists(inputFilePath))
            {
                Console.WriteLine($"File not found: {inputFilePath}");
                return;
            }

            var code = await File.ReadAllTextAsync(inputFilePath);
            var tree = CSharpSyntaxTree.ParseText(code);
            var root = await tree.GetRootAsync().ConfigureAwait(false);

            var classes = root.DescendantNodes().OfType<ClassDeclarationSyntax>();
            var result = classes.Select(c => new
            {
                ClassName = c.Identifier.Text,
                Methods = c.DescendantNodes().OfType<MethodDeclarationSyntax>()
                    .Select(m => m.Identifier.Text).ToArray()
            }).ToList();

            var json = JsonConvert.SerializeObject(result, Formatting.Indented);
            await File.WriteAllTextAsync(outputFilePath, json);

            Console.WriteLine($"Successfully parsed {inputFilePath} and wrote the output to {outputFilePath}");
        }
    }
}