using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] Transform playerHandTransform,
                               playerFieldTransform,
                               enemyHandTransform,
                               enemyFieldTransform;
    [SerializeField] CardController cardPrefab;

    bool isPlayerTurn;

    public static GameManager instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    void Start()
    {
        StartGame();
    }

    void StartGame()
    {
        SettingInitHand();
        isPlayerTurn = true;
        TurnCalc();
    }

    void SettingInitHand()
    {
        for (int i = 0; i < 3; i++)
        {
            CreateCard(playerHandTransform);
            CreateCard(enemyHandTransform);
        }
    }

    void TurnCalc()
    {
        if (isPlayerTurn)
        {
            PlayerTurn();
        }
        else
        {
            EnemyTurn();
        }
    }

    public void ChageTurn()
    {
        isPlayerTurn = !isPlayerTurn;
        if (isPlayerTurn)
        {
            CreateCard(playerHandTransform);
        }
        else
        {
            CreateCard(enemyHandTransform);
        }
        TurnCalc();
    }

    void PlayerTurn()
    {
        Debug.Log("PlayerTurn");
        CardController[] playerFieldCardList = playerFieldTransform.GetComponentsInChildren<CardController>();
        foreach (CardController card in playerFieldCardList)
        {
            card.SetCanAttack(true);
        }
    }

    void EnemyTurn()
    {
        Debug.Log("EnemyTurn");
        CardController[] handCardList = enemyHandTransform.GetComponentsInChildren<CardController>();
        CardController enemyCard = handCardList[0];
        enemyCard.movement.SetCardTransform(enemyFieldTransform);

        CardController[] fieldCardList = enemyFieldTransform.GetComponentsInChildren<CardController>();
        CardController[] enemyCanAttackCardList = Array.FindAll(fieldCardList, card => card.model.canAttack);
        CardController[] playerFieldCardList = playerFieldTransform.GetComponentsInChildren<CardController>();
        if (enemyCanAttackCardList.Length > 0 && playerFieldCardList.Length > 0)
        {
            CardController attacker = enemyCanAttackCardList[0];
            CardController defender = playerFieldCardList[0];
            CardsBattle(attacker, defender);
        }
        ChageTurn();
    }

    public void CardsBattle(CardController attacker, CardController defender)
    {
        Debug.Log("attacker HP" + attacker.model.hp);
        Debug.Log("defender HP" + defender.model.hp);
        attacker.Attack(defender);
        defender.Attack(attacker);
        Debug.Log("attacker HP" + attacker.model.hp);
        Debug.Log("defender HP" + defender.model.hp);
        attacker.CheckAlive();
        defender.CheckAlive();
    }

    void CreateCard(Transform hand)
    {
        CardController card = Instantiate(cardPrefab, hand, false);
        card.Init(1);
    }
}
