# QuPOSChallenge

# Prompt
Presented with a character matrix and a large stream of words, your task is to create a Class
that searches the matrix to look for the words from the word stream. Words may appear
horizontally, from left to right, or vertically, from top to bottom. In the example below, the word
stream has four words and the matrix contains only three of those words ("chill", "cold" and
"wind"):

The search code must be implemented as a class with the following interface:
public class WordFinder
{
public WordFinder(IEnumerable<string> matrix) {
...
}
public IEnumerable<string> Find(IEnumerable<string> wordstream)
{ ...
}
}
The WordFinder constructor receives a set of strings which represents a character matrix. The
matrix size does not exceed 64x64, all strings contain the same number of characters. The
"Find" method should return the top 10 most repeated words from the word stream found in the
matrix. If no words are found, the "Find" method should return an empty set of strings. If any
word in the word stream is found more than once within the stream, the search results
should count it only once
Due to the size of the word stream, the code should be implemented in a high performance
fashion both in terms of efficient algorithm and utilization of system resources. Where possible,
please include your analysis and evaluation.

# Kyle Notes
It doesn't specify that the matrix has to be a square matrix, even though the example shows a square matrix. For the time being, we'll assume a square matrix and hence the matrix span will equal the row count.
However, I'll encode it to the public interface defined but allow for overloads that account for non-square matrices. Here's the use-cases:

Givens:
1. An IEnumerable<string> representing the matrix of letters within which to search, similar to a word-find puzzle; matrix size *shall not* exceed 64x64.
2. A Find method which takes in a wordstream, i.e. the values that exist (or don't) in a word-find puzzle, and returns the top 10 most-repeated words from the word-stream that were found in the matrix.

Cases:
1. Find method shall search vertically from top to bottom and horizontally from left to right within the given character matrix to find words from word stream
2. Find method shall mark words that are found while adding to the count (number of times they are found) as repeats are found (programmer note: consider lazy initialization for this list, to ensure that any words that aren't found at all are not checked for the final 10)
3. Find method shall return the top 10 most-repeated words from the word stream found within the character matrix
4. Edgecase: Find method shall assume that each character matrix is square (programmer's note: this can still contain overrides that can check non-square matrices, no reason to assume otherwise, so long as they adhere to smaller than 64x64 as well)
5. Edgecase: Find method shall *not* assume characters *cannot* be empty (programmer's note: I considered sanitizing input, but given that empty characters feels like an edgecase, and each character is clearly delineated in the character matrix, I think the search algorithm should remain exactly the same whether or not they're sanitized, and given santizing the input would take time and clock cycles, let's avoid it for now)
6. Find method shall return an empty iterable if no words from the list are found in the matrix
7. Find method shall not consider words exceeding 64 characters in length (programmer's note: add a guard statement to bail out early if one exceeds that length to ensure no unnecessary search is done on words that are obviously too long)
8. Find method shall take square root of character matrix count to determine column/row count; if the value is not round, the matrix is not square
9. Find method shall *not* assume case-sensitivity, though it is easy enough to add

Some more programmer notes for my own sanity:
1. Check word-length vs remaining number of columns and rows; if it's higher than whatever is being checked, bail out early
2. Guess at a minimum number of nested loops is 2, but if it's possible with 2, it won't be easy. Since no requirements on RAM are listed, could conceivably consider building all "words" - valid or invalid - from any given matrix and then just counting equal values from the word stream; however, at 64x64, the number of words would be massive, so should not do this. This also prevents the use of some optimizations dealing with column and row count checks and early bail outs, as it would just do all the work brute-force up front.
3. Two separate loops using different matrix traversal methods would work: one for horizontal, one for vertical. Consider if such a design is better or just fancier; requires the bail-out checks to switch which way they're counting, columns vs rows, so might not be worth it over just copy/pasting and changing the code.
4. Another option similar to 3 would be to actually just transpose the matrix and run the same exact algorithm on it. Check if matrices are iterable in C# and consider converting to one in order to support such operations, as this would let you focus on just one algorithm. Further, while we don't know the big O of Microsoft's matrix transpose, it's surely pretty good, and always best to rely on things the language team has implemented in such cases. And finally, this would allow you to just make both matrices immediately and then run the algorithm on both using threads, ensuring that results will be as fast as possible and not dependent on each other; just have to ensure that the results are independent and then put back together, sort of like a map reduce calculation (specifically reducing the results).
5. Thinking about 4, would map-reduce just solve this issue way more easily? It seems like if you took the matrix and its transpose and looped over values and mapped the word inputstream, then reduced the results together for the counts, that would solve the issue. Consider that as you write more.
6. Thinking through the problem, is it possible to solve this with preprocessing? Just take the row length, put the letters from said row together into a string, add the strings to a separate iterable, then just loop over the word inputstream counting the instances of the word within the main string. AsSpan() on a string can be used for ultra-fast searching of said string, ensuring contiguous memory allocation. Actually, Span<T> takes in an array of values, so don't even have to convert it to a string in the first place, sweet! That'll be a lot faster and memory minimal, as well as easier to code, since Count() is extended to it due to the implementation.
7. For the matrix of:
```
aaa
aaa
aaa
```

And a word input stream of `aa`, would this return 12, or 6? Unclear based on the requirements.
8. After some research, it seems like Matrix<T> doesn't exist, so make it.
