using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using static System.Math;

namespace LargestSubstring
{
    class Program
    {
        static void Main(string[] args)
        {
            List<string> resultList = new List<string>();
            //Read values from file and store the data into a jagged array to handle as a matrix
            char[][] matrix = File.ReadAllLines(@$"{Path.GetDirectoryName(AppDomain.CurrentDomain.BaseDirectory)}/Data/matrix.txt")
                   .Select(l => l.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(i => char.Parse(i.Trim())).ToArray())
                   .ToArray();

            //Validate if it is a square matrix
            if (IsSquare(matrix))
            {
                //Read subStrings from rows, columns and diagonals
                GetSubStringsFromRowsCols(matrix, 'R', ref resultList);
                GetSubStringsFromRowsCols(matrix, 'C', ref resultList);
                GetSubStringsFromMainDiagonals(matrix, ref resultList);
                GetSubStringsFromSecondaryDiagonals(matrix, ref resultList);

                //Get the largest string(s) and print them
                var maxLength = resultList.Max(s => s.Length);
                var maxSubstrings = resultList.Where(s => s.Length == maxLength);
                if (maxSubstrings.Count() == 1)
                    Console.WriteLine($"The largest substring in the Matrix is: {maxSubstrings.First()}");
                else
                    Console.WriteLine($"The largest substrings in the Matrix are: {string.Join(",", maxSubstrings.ToArray())}");

            }
            else
                Console.WriteLine("There is not a valid square matrix to process!");

            Console.ReadLine();
        }

        /// <summary>
        /// Get the substrings from a matrix looping by rows or columns.
        /// </summary>
        /// <param name="matrix">Matrix to be iterated.</param>
        /// <param name="type">R => to iterate by rows, C => to iterate by columns.</param>
        /// <param name="resultList">List to handle the result of the funtionality.</param>
        private static void GetSubStringsFromRowsCols(char[][] matrix, char type, ref List<string> resultList)
        {
            var length = matrix.Length;
            var value = char.MinValue;
            var count = 0;
            //Loop matrix by rows or cols
            for (int i = 0; i < length; i++)
            {
                for (int j = 0; j < length; j++)
                {
                    var currentValue = type == 'R' ? matrix[i][j] : matrix[j][i];
                    HandleAdjacentValidation(ref resultList, currentValue, ref value, ref count, j, length);
                }
                value = char.MinValue;
                count = 0;
            }
        }

        /// <summary>
        /// Get the substrings from a matrix looping by the secondary diagonals.
        /// </summary>
        /// <param name="matrix">Matrix to be iterated.</param>
        /// <param name="resultList">List to handle the result of the funtionality.</param>
        private static void GetSubStringsFromSecondaryDiagonals(char[][] matrix, ref List<string> resultList)
        {
            var matrixLength = matrix.Length;
            var length = (matrixLength * 2) - 1;
            var value = char.MinValue;
            var count = 0;
            //Loop matrix by Secondary diagonals
            for (int i = 1; i <= length; i++)
            {
                int col = Max(0, i - matrixLength);
                int charQty = Min(i, Min((matrixLength - col), matrixLength));

                if (charQty > 1)
                {
                    for (int j = 0; j < charQty; j++)
                    {
                        var currentValue = matrix[Min(matrixLength, i) - j - 1][col + j];
                        HandleAdjacentValidation(ref resultList, currentValue, ref value, ref count, j, charQty);
                    }
                }
                value = char.MinValue;
                count = 0;
            }
        }

        /// <summary>
        /// Get the substrings from a matrix looping by the main diagonals.
        /// </summary>
        /// <param name="matrix">Matrix to be iterated.</param>
        /// <param name="resultList">List to handle the result of the funtionality.</param>
        private static void GetSubStringsFromMainDiagonals(char[][] matrix, ref List<string> resultList)
        {
            var matrixLength = matrix.Length;
            var length = (matrixLength * 2) - 1;
            var value = char.MinValue;
            var count = 0;
            //Loop matrix by Main diagonals
            for (int i = 1; i <= length; i++)
            {
                int col = Min(matrixLength - 1, length - i);
                int charQty = Max(i, Max((col - matrixLength), 0));

                if (charQty > matrixLength)
                {
                    charQty = matrixLength - (charQty - matrixLength);
                }
                if (charQty > 1)
                {
                    for (int j = 0; j < charQty; j++)
                    {
                        var currentValue = matrix[Min(matrixLength, i) - j - 1][col - j];
                        HandleAdjacentValidation(ref resultList, currentValue, ref value, ref count, j, charQty);
                    }
                }
                value = char.MinValue;
                count = 0;
            }
        }

        /// <summary>
        /// Common functionality to validate the adjacent values.
        /// </summary>
        /// <param name="resultList">List to handle the result of the funtionality.</param>
        /// <param name="currentValue">Current value to evaluate.</param>
        /// <param name="existingValue">Existing value to evaluate.</param>
        /// <param name="count">Number of times of the existing value.</param>
        /// <param name="currentIndex">Current index to handle end of the complete string.</param>
        /// <param name="length">Length of the complete string.</param>
        private static void HandleAdjacentValidation(ref List<string> resultList, char currentValue, ref char existingValue, ref int count, int currentIndex, int length)
        {
            if (existingValue == char.MinValue || existingValue == currentValue)
            {
                existingValue = currentValue;
                count++;
                if (currentIndex == length - 1)
                {
                    GenerateStringValue(resultList, existingValue, count);
                }
            }
            else
            {
                GenerateStringValue(resultList, existingValue, count);
                existingValue = currentValue;
                count = 1;
            }
        }

        /// <summary>
        /// Common functionality to set the substring into the result list.
        /// </summary>
        /// <param name="resultList">List to handle the result of the funtionality.</param>
        /// <param name="value">Value to be generated.</param>
        /// <param name="count">Number of times of the value in the string.</param>
        private static void GenerateStringValue(List<string> resultList, char value, int count)
        {
            if (count > 1)
            {
                var result = new string(value, count);
                if (!resultList.Contains(result))
                {
                    resultList.Add(new string(value, count));
                }
            }
        }

        /// <summary>
        /// Validate if it is a square matrix
        /// </summary>
        /// <param name="matrix">Matrix to be evaluated.</param>
        /// <returns>True if iit is square otherwise false.</returns>
        private static bool IsSquare(char[][] matrix)
        {
            bool isSquare = false;

            if (matrix != null)
            {
                for (int i = 0; i < matrix.Length; i++)
                {
                    if (matrix[i].Length != matrix.Length)
                        break;
                    else if (i != matrix.Length - 1)
                        continue;
                    else
                        isSquare = true;
                }
            }

            return isSquare;
        }
    }
}
