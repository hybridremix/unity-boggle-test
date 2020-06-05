using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using NUnit.Framework.Internal;
using UnityEngine.TestTools;
using Boggle;
using System.Linq;

namespace Tests
{
    public class BoggleTests
    {
        private MyBoggleSolution    MyBoggle;
        private readonly char[,]    UnityBoard;
        private readonly string     DictionaryPath;

        public BoggleTests()
        {
            DictionaryPath = "Assets/collins_scrabble_words.txt";
            UnityBoard = new char[3, 3] {   { 'D', 'Z', 'X', },
                                            { 'E', 'A', 'I', },
                                            { 'Q', 'U', 'T'  }   };
        }

        // A Test behaves as an ordinary method
        [Test]
        public void TestBoggleConstruction()
        {
            // Use the Assert class to test conditions
            char[,] testBoard = new char[3, 3] {    { 'D', 'Z', 'X', },
                                                    { 'E', 'A', 'I', },
                                                    { 'Q', 'U', 'T'  }   };

            MyBoggle = new MyBoggleSolution(true, UnityBoard);
            Assert.IsTrue(MyBoggle != null);

            MyBoggleSolution.CreateSolver(DictionaryPath);
            Assert.IsTrue(MyBoggle.GetWordTrie() != null);
        }
        [Test]
        public void TestFindWords()
        {
            // Use the Assert class to test conditions
            MyBoggle = new MyBoggleSolution(true, UnityBoard);
            ISolver solver = MyBoggleSolution.CreateSolver(DictionaryPath);
            IResults results = solver.FindWords(UnityBoard);
            Assert.IsTrue(results.Score > 0);
            Assert.IsTrue(results.Words != null);
        }
        [Test]
        public void TestUnityWords()
        {
            // Use the Assert class to test conditions
            MyBoggle = new MyBoggleSolution(true, UnityBoard);
            ISolver solver = MyBoggleSolution.CreateSolver(DictionaryPath);
            IResults results = solver.FindWords(UnityBoard);

            List<string> unityWordlist = new List<string> { "adz", "adze", "ait", "daut", "daze", "eat", "eau", "qua", "quad", "quai",
                                                            "tad", "tae", "tau", "tax", "taxi", "tui", "tuque", "uta", "zax", "zed", "zit" };
            bool wordlistCheck = true;
            for (int i = 0; i < unityWordlist.Count; i++)
            {
                unityWordlist[i] = unityWordlist[i].ToUpper();
                if (!results.Words.Contains(unityWordlist[i]))
                {
                    wordlistCheck = false;
                    break;
                }
            }
            Assert.IsTrue(wordlistCheck);
        }
    }
}
