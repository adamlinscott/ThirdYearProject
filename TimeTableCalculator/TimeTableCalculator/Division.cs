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

			// full size table including overflow weeks
			Team[,] newOpponentTable = new Team[Program.totalWeeks, teams.Length];
			bool[,] newHomeTable = new bool[Program.totalWeeks, teams.Length];

			// initilise first weeks to be empty
			for(int week = 0; week < startWeek-1; week++)
			{
				for (int t = 0; t < teams.Length; t++)
				{
					newOpponentTable[week, t] = null;
					newHomeTable[week, t] = false;
				}
			}

			// copy existing schedule
			for (int week = startWeek; week < startWeek + ((teams.Length -1) * 2); week++)
			{
				for (int t = 0; t < teams.Length; t++)
				{
					newOpponentTable[week, t] = opponentTable[week-startWeek, t];
					newHomeTable[week, t] = homeTable[week - startWeek, t];
				}
			}

			// initilise last weeks to be empty
			for (int week = startWeek + ((teams.Length-1) * 2); week < Program.totalWeeks; week++)
			{
				for (int t = 0; t < teams.Length; t++)
				{
					newOpponentTable[week, t] = null;
					newHomeTable[week, t] = false;
				}
			}

			// for each week that teams play
			for (int week = 0; week < Program.totalWeeks; week++)
			{
				Console.Write("\n-----------------------\nWeek " + (week+1) + "\n-----------------------\n");
				// print line of week to console
				for (int i = 0; i < teams.Length; i++)
				{
					if (newOpponentTable[week, i] == null)
					{
						Console.ForegroundColor = ConsoleColor.Blue;
						Console.WriteLine("Team " + (i+1) + " does not play this week");
						Console.ResetColor();
					}
					else if (teamsInOrder[i].requirements[week] != "None" &&
								((newHomeTable[week, i] && !teamsInOrder[i].requirements[week].Contains("H")) ||
								(!newHomeTable[week, i] && !teamsInOrder[i].requirements[week].Contains("A"))) )
					{
						Console.ForegroundColor = ConsoleColor.Red;
						Console.Write("Team " + (i+1) + " cannot play team " + newOpponentTable[week, i].id);
						if (newHomeTable[week, i])
							Console.WriteLine(" at Home");
						else
							Console.WriteLine(" Away");
						Console.ResetColor();
					}
					else if (newHomeTable[week, i])
						Console.WriteLine("Team " + (i + 1) + " plays team " + newOpponentTable[week, i].id + " at Home");
					else
						Console.WriteLine("Team " + (i + 1) + " plays team " + newOpponentTable[week, i].id + " Away");
				}
			}
		}

		public Division roundRobbin()
		{
			// tables of size teams by weeks
			opponentTable = new Team[2*teams.Length,teams.Length];
			homeTable = new bool[2 * teams.Length, teams.Length];

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
				for (int i = 0; i < (Math.Pow(teams.Length*2, 1/10)) - (Math.Pow(week+startWeek, 1/10)); i++)
					Console.Write(" ");
				if (week + startWeek < 10)
					Console.Write(" ");
				Console.Write((week+startWeek) + "||");

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
					if (teamsInOrder[i] == null )
					{
						Console.ForegroundColor = ConsoleColor.Blue;
						Console.Write("XX");
						Console.ResetColor();
					}
					else if(teams[i].requirements[week] != "None" &&
								((homeTable[week, i] && !teamsInOrder[i].requirements[week].Contains("H")) ||
								(!homeTable[week, i] && !teamsInOrder[i].requirements[week].Contains("A"))))
					{
						Console.ForegroundColor = ConsoleColor.Red;
						if (homeTable[week, i])
							Console.Write("H");
						else
							Console.Write("A");
					}
					else if (homeTable[week, i] )
						Console.Write("H");
					else
						Console.Write("A");

					if (opponentTable[week, i] != null)
					{
						Console.Write(opponentTable[week, i].id);
						Console.ResetColor();
						Console.Write("|");
					}
					else
						Console.Write("|");

				}
				Console.Write("\n");


				rotateArray();
			}

			return this;
		}
	}
}
