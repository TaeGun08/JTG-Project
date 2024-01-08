using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tutorial : MonoBehaviour
{
    private KeyManager keyManager;

    [Header("튜토리얼 교관의 말풍선 및 이동 제어")]
    [SerializeField, Tooltip("말풍선")] private List<GameObject> conversations;
    [SerializeField, Tooltip("자신을 무시하면 뜨는 말풍선")] private GameObject dontConversations;
    [SerializeField, Tooltip("플레이어 전진을 막는 말풍선")] private GameObject Barricade;
    [SerializeField, Tooltip("상호작용할 키 이미지")] private GameObject interactionKeyImage;
    [SerializeField, Tooltip("무시 말풍선을 뜨게 하기 위한 체크콜라이더")] private BoxCollider2D dontConversationsCheckColl;

    private int conversationsCount = 0;
    private bool conversationing = false;
    private bool conversationsEnd = false;
    private bool talking = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            talking = true;

            conversationing = true;
            if (conversationing == true && conversationsEnd == false)
            {
                dontConversations.SetActive(false);
                interactionKeyImage.SetActive(true);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            talking = false;

            if (conversationing == true && conversationsEnd == false)
            {
                interactionKeyImage.SetActive(false);
            }
            else if (conversationsEnd == true)
            {
                int count = conversations.Count;
                for (int i = 0; i < count; i++)
                {
                    conversations[i].SetActive(false);
                }

                Destroy(gameObject, 2);
            }
        }
    }

    private void onTriggerCheck(Collider2D _collision)
    {
        if (_collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            if (Barricade.activeSelf == true)
            {
                dontConversations.SetActive(true);
            }
        }
    }

    private void Start()
    {
        keyManager = KeyManager.instance;

        interactionKeyImage.SetActive(false);
    }

    private void Update()
    {
        collCheck();
        interaction();
        npcEndMove();
    }

    private void collCheck()
    {
        Collider2D collCheck = Physics2D.OverlapBox(dontConversationsCheckColl.bounds.center,
            dontConversationsCheckColl.bounds.size, 0f, LayerMask.GetMask("Player"));

        if (collCheck != null)
        {
            onTriggerCheck(collCheck);
        }
        else
        {
            dontConversations.SetActive(false);
        }
    }

    private void interaction()
    {
        if (conversationsEnd == false && talking == true)
        {
            if (conversationing == true)
            {
                if (Input.GetKeyDown(keyManager.InteractionKey()))
                {
                    if (conversations.Count - 1 < conversationsCount)
                    {
                        Barricade.SetActive(false);
                        conversationsEnd = true;
                        return;
                    }
                    else if (conversationsCount - 1 > -1)
                    {
                        conversations[conversationsCount - 1].SetActive(false);
                    }

                    interactionKeyImage.SetActive(false);
                    conversations[conversationsCount].SetActive(true);
                    conversationsCount++;
                }
            }
        }
    }

    private void npcEndMove()
    {
        if (conversationsEnd == true)
        {
            transform.position += Vector3.up * 18 * Time.deltaTime;
            transform.Rotate(new Vector3(0, 360, 0) * 2* Time.deltaTime);
        }
    }
}
