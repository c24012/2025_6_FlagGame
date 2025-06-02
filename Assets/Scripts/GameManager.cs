using PlayerScript;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Playables;

public class GameManager : MonoBehaviour
{
    [SerializeField] GuiManager guiManager;
    [SerializeField] PlayerData playerData;
    [SerializeField] FighterData fighterData;
    [SerializeField] GameObject flagPf;

    [SerializeField, Header("�R���|�[�l���g")] GameObject[] playersObj;

    [SerializeField, Header("�֐�"), Tooltip("�v���C�l��")] int playerCount = 0;
    [SerializeField, Tooltip("��������(s)")] int timeLimit = 0;
    [SerializeField, Tooltip("�o�ߎ���(s)")] float timeLimit_Counter = 0;
    [SerializeField, Tooltip("�\��������c�莞��(s)")] List<int> displayTheTimeList = new();

    public int[] flagLevels = new int[4];

    public bool gameStart = false;

    private void Awake()
    {
        //�Q�[���V�X�e���ݒ�
        Application.targetFrameRate = 60;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        //������
        timeLimit_Counter = timeLimit;

        //�v���C���[�f�[�^����v���C�l�����擾
        playerCount = playerData.playerCount;

        //�v���C���[���ƂɃt�@�C�^�[�Ɗ��𐶐�
        for (int i = 0; i < playerCount; i++) 
        {
            //�t�@�C�^�[����&�v���C���[�ԍ����w��
            PlayerManager playerManager 
                = Instantiate(
                    fighterData.allFightersList[playerData.playerFighters[i]],
                    playersObj[i].transform
                ).GetComponent<PlayerManager>();

            playerManager.playerNum = i + 1;

            //������
            FlagState flagState = Instantiate(
                flagPf, playersObj[i].transform
                ).GetComponent<FlagState>();

            flagState.flagNum = i + 1;

        }
    }

    private void Start()
    {
        //�Q�[���J�n�J�E���g�_�E��
        guiManager.PlayCountDownAnimation();
    }

    private void Update()
    {
        //�^�C�����~�b�g�J�E���g�_�E��
        if (gameStart)
        {
            //�^�C�����~�b�g�J�E���g
            timeLimit_Counter -= Time.deltaTime;
            //�^�C���A�b�v����
            if (timeLimit_Counter <= 0)
            {
                timeLimit_Counter = 0;
                //�^�C���A�b�v����
                FinishForTimeUp();
                return;
            }
            //���ԕ\�����X�g�ɒl���c���Ă��鎞
            if (displayTheTimeList.Count > 0)
            {
                //���X�g�̂��ꂼ��̒l������������ɋN��
                if(timeLimit_Counter <= displayTheTimeList[0])
                {
                    //�c�莞�ԕ\���p�̕�������`
                    string viewTimeCount;
                    //�c�莞�Ԃ�60�b�𒴂��Ă����番�P�ʂ̕\��
                    if (displayTheTimeList[0] >= 60)
                    {
                        viewTimeCount = $"�c��{displayTheTimeList[0] / 60}��";
                    }
                    //�c�莞�Ԃ�60�b�����������b�P�ʂ̕\��
                    else
                    {
                        viewTimeCount = $"�c��{displayTheTimeList[0]}�b";
                    }
                    //�J�E���g�\���A�j���[�V����
                    guiManager.PlayNewsFlowAnimation(viewTimeCount);
                    //�\����]���X�g����\�������l���폜
                    displayTheTimeList.RemoveAt(0);
                }
            }
        }
    }

    /// <summary>
    /// �Q�[���J�n
    /// </summary>
    public void SetGameStart()
    {
        gameStart = true;
    }

    /// <summary>
    /// �v���C���[�����ꂩ�����������ꍇ�̏���
    /// </summary>
    public void AwardTheWinner(int winnerPlayerNum)
    {
        //�Q�[�����I��
        gameStart = false;
        //���������v���C���[��z��ɕό`
        int[] winnerPlayers = new int[1] { winnerPlayerNum };
        //�����A�j���[�V����
        guiManager.PlayVictoryAnim(winnerPlayers);
    }

    /// <summary>
    /// �^�C���A�b�v���̏��Ҍ���
    /// </summary>
    void FinishForTimeUp()
    {
        //�Q�[�����I��
        gameStart = false;
        //�����x���̍ő�l���擾
        int maxLevel = flagLevels.Max();
        //���Ғǉ��p�̃��X�g���`
        List<int> winnerPlayersList = new();
        //�����x���̍ő�l�ɒB���Ă���v���C���[�̃i���o�[�����X�g�ɒǉ�
        for (int i = 0; i < playerCount; i++) 
        {
            if (flagLevels[i] == maxLevel)
            {
                winnerPlayersList.Add(i + 1);
            }
        }
        //�����v���C���[���X�g��z��ɕό`
        int[] winnerPlayers = winnerPlayersList.ToArray();
        //�����A�j���[�V����
        guiManager.PlayVictoryAnim(winnerPlayers,isTimeUp:true);
    }
}
