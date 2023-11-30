using Newtonsoft.Json;
using PrueferCode;

string? path = args.FirstOrDefault();

if (!args.Any())
{
    Console.WriteLine("Введите путь до файла:");

    path = Console.ReadLine();
}

if (string.IsNullOrEmpty(path) || !File.Exists(path))
{
    Console.WriteLine("Файл не найден.");
    return;
}

var json = await File.ReadAllTextAsync(path);
var graph = JsonConvert.DeserializeObject<List<(int, int)>>(json);

PrueferEncoder encoder = new();

var code = encoder.Encode(graph!);

var settings = new JsonSerializerSettings
{
    Formatting = Formatting.Indented
};

await File.AppendAllTextAsync(path, $"\nКод прюфера: {JsonConvert.SerializeObject(code, settings)}");

var restoredGraph = encoder.Decode(code!);

await File.AppendAllTextAsync(path,$"\nВосстановленный граф: {JsonConvert.SerializeObject(restoredGraph, settings)}");