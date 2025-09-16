using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class PartyHelper
{
	/// <summary>
	/// Tries to detect the default party name from random string that slightly defer from the default name
	/// </summary>
	/// <param name="partyName"></param>
	/// <returns></returns>
	public static string GetPartyId(string partyName)
	{
		partyName = partyName.ToLower().Replace("*", "").Trim();

		if (partyName.Contains("die linke"))
			return "die linke";

		if (partyName.Contains("grüne") || partyName.Contains("gruene") || partyName.Contains("bündnis") || partyName.Contains("buendniss"))
			return "bündnis 90/die grünen";

		if (partyName.Contains("bsw"))
			return "bsw";

		if (partyName.Contains("union"))
			return "cdu/csu";

		return partyName;
	}

	public static HashSet<string> GetParties(string text)
	{
		text = text.ToLower().Trim();

		HashSet<string> parties = new();

		if (text.Contains("afd"))
		{
			parties.Add("afd");
		}

		if (text.Contains("cdu") || text.Contains("csu"))
		{
			parties.Add("cdu/csu");
		}

		if (text.Contains("fdp"))
		{
			parties.Add("fdp");
		}

		if (text.Contains("grüne") || text.Contains("gruene") || text.Contains("bündnis") || text.Contains("buendniss"))
		{
			parties.Add("bündnis 90/die grünen");
		}

		if (text.Contains("spd"))
		{
			parties.Add("spd");
		}

		if (text.Contains("die linke") || text.Contains("der linke"))
		{
			parties.Add("die linke");
		}

		return parties;
	}

	public static Fraction GetFractionByName(List<Fraction> fractions, string name)
	{
		return fractions.FirstOrDefault(f => f.name == GetPartyId(name));
	}

	public static Color GetFractionColor(string name)
	{
		return GetPartyId(name) switch
		{
			"fraktionslos" => new Color(105 / 255f, 105 / 255f, 105 / 255f),
			"afd" => new Color(0, 158 / 255f, 224 / 255f),
			"cdu/csu" => new Color(21 / 255f, 21 / 255f, 24 / 255f),
			"fdp" => new Color(255f / 255f, 237 / 255f, 0 / 255f),
			"bündnis 90/die grünen" => new Color(64 / 255f, 154 / 255f, 60 / 255f),
			"spd" => new Color(227 / 255f, 0 / 255f, 15 / 255f),
			"die linke" => new Color(190 / 255f, 48 / 255f, 117 / 255f),
			"bsw" => new Color(125 / 255f, 37 / 255f, 79 / 255f),
			_ => new Color(255f / 255f, 255f / 255f, 255f / 255f)
		};
	}

	public static int GetFractionSiting(string name)
	{
		return GetPartyId(name) switch
		{
			"fraktionslos" => 100,
			"afd" => 99,
			"cdu/csu" => 50,
			"fdp" => 0,
			"bündnis 90/die grünen" => -30,
			"spd" => -60,
			"die linke" => -90,
			"bsw" => -99,
			_ => -100
		};
	}
}
