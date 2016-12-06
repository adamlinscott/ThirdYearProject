using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeTableCalculator
{
	class Division
	{
		int startWeek;
		int totalTeams;
		Team[] teams;

		//private Division* nextDivision;

		//public Division(Object divisionInfo, Division* nextDivision)
		//{
		//	startWeek = (int)divisionInfo.start_week;
		//	totalTeams = (int)divisionInfo.team_num;
		//	teams = new Team[divisionInfo.teams.Count];
		//	this.nextDivision = nextDivision;
		//}

		public Division(dynamic divisionInfo)
		{
			startWeek = (int) divisionInfo.start_week;
			totalTeams = (int) divisionInfo.team_num;
			teams = new Team[divisionInfo.teams.Count];
			for(int i = 0; i < divisionInfo.teams.Count; i++)
			{
				teams[i] = new Team(divisionInfo.teams[i]);
			}
		}

		private void rotateArray()
		{
			Team tempTeam = teams[0];
			for(int i = 1; i < teams.Length; i++)
			{
				teams[i - 1] = teams[i];
			}
			teams[teams.Length - 1] = tempTeam;
		}

		public int[,] roundRobbin()
		{
			int[,] table = new int[2*teams.Length,teams.Length];

			// write heading of table
			Console.Write("Teams ");
			for (int i = 0; i < teams.Length*2 / 10; i++)
				Console.Write(" ");
			Console.Write("||");
			for (int i = 0; i < teams.Length; i++)
				Console.Write((i+1) + " |");
			Console.WriteLine();
			for (int i = 0; i < teams.Length*2 / 10; i++)
				Console.Write("-");
			for (int i = 0; i < teams.Length; i++)
				Console.Write("---");
			Console.WriteLine("--------");


			// for each week that teams play
			for (int week = 0; week < teams.Length*2; week++)
			{
				//y axis label
				Console.Write("Week ");
				for (int i = 0; i < (teams.Length*2 / 10) - ((week+1)/10); i++)
					Console.Write(" ");
				Console.Write((week+1) + "||");


				for(int pair = teams.Length / 2; pair > 0; pair--)
				{
					table[week, (pair + teams.Length - 1 + week) % teams.Length] = teams[teams.Length - pair].id;
					table[week, ((2*teams.Length) - pair + week) % teams.Length] = teams[pair-1].id;
				}

				// print line of week to console
				for(int i = 0; i < teams.Length; i++)
				{
					//TODO: HOME/AWAY NEEDS CHANGING, NOT CORRECT
					if (week < teams.Length)
						Console.Write("H");
					else
						Console.Write("A");
					Console.Write(table[week, i] + "|");
				}
				Console.Write("\n");

				//rotateArray();
			}

			return table;
		}
	}
}
