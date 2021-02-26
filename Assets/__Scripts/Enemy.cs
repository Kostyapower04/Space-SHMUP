using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Set in Inspector: Enemy")]
    public float speed = 30f; //Скорость м/с
    public float fireRate = 0.3f; //Секунд между выстрелами (не используется)
    public float health = 10;
    public int score = 100; //Очки за уничтожение этого корабля
    public float showDamageDuration = 0.1f; //Длительность эффекта попадания в секундах
    public float powerUpDropChance = 1f; //Вероятность сбросить бонус

    [Header("Set Dynamically: Enemy")]
    public Color[] originalColors;
    public Material[] materials; //Все материалы игрового объекта и его потомков
    public bool showingDamage = false;
    public float damageDoneTime; //Время прекращения отображения эффекта
    public bool notifiedOfDestruction = false; 

    protected BoundsCheck bndCheck;

    void Awake()
    {
        bndCheck = GetComponent<BoundsCheck>();
        materials = Utils.GetAllMaterials(gameObject);
        originalColors = new Color[materials.Length];
        for (int i = 0; i < materials.Length; i++)
        {
            originalColors[i] = materials[i].color;
        }
    }

    //Это свойство: метод, описывающий как поле
    public Vector3 pos
    {
        get
        {
            return (this.transform.position);
        }
        set
        {
            this.transform.position = value;
        }
    }
    // Start is called before the first frame update
    void Start()
    {
   
    }

    // Update is called once per frame
    void Update()
    {
        Move();

        if (showingDamage = true && Time.time > damageDoneTime)
        {
            UnShowDamage();
        }

        if (bndCheck != null && !bndCheck.isOnScreen)
        {
            //Убедиться, что корабль вышел за нижнюю границу экрана
            if(pos.y < bndCheck.camHeight - bndCheck.radius)
            {
                //Корабль за нижней границей, поэтому его нужно уничтожить
                Destroy(gameObject);

            }
        }
    }

    void UnShowDamage()
    {
        for (int i = 0; i < materials.Length; i++)
        {
            materials[i].color = originalColors[i];
        }
        showingDamage = false;
    }

    public virtual void Move()
    {
        Vector3 tempPos = pos;
        tempPos.y -= speed * Time.deltaTime;
        pos = tempPos;
    }

    void OnCollisionEnter(Collision coll)
    {
        GameObject otherGO = coll.gameObject;

        switch (otherGO.tag)
        {
            case "ProjectileHero":
                Projectile p = otherGO.GetComponent<Projectile>();
                //Еcли враг за границами экрана, не наносить ему повреждений
                if (!bndCheck.isOnScreen)
                {
                    Destroy(otherGO);
                    break;
                }

                //Поразить вражеский корабль
                ShowDamage();
                //Получить разрушающую из WEAP_DICT в классе Main
                health -= Main.GetWeaponDefinition(p.type).damageOnHit;
                if (health <= 0)
                {
                    //Сообщить объекту одиночке Main об уничтожении
                    if (!notifiedOfDestruction)
                    {
                        Main.S.ShipDestroyed(this);
                    }
                    notifiedOfDestruction = true;
                    //Уничтожить этот вражеский корабль
                    Destroy(this.gameObject);
                }
                Destroy(otherGO);
                break;
            default:
                print("Enemy hit by non-ProjectileHero: " + otherGO.name);
                break;
        }

        void ShowDamage()
        {
            foreach (Material m in materials)
            {
                m.color = Color.red;
            }
            showingDamage = true;
            damageDoneTime = Time.time + showDamageDuration;

        }
    }
}

