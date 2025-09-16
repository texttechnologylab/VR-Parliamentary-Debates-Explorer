using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static BundestagSpawner;

public class BundestagGenerator: MonoBehaviour
{
	[Header("Parliament Settings")]
	private List<Fraction> _fractions = new List<Fraction>();
	[SerializeField] private int totalRows = 15;
	[SerializeField] private float initialRadius = 5f;
	[SerializeField] private float rowDepth = 1.5f;
	[SerializeField] private float seatWidth = 1.0f;
	[SerializeField] private float aisleWidth = 2.0f;
	[SerializeField] private float startAngle = 20f;
	[SerializeField] private float endAngle = -202;
	[SerializeField] private Speaker delegatePrefab;
	[SerializeField] private int maxSeatsInFrontRow = 3;
	[SerializeField] private float radiusCompensationFactor = 1.5f;

	[Header("Optional Styling")]
	[SerializeField] private bool colorizeSeats = true;

	private TableGenerator _tableGenerator;

	private void Awake()
	{
		_tableGenerator = GetComponent<TableGenerator>();
	}

	public void AutoGenerateParliament(List<Fraction> fractions)
	{
		this._fractions = fractions;

		// Sort seating order
		_fractions.Sort((b, a) => a.sittingPosition.CompareTo(b.sittingPosition));

		GenerateParliament(CalculateSeatDistribution());
	}

	public void ManualGenerateParliament(List<Fraction> fractions, List<FractionSeats> fractionSeats)
	{
		this._fractions = fractions;
		Debug.Log("============");

		Dictionary<int, (int[] seatPerRow, float startAngle)> seatDistribution = new();
		foreach (FractionSeats seats in fractionSeats)
		{
			for (int i = 0; i < _fractions.Count; i++)
			{
				if (_fractions[i].name == seats.fractionName)
				{
					seatDistribution.Add(i, (seats.seatsPerRow, seats.startAngle));
					break;
				}
			}

			Debug.Log(seats.fractionName);

		}

		Debug.Log("============");

		foreach (Fraction fraction in fractions)
		{
			Debug.Log(fraction.name);
		}

		Debug.Log("============");

		GenerateParliament(seatDistribution);
	}

	private void GenerateParliament(Dictionary<int, (int[] seatPerRow, float startAngle)> seatDistribution)
	{		
		// Place delegates
		float totalAngle = endAngle - startAngle;

		float currentAngle = startAngle;

		foreach (int fractionIndex in seatDistribution.Keys)
		{
			Fraction fraction = _fractions[fractionIndex];
			int[] seatsPerRow = seatDistribution[fractionIndex].seatPerRow;

			// Add aisle space between fractions. Ignore first fraction.
			if (fractionIndex > 0)
			{
				currentAngle += aisleWidth / initialRadius;
			}

			// Find the maximum angular width for this fraction
			float maxAngularWidth = 0f;
			for (int row = 0; row < seatsPerRow.Length; row++)
			{
				float rowRadius = initialRadius + (row * rowDepth);
				float rowAngularWidth = seatsPerRow[row] * (seatWidth / rowRadius);
				maxAngularWidth = Mathf.Max(maxAngularWidth, rowAngularWidth);
			}

			// Start placing delegates for this fraction
			float actualFractionStartAngle = seatDistribution[fractionIndex].startAngle == float.MaxValue ? currentAngle : seatDistribution[fractionIndex].startAngle;
			fraction.sittingAngle = actualFractionStartAngle;
			int delegatesPlaced = 0;

			// Create a parent for this fraction's delegates
			GameObject fractionContainer = new GameObject(fraction.name);
			fractionContainer.transform.parent = transform;

			// Place delegates row by row
			for (int row = 0; row < totalRows; row++)
			{
				int seatsInThisRow = seatsPerRow[row];
				if (seatsInThisRow <= 0) continue;

				float rowRadius = initialRadius + (row * rowDepth);
				float rowRadiusSpeaker = rowRadius + 1f;
				float tableAngle = seatWidth / rowRadius;
				float seatAngle = seatWidth / rowRadiusSpeaker;

				// Calculate starting angle for centering seats in this row
				float rowStartTableAngle = actualFractionStartAngle + (seatsPerRow[0] - seatsInThisRow) * tableAngle / 2;
				float rowStartSeatAngle = actualFractionStartAngle + (seatsPerRow[0] - seatsInThisRow) * seatAngle / 2;

				// Place delegates in this row
				for (int seat = 0; seat < seatsInThisRow; seat++)
				{
					float tAngle = rowStartTableAngle + (seat * tableAngle);
					float sAngle = rowStartSeatAngle + (seat * seatAngle);

					// Calculate position
					float speakerX = Mathf.Cos(tAngle) * rowRadiusSpeaker;
					float speakerZ = Mathf.Sin(tAngle) * rowRadiusSpeaker;

					// Create the delegate
					Speaker speaker = CreateDelegate(
						fraction.speakers[delegatesPlaced],
						new Vector3(speakerX, 1, speakerZ),
						rowRadiusSpeaker,
						sAngle,
						GetAisleCenterPosition(fractionIndex, 0),
						fractionContainer.transform);

					float circumferenceAngle = (seat * seatWidth * 360) / (2 * rowRadius * Mathf.PI);
					_tableGenerator.CreateTable(PartyHelper.GetFractionColor(fraction.speakers[delegatesPlaced].party), _tableGenerator.GetTableCoords(Mathf.Rad2Deg * tAngle, rowRadius), 5f, -Mathf.Rad2Deg * tAngle + 90, seatWidth);

					// Create aisle row exit angle
					float aisleExitAngle = rowStartTableAngle + ((seat + 1) * tableAngle);
					if (_fractions[fractionIndex].sittingAngle < aisleExitAngle)
					{
						fraction.sittingAngle = aisleExitAngle;
						_fractions[fractionIndex] = fraction;
					}

					delegatesPlaced++;
					if (delegatesPlaced >= fraction.speakers.Count)
						break;
				}

				if (delegatesPlaced >= fraction.speakers.Count)
					break;
			}

			Debug.Log("Render Table " + (Mathf.Rad2Deg * actualFractionStartAngle));
			_tableGenerator.RenderTable(_fractions[fractionIndex].name, 0, new Vector3(Mathf.Cos(actualFractionStartAngle) * 20, 1, Mathf.Sin(actualFractionStartAngle) * 20));

			// Store the ending angle of this fraction
			float fractionEndAngle = currentAngle + maxAngularWidth;

			// Move to the next fraction
			currentAngle += maxAngularWidth;
		}

		foreach (int fractionIndex in seatDistribution.Keys)
		{
			foreach (SpeakerInformation speaker in _fractions[fractionIndex].speakers)
			{
				speaker.speaker.SetRowExitAngle(_fractions[fractionIndex].sittingAngle);
			}
		}
	}

