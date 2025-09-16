using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
using static SpeechSelectorUI;

[RequireComponent(typeof(MeshFilter))]
public class TableGenerator : MonoBehaviour
{
	[SerializeField] private BundestagManager _manager;

	[Space(10)]
	[Header("Table Generation")]
	[SerializeField] private GameObject _tableArea;
	[SerializeField] private float _baseTableWidth = 0.9f;
	[SerializeField] private float _tableSizeScaling = 0.03f;
	[SerializeField] private Color _baseColor;


	private List<Vector3> _verts;
	private List<int> _tris;
	private List<Color> _vertexColors;

	private void Awake()
	{
		ResetMesh();
	}

	private void ResetMesh()
	{
		_verts = new();
		_tris = new();
		_vertexColors = new();
	}

	public void RenderTable(string party, float angle, Vector3 audioPosition)
	{
		GameObject area = Instantiate(_tableArea, Vector3.zero, Quaternion.Euler(0, angle, 0));

		GameObject areaSppeakerGO = area.transform.GetChild(0).gameObject;  // Speaker Child
		_manager.AddPartyAudio(party, areaSppeakerGO.GetComponent<Speaker>());
		areaSppeakerGO.GetComponent<Speaker>().BasicInitialize(party);
		areaSppeakerGO.transform.position = audioPosition;

		UnityEngine.Mesh mesh = area.GetComponent<MeshFilter>().mesh;

		mesh.Clear();
		mesh.vertices = _verts.ToArray();
		mesh.triangles = _tris.ToArray();
		mesh.colors = _vertexColors.ToArray();
		mesh.RecalculateNormals();
		mesh.RecalculateBounds();
		mesh.RecalculateTangents();

		area.GetComponent<MeshCollider>().sharedMesh = mesh;

		ResetMesh();

	}

