using UnityEngine;

public class UIBase : MonoBehaviour
{
	public virtual void OnSetup()
	{
	}

	public virtual void OnObjectAdded()
	{
	}

	public virtual void OnObjectSelected()
	{
	}

	public virtual void OnObjectDeselected()
	{
	}

	public virtual void OnGoalSelected()
	{
	}

	public virtual void UpdateUI(float duration, int shapes)
	{
	}

	public virtual void UpdateMultiplayerUI(float duration)
	{
	}

	public virtual void ShowGameComplete(LevelCompletion previouslyCompleted, LevelCompletion completed, float gameDuration, int shapeCount, int coinsEarned, int gemsEarned)
	{
	}

	public virtual void ShowNoAdsControl(bool enabled)
	{
	}

	public virtual void EndMultiplayer(float gameDuration, int shapeCount)
	{
	}

	public virtual void LoadMenu()
	{
	}
}
