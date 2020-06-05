using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace Boggle
{
    public class Node
    {
        public char Value { get; set; }
        public int Depth { get; set; }
        public bool IsLeaf { get; set; }
        public Node Parent { get; set; }
        public List<Node> Children { get; set; }

        public Node(char v, int d, Node p, bool l = false)
        {
            Value = v;
            Depth = d;
            Parent = p;
            IsLeaf = l;
            Children = new List<Node>();
        }

        public void MakeLeaf()
        {
            IsLeaf = true;
        }

        public Node FindChildNode(char c)
        {
            foreach (Node child in Children)
            {
                if (child.Value == c)
                    return child;
            }

            return null;
        }
    }

    public class Trie
    {
        private readonly Node RootNode;

        public Trie()
        {
            RootNode = new Node('^', 0, null);
        }

        private void BranchOut(string str)
        {
            Node currentNode = RootNode;

            foreach (char c in str)
            {
                Node foundNode = currentNode.FindChildNode(c);
                if (foundNode == null)
                {
                    Node newNode = new Node(c, currentNode.Depth + 1, currentNode);
                    currentNode.Children.Add(newNode);
                    currentNode = newNode;
                    continue;
                }
                currentNode = foundNode;
            }

            currentNode.MakeLeaf();
            currentNode.Children.Add(new Node('$', currentNode.Depth + 1, currentNode));
        }

        public void GenerateFromList(List<string> list)
        {
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].Length >= BoggleGlobals.g_minWordLength)
                    BranchOut(list[i]);
            }
        }

        public int QueryNodes(string str)
        {
            Node currentNode = RootNode;
            int last = str.Length - 1;
            int result = 0;

            for (int c = 0; c < str.Length; c++)
            {
                Node foundNode = currentNode.FindChildNode(str[c]);
                if (foundNode == null)
                {
                    result = -1;
                    break;
                }
                else if (foundNode.IsLeaf)
                {
                    if (c == last)
                        result = 1;
                }
                currentNode = foundNode;
            }

            return result;
        }
    }

    public class WordSilo
    {
        private int             m_maxLength;
        private string          m_diadicHeader;
        private List<string>    m_wordList;

        public WordSilo()
        {
            m_maxLength = 0;
            m_diadicHeader = "";
            m_wordList = new List<string>();
        }
        public WordSilo(string h)
        {
            m_maxLength = 0;
            m_diadicHeader = h;
            m_wordList = new List<string>();
        }
        public string GetHeader2()
        {
            return m_diadicHeader;
        }
        public void SetHeader2(string h)
        {
            if (h.Length == 2)
                m_diadicHeader = h;
        }
        public void SetMaxLength()
        {
            List<string> list = new List<string>(m_wordList);
            list.Sort((x, y) => x.Length - y.Length);

            m_maxLength = list[list.Count - 1].Length;
        }
        public int GetMaxLength()
        {
            return m_maxLength;
        }
        public int GenerateMaxLength()
        {
            List<string> list = new List<string>(m_wordList);
            list.Sort((x,y) => x.Length - y.Length);

            return list[list.Count - 1].Length;
        }
        public List<string> GetWordList()
        {
            return m_wordList;
        }
        public void AddWord(string word)
        {
            m_wordList.Add(word);
        }
    }

    public class WordDictionary
    {
        private List<string> m_headerList;
        private List<WordSilo> m_wordSilos;

        public WordDictionary()
        {
            m_headerList = new List<string>();
            m_wordSilos = new List<WordSilo>();
        }

        public void CompileDictionary(string path)
        {
            StreamReader reader = new StreamReader(path);

            string word, hdr = String.Empty;
            bool notFirstRun = false;
            WordSilo silo = new WordSilo();
            using (reader = File.OpenText(path))
            {
                while ((word = reader.ReadLine()) != null)
                {
                    hdr = BoggleGlobals.PryDiadicHeader(word);
                    if (m_headerList.Contains(hdr))
                    {
                        if (word.Length <= BoggleGlobals.g_maxWordLength)
                            silo.AddWord(word);
                    }
                    else
                    {
                        if (notFirstRun)
                        {
                            silo.SetMaxLength();
                            m_wordSilos.Add(silo);
                        }
                        m_headerList.Add(hdr);
                        silo = new WordSilo(hdr);
                        silo.AddWord(word);
                        notFirstRun = true;
                    }
                }
                if (word == null)
                {
                    silo.SetMaxLength();
                    m_wordSilos.Add(silo);
                }
            }
        }
        public WordSilo SeekSilo(string header)
        {
            int si = m_headerList.BinarySearch(header);
            return (si < 0) ? null : m_wordSilos[si];
        }
        public List<string> SeekSiloList(string header)
        {
            int si = m_headerList.BinarySearch(header);
            return (si < 0) ? null : m_wordSilos[si].GetWordList();
        }
        public List<string> GetHeaderList()
        {
            return m_headerList;
        }
    }
    public class BoggleSet
    {
        public int m_solutionCount = 0;
        public int m_solutionPoints = 0;
        public bool m_isSolved = false;
        private bool m_isBoardProvided = false;
        private char[,] m_providedBoard = null;
        private string m_dictionaryPath;

        [HideInInspector] public WordDictionary m_dictionary;
        [HideInInspector] public BlockSet m_blocks;
        [HideInInspector] public Trie m_wordTrie;

        public BoggleSet()
        {
            m_dictionary = new WordDictionary();
            m_blocks = new BlockSet();
            m_wordTrie = new Trie();
        }
        public BoggleSet(string dictionaryPath)
        {
            m_dictionary = new WordDictionary();
            m_blocks = new BlockSet();
            m_dictionaryPath = dictionaryPath;
            m_dictionary.CompileDictionary(m_dictionaryPath);
            m_wordTrie = new Trie();

            List<string> items = new List<string>();
            StreamReader reader = new StreamReader(dictionaryPath);

            while (!reader.EndOfStream)
                items.Add(reader.ReadLine());

            m_wordTrie.GenerateFromList(items);
        }
        public int GetPoints() { return m_solutionPoints; }
        public int GetCount() { return m_solutionCount; }
        public void SetProvidedBoard(bool isTest = false, bool isBoardProvided = false, char[,] providedBoard = null)
        {
            m_isBoardProvided = isBoardProvided;
            m_providedBoard = providedBoard;
            if (isTest)
            {
                m_isBoardProvided = true;
                m_providedBoard = new char[3, 3] {  { 'D', 'Z', 'X' },
                                                    { 'E', 'A', 'I' },
                                                    { 'Q', 'U', 'T' }   };
            }

            if (m_isBoardProvided && m_providedBoard != null)
            {
                m_blocks.SpecifyGrid2D(m_providedBoard);
            }
        }
        public void SetDictionaryPath(string path)
        {
            m_dictionaryPath = path;
            m_dictionary.CompileDictionary(m_dictionaryPath);
        }
        public IEnumerable<string> CompileWords(List<string> words)
        {
            // Compile all combination possibilities starting from each block and then store into a List of strings
            for (int r = 0; r < m_blocks.GetBlocks2D().GetLength(0); r++)
            {
                for (int c = 0; c < m_blocks.GetBlocks2D().GetLength(1); c++)
                {
                    m_blocks.GetBlocks2D()[r, c].m_visited = true;
                    List<string> list = new List<string>();
                    words.AddRange(GenerateCombination(list, "", r, c));
                    m_blocks.ResetVisited();
                }
            }

            words = words.Distinct().ToList();
            words.Sort();
            m_solutionCount += words.Count;
            return words.AsEnumerable();
        }
        public IEnumerable<string> CompileWordsFromTrie(List<string> words)
        {
            // Compile all combination possibilities starting from each block and then store into a List of strings
            for (int r = 0; r < m_blocks.GetBlocks2D().GetLength(0); r++)
            {
                for (int c = 0; c < m_blocks.GetBlocks2D().GetLength(1); c++)
                {
                    m_blocks.GetBlocks2D()[r, c].m_visited = true;
                    List<string> list = new List<string>();
                    words.AddRange(GenerateCombinationFromTrie(list, "", r, c));
                    m_blocks.ResetVisited();
                }
            }

            words = words.Distinct().ToList();
            words.Sort();
            m_solutionCount += words.Count;
            return words.AsEnumerable();
        }
        public List<string> GenerateCombination(List<string> list, string prev, int row, int col)
        {
            //Debug.Log("GenerateCombination() iteration");         //DEBUG
            string cur = m_blocks.GetBlocks2D()[row, col].m_top.ToString();
            if (cur == "Q") 
                cur += "U";
            string combo = prev + cur;

            if (combo.Length == 2)
            {
                if (m_dictionary.GetHeaderList().BinarySearch(combo) < 0)
                    return list;
            }

            // For each combination, locate its potential silo by its header
            // Search for each combination in the silo and count each success
            if (combo.Length >= BoggleGlobals.g_minWordLength)
            {
                WordSilo silo = m_dictionary.SeekSilo(BoggleGlobals.PryDiadicHeader(combo));
                if (silo != null)
                {
                    if (combo.Length > silo.GetMaxLength())
                        return list;

                    int result = LookUpWord(combo, silo.GetWordList());
                    if (result == 1)
                    {
                        //Debug.Log("DEBUG --- Found: " + combo);         //DEBUG
                        list.Add(combo);
                    }
                }
            }

            List<AlphabetBlock> adjacent = m_blocks.FindNewAdjacent(row, col);
            if (adjacent.Count > 0)
            {
                for (int a = 0; a < adjacent.Count; a++)
                {
                    if (!adjacent[a].m_visited)
                    {
                        adjacent[a].m_visited = true;
                        list = GenerateCombination(list, combo, adjacent[a].m_row, adjacent[a].m_col);
                    }
                }
            }

            m_blocks.GetBlocks2D()[row, col].m_visited = false;
            return list;
        }
        public List<string> GenerateCombinationFromTrie(List<string> list, string prev, int row, int col)
        {
            //Debug.Log("GenerateCombination() iteration");         //DEBUG
            string cur = m_blocks.GetBlocks2D()[row, col].m_top.ToString();
            if (cur == "Q")
                cur += "U";
            string combo = prev + cur;

            int n = m_wordTrie.QueryNodes(combo);
            if (n < 0)
            {
                m_blocks.GetBlocks2D()[row, col].m_visited = false;
                return list;
            }
            else if (n > 0)
            {
                list.Add(combo);
                ScoreWord(combo);
            }

            List<AlphabetBlock> adjacent = m_blocks.FindNewAdjacent(row, col);
            if (adjacent.Count > 0)
            {
                for (int a = 0; a < adjacent.Count; a++)
                {
                    if (!adjacent[a].m_visited)
                    {
                        adjacent[a].m_visited = true;
                        list = GenerateCombinationFromTrie(list, combo, adjacent[a].m_row, adjacent[a].m_col);
                    }
                }
            }

            m_blocks.GetBlocks2D()[row, col].m_visited = false;
            return list;
        }
        public int LookUpWord(string combo, List<string> silo)
        {
            int c = silo.BinarySearch(combo);
            if (c > 0)
            {
                ScoreWord(silo[c]);
                return 1;
            }

            /*for (int i = 0; i < silo.Count; i++)
            {
                if (silo[i] == combo)
                {
                    ScoreWord(silo[i]);
                    return 1;
                }
            }*/

            return 0;
        }
        public void ScoreWord(string word)
        {
            if (word is null)
            {
                throw new ArgumentNullException(nameof(word));
            }

            switch (word.Length)
            {
                case int n when (n < 3):
                    break;
                case int n when (n == 3 || n == 4):
                    m_solutionPoints += 1;
                    break;
                case 5:
                    m_solutionPoints += 2;
                    break;
                case 6:
                    m_solutionPoints += 3;
                    break;
                case 7:
                    m_solutionPoints += 4;
                    break;
                default:
                    m_solutionPoints += 11;
                    break;
            }
        }
    }
}
