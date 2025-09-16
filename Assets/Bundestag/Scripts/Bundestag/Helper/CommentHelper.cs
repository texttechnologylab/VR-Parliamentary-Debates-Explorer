using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CommentHelper
{
    public static HashSet<string> GetCommentEvent(string text)
    {

		HashSet<string> involvedParties = new();

		text = text.ToLower().Trim();
        if (text.Contains("beifall"))
        {
			involvedParties = PartyHelper.GetParties(text);
		}

		return involvedParties;
    }
}
