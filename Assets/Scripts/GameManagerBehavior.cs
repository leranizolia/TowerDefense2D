using UnityEngine;
using UnityEngine.UI;

public class GameManagerBehavior : MonoBehaviour
{
    public Text goldLabel;
    private int gold;
    public Text waveLabel;
    public GameObject[] nextWaveLabels;
    public bool gameOver = false;
    private int wave;
    public Text healthLabel;
    public GameObject[] healthIndicator;
    private int health;

    public int Gold
    {
        get
        {
            return gold;
        }
        set
        {
            gold = value;
            goldLabel.GetComponent<Text>().text = "GOLD" + gold;
        }
    }

    public int Wave
    {
        get
        {
            return wave;
        }
        set
        {
            wave = value;
            if (!gameOver)
            {
                //обходим в цикле все метки nextWaveLabels — у этих меток есть компонент Animator.
                //Чтобы задействовать анимацию Animator мы задаём триггер nextWave
                for (int i = 0; i < nextWaveLabels.Length; i++)
                {
                    nextWaveLabels[i].GetComponent<Animator>().SetTrigger("nextWave");
                }
            }
            waveLabel.text = "WAVE: " + (wave + 1);
        }
    }

    public int Health
    {
        get
        {
            return health;
        }
        set
        {
            //1
            if (value < health)
            {
                Camera.main.GetComponent<CameraShake>().Shake();
            }
            //Обновляем частную переменную и метку здоровья в верхнем левом углу экрана.
            health = value;
            healthLabel.text = "HEALTH: " + health;
            //Обновляем частную переменную и метку здоровья в верхнем левом углу экрана.
            if (health <= 0 &&  !gameOver)
            {
                gameOver = true;
                GameObject gameOverText = GameObject.FindGameObjectWithTag("GameOver");
                gameOverText.GetComponent<Animator>().SetBool("gameOver", true);
            }
            //Убираем одного из монстров с печенья. Если мы просто отключаем их, то эту часть можно написать проще,
            //но здесь мы поддерживаем повторное включение на случай добавления здоровья
            for (int i = 0; i < healthIndicator.Length; i++)
            {
                if (i < Health)
                {
                    healthIndicator[i].SetActive(true);
                }
                else
                {
                    healthIndicator[i].SetActive(false);
                }
            }
        }
    }

    private void Start()
    {
        Gold = 1000;
        Wave = 0;
        Health = 5;
    }
}
