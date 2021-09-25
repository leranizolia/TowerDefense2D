using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaceMonster : MonoBehaviour
{
    public GameObject monsterPrefab;
    private GameObject monster;
    private GameManagerBehavior gameManager;

    private void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManagerBehavior>();
    }
    //Примечание: можно сделать это, задав поле в редакторе Unity или добавив статический метод в GameManager,
    //возвращающий экземпляр синглтона, из которого мы можем получить GameManagerBehavior.
    //Однако в показанном выше блоке кода есть тёмная лошадка:
    //метод Find, который медленнее работает в процессе выполнения приложения;
    //но он удобен и его можно применять в умеренных количествах.


    //Чтобы на одну точку можно было поставить только одного монстра
    private bool CanPlaceMonster()
    {
        int cost = monsterPrefab.GetComponent<MonsterData>().levels[0].cost;
        return monster == null && gameManager.Gold >= cost;
    }

    //Unity автоматически вызывает OnMouseUp, когда игрок касается физического коллайдера GameObject
    private void OnMouseUp()
    {
        //При вызове этот метод ставит монстра, если CanPlaceMonster() возвращает true
        if (CanPlaceMonster())
        {
            //Мы создаём монстра с помощью метода Instantiate, который создаёт экземпляр заданного префаба с указанной позицией и поворотом.
            //В данном случае мы копируем monsterPrefab, задаём ему текущую позицию GameObject и отсутствие поворота,
            //передаём результат в GameObject и сохраняем его в monster
            monster = (GameObject)
                Instantiate(monsterPrefab, transform.position, Quaternion.identity);
            //В конце мы вызываем PlayOneShot для воспроизведения звукового эффекта,
            //прикреплённого к компоненту AudioSource объекта
            AudioSource audioSource = gameObject.GetComponent<AudioSource>();
            audioSource.PlayOneShot(audioSource.clip);
            gameManager.Gold -= monster.GetComponent<MonsterData>().CurrentLevel.cost;
        }
        else if (CanUpgradeMonster())
        {
            monster.GetComponent<MonsterData>().IncreaseLevel();
            AudioSource audioSource = gameObject.GetComponent<AudioSource>();
            audioSource.PlayOneShot(audioSource.clip);
            gameManager.Gold -= monster.GetComponent<MonsterData>().CurrentLevel.cost;
        }
    }

    private bool CanUpgradeMonster()
    {
        if (monster != null)
        {
            MonsterData monsterData = monster.GetComponent<MonsterData>();
            MonsterLevel nextLevel = monsterData.GetNextLevel();
            if (nextLevel != null && gameManager.Gold >= nextLevel.cost)
            {
                return true;
            }
        }
        return false;
    }
}
