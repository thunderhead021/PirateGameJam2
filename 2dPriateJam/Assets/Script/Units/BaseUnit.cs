using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using static Unity.VisualScripting.Member;

public class BaseUnit : MonoBehaviour
{
    public Tile curTile;
    [HideInInspector]
    public Sprite TurnIcon;
    public Sprite PlayerSideSprite;
    public Sprite EnemySideSprite;
    public Side unitSide = Side.None;
    public int Hp = 10;
    public int AttackPower = 2;
    public int moveRange;
    public int attackRange;
    public MoveType moveType;
    public AttackType attackType;
    public AttackEffect AttackEffect;
    private HashSet<UnitStatus> curStatus = new();
    public int speed;
    public EnemyBehaviour enemyBehaviour;
    public AttackBehaviour attackBehaviour;
    public TextMeshPro curHp;
    public bool hasMoved { get; private set; } = false;
    private int MaxHp;
    private int posionedTurn = 0;
    public AudioSource audioSource;
    public List<AudioClip> attackSounds;
    public List<AudioClip> reciveDamageSounds;
    public List<AudioClip> clickOnSounds;
    public List<AudioClip> deathSounds;
    public List<AudioClip> reviveSounds;
    public List<AudioClip> moveSounds;
    [SerializeField]
    private SpriteRenderer spriteRenderer;
    [SerializeField]
    private Material defaultMaterial;
    [SerializeField]
    private Material damageMaterial;
    [SerializeField]
    private Material selectMaterial;

    public void DamageEffect(bool isRevert = false) 
    {
        if (isRevert) 
        {
            spriteRenderer.material = defaultMaterial;
            damageMaterial.SetFloat("_TintAmount", 0);
        }
        else 
        {
            spriteRenderer.material = damageMaterial;
            float amount = Mathf.Lerp(1, 0, Time.deltaTime);
            amount = Mathf.Clamp(amount, 0, 1);
            Select(false);
            damageMaterial.SetFloat("_TintAmount", amount);
        }  
    }

    public void MoveSoundPlay()
    {
        audioSource.clip = moveSounds[Random.Range(0, moveSounds.Count)];
        audioSource.Play();
    }

    private void DeathSoundPlay()
    {
        audioSource.clip = deathSounds[Random.Range(0, deathSounds.Count)];
        audioSource.Play();
    }

    private void ReviveSoundPlay()
    {
        audioSource.clip = reviveSounds[Random.Range(0, reviveSounds.Count)];
        audioSource.Play();
    }

    public void ClickOnSoundPlay() 
    {
        audioSource.clip = clickOnSounds[Random.Range(0, clickOnSounds.Count)];
        audioSource.Play();
    }

    public void AttackSoundPlay()
    {
        audioSource.clip = attackSounds[Random.Range(0, attackSounds.Count)];
        audioSource.Play();
    }

    public void ReciveDamageSoundPlay()
    {
        audioSource.clip = reciveDamageSounds[Random.Range(0, reciveDamageSounds.Count)];
        audioSource.Play();
    }

    private void Start()
    {
        MaxHp = Hp;
        SetSprite();
    }

    public void PlaySound(AudioClip clip) 
    {
        audioSource.clip = clip;
        audioSource.Play();
    }

    public void SetSprite() 
    {
        spriteRenderer.sprite = unitSide == Side.Player ? PlayerSideSprite : EnemySideSprite;
        TurnIcon = unitSide == Side.Player ? PlayerSideSprite : EnemySideSprite;
    }

    public void RemoveGameObject() 
    {
        StartCoroutine(RemoveGameObjectHelper());
    }

    IEnumerator RemoveGameObjectHelper() 
    {
        DeathSoundPlay();
        while (audioSource.isPlaying)
        {
            yield return new WaitForSeconds(0.1f);
        }
        Destroy(gameObject);
    }

    public void ReviveUnit() 
    {
        StartCoroutine(ReviveUnitHelper());    
    }

    IEnumerator ReviveUnitHelper()
    {
        Hp = MaxHp;
        hasMoved = false;
        posionedTurn = 0;
        curStatus.Clear();
        Select(false);
        ReviveSoundPlay();
        while (audioSource.isPlaying)
        {
            yield return new WaitForSeconds(0.1f);
        }
        UpdateHp();
        SetSprite();
    }

    public void Select(bool isSelect) 
    {
        spriteRenderer.material = isSelect ? selectMaterial : defaultMaterial;
    }

    public void EnemyMove() 
    {
        StartCoroutine(EnemyMoveHelper());
    }

    IEnumerator EnemyMoveHelper() 
    {
        List<Tile> allMoveable = GridManager.instance.GetAllMoveableTiles(curTile, moveRange, moveType);
        yield return new WaitForSeconds(2);
        switch (enemyBehaviour)
        {
            case EnemyBehaviour.Chaser:
                ChaserMove(allMoveable);
                break;
            case EnemyBehaviour.Groupe:
                GroupeMove(allMoveable);
                break;
            case EnemyBehaviour.Coward:
                CowardMove(allMoveable);
                break;
            case EnemyBehaviour.Random:
                allMoveable[Random.Range(0, allMoveable.Count - 1)].SetUnit(this);
                break;
        }
        MoveSoundPlay();
        while (audioSource.isPlaying)
        {
            yield return new WaitForSeconds(0.1f);
        }
        //enemy attack here
        List<Tile> allAttackable = GridManager.instance.GetAllAttackableTiles(curTile, attackRange, attackType);
        if (allAttackable.Count > 0)
        {
            switch (attackBehaviour)
            {
                case AttackBehaviour.Focus:
                    FocusAttack(allAttackable);
                    break;
                case AttackBehaviour.Spread:
                    SpreadAttack(allAttackable);
                    break;
                case AttackBehaviour.Random:
                    DealDamage(allAttackable[Random.Range(0, allAttackable.Count - 1)].curUnit, AttackPower, AttackEffect);
                    break;
            }
        }
        while (audioSource.isPlaying)
        {
            yield return new WaitForSeconds(0.1f);
        }
        Move();
    }

