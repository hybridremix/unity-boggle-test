using System.IO;
using UnityEngine;

namespace Boggle
{
    public class WordDictionary
    {
        public WordDictionary() { }

        public void CompileDictionary(string path)
        {
            using (StreamReader reader = File.OpenText(path))
            {
                // TODO :: Import word list and compile an efficient, searchable data structure.
            }
        }
    }
    public class BoggleSet
    {
        public int SolutionCount = 0;
        public int SolutionPoints = 0;
        public bool IsSolved = false;

        [HideInInspector] public WordDictionary GameDictionary;
        [HideInInspector] public BlockSet GameBlocks;

        public BoggleSet()
        {
            GameDictionary = new WordDictionary();
            GameBlocks = new BlockSet();
        }
        public BoggleSet(string dictionaryPath)
        {
            GameDictionary = new WordDictionary();
            GameBlocks = new BlockSet();
            GameDictionary.CompileDictionary(dictionaryPath);
        }
    }
}
