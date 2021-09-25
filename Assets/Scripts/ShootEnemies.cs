using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootEnemies : MonoBehaviour
{
    //переменную для слежения за всеми врагами в радиусе действия
    public List<GameObject> enemiesInRange;

    //переменные отслеживают время последнего выстрела монстра, а также структуру MonsterData,
    //которая содержит информацию о типе снарядов монстра, частоту стрельбы
    private float lastShotTime;
    private MonsterData monsterData;

    private void Start()
    {
        lastShotTime = Time.time;
        monsterData = gameObject.GetComponentInChildren<MonsterData>();
        enemiesInRange = new List<GameObject>();
    }

    //В OnEnemyDestroy мы удаляем врага из enemiesInRange.
    //Когда враг наступает на триггер вокруг монстра, вызывается OnTriggerEnter2D
    void OnEnemyDestroy(GameObject enemy)
    {
        enemiesInRange.Remove(enemy);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        //Затем мы добавляем врага в список enemiesInRange и добавляем EnemyDestructionDelegate событие OnEnemyDestroy.
        //Так мы гарантируем, что при уничтожении врага будет вызвано OnEnemyDestroy.
        if (other.gameObject.tag.Equals("Enemy"))
        {
            enemiesInRange.Add(other.gameObject);
            EnemyDestructionDelegate del =
                other.gameObject.GetComponent<EnemyDestructionDelegate>();
            del.enemyDelegate += OnEnemyDestroy;
        }
    }

    //В OnTriggerExit2D мы удаляем врага из списка и разрегистрируем делегата.
    void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.tag.Equals("Enemy"))
        {
            enemiesInRange.Remove(other.gameObject);
            EnemyDestructionDelegate del =
                other.gameObject.GetComponent<EnemyDestructionDelegate>();
            del.enemyDelegate -= OnEnemyDestroy;
        }
    }

    void Shoot(Collider2D target)
    {
        GameObject bulletPrefab = monsterData.CurrentLevel.bullet;
        //Получаем начальную и целевую позиции пули.
        //Задаём позицию z равной z bulletPrefab.
        //Раньше мы задали позицию префаба снаряда по z таким образом, чтобы снаряд появлялся под стреляющим монстром, но над врагами
        Vector3 startPosition = gameObject.transform.position;
        Vector3 targetPosition = target.transform.position;
        startPosition.z = bulletPrefab.transform.position.z;
        targetPosition.z = bulletPrefab.transform.position.z;

        //Создаём экземпляр нового снаряда с помощью bulletPrefab соответствующего MonsterLevel.
        //Назначаем startPosition и targetPosition снаряда
        GameObject newBullet = (GameObject)Instantiate(bulletPrefab);
        newBullet.transform.position = startPosition;
        BulletBehavior bulletComp = newBullet.GetComponent<BulletBehavior>();
        bulletComp.target = target.gameObject;
        bulletComp.startPosition = startPosition;
        bulletComp.targetPosition = targetPosition;

        //когда монстр стреляет, запускаем анимацию стрельбы и воспроизводим звук лазера
        Animator animator =
            monsterData.CurrentLevel.visualization.GetComponent<Animator>();
        animator.SetTrigger("fireShot");
        AudioSource audioSource = gameObject.GetComponent<AudioSource>();
        audioSource.PlayOneShot(audioSource.clip);
    }

    private void Update()
    {
        GameObject target = null;
        //Определяем цель монстра. Начинаем с максимально возможного расстояния в minimalEnemyDistance.
        //Обходим в цикле всех врагов в радиусе действия и делаем врага новой целью,
        //если его расстояние до печеньки меньше текущего наименьшего.
        float minimalEnemyDistance = float.MaxValue;
        foreach (GameObject enemy in enemiesInRange)
        {
            float distanceToGoal = enemy.GetComponent<MoveEnemy>().DistanceToGoal();
            if (distanceToGoal < minimalEnemyDistance)
            {
                target = enemy;
                minimalEnemyDistance = distanceToGoal;
            }
        }
        //Вызываем Shoot, если прошедшее время больше частоты стрельбы монстра
        //и задаём lastShotTime значение текущего времени.
        if (target != null)
        {
            if (Time.time - lastShotTime > monsterData.CurrentLevel.fireRate)
            {
                Shoot(target.GetComponent<Collider2D>());
                lastShotTime = Time.time;
            }
            //Вычисляем угол поворота между монстром и его целью.
            //Выполняем поворот монстра на этот угол.
            //Теперь он всегда будет смотреть на цель.
            Vector3 direction = gameObject.transform.position - target.transform.position;
            gameObject.transform.rotation = Quaternion.AngleAxis(
                Mathf.Atan2(direction.y, direction.x) * 180 / Mathf.PI,
                new Vector3(0, 0, 1));
        }
    }
}
