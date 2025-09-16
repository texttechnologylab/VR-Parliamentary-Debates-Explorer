using Newtonsoft.Json.Linq;
using Oculus.Platform.Models;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(BundestagGenerator))]
public class BundestagSpawner : MonoBehaviour
{
	private BundestagGenerator _bundestagGenerator;

	private List<Fraction> _fractions = new List<Fraction>();

	[SerializeField] private bool _enableAutomaticGeneration;

	[Space(5f)]
	[Header("Automatic Generation")]
	[SerializeField] private List<MergeFractions> _fractionNamesToMerge;

	[Space(5f)]
	[Header("Manual Generation")]
	[SerializeField] private List<FractionSeats> _manualFractionSeats;

	public void Initialize()
	{
		_bundestagGenerator = GetComponent<BundestagGenerator>();

		if (_enableAutomaticGeneration)
		{
			foreach (var fraction in _fractionNamesToMerge)
			{
				fraction.cleanList();
			}
		}
		else
		{
			foreach (var fractionSeats in _manualFractionSeats)
			{
				fractionSeats.cleanValues();
			}
		}

		InitializeRoom();

		InitializeAbgeordnete();
	}

	private void InitializeRoom()
	{
		StartCoroutine(GetSpeakers());
	}

	private void InitializeAbgeordnete()
	{

	}

	IEnumerator GetSpeakers()
	{
		UnityWebRequest uwr = UnityWebRequest.Get("http://service.vrparliament.lehre.texttechnologylab.org/abgeordnete/getAllIds");
		yield return uwr.SendWebRequest();

		if (uwr.result == UnityWebRequest.Result.ConnectionError)
		{
			Debug.Log("Error While Sending: " + uwr.error);
		}
		else
		{
			JArray jArray = JArray.Parse(uwr.downloadHandler.text);

			List<int> ids = new List<int>();
			foreach (var vSpeakerNo in jArray)
			{
				int speakerNo = vSpeakerNo.ToObject<int>();
				ids.Add(speakerNo);
			}

			yield return StartCoroutine(AddSpeakerById(ids));

			if(_enableAutomaticGeneration)
				_bundestagGenerator.AutoGenerateParliament(_fractions);
			else
				_bundestagGenerator.ManualGenerateParliament(_fractions, _manualFractionSeats);

			/*
			_tableGenerator.CreateTables("afd", _speakers["afd"], Color.blue, 20f,
				new List<int> { 1, 3, 3, 4, 5, 6 },
				new List<int> { 8, 9, 10, 11, 12, 10 });  // 5 should be enough

			_tableGenerator.CreateTables("fraktionslos", _speakers["fraktionslos"], Color.blue, 20f,
				new List<int> { 0, 0, 0, 0, 0, 0 },
				new List<int> { 0, 0, 0, 0, 0, 0, 13 });  // 7 should be enough

			_tableGenerator.CreateTables("cdu/csu", _speakers["cdu/csu"], Color.black, -7.6f,
				new List<int> { 5, 6, 9, 10, 12, 13 },
				new List<int> { 19, 20, 22, 23, 25, 27, 15 });  // 6 should be enough

			_tableGenerator.CreateTables("fdp", _speakers["fdp"], Color.yellow, -67,
				new List<int> { 2, 3, 4, 5, 6, 6 },
				new List<int> { 7, 8, 9, 10, 11, 12, 13 });  // 8 should be enough

			_tableGenerator.CreateTables("bündnis 90/die grünen", _speakers["bündnis 90/die grünen"], Color.green, -101,
				new List<int> { 2, 4, 5, 6, 7, 8 },
				new List<int> { 10, 11, 12, 13, 14, 15, 16 });  // 11 should be enough

			_tableGenerator.CreateTables("spd", _speakers["spd"], Color.red, -140,
				new List<int> { 4, 6, 8, 11, 14, 15 },
				new List<int> { 18, 20, 22, 22, 24, 26, 28, 5 });  // 17 should be enough

			_tableGenerator.CreateTables("die linke", _speakers["die linke"], Color.red, -189,
				new List<int> { 0, 0, 0, 0, 0, 0 },
				new List<int> { 2, 3, 4, 4, 5, 5, 8 }); // 6 should be enough

			_tableGenerator.CreateTables("bsw", _speakers["bsw"], Color.red, -202,
				new List<int> { 0, 0, 0, 0, 0, 0 },
				new List<int> { 1, 1, 1, 1, 1, 2, 3 }); // 3 should be enough
			*/
		}
	}

