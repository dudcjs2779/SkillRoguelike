using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainingRoomTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerInputControls.Instance.ChnageMapTrainingEnter();

            foreach (var item in GameManager.Instance.EquipActiveList)
            {
                item.skillLv = 1;
            }

        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerInputControls.Instance.ChangeMapStartMap();

            foreach (var item in GameManager.Instance.EquipActiveList)
            {
                item.skillLv = 0;
            }
        }
    }
}
