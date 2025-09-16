using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Fraction
{
    public string name;
    public List<SpeakerInformation> speakers;
    public Color color;
	/// <summary>
	/// The more negative the number the more left, the more positive the more right. 0 is neurtral. This is not a political statement but rather based on the design of number rays.
	/// </summary>
	public int sittingPosition;
	public float sittingAngle;
}
