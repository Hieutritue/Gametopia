using UnityEngine;
using DefaultNamespace.Map;
using System.Collections;
using DefaultNamespace;
using MoreMountains.Feedbacks;
using MoreMountains.FeedbacksForThirdParty;

[RequireComponent(typeof(Rigidbody))]
public class Gnome : MonoBehaviour
{
    public float mineDistance = 0.25f;

    [SerializeField] private Transform _visual;
    [SerializeField] private MMF_Player _jumpFeedback;
    [SerializeField] private MMF_Player _mineFeedback;
    [SerializeField] private MMF_Player _moveFeedback;

    [SerializeField] private float _cooldownMult = 1;
    private MMF_RotationSpring _rotationSpring;

    Rigidbody rb;
    Tile targetTile;

    bool mining;

    float _jumpTimer;
    float _mineTimer;
    private bool _isMoving;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        _rotationSpring = _moveFeedback.GetFeedbackOfType<MMF_RotationSpring>();
    }

    void Start()
    {
        PickNewTile();
    }

    void Update()
    {
        if (targetTile == null)
        {
            mining = false;
            PickNewTile();
            return;
        }

        float dist = DistanceToTile();

        // ✅ if pushed away → stop mining
        if (dist > mineDistance)
        {
            mining = false;
        }

        if (mining)
        {
            MineLogic();
        }
        else
        {
            MoveToTile();
        }
    }

    // ------------------------
    // MOVE
    // ------------------------

    void MoveToTile()
    {
        Vector3 target = targetTile.transform.position;

        Vector3 dir = target - transform.position;
        dir.y = 0;

        float dist = dir.magnitude;

        if (dist < mineDistance)
        {
            if (_isMoving)
            {
                _moveFeedback?.StopFeedbacks();
                _visual.localRotation = Quaternion.identity;
                _isMoving = false;
            }

            rb.linearVelocity = Vector3.zero;
            mining = true;
            _mineTimer = 0f;
            return;
        }

        if (!_isMoving)
        {
            _moveFeedback?.RestoreInitialValues();
            _moveFeedback?.PlayFeedbacks();
            _isMoving = true;
        }

        dir.Normalize();

        rb.linearVelocity = new Vector3(
            dir.x * PlayerData.Instance.GnomeSpeed,
            rb.linearVelocity.y,
            dir.z * PlayerData.Instance.GnomeSpeed
        );

        RotateTowards(dir);

        JumpLogic();
    }

    // ------------------------
    // MINING
    // ------------------------

    void MineLogic()
    {
        rb.linearVelocity = new Vector3(
            0,
            rb.linearVelocity.y,
            0
        );

        _mineTimer += Time.deltaTime;

        if (_mineTimer >= PlayerData.Instance.GnomeMineCooldown * _cooldownMult)
        {
            _mineTimer = 0f;

            if (targetTile)
            {
                _mineFeedback?.PlayFeedbacks();
                targetTile.TakeDamage(PlayerData.Instance.GnomeMineDamage,
                    PlayerData.Instance.GnomeCritChance,
                    PlayerData.Instance.GnomeCritDamage,
                    PlayerData.Instance.GnomeCanTriggerExplosion ? PlayerData.Instance.ExplosionChance / 2 : 0f);

                var attackTwiceRoll = Random.value * 100;
                if (attackTwiceRoll <= PlayerData.Instance.GnomeChanceToTriggerAttackTwice)
                {
                    StartCoroutine(SecondAttack(targetTile));
                }
            }
        }
    }

    private IEnumerator SecondAttack(Tile tile)
    {
        yield return new WaitForSeconds(0.1f);
        if (tile)
        {
            _mineFeedback?.PlayFeedbacks();
            tile.TakeDamage(PlayerData.Instance.GnomeMineDamage,
                PlayerData.Instance.GnomeCritChance,
                PlayerData.Instance.GnomeCritDamage,
                PlayerData.Instance.GnomeCanTriggerExplosion ? PlayerData.Instance.ExplosionChance / 2 : 0f);
        }
    }

    // ------------------------
    // JUMP
    // ------------------------

    void JumpLogic()
    {
        _jumpTimer += Time.deltaTime;

        if (_jumpTimer >= PlayerData.Instance.GnomeJumpCooldown)
        {
            _jumpFeedback?.PlayFeedbacks();
            rb.AddForce(Vector3.up * 5f, ForceMode.Impulse);
            _jumpTimer = 0f;
        }
    }

    // ------------------------
    // ROTATE
    // ------------------------

    void RotateTowards(Vector3 dir)
    {
        if (dir == Vector3.zero) return;

        Quaternion targetRot = Quaternion.LookRotation(dir);

        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRot,
            10f * Time.deltaTime
        );
    }

    // ------------------------
    // HELPERS
    // ------------------------

    float DistanceToTile()
    {
        if (!targetTile) return 999f;

        Vector3 a = transform.position;
        Vector3 b = targetTile.transform.position;

        a.y = 0;
        b.y = 0;

        return Vector3.Distance(a, b);
    }

    void PickNewTile()
    {
        targetTile = GetRandomTileOfHighestHeight();
    }

    Tile GetRandomTileOfHighestHeight()
    {
        var tiles = MapManager.Instance.GetAllTilesOfHighestHeight();

        if (tiles == null || tiles.Count == 0)
            return null;

        int randomIndex = Random.Range(0, tiles.Count);

        return tiles[randomIndex];
    }
}