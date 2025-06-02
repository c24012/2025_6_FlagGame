using System.Collections;
using System.Collections.Generic;
using System.Runtime;
using UnityEngine;

public class FlagState : MonoBehaviour,IPlayerDamage
{
    [SerializeField] Animator anim;
    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] Rigidbody2D rb;
    [SerializeField] Sprite[] flagSprites;

    GameObject spawnPointObj;

    [Header("�X�e�[�^�X"),Tooltip("���v���C���[�ԍ�")]public int flagNum = 0;
    [Tooltip("�ő�ϋv�l")] public int maxDurability = 100;
    [SerializeField, Tooltip("�S�[���ɂ����鎞��")] public float[] goalTimes = new float[3] { 5, 7, 9 };
    [SerializeField, Tooltip("���X�|�[������")] float respawnTime = 2f;
    [Tooltip("���݊����x��")]public int flagLevel = 0;

    [SerializeField,Header("�m�F�p"), Tooltip("���ݑϋv�l")] 
    int durability = 100;
    public bool isBreak = false;
    public bool isPutGoal = false;
    

    private void Start()
    {
        //������
        Init();

        //���̐F��ԍ��ɉ����ĂɕύX
        spriteRenderer.sprite = flagSprites[flagNum - 1];

        //�����̃X�|�[���|�C���g���擾
        GameObject[] spawnPoint = GameObject.FindGameObjectsWithTag("SpawnPoint");
        foreach (GameObject spawnPointObj in spawnPoint)
        {
            //SpawnPoint�^�O�̃I�u�W�F�N�g����SpawnPointState���擾
            if (spawnPointObj.TryGetComponent<SpawnPointState>(out SpawnPointState spawnPointState))
            {
                //���������̔ԍ��Ɗ��̔ԍ�����v������K�v�ȃR���|�[�l���g���擾
                if (spawnPointState.spawnPointNum == flagNum)
                {
                    this.spawnPointObj = spawnPointObj;
                    break;
                }
            }
            else
            {
                Debug.LogError("SpawnPointState�̂Ȃ�SpawnPoint�^�O�̃I�u�W�F�N�g��������܂���");
            }
        }

        //�����|�W�V�����Ɉړ�
        transform.position = spawnPointObj.transform.position;
    }

    private void FixedUpdate()
    {
        if (anim == null) return;
        anim.SetFloat("x",Input.GetAxis(flagNum + "P_Horizontal"));
    }

    /// <summary>
    /// �֐�������
    /// </summary>
    void Init()
    {
        durability = maxDurability;
    }

    /// <summary>
    /// ������Ă�����̗h���A�j���[�V�����t���O�w�菈��
    /// </summary>
    public void SetFlagAnimation(bool hasFlag)
    {
        anim.SetBool("HasFlag", hasFlag);
    }

    /// <summary>
    /// �v���C���[����̃_���[�W����
    /// </summary>
    public void IDamage(int damage,float force = 0,bool isLeft = false)
    {
        //�ϋv�l��0�ȉ��ɂȂ�ꍇ�͑ϋv�l��0�Ɏw��
        if ((durability - damage) <= 0)
        {
            durability = 0;
            BreakAndRespawn(isBreak: true);
        }
        //����ȊO�͑ϋv�l��Damage�����炷
        else
        {
            durability -= damage;
        }
    }

    /// <summary>
    /// ���̔j�󁕃��X�|�[������
    /// </summary>
    public void BreakAndRespawn(bool isBreak)
    {
        //�j��
        if (isBreak)
        {
            //RigidBody��OFF
            rb.simulated = false;
            //SpriteRenderer��OFF
            spriteRenderer.enabled = false;
            //�|�W�V������ύX
            transform.position = spawnPointObj.transform.position;
            //�A�j���[�V�������Đ�
            StartCoroutine(RespawnAnimation());
            //���x���ɍ��킹��Sprite�ɕύX
            spriteRenderer.sprite = flagSprites[(flagNum - 1) + (flagLevel * 4)];
            //���j��t���O��ON
            this.isBreak = true;
        }
        //������
        else
        {
            //�ϋv�l���ő�܂ŉ�
            durability = maxDurability;
            //RigidBody��ON
            rb.simulated = true;
            //SpriteRenderer��ON
            spriteRenderer.enabled = true;
            //���j��t���O��OFF
            this.isBreak = false;
        }
    }

    /// <summary>
    /// ���X�|�[���A�j���[�V����
    /// </summary>
    IEnumerator RespawnAnimation()
    {
        yield return new WaitForSeconds(respawnTime);
        WaitForSeconds wait = new(0.08f);
        for(int i = 0; i < 6; i++)
        {
            spriteRenderer.enabled = true;
            yield return wait;
            spriteRenderer.enabled = false;
            yield return wait;
        }
        BreakAndRespawn(false);
    }

}
