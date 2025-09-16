using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using VaSiLi.Networking;
using WebSocketSharp;

public class SpeechSelectorUI : MonoBehaviour
{
	[Header("Dependencies")]
	[SerializeField] private BundestagSpawner _bundestagSpawner;
	[SerializeField] private SpeakerCommunicator _speakerCommunicator;
	[SerializeField] private BundestagManager _bundestagsManager;

	[Space(20f)]
	[Header("UI")]
	[SerializeField] private List<TMP_Text> _textBoards;
	[SerializeField] private TMP_Text _currentProtocolInput;
	[SerializeField] private Button _agendaPrefab;
	[SerializeField] private GameObject _agendaParent;
	[SerializeField] private Button _speechPrefab;
	[SerializeField] private GameObject _speechParent;

	[Space(20f)]
	[Header("REST Urls")]
	[SerializeField] public string _getProtocolInformationUrl = "http://localhost:8080/speeches/getNav/";
	[SerializeField] public string _getSpeechInformationUrl = "http://localhost:8080/speeches/getSpeech/";

	public static int currentSitzungsNr = 999;

	private ProtocolResponse _protocolResponse;

	public async void Awake()
	{
		currentSitzungsNr = 999;
		var req = await JsonRequest.GetRequest(_getProtocolInformationUrl);
		var content = await req.Content.ReadAsStringAsync();
		content = "{\"data\":" + content + "}";
		_protocolResponse = JsonUtility.FromJson<ProtocolResponse>(content);
	}

	public void DisplayTextOnBoard(string text)
	{
		foreach (var board in _textBoards)
		{
			board.text = text;
		}
	}

	public void PressProtocolButton(string input)
	{
		switch (input)
		{
			case "delete":
				if (_currentProtocolInput.text.IsNullOrEmpty())
					return;

				_currentProtocolInput.text = _currentProtocolInput.text.Substring(0, _currentProtocolInput.text.Length - 1);
				break;
			case "enter":
				DisplayAgendas(int.Parse(_currentProtocolInput.text));
				break;
			default:
				if (_currentProtocolInput.text.Length == 4)
					return;

				_currentProtocolInput.text += input;
				break;
		}
	}

	public void DisplayAgendas(int id)
	{
		List<AgendaData> agendas = GetAgendasByProtocolId(id);

		foreach (Transform t in _agendaParent.transform)
		{
			Destroy(t.gameObject);
		}

		for (int i = 0; i < agendas.Count; i++)
		{
			Button agendaButton = Instantiate(_agendaPrefab, _agendaParent.transform);
			agendaButton.name = i.ToString();
			List<string> ids = agendas[i].ids;
			agendaButton.onClick.AddListener(() => DisplaySpeeches(ids));

			if (agendas[i].title == null || agendas[i].title.Length > 130)
				agendaButton.GetComponentInChildren<TMP_Text>().text = $"Tagesordnungspunkt {i}";
			else
				agendaButton.GetComponentInChildren<TMP_Text>().text = agendas[i].title;
		}
	}

	List<SpeechResponse> speechResponses = new List<SpeechResponse>();
	private void DisplaySpeeches(List<string> speechIds)
	{
		if (loadSpeechCoroutine != null)
			StopCoroutine(loadSpeechCoroutine);
		speechResponses.Clear();

		foreach (Transform t in _speechParent.transform)
		{
			Destroy(t.gameObject);
		}

		foreach (string speechId in speechIds)
		{
			Button speechButton = Instantiate(_speechPrefab, _speechParent.transform);
			speechButton.name = speechId;
			speechButton.onClick.AddListener(() => LoadSpeech(speechId));
			speechButton.GetComponentInChildren<TMP_Text>().text = speechId;  // Dummy Name
		}

		loadSpeechCoroutine = StartCoroutine(LoadSpeechInfos(speechIds));  // Load Speeches and display real names
	}

