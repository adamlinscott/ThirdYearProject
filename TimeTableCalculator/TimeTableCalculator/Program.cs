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
		static void Main(string[] args)
		{
			// Get the current directory.
			string path = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), @"..\..\..\..\"));
			Console.WriteLine("Current dir: " + path);

			string JSONString = System.IO.File.ReadAllText(path + @"input.json");
			var JSONObject = JsonParser.Deserialize(JSONString);

			Console.ReadKey();
		}
	}
}
