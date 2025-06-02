using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlayerScript
{
    public class PlayerJumpShotAttack : MonoBehaviour
    {
        public PlayerManager pManager;

        [SerializeField] GameObject shotObj;
        [SerializeField] float rayLength = 50f;//���C�̒���

        GameObject bulletObj;
        PlayerBullet bulletScr;
        Vector2 colOffset;
        Vector2 offsetTf;

        public void PlayerJumpShot()
        {
            //�ڂ̑O�ɕǂ����݂���ꍇ�A�ԋp
            RaycastHit2D hit2D = Physics2D.Raycast(transform.position, transform.right, rayLength, 1 << LayerMask.NameToLayer("WorldObject"));
            if (hit2D.collider != null && hit2D.collider.CompareTag("Ground"))
            {
                return;
            }

            //�W�����v�U���̏���
            colOffset = pManager.jumpAttack.attackCol.offset;
            offsetTf = (Vector2)transform.position + colOffset;
            bulletObj = Instantiate(shotObj, offsetTf, Quaternion.identity, transform);
            bulletScr = bulletObj.GetComponent<PlayerBullet>();
            bulletScr.pManager = pManager;
            bulletObj.layer = pManager.layerNum;
        }
    }

}
