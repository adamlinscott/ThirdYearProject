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
			Console.WriteLine("Current dir: " + path);

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
			// Cerate object for first division
			Division div1 = new Division(JSONObject.Division_1);
			div1.roundRobbin();
			Console.ReadKey();
		}
	}
}
