using System;
using System.Collections.Generic;
using System.Linq;

public class WordFinder
{
  /* 
    Utilizing two concepts from my notes that you can find in the README file, I recognized that the algorithm for searching horizontally or vertically is exactly the same if you can transpose the matrix, so that's what I do here.
  */
  private StringMatrix _horizontal;
  private StringMatrix _vertical;

  /// <summary>
  /// Primary ctor for the challenge. This assumes it's a square matrix.
  /// </summary>
  /// <param name="characterMatrix">The matrix itself</param>
  public WordFinder(IEnumerable<string> characterMatrix)
  {
    double squareSideCount = Math.Sqrt(characterMatrix.Count());
    Guards.GuardArgumentCheck(squareSideCount % 1 == 0, "Constructor assumes the matrix to be square; see the overloads for non-square matrices.");
    Guards.GuardArgumentCheck(squareSideCount <= 8, "Character matrix is restricted to fewer than 64 characters on either side.");
    Guards.GuardArgumentCheck(squareSideCount > 0, "Character matrix empty."); // Wondering if this is a valid usecase actually, having one of 0 size. For now, we'll just throw, then find out in the "Find Out" phase of the feedback.
    
    int squareSideCountAsInt = (int)squareSideCount; // Shouldn't get past my checks and still do this invalidly

    _horizontal = new StringMatrix(squareSideCountAsInt, squareSideCountAsInt, characterMatrix);
    _vertical = _horizontal.Transpose();
    // Technically, using horizontal and vertical matrices to represent the same matrix feels a little bit like a cheat-code. However, it should cut down on the complexity as well as improve performance of any associated algorithms written for this challenge if they do not have to worry about the matrix shape.
  }

  /// <summary>
  /// Overloaded ctor to account for non-square matrices
  /// </summary>
  /// <param name="columns">Count of columns for the matrix</param>
  /// <param name="rows">Count of rows for the matrix</param>
  /// <param name="characterMatrix">The matrix itself</param>
  public WordFinder(int columns, int rows, IEnumerable<string> characterMatrix)
  {
    Guards.GuardArgumentCheck(columns <= 64, "Column count must be less than 64 for character matrix.");
    Guards.GuardArgumentCheck(rows <= 64, "Row count must be less than 64 for character matrix.");
    Guards.GuardArgumentCheck(columns > 0, "Column count must be greater than 0 for character matrix.");
    Guards.GuardArgumentCheck(rows > 0, "Row count must be greater than 0 for character matrix.");

    _horizontal = new StringMatrix(columns, rows, characterMatrix);
    _vertical = _horizontal.Transpose();
  }

  /// <summary>
  /// Finds the appropriate words from the wordstream within the matrix, searching left-to-right and top-to-bottom. See README for usecases.
  /// </summary>
  /// <param name="wordstream">The stream of words against which to check the matrix</param>
  /// <returns>The list of the top 10 (or an empty list, if no words are found) most-found words from the wordstream</returns>
  public IEnumerable<string> Find(IEnumerable<string> wordstream)
  {
    if (null == wordstream || !wordstream.Any())
      return new List<string>(); // Not sure if this is really a valid usecase, but easy check to skip the entire function.
  
    // Not a huge fan of var, but when the type is so explicit, it's generally fine and shortens the codebase a good amount when dealing with wacky generics
    var wordsAndCounts = new Dictionary<string, int>(); // Will lazily initialize the values here in order to minutely shrink the resulting space if a word isn't found, no entry will be made

    foreach(string word in wordstream)
    {
      if (string.Empty == word)
        continue;

      int wordCount = 0;
      for(int row = 0; row < _horizontal.Rows; row++)
      {
        var horizontalCount = _horizontal.CountInRow(row, word);
        wordCount += horizontalCount;
        var verticalCount = _vertical.CountInRow(row, word);
        wordCount += verticalCount;
      }
      
      if (wordCount > 0)
      {
        wordsAndCounts[word] = wordCount;
      }
    }

    // Normally I wouldn't use LINQ for a challenge involving efficiency (it's awful at being efficient), but its use is minimal here, so not too bad. Never, ever use LINQ in statements like the Matrix class' CountInRow function, where it will rack up multiple allocations and give extremely poor performance.
    IEnumerable<string> results = wordsAndCounts.OrderByDescending(kvp => kvp.Value).Select(kvp => kvp.Key);
    int resultCount = results.Count();
    return resultCount <= 10 ? results : results.Take(10);
  }
}