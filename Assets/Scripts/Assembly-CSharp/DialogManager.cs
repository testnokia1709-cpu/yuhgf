using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogManager : MonoBehaviour
{
	public static DialogManager Instance;

	public GameObject ParentCanvas;

	public GameObject Background;

	public GameObject OptionDialogPanel;

	public Text OptionBodyText;

	public Text OptionText1;

	public Text OptionText2;

	public Text OptionText3;

	public Button OptionButton1;

	public Button OptionButton2;

	public Button OptionButton3;

	public GameObject YesNoDialogPanel;

	public Text YesNoBodyText;

	public Text YesNoAdditionalText;

	public Button YesNoButton1;

	public Button YesNoButton2;

	public Text YesNoText1;

	public Text YesNoText2;

	public GameObject OkDialogPanel;

	public Text OkBodyText;

	public Button OkButton1;

	public Text OkText1;

	public GameObject PostDialogPanel;

	public InputField MessageText;

	public Text TitleText;

	public Button SendButton;

	public Button CancelButton;

	public Text SendButtonText;

	public Image ProviderImage;

	public RawImage ScreenshotImage;

	public GameObject InputDialogPanel;

	public InputField InputText;

	public Text InputTitleText;

	public Button SubmitButton;

	public Button SubmitCancelButton;

	public GameObject NoticeDialogPanel;

	public Text NoticeTitleText;

	public Text NoticeBodyText;

	public Button NoticeButton;

	public Text NoticeButtonText;

	public Scrollbar NoticeScrollBar;

	public GameObject LoadingDialogPanel;

	public Text LoadingText;

	private Action<int> m_optionSelected;

	private float m_dialogStartTime;

	private float m_dialogDisplayTime;

	private bool m_isTimedDisplay;

	private List<Action> m_deferredActions;

	private bool m_waitOneFrame;

	public bool IsShown { get; private set; }

	private void Awake()
	{
		if (Instance != null)
		{
			UnityEngine.Object.Destroy(ParentCanvas);
			UnityEngine.Object.Destroy(this);
			return;
		}
		Instance = this;
		UnityEngine.Object.DontDestroyOnLoad(this);
		UnityEngine.Object.DontDestroyOnLoad(ParentCanvas);
		m_deferredActions = new List<Action>();
	}

	private void Start()
	{
		CloseDialog();
	}

	private void Update()
	{
		if (m_isTimedDisplay && Time.realtimeSinceStartup - m_dialogStartTime > m_dialogDisplayTime)
		{
			CloseDialog();
			m_isTimedDisplay = false;
		}
		bool flag = NoticeScrollBar.size < 0.95f;
		if ((NoticeScrollBar.gameObject.activeSelf && !flag) || (!NoticeScrollBar.gameObject.activeSelf && flag))
		{
			NoticeScrollBar.gameObject.SetActive(flag);
		}
		if (m_deferredActions.Count > 0 && m_waitOneFrame)
		{
			m_deferredActions[0]();
			m_deferredActions.RemoveAt(0);
			m_waitOneFrame = false;
		}
		if (m_deferredActions.Count > 0)
		{
			m_waitOneFrame = true;
		}
	}

	public static void ShowDialog(string body, string option1, string option2, string option3, Action<int> action = null)
	{
		Debug.Log("ShowDialog(3): " + body);
		CloseDialog();
		Instance.Background.transform.localPosition = new Vector3(0f, 0f, 0f);
		Instance.OptionBodyText.text = body;
		Instance.OptionText1.text = option1;
		Instance.OptionText2.text = option2;
		Instance.OptionText3.text = option3;
		Instance.OptionButton1.gameObject.SetActive(!string.IsNullOrEmpty(option1));
		Instance.OptionButton2.gameObject.SetActive(!string.IsNullOrEmpty(option2));
		Instance.OptionButton3.gameObject.SetActive(!string.IsNullOrEmpty(option3));
		Instance.OptionDialogPanel.transform.localPosition = new Vector3(0f, 0f, 0f);
		Instance.m_optionSelected = action;
		Instance.IsShown = true;
	}

	public static void ShowDialog(string body, string text1, string text2, Action<int> action = null)
	{
		ShowDialogWithAdditionalText(body, string.Empty, text1, text2, action);
	}

	public static void ShowDialogWithAdditionalText(string body, string additionalText, string text1, string text2, Action<int> action = null)
	{
		Debug.Log("ShowDialog(2): " + body);
		CloseDialog();
		Instance.Background.transform.localPosition = new Vector3(0f, 0f, 0f);
		Instance.YesNoBodyText.text = body;
		Instance.YesNoAdditionalText.text = additionalText;
		Instance.YesNoText1.text = text1;
		Instance.YesNoText2.text = text2;
		Instance.YesNoButton1.gameObject.SetActive(!string.IsNullOrEmpty(text1));
		Instance.YesNoButton2.gameObject.SetActive(!string.IsNullOrEmpty(text2));
		Instance.YesNoDialogPanel.transform.localPosition = new Vector3(0f, 0f, 0f);
		Instance.m_optionSelected = action;
		Instance.IsShown = true;
	}

	public static void ShowDialog(string body, string text, Action<int> action = null)
	{
		Debug.Log("ShowDialog(1): " + body);
		CloseDialog();
		Instance.Background.transform.localPosition = new Vector3(0f, 0f, 0f);
		Instance.OkBodyText.text = body;
		Instance.OkText1.text = text;
		Instance.OkButton1.gameObject.SetActive(!string.IsNullOrEmpty(text));
		Instance.OkDialogPanel.transform.localPosition = new Vector3(0f, 0f, 0f);
		Instance.m_optionSelected = action;
		Instance.IsShown = true;
	}

	public static void ShowNotice(string body, string title, string text, Action<int> action = null)
	{
		Debug.Log("ShowNotice: " + body);
		CloseDialog();
		Instance.Background.transform.localPosition = new Vector3(0f, 0f, 0f);
		Instance.NoticeBodyText.text = body;
		Instance.NoticeTitleText.text = title;
		Instance.NoticeButtonText.text = text;
		Instance.NoticeScrollBar.value = 1f;
		Instance.NoticeDialogPanel.transform.localPosition = new Vector3(0f, 0f, 0f);
		Instance.m_optionSelected = action;
		Instance.IsShown = true;
	}

	public static void ShowDialog(string body, float duration)
	{
		CloseDialog();
		Instance.m_dialogStartTime = Time.realtimeSinceStartup;
		Instance.m_dialogDisplayTime = duration;
		Instance.m_isTimedDisplay = true;
		ShowDialog(body);
	}

	public static void ShowDialog(string body)
	{
		Debug.Log("ShowDialog: " + body);
		CloseDialog();
		Instance.Background.transform.localPosition = new Vector3(0f, 0f, 0f);
		Instance.LoadingText.text = body;
		Instance.LoadingDialogPanel.transform.localPosition = new Vector3(0f, 0f, 0f);
		Instance.m_optionSelected = null;
		Instance.IsShown = true;
	}

	public static void ShowLoadingDialog()
	{
		ShowDialog(TextLibrary.Get(StringId.S_LOADING));
	}

	public static void CloseDialog()
	{
		Instance.Background.transform.localPosition = new Vector3(Screen.width, Screen.height, 0f);
		Instance.OptionDialogPanel.transform.localPosition = new Vector3(Screen.width, Screen.height, 0f);
		Instance.YesNoDialogPanel.transform.localPosition = new Vector3(Screen.width, Screen.height, 0f);
		Instance.OkDialogPanel.transform.localPosition = new Vector3(Screen.width, Screen.height, 0f);
		Instance.PostDialogPanel.transform.localPosition = new Vector3(Screen.width, Screen.height, 0f);
		Instance.LoadingDialogPanel.transform.localPosition = new Vector3(Screen.width, Screen.height, 0f);
		Instance.NoticeDialogPanel.transform.localPosition = new Vector3(Screen.width, Screen.height, 0f);
		Instance.InputDialogPanel.transform.localPosition = new Vector3(Screen.width, Screen.height, 0f);
		Instance.IsShown = false;
	}

	public static void ShowPostDialog(Sprite providerSprite, Texture2D texture, string title, string message, string placeholderMessage, string oktext, int charLimit, Action<int> action = null)
	{
		Debug.Log("ShowPostDialog: " + title + " " + message);
		CloseDialog();
		Instance.Background.transform.localPosition = new Vector3(0f, 0f, 0f);
		Instance.ProviderImage.sprite = providerSprite;
		Instance.ScreenshotImage.texture = texture;
		Instance.TitleText.text = title;
		Instance.MessageText.text = message;
		if (placeholderMessage != string.Empty)
		{
			Instance.MessageText.placeholder.GetComponent<Text>().text = placeholderMessage;
		}
		Instance.MessageText.characterLimit = charLimit;
		Instance.SendButtonText.text = oktext;
		Instance.PostDialogPanel.transform.localPosition = new Vector3(0f, 0f, 0f);
		Instance.m_optionSelected = action;
		Instance.IsShown = true;
	}

	public static void ShowInputDialog(string title, string placeholderText, string oktext, int maxLength = 0, Action<int> action = null)
	{
		CloseDialog();
		Instance.Background.transform.localPosition = new Vector3(0f, 0f, 0f);
		Instance.InputText.text = string.Empty;
		Instance.InputText.characterLimit = maxLength;
		Instance.InputTitleText.text = title;
		Instance.InputText.placeholder.GetComponent<Text>().text = placeholderText;
		Instance.SubmitButton.GetComponentInChildren<Text>().text = oktext;
		Instance.InputDialogPanel.transform.localPosition = new Vector3(0f, 0f, 0f);
		Instance.m_optionSelected = action;
		Instance.IsShown = true;
	}

	public void ButtonClicked(Button button)
	{
		int index = -1;
		if (button == OptionButton1 || button == YesNoButton1 || button == OkButton1 || button == SendButton || button == NoticeButton || button == SubmitButton)
		{
			index = 0;
		}
		else if (button == OptionButton2 || button == YesNoButton2 || button == CancelButton || button == SubmitCancelButton)
		{
			index = 1;
		}
		else if (button == OptionButton3)
		{
			index = 2;
		}
		CloseDialog();
		m_deferredActions.Add(delegate
		{
			if (m_optionSelected != null)
			{
				m_optionSelected(index);
			}
		});
	}

	public List<Text> GetAllText()
	{
		List<Text> list = new List<Text>();
		list.Add(OptionBodyText);
		list.Add(OptionText1);
		list.Add(OptionText2);
		list.Add(OptionText3);
		list.Add(YesNoBodyText);
		list.Add(YesNoText1);
		list.Add(YesNoText2);
		list.Add(OkBodyText);
		list.Add(OkText1);
		list.Add(MessageText.textComponent);
		list.Add(TitleText);
		list.Add(SendButtonText);
		list.Add(NoticeTitleText);
		list.Add(NoticeBodyText);
		list.Add(NoticeButtonText);
		list.Add(LoadingText);
		return list;
	}
}
