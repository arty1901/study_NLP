using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace VectorDocument
{
	internal class Program
	{
		static void Main(string[] args)
		{
			var doc1 = "The Carolina Hurricanes won the Stanley Cup";
			var doc2 = "The Minnesota Twins won the World Series";

			var doc3 =
				"Lorem ipsum dolor sit amet, consectetur adipiscing elit. Etiam cursus, nulla quis sagittis sagittis, eros ipsum cursus mi, nec hendrerit.";
			var doc4 =
				"Lorem ipsum dolor sit amet, consectetur adipiscing elit. Aliquam posuere nisi placerat luctus ultricies. Pellentesque malesuada, quam vitae posuere accumsan.";

			var vectorDocument = new VectorDocument(doc3, doc4);
			vectorDocument.CalcDocumentsVectors();

			Console.WriteLine(vectorDocument.PrintCommonTerms());
			Console.WriteLine(vectorDocument.ToString());
		}
	}

	public class VectorDocument
	{
		private List<string> CommonTerms {  get; set; }
		private static Regex _pattern = new ("\\W");

		private readonly List<string> _firstDocument;
		private readonly List<string> _secondDocument;

		private List<byte> _firstDocumentVector;
		private List<byte> _secondDocumentVector;

		public VectorDocument(string firstDocument, string secondDocument)
		{
			_firstDocument = _pattern.Split(firstDocument.ToLower()).ToList();
			_secondDocument = _pattern.Split(secondDocument.ToLower()).ToList();

			CommonTerms = _firstDocument.Union(_secondDocument).ToList();
		}

		/// <summary>
		/// Высчитываем вектора документов
		/// </summary>
		public void CalcDocumentsVectors()
		{
			_firstDocumentVector = new List<byte>(ProcessDocument(_firstDocument));
			_secondDocumentVector = new List<byte>(ProcessDocument(_secondDocument));
		}

		/// <summary>
		/// Обработка документов
		/// </summary>
		/// <param name="doc"></param>
		/// <returns></returns>
		private byte[] ProcessDocument(List<string> doc)
		{
			var resultVector = new byte[CommonTerms.Count];

			for (int i = 0; i < CommonTerms.Count; i++)
				resultVector[i] = doc.Contains(CommonTerms[i]) ? (byte)1 : (byte)0;

			return resultVector;
		}

		/// <summary>
		/// Печать терминов
		/// </summary>
		/// <returns>Строка терминов</returns>
		public string PrintCommonTerms()
		{
			var builder = new StringBuilder();

			builder.Append("Common terms: ");
			foreach (var commonTerm in CommonTerms)
			{
				builder.Append($"{commonTerm}, ");
			}

			return builder.ToString();
		}
		
		/// <summary>
		/// Переопределяем ToString метод
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return BuildForString("First vector: <", _firstDocumentVector) +
			       BuildForString("Second vector: <", _secondDocumentVector);
		}

		/// <summary>
		/// Обработка списка к читаемому виду
		/// </summary>
		/// <param name="name"></param>
		/// <param name="vector"></param>
		/// <returns>Текст с вектором документа</returns>
		private string BuildForString(string name, List<byte> vector)
		{
			var builder = new StringBuilder();

			builder.Append(name);
			for (int i = 0; i < vector.Count; i++)
			{
				builder.Append(i == CommonTerms.Count - 1
					? $"{vector[i]}> \n"
					: $"{vector[i]},");
			}

			return builder.ToString();
		}
	}
}
