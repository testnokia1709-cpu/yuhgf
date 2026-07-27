using System;

[Flags]
public enum LevelCompletion
{
	Unsolved = 0,
	Solved = 1,
	TimeSolved = 2,
	ShapeSolved = 4,
	Complete = 7
}
