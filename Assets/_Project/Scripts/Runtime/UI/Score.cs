using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace PuzzleBobble
{
    public class Score : MonoBehaviour
    {
        [SerializeField] CanvasGroup winCanvas;
        [SerializeField] Button quitButton, retryButton;
        [SerializeField] TMPro.TextMeshProUGUI winscoreText;
        [SerializeField] TMPro.TextMeshProUGUI scoreText;

        Enemy[] enemies;
        int enemiesLeft = 0;
        int score = 0;
        private void Start()
        {
            quitButton.onClick.AddListener(Quit);
            retryButton.onClick.AddListener(Retry);

            enemies = GameObject.FindObjectsOfType<Enemy>();
            foreach (var enemy in enemies)
            {
                enemy.OnDie += AddScore;
                enemy.OnLoose += Loose;
            }
            scoreText.text = $"{score}/{enemies.Length}";
            enemiesLeft = enemies.Length;

            winCanvas.alpha = 0;
            winCanvas.blocksRaycasts = false;
            winCanvas.interactable = false;
        }

        private void Retry()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        private void Quit()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
	Application.Quit();
#endif
        }

        private void Loose()
        {
            enemiesLeft--;
            if (enemiesLeft <= 0) CheckWin();
        }

        private void AddScore()
        {
            enemiesLeft--;
            score++;
            scoreText.text = $"{score}/{enemies.Length}";
            if (enemiesLeft <= 0) CheckWin();
        }

        void CheckWin()
        {
            PlayerController.Instance.gameObject.SetActive(false);
            winscoreText.text = scoreText.text;
            winCanvas.DOFade(1, .6f);
            winCanvas.blocksRaycasts = true;
            winCanvas.interactable = true;
        }
    }
}
