using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour {

    [SerializeField]
    GameObject gameUI;
    [SerializeField]
    GameObject gameEndUI;

    [SerializeField]
    GameObject currentScore;
    [SerializeField]
    GameObject newBestScore;

    [SerializeField]
    Text monsterDistanceText;
    [SerializeField]
    Text scoreText;

    Transform characterPosition;
    Transform monsterPosition;

    [SerializeField]
    Image[] lives;

    private void Start()
    {
        characterPosition = GameObject.FindGameObjectWithTag("Character").transform;
        monsterPosition = GameObject.FindGameObjectWithTag("Monster").transform;
    }

    public void EndGame()
    {
        gameUI.SetActive(false);
        gameEndUI.SetActive(true);

        currentScore.GetComponent<Text>().text = PlayerPrefs.GetInt("CurrentScore").ToString();

        if (PlayerPrefs.GetInt("CurrentScore") > PlayerPrefs.GetInt("BestScore"))
        {
            PlayerPrefs.SetInt("BestScore", PlayerPrefs.GetInt("CurrentScore"));
            newBestScore.SetActive(true);
        }
        else
            newBestScore.SetActive(false);
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(1);
    }

    public void BackToMainMenu()
    {
        SceneManager.LoadScene(0);
    }

    public void SetScore(int score)
    {
        scoreText.text = score.ToString();
    }

    public void SetMonsterDistance()
    {
        monsterDistanceText.text = ((int)(characterPosition.position.x - monsterPosition.position.x)).ToString();
    }

    public void OnLivesChange(int value)
    {
        if (value < 0)
        {
            for (int i = 4; i >= 0; i--)
            {
                if (lives[i].color.a != 0)
                {
                    for (int j = i; j >= 0 && value < 0; j--, value++)
                    {
                        lives[i].color = new Color(255.0f, 255.0f, 255.0f, 0.0f);
                        Instantiate(Resources.Load("LivesLost"), lives[i].transform);
                    }
            }
            }
        }
    }
}
