using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreGameManagerScript : MonoBehaviour
{
    public static PreGameManagerScript Instance { get; private set; }

    [Header("Areas")]
    public GameObject PlayerArea;
    public GameObject EnemeyArea;

    [Header("Objects")]
    public GameObject startGameButton;

    [Header("Data")]
    public GameObject[] Cards;
    public GameObject[] selectedPlayerCards = new GameObject[4];
    public GameObject[] selectedEnemyCards = new GameObject[4];
    public List<GameObject> instantiatedCards = new List<GameObject>();
    private int deckSize = 4;
    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
        GameManager.OnGameStateChanged += GameManagerOnGameStateChanged;
    }

    // Manage cards
    public void DrawCards()
    {
        if (instantiatedCards.Count > 0)
        {
            foreach (GameObject card in instantiatedCards)
            {
                Destroy(card);
            }
            instantiatedCards.Clear();
        }
        foreach (GameObject card in Cards)
        {
            GameObject instantiatedCard = Instantiate(card, Vector2.zero, Quaternion.identity);
            instantiatedCard.transform.SetParent(PlayerArea.transform, false);
            instantiatedCards.Add(instantiatedCard);
            instantiatedCard.GetComponent<CardManagerScript>().HandlePreGameState();
        }
        foreach (GameObject card in Cards)
        {
            GameObject instantiatedCard = Instantiate(card, Vector2.zero, Quaternion.identity);
            instantiatedCard.transform.SetParent(EnemeyArea.transform, false);
            instantiatedCards.Add(instantiatedCard);
            instantiatedCard.GetComponent<CardManagerScript>().HandlePreGameState();
        }
    }
    public int SelectCard(GameObject card)
    {
        if (string.Equals(card.transform.parent.name, PlayerArea.name))
        {
            return ManageSelectedCardsArray(selectedPlayerCards, card);
        }
        else if (string.Equals(card.transform.parent.name, EnemeyArea.name))
        {
            return ManageSelectedCardsArray(selectedEnemyCards, card);
        }
        return -1;
    }
    // returning int to communicated the state of selection, if selected as hero (as first in array) returns 1, 
    // if selected as companion return 2 if deselected return 3 (error -1)
    private int ManageSelectedCardsArray(GameObject[] selectedCardsArray, GameObject card)
    {
        bool found = false;
        bool placed = false;
        int returnValue = 3;
        for (int i = 0; i < deckSize && !found; i++)
        {
            if (GameObject.ReferenceEquals(card, selectedCardsArray[i]))
            {
                selectedCardsArray[i] = null;
                found = true;
            }
        }
        if (!found)
        {
            for (int i = 0; i < deckSize && !placed; i++)
            {
                if (selectedCardsArray[i] == null)
                {
                    selectedCardsArray[i] = card;
                    placed = true;
                    if (i == 0)
                    {
                        returnValue = 1;
                    }
                    else
                    {
                        returnValue = 2;
                    }
                }
            }
        }
        return returnValue;
    }

    // Game state change
    private void GameManagerOnGameStateChanged(GameState state)
    {
        switch (state)
        {
            case GameState.PreGame:
                HandlePreGameState();
                break;
            case GameState.InGame:
                HandleInGameState();
                break;
            case GameState.PostGame:
                HandlePostGameState();
                break;
            default:
                break;
        }
    }
    private void HandlePreGameState()
    {
        startGameButton.gameObject.SetActive(true);
        DrawCards();
    }
    private void HandleInGameState()
    {
        startGameButton.gameObject.SetActive(false);
    }
    private void HandlePostGameState()
    {
        startGameButton.gameObject.SetActive(false);
    }

    // Utils
    public GameObject[] GetSelectedCardByTurn()
    {
        if (InGameManagerScript.Instance.State == InGameState.PlayerTurn)
        {
            return selectedPlayerCards;
        }
        else if (InGameManagerScript.Instance.State == InGameState.EnemyTurn)
        {
            return selectedEnemyCards;
        }
        return null;
    }
    public GameObject[] GetSelectedCardByNonTurn()
    {
        if (InGameManagerScript.Instance.State == InGameState.PlayerTurn)
        {
            return selectedEnemyCards;
        }
        else if (InGameManagerScript.Instance.State == InGameState.EnemyTurn)
        {
            return selectedPlayerCards;
        }
        return null;
    }
    public GameObject[] GetSelectedCardsByOwner(PlayerType owner)
    {
        if (owner == PlayerType.player)
        {
            return selectedPlayerCards;
        }
        else if (owner == PlayerType.enemy)
        {
            return selectedEnemyCards;
        }
        return null;
    }

}
