using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BossCutScene : MonoBehaviour
{
    [SerializeField] private List<GameObject> cutObj;
    private float cutMoveTimer;
    private bool cutChage = false;

    private void FixedUpdate()
    {
        cutMoveTimer += Time.deltaTime;

        cutMove();
    }

    private void cutMove()
    {
        if (cutMoveTimer  <= 0.2f)
        {
            cutObj[0].transform.position -= new Vector3(120, 0, 0) * Time.deltaTime;
        }
        else if (cutMoveTimer > 0.1f && cutMoveTimer <= 0.4f)
        {
            cutObj[1].transform.position -= new Vector3(120, 0, 0) * Time.deltaTime;
        }
        else if (cutMoveTimer > 0.2f && cutMoveTimer <= 0.6f)
        {
            cutObj[2].transform.position -= new Vector3(120, 0, 0) * Time.deltaTime;
        }
        else if (cutMoveTimer > 0.6f && cutMoveTimer <= 0.7f)
        {
            cutObj[3].transform.position -= new Vector3(120, 0, 0) * Time.deltaTime;
        }
        else if (cutMoveTimer >= 1.5f && cutChage == false)
        {
            SceneManager.LoadSceneAsync("BossStage");
            cutChage = true;
        }
    }
}
