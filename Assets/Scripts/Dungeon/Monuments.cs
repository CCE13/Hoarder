using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using System;
using UnityEngine.Events;
using UnityEngine.UI;
using Player;
using TMPro;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class Monuments : MonoBehaviour, Iinteractable
{
    public enum MonumentType
    {
        MonumentOfGreed,
        MonumentOfBarter,
        MonumentOfAtonement,
        MonumentOfValor,
        MonumentOfSacrifice
    }
    public MonumentType monumentType;
   [Header("Interact UI")]
    public GameObject interactUI;
    
    public bool cardSelected = false;

    [Header("Card UI")]
    public CardEvent[] cardEvents;
    public Image radialImage;


    public ParticleSystem coinSplosion;


    [Range(0,1)]public float percentageOfHealthToSacrifice;

    public int goldRequired;
    private Vector3 interactShowedSize => Vector3.one;
    private Vector3 showedSize => new Vector3(1.2f,1.6f,1.9f);
    private Vector3 hoveredSize => new Vector3(1.3f, 1.7f, 2f);
    private const float _timeToShow = 0.5f;
    private int _currentCardIdSelected;
    private bool _isAppeared = false;
    private bool _isInteracting;
    private bool cardsSet;
    private bool _used;


    private Controls controls;
    private GameObject playerInteracting;
    private SwordMan swordsMan;
    private Ranger ranger;
    private CoinCounter coinCounter;

    private DungeonController inst;


    public static event Action<PlayerController,Transform> Reviving;

    [Serializable]
    public class CardEvent
    {   
        public GameObject cardToSelect;
        public Material material;
        public int coinCost;
        public UnityEvent cardSelected;
    }
    #region editor
#if UNITY_EDITOR
    [CustomEditor(typeof(Monuments))]
    [System.Serializable]
    public class ModifierEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            Monuments monument = (Monuments)target;
            EditorUtility.SetDirty(target);
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Settings");
            EditorGUILayout.Space();

            monument.monumentType = (MonumentType)EditorGUILayout.EnumPopup(monument.monumentType);

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Varaibles");

            if (monument.monumentType == MonumentType.MonumentOfGreed || monument.monumentType == MonumentType.MonumentOfBarter)
            {
                monument.radialImage = EditorGUILayout.ObjectField("Radial Image", monument.radialImage, typeof(Image), true) as Image;
                monument.interactUI = EditorGUILayout.ObjectField("Interact UI",monument.interactUI,typeof(GameObject),true) as GameObject;
               monument.cardSelected = EditorGUILayout.Toggle("Card Selected", monument.cardSelected);
                SerializedProperty cardEvents = serializedObject.FindProperty("cardEvents");
                EditorGUILayout.PropertyField(cardEvents, includeChildren: true);
                if (cardEvents.hasChildren)
                {
                    serializedObject.ApplyModifiedProperties();
                }
                
            }
            if(monument.monumentType == MonumentType.MonumentOfSacrifice)
            {
                monument.radialImage = EditorGUILayout.ObjectField("Radial Image", monument.radialImage, typeof(Image), true) as Image;
                monument.interactUI = EditorGUILayout.ObjectField("Interact UI", monument.interactUI, typeof(GameObject), true) as GameObject;
                monument.coinSplosion = EditorGUILayout.ObjectField("Coin Explosion", monument.coinSplosion, typeof(ParticleSystem), true) as ParticleSystem;
                monument.percentageOfHealthToSacrifice = EditorGUILayout.Slider("Percentage Of Health",monument.percentageOfHealthToSacrifice, 0f, 1f);
            }
            if(monument.monumentType == MonumentType.MonumentOfAtonement)
            {
                monument.goldRequired = EditorGUILayout.IntField("Gold Required" ,monument.goldRequired);
                monument.radialImage = EditorGUILayout.ObjectField("Radial Image", monument.radialImage, typeof(Image), true) as Image;
                monument.interactUI = EditorGUILayout.ObjectField("Interact UI", monument.interactUI, typeof(GameObject), true) as GameObject;
            }
            if(monument.monumentType == MonumentType.MonumentOfValor)
            {
                monument.radialImage = EditorGUILayout.ObjectField("Radial Image", monument.radialImage, typeof(Image), true) as Image;
                monument.interactUI = EditorGUILayout.ObjectField("Interact UI", monument.interactUI, typeof(GameObject), true) as GameObject;
            }
        }
    }
