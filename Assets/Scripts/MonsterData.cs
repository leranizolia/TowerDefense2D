using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//Так мы создаём MonsterLevel.
//В нём группируются цена (в золоте, которое мы будем поддерживать ниже)
//и визуальное представление уровня монстра.

//Мы добавляем сверху [System.Serializable], чтобы экземпляры класса можно было изменять в инспекторе.
//Это позволяет нам быстро менять все значения класса Level, даже когда игра запущена.
//Это невероятно полезно для балансировки игры.
[System.Serializable]
public class MonsterLevel
{
    public int cost;
    public GameObject visualization;

    // зададим префаб снаряда и частота стрельбы для каждого уровня монстров
    public GameObject bullet;
    public float fireRate;
}

public class MonsterData : MonoBehaviour
{
    public List<MonsterLevel> levels;
    private MonsterLevel currentLevel;

    //Задаём свойство частной переменной currentLevel.
    //Задав свойство, мы сможем вызывать её как любую другую переменную: или как CurrentLevel (внутри класса)
    //или как monster.CurrentLevel (за его пределами).
    //Мы можем определить в методе геттера или сеттера свойства любое поведение,
    //а создавая только геттер, сеттер или их обоих, можно управлять характеристиками свойства: только для чтения, только для записи и для записи/чтения.
    public MonsterLevel CurrentLevel
    {
        //В геттере мы возвращаем значение currentLevel
        get
        {
            return currentLevel;
        }
        //В сеттере мы присваиваем currentLevel новое значение.
        //Затем мы получаем индекс текущего уровня.
        //Наконец, мы проходим в цикле по всем уровням и включаем/отключаем визуальное отображение в зависимости от currentLevelIndex.
        //Это отлично, потому что при изменении currentLevel спрайт обновляется автоматически.
        set
        {
            currentLevel = value;
            int currentLevelIndex = levels.IndexOf(currentLevel);

            GameObject levelVisualization = levels[currentLevelIndex].visualization;
            for (int i = 0; i < levels.Count; i++)
            {
                if (levelVisualization != null)
                {
                    if (i == currentLevelIndex)
                    {
                        levels[i].visualization.SetActive(true);
                    }
                    else
                    {
                        levels[i].visualization.SetActive(false);
                    }
                }
            }
        }
    }

    //Здесь мы при размещении задаём CurrentLevel.
    //Это гарантирует, что будет показан только нужный спрайт.
    private void OnEnable()
    {
        CurrentLevel = levels[0];
    }

    //Примечание: важно инициализировать свойство в OnEnable, а не в OnStart, потому что мы вызываем порядковые методы при создании экземпляров префабов.
    //OnEnable будет вызван сразу же при создании префаба(если префаб был сохранён в состоянии enabled), но OnStart не вызывается, пока объект не начинает выполняться как часть сцены.
    //Нам необходимо проверять эти данные до размещения монстра, поэтому мы инициализируем их в OnEnable


    //метод, чтобы узнать, возможен ли апгрейд монстра
    public MonsterLevel GetNextLevel()
    {
        int currentLevelIndex = levels.IndexOf(currentLevel);
        int maxLevelIndex = levels.Count - 1;
        if (currentLevelIndex < maxLevelIndex)
        {
            return levels[currentLevelIndex];
        }
        else
        {
            return null;
        }
    }

    public void IncreaseLevel()
    {
        int currentLevelIndex = levels.IndexOf(CurrentLevel);
        if (currentLevelIndex < levels.Count - 1)
        {
            CurrentLevel = levels[currentLevelIndex + 1];
        }
    }
}
