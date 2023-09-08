using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainingRoom : MonoBehaviour
{
    [SerializeField] GameObject Oak;

    public GameObject trainingOak;
    public Transform spawnTransform;

    Enemy enemy;

    bool isSpawningEnemy;
    [SerializeField] bool isTrainingEnemy;
    [SerializeField] bool isEnemyHp;

    // Start is called before the first frame update
    void Start()
    {
        trainingOak = Instantiate(Oak, spawnTransform.position, Quaternion.LookRotation(Vector3.left));
        enemy = trainingOak.GetComponent<Enemy>();
        enemy.isTraining = isTrainingEnemy;
        enemy.HpFull = isEnemyHp;
    }

    // Update is called once per frame
    void Update()
    {
        if(enemy.isDead && !isSpawningEnemy){
            Debug.Log("Enemy Dead");
            StartCoroutine(SpawnEnemy());
        }
    }

    IEnumerator SpawnEnemy(){
        isSpawningEnemy = true;
        yield return new WaitForSeconds(2f);
        trainingOak = Instantiate(Oak, spawnTransform.position, Quaternion.LookRotation(Vector3.left));
        enemy = trainingOak.GetComponent<Enemy>();
        enemy.isTraining = isTrainingEnemy;
        enemy.HpFull = isEnemyHp;
        isSpawningEnemy = false;
    }
}
