using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI : MonoBehaviour
{
    GameManager gameManager;
    private void Start()
    {
        gameManager = GameManager.instance;
    }
    public IEnumerator EnemyTurn()
    {
        Debug.Log("EnemyTurn");

        CardController[] enemyFieldCardList = gameManager.enemyFieldTransform.GetComponentsInChildren<CardController>();
        gameManager.SettingCanAttackView(enemyFieldCardList, true);

        yield return new WaitForSeconds(1);

        CardController[] handCardList = gameManager.enemyHandTransform.GetComponentsInChildren<CardController>();

        while (Array.Exists(handCardList, card =>(card.model.cost <= gameManager.enemy.manaCost)&& (!card.IsSpell || (card.IsSpell && card.CanUseSpell())) ))
        {
            CardController[] selectableHandCardList = Array.FindAll(handCardList, card =>(card.model.cost <= gameManager.enemy.manaCost)&& (!card.IsSpell || (card.IsSpell && card.CanUseSpell())));
            CardController selectCard = selectableHandCardList[0];
            if (selectCard.IsSpell)
            {
                CastSpellOf(selectCard);
            }
            else
            {
                StartCoroutine(selectCard.movement.MoveToField(gameManager.enemyFieldTransform));
                selectCard.OnFiled();
            }
            yield return new WaitForSeconds(1);
            handCardList = gameManager.enemyHandTransform.GetComponentsInChildren<CardController>();
        }

        yield return new WaitForSeconds(1);

        CardController[] fieldCardList = gameManager.enemyFieldTransform.GetComponentsInChildren<CardController>();

        while (Array.Exists(fieldCardList, card => card.model.canAttack))
        {
            CardController[] enemyCanAttackCardList = Array.FindAll(fieldCardList, card => card.model.canAttack);
            CardController[] playerFieldCardList = gameManager.playerFieldTransform.GetComponentsInChildren<CardController>();
            CardController attacker = enemyCanAttackCardList[0];

            if (playerFieldCardList.Length > 0)
            {
                if (Array.Exists(playerFieldCardList, card => card.model.ability == ABILITY.SHIELD))
                {
                    playerFieldCardList = Array.FindAll(playerFieldCardList, card => card.model.ability == ABILITY.SHIELD);
                }
                CardController defender = playerFieldCardList[0];
                StartCoroutine(attacker.movement.MoveToTarget(defender.transform));
                yield return new WaitForSeconds(0.51f);
                gameManager.CardsBattle(attacker, defender);
            }
            else
            {
                StartCoroutine(attacker.movement.MoveToTarget(gameManager.playerHero));
                yield return new WaitForSeconds(0.25f);
                gameManager.AttackToHero(attacker);
                yield return new WaitForSeconds(0.25f);
                gameManager.CheckHeroHP();
            }
            fieldCardList = gameManager.enemyFieldTransform.GetComponentsInChildren<CardController>();
            yield return new WaitForSeconds(1);
        }

        yield return new WaitForSeconds(1);

        gameManager.ChageTurn();
    }

    void CastSpellOf(CardController card)
    {
        CardController target = null;
        if (card.model.spell == SPELL.HEAL_FRIEND_CARD)
        {
            target = gameManager.GetFieldFieldCards(card.model.isPlayerCard)[0];
        }
        if (card.model.spell == SPELL.DAMAGE_ENEMY_CARD)
        {
            target = gameManager.GetEnemyFieldCards(card.model.isPlayerCard)[0];
        }
        card.UseSpellTo(target);
    }
}
