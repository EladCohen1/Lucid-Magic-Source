using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Linq;

public class CardManagerScript : MonoBehaviour
{
    [Header("Scripts")]
    CardSelectionScript cardSelectionScript;
    CardAttackScript cardAttackScript;
    CardReviveCompanionScript cardReviveCompanionScript;
    CardAbilityScript cardAbilityScript;
    [SerializeField]
    public List<AbilityTriggerScript> abilityTriggerScripts;

    [Header("Objects")]
    private Text HealthValueText;
    private Text AttackValueText;

    [Header("Areas")]
    public GameObject PlayerGraveyard;
    public GameObject EnemyGraveyard;


    [Header("Game Variables")]
    [SerializeField]
    private int baseHealth;
    [SerializeField]
    private int maxHealth;
    [SerializeField]
    private int currentHealth;
    [SerializeField]
    private int baseAttack;
    [SerializeField]
    private int currentAttack;
    private bool isHero;
    private bool isDead;
    public PlayerType Owner;
    public List<Effect> Effects;

    // Public Game Variables
    public int BaseHealth { get { return baseHealth; } private set { baseHealth = value; } }
    public int MaxHealth { get { return maxHealth; } private set { maxHealth = value; } }
    public int CurrentHealth { get { return currentHealth; } private set { currentHealth = value; } }
    public int BaseAttack { get { return baseAttack; } private set { baseAttack = value; } }
    public int CurrentAttack { get { return currentAttack; } private set { currentAttack = value; } }
    public bool IsHero { get { return isHero; } private set { isHero = value; } }
    public bool IsDead { get { return isDead; } private set { isDead = value; } }

    void Awake()
    {
        cardSelectionScript = gameObject.GetComponent<CardSelectionScript>();
        cardAttackScript = gameObject.GetComponent<CardAttackScript>();
        cardReviveCompanionScript = gameObject.GetComponent<CardReviveCompanionScript>();
        cardAbilityScript = gameObject.GetComponent<CardAbilityScript>();
        HealthValueText = transform.Find("Health Value").GetComponent<Text>();
        AttackValueText = transform.Find("Attack Value").GetComponent<Text>();
        PlayerGraveyard = GameObject.Find("PlayerGraveyard");
        EnemyGraveyard = GameObject.Find("EnemyGraveyard");
    }
    void Start()
    {
        Effects = new List<Effect>();
        RevertToBaseStats();
        UpdateDisplay();
    }
    void OnEnable()
    {
        GameManager.OnGameStateChanged += GameManagerOnGameStateChanged;
        InGameManagerScript.OnInGameStateChanged += InGameManagerOnInGameStateChanged;
    }
    void OnDisable()
    {
        GameManager.OnGameStateChanged -= GameManagerOnGameStateChanged;
        InGameManagerScript.OnInGameStateChanged -= InGameManagerOnInGameStateChanged;
    }
    void OnDestroy()
    {
        cardSelectionScript.enabled = false;
        cardAttackScript.enabled = false;
        cardReviveCompanionScript.enabled = false;
        cardAbilityScript.enabled = false;
        foreach (var script in abilityTriggerScripts)
        {
            script.enabled = false;
        }
    }

    void Update()
    {
        if (currentHealth < maxHealth && !isDead)
        {
            HealthValueText.color = Color.red;
        }
        else
        {
            HealthValueText.color = Color.black;
        }
        foreach (Effect effect in Effects)
        {
            if (isDead)
            {
                effect.Icon.SetActive(false);
            }
            else
            {
                effect.Icon.SetActive(true);
            }
        }
    }
    // GameManager change state
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
    public void HandlePreGameState()
    {
        cardSelectionScript.enabled = true;
        cardAttackScript.enabled = false;
        cardReviveCompanionScript.enabled = false;
        cardAbilityScript.enabled = false;
        foreach (var script in abilityTriggerScripts)
        {
            script.enabled = false;
        }
    }
    public void HandleInGameState()
    {
        cardSelectionScript.enabled = false;
        cardAttackScript.enabled = true;
        cardReviveCompanionScript.enabled = true;
        cardAbilityScript.enabled = true;
        foreach (var script in abilityTriggerScripts)
        {
            script.enabled = true;
        }
    }
    public void HandlePostGameState()
    {
        cardSelectionScript.enabled = false;
        cardAttackScript.enabled = false;
        cardReviveCompanionScript.enabled = false;
        cardAbilityScript.enabled = false;
        foreach (var script in abilityTriggerScripts)
        {
            script.enabled = false;
        }
    }

