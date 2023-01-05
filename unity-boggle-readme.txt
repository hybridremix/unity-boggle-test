This custom Boggle project is derived from the standard Boggle game many have played in their
living rooms. The rules can be found on Wikipedia: https://en.wikipedia.org/wiki/Boggle.

The goal of this test is to analyze your ability to write an algorithm for solving a Boggle round
with efficient performance using C# and Unity. The performance standard is set by the initial
functionality. After first loading the project and playing the BoggleCanvas scene, hit the SOLVE
button and notice that the result string appears immediately. A correct solution will produce the
result string with identical performance.

The collins_scrabble_words.txt file is your dictionary. A word cannot be counted unless it appears
on this word list. You will need to 

There are two TODOs in the code that set your starting points:
 - 1) The WordDictionary class is where the list of reference words is imported. The import logic
      
 - 2) The GridInterface class is where the UI is constructed. The SolveOnClick() method is where
      that SOLVE button will look for the solution.

Other fuctional expectations for a correct solution:
- The results statement, "This grid contains 0 words for a total of 0 points!" should print the
  word count and points totals from your solver.
- TODO

Feel free to embellish your project as much as you like if you are interested in showing off
additional skills. Particularly impressive would be adding unit tests for your own functions in
the BoggleTests script.