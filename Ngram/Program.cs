using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Ngram
{
	internal class Program
	{
		static async Task Main(string[] args)
		{
			string path = @"C:\Users\arty1\source\repos\Study\Ngram\text.txt";

			Console.Write("Enter the size of n gram: ");
			int.TryParse(Console.ReadLine()?.Trim(), out int ngramSize);

			//var result = (await Ngram.Tokenize(path, Encoding.UTF8)).GetNgrams(ngramSize);
			var result = "qwertyuiop".GetNgrams(ngramSize);

			int counter = 0;
			foreach (var ngram in result)
			{
				if (counter == 100)
					break;

				Console.WriteLine(ngram);
			}
		}
	}

	public static class Ngram
	{
		private static Regex _pattern = new Regex("\\W");

		public static async Task<IEnumerable<string>> Tokenize(string path, Encoding encoding)
		{
			var text = await File.ReadAllTextAsync(path, encoding);

			var result = _pattern.Split(text);

			return result;
		}

		/// <summary>
		/// Get ngrams from a word
		/// </summary>
		/// <param name="str">inital word</param>
		/// <param name="size">n-gram size</param>
		/// <returns></returns>
		public static IEnumerable<string> GetNgrams(this string str, int size)
		{
			var ngrams = new List<string>();

			for (int i = 0; i < str.Length - size; i++)
				ngrams.Add(str.Substring(i, size));

			return ngrams;
		}

		/// <summary>
		/// Get N Grams from a text
		/// </summary>
		/// <param name="size">n-gram size</param>
		/// <param name="tokens">tokenized sentence</param>
		/// <returns>list of ngrams</returns>
		public static IEnumerable<string[]> GetNgrams(this IEnumerable<string> tokens, int size)
		{
			if (size < 1)
				throw new ArgumentException();

			var ngrams = new List<string[]>();
			var tokensList = tokens.ToList();

			for (int i = 0; i < tokensList.Count() - size; i++)
			{
				var ngram = new string[size];

				for (int j = 0; j < size; j++)
					ngram[j] = tokensList[i + j];

				ngrams.Add(ngram);
			}

			return ngrams;
		}
	}
}
