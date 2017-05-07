using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Json;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;

namespace TimeTableCalculator
{
	class Program
	{
		public static int totalWeeks;
		public static int divCount;
		public static DateTime startWeek;
		public static DateTime[] bankHolidays;
		public static Division[] divisions;
		public static string input;
		public static int totalTeamsNumber;

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
			totalTeamsNumber = 0;
			divCount = JSONObject.divisions.Count;
			divisions = new Division[divCount];
			for (int d = 0; d < divCount; d++)
			{
				divisions[d] = new Division(JSONObject.divisions[d]);
				totalTeamsNumber += divisions[d].totalTeams;
			}
			startMenu();
		}


		static void startMenu()
		{
			Console.Clear();
			Console.ForegroundColor = ConsoleColor.Yellow;
			Console.WriteLine("\t| Root Menu |\n\n");
			Console.ResetColor();

			Console.WriteLine("Please select from the following options:\n");


			Console.WriteLine("1: Show data info");
			Console.WriteLine("2: Random search fast ({0} solution calculations maximum)", totalTeamsNumber * 10);
			Console.WriteLine("3: Random search slow ({0} solution calculations maximum)", totalTeamsNumber * 1000);
			Console.WriteLine("Q/q: Close program\n");

			input = Regex.Replace(Console.ReadLine().ToLower(), @"\s", "");


			if (input == "1" || input == "datainfo")
			{
				Console.Clear();
				ShowData();
				startMenu();
			}
			else if (input == "2")
			{
				Console.Clear();
				calculateSolutions();
				startMenu();
			}
			else if (input == "3")
			{
				Console.Clear();
				calculateSolutions(1000);
				startMenu();
			}
			else if (input == "4")
			{
				Console.Clear();
				startMenu();
			}
			else if (input == "q" || input == "quit" || input == "close" || input == "end")
			{
				return;
			}
			else
				startMenu();
		}


		static void ShowData()
		{

			Console.ForegroundColor = ConsoleColor.Yellow;
			Console.WriteLine("\t| Data Information |\n\n");
			Console.ResetColor();

			Console.WriteLine("Total number of weeks: {0}\n", totalWeeks);

			Console.WriteLine("Number of divisions: {0}\n", divCount);

			foreach (Division div in divisions)
			{
				Console.WriteLine("{0}:", div.name);
				foreach (Team t in div.teams)
				{
					Console.WriteLine("\tTeam {0}: {1}", t.id, t.name);
				} Console.WriteLine();
			} Console.WriteLine();

			Console.WriteLine("Press any key to return to menu");
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

		public static void calculateSolutions( int maxAttempts = 10 )
		{
			for (int d = 0; d < divCount; d++)
			{
				Console.WriteLine("\n\nSolving {0}", divisions[d].name);
				bool bestFound = false;
				int attempt = 0;
				Team[] bestDivOrder = new Team[divisions[d].teams.Length];
				int bestDivRank = 999;
				while (!bestFound && attempt < maxAttempts)
				{
					attempt++;
					Console.SetCursorPosition(0, Console.CursorTop);
					Console.Write(new string(' ', Console.WindowWidth));
					Console.SetCursorPosition(0, Console.CursorTop - 1);
					Console.Write("Progress: |");
					for(int i = 0; i < 100*attempt/maxAttempts; i++)
					{
						Console.Write("#");
					}
					for (int i = 0; i < (100 * (maxAttempts - attempt) / maxAttempts); i++)
					{
						Console.Write(" ");
					}
					Console.Write("|");

					int bestRank = quickFindBestRotation(d);
				
					if (divisions[d].bestSolutionRank == bestRank)
					{
						Console.SetCursorPosition(0, Console.CursorTop);
						Console.Write(new string(' ', Console.WindowWidth));
						Console.SetCursorPosition(0, Console.CursorTop - 1);
						Console.Write("Progress: |");
						for (int i = 0; i < 100; i++)
						{
							Console.Write("#");
						}
						Console.Write("|");
						bestFound = true;
					}
					if (bestRank < bestDivRank)
					{
						Array.Copy(divisions[d].teams, bestDivOrder, bestDivOrder.Length);
						bestDivRank = bestRank;
					}
					Random rnd = new Random();
					if (!bestFound)
						//divisions[d].fixFirstError();
						divisions[d].swapTeams(rnd.Next(divisions[d].teams.Length), rnd.Next(divisions[d].teams.Length));
				}

				Array.Copy(bestDivOrder, divisions[d].teams, bestDivOrder.Length);
				divisions[d].roundRobbin();
			}

			while (true)
			{
				Console.Clear();

				Console.ForegroundColor = ConsoleColor.Yellow;
				Console.WriteLine("\t| Search Menu |\n\n");
				Console.ResetColor();

				Console.WriteLine("Please select from the following options:\n");


				Console.WriteLine("1: Show best found solutions");
				Console.WriteLine("2: Show completed division timetables");
				Console.WriteLine("3: Back to main menu");
				Console.WriteLine("Q/q: Close program\n");

				input = Regex.Replace(Console.ReadLine().ToLower(), @"\s", "");


				if (input == "1" || input == "best")
				{
					Console.Clear();

					foreach(Division d in divisions)
					{
						d.printTable();
						int rank = d.validateSolution();
						if (d.bestSolutionRank == rank)
						{
							Console.ForegroundColor = ConsoleColor.Green;
						}
						Console.Write("best solution was valued {0} (best possible is {1}).\nRound robin with input order ", rank, d.bestSolutionRank);
						d.printOrder();
						Console.WriteLine();
						Console.ResetColor();
					}

					Console.WriteLine("Press any key to return to menu");
					Console.ReadKey();
				}
				else if (input == "2")
				{
					Console.Clear();

					foreach (Division d in divisions)
					{
						int rank = d.validateSolution();
						if (d.bestSolutionRank == rank)
						{
							d.printTable();
							Console.ForegroundColor = ConsoleColor.Green;
							Console.Write("best possible solution found was valued {0}.\nRound robin with input order ", rank);
							d.printOrder();
							Console.WriteLine();
						}
						Console.ResetColor();
					}

					Console.WriteLine("Press any key to return to menu");
					Console.ReadKey();
				}
				else if (input == "3")
				{
					Console.Clear();
					return;
				}
				else if (input == "q" || input == "quit" || input == "close" || input == "end")
				{
					System.Environment.Exit(0);
				}
			}
		}
	}
}
