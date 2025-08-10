using UnityEngine;
using UnityEngine.Localization.Settings;

public class LocalizationSwitcher : MonoBehaviour
{
	[SerializeField]
	private GameObject _englishButton;
	[SerializeField]
	private GameObject _russianButton;

	private void Start()
	{
		ShowLocalesButtons();
	}

	public void SetLanguage(string languageCode)
	{
		var locales = LocalizationSettings.AvailableLocales.Locales;

		for (int i = 0; i < locales.Count; i++)
		{
			if (locales[i].Identifier.Code == languageCode)
			{
				LocalizationSettings.SelectedLocale = locales[i];
				ShowLocalesButtons();
				return;
			}
		}

		Debug.LogError($"язык с кодом '{languageCode}' не найден!");
	}

	public void ShowLocalesButtons()
	{
		switch (LocalizationSettings.SelectedLocale.Identifier.Code)
		{
			case "ru-RU":
				_russianButton.SetActive(false);
				_englishButton.SetActive(true);
				break;
			case "en-US":
				_russianButton.SetActive(true);
				_englishButton.SetActive(false);
				break;
		}
	}
}
