using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; //Для загрузки и перезагрузки сцен

public class Main : MonoBehaviour
{
    static public Main S; //Объект-одиночка Main
    static Dictionary<WeaponType, WeaponDefinition> WEAP_DICT;
    [Header("Set in Inspector")]
    public GameObject[] prefabEnemies; //Массив шаблонов Enemy 
    public float enemySpawnPerSecond = 0.5f; //Вражеских кароблей в секунду

    public float enemyDefaultPadding = 1.5f; //Отступ для позиционирования

    public WeaponDefinition[] weaponDefinitions;
    public GameObject prefabPowerUp;
    public WeaponType[] powerUpFrequency = new WeaponType[] {
                                            WeaponType.blaster, WeaponType.blaster,
                                            WeaponType.spread,
                                            WeaponType.shield };

    private BoundsCheck bndcheck;

    public void ShipDestroyed(Enemy e)
    {
        //Сгенерировать бонус с заданной вероятностью
        if (Random.value <= e.powerUpDropChance)
        {
            //Выбрать тип бонуса
            //Выбрать один из элементов powerUpFrequency
            int ndx = Random.Range(0, powerUpFrequency.Length);
            WeaponType puType = powerUpFrequency[ndx];
            //Создать экземпляр PowerUp
            GameObject go = Instantiate(prefabPowerUp) as GameObject;
            PowerUp pu = go.GetComponent<PowerUp>();
            //Установить соответствующий тип WeaponType
            pu.SetType(puType);
            //Поместить в место, где находился разрушенный корабль
            pu.transform.position = e.transform.position;
        }
    }

    void Awake()
    {
        S = this;
        //Записать в bndCheck ссылку на компонент BoundsCheck этого игрового объекта
        bndcheck = GetComponent<BoundsCheck>();
        //Вызывать SpawnEnemy() один раз (в 2 секунды при значениях по умолчанию)
        Invoke("SpawnEnemy", 1f / enemySpawnPerSecond);

        //Словарь с ключами типа WeaponType
        WEAP_DICT = new Dictionary<WeaponType, WeaponDefinition>();
        foreach (WeaponDefinition def in weaponDefinitions)
        {
            WEAP_DICT[def.type] = def;
        }
    }

    public void SpawnEnemy()
    {
        //Выбрать случайный шаблон Enemy для создания
        int ndx = Random.Range(0, prefabEnemies.Length);
        GameObject go = Instantiate<GameObject>(prefabEnemies[ndx]);

        //Разместить вражеский корабль над экраном в случайной позиции x
        float enemyPadding = enemyDefaultPadding;
        if (go.GetComponent<BoundsCheck>() != null)
        {
            enemyPadding = Mathf.Abs(go.GetComponent<BoundsCheck>().radius);
        }

        //Установить начальные кординаты созданного вражеского корабля
        Vector3 pos = Vector3.zero;
        float xMin = -bndcheck.camWidth + enemyPadding;
        float xMax = bndcheck.camWidth - enemyPadding;
        pos.x = Random.Range(xMin, xMax);
        pos.y = bndcheck.camHeight + enemyPadding;
        go.transform.position = pos;

        //Снова вызвать SpawnEnemy()
        Invoke("SpawnEnemy", 1f / enemySpawnPerSecond);
    }

    public void DelayedRestart(float delay)
    {
        //Вызвать метод Restart() через delay секунд
        Invoke("Restart", delay);
    }

    public void Restart()
    {
        //Перезагрузить _Scene_0 чтобы перезапустить игру
        SceneManager.LoadScene("_Scene_0");
    }
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    static public WeaponDefinition GetWeaponDefinition(WeaponType wt)
    {
        //Проверить наличие указанного ключа в словаре
        //Попытка извлечь значение по отсутствующему ключу вышовет ошибку,
        //поэтому следующая инструкция играет важную роль
        if (WEAP_DICT.ContainsKey(wt))
        {
            return (WEAP_DICT[wt]);
        }
        //Следующая инструкция возвращает новый экземпляр WeaponDefinition
        //С типом оружия WeaponType.none, что означает неудачную попытку 
        //найти требуемое определение WeaponDefinition
        return (new WeaponDefinition());
    }
}
