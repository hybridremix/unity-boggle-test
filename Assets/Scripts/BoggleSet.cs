using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;

namespace Boggle
{
    static class BoggleGlobals
    {
        public static int g_numBlockSides = 6;
        public static int g_iGridSize = 3;
        public static int g_minWordLength = 3;
        public static int g_maxWordLength = g_iGridSize * g_iGridSize;
        public static string PryDiadicHeader(string str)
        {
            char[] array = str.ToCharArray(0, 2);
            string header = new string(array);
            return header;
        }
        public static string PryTriadicHeader(string str)
        {
            char[] array = str.ToCharArray(0, 3);
            string header = new string(array);
            return header;
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

        public BoggleSet()
        {
            m_dictionary = new WordDictionary();
            m_blocks = new BlockSet();
        }
        public BoggleSet(string dictionaryPath)
        {
            m_dictionary = new WordDictionary();
            m_blocks = new BlockSet();
            m_dictionaryPath = dictionaryPath;
            m_dictionary.CompileDictionary(m_dictionaryPath);
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
                                                    { 'E', 'A', 'T' },
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
        public List<string> GenerateCombination(List<string> list, string prev, int row, int col)
        {
            //Debug.Log("GenerateCombination() iteration");         //DEBUG
            string cur = m_blocks.GetBlocks2D()[row, col].m_top.ToString();
            if (cur == "Q")
            { cur += "U"; }
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