#endif
    #endregion


    private void OnValidate()
    {
        gameObject.name = monumentType.ToString();
    }
    private void Awake()
    {
        swordsMan = FindObjectOfType<SwordMan>();
        ranger = FindObjectOfType<Ranger>();
        coinCounter = FindObjectOfType<CoinCounter>();
        controls = InputManager.instance.control;
        controls.SwordsMan.InteractionNAV.started += ctx => CheckHoveredCard(ctx.ReadValue<Vector2>(),swordsMan);
        controls.SwordsMan.InteractionSelect.started += ctx => SelectCard(cardEvents[_currentCardIdSelected].cardToSelect, swordsMan);
        controls.Ranger.InteractionNAV.started += ctx => CheckHoveredCard(ctx.ReadValue<Vector2>(),ranger);
        controls.Ranger.InteractionSelect.started += ctx => SelectCard(cardEvents[_currentCardIdSelected].cardToSelect,ranger);
    }

    private void Start()
    {
        foreach (CardEvent card in cardEvents)
        {
            card.cardToSelect.GetComponent<Image>().material = Instantiate(card.cardToSelect.GetComponent<Image>().material);
            card.cardToSelect.transform.GetChild(0).GetComponent<Image>().material = Instantiate(card.cardToSelect.GetComponent<Image>().material);
            card.material = card.cardToSelect.GetComponent<Image>().material;
        }

        inst = DungeonController.instance;
    }

    public void DebugTest(string name)
    {
        Debug.Log(name);
    }

    public void CanInteract()
    {
        if (_used) return;
        if (playerInteracting != null) return;
        if (cardSelected) return;

        if(monumentType == MonumentType.MonumentOfSacrifice)
        {
            interactUI.GetComponent<TMP_Text>().text = "[ 50% Hp ]";
        }

        if (monumentType == MonumentType.MonumentOfValor)
        {
            interactUI.GetComponent<TMP_Text>().text = "[ Challenge ]";
        }

        if (monumentType == MonumentType.MonumentOfBarter)
        {
            interactUI.GetComponent<TMP_Text>().text = "[ Shop ]";
        }

        if (monumentType == MonumentType.MonumentOfGreed)
        {
            interactUI.GetComponent<TMP_Text>().text = "[ ???? ]";
        }

        if (monumentType == MonumentType.MonumentOfAtonement)
        {
            int _goldRequired = goldRequired * ( 1 + inst.difficultyLevel * 5 );
            interactUI.GetComponent<TMP_Text>().text = $"[ {_goldRequired} ]";
        }

        Show(interactUI, interactShowedSize);
        Debug.Log("Can Interact");
    }


    /// <summary>
    /// Hides the interaction ui
    /// </summary>
    public void CannotInteract()
    {

        if (monumentType == MonumentType.MonumentOfGreed || monumentType == MonumentType.MonumentOfBarter)
        {
            foreach (CardEvent cardevent in cardEvents)
            {
                GameObject card = cardevent.cardToSelect;
                Hide(card, Vector3.zero);
            }
        }

        playerInteracting = null;
        _isInteracting = false;
        _isAppeared = false;
        Hide(interactUI, Vector3.zero);
        Debug.Log("Left interaction zone");
    }

    /// <summary>
    /// Checks if any other _player is interacting with the monumenet.
    /// If yes, return
    /// If no, Show the cards delegated to the monument.
    /// </summary>
    /// <param name="playerInteractedWith"></param>
    public void Interacted(GameObject playerInteractedWith)
    {
        if (playerInteracting != null) return; 
        if (cardSelected) return;
        if (_used) return;
        Hide(interactUI, Vector3.zero);
        playerInteracting = playerInteractedWith;
        if(monumentType == MonumentType.MonumentOfGreed)
        {
            Debug.Log("Reached");
            ModifierEventManager.AddModifierChosen = false;
            ModifierEventManager.RemoveModifierChosen = false;
            foreach (CardEvent card in cardEvents)
            {
                UnityAction events = ModifierEventManager.Event(playerInteractedWith.GetComponent<PlayerDamageStats>(),transform);
                card.cardSelected.AddListener(events); 
                Debug.Log(card.cardToSelect.ToString());
            }
        }
        if(monumentType == MonumentType.MonumentOfBarter)
        {
            //get a random set rarity.
            List<ModiferCollectable> rarity = WeightProbabilityManager.RandomRarity();
            int coinCost = WeightProbabilityManager.CoinRarityCost(rarity);
            foreach (CardEvent card in cardEvents)
            {
                if (cardsSet) { continue;}
                //get a random modifier
                GameObject modifier = WeightProbabilityManager.RandomCollectibleOfRarity(rarity);
                Debug.Log("Reached");
                card.cardToSelect.transform.GetChild(0).GetComponent<Image>().sprite = modifier.GetComponent<ModiferCollectable>().modifier.spriteToShow;

                //set the picture in the modifier on the card
                card.cardSelected.AddListener(() => ModifierEventManager.SpawnModifier(transform,modifier));

                // Cost scale
                card.coinCost = coinCost * ( 1 + inst.difficultyLevel * 3);
                TMP_Text coinCostText = card.cardToSelect.transform.GetChild(1).GetChild(0).GetComponent<TMP_Text>();
                coinCostText.text = $"<sprite=0>{card.coinCost}";
            }
            cardsSet = true;
        }

        if (monumentType == MonumentType.MonumentOfGreed || monumentType == MonumentType.MonumentOfBarter)
        {
            RequireCards();
        }
        if(monumentType == MonumentType.MonumentOfSacrifice)
        {
            MonumentOfSacrifice();
        }
        if(monumentType == MonumentType.MonumentOfAtonement)
        {
            MonumentOfAtonement();
        }

        if(monumentType == MonumentType.MonumentOfValor)
        {
            MonumentOfValor();
        }

    }

    private void MonumentOfValor()
    {
        int toSpawnAmt = UnityEngine.Random.Range(3 + inst.difficultyLevel, 6 + inst.difficultyLevel);
        float xOffset = UnityEngine.Random.Range(-12, 12);
        float zOffset = UnityEngine.Random.Range(-12, 12);

        // Spawns enemy.
        for (int i = 0; i < toSpawnAmt; i++)
        {
            // List of enemy type.
            int enemyVar = UnityEngine.Random.Range(0, 2);
            bool enemyType = true;

            if (enemyVar == 0)
            {
                enemyType = false;
            }

            var _spawnedEnemy = ObjectPool.SharedInstance.GetEnemy(enemyType, 100 + (50 * inst.difficultyLevel));

            _spawnedEnemy.transform.position = new Vector3(transform.position.x + xOffset, 1, transform.position.z + zOffset);
            _spawnedEnemy.transform.rotation = Quaternion.Euler(new Vector3(0,UnityEngine.Random.Range(0,361),0));

            _spawnedEnemy.SetActive(true);
        }

        StartCoroutine(Used());
    }

    private void MonumentOfSacrifice()
    {
        var health = playerInteracting.GetComponent<Health>();
        var healthForGold = Mathf.RoundToInt(health.currentHealth * percentageOfHealthToSacrifice);
        health.LoseHealth(healthForGold);

        var burst = coinSplosion.emission.GetBurst(0);
        var newBurst = new ParticleSystem.Burst(burst.time, healthForGold, burst.cycleCount, burst.repeatInterval);
        coinSplosion.emission.SetBurst(0, newBurst);
        coinSplosion.Play();
        StartCoroutine(Used());
    }

    private void MonumentOfAtonement()
    {
        int _goldRequired = goldRequired * ( 1 + inst.difficultyLevel * 5 );
        if(coinCounter.currentCoinCount < _goldRequired) { return; }
        if(playerInteracting == swordsMan.gameObject)
        {
            if (ranger.isDead)
            {
                Reviving?.Invoke(ranger,transform);
                coinCounter.LoseGold(_goldRequired);
                StartCoroutine(Used());
            }
        }
        if(playerInteracting == ranger.gameObject)
        {
            if (swordsMan.isDead)
            {
                Reviving?.Invoke(swordsMan,transform);
                coinCounter.LoseGold(_goldRequired);
                StartCoroutine(Used());
            }
        }
    }

    private void RequireCards()
    {
        if (!_isAppeared)
        {
            _currentCardIdSelected = 0;
            _isAppeared = true;
            _isInteracting = true;
            CardHover();
        }
        else
        {
            _isAppeared = false;

            foreach (CardEvent cardevent in cardEvents)
            {
                GameObject card = cardevent.cardToSelect;
                Hide(card, Vector3.zero);
            }
            _isInteracting = false;
        }
    }


    #region Monument of greed/ Monument of Barter

    /// <summary>
    /// Shows the <paramref name="card"/> and scales it to the <paramref name="size"/>
    /// </summary>
    /// <param name="card"></param>
    /// <param name="size"></param>
    private void Show(GameObject card, Vector3 size)
    {
        StartCoroutine(Scaling(card, size));
    }


    /// <summary>
    /// Hides the <paramref name="card"/> and scales it to the <paramref name="size"/>
    /// </summary>
    /// <param name="card"></param>
    /// <param name="size"></param>
    private void Hide(GameObject card, Vector3 size)
    {
        StartCoroutine(Scaling(card, size));
    }

    /// <summary>
    /// Hovers the <paramref name="card"/> and scales it to the <paramref name="size"/>
    /// </summary>
    /// <param name="card"></param>
    /// <param name="size"></param>
    private void Hovering(GameObject card, Vector3 size)
    {
        StartCoroutine(Scaling(card, size));
    }
    private IEnumerator Scaling(GameObject card, Vector3 size)
    {
        for (float i = 0; i < _timeToShow; i += Time.fixedDeltaTime)
        {
            card.transform.localScale = new Vector3(
                Mathf.Lerp(card.transform.localScale.x, size.x, i),
                Mathf.Lerp(card.transform.localScale.y, size.y, i),
                Mathf.Lerp(card.transform.localScale.z, size.z, i));
            yield return null;
        }
    }

    //Checks the current card id selected , and hovers the card of that specific id
    private void CheckHoveredCard(Vector2 directionToChangeCard,PlayerController player)
    {
        if (!playerInteracting) return;
        if (player != playerInteracting.GetComponent<PlayerController>()) { return; }
        if (!_isAppeared) { return; }
        if (!_isInteracting) { return; }
        if (cardSelected) { return; }

        if (directionToChangeCard.x > 0)
        {
            //move selected card to the right.
            _currentCardIdSelected++;
            if (_currentCardIdSelected == cardEvents.Length) { _currentCardIdSelected = cardEvents.Length - 1; }
            CardHover();
        }
        else
        {
            _currentCardIdSelected--;
            if (_currentCardIdSelected < 0) { _currentCardIdSelected = 0; }
            CardHover();
        }
    }

    //hovers the card
    private void CardHover()
    {
        if (cardSelected) { return; }
        foreach (CardEvent cardEvent in cardEvents)
        {
            GameObject card = cardEvent.cardToSelect;
            Show(card, showedSize);
            GameObject cardToHover = cardEvents[_currentCardIdSelected].cardToSelect;
            Hovering(cardToHover, hoveredSize);
        }
    }


    //runs an event when the card is selected
    private void SelectCard(GameObject cardSelect, PlayerController player)
    {
        if (!playerInteracting) return;
        if (player != playerInteracting.GetComponent<PlayerController>()) { return; }
        if (!_isAppeared) { return; }
        if (cardSelected) { return; }
        foreach (CardEvent cardEvent in cardEvents)
        {
            if (cardEvent.cardToSelect == cardSelect)
            {
                if (monumentType == MonumentType.MonumentOfBarter|| monumentType == MonumentType.MonumentOfGreed)
                {
                    if(coinCounter.currentCoinCount < cardEvent.coinCost) 
                    {
                        Debug.Log("Not Enough Gold");
                        return; 
                    }
                    else
                    {
                        coinCounter.LoseGold(cardEvent.coinCost);
                    }
                }
                cardEvent.cardSelected?.Invoke();
                break;
            }
        }
        foreach (CardEvent cardEvent in cardEvents)
        {
            if (cardEvent.cardToSelect != cardSelect)
            {
                //disolve
               StartCoroutine(Dissolve(cardEvent.cardToSelect));
                //Hide(cardEvent.cardToSelect, Vector3.zero);
            }
        }
        cardSelected = true;
        playerInteracting = null;
        StartCoroutine(Used());
        Debug.Log("Card Selected");
    }

    private IEnumerator Used()
    {
        for (float i = 0; i < 1f; i+=Time.deltaTime)
        {
            radialImage.fillAmount = Mathf.Lerp(1, 0, i);
            yield return new WaitForSeconds(0.01f);
        }
        if(radialImage.fillAmount != 0)
        {
            radialImage.fillAmount = 0;
        }

        _used = true;
    }
    private IEnumerator Dissolve(GameObject cardToSelect)
    {
        Material material = cardToSelect.GetComponent<Image>().material;
        Material childMaterial = cardToSelect.transform.GetChild(0).GetComponent<Image>().material;
        GameObject coinCost = cardToSelect.transform.GetChild(1).gameObject;
        coinCost.SetActive(false);
        for (float i = -0.2f; i < 1; i += Time.deltaTime)
        {
            material.SetFloat(Shader.PropertyToID("Dissolve"), Mathf.MoveTowards(material.GetFloat(Shader.PropertyToID("Dissolve")), i, 1f));
            childMaterial.SetFloat(Shader.PropertyToID("Dissolve"), Mathf.MoveTowards(childMaterial.GetFloat(Shader.PropertyToID("Dissolve")), i, 1f));
            yield return null;
        }
    }
    #endregion
    


}

