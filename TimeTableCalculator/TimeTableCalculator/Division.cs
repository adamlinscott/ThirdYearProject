using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeTableCalculator
{
	class Division
	{
		private Division* nextDivision;

		public void Division(Object divisionInfo, Division* nextDivision)
		{
			this.nextDivision = nextDivision;
		}

		public void Division(Object divisionInfo)
		{

		}
	}
}
