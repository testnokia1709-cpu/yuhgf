using System.Collections.Generic;

namespace Amazon.S3.Model
{
	public abstract class AnalyticsNAryOperator : AnalyticsFilterPredicate
	{
		private readonly List<AnalyticsFilterPredicate> operands;

		public List<AnalyticsFilterPredicate> Operands
		{
			get
			{
				return operands;
			}
		}

		protected AnalyticsNAryOperator(List<AnalyticsFilterPredicate> operands)
		{
			this.operands = operands;
		}
	}
}
