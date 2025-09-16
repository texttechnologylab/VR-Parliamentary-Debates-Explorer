using Org.BouncyCastle.Ocsp;
using SIPSorceryMedia.Abstractions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using VaSiLi.Networking;

public class SpeakerCommunicator : MonoBehaviour
{
	[Header("Speaker Detection")]
	[SerializeField] private LayerMask _speakerMask;
	private Speaker _currentlySpeakingTo;
	private bool _currentlyInDialogue;

	[Space(10)]
	[Header("Model URLs")]
	public string getFittingResponses = "http://localhost:8080/speeches/getTopSpeeches";
	public string speechToTextUrl = "http://localhost:5000/whisperx";
	public string ollamaUrl = "http://ollama.lehre.texttechnologylab.org/api/chat";
	public string barkUrl = "http://localhost:5000/bark";
	public string llmModel = "llama3.2";

	private AudioClip _clip;

	private void Start()
	{
		Debug.Log("=================");
		foreach (var item in Microphone.devices)
		{
			Debug.Log(item);
		}
	}

	// Update is called once per frame
	void Update()
	{	

		// No shits clue what "Four" is (should be Y)
		if (!_currentlyInDialogue && (OVRInput.GetDown(OVRInput.Button.Four) || Input.GetKeyDown(KeyCode.Y)))
		{

			RaycastHit hit;

			if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, 8f, _speakerMask))
			{
				Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * hit.distance, Color.yellow);

				_currentlySpeakingTo = hit.collider.GetComponent<Speaker>();
				_currentlyInDialogue = true;
				Debug.Log("Speaking to " + _currentlySpeakingTo.name);
			}
			else
			{
				_currentlySpeakingTo = null;
				Debug.Log("Schaue jemanden an um ein Gespräch zu beginnen.");
				return;
			}

			Debug.Log("Start Rec");
			_clip = Microphone.Start(Microphone.devices[0], true, 5, 44100);	
		}

		else if (_currentlyInDialogue && _currentlySpeakingTo != null && (OVRInput.GetUp(OVRInput.Button.Four) || Input.GetKeyUp(KeyCode.Y)))
		{
			Microphone.End(Microphone.devices[0]);

			SendSpeechData(SavWav.GetWav(_clip, out uint length));

			Debug.Log("End Rec");
		}
	}

	async void SendSpeechData(byte[] wavByte)
	{
		var base64 = Convert.ToBase64String(wavByte);

		AudioMessage audioMessage = new AudioMessage(base64);

		// send Audio
		Debug.Log("Sending Audio...");
		var req = await JsonRequest.PostRequest<AudioMessage>(speechToTextUrl, audioMessage);
		var content = await req.Content.ReadAsStringAsync();
		var audioData = JsonUtility.FromJson<AudioReturn>(content);

		if (_currentlySpeakingTo == null) return;

		Debug.Log("User request: " + audioData.transcription);

		// Get fitting responses
		string promtTranscription = audioData.transcription;

		RAGMessage ragMsg = new RAGMessage
		{
			firstName = _currentlySpeakingTo.information.firstName,
			lastName = _currentlySpeakingTo.information.lastName,
			text = promtTranscription,
			maxSitzungsNr = SpeechSelectorUI.currentSitzungsNr
		};
		req = await JsonRequest.PostRequest<RAGMessage>(getFittingResponses, ragMsg);
		content = await req.Content.ReadAsStringAsync();
		content = "{\"topMessages\": " + content + "}";
		RAGReturn ragReturn = JsonUtility.FromJson<RAGReturn>(content);

		Debug.Log("Rag content: " + content);

		string promt = getTopMessagesAsString(ragReturn.topMessages) + "\n## Nachricht:\n" + promtTranscription;

		// Add promt with fitting responses to LLM chat history
		_currentlySpeakingTo.information.AddPromtHistory(promt);

		OllamaMessage ollamaMessage = new OllamaMessage() { model = llmModel, messages = _currentlySpeakingTo.information.conversationHistory, stream = false };

		// Generate response
		req = await JsonRequest.PostRequest<OllamaMessage>(ollamaUrl, ollamaMessage);
		content = await req.Content.ReadAsStringAsync();

		var ollamaData = JsonUtility.FromJson<OllamaReturn>(content);

		Debug.Log("Ollama response: " + ollamaData.message.content);
		string[] ollamaMessageSplit = ollamaData.message.content.Split("</think>"); // Deepseek sends <think> ... </think>. Dont need that
		string ollamaResponseMessage = ollamaMessageSplit.Length == 1 ? ollamaMessageSplit[0] : ollamaMessageSplit[1];

		// Add response to speaker's chat history
		_currentlySpeakingTo.information.AddResponseHistory(ollamaData.message);

		string b64 = await TextToSpeech(ollamaResponseMessage);

		Debug.Log("Got TTS. I Speak now");

		_currentlySpeakingTo.SpeakBase64(new AudioObject { b64Audio = b64 });

		// End dialogue
		_currentlyInDialogue = false;
	}

	public async Task<string> TextToSpeech(string text)
	{
		var req = await JsonRequest.PostRequest<BarkMessage>(barkUrl, new BarkMessage { prompt = text });
		var content = await req.Content.ReadAsStringAsync();
		print(content);
		var barkData = JsonUtility.FromJson<BarkReturn>(content);

		return barkData.base64;
	}

	private string getTopMessagesAsString(TopMessage[] messages)
	{
		string result = "## Kontext (Redenausschnitte – zur stilistischen Orientierung und Argumenten):\n";
		for (int i = 0; i < messages.Length; i++)
		{
			result += $"[REDE_{i + 1}]: \"{messages[i].sofaSubstring}\"\n";
		}

		if(messages.Length == 0)
		{
			result += "[Es konnten keine Reden gefunden werden. Antworte mit deinem Wissen, oder wenn du es nicht weißt, dannn sag, dass du es nicht weißt.]\n";
		}

		return result;
	}

	[Serializable]
	private struct AudioMessage
	{
		public string audioBase64;

		public AudioMessage(string audioBase64)
		{
			this.audioBase64 = audioBase64;
		}
	}

	[Serializable]
	private struct OllamaMessage
	{
		public string model;
		public List<OllamaHistory> messages;
		public bool stream;
	}

	[Serializable]
	private struct BarkMessage
	{
		public string prompt;
	}

	[Serializable]
	public struct OllamaHistory
	{
		public string role;
		public string content;
	}

	private struct AudioReturn
	{
		public string error;
		public string status;
		public string transcription;
	}

	[Serializable]
	private struct OllamaReturn
	{
		public string model;
		public string created_at;
		public OllamaHistory message;
		public string done_reason;
		public bool done;
		public long total_duration;
		public long load_duration;
		public int promt_eval_count;
		public double promt_eval_duration;
		public int eval_count;
		public int eval_duration;
	}

	[Serializable]
	private struct BarkReturn
	{
		public string base64;
	}

	[Serializable]
	private struct RAGMessage
	{
		public int maxSitzungsNr;
		public string text;
		public string firstName;
		public string lastName;
	}

	[Serializable]
	private struct RAGReturn
	{
		public TopMessage[] topMessages;
	}

	[Serializable]
	private struct TopMessage
	{
		public string id;
		public string sofaString;
		public string sofaSubstring;
	}
}