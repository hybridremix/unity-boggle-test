using System.Collections.Generic;
using System.IO;

namespace Boggle
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
        private static WordDictionary   Dictionary;
        private static BoggleSet        Boggle;
        private static Trie             WordTrie;
        private IEnumerable<string>     FoundWords  { get; set; }
        private int                     TotalPoints { get; set; }

        IEnumerable<string> IResults.Words  { get => GetWords(); }
        int IResults.Score                  { get => GetScore(); }

        public MyBoggleSolution(bool isBoardProvided = false, char[,] providedBoard = null)
        {
            Boggle = new BoggleSet();
            Boggle.SetProvidedBoard(false, isBoardProvided, providedBoard);
        }

        public IEnumerable<string> GetWords()   { return FoundWords;  }
        public int GetScore()                   { return TotalPoints; }
        public WordDictionary GetDictionary()   { return Dictionary;  }
        public Trie GetWordTrie()               { return WordTrie;    }

        public IResults FindWords(char[,] board)
        {
            MyBoggleSolution solution = new MyBoggleSolution(true, board);
            Boggle.m_dictionary = Dictionary;
            Boggle.m_wordTrie = WordTrie;
            List<string> words = new List<string>();

            solution.FoundWords = Boggle.CompileWordsFromTrie(words);
            solution.TotalPoints = Boggle.GetPoints();
  
            return solution;
        }
        public static ISolver CreateSolver(string dictionaryPath)
        {
            MyBoggleSolution solution = new MyBoggleSolution();
            Boggle.m_dictionary.CompileDictionary(dictionaryPath);
            Dictionary = Boggle.m_dictionary;

            List<string> items = new List<string>();
            StreamReader reader = new StreamReader(dictionaryPath);

            while (!reader.EndOfStream)
            {
                items.Add(reader.ReadLine());
            }

            Boggle.m_wordTrie.GenerateFromList(items);
            WordTrie = Boggle.m_wordTrie;

            return solution;
        }
    }
}