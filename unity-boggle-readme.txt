This custom Boggle project is derived from the standard Boggle game many have played in their
living rooms. The rules can be found on Wikipedia: https://en.wikipedia.org/wiki/Boggle.

The goal of this test is to analyze your ability to write two distinct algorithms with efficient
performance using C# and Unity. When first loading the project and playing the BoggleCanvas scene,
hit the SOLVE button and notice that the result string appears immediately. A correct solution
will produce accurate results without slowing this performance.

The collins_scrabble_words.txt file is your dictionary. A word cannot be counted valid unless it
appears on this word list. Points are calculated according to word length. Both the standard word
construction and point calculation rules can be found on the Rules section of the Wikipedia page.

There are two TODOs in the code that set your starting points:
 - 1) The WordDictionary class is where the list of reference words is constructed. Use the
      CompileDictionary() method to centralize your import logic.
 - 2) The GridInterface class is where the UI is constructed. The SOLVE button will look for the
      solution algorithm in the SolveOnClick() method.

Other expected results:
- The solution should maintain its performance even when increasing the BoggleGlobals.GridSize up
  to 6, 8, even 10 or 12.
- The results statement, "This grid contains <X> words for a total of <Y> points!" should print
  correct word count and point totals from your solver.
- There should be some reasonable commenting throughout the solution to explain the logic.

Solve the two algorithms in nearly any way that you prefer. There is almost no sacred code in the
project. As long as the core functionality does not break (grid layout, shake functionality,
button behaviors, etc.), feel free to add logic wherever you would like.

Feel free to embellish your project as much as you like if you are interested in showing off
additional skills. Particularly impressive would be adding unit tests for your own functions in
the BoggleTests script.