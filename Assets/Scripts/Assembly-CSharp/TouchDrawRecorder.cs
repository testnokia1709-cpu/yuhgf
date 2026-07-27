using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using UnityEngine;

public class TouchDrawRecorder : MonoBehaviour
{
	public static TouchDrawRecorder Instance;

	public bool AutoPlayback;

	public bool Loop;

	public TouchDrawPhysics DrawPhysics;

	private List<TimedPoint> m_points;

	private bool m_recording;

	private float m_recordStartTime;

	private bool m_playback;

	private float m_playbackStartTime;

	private int m_playbackIndex;

	private bool m_dataLoaded;

	private int m_index = 1;

	private Vector2 m_previousPoint;

	private static int PlaybackCount = 6;

	private static readonly string FILENAME = "TouchDrawRecording";

	public bool IsRecording
	{
		get
		{
			return m_recording;
		}
	}

	public List<TimedPoint> Points
	{
		get
		{
			return m_points;
		}
	}

	private void Awake()
	{
		if (Instance != null)
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}
		else
		{
			Instance = this;
		}
	}

	public void Start()
	{
		TouchDrawPhysics drawPhysics = DrawPhysics;
		drawPhysics.OnRecordPoint = (Action<Vector2, PointType>)Delegate.Combine(drawPhysics.OnRecordPoint, new Action<Vector2, PointType>(RecordPoint));
	}

	public void StartRecording()
	{
		m_recording = true;
		m_playback = false;
		m_recordStartTime = Time.time;
		m_points = new List<TimedPoint>();
	}

	public void StopRecording(bool addEndTimePoint = true)
	{
		if (m_recording)
		{
			if (addEndTimePoint)
			{
				RecordPoint(m_previousPoint, PointType.End);
			}
			m_recording = false;
		}
	}

	public void RecordPoint(Vector2 point, PointType type)
	{
		if (m_recording)
		{
			if (type == PointType.End)
			{
				point = m_previousPoint;
			}
			m_points.Add(new TimedPoint
			{
				Point = point,
				PointTime = Time.time - m_recordStartTime,
				PointType = type
			});
			m_previousPoint = point;
		}
	}

	public void Playback()
	{
		m_recording = false;
		m_playback = true;
		m_playbackStartTime = Time.time;
		m_playbackIndex = 0;
		m_index = UnityEngine.Random.Range(1, PlaybackCount);
	}

	public void Stop()
	{
		m_playback = false;
	}

	public void Update()
	{
		if (!m_dataLoaded)
		{
			Load();
			m_dataLoaded = true;
		}
		if (AutoPlayback && !m_playback)
		{
			Playback();
			AutoPlayback = false;
		}
		if (!m_playback)
		{
			return;
		}
		if (m_points != null && m_points.Count > 0 && m_playbackIndex < m_points.Count)
		{
			float num = Time.time - m_playbackStartTime;
			TimedPoint timedPoint = m_points[m_playbackIndex];
			if (num > timedPoint.PointTime)
			{
				switch (timedPoint.PointType)
				{
				case PointType.Begin:
					DrawPhysics.StartDrawObject(timedPoint.Point);
					break;
				case PointType.Middle:
					DrawPhysics.DrawObject(timedPoint.Point);
					break;
				case PointType.End:
					DrawPhysics.EndDrawObject(timedPoint.Point);
					break;
				}
				m_playbackIndex++;
			}
		}
		else
		{
			DrawPhysics.ClearShapes();
			if (Loop)
			{
				Load();
				m_playbackStartTime = Time.time;
				m_playbackIndex = 0;
			}
			else
			{
				m_playback = false;
			}
		}
	}

	public void Save()
	{
		try
		{
			string path = Application.dataPath + "/Resources/" + FILENAME + ".txt";
			XmlSerializer xmlSerializer = new XmlSerializer(typeof(List<TimedPoint>));
			FileStream fileStream = new FileStream(path, FileMode.Create);
			xmlSerializer.Serialize(fileStream, m_points);
			fileStream.Close();
		}
		catch (Exception exception)
		{
			Debug.LogException(exception);
		}
	}

	public void Load()
	{
		m_points = null;
		try
		{
			TextAsset textAsset = Resources.Load(FILENAME + m_index) as TextAsset;
			StringReader stringReader = new StringReader(textAsset.text);
			XmlSerializer xmlSerializer = new XmlSerializer(typeof(List<TimedPoint>));
			m_points = (List<TimedPoint>)xmlSerializer.Deserialize(stringReader);
			stringReader.Close();
		}
		catch (Exception ex)
		{
			Debug.LogError("Failed to load touch recording: " + ex.Message);
		}
		m_index++;
		if (m_index > PlaybackCount)
		{
			m_index = 1;
		}
	}
}
