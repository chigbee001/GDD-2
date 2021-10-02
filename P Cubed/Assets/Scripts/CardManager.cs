using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardManager : MonoBehaviour
{
    private Queue<Card> cards = new Queue<Card>();
    private Card activeCard = null;
    public Sprite[] cardEffects = new Sprite[5];

    // Start is called before the first frame update
    void Start()
    {
        cards.Enqueue(new Card(HitboxShape.Rectangle, new Vector2(2, 2), 3, false, false, 0, cardEffects[0], Vector3.zero, 1));
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0) && cards.Count > 0)
        {
            //activate card
            cards.Peek().Activate(GetMousePosInWorld(Input.mousePosition));

            //remove activated card from top, put it on the bottom
            activeCard = cards.Dequeue();
            cards.Enqueue(activeCard);
        }

        if(Input.GetMouseButtonDown(1) && cards.Count > 1){
            //get the first card
            Card firstCard = cards.Dequeue();

            //activate card
            cards.Peek().Activate(GetMousePosInWorld(Input.mousePosition));

            activeCard = cards.Dequeue();

            //send first card to the back so it ends up back in front after for
            cards.Enqueue(firstCard);

            //cycle all cards other than first one and active one
            for(int i = 0; i < cards.Count - 2; i++)
            {
                cards.Enqueue(cards.Dequeue());
            }

            //send active card to the back
            cards.Enqueue(activeCard);
        }

        if (activeCard != null)
        {
            activeCard.Update();
        }
    }

    //get mouse position in world coords
    private Vector3 GetMousePosInWorld(Vector3 inputPos)
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = 10;
        return Camera.main.ScreenToWorldPoint(mousePos);
    }
}
