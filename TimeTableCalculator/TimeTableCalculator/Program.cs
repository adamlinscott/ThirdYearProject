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


			divisions = new Division[JSONObject.divisions.Count];
			divisions[0] = new Division(JSONObject.divisions[0]);
			int bestRank = 999;
			Team[] bestOrder = new Team[divisions[0].teams.Length];
			for(int i = 0; i < 10; i++)
			{
				divisions[0].roundRobbin();
				int tempScore = divisions[0].validateSolution();
				if(tempScore < bestRank)
				{
					bestRank = tempScore;
					bestOrder = divisions[0].teams;
				}
				divisions[0].printTable();
				divisions[0].printOrder();
				divisions[0].rotateArray();
				Console.WriteLine();
			}

			Console.WriteLine("best solution was valued " + bestRank);
			divisions[0].teams = bestOrder;
			divisions[0].printOrder();

			Console.ReadKey();
		}
	}
}
