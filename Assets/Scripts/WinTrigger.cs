using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class WinTrigger : MonoBehaviour
{
    public Material greenMaterial;
    public Material redMaterial;
    public TMP_Text winText;
    public TMP_Text countdownText;
    public Button resetButton;
    public float stationaryTimeRequired = 5f;

    private Renderer cubeRenderer;
    private bool playerInside = false;
    private float stationaryTimer = 0f;
    private bool countingDown = false;
    private bool hasWon = false;
    public Rigidbody playerRb;

    void Start()
    {
        cubeRenderer = GetComponent<Renderer>();
        cubeRenderer.material = redMaterial;

        winText.gameObject.SetActive(false);
        countdownText.gameObject.SetActive(false);
        resetButton.gameObject.SetActive(false);

        resetButton.onClick.AddListener(ResetGame);
    }

    void Update()
    {
        if (playerInside && !hasWon)
        {
            Vector3 horizontalVelocity = new Vector3(playerRb.velocity.x, 0f, playerRb.velocity.z);

            if (horizontalVelocity.magnitude < 0.6f)
            {
                if (!countingDown)
                {
                    countingDown = true;
                    countdownText.gameObject.SetActive(true);
                }

                stationaryTimer += Time.deltaTime;

                int remainingTime = Mathf.CeilToInt(stationaryTimeRequired - stationaryTimer);
                countdownText.text = remainingTime.ToString();

                float progress = Mathf.Clamp01(stationaryTimer / stationaryTimeRequired);
                cubeRenderer.material.color = Color.Lerp(redMaterial.color, greenMaterial.color, progress);

                if (stationaryTimer >= stationaryTimeRequired)
                {
                    WinGame();
                }
            }
            else
            {
                ResetCountdown();
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !hasWon)
        {
            playerInside = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            ResetCountdown();
            playerInside = false;
        }
    }

    void ResetCountdown()
    {
        countingDown = false;
        stationaryTimer = 0f;
        cubeRenderer.material = redMaterial;
        countdownText.gameObject.SetActive(false);
    }

    void WinGame()
    {
        hasWon = true;
        cubeRenderer.material = greenMaterial;
        countdownText.gameObject.SetActive(false);
        winText.gameObject.SetActive(true);
        resetButton.gameObject.SetActive(true);
        Time.timeScale = 0f;
    }

    void ResetGame()
    {
        Time.timeScale = 1f;
        cubeRenderer.material = redMaterial;
        winText.gameObject.SetActive(false);
        resetButton.gameObject.SetActive(false);
        countdownText.gameObject.SetActive(false);
        stationaryTimer = 0f;
        countingDown = false;
        hasWon = false;
        playerInside = false;

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
