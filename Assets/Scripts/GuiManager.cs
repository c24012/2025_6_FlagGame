using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Playables;

public class GuiManager : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI newsText;
    [SerializeField] TextMeshProUGUI victoryText;

    [SerializeField] PlayableDirector countDownTL;
    [SerializeField] PlayableDirector newsTL;
    [SerializeField] PlayableDirector timeUpTL;
    [SerializeField] PlayableDirector aloneTL;

    /// <summary>
    /// �Q�[���J�n�J�E���g�_�E��
    /// </summary>
    public void PlayCountDownAnimation()
    {
        countDownTL.Play();
    }

    /// <summary>
    /// �c�莞�ԕ\���e�L�X�g�A�j���[�V����
    /// </summary>
    public void PlayNewsFlowAnimation(string timeStr)
    {
        newsText.text = timeStr;
        newsTL.Play();
    }

    /// <summary>
    /// �Q�[���I�����̏��ҕ\��
    /// </summary>
    public void PlayVictoryAnim(int[] winnerPlayerNum, bool isTimeUp = false)
    {
        //�\���p�̕�������`
        string viewWinnerStr = null;
        //���������v���C���[�S���𕶎���ɓ���Ă���
        for (int i = 0; i < winnerPlayerNum.Length; i++) 
        {
            viewWinnerStr += $"{winnerPlayerNum[i]}P";
            //�\�������Ō�̃v���C���[�ȊO�͊Ԃ�[��]������
            if(i != winnerPlayerNum.Length - 1) viewWinnerStr += " ";
        }
        //�Ō�ɉ��s����WIN��ǉ�
        viewWinnerStr += "\nWIN";

        //���������������\���e�L�X�g�ɑ��
        victoryText.text = viewWinnerStr;

        //�I���̗��R���^�C���A�b�v�̏ꍇ�̃A�j���[�V����
        if (isTimeUp)
        {
            timeUpTL.Play();
        }
        //�v���C���[�̂����ꂩ���S�[�������ꍇ�̃A�j���[�V����
        else
        {
            aloneTL.Play();
        }
    }
}