    // Turn Change
    private void InGameManagerOnInGameStateChanged(InGameState state)
    {
        switch (state)
        {
            case InGameState.PlayerTurn:
                UpdateTurnsActiveEffects();
                UpdateEffectsNewTurn();
                break;
            case InGameState.EnemyTurn:
                UpdateTurnsActiveEffects();
                UpdateEffectsNewTurn();
                break;
            default:
                break;
        }
    }

    private void UpdateEffectsNewTurn()
    {
        if (IsEffectTypeInEffects(EffectType.Harvest))
        {
            Effect harvestEffect = GetFirstEffectFromEffectsByType(EffectType.Harvest);
            if (harvestEffect.turnsActive > 1)
            {
                EffectType.ActiveHarvest.ActivateEffect(gameObject, harvestEffect.abilityReference);
            }
        }
        if (IsEffectTypeInEffects(EffectType.Rage))
        {
            Effect rageEffect = GetFirstEffectFromEffectsByType(EffectType.Rage);
            if (rageEffect.turnsActive > 1)
            {
                EffectType.Rage.RemoveLast(gameObject);
            }
        }
    }

    private void UpdateTurnsActiveEffects()
    {
        foreach (Effect effect in Effects)
        {
            // only updating effects that are relevant
            switch (effect.type)
            {
                case EffectType.Rage:
                    effect.turnsActive++;
                    break;
                case EffectType.Harvest:
                    effect.turnsActive++;
                    break;
                default:
                    break;
            }
        }
    }

    // Public functions
    public void UpdateDisplay()
    {
        HealthValueText.text = currentHealth.ToString();
        AttackValueText.text = currentAttack.ToString();
        if (cardAttackScript.enabled)
        {
            cardAttackScript.UpdateCanAttackDisplay();
        }
    }
    public void ReduceHealthWithoutKillingOrCappingToZero(int amount)
    {
        currentHealth -= amount;
        UpdateDisplay();
    }
    public void IncreaseMaxHealth(int amount)
    {
        maxHealth += amount;
    }
    public void ReduceMaxHealth(int amount)
    {
        maxHealth -= amount;
        IncreaseCurrentHealth(0); // to check if over max
    }
    public void ReduceCurrentHealth(int amount)
    {
        int amountToReduce = amount;
        if (IsEffectTypeInEffects(EffectType.Rage))
        {
            amountToReduce--;
        }
        if (BattleFieldManagerScript.Instance.CurrentBattleField == BattleField.Forest)
        {
            amountToReduce--;
            if (amountToReduce < 0)
            {
                amountToReduce = 0;
            }
        }
        if (currentHealth - amountToReduce <= 0)
        {
            currentHealth -= amountToReduce;
            KillCard();
        }
        else
        {
            currentHealth -= amountToReduce;
        }
        UpdateDisplay();
    }
    public void IncreaseCurrentHealth(int amount)
    {
        if (isHero)
        {
            currentHealth += amount;
        }
        else
        {
            if (currentHealth + amount > maxHealth)
            {
                currentHealth = maxHealth;
            }
            else
            {
                currentHealth += amount;
            }
        }
        UpdateDisplay();
    }
    public void ReduceCurrentAttack(int amount)
    {
        if (currentAttack - amount <= 0)
        {
            currentAttack = 0;
        }
        else
        {
            currentAttack -= amount;
        }
        UpdateDisplay();
    }
    public void IncreaseCurrentAttack(int amount)
    {
        currentAttack += amount;
        UpdateDisplay();
    }
    public void RevertToBaseStats()
    {
        currentHealth = baseHealth;
        currentAttack = baseAttack;
        maxHealth = baseHealth;
        UpdateDisplay();
    }
    public void UpdateToRevivestats()
    {
        currentHealth = maxHealth;
        UpdateDisplay();
    }
    public void MakeHero()
    {
        isHero = true;
        baseHealth = InGameManagerScript.Instance.HeroHealth;
        // hero doesn't use maxHealth
        currentHealth = baseHealth;
        UpdateDisplay();
    }

