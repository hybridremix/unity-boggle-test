using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using NUnit.Framework.Internal;
using UnityEngine.TestTools;
using Boggle;
using BoggleInterface;

namespace Tests
{
    public class BoggleTests
    {
        private MyBoggleSolution mySolution;

        // A Test behaves as an ordinary method
        [Test]
        public void TestBoggleConstruction()
        {
            // Use the Assert class to test conditions
            char[,] testBoard = new char[3, 3] {    { 'D', 'Z', 'X', },
                                                    { 'E', 'A', 'I', },
                                                    { 'Q', 'U', 'T'  }   };

            mySolution = new MyBoggleSolution(true, testBoard);
            Assert.IsTrue(mySolution != null);

            MyBoggleSolution.CreateSolver("Assets/collins_scrabble_words.txt");
            Assert.IsTrue(mySolution.GetDictionary() != null);
        }
        [Test]
        public void TestFindWords()
        {
            // Use the Assert class to test conditions
            char[,] testBoard = new char[3, 3] {    { 'D', 'Z', 'X', },
                                                    { 'E', 'A', 'I', },
                                                    { 'Q', 'U', 'T'  }   };

            mySolution = new MyBoggleSolution(true, testBoard);
            ISolver solver = MyBoggleSolution.CreateSolver("Assets/collins_scrabble_words.txt");
            IResults results = solver.FindWords(testBoard);
            Assert.IsTrue(results.Score > 0);
            Assert.IsTrue(results.Words != null);
        }
    }
}
