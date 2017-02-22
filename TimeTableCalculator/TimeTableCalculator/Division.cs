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
		string name;
		Team[] teams;
		Team[] teamsInOrder;
		Team[,] opponentTable;
		bool[,] homeTable;
		Team[,] fullOpponentTable;
		bool[,] fullHomeTable;

		public Division(dynamic divisionInfo)
		{
			name = (string)divisionInfo["name"];
			startWeek = (int) divisionInfo["start_week"];
			totalTeams = (int) divisionInfo["team_num"];
			teams = new Team[divisionInfo["teams"].Count];
			for(int i = 0; i < divisionInfo["teams"].Count; i++)
			{
				teams[i] = new Team(divisionInfo["teams"][i]);
			}
			teamsInOrder = teams;
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

		public void validateSolution()
		{
			// for each week that teams play
			for (int week = 0; week < Program.totalWeeks; week++)
			{
				Console.Write("\n-----------------------\nWeek " + (week+1) + "\n-----------------------\n");
				// print line of week to console
				for (int i = 0; i < teams.Length; i++)
				{
					if (fullOpponentTable[week, i] == null)
					{
						Console.ForegroundColor = ConsoleColor.Blue;
						Console.WriteLine("Team " + (i+1) + " does not play this week");
						Console.ResetColor();
					}
					else if (teamsInOrder[i].requirements[week] != "None" &&
								((fullHomeTable[week, i] && !teamsInOrder[i].requirements[week].Contains("H")) ||
								(!fullHomeTable[week, i] && !teamsInOrder[i].requirements[week].Contains("A"))) )
					{
						Console.ForegroundColor = ConsoleColor.Red;
						Console.Write("Team " + (i+1) + " cannot play team " + fullOpponentTable[week, i].id);
						if (fullHomeTable[week, i])
							Console.WriteLine(" at Home");
						else
							Console.WriteLine(" Away");
						Console.ResetColor();
					}
					else if (fullHomeTable[week, i])
						Console.WriteLine("Team " + (i + 1) + " plays team " + fullOpponentTable[week, i].id + " at Home");
					else
						Console.WriteLine("Team " + (i + 1) + " plays team " + fullOpponentTable[week, i].id + " Away");
				}
			}
		}


		public void printTable()
		{
			// write heading of table
			Console.Write("Teams ");
			for (int i = 0; i < teams.Length * 2 / 10; i++)
				Console.Write(" ");
			Console.Write("||");
			for (int i = 0; i < teams.Length; i++)
				Console.Write((i + 1) + " |");
			Console.WriteLine();
			for (int i = 0; i < teams.Length * 2 / 10; i++)
				Console.Write("-");
			for (int i = 0; i < teams.Length; i++)
				Console.Write("---");
			Console.WriteLine("--------");

			// for each week that teams play
			for (int week = 0; week < Program.totalWeeks; week++)
			{
				//y axis label
				Console.Write("Week ");
				for (int i = 0; i < (Math.Pow(teams.Length * 2, 1 / 10)) - (Math.Pow(week + startWeek, 1 / 10)); i++)
					Console.Write(" ");
				if (week + startWeek < 10)
					Console.Write(" ");
				Console.Write((week+1) + "||");

				// print line of week to console
				for (int i = 0; i < teams.Length; i++)
				{
					if (fullOpponentTable[week, i] == null)
					{
						Console.ForegroundColor = ConsoleColor.Blue;
						Console.Write("XX");
						Console.ResetColor();
					}
					else if (teams[i].requirements[week] != "None" &&
								((fullHomeTable[week, i] && !teamsInOrder[i].requirements[week].Contains("H")) ||
								(!fullHomeTable[week, i] && !teamsInOrder[i].requirements[week].Contains("A"))))
					{
						Console.ForegroundColor = ConsoleColor.Red;
						if (fullHomeTable[week, i])
							Console.Write("H");
						else
							Console.Write("A");
					}
					else if (fullHomeTable[week, i])
						Console.Write("H");
					else
						Console.Write("A");

					if (fullOpponentTable[week, i] != null)
					{
						Console.Write(fullOpponentTable[week, i].id);
						Console.ResetColor();
						Console.Write("|");
					}
					else
						Console.Write("|");

				}
				Console.Write("\n");
			}
		}


		public Division roundRobbin()
		{
			// tables of size teams by weeks
			opponentTable = new Team[2*teams.Length,teams.Length];
			homeTable = new bool[2 * teams.Length, teams.Length];

			// for each week that teams play
			for (int week = 0; week < teams.Length*2 ; week++)
			{
				Team[] weekPairings = new Team[teams.Length];
				// for each pair of teams in week
				for(int pair = 0; pair < teams.Length / 2; pair++)
				{
					weekPairings[teams[pair].id-1] = teams[teams.Length - pair-1];
					weekPairings[teams[teams.Length - pair-1].id-1] = teams[pair];

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
				foreach( Team t in teams)
				{
					Console.Write(t.id + " ");
				}
				Console.WriteLine();
				rotateArray();
			}


			// full size table including overflow weeks
			fullOpponentTable = new Team[Program.totalWeeks, teams.Length];
			fullHomeTable = new bool[Program.totalWeeks, teams.Length];

			// initilise first weeks to be empty
			for (int week = 0; week < startWeek - 2; week++)
			{
				for (int t = 0; t < teams.Length; t++)
				{
					fullOpponentTable[week, t] = null;
					fullHomeTable[week, t] = false;
				}
			}

			// copy existing schedule
			for (int week = startWeek - 1; week < startWeek + ((teams.Length - 1) * 2); week++)
			{
				for (int t = 0; t < teams.Length; t++)
				{
					fullOpponentTable[week, t] = opponentTable[week - startWeek + 1, t];
					fullHomeTable[week, t] = homeTable[week - startWeek + 1, t];
				}
			}

			// initilise last weeks to be empty
			for (int week = startWeek + ((teams.Length - 1) * 2) - 1; week < Program.totalWeeks; week++)
			{
				for (int t = 0; t < teams.Length; t++)
				{
					fullOpponentTable[week, t] = null;
					fullHomeTable[week, t] = false;
				}
			}

			return this;
		}
	}
}