	public void CreateTable(Color topColor, Vector3 position, float angle, float rotation, float width = 0.8f, float height = 0.7f, float depth = 0.5f)
    {

		float frontWidth = 0.2f;

		Vector3 topPosPivot = position + new Vector3(0, height, 0);
		Vector3 topPos = position + new Vector3(0, height, frontWidth);

		Vector3 topPos2 = position + new Vector3(0, height + 0.1f, 0);

		width += 0.0025f;

		Vector3[] top = new Vector3[] {

			// Bottom layer
			// 3--------2
			//  |    / |
			//   | /  |
			//   0----1
			RotatePointAroundPivot(topPos + new Vector3(-width / 2f, 0, 0), topPosPivot, rotation),
			RotatePointAroundPivot(topPos + new Vector3(width / 2f, 0, 0), topPosPivot, rotation),
			RotatePointAroundPivot(topPos + new Vector3(width / 2f, 0, 0) + Angle2Pos(depth - frontWidth, 90 - angle), topPosPivot, rotation),
			RotatePointAroundPivot(topPos + new Vector3(-width / 2f, 0, 0) + Angle2Pos(depth - frontWidth, 90 + angle), topPosPivot, rotation),

			// Top layer
			// 7--------6
			//  |    / |
			//   | /  |
			//   4----5
			RotatePointAroundPivot(topPos2 + new Vector3(-width / 2f, 0, 0), topPos2, rotation),
			RotatePointAroundPivot(topPos2 + new Vector3(width / 2f, 0, 0), topPos2, rotation),
			RotatePointAroundPivot(topPos2 + new Vector3(width / 2f, 0, 0) + Angle2Pos(depth, 90 - angle), topPos2, rotation),
			RotatePointAroundPivot(topPos2 + new Vector3(-width / 2f, 0, 0) + Angle2Pos(depth, 90 + angle), topPos2, rotation)
		};

		_vertexColors.Add(_baseColor);
		_vertexColors.Add(_baseColor);
		_vertexColors.Add(_baseColor);
		_vertexColors.Add(_baseColor);
		_vertexColors.Add(_baseColor);
		_vertexColors.Add(_baseColor);
		_vertexColors.Add(_baseColor);
		_vertexColors.Add(_baseColor);

		Vector3[] front = new Vector3[] {

			// Front layer
			// 4----5
			// |    |
			// |    |
			// |    |
			// 8----9
			RotatePointAroundPivot(position + new Vector3(-width / 2f, 0, 0), position, rotation),
			RotatePointAroundPivot(position + new Vector3(width / 2f, 0, 0), position, rotation),

			// Back layer
			// 0----1
			// |    |
			// |    |
			// |    |
			// 10---11
			RotatePointAroundPivot(position + new Vector3(-width / 2f, 0, 0) + Angle2Pos(frontWidth, 90 + angle), position, rotation),
			RotatePointAroundPivot(position + new Vector3(width / 2f, 0, 0) + Angle2Pos(frontWidth, 90 - angle), position, rotation),
		};

		_vertexColors.Add(_baseColor);
		_vertexColors.Add(_baseColor);
		_vertexColors.Add(_baseColor);
		_vertexColors.Add(_baseColor);

		// Copy of top layer so vertex colors work correctly
		Vector3[] actualTop = new Vector3[] {
			// Top layer
			// 15--------14
			//  |    /   |
			//   | /    |
			//   12----13
			RotatePointAroundPivot(topPos2 + new Vector3(-width / 2f, 0, 0), topPos2, rotation),
			RotatePointAroundPivot(topPos2 + new Vector3(width / 2f, 0, 0), topPos2, rotation),
			RotatePointAroundPivot(topPos2 + new Vector3(width / 2f, 0, 0) + Angle2Pos(depth, 90 - angle), topPos2, rotation),
			RotatePointAroundPivot(topPos2 + new Vector3(-width / 2f, 0, 0) + Angle2Pos(depth, 90 + angle), topPos2, rotation)
		};

		_vertexColors.Add(topColor);
		_vertexColors.Add(topColor);
		_vertexColors.Add(topColor);
		_vertexColors.Add(topColor);

		int count = _verts.Count;

		_verts.AddRange(top);
		_verts.AddRange(front);
		_verts.AddRange(actualTop);


		// Top
		_tris.Add(count + 2);
        _tris.Add(count + 3);
		_tris.Add(count + 0);
		_tris.Add(count + 0);
		_tris.Add(count + 1);
		_tris.Add(count + 2);

		_tris.Add(count + 14);
		_tris.Add(count + 13);
		_tris.Add(count + 12);
		_tris.Add(count + 12);
		_tris.Add(count + 15);
		_tris.Add(count + 14);

		// Tops back
		_tris.Add(count + 6);
		_tris.Add(count + 7);
		_tris.Add(count + 3);
		_tris.Add(count + 3);
		_tris.Add(count + 2);
		_tris.Add(count + 6);

		// Tops side
		_tris.Add(count + 6);
		_tris.Add(count + 2);
		_tris.Add(count + 1);
		_tris.Add(count + 1);
		_tris.Add(count + 5);
		_tris.Add(count + 6);

		_tris.Add(count + 7);
		_tris.Add(count + 4);
		_tris.Add(count + 0);
		_tris.Add(count + 0);
		_tris.Add(count + 3);
		_tris.Add(count + 7);

		// Front
		_tris.Add(count + 5);
		_tris.Add(count + 9);
		_tris.Add(count + 8);
		_tris.Add(count + 8);
		_tris.Add(count + 4);
		_tris.Add(count + 5);

		_tris.Add(count + 1);
		_tris.Add(count + 0);
		_tris.Add(count + 10);
		_tris.Add(count + 10);
		_tris.Add(count + 11);
		_tris.Add(count + 1);

		// Fronts side
		_tris.Add(count + 1);
		_tris.Add(count + 11);
		_tris.Add(count + 9);
		_tris.Add(count + 9);
		_tris.Add(count + 5);
		_tris.Add(count + 1);

		_tris.Add(count + 0);
		_tris.Add(count + 4);
		_tris.Add(count + 8);
		_tris.Add(count + 8);
		_tris.Add(count + 10);
		_tris.Add(count + 0);

	}

	public Vector3 GetTableCoords(float circumferenceAngle, float radius)
	{
		Vector3 pos = Angle2Pos(radius, circumferenceAngle);

		pos.y = 0;
		
		return pos;
	}

	private Vector3 Angle2Pos(float radius, float angle)
    {
		float rad = Mathf.Deg2Rad * angle;

		float x = radius * Mathf.Cos(rad);
		float y = radius * Mathf.Sin(rad);

		return new Vector3(x, 0, y);
	}

	private Vector3 RotatePointAroundPivot(Vector3 point, Vector3 pivot, float angle)
	{
		Vector3 dir = point - pivot;
		dir = Quaternion.Euler(new Vector3(0, angle, 0)) * dir;
		point = dir + pivot;
		return point;
	}
}

[System.Serializable]
public class TableRow
{
	public bool[] doesNotExist;
}
