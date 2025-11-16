using UnityEngine;
using System.Collections.Generic;

public class CardManager : MonoBehaviour
{
    [SerializeField] private List<GameObject> summonCardPrefabs = new List<GameObject>();
    [SerializeField] private List<GameObject> spellCardPrefabs = new List<GameObject>();

    private List<Card> allCards = new List<Card>();
    private Dictionary<string, Card> cardsByName = new Dictionary<string, Card>();

    private void Start()
    {
        InitializeCards();
    }

    /// <summary>
    /// Initialize all available cards
    /// </summary>
    private void InitializeCards()
    {
        allCards.Clear();
        cardsByName.Clear();

        // Load summon cards
        foreach (var prefab in summonCardPrefabs)
        {
            SummonCard summonCard = prefab.GetComponent<SummonCard>();
            if (summonCard != null)
            {
                allCards.Add(summonCard);
                cardsByName[summonCard.cardName] = summonCard;
                Debug.Log($"Loaded Summon Card: {summonCard.cardName}");
            }
        }

        // Load spell cards
        foreach (var prefab in spellCardPrefabs)
        {
            SpellCard spellCard = prefab.GetComponent<SpellCard>();
            if (spellCard != null)
            {
                allCards.Add(spellCard);
                cardsByName[spellCard.cardName] = spellCard;
                Debug.Log($"Loaded Spell Card: {spellCard.cardName}");
            }
        }

        Debug.Log($"CardManager initialized with {allCards.Count} total cards");
    }

    /// <summary>
    /// Get a card by name
    /// </summary>
    public Card GetCardByName(string cardName)
    {
        if (cardsByName.ContainsKey(cardName))
        {
            return cardsByName[cardName];
        }

        Debug.LogWarning($"Card '{cardName}' not found!");
        return null;
    }

    /// <summary>
    /// Get all available cards (excluding a specific card if needed)
    /// </summary>
    public List<Card> GetAvailableCards(string excludeCardName = "")
    {
        List<Card> available = new List<Card>();

        foreach (var card in allCards)
        {
            if (card.gameObject.name != excludeCardName && card != null)
            {
                available.Add(card);
            }
        }

        return available;
    }

    /// <summary>
    /// Get all summon cards
    /// </summary>
    public List<Card> GetSummonCards()
    {
        List<Card> summons = new List<Card>();

        foreach (var card in allCards)
        {
            if (card is SummonCard)
            {
                summons.Add(card);
            }
        }

        return summons;
    }

    /// <summary>
    /// Get all spell cards
    /// </summary>
    public List<Card> GetSpellCards()
    {
        List<Card> spells = new List<Card>();

        foreach (var card in allCards)
        {
            if (card is SpellCard)
            {
                spells.Add(card);
            }
        }

        return spells;
    }

    /// <summary>
    /// Get a random card
    /// </summary>
    public Card GetRandomCard()
    {
        if (allCards.Count == 0) return null;

        return allCards[Random.Range(0, allCards.Count)];
    }

    /// <summary>
    /// Get a random card excluding Pandora's Box
    /// </summary>
    public Card GetRandomCardExcludingPandora()
    {
        List<Card> available = new List<Card>();

        foreach (var card in allCards)
        {
            if (card.gameObject.name != "PandorasBox")
            {
                available.Add(card);
            }
        }

        if (available.Count == 0) return null;

        return available[Random.Range(0, available.Count)];
    }

    /// <summary>
    /// Get all cards
    /// </summary>
    public List<Card> GetAllCards()
    {
        return new List<Card>(allCards);
    }

    /// <summary>
    /// Get card count
    /// </summary>
    public int GetCardCount()
    {
        return allCards.Count;
    }
}