    public void DealDamage(BaseUnit target, int amount, AttackEffect attackEffect) 
    {
        StartCoroutine(DealDamageHelper(target, amount, attackEffect));
    }

    IEnumerator DealDamageHelper(BaseUnit target, int amount, AttackEffect attackEffect) 
    {
        AttackSoundPlay();
        target.Hp -= amount;
        while (audioSource.isPlaying)
        {
            yield return new WaitForSeconds(0.1f);
        }
        if (target.Hp <= 0)
        {
            target.Death();
        }
        else
        {
            target.UpdateHp();
            target.ApplyStatus(attackEffect);
            target.ReciveDamageSoundPlay();
            while (target.audioSource.isPlaying)
            {
                target.DamageEffect();
                yield return new WaitForSeconds(0.1f);
            }
            target.DamageEffect(true);
        }
    }

    public void ApplyStatus(AttackEffect attackEffect) 
    {
        switch (attackEffect) 
        {
            case AttackEffect.Dot:
                curStatus.Add(UnitStatus.Posion);
                posionedTurn = 2;
                break;
        }
    }

    public void CheckStatus() 
    {
        foreach (UnitStatus status in curStatus) 
        {
            switch (status) 
            {
                case UnitStatus.Posion:
                    Hp -= 1;
                    if (Hp <= 0)
                    {
                        Death();
                    }
                    else
                    {
                        UpdateHp();
                        posionedTurn--;
                    }
                    break;
            }
        }
        if (posionedTurn <= 0)
        {
            curStatus.Remove(UnitStatus.Posion);
        }
    }

    public void UpdateHp() 
    {
        curHp.text = Hp.ToString();
    }

    public virtual void Death() 
    {
        UnitManager.instance.RemoveUnit(this);
    }

    public void Move() 
    {
        if (!hasMoved)
        {
            hasMoved = true;
            spriteRenderer.color = new Color32(142, 129, 129, 255);
        }
    }

    public void ResetMove()
    {
        hasMoved = false;
        spriteRenderer.color = Color.white;
    }

    private void ChaserMove(List<Tile> tileList) 
    {
        Tile nearestTile = null;
        float shortestDistance = float.MaxValue;
        foreach (Tile tile in tileList)
        {
            float distance = Vector2.Distance(UnitManager.instance.GetRandomNotAllyUnit(unitSide).curTile.pos, tile.pos);

            if (distance < shortestDistance)
            {
                shortestDistance = distance;
                nearestTile = tile;
            }
        }

        if (nearestTile != null) 
        {
            nearestTile.SetUnit(this);
        }
    }

    private void CowardMove(List<Tile> tileList) 
    {
        Tile farestTile = null;
        float longestDistance = float.MinValue;
        foreach (Tile tile in tileList)
        {
            float distance = Vector2.Distance(UnitManager.instance.GetRandomNotAllyUnit(unitSide).curTile.pos, tile.pos);

            if (distance >= longestDistance)
            {
                longestDistance = distance;
                farestTile = tile;
            }
        }

        if (farestTile != null)
        {
            farestTile.SetUnit(this);
        }
    }

    private void GroupeMove(List<Tile> tileList) 
    {
        GameObject[] units = GameObject.FindGameObjectsWithTag("Enemy");
        if (units.Length != 0) 
        {
            Tile nearestTile = null;
            float shortestDistance = float.MaxValue;
            foreach (Tile tile in tileList)
            {
                foreach (GameObject unit in units)
                {
                    // Calculate the Euclidean distance to the current unit
                    float distance = Vector2.Distance(UnitManager.instance.GetRandomNotAllyUnit(unitSide).curTile.pos, unit.GetComponent<BaseUnit>().curTile.pos);

                    // Update if this tile is closer to any unit
                    if (distance < shortestDistance)
                    {
                        shortestDistance = distance;
                        nearestTile = tile;
                    }
                }
            }
            if (nearestTile != null)
            {
                nearestTile.SetUnit(this);
            }
        } 
    }

    private void FocusAttack(List<Tile> tileList) 
    {
        BaseUnit target = null;
        int hp = int.MaxValue;
        foreach (Tile tile in tileList) 
        {
            if (tile.curUnit.Hp < hp) 
            {
                target = tile.curUnit;
                hp = tile.curUnit.Hp;
            }
        }

        if (target != null) 
        {
            DealDamage(target, AttackPower, AttackEffect);
        }
    }

    private void SpreadAttack(List<Tile> tileList)
    {
        BaseUnit target = null;
        int hp = int.MinValue;
        foreach (Tile tile in tileList)
        {
            if (tile.curUnit.Hp > hp)
            {
                target = tile.curUnit;
                hp = tile.curUnit.Hp;
            }
        }

        if (target != null)
        {
            DealDamage(target, AttackPower, AttackEffect);
        }
    }

}

public enum EnemyBehaviour 
{
    Chaser,
    Coward,
    Groupe,
    Random
}

public enum AttackBehaviour 
{
    Focus,
    Spread,
    Random
}

public enum Side 
{
    None,
    Player,
    Enemy
}

public enum UnitStatus 
{
    Normal,
    Posion
}

public enum AttackEffect
{
    Normal,
    Dot
}
