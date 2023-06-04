using Newtonsoft.Json;
using System.Text;

internal class Program
{
    private static async Task Main(string[] args)
    {
        Console.WriteLine("Введите путь до файла:");

        var path = Console.ReadLine();

        //Если файл не найден, то завершаем выполнение программы.
        if (string.IsNullOrEmpty(path) || !File.Exists(path))
        {
            Console.WriteLine("Файл не найден.");
            return;
        }

        //читаем всё содержимое файла
        var data = await File.ReadAllTextAsync(path);
        //создаем граф
        var graph = new List<(int x1, int x2)>();

        //разбиваем содержимое файла на строки и добавляем в граф каждое считанное число
        var lines = data.Split('\n');

        var firstLine = lines[0].Split(' ');
        var secondLine = lines[1].Split(' ');

        for (int i = 0; i < firstLine.Length; i++)
        {
            graph.Add((int.Parse(firstLine[i]), int.Parse(secondLine[i])));
        }

        //создаем класс метода кода Прюфера
        PrueferEncoder encoder = new();

        //Кодирвоанжие графа
        var code = encoder.Encode(graph);

        //вывод в файл и на консоль
        StringBuilder sb = new();
        sb.AppendLine("Код прюфера:");

        code?.ToList().ForEach(x =>
        {
            Console.Write(x);
            sb.Append(x);
        });

        sb.AppendLine();
        Console.WriteLine();

        //восстановление графа по коду прюфера
        var restoredGraph = encoder.Decode(code!);

        //вывод в файл и на консоль
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
    }
}

public class PrueferEncoder
{
    //Кодирование графа
    public IEnumerable<int> Encode(IEnumerable<(int x1, int x2)> graph)
    {
        IList<int> prueferCode = new List<int>();
        var graphClone = new List<(int x1, int x2)>(graph);

        while (graphClone.Count() > 1)
        {
            var minIndex = GetIndexOfMin(graphClone);

            prueferCode.Add(graphClone[minIndex].x1);

            graphClone.RemoveAt(minIndex);
        }

        return prueferCode;
    }

    //Декодирование графа
    public IEnumerable<(int x1, int x2)> Decode(IEnumerable<int> prueferCode)
    {
        IList<(int x1, int x2)> graph = new List<(int x1, int x2)>();
        IList<int> restoredVertexes = new List<int>();
        IList<int> prueferCodeClone = new List<int>(prueferCode);
        var vertexCount = prueferCode.Count() + 2;

        int restoredVertex;

        //Пока не восстановлен весь код прюфера ищем отсутствующую вершину и записываем в список восстановленных
        while (prueferCodeClone.Count > 0)
        {
            restoredVertex = FindMissingVertex(prueferCodeClone, restoredVertexes, vertexCount);

            if (restoredVertex == -1)
            {
                throw new InvalidOperationException("Ошибка при вычислении, не найдена отсутствующая вершина.");
            }

            restoredVertexes.Add(restoredVertex);
            graph.Add((prueferCodeClone[0], restoredVertex));

            prueferCodeClone.RemoveAt(0);
        }

        //Восстанавливаем две оставшиеся вершины
        restoredVertex = FindMissingVertex(prueferCodeClone, restoredVertexes, vertexCount);
        restoredVertexes.Add(restoredVertex);

        graph.Add((
            graph.Last().x2,
            restoredVertex
            ));

        restoredVertex = FindMissingVertex(prueferCodeClone, restoredVertexes, vertexCount);
        restoredVertexes.Add(restoredVertex);

        graph.Add((
            graph.Last().x2,
            restoredVertex
            ));

        return graph;
    }

    //Вспомогательый метод нахождения минимальной вершины графа для занесения в код.
    private int GetIndexOfMin(IEnumerable<(int x1, int x2)> graph)
    {
        var min = graph.Where(e => !graph.Any(edge => edge.x1 == e.x2)).MinBy(e => e.x2);

        return graph.ToList().FindIndex(v => v == min);
    }

    //Ищет недостающую вершину при восстановлении
    private int FindMissingVertex(IEnumerable<int> prueferCode, IEnumerable<int> restoredVertex, int vertexCount)
    {
        for (int vertex = 1; vertex <= vertexCount; vertex++)
        {
            if (!prueferCode.Contains(vertex) && !restoredVertex.Contains(vertex))
            {
                return vertex;
            }
        }

        return -1;
    }
}