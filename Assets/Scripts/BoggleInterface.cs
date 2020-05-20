using System.Collections.Generic;
using System.Linq;
using Boggle;

namespace BoggleInterface
{
    public interface IResults
    {
        IEnumerable<string> Words { get; }
        int Score { get; }
    }

    public interface ISolver
    {
        IResults FindWords(char[,] board);
    }

    public class MyBoggleSolution : IResults, ISolver
    {
        private static WordDictionary   m_Dictionary;
        private static BoggleSet        m_Boggle;
        private IEnumerable<string>     m_FoundWords { get; set; }
        private int                     m_Points { get; set; }

        IEnumerable<string> IResults.Words { get => GetWords(); }
        int IResults.Score { get => GetScore(); }

        public MyBoggleSolution(bool isBoardProvided = false, char[,] providedBoard = null)
        {
            m_Boggle = new BoggleSet();
            m_Boggle.SetProvidedBoard(false, isBoardProvided, providedBoard);
        }
        public IEnumerable<string> GetWords()
        {
            return m_FoundWords;
        }
        public int GetScore()
        {
            return m_Points;
        }
        public WordDictionary GetDictionary()
        {
            return m_Dictionary;
        }
        public IResults FindWords(char[,] board)
        {
            MyBoggleSolution solution = new MyBoggleSolution(true, board);
            m_Boggle.m_dictionary = m_Dictionary;
            List<string> words = new List<string>();

            solution.m_FoundWords = m_Boggle.CompileWords(words);
            solution.m_Points = m_Boggle.GetPoints();

            return solution;
        }
        public static ISolver CreateSolver(string dictionaryPath)
        {
            m_Boggle.m_dictionary.CompileDictionary(dictionaryPath);
            m_Dictionary = m_Boggle.m_dictionary;

            return new MyBoggleSolution();
        }
    }
}