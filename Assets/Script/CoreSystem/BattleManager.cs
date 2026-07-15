using System.Collections;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    public enum BattleState { Start, PlayerTurn, EnemyTurn, Win, Lose }
    public BattleState currentState;

    [Header("Linked Managers")]
    public DeckManager deckManager;

    [Header("Player Stats")]
    public int maxMentalHealth = 100;
    public int currentMentalHealth;
    public int maxStamina = 3;
    public int currentStamina;

    [Header("Active Enemy")]
    public EnemyData currentEnemyData;
    public int currentEnemyRevisiBar;

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

    // 1. Persiapan Awal Pertarungan
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

        deckManager.DrawCards(4);

        Debug.Log("Giliran Player Dimulai! Silakan mainkan kartumu, bre.");
    }

    private IEnumerator ExecuteEnemyTurn()
    {
        Debug.Log("Dosen/Draf Skripsi sedang memeriksa...");
        yield return new WaitForSeconds(1.5f);

        if (currentEnemyData != null)
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