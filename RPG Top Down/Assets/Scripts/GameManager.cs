using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GameManager : MonoBehaviour
{
    public Int32 CalculateHealth(Entity entity)
    {
        // Formula para calcular a vida máxima do jogador com base no nível e na resistência
        // Formula: (Resistência * 10) + (Level * 4) + 10
        Int32 result = (entity.resistence * 10) + (entity.level * 4) + 10;
        Debug.LogFormat("CalculateHealth: {0}", result);
        return result;
    }

    public Int32 CalculateMana(Entity entity)
    {
        // Formula para calcular a mana máxima do jogador com base no nível e na inteligência
        // Formula: (Inteligência * 10) + (Level * 4) + 5
        Int32 result = (entity.intelligence * 10) + (entity.level * 4) + 5;
        Debug.LogFormat("CalculateMana: {0}", result);
        return result;
    }

    public Int32 CalculateStamina(Entity entity)
    {
        // Formula para calcular a stamina máxima do jogador com base no nível e na resistência
        // Formula: (Resistência * 10) + (Level * 4) + 5
        Int32 result = (entity.resistence * entity.willPower) + (entity.level * 2) + 5;
        Debug.LogFormat("CalculateStamina: {0}", result);
        return result;
    }

    public Int32 CalculateDamage(Entity entity, int weaponDamage)
    {
        // Formula para calcular o dano máximo do jogador com base no nível, na força e na arma
        // Formula: (Força * 2) + (Weapon* 2) + (Level * 3) + random (1, 20)
        System.Random random = new System.Random();
        Int32 result = (entity.strenght * 2) + (weaponDamage * 2) + (entity.level * 3) + random.Next(1,20);
        Debug.LogFormat("CalculateDamage: {0}", result);
        return result;
    }

    public Int32 CalculateDefense(Entity entity, int armorDefense)
    {
        // Formula para calcular a defesa máxima do jogador com base no nível e na armadura
        // Formula: (Defesa * 2) + (Level * 3) + armorDefense
        Int32 result = (entity.resistence * 2) + (entity.level * 3) + armorDefense;
        Debug.LogFormat("CalculateDefense: {0}", result);
        return result;
    }
}