	private Coroutine loadSpeechCoroutine;
	private IEnumerator LoadSpeechInfos(List<string> ids)
	{
		for (int i = 0; i < ids.Count; i++)
		{

			UnityWebRequest uwr = UnityWebRequest.Get(_getSpeechInformationUrl + ids[i]);
			yield return uwr.SendWebRequest();

			if (uwr.result == UnityWebRequest.Result.ConnectionError)
			{
				Debug.Log("Error While Sending: " + uwr.error);
			}
			else
			{
				SpeechResponse speech = JsonUtility.FromJson<SpeechResponse>(uwr.downloadHandler.text);

				if (_speechParent.transform.childCount > i)
				{
					TMP_Text text = _speechParent.transform.GetChild(i).GetComponent<Button>().GetComponentInChildren<TMP_Text>();

					speechResponses.Add(speech);

					if (speech.speechSections.Count == 0) continue;

					if (!speech.speechSections[0].speaker.fristName.IsNullOrEmpty())
						text.text = speech.speechSections[0].speaker.fristName;
					else
						text.text = "";

					if (!speech.speechSections[0].speaker.lastName.IsNullOrEmpty())
						text.text += " " + speech.speechSections[0].speaker.lastName;

					if (text.text.IsNullOrEmpty())
						text.text = ids[i];
				}
			}
		}
	}

	private List<Speaker> _disabledSpeakers = new List<Speaker>();
	public async void LoadSpeech(string speechId)
	{
		// Reset Disabled Speakers
		foreach (var disSpeaker in _disabledSpeakers)
		{
			disSpeaker.gameObject.SetActive(true);
		}
		_disabledSpeakers.Clear();

		// Get Speech info
		SpeechResponse speech = await GetSpeechById(speechId);

		Debug.Log("Loading " + speechId + "...");

		// Apply speech info
		foreach (var v in speech.entschuldigteAbgeordnete)
		{
			SpeakerInformation si = _bundestagSpawner.GetSpeaker(v);
			if (si != null && si.speaker != null)
			{
				_disabledSpeakers.Add(si.speaker);
				si.speaker.gameObject.SetActive(false);
			}
		}

		_bundestagsManager.SpeakSpeech(speech);
	}

	public void StopCurrentSpeech()
	{
		_bundestagsManager.StopGenerateSpeech();
	}

	public void TogglePauseCurrentSpeech()
	{
		_bundestagsManager.TogglePauseSpeech();
	}

	public List<AgendaData> GetAgendasByProtocolId(int id)
	{
		return _protocolResponse.data.Find(sd => sd.id == id).titles;
	}

	public async Task<SpeechResponse> GetSpeechById(string id)
	{
		var req = await JsonRequest.GetRequest(_getSpeechInformationUrl + id);
		var content = await req.Content.ReadAsStringAsync();
		SpeechResponse response = JsonUtility.FromJson<SpeechResponse>(content);
		currentSitzungsNr = response.sitzungsNr;
		return response;
	}

	[Serializable]
	public struct ProtocolResponse
	{
		public List<SitzungData> data;
	}

	[Serializable]
	public struct SitzungData
	{
		public int id;
		public List<AgendaData> titles;
	}

	[Serializable]
	public struct AgendaData
	{
		public string title;
		public List<string> ids;
	}

	[Serializable]
	public struct SpeechResponse
	{
		public string id;
		public int begin;
		public int end;
		public string sofaString;
		public Agenda agenda;
		public int wahlperiode;
		public int sitzungsNr;
		public double datum;
		public List<Sentiment> sentiments;
		public List<SpeechSection> speechSections;
		public List<Embedding> embeddings;
		public List<int> entschuldigteAbgeordnete;

	}

	[Serializable]
	public struct Agenda
	{
		public string titleRede;
		public string titleTop;
	}

	[Serializable]
	public struct Sentiment
	{
		public int begin;
		public int end;
		public float sentiment;
	}

	[Serializable]
	public struct SpeechSection
	{
		public int begin;
		public int end;
		public string type;
		public string text;
		public Speakr speaker;
	}

	[Serializable]
	public struct Speakr
	{
		public string fristName;
		public string lastName;
		public string id;
		public string party;
	}

	[Serializable]
	public struct Embedding
	{
		public int begin;
		public int end;
		public List<float> floats;
	}
}
