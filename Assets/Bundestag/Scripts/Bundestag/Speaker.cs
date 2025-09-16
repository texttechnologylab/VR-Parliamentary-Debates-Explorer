using Oculus.Interaction;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UIElements;

public class Speaker : MonoBehaviour
{
	public SpeakerInformation information { get; private set; }

	private AudioSource _audioSource;

	private float _rowRadius;
	private float _positionAngle;
	private float _rowExitAngle;
	private Vector3 _isleExitPos;
	private Vector3 _baseRotation;
	private Vector3 _chairPosition;

	public SpeakerAction speakerAction;
	private Vector3 _nextPathfindWaypoint;

	private float _currentRotation;
	[SerializeField] private float _moveSpeed;
	[SerializeField] private float _moveFromChairDistance = 1.5f;

	[SerializeField] private List<Vector3> _waypoints;
	[SerializeField] private Vector3 _destinationWaypoint;
	private Stack<Vector3> _waypointHistory = new();

	// Moving out of row
	private float _currentAngle;

	public void SetRowExitAngle(float angle)
	{
		_rowExitAngle = angle;
	}

	public void Initialize(SpeakerInformation information, float rowRadius, float positionAngle, Vector3 isleExitPos)
	{
		information.speaker = this;
		transform.LookAt(new Vector3(0, 1.8f, 0));

		this.information = information;

		ApplyImage();

		_positionAngle = positionAngle;

		_baseRotation = transform.rotation.eulerAngles;
		_rowRadius = rowRadius + _moveFromChairDistance;

		_isleExitPos = isleExitPos;

		_nextPathfindWaypoint = Vector3.zero;
		_chairPosition = transform.position;

		gameObject.name = information.firstName + " " + information.lastName + " [" + information.id + "]";

		// Vocal part
		_audioSource = GetComponent<AudioSource>();

		MD5 hash = MD5.Create();
		var hashed = hash.ComputeHash(Encoding.UTF8.GetBytes(gameObject.name));

		int iHash = BitConverter.ToInt32(hashed, 0);

		float pitch = Mathf.Abs(iHash) % 21;
		pitch = pitch - 5;
		pitch = pitch / 100f;
		_audioSource.pitch = 1 + pitch;
	}

	public void BasicInitialize(string name)
	{
		gameObject.name = name;
        _audioSource = GetComponent<AudioSource>();
		_chairPosition = transform.position;

        MD5 hash = MD5.Create();
		var hashed = hash.ComputeHash(Encoding.UTF8.GetBytes(gameObject.name));

		int iHash = BitConverter.ToInt32(hashed, 0);

		float pitch = Mathf.Abs(iHash) % 21;
		pitch = pitch - 5;
		pitch = pitch / 100f;
		_audioSource.pitch = 1 + pitch;
	}

	private void ApplyImage()
	{
		if (information.image == null) return;

		string base64String = information.image;
		byte[] data = System.Convert.FromBase64String(base64String);
		Texture2D texture = new Texture2D(0, 0);
		ImageConversion.LoadImage(texture, data);

		GetComponent<MeshRenderer>().material.mainTexture = texture;
	}

	public void StartWalkingToPodium()
	{
		if (speakerAction == SpeakerAction.Listening)
			speakerAction = SpeakerAction.StartWalkingToPodium;
	}

	public void Update()
	{
		if(transform.localScale.y == 0.9f && speakerAction == SpeakerAction.Listening)
			speakerAction = SpeakerAction.StartWalkingToPodium;

		switch (speakerAction)
		{
			case SpeakerAction.StartWalkingToPodium:
				_currentRotation = 0;
				speakerAction = SpeakerAction.TurnAround;
				transform.localScale = new Vector3(0.75f, 0.75f, 0.75f);
				break;

			case SpeakerAction.TurnAround:
				TurnAwayFromChair();
				break;

			case SpeakerAction.WalkAwayFromChair:
				WalkAwayFromChair();
				break;

			case SpeakerAction.WalkOutOfRow:
				WalkOutOfRow();
				break;

			case SpeakerAction.WalkDownIsle:
				WalkDownIsle();
				break;

			case SpeakerAction.WalkToPodium:
				WalkToPodium();
				break;

			default:
				break;
		}
	}

