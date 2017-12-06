using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartGame : MonoBehaviour {
    
    Text bestScore;
    [SerializeField]
    GameObject mainMenu;
    [SerializeField]
    GameObject loadingCanvas;

    private void Awake()
    {
        bestScore = mainMenu.GetComponentInChildren<Text>();
    }

    private void Start()
    {
        bestScore.text = "Best score : " + PlayerPrefs.GetInt("BestScore").ToString();
    }

    public void StartGameScene()
    {
        mainMenu.SetActive(false);
        loadingCanvas.SetActive(true);

        SceneManager.LoadSceneAsync(1);
    }}
