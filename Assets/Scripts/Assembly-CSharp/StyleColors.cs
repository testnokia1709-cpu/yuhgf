using UnityEngine;

public class StyleColors : ScriptableObject
{
	public Color BackgroundLight;

	public Color BackgroundDark;

	public Color Accent;

	public Color ThemeLight;

	public Color ThemeMedium;

	public Color ThemeDark;

	public Color GetColor(StyleColor styleColor)
	{
		switch (styleColor)
		{
		case StyleColor.BackgroundLight:
			return BackgroundLight;
		case StyleColor.BackgroundDark:
			return BackgroundDark;
		case StyleColor.Accent:
			return Accent;
		case StyleColor.ThemeLight:
			return ThemeLight;
		case StyleColor.ThemeMedium:
			return ThemeMedium;
		case StyleColor.ThemeDark:
			return ThemeDark;
		case StyleColor.White:
			return Color.white;
		case StyleColor.Black:
			return Color.black;
		default:
			return Color.white;
		}
	}
}
