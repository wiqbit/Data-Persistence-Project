using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainManager : MonoBehaviour
{
    public Brick BrickPrefab;
    public int LineCount = 6;
    public Rigidbody Ball;

    public Text ScoreText;
    public GameObject GameOverText;
    
    private bool m_Started = false;
    private int m_Points;
    
    private bool m_GameOver = false;

    private static MainManager _instance = null;
    public Text _bestScoreAndName = null;
	private string _name = string.Empty;
    private int _bestScore = 0;
    private string _bestName = string.Empty;

	private void Awake()
	{
		if (_instance == null)
        {
            _instance = this;
            _instance.Name = CanvasController._nameText;
        }
	}

	void Start()
    {
		// Start is called before the first frame update
		const float step = 0.6f;
        int perLine = Mathf.FloorToInt(4.0f / step);
        
        int[] pointCountArray = new [] {1,1,2,2,5,5};
        for (int i = 0; i < LineCount; ++i)
        {
            for (int x = 0; x < perLine; ++x)
            {
                Vector3 position = new Vector3(-1.5f + step * x, 2.5f + i * 0.3f, 0);
                var brick = Instantiate(BrickPrefab, position, Quaternion.identity);
                brick.PointValue = pointCountArray[i];
                brick.onDestroyed.AddListener(AddPoint);
            }
        }

		string path = Application.persistentDataPath + "/savefile.json";

		if (File.Exists(path))
		{
			string json = File.ReadAllText(path);
			SaveData data = JsonUtility.FromJson<SaveData>(json);

			BestName = string.IsNullOrEmpty(data.BestName) ? Name : data.BestName;
            BestScore = data.BestScore;
		}

        UpdateBestScoreAndName();
	}

    private void Update()
    {
        if (!m_Started)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                m_Started = true;
                float randomDirection = Random.Range(-1.0f, 1.0f);
                Vector3 forceDir = new Vector3(randomDirection, 1, 0);
                forceDir.Normalize();

                Ball.transform.SetParent(null);
                Ball.AddForce(forceDir * 2.0f, ForceMode.VelocityChange);
            }
        }
        else if (m_GameOver)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
        }
    }

    void AddPoint(int point)
    {
        m_Points += point;
        ScoreText.text = $"Score : {m_Points}";

        if (m_Points > BestScore)
        {
            BestScore = m_Points;
            BestName = _name;

			SaveData data = new SaveData()
			{
				BestName = BestName,
				BestScore = BestScore
			};

			string json = JsonUtility.ToJson(data);

			File.WriteAllText(Application.persistentDataPath + "/savefile.json", json);
		}
    }

    public void GameOver()
    {
        m_GameOver = true;
        GameOverText.SetActive(true);
    }

    private void UpdateBestScoreAndName()
    {
		_bestScoreAndName.text = $"Best Score: {BestScore} Name: {BestName}";
	}

    public static MainManager Instance { get; }

    public string Name
    {
        get { return _name; }
        set
        {
            _name = value;
            UpdateBestScoreAndName();
        }
    }

    private int BestScore
    {
		get { return _bestScore; }
		set
		{
			_bestScore = value;
			UpdateBestScoreAndName();
		}
	}

    private string BestName
    {
        get { return _bestName; }
        set
        {
            _bestName = value;
            UpdateBestScoreAndName();
        }
    }

	[System.Serializable]
	class SaveData
	{
        public string BestName = string.Empty;
        public int BestScore = 0;
	}
}
