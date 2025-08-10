using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
	[Header("UI Elements")]
	[SerializeField] 
	private GameObject _startScreen;
	[SerializeField] 
	private GameObject _lap1FinishedScreen;
	[SerializeField] 
	private GameObject _resultScreen;
	[SerializeField] 
	private TextMeshProUGUI _lapText;
	[SerializeField] 
	private TextMeshProUGUI _resultText;

	private void Start()
	{
		EventBus.Instance.OnRaceStateChanged.AddListener(HandleRaceState);
		EventBus.Instance.OnWin.AddListener(SetWinnerText);
	}

	public void SetWinnerText(string winner)
	{
		switch (winner)
		{
			case "player":
				_resultText.text = "Гонка завершена. Игрок победил";
				break;
			case "ghost":
				_resultText.text = "Гонка завершена. Призрак победил";
				break;
		}
	}

	public void OnStartButtonClicked()
	{
		_startScreen.SetActive(false);
		RaceManager.Instance.StartRace();
	}

	private void HandleRaceState(RaceState state)
	{
		switch (state)
		{
			case RaceState.Lap1:
				_lapText.text = "LAP: 1/2";
				break;
			case RaceState.Lap1Finished:
				_lapText.text = "Первый круг завершен";
				_lap1FinishedScreen.SetActive(true);
				break;
			case RaceState.Lap2Preparation:
				_lap1FinishedScreen.SetActive(false);
				_lapText.text = "Подготовка";
				_startScreen.SetActive(true);
				break;
			case RaceState.Lap2:
				_lapText.text = "LAP: 2/2";
				break;
			case RaceState.Lap2Finished:
				_resultScreen.SetActive(true);
				break;
		}
	}
}