	private void TurnAwayFromChair()
	{
		transform.rotation = Quaternion.Euler(_baseRotation.x, Mathf.SmoothDamp(_baseRotation.y, _baseRotation.y + 180, ref _currentRotation, 0.5f), _baseRotation.z);
		if (transform.rotation.y - 180 < _baseRotation.y + 3 && transform.rotation.y - 180 > _baseRotation.y - 3)
		{
			speakerAction = SpeakerAction.WalkAwayFromChair;
		}

		//Skip for now
		speakerAction = SpeakerAction.WalkAwayFromChair;
	}

	private void WalkAwayFromChair()
	{
		Vector3 targetPos = _chairPosition + transform.position.normalized * _moveFromChairDistance;

		if (!MoveTo(targetPos))
		{
			_currentAngle = _positionAngle;
			speakerAction = SpeakerAction.WalkOutOfRow;
		}
	}

	private void WalkOutOfRow()
	{

		float x = Mathf.Cos(_currentAngle) * _rowRadius;
		float z = Mathf.Sin(_currentAngle) * _rowRadius;

		transform.position = new Vector3(x, transform.position.y, z);
		_currentAngle += Mathf.Deg2Rad * 5 / _rowRadius * _moveSpeed * Time.deltaTime;

		if(_rowExitAngle - _currentAngle < Mathf.Deg2Rad * 2f && _rowExitAngle - _currentAngle > Mathf.Deg2Rad * -2f)
		{
			speakerAction = SpeakerAction.WalkDownIsle;
		}
	}

	private void WalkDownIsle()
	{
		if (!MoveTo(_isleExitPos, false))
		{
			_nextPathfindWaypoint = Vector3.zero;
			_waypointHistory.Clear();
			speakerAction = SpeakerAction.WalkToPodium;
		}
	}

	public void WalkToPodium()
	{
		if (_nextPathfindWaypoint == Vector3.zero)
		{
			Vector3 closest = Vector3.zero;
			foreach (Vector3 t in _waypoints)
			{
				if (_waypointHistory.Count > 0 && _waypointHistory.Peek() == t)
					continue;

				if (closest == Vector3.zero)
					closest = t;

				if (Vector3.Distance(transform.position, t) < Vector3.Distance(transform.position, closest)){
					closest = t;
				}
			}

			if (Vector3.Distance(transform.position, _destinationWaypoint) < Vector3.Distance(transform.position, closest)){
				closest = _destinationWaypoint;
			}

			_nextPathfindWaypoint = closest;
			_waypointHistory.Push(closest);
		}

		if (!MoveTo(_nextPathfindWaypoint))
		{
			if (_waypointHistory.Peek() == _destinationWaypoint)
			{
				speakerAction = SpeakerAction.Speaking;
				transform.LookAt(new Vector3(0, 0.5f, 0));
				transform.position = transform.position + new Vector3(0, 0.5f, 0);
			}
			else
				_nextPathfindWaypoint = Vector3.zero;
		}
	}

	/// <summary>
	/// Move towards a point
	/// </summary>
	/// <param name="targetPos"> The point to walk to</param>
	/// <returns> True is walked, false if destination reached </returns>
	private bool MoveTo(Vector3 targetPos, bool walkY = false)
	{
		Vector3 direction = targetPos - transform.position;
		direction = Vector3.Normalize(direction);

		Vector3 movement = direction * _moveSpeed * 0.1f * Time.deltaTime;
		if (!walkY)
		{
			movement.y = 0;

			if (Vector2.Distance(new Vector2(transform.position.x, transform.position.z), new Vector2(targetPos.x, targetPos.z)) > 0.15f)
			{
				transform.position += movement;
				return true;
			}
		}
		else
		{
			if (Vector3.Distance(transform.position, targetPos) > 0.1f)
			{
				transform.position += movement;
				return true;
			}
		}

		if (!walkY)
		{
			transform.position = new Vector3(targetPos.x, transform.position.y, targetPos.z);
		}
		else
		{
			transform.position = targetPos;
		}

		return false;
	}

