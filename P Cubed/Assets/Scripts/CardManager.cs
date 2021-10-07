using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardManager : MonoBehaviour
{
    private Queue<Card> cards = new Queue<Card>();
    private Card activeCard = null;
    public Sprite[] cardEffects = new Sprite[5];
    public Transform cardDisplay;
    public GameObject cardUIPrefab;
    private GameManager gameManager;
    private float cardUIOffset;

    // Start is called before the first frame update
    void Start()
    {
        //create cards
        cards.Enqueue(new Card("Rupture", HitboxShape.Rectangle, new Vector2(2, 2), 2, false, false, 0, cardEffects[0], .5f));
        cards.Enqueue(new Card("Fireball", HitboxShape.Ellipse, new Vector2(.75f, .5f), 3, true, true, 10, cardEffects[1], .75f));
        cards.Enqueue(new Card("Meteor", HitboxShape.Circle, new Vector2(4, 4), 4, false, false, 0, cardEffects[2], 1.5f));
        cards.Enqueue(new Card("Lightning", HitboxShape.Rectangle, new Vector2(3, .25f), 5, true, true, 15, cardEffects[3], .5f));
        cards.Enqueue(new Card("Sun Disc", HitboxShape.Circle, new Vector2(1, 1), 6, true, true, 8, cardEffects[4], .75f));

        //set up card display
        cardUIOffset = -1 * (cardUIPrefab.GetComponent<RectTransform>().rect.height + 10);
        Card[] cardArr = cards.ToArray();
        for(int i = 0; i < cardArr.Length; i++)
        {
            GameObject newCardUI = Instantiate(cardUIPrefab, cardDisplay);
            newCardUI.GetComponent<RectTransform>().localPosition = new Vector3(0, (cardArr.Length - 1 - i) * cardUIOffset, 0);
            newCardUI.transform.GetChild(0).GetComponent<Text>().text = cardArr[i].Name;
        }

        //get reference to gamemanager
        gameManager = FindObjectOfType<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if(gameManager.IsPaused) 
        {
            return;
        }

        if (Input.GetMouseButtonDown(0) && cards.Count > 0 && activeCard == null)
        {
            //activate card
            cards.Peek().Activate(GetMousePosInWorld(Input.mousePosition), transform.position);

            //remove activated card from top, put it on the bottom
            activeCard = cards.Dequeue();
            cards.Enqueue(activeCard);

            //update card display
            UpdateCardDisplay(activeCard);
        }

        if(Input.GetMouseButtonDown(1) && cards.Count > 1 && activeCard == null)
        {
            //get the first card
            Card firstCard = cards.Dequeue();

            //activate card
            cards.Peek().Activate(GetMousePosInWorld(Input.mousePosition), transform.position);

            activeCard = cards.Dequeue();

            //send first card to the back so it ends up back in front after for
            cards.Enqueue(firstCard);

            //cycle all cards other than first one and active one
            for(int i = 0; i < cards.Count - 1; i++)
            {
                cards.Enqueue(cards.Dequeue());
            }

            //send active card to the back
            cards.Enqueue(activeCard);

            //update card display
            UpdateCardDisplay(activeCard);
        }

        if (activeCard != null)
        {
            activeCard.Update();

            if (!activeCard.Active)
            {
                activeCard = null;
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
            newCardUI.transform.GetChild(0).GetComponent<Text>().text = cardArr[i].Name;
        }
    }
}
