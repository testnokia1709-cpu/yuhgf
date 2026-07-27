using System;
using System.Xml;
using Amazon.S3.Model.Internal.MarshallTransformations;

namespace Amazon.S3.Model
{
	public class SelectParameters
	{
		public InputSerialization InputSerialization { get; set; }

		public ExpressionType ExpressionType { get; set; }

		public string Expression { get; set; }

		public OutputSerialization OutputSerialization { get; set; }

		internal bool IsSetInputSerialization()
		{
			return InputSerialization != null;
		}

		internal bool IsSetExpressionType()
		{
			return ExpressionType != null;
		}

		internal bool IsSetExpression()
		{
			return Expression != null;
		}

		internal bool IsSetOutputSerialization()
		{
			return OutputSerialization != null;
		}

		internal void Marshall(string memberName, XmlWriter xmlWriter)
		{
			if (!IsSetInputSerialization())
			{
				throw new ArgumentException("SelectParameters.InputSerialization is a required property and must be set before making this call.");
			}
			if (!IsSetExpression())
			{
				throw new ArgumentException("SelectParameters.Expression is a required property and must be set before making this call.");
			}
			if (!IsSetExpressionType())
			{
				throw new ArgumentException("SelectParameters.ExpressionType is a required property and must be set before making this call.");
			}
			if (!IsSetOutputSerialization())
			{
				throw new ArgumentException("SelectParameters.OutputSerialization is a required property and must be set before making this call.");
			}
			xmlWriter.WriteStartElement(memberName);
			if (IsSetInputSerialization())
			{
				InputSerialization.Marshall("InputSerialization", xmlWriter);
			}
			if (IsSetExpressionType())
			{
				xmlWriter.WriteElementString("ExpressionType", S3Transforms.ToXmlStringValue(ExpressionType.Value));
			}
			if (IsSetExpression())
			{
				xmlWriter.WriteElementString("Expression", S3Transforms.ToXmlStringValue(Expression));
			}
			if (IsSetOutputSerialization())
			{
				OutputSerialization.Marshall("OutputSerialization", xmlWriter);
			}
			xmlWriter.WriteEndElement();
		}
	}
}
