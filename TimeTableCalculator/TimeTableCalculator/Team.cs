namespace TimeTableCalculator
{
	class Team
	{
		public int id;
		public string name;
		public string[] requirements;

		public Team(dynamic teamInfo)
		{
			id = (int) teamInfo["num"];
			name = teamInfo["name"];
			requirements = new string[Program.totalWeeks];
			for(int i = 0; i < Program.totalWeeks; i++)
			{
				requirements[i] = teamInfo["requirements"][i];
			}
		}
	}
}
