using Fusion;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    [Header("Textos de puntaje")]
    public TextMeshProUGUI hostText;
    public TextMeshProUGUI clientText;

    [Header("Panel de victoria")]
    public GameObject winPanel;
    public TextMeshProUGUI winText;
    public Button restartButton;

    private PlayerScore hostScore;
    private PlayerScore clientScore;

    void Start()
    {
        if (winPanel != null)
            winPanel.SetActive(false);

        if (restartButton != null)
            restartButton.onClick.AddListener(RestartGame);
    }

    void Update()
    {
        // Si aún no tenemos referencias, las buscamos
        if (hostScore == null || clientScore == null)
        {
            FindPlayers();
        }

        // Actualizar textos de marcador
        if (hostText != null)
            hostText.text = "Host: " + (hostScore != null ? hostScore.Score.ToString() : "0");

        if (clientText != null)
            clientText.text = "Client: " + (clientScore != null ? clientScore.Score.ToString() : "0");
    }

    void FindPlayers()
    {
        var players = FindObjectsOfType<PlayerScore>();

        foreach (var p in players)
        {
            if (p.IsHost)
                hostScore = p;
            else
                clientScore = p;
        }
    }

    public void ShowWinner(bool hostWins)
    {
        if (winPanel != null)
            winPanel.SetActive(true);

        if (winText != null)
            winText.text = hostWins ? "Jugador Host ha ganado" : "Jugador Client ha ganado";

        // 🔴 Aquí arreglamos el MOUSE para el panel final
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        Time.timeScale = 0f;
    }

    void RestartGame()
    {
        Time.timeScale = 1f;

        var runner = FindObjectOfType<NetworkRunner>();
        if (runner != null)
        {
            runner.LoadScene("Game");
        }
        else
        {
            SceneManager.LoadScene("Game");
        }
    }
}