	IEnumerator AddSpeakerById(List<int> ids)
	{
		foreach (int id in ids)
		{
			UnityWebRequest uwr = UnityWebRequest.Get($"http://service.vrparliament.lehre.texttechnologylab.org/abgeordnete/getAbgeordneterById/{id}");
			yield return uwr.SendWebRequest();

			if (uwr.result == UnityWebRequest.Result.ConnectionError)
			{
				Debug.Log("Error While Sending: " + uwr.error);
			}
			else
			{
				JObject json = JObject.Parse(uwr.downloadHandler.text);
				SpeakerInformation si = new SpeakerInformation(json);

				Debug.Log(si.firstName + " " + si.lastName);

				string frationName = si.party;
				if (_enableAutomaticGeneration)
				{
					foreach (MergeFractions mFractrion in _fractionNamesToMerge)
					{
						if (mFractrion.shouldMerge(frationName))
						{
							frationName = mFractrion.fractionNames[0];
							break;
						}
					}
				}

				Fraction fraction = PartyHelper.GetFractionByName(_fractions, frationName);
				if (fraction.name != default)  // If the fraction exists
				{
					fraction.speakers.Add(si);
				}
				else
				{
					_fractions.Add(new Fraction
					{
						name = PartyHelper.GetPartyId(frationName),
						speakers = new() { si },
						color = PartyHelper.GetFractionColor(frationName),
						sittingPosition = PartyHelper.GetFractionSiting(frationName)
					});
				}
			}
		}

		print(_fractions[0].speakers.Count);
		print(_fractions[1].speakers.Count);
		print(_fractions[2].speakers.Count);

	}

	/// <summary>
	/// Tries to get speaker by id, if that fails search for name
	/// </summary>
	/// <param name="party"></param>
	/// <param name="id"></param>
	/// <param name="firstName"></param>
	/// <param name="lastName"></param>
	/// <returns></returns>
	public SpeakerInformation GetSpeaker(string party, int id, string firstName, string lastName)
	{
		SpeakerInformation si = GetSpeaker(party, id);

		if (si == null)
			si = GetSpeaker(party, firstName, lastName);

		return si;
	}

	public SpeakerInformation GetSpeaker(string party, string firstName, string lastName)
	{
		firstName = firstName.Replace(",", "").Trim();
		lastName = lastName.Replace(":", "").Trim();

		Fraction fraction = PartyHelper.GetFractionByName(_fractions, party);
		// Faster iteration if possible
		if (fraction.name != default)
		{
			Debug.Log(fraction.name + " " + firstName + " " + lastName);

			foreach (var f in fraction.speakers)
			{
				Debug.Log(firstName + " " + lastName + " " + f.firstName + " " + f.lastName);	
			}

			// TODO: Also check for fristname
			SpeakerInformation si = fraction.speakers.Find(sp => sp.firstName == firstName && sp.lastName == lastName);

			if (si != null)
				return si;
		}

		return GetSpeaker(firstName, lastName);
	}

	public SpeakerInformation GetSpeaker(string firstName, string lastName)
	{
		firstName = firstName.Replace(",", "").Trim();
		lastName = lastName.Replace(":", "").Trim();

		foreach (var fraction in _fractions)
		{
			// TODO: Also check for fristname
			SpeakerInformation si = fraction.speakers.Find(sp => sp.firstName == firstName && sp.lastName == lastName);

			if (si != null)
				return si;
		}

		return null;
	}

	public SpeakerInformation GetSpeaker(string party, int id)
	{
		Fraction fraction = PartyHelper.GetFractionByName(_fractions, party);
		// Faster iteration if possible
		if (fraction.name != default)
		{
			SpeakerInformation si = fraction.speakers.Find(sp => sp.id == id);

			if (si != null)
				return si;
		}

		// Iterate trhrough every speaker
		return GetSpeaker(id);
	}

	public SpeakerInformation GetSpeaker(int id)
	{
		// Iterate through every speaker
		foreach (var fraction in _fractions)
		{
			SpeakerInformation si = fraction.speakers.Find(sp => sp.id == id);

			if (si != null)
				return si;
		}

		return null;
	}

	public List<SpeakerInformation> GetAllSpeakers()
	{
		List<SpeakerInformation> siList = new();

		foreach (Fraction frac in _fractions)
		{
			siList.AddRange(frac.speakers);
		}

		return siList;
	}

	[System.Serializable]
	private class MergeFractions
	{
		public List<string> fractionNames;

		public void cleanList()
		{
			for (int i = 0; i < fractionNames.Count; i++)
			{
				fractionNames[i] = PartyHelper.GetPartyId(fractionNames[i]);
			}
		}

		public bool shouldMerge(string name)
		{
			return fractionNames.Contains(PartyHelper.GetPartyId(name));
		}
	}

	[System.Serializable]
	public class FractionSeats
	{
		public string fractionName;
		public int[] seatsPerRow;
		public float startAngle;

		public void cleanValues()
		{
			fractionName = PartyHelper.GetPartyId(fractionName);
			startAngle = Mathf.Deg2Rad * startAngle;
		}
	}
}