	private Dictionary<int, (int[] seatPerRow, float startAngle)> CalculateSeatDistribution()
	{
		Dictionary<int, (int[] seatPerRow, float startAngle)> distribution = new();

		int fractionIndex = 0;
		foreach (Fraction fraction in _fractions)
		{
			int[] seatsPerRow = new int[totalRows];
			int remainingDelegates = fraction.speakers.Count;

			// Calculate the circumference ratio between the last row and first row
			float frontRowRadius = initialRadius;
			float backRowRadius = initialRadius + ((totalRows - 1) * rowDepth);
			float circumferenceRatio = backRowRadius / frontRowRadius;

			// Calculate a distribution factor for each row based on its radius
			float[] rowDistributionFactors = new float[totalRows];
			float totalFactor = 0f;

			for (int row = 0; row < totalRows; row++)
			{
				float rowRadius = initialRadius + (row * rowDepth);
				// Linear interpolation between 1 and circumferenceRatio, adjusted by compensation factor
				float factor = 1 + (row / (float)(totalRows - 1)) * (circumferenceRatio - 1) * radiusCompensationFactor;
				rowDistributionFactors[row] = factor;
				totalFactor += factor;
			}

			// Normalize factors to distribute the exact number of delegates
			for (int row = 0; row < totalRows; row++)
			{
				rowDistributionFactors[row] /= totalFactor;

				int seatsInRow = Mathf.RoundToInt(fraction.speakers.Count * rowDistributionFactors[row]);

				if (row == 0)
				{
					seatsInRow = Mathf.Min(seatsInRow, maxSeatsInFrontRow);
				}

				// Update remaining delegates
				seatsPerRow[row] = seatsInRow;
				remainingDelegates -= seatsInRow;
			}

			// Adjust if we have too many or too few delegates allocated
			if (remainingDelegates < 0)
			{
				// Remove extra delegates starting from the front rows
				for (int row = 0; row < totalRows && remainingDelegates < 0; row++)
				{
					// Ensure we leave at least 1 seat per row
					int canRemove = Mathf.Max(0, seatsPerRow[row] - 1);
					int remove = Mathf.Min(canRemove, -remainingDelegates);

					seatsPerRow[row] -= remove;
					remainingDelegates += remove;
				}
			}
			else if (remainingDelegates > 0)
			{
				// Distribute remaining delegates to back rows first
				for (int row = totalRows - 1; row >= 0 && remainingDelegates > 0; row--)
				{
					// Skip front row if it's already at max
					if (row == 0 && seatsPerRow[0] >= maxSeatsInFrontRow)
						continue;

					seatsPerRow[row]++;
					remainingDelegates--;
				}
			}

			distribution[fractionIndex] = (seatsPerRow, float.MaxValue);
			fractionIndex++;
		}

		return distribution;
	}

	public Vector3 GetAisleCenterPosition(int fractionIndex, int row)
	{
		float angle = _fractions[fractionIndex].sittingAngle;
		float rowRadius = initialRadius + (row * rowDepth);

		float x = Mathf.Cos(angle) * rowRadius;
		float z = Mathf.Sin(angle) * rowRadius;

		return new Vector3(x, 0, z);
	}

	public Vector3 GetAisleCenterPosition(Fraction fraction, int row)
	{
		float angle = fraction.sittingAngle;
		float rowRadius = initialRadius + (row * rowDepth);

		float x = Mathf.Cos(angle) * rowRadius;
		float z = Mathf.Sin(angle) * rowRadius;

		return new Vector3(x, 0, z);
	}

	private Speaker CreateDelegate(SpeakerInformation si, Vector3 position, float rowRadius, float aisleAngle, Vector3 aislePosition, Transform parent)
	{
		Speaker speaker = Instantiate(delegatePrefab, position, Quaternion.identity, parent);
		speaker.Initialize(si, rowRadius, aisleAngle, aislePosition);

		// Apply fraction color if applicable
		if (colorizeSeats)
		{
			/*Renderer renderer = delegateObj.GetComponent<Renderer>();
			if (renderer != null)
			{
				renderer.material.color = fraction.color;
			}*/
		}

		return speaker;
	}
}