using System.Collections.Generic;
using Amazon.S3.Model.Internal;

namespace Amazon.S3.Model
{
	public sealed class AnalyticsAndOperator : AnalyticsNAryOperator
	{
		public AnalyticsAndOperator(List<AnalyticsFilterPredicate> operands)
			: base(operands)
		{
		}

		internal override void Accept(IAnalyticsPredicateVisitor analyticsPredicateVisitor)
		{
			analyticsPredicateVisitor.visit(this);
		}
	}
}
