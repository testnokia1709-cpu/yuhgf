using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ImageBar : MonoBehaviour
{
	public List<Image> ImageList;

	[HideInInspector]
	public List<Button> ButtonList;

	public float DeselectedAlpha = 0.5f;

	public ImageClickEvent OnClick;

	private int m_selected;

	private int m_mask;

	public int SelectedCount
	{
		get
		{
			return m_selected;
		}
		set
		{
			m_selected = value;
			setMaskFromCount(value);
			refresh();
		}
	}

	public int SelectedMask
	{
		get
		{
			return m_mask;
		}
		set
		{
			m_mask = value;
			setCountFromMask(value);
			refresh();
		}
	}

	public int IndexClicked { get; private set; }

	private void Awake()
	{
		ButtonList = new List<Button>();
		if (ImageList.Count == 0)
		{
			Button[] componentsInChildren = base.gameObject.GetComponentsInChildren<Button>();
			ImageList = new List<Image>();
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				ButtonList.Add(componentsInChildren[i]);
				ImageList.Add(componentsInChildren[i].gameObject.GetComponent<Image>());
			}
		}
		else
		{
			int count = ImageList.Count;
			for (int j = 0; j < count; j++)
			{
				ButtonList.Add(ImageList[j].gameObject.GetComponent<Button>());
			}
		}
	}

	private void Start()
	{
		refresh();
		int count = ButtonList.Count;
		for (int i = 0; i < count; i++)
		{
			Button button = ButtonList[i];
			if (button != null)
			{
				EventTrigger eventTrigger = button.gameObject.AddComponent<EventTrigger>();
				EventTrigger.Entry entry = new EventTrigger.Entry();
				entry.eventID = EventTriggerType.PointerDown;
				int index = i;
				entry.callback.AddListener(delegate
				{
					ImageClicked(index);
				});
				eventTrigger.triggers.Add(entry);
			}
		}
	}

	public void ImageClicked(int index)
	{
		IndexClicked = index;
		OnClick.Invoke();
	}

	private void setCountFromMask(int value)
	{
		int num = 0;
		for (int i = 0; i < 32; i++)
		{
			int num2 = 1 << i;
			if ((value & num2) == num2)
			{
				num++;
			}
		}
		m_selected = num;
	}

	private void setMaskFromCount(int value)
	{
		int num = 0;
		for (int i = 0; i < value; i++)
		{
			num |= 1 << i;
		}
		m_mask = num;
	}

	private void refresh()
	{
		int num = 0;
		foreach (Image image in ImageList)
		{
			int num2 = 1 << num;
			bool flag = (m_mask & num2) == num2;
			image.color = new Color(image.color.r, image.color.g, image.color.b, (!flag) ? DeselectedAlpha : 1f);
			num++;
		}
	}
}
