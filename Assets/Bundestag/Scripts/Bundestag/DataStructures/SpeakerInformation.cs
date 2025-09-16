using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using WebSocketSharp;
using static SpeakerCommunicator;

public class SpeakerInformation
{
    public int id { get; private set; }
	public string firstName { get; private set; }
	public string lastName { get; private set; }
	public string profession { get; private set; }
	public string party { get; private set; }
	public string biography { get; private set; }
	public string image { get; private set; }
	public Speaker speaker;

	public List<OllamaHistory> conversationHistory { get; private set; }

	public SpeakerInformation() { }

	public SpeakerInformation(int id, string firstName, string lastName, string profession,
        string party, string biography, string image)
    {
		conversationHistory = new();

		this.id = id;
        this.firstName = firstName;
        this.lastName = lastName;
        this.profession = profession;
        this.party = party;
        this.biography = biography;
        this.image = image;
    }

	public SpeakerInformation(JObject requestResult)
	{
		conversationHistory = new();

		if (requestResult.TryGetValue("abgeordnetenId", out JToken id))
            this.id = id.ToObject<int>();

		if (requestResult.TryGetValue("name", out JToken name))
        {
            string[] names = name.ToString().Split(',');
            this.lastName = names[0].Trim();

            if(names.Length > 1)
                this.firstName = names[1].Trim();
		}

		if (requestResult.TryGetValue("beruf", out JToken profession))
			this.profession = profession.ToString();

		if (requestResult.TryGetValue("partei", out JToken party))
			this.party = PartyHelper.GetPartyId(party.ToString());

		if (requestResult.TryGetValue("biografie", out JToken biography))
			this.biography = biography.ToString();

		if (requestResult.TryGetValue("image", out JToken image))
			this.image = image.ToString();
	}

	public void AddPromtHistory(string promt)
	{
		if(conversationHistory.Count == 0)
		{
			string sysPrompt = GenerateSystemPrompt();
			conversationHistory.Add(new OllamaHistory { role = "system", content = sysPrompt });
		}

		conversationHistory.Add(new OllamaHistory { role = "user", content = promt });
	}

	public void AddResponseHistory(OllamaHistory response)
	{
		conversationHistory.Add(response);
	}

	private string GenerateSystemPrompt()
	{
		return $"Sie bist nun ein virtueller Bundestagsabgeordneter basierend auf der Personalisierung unten. " +
			$"Wir halten zu zweit einen Dialog. Antworten Sie im Stil und Argumentationsmustern, die sich aus den eventuell bereitgestellten Redenausschnitten ('Kontext') ergeben. " +
			$"Bleiben Sie in der Rolle und berücksichtigen Sie Partei, Hintergrund und Sprachebene der echten Person." +
			$"Meine Nachrichten beinhalten eien 'Kontext' und eine 'Nachricht'. Die 'Nachricht' ist meine Nachricht an Sie. " +
			$"Der 'Kontext' sind von Ihnen gehaltene Redeausschnitte die zu meiner Nachricht passen und soll in Ihrer Antwort berücksictigt werden.\n\n" +
			$"## Abgeordnetenprofil:\n" +
			$"### Name: {firstName} {lastName}\n" +
			$"### Partei: {party}\n" +
			$"### Biografie: {biography}";
	}
}
