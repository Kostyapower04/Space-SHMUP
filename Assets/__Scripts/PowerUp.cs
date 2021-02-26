using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUp : MonoBehaviour
{
    [Header("Set in inspector")]
    //Необычное, но удобное применение Vector2. 
    //Х хранит минимальное значение, а у - максимальное ддля метода Random.Range(), который будет вызван позже.
    public Vector2 rotMinMax = new Vector2(15, 90);
    public Vector2 driftMinMax = new Vector2(.25f, 2);
    public float lifeTime = 6f; //Время в секундах существования PowerUp
    public float fadeTime = 4f; //Секунды, когда он исчезнет

    [Header("Set Dynamically")]
    public WeaponType type; //Тип бонуса
    public GameObject cube; //Ссылка на вложенный куб
    public TextMesh letter; //Ссылка TextMesh
    public Vector3 rotPerSecond; //Скорость вращения
    public float birthTime;

    private Rigidbody rigid;
    private BoundsCheck bndCheck;
    private Renderer cubeRend;

    void Awake()
    {
        //Получить ссылку на куб
        cube = transform.Find("Cube").gameObject;
        //Получить ссылки на TextMesh и другие компоненты
        letter = GetComponent<TextMesh>();
        rigid = GetComponent<Rigidbody>();
        bndCheck = GetComponent<BoundsCheck>();
        cubeRend = GetComponent<Renderer>();

        //Выбрать случайную скорость
        Vector3 vel = Random.onUnitSphere; // Получить случайную скорость XYZ
                                           // Random.onUnitSphere возвращает вектор, указывающий на случайную точку, находящуюся на поверхности сферы с радиусом 1 и 
                                           //с центром в начале координат
        vel.z = 0; //Отобразить vel на плоскость XY
        vel.Normalize(); //Нормализация устанавливает длину Vector3 равной 1 м
        vel *= Random.Range(driftMinMax.x, driftMinMax.y);
        rigid.velocity = vel;

        //Установить угол поворота этого игрового объекта равным R:[0, 0, 0]
        transform.rotation = Quaternion.identity;
        // Quaternion.identity равноценно отсутствию поворота
        //Выбрать случайную скорость вращения для вложенного куба с использованием  rotMinMax.x и rotMinMax.y
        rotPerSecond = new Vector3(Random.Range(rotMinMax.x, rotMinMax.y),
        Random.Range(rotMinMax.x, rotMinMax.y),
        Random.Range(rotMinMax.x, rotMinMax.y));

        birthTime = Time.time;
    }
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        cube.transform.rotation = Quaternion.Euler(rotPerSecond * Time.time);
        //Эффект растворения куба с течением времени
        //Со значениями по умолчанию бонус существует 10 секунд
        //А затем растворяется 4 секунды.
        float u = (Time.time - (birthTime + lifeTime)) / fadeTime;
        //В течении lifeTime секунд значение u будет <= 0. Потом станет положительным и через fadeTime станет больше 1
        //Если u >= 1, Уничтожить бонус
        if (u >= 1)
        {
            Destroy(this.gameObject);
            return;
        }

        //Использовать u для определения альфа-значение=я куба и буквы
        if (u > 0)
        {
            Color c = cube.GetComponent<Renderer>().material.color;
            c.a = 1f - u;
            cube.GetComponent<Renderer>().material.color = c;
            //Бука тоже должна растворяться, но медленнее
            c = letter.color;
            c.a = 1f - (u * 0.5f);
            letter.color = c;
        }
        if (!bndCheck.isOnScreen)
        {
            //Если бонус полностью вышел за границу экрана, уничтожить его
            Destroy(gameObject);
        }

    }
    public void SetType(WeaponType wt)
    {
        //Получить WeaponDefinition из Main
        WeaponDefinition def = Main.GetWeaponDefinition(wt);
        //Установить цвет дочернего куба
        cube.GetComponent<Renderer>().material.color = def.color;
        //letter.color = def.color; //Букву тоже надо окрасить в тот же цвет
        letter.text = def.letter; //Устанвить отображаемую букву
        type = wt; //В заключение установить фактический тип
    }
    public void AbsorbedBy(GameObject target)
    {
        //Эта функция реализуется классом Hero, когда игрок подбирает бонус
        //Можно было бы реализовать эффект полного поглощения бонуса, уменьшая его
        //размеры в течении нескольких секунд, но пока просто уничтожим
        //this.gameObject
        Destroy(this.gameObject);
    }
}