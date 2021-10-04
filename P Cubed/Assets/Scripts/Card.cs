using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum HitboxShape
{
    Circle,
    Rectangle,
    Ellipse
}

public class Card
{
    //card stuff
    private string name;
    private HitboxShape hitboxType;
    private Vector2 size;
    private float damage;
    private bool isSkillShot;
    private bool isProjectile;
    private float speed;
    private Sprite effectSprite;
    private Vector3 origin;
    private float timeActive;

    //internal stuff
    private bool active;
    private float timeActiveCounter;
    private GameObject currentObj;

    /// <summary>
    /// create a card object
    /// </summary>
    /// <param name="name">name of card</param>
    /// <param name="hitboxType">used for both: targeted can only be a rectangle or a circle, skillshot can also be an ellipse</param>
    /// <param name="size">used for both: width and height of hitbox</param>
    /// <param name="damage">used for both: damage to deal</param>
    /// <param name="isSkillShot">used for both: whether the attack is targeted or a skillshot</param>
    /// <param name="isProjectile">used for skillshot: should it be given the projectile script</param>
    /// <param name="speed">used for skillshot: speed it should travel at</param>
    /// <param name="effectSprite">used for both: sprite to show over hitbox</param>
    /// <param name="origin">used for skillshot: where skillshot should start when it is created</param>
    /// <param name="timeActive">used for skillshot: time hitbox should be active for</param>
    public Card(string name, HitboxShape hitboxType, Vector2 size, float damage, bool isSkillShot, bool isProjectile, float speed, Sprite effectSprite, Vector3 origin, float timeActive)
    {
        this.name = name;
        this.hitboxType = hitboxType;
        this.size = size;
        this.damage = damage;
        this.isSkillShot = isSkillShot;
        this.isProjectile = isProjectile;
        this.speed = speed;
        this.effectSprite = effectSprite;
        this.origin = origin;
        this.timeActive = timeActive;

        active = false;
        timeActiveCounter = 0;
    }

    public void Activate(Vector3 mousePos)
    {
        active = true;
        mousePos = new Vector3(mousePos.x, mousePos.y, 0);

        //skillshot attack that travels to the mouse position
        if (isSkillShot)
        {
            //create skillshot
            currentObj = new GameObject("card");
            currentObj.transform.localScale = new Vector3(size.x, size.y, 1);
            currentObj.transform.position = origin;
            currentObj.AddComponent<SpriteRenderer>().sprite = effectSprite;

            //add collider
            switch (hitboxType)
            {
                case HitboxShape.Circle:
                case HitboxShape.Ellipse:
                    currentObj.AddComponent<CircleCollider2D>().isTrigger = true;
                    break;

                case HitboxShape.Rectangle:
                    currentObj.AddComponent<BoxCollider2D>().isTrigger = true;
                    break;

                default:
                    Debug.Log("how the fuck did default happen, that isnt possible");
                    break;
            }

            //get direction
            Vector3 direction = mousePos - origin;
            //rotate it to match direction
            if (direction != Vector3.zero)
            {
                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                currentObj.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
            }

            //if it is a projectile give it speed and projectile script
            if (isProjectile)
            {
                //currentObj.AddComponent<projectile>().SetValues(direction, speed);
            }

            //set timer
            timeActiveCounter = timeActive;
        }
        //targeted attack that occurs imediately at the mouse position
        else
        {
            Collider2D[] enemiesToDamage = new Collider2D[0];

            //create collider
            switch (hitboxType)
            {
                case HitboxShape.Circle:
                case HitboxShape.Ellipse:
                    enemiesToDamage = Physics2D.OverlapCircleAll(mousePos, size.x);
                    break;

                case HitboxShape.Rectangle:
                    enemiesToDamage = Physics2D.OverlapBoxAll(mousePos, size, 0);
                    break;

                default:
                    Debug.Log("how the fuck did default happen, that isnt possible");
                    break;
            }

            //damage enemies
            foreach (Collider2D enemy in enemiesToDamage)
            {
                //damage enemies
            }

            //create effect object
            currentObj = new GameObject("card");
            currentObj.AddComponent<SpriteRenderer>().sprite = effectSprite;
            currentObj.transform.position = mousePos;
            currentObj.transform.localScale = new Vector3(size.x, size.y, 1);

            //set timer
            timeActiveCounter = 1;
        }
    }

    public void Update()
    {
        timeActiveCounter -= Time.deltaTime;

        if (timeActiveCounter <= 0)
        {
            active = false;

            if (currentObj != null)
            {
                GameObject.Destroy(currentObj);
            }
        }
    }

    public bool Active
    {
        get { return active; }
    }

    public string Name
    {
        get { return name; }
    }
}
