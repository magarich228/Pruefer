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
        //Создание пустого кода
        IList<int> prueferCode = new List<int>();

        //Копия графа
        var graphClone = new List<(int x1, int x2)>(graph);

        //Пока у графа не осталось одно ребро получаем минимальную вершину,
        //убираем её с графа и записываем вершину, которая идет в код прюфера
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
        //Пустой граф, где x1 - начальная вершина ребра, x2 - конечная
        IList<(int x1, int x2)> graph = new List<(int x1, int x2)>();
        //Восстановленные вершины
        IList<int> vosstanovlenayVershini = new List<int>();
        //Копия кода прюфера
        IList<int> prueferCodeClone = new List<int>(prueferCode);

        //Кол-во вершин графа
        var vertexCount = prueferCode.Count() + 2;

        int vosstanovlennayVershina;

        //Пока не восстановлен весь код прюфера
        while (prueferCodeClone.Count > 0)
        {
            //Ищем отсутствующую вершину
            vosstanovlennayVershina = FindMissingVertex(prueferCodeClone, vosstanovlenayVershini, vertexCount);

            if (vosstanovlennayVershina == -1)
            {
                throw new InvalidOperationException("Ошибка при вычислении, не найдена отсутствующая вершина.");
            }

            //Добавление найденной вершины в список восстановленных вершин и
            //добавление вершины к графу
            vosstanovlenayVershini.Add(vosstanovlennayVershina);
            graph.Add((prueferCodeClone[0], vosstanovlennayVershina));

            //Убираем вершину из кода прюфера
            prueferCodeClone.RemoveAt(0);
        }

        //Восстанавливаем две оставшиеся вершины таким же образом
        vosstanovlennayVershina = FindMissingVertex(prueferCodeClone, vosstanovlenayVershini, vertexCount);
        vosstanovlenayVershini.Add(vosstanovlennayVershina);

        graph.Add((
            graph.Last().x2,
            vosstanovlennayVershina
            ));

        vosstanovlennayVershina = FindMissingVertex(prueferCodeClone, vosstanovlenayVershini, vertexCount);
        vosstanovlenayVershini.Add(vosstanovlennayVershina);

        graph.Add((
            graph.Last().x2,
            vosstanovlennayVershina
            ));

        return graph;
    }

    //Вспомогательый метод нахождения минимальной вершины графа для занесения в код.
    private int GetIndexOfMin(IEnumerable<(int x1, int x2)> graph)
    {
        var min = graph
            .Where(e => !graph.Any(edge => edge.x1 == e.x2)) //Берем все висящие ребра
            .MinBy(e => e.x2); //Находим минимальную вершину среди них

        //возращаем минимальную вершину
        return graph.ToList().FindIndex(v => v == min);
    }

    //Ищет недостающую вершину при восстановлении
    private int FindMissingVertex(IEnumerable<int> prueferCode, IEnumerable<int> vosstanovlenniiVershini, int kolvoVershin)
    {
        //Перебираем все вершины
        for (int vershina = 1; vershina <= kolvoVershin; vershina++)
        {
            //Если вершины нет не в графе, не в списке восстановленных вершин, возвращаем её
            if (!prueferCode.Contains(vershina) && !vosstanovlenniiVershini.Contains(vershina))
            {
                return vershina;
            }
        }

        return -1;
    }
}