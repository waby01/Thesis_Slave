using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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

    [Header("Player UI Elements")]
    public TextMeshProUGUI playerMentalHealthText;
    public TextMeshProUGUI playerStaminaText;

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
        }

        deckManager.SetupDeck();
        ChangeState(BattleState.PlayerTurn);
    }

    private void StartPlayerTurn()
    {
        currentStamina = maxStamina;

        cardsInHand = deckManager.DrawCards(4);

        UpdateHandUI();

        Debug.Log($"Giliran Player! Stamina: {currentStamina}/3. Silakan mainkan kartumu, bre.");
    }

    public void PlayCard(CardData card)
    {
        if (currentState != BattleState.PlayerTurn)
        {
            Debug.LogWarning("Bukan giliranmu, bre! Sabar, dosen lagi koreksi.");
            return;
        }

        if (currentStamina < card.staminaCost)
        {
            Debug.LogWarning($"Stamina gak cukup buat mainin {card.cardName}! Butuh {card.staminaCost} Stamina.");
            return;
        }

        currentStamina -= card.staminaCost;
        Debug.Log($"Memainkan kartu: {card.cardName}. Stamina berkurang {card.staminaCost}. Sisa Stamina: {currentStamina}");

        if (card.cardName == "GANTI JUDUL")
        {
            Debug.Log("Efek GANTI JUDUL aktif! Membuang semua kartu dan menarik ulang...");
            cardsInHand = deckManager.DiscardHandAndRedraw();
        }
        else
        {
            switch (card.cardType.ToString())
            {
                case "Damage":
                    currentEnemyRevisiBar -= card.effectValue;
                    if (currentEnemyRevisiBar < 0) currentEnemyRevisiBar = 0;
                    Debug.Log($"Coretan Skripsi! Revisi Bar {currentEnemyData.enemyName} berkurang {card.effectValue}. Sisa: {currentEnemyRevisiBar}");
                    break;

                case "Heal":
                    currentMentalHealth += card.effectValue;
                    if (currentMentalHealth > maxMentalHealth) currentMentalHealth = maxMentalHealth;
                    Debug.Log($"Healing! Mental Health bertambah {card.effectValue}. Sekarang: {currentMentalHealth}");
                    break;

                case "Draw":
                    List<CardData> extraCards = deckManager.DrawCards(card.effectValue);
                    cardsInHand.AddRange(extraCards);
                    Debug.Log($"Efek Draw! Menarik {card.effectValue} kartu tambahan ke tangan.");
                    break;

                case "Stamina Buff":
                    currentStamina += card.effectValue;
                    Debug.Log($"Efek Stamina! Stamina bertambah {card.effectValue}. Sekarang: {currentStamina}");
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

        if (currentEnemyData != null && enemyRevisiText != null)
        {
            if (enemyNameText != null) enemyNameText.text = currentEnemyData.enemyName;
            enemyRevisiText.text = $"Revisi Bar: {currentEnemyRevisiBar} / {currentEnemyData.maxRevisiBar}";
        }

        if (playerMentalHealthText != null)
        {
            playerMentalHealthText.text = $"Mental Health: {currentMentalHealth} / {maxMentalHealth}";
        }

        if (playerStaminaText != null)
        {
            playerStaminaText.text = $"Stamina: {currentStamina} / {maxStamina}";
        }
    }

    private IEnumerator ExecuteEnemyTurn()
    {
        Debug.Log("Dosen/Draf Skripsi sedang memeriksa...");
        yield return new WaitForSeconds(1.5f);

        if (currentEnemyData != null && currentEnemyRevisiBar > 0)
        {
            currentMentalHealth -= currentEnemyData.baseDamage;
            Debug.Log($"{currentEnemyData.enemyName} memberikan coretan! Mental Health berkurang {currentEnemyData.baseDamage}.");
        }

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