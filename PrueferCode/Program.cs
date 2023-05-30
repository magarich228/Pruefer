using PrueferCode;
using System.Text;

Console.WriteLine("Введите путь до файла:");

var path = Console.ReadLine();

if (string.IsNullOrEmpty(path) || !File.Exists(path))
{
    Console.WriteLine("Файл не найден.");
    return;
}

var data = await File.ReadAllTextAsync(path);
var graph = new List<(int x1, int x2)>();

var lines = data.Split('\n');

var firstLine = lines[0].Split(' ');
var secondLine = lines[1].Split(' ');

for (int i = 0; i < firstLine.Length; i++)
{
    graph.Add((int.Parse(firstLine[i]), int.Parse(secondLine[i])));
}

PrueferEncoder encoder = new();

var code = encoder.Encode(graph);

StringBuilder sb = new();
sb.AppendLine("Код прюфера:");

code?.ToList().ForEach(x =>
{
    Console.Write(x);
    sb.Append(x);
});

sb.AppendLine();
Console.WriteLine();

var restoredGraph = encoder.Decode(code!);

sb.AppendLine("Декодированный граф:");

foreach (var edge in restoredGraph)
{
    sb.Append($"{edge.x1} ");
    Console.Write($"{edge.x1} ");
}

sb.AppendLine();
Console.WriteLine();

foreach (var edge in restoredGraph)
{
    sb.Append($"{edge.x2} ");
    Console.Write($"{edge.x2} ");
}

await File.AppendAllTextAsync(path, sb.ToString());