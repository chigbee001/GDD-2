using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardManager : MonoBehaviour
{
    private Queue<Card> cards = new Queue<Card>();
    private List<Card> activeCards = new List<Card>();
    public Sprite[] cardEffects = new Sprite[5];
    public Transform cardDisplay;
    public GameObject cardUIPrefab;
    private GameManager gameManager;
    private float cardUIOffset;
    private Player player;
    public RuntimeAnimatorController[] cardAnims = new RuntimeAnimatorController[5];

    //mana
    private int totalMana = 10; //max mana
    private float manaGainRate = 1; //1 mana/second
    private float currentMana;
    //mana bar
    public Transform manaDisplayParent;
    private Image manaBar;
    private Image barBackground;
    private Text manaText;
    private float barFullSize;
    private float barHeight;
    private float manaBarFlashTimer = 0;

    // Start is called before the first frame update
    void Start()
    {
        //create cards
        cards.Enqueue(new Card("Rupture", HitboxShape.Rectangle, new Vector2(2, 2), 2, false, false, 0, cardEffects[0], 0, 1, cardAnims[0]));
        cards.Enqueue(new Card("Fireball", HitboxShape.Ellipse, new Vector2(.75f, .5f), 3, true, true, 10, cardEffects[1], 1f, 1, cardAnims[1]));
        cards.Enqueue(new Card("Meteor", HitboxShape.Circle, new Vector2(4, 4), 4, false, false, 0, cardEffects[2], 0, 4, cardAnims[2]));
        cards.Enqueue(new Card("Lightning", HitboxShape.Rectangle, new Vector2(3, .25f), 5, true, true, 15, cardEffects[3], 1f, 2, null));
        cards.Enqueue(new Card("Sun Disc", HitboxShape.Circle, new Vector2(1, 1), 6, true, true, 8, cardEffects[4], 1f, 3, cardAnims[4]));

        //set up card display
        cardUIOffset = -1 * (cardUIPrefab.GetComponent<RectTransform>().rect.height + 10);
        Card[] cardArr = cards.ToArray();
        for(int i = 0; i < cardArr.Length; i++)
        {
            GameObject newCardUI = Instantiate(cardUIPrefab, cardDisplay);
            newCardUI.GetComponent<RectTransform>().localPosition = new Vector3(0, (cardArr.Length - 1 - i) * cardUIOffset, 0);
            newCardUI.transform.GetChild(0).GetComponent<Text>().text = cardArr[i].Name + " - " + cardArr[i].ManaCost;
        }

        //get reference to gamemanager
        gameManager = FindObjectOfType<GameManager>();

        //mana set up
        currentMana = 0;
        barBackground = manaDisplayParent.GetChild(0).GetComponent<Image>();
        manaBar = manaDisplayParent.GetChild(1).GetComponent<Image>();
        manaText = manaDisplayParent.GetChild(2).GetComponent<Text>();
        barFullSize = barBackground.rectTransform.rect.width;
        barHeight = barBackground.rectTransform.rect.height;
        UpdateManaBar(currentMana, totalMana);

        player = gameObject.GetComponent<Player>();
    }

    // Update is called once per frame
    void Update()
    {
        if(gameManager.IsPaused) 
        {
            return;
        }

        if (Input.GetMouseButtonDown(0) && cards.Count > 0 && !player.Stunned)
        {
            //activate card if there is sufficient mana
            if (currentMana >= cards.Peek().ManaCost)
            {
                cards.Peek().Activate(GetMousePosInWorld(Input.mousePosition), transform.position);
                currentMana -= cards.Peek().ManaCost;
                UpdateManaBar(currentMana, totalMana);
            }
            else
            {
                FlashManaBar();
            }

            //remove activated card from top, put it on the bottom
            activeCards.Add(cards.Dequeue());
            cards.Enqueue(activeCards[activeCards.Count - 1]);

            //update card display
            UpdateCardDisplay(activeCards[activeCards.Count - 1]);
        }

        if(Input.GetMouseButtonDown(1) && cards.Count > 1 && !player.Stunned)
        {
            //get the first card
            Card firstCard = cards.Dequeue();

            //activate card if there is sufficient mana
            if (currentMana >= cards.Peek().ManaCost)
            {
                cards.Peek().Activate(GetMousePosInWorld(Input.mousePosition), transform.position);
                currentMana -= cards.Peek().ManaCost;
                UpdateManaBar(currentMana, totalMana);
            }
            else
            {
                FlashManaBar();
            }
            
            //add card to activeCards
            activeCards.Add(cards.Dequeue());

            //send first card to the back so it ends up back in front after for
            cards.Enqueue(firstCard);

            //cycle all cards other than first one and active one
            for(int i = 0; i < cards.Count - 1; i++)
            {
                cards.Enqueue(cards.Dequeue());
            }

            //send active card to the back
            cards.Enqueue(activeCards[activeCards.Count - 1]);

            //update card display
            UpdateCardDisplay(activeCards[activeCards.Count - 1]);
        }

        //call update of active cards
        for (int i = 0; i < activeCards.Count; i++)
        {
            activeCards[i].Update();

            if (!activeCards[i].Active)
            {
                activeCards.RemoveAt(i);
                i--;
            }
        }

        //increase mana
        if (currentMana < totalMana)
        {
            currentMana += manaGainRate * Time.deltaTime;

            if (currentMana > totalMana)
            {
                currentMana = totalMana;
            }

            UpdateManaBar(currentMana, totalMana);
        }

        if (manaBarFlashTimer > 0)
        {
            manaBarFlashTimer -= Time.deltaTime;

            manaBar.color = new Color(0, (1 - manaBarFlashTimer / .5f) * .3f, 1 - manaBarFlashTimer / .5f, 1);

            if (manaBarFlashTimer <= 0)
            {
                manaBar.color = new Color(0, .3f, 1, 1);
            }
        }
    }

    //get mouse position in world coords
    private Vector3 GetMousePosInWorld(Vector3 inputPos)
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = 10;
        return Camera.main.ScreenToWorldPoint(mousePos);
    }

    //update card display to reflect internal queue
    //this way of doing it is garbage, I will change it eventually, I can't tell if I'm doing it wrong but it's 20 times harder than making the card system was
    private void UpdateCardDisplay(Card usedCard)
    {
        foreach (Transform child in cardDisplay.transform)
        {
            Destroy(child.gameObject);
        }

        Card[] cardArr = cards.ToArray();
        for (int i = 0; i < cardArr.Length; i++)
        {
            //if (cardDisplay.GetChild(i).GetComponentInChildren<Text>().text == usedCard.Name)
            //{
            //    cardDisplay.GetChild(i).GetComponent<RectTransform>().anchoredPosition = new Vector3(0, 0, 0);
            //}
            //else
            //{
            //    cardDisplay.GetChild(i).GetComponent<RectTransform>().anchoredPosition = new Vector3(0, (cardArr.Length - i) * -110, 0);
            //}
            
            GameObject newCardUI = Instantiate(cardUIPrefab, cardDisplay);
            newCardUI.GetComponent<RectTransform>().localPosition = new Vector3(0, (cardArr.Length - 1 - i) * cardUIOffset, 0);
            newCardUI.transform.GetChild(0).GetComponent<Text>().text = cardArr[i].Name + " - " + cardArr[i].ManaCost;
        }
    }

    //update mana bar and text
    private void UpdateManaBar(float amount, float max)
    {
        //change bar
        manaBar.rectTransform.sizeDelta = new Vector2(amount / max * barFullSize, barHeight);

        //change text
        manaText.text = amount.ToString("F0");
    }

    //flash mana bar when there isnt enough mana to cast a spell
    private void FlashManaBar()
    {
        manaBar.color = new Color(0, 0, 0, 1);
        manaBarFlashTimer = .5f;
    }

    /// <summary>
    /// upgrades the chosen stat of the chosen card
    /// </summary>
    /// <param name="name">name of card to upgrade</param>
    /// <param name="stat">stat of card to upgrade: mana, damage</param>
    /// <returns>returns true if it was able to upgrade, returns false if the stat is at max or any other reason no upgrade would ocurr</returns>
    public bool UpgradeCard(string name, string stat)
    {
        Card[] cardArr = cards.ToArray();
        foreach (Card card in cardArr)
        {
            if (card.Name == name.ToLower())
            {
                //perhaps there will be more stats to upgrade later, need to figure out good numbers
                switch (stat.ToLower())
                {
                    case "mana":
                        //reduce mana cost
                        if (card.ManaCost > 1)
                        {
                            card.ManaCost -= 1;
                            return true;
                        }
                        break;

                    case "damage":
                        //increase damage
                        card.Damage += 1;
                        return true;
                        //break;

                    default:
                        break;
                }
            }
        }

        return false;
    }
}
