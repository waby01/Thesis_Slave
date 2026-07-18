using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BattleManager : MonoBehaviour
{
    public enum BattleState { Start, PlayerTurn, EnemyTurn, Win, Lose }
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

    private List<CardData> cardsInHand = new List<CardData>();

    private void Start()
    {
        ChangeState(BattleState.Start);
    }

    public void ChangeState(BattleState newState)
    {
        currentState = newState;

        switch (currentState)
        {
            case BattleState.Start:
                SetupBattle();
                break;
            case BattleState.PlayerTurn:
                StartPlayerTurn();
                break;
            case BattleState.EnemyTurn:
                StartCoroutine(ExecuteEnemyTurn());
                break;
            case BattleState.Win:
                Debug.Log("Selamat! Bab Skripsi Berhasil Di-ACC!");
                break;
            case BattleState.Lose:
                Debug.Log("Waduh! Mental Health Drop, Kena Mental!");
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

    private void StartPlayerTurn()
    {
        currentStamina = maxStamina;
        cardsInHand = deckManager.DrawCards(4);

        UpdateHandUI();
        DisplayEnemyIntent();

        Debug.Log($"Giliran Player! Stamina: {currentStamina}/3. Silakan mainkan kartumu, bre.");
    }

    public void PlayCard(CardData card)
    {
        if (currentState != BattleState.PlayerTurn) return;

        if (currentStamina < card.staminaCost)
        {
            Debug.LogWarning($"Stamina gak cukup!");
            return;
        }

        currentStamina -= card.staminaCost;

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

            deckManager.DiscardCard(card);
            cardsInHand.Remove(card);
        }

        UpdateHandUI();

        if (currentEnemyRevisiBar <= 0)
        {
            ChangeState(BattleState.Win);
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
            enemyIntentText.text = $"Intent: ⚔️ {currentEnemyData.baseDamage} Dmg";
        }
    }

    private IEnumerator ExecuteEnemyTurn()
    {
        if (enemyIntentText != null) enemyIntentText.text = "Checking...";
        Debug.Log("Dosen/Draf Skripsi sedang memeriksa...");
        yield return new WaitForSeconds(1.5f);

        if (currentEnemyData != null && currentEnemyRevisiBar > 0)
        {
            currentMentalHealth -= currentEnemyData.baseDamage;
            Debug.Log($"{currentEnemyData.enemyName} memberikan coretan!");
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

            ChangeState(BattleState.PlayerTurn);
        }
    }

    public void OnEndTurnButtonPressed()
    {
        if (currentState == BattleState.PlayerTurn)
        {
            ChangeState(BattleState.EnemyTurn);
        }
    }
}