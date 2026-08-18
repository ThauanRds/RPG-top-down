using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GameManager : MonoBehaviour
{
    public Int32 CalculateHealth(Player player)
    {
        // Formula para calcular a vida máxima do jogador com base no nível e na resistência
        // Formula: (Resistência * 10) + (Level * 4) + 10
        Int32 result = (player.entity.resistence * 10) + (player.entity.level * 4) + 10;
        Debug.LogFormat("CalculateHealth: {0}", result);
        return result;
    }

    public Int32 CalculateMana(Player player)
    {
        // Formula para calcular a mana máxima do jogador com base no nível e na inteligência
        // Formula: (Inteligência * 10) + (Level * 4) + 5
        Int32 result = (player.entity.intelligence * 10) + (player.entity.level * 4) + 5;
        Debug.LogFormat("CalculateMana: {0}", result);
        return result;
    }

    public Int32 CalculateStamina(Player player)
    {
        // Formula para calcular a stamina máxima do jogador com base no nível e na resistência
        // Formula: (Resistência * 10) + (Level * 4) + 5
        Int32 result = (player.entity.resistence * player.entity.willPower) + (player.entity.level * 2) + 5;
        Debug.LogFormat("CalculateMana: {0}", result);
        return result;
    }

    public Int32 CalculateDamage(Player player, int weaponDamage)
    {
        // Formula para calcular o dano máximo do jogador com base no nível, na força e na arma
        // Formula: (Força * 2) + (Weapon* 2) + (Level * 3) + random (1, 20)
        System.Random random = new System.Random();
        Int32 result = (player.entity.strenght * 2) + (weaponDamage * 2) + (player.entity.level * 3) + random.Next(1,20);
        Debug.LogFormat("CalculateDamage: {0}", result);
        return result;
    }

    public Int32 CalculateDefense(Player player, int armorDefense)
    {
        // Formula para calcular a defesa máxima do jogador com base no nível e na armadura
        // Formula: (Defesa * 2) + (Level * 3) + armorDefense
        Int32 result = (player.entity.resistence * 2) + (player.entity.level * 3) + armorDefense;
        Debug.LogFormat("CalculateDefense: {0}", result);
        return result;
    }
}