    // kill card
    public bool WillKillHero(int damage)
    {
        if (isHero)
        {
            if (currentHealth - damage <= 0)
            {
                return true;
            }
        }
        return false;
    }
    public void KillCard()
    {
        if (Owner == PlayerType.player)
        {
            if (isHero)
            {
                // end game with enemy winner;
                GameManager.Instance.gameOutCome = GameOutCome.EnemyWon;
                GameManager.Instance.UpdateGameState(GameState.PostGame);
            }
            else
            {
                transform.SetParent(PlayerGraveyard.transform, false);
                InGameManagerScript.OnCardDeath?.Invoke();
                isDead = true;
                KillCardUpdateEffects();
            }
        }
        else if (Owner == PlayerType.enemy)
        {
            if (isHero)
            {
                // end game with player winner;
                GameManager.Instance.gameOutCome = GameOutCome.PlayerWon;
                GameManager.Instance.UpdateGameState(GameState.PostGame);
            }
            else
            {
                transform.SetParent(EnemyGraveyard.transform, false);
                isDead = true;
                InGameManagerScript.OnCardDeath?.Invoke();
                KillCardUpdateEffects();
            }
        }
    }
    public void KillCard(PlayerType player)
    {
        if (player == PlayerType.player)
        {
            if (isHero)
            {
                // end game with enemy winner;
                GameManager.Instance.gameOutCome = GameOutCome.EnemyWon;
                GameManager.Instance.UpdateGameState(GameState.PostGame);
            }
            else
            {
                transform.SetParent(PlayerGraveyard.transform, false);
                InGameManagerScript.OnCardDeath?.Invoke();
                isDead = true;
                KillCardUpdateEffects();
            }
        }
        else if (player == PlayerType.enemy)
        {
            if (isHero)
            {
                // end game with player winner;
                GameManager.Instance.gameOutCome = GameOutCome.PlayerWon;
                GameManager.Instance.UpdateGameState(GameState.PostGame);
            }
            else
            {
                transform.SetParent(EnemyGraveyard.transform, false);
                InGameManagerScript.OnCardDeath?.Invoke();
                isDead = true;
                KillCardUpdateEffects();
            }
        }
    }

    // Revive Card
    public void ReviveCard()
    {
        UpdateToRevivestats();
        isDead = false;
        cardAttackScript.AlreadyAttacked = true;
        cardReviveCompanionScript.IsHovering = false;
        if (Owner == PlayerType.player)
        {
            transform.SetParent(InGameManagerScript.Instance.PlayerCompanionsArea.transform, false);
        }
        else if (Owner == PlayerType.enemy)
        {
            transform.SetParent(InGameManagerScript.Instance.EnemyCompanionsArea.transform, false);
        }
    }

    // Utils
    public void DisplayReviveStats()
    {
        HealthValueText.text = maxHealth.ToString();
        AttackValueText.text = currentAttack.ToString();
    }
    private void KillCardUpdateEffects()
    {
        for (int i = Effects.Count() - 1; i >= 0; i--)
        {
            if (EffectTypeMethods.IsRemovedOnDeath(Effects[i], gameObject))
            {
                Effects[i].type.Remove(gameObject, i);
            }
        }
    }
    public bool IsEffectTypeInEffects(EffectType effectType)
    {
        return Effects.Where(item => item.type == effectType).ToArray().Length > 0;
    }
    public Effect GetFirstEffectFromEffectsByType(EffectType effectType)
    {
        Effect[] array = Effects.Where(item => item.type == effectType).ToArray();
        if (array.Length > 0)
        {
            return array[0];
        }
        return null;
    }
}