using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class BattleManager : MonoBehaviour
{
    public enum BattleState { Start, PlayerTurn, ExecutingPlayerActions, EnemyTurn, Win, Lose }
    public BattleState currentState;

    [Header("Linked Managers")]
    public DeckManager deckManager;
    public SlotCard slotCardVisual;

    [Header("Player Stats")]
    public int maxMentalHealth = 100;
    public int currentMentalHealth;
    public int maxStamina = 3;
    public int currentStamina;

    [Header("Active Enemy")]
    public EnemyData currentEnemyData;
    public int currentEnemyRevisiBar;

    [Header("Enemy UI Elements")]
    public TextMeshProUGUI enemyNameText;
    public TextMeshProUGUI enemyRevisiText;
    public Image enemyArtImage;
    public TextMeshProUGUI enemyIntentText;
    public Slider enemyRevisiSlider;

    [Header("Player UI Elements")]
    public TextMeshProUGUI playerMentalHealthText;
    public TextMeshProUGUI playerStaminaText;
    public Slider playerMentalSlider;
    public Button endTurnButton;

    [Header("Game Flow UI Panels")]
    public GameObject winPanel;
    public GameObject losePanel;
    public GameObject pausePanel;

    [Header("Turn Indicator Panels")]
    public GameObject playerTurnPanel;
    public GameObject enemyTurnPanel;

    private List<CardData> cardsInHand = new List<CardData>();
    private List<CardData> queuedCards = new List<CardData>();
    private bool isPaused = false;
    private bool isPlayerTurnActive = false;

    private void Start()
    {
        Time.timeScale = 1f;

        if (winPanel != null) winPanel.SetActive(false);
        if (losePanel != null) losePanel.SetActive(false);
        if (pausePanel != null) pausePanel.SetActive(false);
        if (playerTurnPanel != null) playerTurnPanel.SetActive(false);
        if (enemyTurnPanel != null) enemyTurnPanel.SetActive(false);

        UpdateEndTurnButtonState();
        ChangeState(BattleState.Start);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.P))
        {
            if (currentState == BattleState.PlayerTurn || currentState == BattleState.ExecutingPlayerActions)
            {
                if (isPaused) ResumeGame();
                else PauseGame();
            }
        }
    }

    public void ChangeState(BattleState newState)
    {
        currentState = newState;
        UpdateEndTurnButtonState();

        switch (currentState)
        {
            case BattleState.Start:
                SetupBattle();
                break;
            case BattleState.PlayerTurn:
                StartCoroutine(StartPlayerTurnRoutine());
                break;
            case BattleState.ExecutingPlayerActions:
                StartCoroutine(ExecutePlayerTurnActions());
                break;
            case BattleState.EnemyTurn:
                StartCoroutine(ExecuteEnemyTurn());
                break;
            case BattleState.Win:
                Debug.Log("Selamat! Bab Skripsi Berhasil Di-ACC!");
                if (winPanel != null) winPanel.SetActive(true);

                if (AudioManagers.Instance != null)
                {
                    AudioManagers.Instance.SetPanelMuteOverride(true);
                    AudioManagers.Instance.PlaySFX(AudioManagers.Instance.winSFX);
                }
                UpdateEndTurnButtonState();
                break;
            case BattleState.Lose:
                Debug.Log("Waduh! Mental Health Drop, Kena Mental!");
                if (losePanel != null) losePanel.SetActive(true);

                if (AudioManagers.Instance != null)
                {
                    AudioManagers.Instance.SetPanelMuteOverride(true);
                    AudioManagers.Instance.PlaySFX(AudioManagers.Instance.loseSFX);
                }
                UpdateEndTurnButtonState();
                break;
        }
    }

    private void SetupBattle()
    {
        currentMentalHealth = maxMentalHealth;
        currentStamina = maxStamina;

        if (currentEnemyData != null)
        {
            currentEnemyRevisiBar = currentEnemyData.maxRevisiBar;

            if (enemyRevisiSlider != null) enemyRevisiSlider.maxValue = currentEnemyData.maxRevisiBar;
            if (playerMentalSlider != null) playerMentalSlider.maxValue = maxMentalHealth;

            if (enemyArtImage != null) enemyArtImage.sprite = currentEnemyData.enemySprite;
        }

        deckManager.SetupDeck();
        ChangeState(BattleState.PlayerTurn);
    }

    private IEnumerator StartPlayerTurnRoutine()
    {
        isPlayerTurnActive = false;
        UpdateEndTurnButtonState();

        currentStamina = maxStamina;
        queuedCards.Clear();
        cardsInHand = deckManager.DrawCards(4);

        UpdateHandUI();
        DisplayEnemyIntent();

        if (playerTurnPanel != null)
        {
            playerTurnPanel.SetActive(true);
            yield return new WaitForSeconds(1.5f);
            playerTurnPanel.SetActive(false);
        }

        isPlayerTurnActive = true;
        UpdateEndTurnButtonState();
        Debug.Log($"Giliran Player! Stamina: {currentStamina}/3.");
    }

    public void PlayCard(CardData card)
    {
        if (isPaused || currentState != BattleState.PlayerTurn || !isPlayerTurnActive || IsAnyPanelActive()) return;

        if (currentStamina < card.staminaCost)
        {
            Debug.LogWarning($"Stamina gak cukup!");
            return;
        }

        currentStamina -= card.staminaCost;
        queuedCards.Add(card);

        if (AudioManagers.Instance != null) AudioManagers.Instance.PlaySFX(AudioManagers.Instance.playCardSFX);

        deckManager.DiscardCard(card);
        cardsInHand.Remove(card);

        UpdateHandUI();
    }

    public void OnEndTurnButtonPressed()
    {
        if (CanClickEndTurn())
        {
            isPlayerTurnActive = false;
            UpdateEndTurnButtonState();

            if (AudioManagers.Instance != null) AudioManagers.Instance.PlaySFX(AudioManagers.Instance.buttonClickSFX);
            ChangeState(BattleState.ExecutingPlayerActions);
        }
    }

    private IEnumerator ExecutePlayerTurnActions()
    {
        UpdateEndTurnButtonState();
        Debug.Log("Mengeksekusi serangan player...");

        foreach (CardData card in queuedCards)
        {
            if (card.cardName == "GANTI JUDUL")
            {
                cardsInHand = deckManager.DiscardHandAndRedraw();
            }
            else
            {
                switch (card.cardType.ToString())
                {
                    case "Damage":
                        currentEnemyRevisiBar -= card.effectValue;
                        if (currentEnemyRevisiBar < 0) currentEnemyRevisiBar = 0;
                        break;

                    case "Heal":
                        currentMentalHealth += card.effectValue;
                        if (currentMentalHealth > maxMentalHealth) currentMentalHealth = maxMentalHealth;
                        break;

                    case "Draw":
                        List<CardData> extraCards = deckManager.DrawCards(card.effectValue);
                        cardsInHand.AddRange(extraCards);
                        break;

                    case "Stamina Buff":
                        currentStamina += card.effectValue;
                        break;
                }
            }

            UpdateHandUI();

            if (currentEnemyRevisiBar <= 0)
            {
                ChangeState(BattleState.Win);
                yield break;
            }

            yield return new WaitForSeconds(1.0f);
        }

        ChangeState(BattleState.EnemyTurn);
    }

    private IEnumerator ExecuteEnemyTurn()
    {
        UpdateEndTurnButtonState();

        if (enemyTurnPanel != null)
        {
            enemyTurnPanel.SetActive(true);
            yield return new WaitForSeconds(1.5f);
            enemyTurnPanel.SetActive(false);
        }

        if (enemyIntentText != null) enemyIntentText.text = "Checking...";
        Debug.Log("Dosen/Draf Skripsi sedang memeriksa...");
        yield return new WaitForSeconds(1.0f);

        if (currentEnemyData != null && currentEnemyRevisiBar > 0)
        {
            currentMentalHealth -= currentEnemyData.baseDamage;
            Debug.Log($"{currentEnemyData.enemyName} memberikan coretan!");

            if (AudioManagers.Instance != null) AudioManagers.Instance.PlaySFX(AudioManagers.Instance.enemyAttackSFX);
        }

        UpdateHandUI();

        if (currentMentalHealth <= 0)
        {
            ChangeState(BattleState.Lose);
        }
        else
        {
            deckManager.DiscardHand();
            cardsInHand.Clear();
            UpdateHandUI();

            yield return new WaitForSeconds(2.0f);

            ChangeState(BattleState.PlayerTurn);
        }
    }

    private bool IsAnyPanelActive()
    {
        bool isWinActive = winPanel != null && winPanel.activeSelf;
        bool isLoseActive = losePanel != null && losePanel.activeSelf;
        bool isPauseActive = pausePanel != null && pausePanel.activeSelf;
        bool isPlayerTurnPanelActive = playerTurnPanel != null && playerTurnPanel.activeSelf;
        bool isEnemyTurnPanelActive = enemyTurnPanel != null && enemyTurnPanel.activeSelf;

        return isWinActive || isLoseActive || isPauseActive || isPlayerTurnPanelActive || isEnemyTurnPanelActive;
    }

    private bool CanClickEndTurn()
    {
        return !isPaused && currentState == BattleState.PlayerTurn && isPlayerTurnActive && !IsAnyPanelActive();
    }

    private void UpdateEndTurnButtonState()
    {
        if (endTurnButton != null)
        {
            endTurnButton.interactable = CanClickEndTurn();
        }
    }

    private void UpdateHandUI()
    {
        if (slotCardVisual != null)
        {
            slotCardVisual.RefreshHandVisuals(cardsInHand);
        }

        if (currentEnemyData != null)
        {
            if (enemyNameText != null) enemyNameText.text = currentEnemyData.enemyName;
            if (enemyRevisiText != null) enemyRevisiText.text = $"Revisi Bar: {currentEnemyRevisiBar} / {currentEnemyData.maxRevisiBar}";
            if (enemyRevisiSlider != null) enemyRevisiSlider.value = currentEnemyBarValue();
        }

        if (playerMentalHealthText != null) playerMentalHealthText.text = $"Mental Health: {currentMentalHealth} / {maxMentalHealth}";
        if (playerMentalSlider != null) playerMentalSlider.value = currentMentalHealth;
        if (playerStaminaText != null) playerStaminaText.text = $"Stamina: {currentStamina} / {maxStamina}";
    }

    private int currentEnemyBarValue() => currentEnemyRevisiBar < 0 ? 0 : currentEnemyRevisiBar;

    private void DisplayEnemyIntent()
    {
        if (currentEnemyData != null && enemyIntentText != null)
        {
            enemyIntentText.text = $"Intent: {currentEnemyData.baseDamage} Dmg";
        }
    }

    public void PauseGame()
    {
        isPaused = true;
        Time.timeScale = 0f;
        if (pausePanel != null) pausePanel.SetActive(true);

        UpdateEndTurnButtonState();

        if (AudioManagers.Instance != null)
        {
            AudioManagers.Instance.SetPanelMuteOverride(true);
        }
    }

    public void ResumeGame()
    {
        isPaused = false;
        if (pausePanel != null) pausePanel.SetActive(false);
        Time.timeScale = 1f;

        UpdateEndTurnButtonState();

        if (AudioManagers.Instance != null)
        {
            AudioManagers.Instance.SetPanelMuteOverride(false);
            AudioManagers.Instance.PlaySFX(AudioManagers.Instance.buttonClickSFX);
        }
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        if (AudioManagers.Instance != null) AudioManagers.Instance.PlaySFX(AudioManagers.Instance.buttonClickSFX);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void GoToMainMenu()
    {
        Time.timeScale = 1f;
        if (AudioManagers.Instance != null) AudioManagers.Instance.PlaySFX(AudioManagers.Instance.buttonClickSFX);
        SceneManager.LoadScene("MainMenu");
    }
}