	public void GoToChair()
    {
		transform.position = _chairPosition;
		speakerAction = SpeakerAction.Listening;

	}

	private Coroutine _currentSpeaking;
	private List<AudioObject> _audioObjects = new();
	public void StopSpeak()
	{
		if(_currentSpeaking != null)
		{
			StopCoroutine(_currentSpeaking);
		}

		_audioObjects.Clear();
        _audioSource.Stop();
        DeleteTempPath();
	}

	public void SpeakBase64(AudioObject audioObject)
	{
		_startedSpeaking = false;
		_isPaused = false;
		StopSpeak();
		_audioObjects = new List<AudioObject> { audioObject };
		_currentSpeaking = StartCoroutine(SpeakBase64Enumerator());
	}

	public void SpeakBase64()
	{
		_startedSpeaking = false;
		_isPaused = false;
		StopSpeak();
		_currentSpeaking = StartCoroutine(SpeakBase64Enumerator());
	}

	public void AddVoiceSection(string b64Section)
	{
		_audioObjects.Add(new AudioObject { b64Audio = b64Section });
	}

	private string tempPath = "";
	private bool _startedSpeaking = false;
	private bool _isPaused = false;
	private IEnumerator SpeakBase64Enumerator()
	{
		int i = 0;
		while (_audioObjects.Count > i)
		{
			AudioClip clipToPlay = null;
			// Convert B64 to .Wav file
			if (_audioObjects[i].audioClip == null)
			{
				// TODO TTS Error: Calculated padded input size per channel: (4). Kernel size: (5). Kernel size can't be greater than actual input size
				if (_audioObjects[i].b64Audio == null)
				{
					i++;

					if (!(_audioObjects.Count > i))
						_startedSpeaking = true;
					continue;
				}

				var audioBytes = Convert.FromBase64String(_audioObjects[i].b64Audio);
				tempPath = Application.persistentDataPath + "tmpWAVBase64.wav";

				File.WriteAllBytes(tempPath, audioBytes);
				UnityWebRequest request = UnityWebRequestMultimedia.GetAudioClip(tempPath, AudioType.WAV);
				yield return request.SendWebRequest();

				if (request.result.Equals(UnityWebRequest.Result.ConnectionError))
					Debug.LogError(request.error);
				else
				{
					// Play file
					clipToPlay = DownloadHandlerAudioClip.GetContent(request);
				}
			}
			else
			{
				clipToPlay = _audioObjects[i].audioClip;
			}

			if (clipToPlay != null)
			{
				_audioSource.clip = clipToPlay;
				_audioSource.Play();
			}

			_startedSpeaking = true;

			// Wait until section is finished => start next one in next iteration
			while (_audioSource.isPlaying || _isPaused)
			{
				yield return null;
			}

			i++;
			DeleteTempPath();
		}
	}

	public void TogglePauseSpeech()
	{
		SetPauseSpeech(!_isPaused);
	}

	public void SetPauseSpeech(bool isPaused)
	{
		_isPaused = isPaused;
		if(_isPaused)
			_audioSource.Pause();
		else
			_audioSource.UnPause();

		Debug.Log(_isPaused + " " + _audioSource.isPlaying);

	}

	private void DeleteTempPath()
	{
		if(File.Exists(tempPath))
			{ File.Delete(tempPath); }
	}

	public List<Speaker> ToList() => new List<Speaker> { this };

	public bool IsSpeaking() => _audioSource.isPlaying || _isPaused;

	public bool StartedSpeaking() => _startedSpeaking;
}

public enum SpeakerAction
{
	Listening,
	StartWalkingToPodium,
	TurnAround,
	WalkAwayFromChair,
	WalkOutOfRow,
	WalkDownIsle,
	WalkToPodium,
	Speaking,
	WalktingFromPodium
}
