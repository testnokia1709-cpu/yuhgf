using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[ExecuteInEditMode]
public class ToggleButton : Toggle
{
	public Graphic OffGraphic;

	public GameObject OnObject;

	public GameObject OffObject;

	public new bool isOn
	{
		get
		{
			return base.isOn;
		}
		set
		{
			base.isOn = value;
			UpdateToggle();
		}
	}

	public override void OnPointerClick(PointerEventData eventData)
	{
		base.OnPointerClick(eventData);
		UpdateToggle();
	}

	protected override void Start()
	{
		base.Start();
		onValueChanged.AddListener(delegate
		{
			UpdateToggle();
		});
		UpdateToggle();
	}

	public void UpdateToggle()
	{
		if (OffGraphic != null)
		{
			OffGraphic.gameObject.SetActive(!isOn);
		}
		if (OnObject != null)
		{
			OnObject.SetActive(isOn);
		}
		if (OffObject != null)
		{
			OffObject.SetActive(!isOn);
		}
	}

	public void SetToggle(bool value)
	{
		isOn = value;
		UpdateToggle();
	}
}
