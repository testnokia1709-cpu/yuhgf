using System;
using CloudOnce.Internal;

public class Achievement
{
	public int Target;

	public float Progress;

	public bool Earned;

	public StringId LocalizedDescription;

	public Func<int, float> Calculate;

	public UnifiedAchievement RawAchievement;
}
