
// Test cases look like a mess, but in fact it's very simple: the dictionary key is the character matrix, and the value are the words to find within said matrix; they're only ugly to build manually like this.
var testCases = new Dictionary<string, string[]>();
testCases["a b x t l r w z a b x t l r w z a b x t l r w z a b x t l r w z a b x t l r w z a b x t l r w z a b x t l r w z z w r l t x b a"] = new[] { "abx", "bbb", "abxtlrw", "qypbm", "rwqfdfmnjp", "aa", "aaaaaaa", "b", "bbbb", "zwrltxb", "xx", "tt", "ll", "rr", "ww", "zz" };
testCases["a b x z a b t a b"] = new[] { "", "", "ab", "xb", "bb", "ab", "ab", "ab", "ab", "ab", "ab", "ab", "ab", "ab", "bb", "xb", "az", "bb" };
testCases["a b x z a b t a b t a b t a b"] = new[] { "", "", "ab", "xb", "bb", "ab", "ab", "ab", "ab", "ab", "ab", "ab", "ab", "ab", "bb", "xb", "az", "bb", "tt", "ta", "tab" }; // Non-square!
// The spaces here are just to simplify the creation of the values for the test, they aren't needed if you're passing in a proper IEnumerable<string> that already exists.

foreach(var testCase in testCases)
{
  var tcase = testCase.Key.Split(" ");
  Console.WriteLine("Case size: " + tcase.Length.ToString());
  WordFinder wordFinder = null;
  if (Math.Sqrt(tcase.Length) % 1 == 0)
    wordFinder = new WordFinder(tcase);
  else
    wordFinder = new WordFinder(3, 5, tcase);

  IEnumerable<string> results = wordFinder.Find(testCase.Value);
  foreach(var result in results)
  {
    Console.WriteLine(result);
  }
  Console.WriteLine();
}