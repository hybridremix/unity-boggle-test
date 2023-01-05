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
        public static int g_NumBlockSides = 6;
        public static int g_GridSize = 3;
        public static int g_MinWordLength = 3;
        public static int g_MaxWordLength = g_GridSize * g_GridSize;
    }

    public class WordDictionary
    {
        public WordDictionary() { }

        public void CompileDictionary(string path)
        {
            StreamReader reader = new StreamReader(path);

            using (reader = File.OpenText(path))
            {
                // TODO
            }
        }
    }
    public class BoggleSet
    {
        public int SolutionCount = 0;
        public int SolutionPoints = 0;
        public bool IsSolved = false;
        private string DictionaryPath;

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
            DictionaryPath = dictionaryPath;
            GameDictionary.CompileDictionary(DictionaryPath);
        }
    }
}
