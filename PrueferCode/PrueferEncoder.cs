namespace PrueferCode
{
    public class PrueferEncoder
    {
        public IEnumerable<int> Encode(int[,] graph)
        {
            if (graph.GetLength(0) != 2)
            {
                throw new ArgumentException("Неправильный формат графа.");
            }

            IList<(int x1, int x2)> graphList = new List<(int x1, int x2)>();

            for (int i = 0; i < graph.GetLength(1); i++)
            {
                graphList.Add((graph[0, i], graph[1, i]));
            }

            return Encode(graphList);
        }

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

        public IEnumerable<(int x1, int x2)> Decode(IEnumerable<int> prueferCode)
        {
            IList<(int x1, int x2)> graph = new List<(int x1, int x2)>();
            IList<int> restoredVertexes = new List<int>();
            IList<int> prueferCodeClone = new List<int>(prueferCode);
            var vertexCount = prueferCode.Count() + 2;

            int restoredVertex;

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

        private int GetIndexOfMin(IEnumerable<(int x1, int x2)> graph)
        {
            var min = graph.Where(e => !graph.Any(edge => edge.x1 == e.x2)).MinBy(e => e.x2);

            return graph.ToList().FindIndex(v => v == min);
        }

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
}
