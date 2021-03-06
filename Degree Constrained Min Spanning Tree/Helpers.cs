﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DCMSC_Exact
{
    class Helpers
    {
        /// <summary>
        /// Inicializa uma nova matriz quadrada de ordem <paramref name="o"/>.
        /// </summary>
        /// <param name="o">Ordem da matriz quadrada</param>
        /// <returns>Uma nova matriz quadrada</returns>
        public static int?[,] InicializaMatrix(int o)
        {
            int?[,] matrix = new int?[o, o];
            return matrix;
        }

        /// <summary>
        /// Imprime uma matriz simétrica <paramref name="m"/> na tela ou na mensagem de Debug
        /// </summary>
        /// <param name="m">Matriz a simétrica a ser imprimida</param>
        /// <param name="to_console">Flag para mandar ou nao pro console</param>
        public static void PrintMatrix(int?[,] m, bool to_console = true)
        {
            StringBuilder sb_message = new StringBuilder();

            for (int i = 0; i < m.GetLength(0); i++)
            {
                for (int j = 0; j < m.GetLength(1); j++)
                {
                    if (null != m[i, j])
                        sb_message.AppendFormat("{0,4}", m[i, j]);
                    else
                        sb_message.Append("   -");
                    sb_message.Append(" ");
                }
                sb_message.AppendLine();
            }

            //foreach (var i in m)
            //{
            //    foreach (var j in i)
            //    {
            //        if (null != j)
            //            sb_message.AppendFormat("{0,4}", j);
            //        else
            //            sb_message.Append("   -");
            //        sb_message.Append(" ");
            //    }
            //    sb_message.AppendLine();
            //}
            if (to_console)
                Console.WriteLine(sb_message);
            else
                Debug.Print(sb_message.ToString());
        }

        /// <summary>
        /// Imprime na tela, de maneira ordenada a lista de arestas passada.
        /// <example><paramref name="name"/> = {0-1, 1-2, }</example>
        /// </summary>
        /// <param name="name">Nome da lista de arestas</param>
        /// <param name="l">A lista de arestas</param>
        /// <param name="to_console">Flag para dizer se deve imprimir na saída principal ou não</param>
        public static void PrintEdges(string name, List<Tuple<int, int>> l, bool to_console = false)
        {
            StringBuilder sb_message = new StringBuilder();
            sb_message.AppendFormat("{0} = ", name);
            sb_message.Append("{");

            foreach (var edge in l)
                sb_message.AppendFormat("{0}-{1}, ", edge.Item1 + 1, edge.Item2 + 1);

            sb_message.AppendLine("}");

            if (to_console)
                Console.WriteLine(sb_message);

            Debug.Print(sb_message.ToString());
        }

        /// <summary>
        /// Verifica se a aresta com os vértices <paramref name="a"/> e <paramref name="b"/>
        /// estão na lista <paramref name="l"/>
        /// </summary>
        /// <param name="l">Lista de arestas que deve cobrir ou não tais arestas</param>
        /// <param name="a">Um dos vértices da aresta a ser verificada</param>
        /// <param name="b">O outro vértice da aresta a ser verificada</param>
        /// <returns></returns>
        public static bool CoveredEdge(List<Tuple<int, int>> l, int a, int b)
        {
            return l.Exists(i => (i.Item1 == a && i.Item2 == b) || (i.Item1 == b && i.Item2 == a));
        }

        /// <summary>
        /// Calcula o custo da lista de arestas <paramref name="l"/> dentro do grafo
        /// representado pela matrix <paramref name="matrix"/>
        /// </summary>
        /// <param name="l">Lista de arestas</param>
        /// <param name="matrix">Representação matricial do grafo</param>
        /// <returns></returns>
        public static int Cost(List<Tuple<int, int>> l, int?[,] matrix)
        {
            int cost = 0;
            foreach (var edge in l)
                cost += (int)matrix[edge.Item1, edge.Item2];

            //Debug.Print("Custo da árvore = {0}", cost);
            return cost;
        }

        /// <summary>
        /// Cria uma matriz quadrada a partir do arquivo de teste passado.
        /// </summary>
        /// <param name="filepath">Endereço do arquivo de teste. Pode ser relativo ou absoluto.</param>
        /// <param name="ordem">Parâmetro de saída para a ordem da matriz</param>
        /// <returns>Matriz quadrada preenchida de acordo com o arquivo de teste.</returns>
        public static int?[,] CriaMatrix(string filepath, ref int ordem)
        {
            using (StreamReader testcase = new StreamReader(filepath))
            {
                string linha;
                string pattern = @"^(?<in>\d+)\s+(?<out>\d+)\s+(?<weight>\d+)";
                Regex rx_line = new Regex(pattern);

                Debug.Print("Criando array");

                int?[,] matrix = Helpers.InicializaMatrix(ordem);

                while (!testcase.EndOfStream)
                {
                    linha = testcase.ReadLine();
                    Match match = Regex.Match(linha, pattern);

                    int a = int.Parse(match.Groups["in"].Value);
                    int b = int.Parse(match.Groups["out"].Value);
                    int w = int.Parse(match.Groups["weight"].Value);

                    matrix[a, b] = w;
                    matrix[b, a] = w;
                }

                return matrix;
            }
        }

        public static void AddEdge(int i, int j, ref List<Tuple<int, int>> l)
        {
            if (i < j)
                l.Add(Tuple.Create(i, j));
            else
                l.Add(Tuple.Create(j, i));
        }

        public static bool Possivel(List<Tuple<int, int>> l, int d, int o)
        {
            Dictionary<int, int> v = new Dictionary<int, int>();

            for (int i = 0; i < o; i++)
                v.Add(i, 0);

            foreach (var e in l)
            {
                v[e.Item1]++;
                v[e.Item2]++;

                if (v[e.Item1] > d || v[e.Item2] > d)
                    return false;
            }
            return true;
        }

        public static void QSort<T>(List<Tuple<int, int>> el, T?[,] w, int l, int r) where T : struct,IComparable
        {
            int left = l, right = r;
            int mid = (left + right) / 2;
            T pivot = (T)w[el[mid].Item1, el[mid].Item2];
            while (left < right)
            {
                while (((T)w[el[left].Item1, el[left].Item2]).CompareTo(pivot) < 0)
                    left++;
                while ((((T)w[el[right].Item1, el[right].Item2]).CompareTo(pivot) > 0))
                    right--;
                if (left <= right)
                {
                    var aux = el[left];
                    el[left] = el[right];
                    el[right] = aux;
                    left++;
                    right--;
                }
            }
            if (right > l)
                QSort<T>(el, w, l, right);
            if (left < r)
                QSort<T>(el, w, left, r);
        }
    }

    class Tree
    {
        public int Root { get; private set; }
        public List<int> vertices { get; private set; }
        public List<Tuple<int, int>> edges { get; private set; }

        public Tree(int root)
        {
            vertices = new List<int>();
            edges = new List<Tuple<int, int>>();

            this.Root = root;
            vertices.Add(root);
        }
        public void AddTree(Tree t)
        {
            this.edges.AddRange(t.edges);
            this.vertices.AddRange(t.vertices);
        }
    }
}
