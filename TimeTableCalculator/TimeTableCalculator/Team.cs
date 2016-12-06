using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeTableCalculator
{
	class Team
	{
		public int id;
		public string name;
		string[] requirements;

		public Team(dynamic teamInfo)
		{
			id = (int) teamInfo["num"];
			name = teamInfo["name"];
			requirements = new string[Program.totalWeeks];
		}
	}
}
