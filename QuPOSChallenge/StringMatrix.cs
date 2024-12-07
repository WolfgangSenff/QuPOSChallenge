using System;
using System.Collections.Generic;
using System.Text;

/// <summary>
/// Matrix record to represent data in a matrix format; being a record makes it fairly fast and safe; could use generics, but converting generics to the method I'm using to create the data and put it into my matrix is wrought with edge-cases, so instead focus on strings only
/// </summary>
/// <typeparam name="T">Structs only for the time being</typeparam>
/// <param name="Columns">The column count</param>
/// <param name="Rows">The row count</param>
/// <param name="SeedData">The actual data in the matrix</param>
public record StringMatrix(int Columns, int Rows, IEnumerable<string> SeedData)
{
  Dictionary<string, int> _cache = new Dictionary<string, int>();
  public string[] Data { get; } = CreateData(Columns, Rows, SeedData);

  #region Public methods
  /// <summary>
  /// Transposes the matrix and returns a new StringMatrix instance.
  /// </summary>
  /// <returns>A new transposed StringMatrix.</returns>
  public StringMatrix Transpose()
  {
    var transposedValues = new string[Rows * Columns];

    for (int row = 0; row < Rows; row++)
    {
        for (int col = 0; col < Columns; col++)
        {
            transposedValues[col * Rows + row] = Data[row][col].ToString();
        }
    }

    return new StringMatrix(Rows, Columns, transposedValues);
  }

  /// <summary>
  /// Count 'word' instances in a given row of the matrix
  /// </summary>
  /// <param name="row">The row in which to search</param>
  /// <param name="word">The word to search for</param>
  /// <returns>The count</returns>
  public int CountInRow(int row, string word)
  {
    if (row >= Data.Count()) return 0;
    if (string.IsNullOrEmpty(word)) return 0; // Choosing to count this as not valid, but not throwing an error either, just returning 0 and saying the empty string is not found; could do the opposite, saying it's always found, but prefer not to.
    
    // Above is faster even than the cache so ignore the cache
    int count;
    bool cacheContains = _cache.TryGetValue(word, out count);
    if (cacheContains) return count; // Added caching when I realized the performance, while still very good for very large matrices and word lists, is slowed down by repeat words in the word list. This wasn't in my original notes, seemed good to add it though, since caching often fixes lots of perf. issues.

    int index = 0;
    string rowWord = Data[row];
    while(-1 != (index = rowWord.IndexOf(word, index)))
    { // This weird-looking loop ensures that looking for "aa" in "aaaaaaaaa" returns 12 instead of a replacement method, which would return only 6.
      count++;
      index++;
    }
    return count;
  }
  #endregion Public methods

  #region Private methods
  /// <summary>
  /// Creates the backing data array from the original input values.
  /// </summary>
  /// <param name="columns">The column count.</param>
  /// <param name="rows">The row count.</param>
  /// <param name="values">The original enumerable of single-character strings.</param>
  /// <returns>The backing data array.</returns>
  private static string[] CreateData(int columns, int rows, IEnumerable<string> values)
  {
      Guards.GuardArgumentCheck(columns * rows == values.Count(), "The number of values does not match the specified matrix dimensions.");

      string[] data = new string[rows];
      var valueArray = values.ToArray();

      for (int row = 0; row < rows; row++)
      {
        // Use a StringBuilder, as it's faster and more effective than plain ol' string concatenation
        var builder = new StringBuilder(columns);
        for (int col = 0; col < columns; col++)
        {
            builder.Append(valueArray[row * columns + col]);
        }
        data[row] = builder.ToString();
      }

      return data;
  }
  #endregion Private methods

  public override string ToString()
  {
    var result = new StringBuilder();
    foreach(var word in Data)
    {
      result.Append(word);
      result.Append("\n");
    }

    return result.ToString();
  }
}