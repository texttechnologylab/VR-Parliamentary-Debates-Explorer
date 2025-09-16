using SIPSorceryMedia.Abstractions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using WebSocketSharp;
using static SpeechSelectorUI;

[RequireComponent(typeof(BundestagSpawner))]
public class BundestagManager : MonoBehaviour
{
	private BundestagSpawner _spawner;
	[SerializeField] private SpeechSelectorUI _speechUI;
	[SerializeField] private SpeakerCommunicator _speakerCommunicator;
	private Dictionary<string, Speaker> _partyAudioSources;
	[SerializeField] private AudioClip _clapClip;
	[SerializeField] private AudioClip _laughClip;

	private void Awake()
	{
		_partyAudioSources = new();
		_spawner = GetComponent<BundestagSpawner>();
	}

	private void Start()
	{
		_spawner.Initialize();
	}

	public void AddPartyAudio(string party, Speaker audioSource)
	{
		_partyAudioSources.Add(party, audioSource);
	}

	private Speaker _mainSpeaker = null;
	private Coroutine currentSpeech;
	private List<(List<Speaker> speaker, AudioObject b64, string text)> speechSectionsB64 = new();
	private bool _cancel = false;
	public async void SpeakSpeech(SpeechResponse speech)
	{
		_cancel = false;
		// Get Main Speaker
		Speakr mainSpeaker = speech.speechSections[0].speaker;
		Debug.Log("Mainspeaker id: " + mainSpeaker.id);
		SpeakerInformation si = _spawner.GetSpeaker(mainSpeaker.party, int.Parse(mainSpeaker.id), mainSpeaker.fristName, mainSpeaker.lastName);

		if (si == null)
		{
			Debug.LogError("error: " + mainSpeaker.id + " Not found " + mainSpeaker.fristName + " " + mainSpeaker.lastName);
			return;
		}

		print(si.firstName + " " + si.lastName + " " + si.id);
		Speaker speaker = si.speaker;

		// Load voice //
		StopCurrentSpeech();
		if (_mainSpeaker != null)
			_mainSpeaker.GoToChair();
		speaker.StartWalkingToPodium();

		_mainSpeaker = speaker;
		bool startedSpeaking = false;

		// Get Speech sections //
		foreach (var section in speech.speechSections)
		{
			if (_cancel)
				return;

			int end = section.end;
			if (end > speech.sofaString.Length)
				end = speech.sofaString.Length - 1;

			string sectionText = speech.sofaString.Substring(section.begin, section.end - section.begin);

			if (section.speaker.id.IsNullOrEmpty() && section.speaker.lastName.IsNullOrEmpty())
			{
				if (section.type == "comment")
				{
					print("this one seems like a Beifall! " + sectionText);
					HashSet<string> parties = CommentHelper.GetCommentEvent(sectionText);

					List<Speaker> partySpeakers = new List<Speaker>();
					foreach (var party in parties)
					{
						if(_partyAudioSources.TryGetValue(party, out Speaker partySpeaker))
						{
							partySpeakers.Add(partySpeaker);
						}
					}

					speechSectionsB64.Add((partySpeakers, new AudioObject { audioClip = _clapClip}, sectionText));
				}
			}
			else
			{
				// Get section's speaker
				Speakr sectionSpeaker = section.speaker;
				if (sectionSpeaker.id.IsNullOrEmpty())
				{
					si = _spawner.GetSpeaker(sectionSpeaker.party, sectionSpeaker.fristName, sectionSpeaker.lastName);
				}
				else
				{
					si = _spawner.GetSpeaker(sectionSpeaker.party, int.Parse(sectionSpeaker.id), sectionSpeaker.fristName, sectionSpeaker.lastName);
				}

				Debug.Log(speech.sofaString.Length + " " + section.begin + " " + section.end);

				// Get Voice
				string b64Audio = await _speakerCommunicator.TextToSpeech(sectionText);

				Debug.Log(sectionSpeaker.party + " | " + sectionSpeaker.fristName + " | " + sectionSpeaker.lastName + " | " + sectionText);

				speechSectionsB64.Add((si.speaker.ToList(), new AudioObject { b64Audio = b64Audio }, sectionText));
			}

			if (speech.sofaString.Length / 2 < end)
			{
				if (!startedSpeaking)
				{
					startedSpeaking = true;

					currentSpeech = StartCoroutine(SpeakBase64Enumerator());
				}
			}

		}
	}

	private List<Speaker> currentSpeakers = new();
	private IEnumerator SpeakBase64Enumerator()
	{
		int i = 0;
		while (speechSectionsB64.Count > i)
		{
			if(i == 0)
			{
				while (speechSectionsB64[i].speaker[0].speakerAction != SpeakerAction.Speaking)
				{
					yield return null;
				}
			}

			// Play file
			print("=======");
			currentSpeakers = speechSectionsB64[i].speaker;

			foreach (Speaker speaker in currentSpeakers)
			{
				print(speaker.gameObject.name);
				speaker.SpeakBase64(speechSectionsB64[i].b64);
			}

			_speechUI.DisplayTextOnBoard(speechSectionsB64[i].text);

			// Check if every speaker started speaking
			bool doesntSpeak = true;
			while (doesntSpeak)
			{
				doesntSpeak = false;
				foreach (Speaker speaker in currentSpeakers)
				{
					if (!speaker.StartedSpeaking())
						doesntSpeak = true;
				}

				yield return new WaitForEndOfFrame();
			}



			bool isSpeaking = true;
			// Wait until section is finished => start next one in next iteration
			while (isSpeaking)
			{
				isSpeaking = false;
				foreach (Speaker speaker in currentSpeakers)
				{
					if (speaker.IsSpeaking())
					{
						isSpeaking = true;
						break;
					}
				}

				yield return null;
			}

			i++;
		}
	}

	public void StopCurrentSpeech()
	{
		if (currentSpeech != null)
		{
			StopCoroutine(currentSpeech);
		}

		foreach(Speaker speaker in currentSpeakers)
		{
			speaker.StopSpeak();
		}

		speechSectionsB64.Clear();
	}

	public void StopGenerateSpeech()
	{
		_cancel = true;
		StopCurrentSpeech();
	}

	public void TogglePauseSpeech()
	{
		foreach (Speaker speaker in currentSpeakers)
		{
			speaker.TogglePauseSpeech();
		}
	}
}
