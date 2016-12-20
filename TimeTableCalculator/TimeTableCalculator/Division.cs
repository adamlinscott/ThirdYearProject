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

		public Team[,] roundRobbin()
		{
			// tables of size teams by weeks
			Team[,] opponentTable = new Team[2*teams.Length,teams.Length];
			bool[,] homeTable = new bool[2 * teams.Length, teams.Length];

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
			for (int week = 0; week < teams.Length*2 ; week++)
			{
				//y axis label
				Console.Write("Week ");
				for (int i = 0; i < (teams.Length*2 / 10) - ((week+1)/10); i++)
					Console.Write(" ");
				Console.Write((week+1) + "||");

				Team[] weekPairings = new Team[teams.Length];
				// for each pair of teams in week
				for(int pair = teams.Length / 2; pair > 0; pair--)
				{
					weekPairings[pair-1] = teams[teams.Length - pair];
					weekPairings[teams.Length - pair] = teams[pair - 1];

					if(week < teams.Length)
					{
						homeTable[week, (pair + teams.Length - 1 + week) % teams.Length] = true;
						homeTable[week, ((2 * teams.Length) - pair + week) % teams.Length] = false;
					} 
					else
					{
						homeTable[week, (pair + teams.Length - 1 + week) % teams.Length] = false;
						homeTable[week, ((2 * teams.Length) - pair + week) % teams.Length] = true;
					}
				}
				

				// Put weekParings data into opponent table
				for(int i = 0; i < weekPairings.Length; i++)
				{
					if(weekPairings[i] != null)
						opponentTable[week, teams[i].id - 1] = weekPairings[i];
				}


				// print line of week to console
				for (int i = 0; i < teams.Length; i++)
				{
					if (opponentTable[week, i] == null)
						Console.Write("XX");
					else if (homeTable[week, i])
						Console.Write("H");
					else
						Console.Write("A");

					if (opponentTable[week, i] != null)
						Console.Write(opponentTable[week, i].id + "|");
					else
						Console.Write("|");
				}
				Console.Write("\n");


				rotateArray();
			}

			return opponentTable;
		}
	}
}
