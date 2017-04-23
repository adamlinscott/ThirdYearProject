using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Json;
using System.IO;
using System.Reflection;

namespace TimeTableCalculator
{
	class Program
	{
		public static int totalWeeks;
		public static int divCount;
		public static DateTime startWeek;
		public static DateTime[] bankHolidays;
		public static Division[] divisions;

		public static DateTime ConvertDate( string stringDate )
		{
			int year = Convert.ToInt32(stringDate.Substring(0, 4));
			int month = Convert.ToInt32(stringDate.Substring(5, 2));
			int day = Convert.ToInt32(stringDate.Substring(9, 2));
			return new DateTime(year,month,day);
		}

		static void Main(string[] args)
		{
			// Get the current directory.
			string path = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), @"..\..\..\..\"));

			// Convert JSON file to dynamic C# object
			string JSONString = System.IO.File.ReadAllText(path + @"input.json");
			var JSONObject = JsonParser.Deserialize(JSONString);

			// Set global vars
			totalWeeks = (int) JSONObject.total_weeks;
			startWeek = ConvertDate(JSONObject.start_date);
			bankHolidays = new DateTime[JSONObject.bank_hols.Count];
			for(int i = 0; i > JSONObject.bank_hols.Count; i++)
			{
				bankHolidays[i] = ConvertDate(JSONObject.bank_hols[i]);
			}

			divCount = JSONObject.divisions.Count;
			divisions = new Division[divCount];
			for (int d = 0; d < divCount; d++)
			{
				divisions[d] = new Division(JSONObject.divisions[d]);
			}

			calculateSolutions();

			Console.ReadKey();
		}

		public static int quickFindBestRotation(int d)
		{
			int bestRank = 999;
			Team[] bestOrder = new Team[divisions[d].teams.Length];
			for (int i = 0; i < divisions[d].teams.Length; i++)
			{
				divisions[d].roundRobbin();
				int tempScore = divisions[d].validateSolution();
				if (tempScore < bestRank)
				{
					bestRank = tempScore;
					Array.Copy(divisions[d].teams, bestOrder, bestOrder.Length);
				}
				divisions[d].rotateArray();
			}

			Array.Copy(bestOrder, divisions[d].teams, bestOrder.Length);
			divisions[d].roundRobbin();

			return bestRank;
		}

		public static void calculateSolutions()
		{
			for (int d = 0; d < divCount; d++)
			{
				bool bestFound = false;
				int attempt = 0;
				Team[] bestDivOrder = new Team[divisions[d].teams.Length];
				int bestDivRank = 999;
				while (!bestFound /*&& attempt < 1000*/)
				{
					attempt++;
					int bestRank = quickFindBestRotation(d);
				
					if (divisions[d].bestSolutionRank == bestRank)
					{
						divisions[d].printTable();
						Console.ForegroundColor = ConsoleColor.Green;
						bestFound = true;
					}
					if (bestRank < bestDivRank)
					{
						Array.Copy(divisions[d].teams, bestDivOrder, bestDivOrder.Length);
						bestDivRank = bestRank;
					}
					Console.WriteLine("best solution for attempt " + attempt + " was valued " + bestRank + " (best possible is " + divisions[d].bestSolutionRank + ". Best found is " + bestDivRank + ")");
					Console.ResetColor();
					Console.WriteLine();
					divisions[d].printOrder();
					Console.WriteLine();
					Random rnd = new Random();
					if (!bestFound)
						//divisions[d].fixFirstError();
						divisions[d].swapTeams(rnd.Next(divisions[d].teams.Length), rnd.Next(divisions[d].teams.Length));
				}
			}
		}
	}
}
