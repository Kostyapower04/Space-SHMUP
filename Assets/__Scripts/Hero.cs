using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hero : MonoBehaviour
{
    static public Hero S; //Одиночка
    [Header("Set in Inspector")]
    //Поля, управляющие движением корабля
    public float speed = 30;
    public float rollMult = -45;
    public float pitchMult = 30;
    public float gameRestartDelay = 0f;
    public GameObject projectilePrefab;
    public float projectileSpeed = 40;
    public Weapon[] weapons;

    [Header("Set Dynamically")]
    [SerializeField]
    private float _shieldLevel = 1;

    //Эта переменная хранит ссылку на последний столкнувшийся игровой объект
    private GameObject lastTriggerGo = null;

    //Объявление нового делегата типа WeaponFiredelegate
    public delegate void WeaponFireDelegate();
    //Создать поле типа WeaponFireDelegate c именем fireDelegate
    public WeaponFireDelegate fireDelegate;

    void Awake()
    {
        if (S == null)
        {
            S = this; //Сохранить ссылку на одиночку
        }

        else
        {
            Debug.LogError("Hero.Awake() - Attempted to assughn second Hero.S!");
        }
        //fireDelegate += TempFire;

    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //Извлечь информацию из класса Input
        float xAsis = Input.GetAxis("Horizontal");
        float yAsis = Input.GetAxis("Vertical");
        //Изменить transform.position, опираясь на информfцию по осям
        Vector3 pos = transform.position;
        pos.x += xAsis * speed * Time.deltaTime;
        pos.y += yAsis * speed * Time.deltaTime;
        transform.position = pos;

        //Повернуть корабль, чтобы придать ощущение динамизма
        transform.rotation = Quaternion.Euler(yAsis * pitchMult, xAsis * rollMult, 0);

        //Позволить кораблю выстрелить
        //if (Input.GetKeyDown(KeyCode.Space))
        //{
        //    TempFire();
        //}

        //Произвести выстрел из всех видов оружия вызовом fireDelegate
        //Сначала проверть нажатие кавиши: Axis("Jump")
        //Затем убедиться, что значение fireDelegate не равно null,
        //Чтобы избежать ошибки
        if (Input.GetAxis("Jump") == 1 && fireDelegate != null)
        {
            fireDelegate();
        }
    }

    void TempFire()
    {
        GameObject projGO = Instantiate<GameObject>(projectilePrefab);
        projGO.transform.position = transform.position;
        Rigidbody rigidB = projGO.GetComponent<Rigidbody>();
        rigidB.velocity = Vector3.up * projectileSpeed;

        Projectile proj = projGO.GetComponent<Projectile>();
        proj.type = WeaponType.blaster;
        float tSpeed = Main.GetWeaponDefinition(proj.type).velocity;
        rigidB.velocity = Vector3.up * tSpeed;
    }
    void OnTriggerEnter(Collider other)
    {
        Transform rootT = other.gameObject.transform.root;
        GameObject go = rootT.gameObject;
        //print("Triggered:" + go.name);

        //Гарантировать невозможность повторного столкновения с тем же объектом
        if (go == lastTriggerGo)
        {
            return;
        }
        lastTriggerGo = go;

        if (go.tag == "Enemy") //Если защитное поле столнулось с вражеским кораблем...
        {
            _shieldLevel--; //...уменьшить уровень защиты на 1...
            Destroy(go); //...и уничтожить врага
        }
        else if (go.tag == "PowerUp")
        {
            //Если защитное поле столкнулось с бонусом
            AbsorbPowerUp(go);
        }
        else
        {
            print("Triggered by non-Enemy: " + go.name);
        }
    }

    public void AbsorbPowerUp(GameObject go)
    {
        PowerUp pu = go.GetComponent<PowerUp>();
        switch (pu.type)
        {
            case WeaponType.shield:
                shieldLevel++;
                break;

            default: 
                if (pu.type == weapons[0].type)
                {
                    // then increase the number of weapons of this type
                    Weapon w = GetEmptyWeaponSlot(); // Find an available weapon
                    if (w != null)
                    {
                        //Установить в pu.type
                        w.SetType(pu.type);
                    }
                }
                else
                {
                    //Если оружие другого типа
                    ClearWeapons();
                    weapons[0].SetType(pu.type);
                }
                break;
        }
        pu.AbsorbedBy(this.gameObject);
    }

    public float shieldLevel
    {
        get
        {
            return (_shieldLevel);
        }
        set
        {
            _shieldLevel = Mathf.Min(value, 4);
            //Если уровень поля упал ниже нуля
            if (value < 0)
            {
                Destroy(this.gameObject);
                //Сообщить объекту Main.S о необходимости перезапустить игру
                Main.S.DelayedRestart(gameRestartDelay);
            }
        }
    }

    Weapon GetEmptyWeaponSlot()
    {
        for (int i = 0; i < weapons.Length; i++)
        {
            if (weapons[i].type == WeaponType.none)
            {
                return (weapons[i]);
            }
        }
        return (null);
    }

    void ClearWeapons()
    {
        foreach (Weapon w in weapons)
        {
            w.SetType(WeaponType.none);
        }
    }
}



