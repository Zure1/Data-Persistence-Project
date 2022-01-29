using System;
using System.Collections;
using System.Collections.Generic;
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
    public Text HighScoreAndNameText;
    public GameObject GameOverText;
    
    private bool m_Started = false;
    private int m_Points;
    private int m_Highscore;
    
    private bool m_GameOver = false;

    // Start is called before the first frame update
    void Start()
    {
        if (PersistenceManager.Instance != null)
        {
            LoadHighscore();
            HighScoreAndNameText.text = $"High Score : {m_Highscore}, Name : {PersistenceManager.Instance.PlayerName}";
        }

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
    }

    private void Update()
    {
        if (!m_Started)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                m_Started = true;
                float randomDirection = UnityEngine.Random.Range(-1.0f, 1.0f);
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
    }

    public void GameOver()
    {
        m_GameOver = true;
        GameOverText.SetActive(true);

        if (m_Points > m_Highscore)
        {
            m_Highscore = m_Points;
            SaveHighscore();

            HighScoreAndNameText.text = $"High Score : {m_Highscore}, Name : {PersistenceManager.Instance.PlayerName}";
        }
    }

    [Serializable]
    class SaveData
    {
        public static string Path = Application.persistentDataPath + "/savefile.json";
        public int Highscore;
    }

    public void SaveHighscore()
    {
        var data = new SaveData();
        data.Highscore = m_Highscore;

        var json = JsonUtility.ToJson(data);

        File.WriteAllText(SaveData.Path, json);
    }

    public void LoadHighscore()
    {
        var path = SaveData.Path;
        if (!File.Exists(path))
        {
            return;
        }

        string json = File.ReadAllText(path);
        var data = JsonUtility.FromJson<SaveData>(json);

        m_Highscore = data.Highscore;
    }
}
