using PlayerScript;
using UnityEngine;

[RequireComponent(typeof(ItemController))]
public class RocketScript : ItemController
{
    public Sprite spriteEx;

    public SpriteRenderer spriteRen;

    public Rigidbody2D rigi;

    public Collider2D coll;

    public Collider2D collEx;

    public float speed = 1.5f;

    public float flyTime = 1.5f;

    public float exTime = 1f;

    public int damageValue;

    public int knockBackForce = 1;

    bool isBreak = false;

    public void SetPlayerManager(PlayerManager playerManager)
    {
        pManager = playerManager;
    }

    void Start()
    {
        //生成したプレイヤーには接触しないよう設定(爆風は被弾する)
        int layerNum = LayerMask.NameToLayer(pManager.playerNum + "P");
        coll.excludeLayers = 1 << layerNum;

        if (pManager.controller.isLeft)
        {
            spriteRen.flipY = true;
        }
        else
        {
            spriteRen.flipY = false;
        }

        Invoke(nameof(Death),flyTime);
    }

    private void Update()
    {

        if (spriteRen.flipY)
        {

            rigi.AddForce(500 * speed * Time.deltaTime * -transform.up);
        }
        else
        {

            rigi.AddForce(500 * speed * Time.deltaTime * transform.up);
        }
    }

    public void Death()
    {
        if (isBreak) return;
        isBreak = true;

        coll.enabled = false;

        collEx.enabled = true;

        spriteRen.sprite = spriteEx;

        transform.localScale = transform.localScale * 4;

        rigi.constraints = RigidbodyConstraints2D.FreezeAll;

        Destroy(gameObject, exTime);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Death();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //攻撃判定にプレイヤーが当たったら
        if (collision.CompareTag("Player"))
        {
            //爆発の中心から左側にいるか
            bool isLeft = collision.transform.position.x < transform.position.x;
            //そのプレイヤーのIDamageを攻撃力をのせて起動
            if (collision.gameObject.TryGetComponent<IPlayerDamage>(out IPlayerDamage idamage))
            {
                idamage.IDamage(damageValue, knockBackForce, isLeft);
            }
        }
        //攻撃判定に旗オブジェクトが当たったら
        if (collision.CompareTag("Flag"))
        {
            //旗のIDamageを攻撃力をのせて起動
            if (collision.gameObject.TryGetComponent<IPlayerDamage>(out IPlayerDamage idamage))
            {
                idamage.IDamage(damageValue);
            }
        }
    }
}
