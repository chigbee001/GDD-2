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
    private float timeActive;

    //internal stuff
    private bool active;
    private float timeActiveCounter;
    private GameObject currentObj;
    private LayerMask enemyLayer;

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
    /// <param name="timeActive">used for skillshot: time hitbox should be active for, used for both: time before next attack can be used</param>
    public Card(string name, HitboxShape hitboxType, Vector2 size, float damage, bool isSkillShot, bool isProjectile, float speed, Sprite effectSprite, float timeActive)
    {
        this.name = name;
        this.hitboxType = hitboxType;
        this.size = size;
        this.damage = damage;
        this.isSkillShot = isSkillShot;
        this.isProjectile = isProjectile;
        this.speed = speed;
        this.effectSprite = effectSprite;
        this.timeActive = timeActive;

        active = false;
        timeActiveCounter = 0;
        enemyLayer = LayerMask.NameToLayer("enemy");
    }

    /// <summary>
    /// active card, spawn hitbox and display effect
    /// </summary>
    /// <param name="mousePos">mouse position in world</param>
    /// <param name="origin">if skillshot, where should it start</param>
    public void Activate(Vector3 mousePos, Vector3 origin)
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
            currentObj.tag = "pBullet";

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

            Projectile proj = currentObj.AddComponent<Projectile>();
            proj.direction = direction;
            proj.speed = speed;

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
                //damage enemies (eventually enemies will be able to take damage but for now we have this)
                if (enemy.tag == "Enemy")
                {
                    //edited to damage enemy otherwise enemies will no longer spawn due to how enemies are handled
                    enemy.GetComponentInParent<Enemy>().TakeDamage(1);
                    //GameObject.Destroy(enemy.gameObject);
                }
            }

            //create effect object
            currentObj = new GameObject("card");
            currentObj.AddComponent<SpriteRenderer>().sprite = effectSprite;
            currentObj.transform.position = mousePos;
            currentObj.transform.localScale = new Vector3(size.x, size.y, 1);

            //set timer
            timeActiveCounter = timeActive;
        }
    }

    /// <summary>
    /// update card gameobject every time it is called. counts down time active and destroys itself at the end
    /// </summary>
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

    /// <summary>
    /// is the card active
    /// </summary>
    public bool Active
    {
        get { return active; }
    }

    /// <summary>
    /// name of card
    /// </summary>
    public string Name
    {
        get { return name; }
    }
}
