using System;

namespace Amazon.Runtime.Internal
{
	public interface IRuntimePipelineCustomizer
	{
		string UniqueName { get; }

		void Customize(Type type, RuntimePipeline pipeline);
	}
}
