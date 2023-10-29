using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class LichAi : EnemyManager
{
    public NavMeshAgent agent;
    public bool isAttacking = false;

    public GameObject defaultAttackPrefab;
    public GameObject summonPawnAttackPrefab;
    public GameObject energyBeamAttackPrefab;
    public GameObject skyBeamAttackPrefab;
    public GameObject aoeAttackPrefab;

    // Attack probabilities (sum to 100%)
    public float rangeAttackProbability = 40f;
    public float summonPawnAttackProbability = 5f;
    public float energyBeamAttackProbability = 20f;
    public float skyBeamAttackProbability = 20f;
    public float aoeAttackProbability = 15f;

    public GameObject pentagonObject;
    public GameObject sigilObject;
    public List<Sprite> sigilSPrites = new List<Sprite>();
    public Color magicColor;

    void Start()
    {
        currentHealth = enemyStats.health;
        playerManager = FindObjectOfType<PlayerManager>();
        agent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        agent.destination = playerManager.transform.position;
        if (Vector3.Distance(transform.position, playerManager.transform.position) > 2f)
        {
            rangeAttackProbability = 45f;
            aoeAttackProbability = 5f;
        }
        else
        {
            rangeAttackProbability = 5f;
            aoeAttackProbability = 45f;
        }

        if (!isAttacking)
        {
            isAttacking = true;
            StartCoroutine(SelectAttack());
        }
    }

    IEnumerator SelectAttack()
    {
        float attackCooldown = Random.Range(2f, 4f);
        float randomValue = Random.Range(0f, 100f);

        if(randomValue < rangeAttackProbability)
        {
            yield return StartCoroutine(RangeAttack());
        }
        else if(randomValue < rangeAttackProbability + summonPawnAttackProbability)
        {
            magicColor = new Color(128f, 0f, 128f);
            sigilObject.GetComponent<SpriteRenderer>().sprite = sigilSPrites[2];
            yield return StartCoroutine(SummonPawnAttack());
        }
        else if(randomValue < rangeAttackProbability + summonPawnAttackProbability + energyBeamAttackProbability)
        {
            magicColor = Color.red;
            sigilObject.GetComponent<SpriteRenderer>().sprite = sigilSPrites[0];
            yield return StartCoroutine(EnergyBeamAttack());
        }
        else if(randomValue < rangeAttackProbability + summonPawnAttackProbability + energyBeamAttackProbability + skyBeamAttackProbability)
        {
            magicColor = Color.red;
            sigilObject.GetComponent<SpriteRenderer>().sprite = sigilSPrites[0];
            yield return StartCoroutine(SkyBeamAttack());
        }
        else
        {
            magicColor = Color.blue;
            sigilObject.GetComponent<SpriteRenderer>().sprite = sigilSPrites[1];
            yield return StartCoroutine(AoeAttack());
        }

        yield return new WaitForSeconds(attackCooldown);
        isAttacking = false;
    }

    IEnumerator RangeAttack()
    {
        Instantiate(defaultAttackPrefab, new Vector3(transform.position.x + Random.Range(1,3), transform.position.y + 3, transform.position.z + Random.Range(1,3)), Quaternion.Euler(new Vector3(-90,0,0)));
        yield return new WaitForSeconds(1.5f);
    }

    IEnumerator SummonPawnAttack()
    {
        StartCoroutine(FadeInAnim());

        yield return new WaitForSeconds(3f);

        Instantiate(summonPawnAttackPrefab, new Vector3(transform.position.x, 0.1f, transform.position.z), Quaternion.Euler(new Vector3(-90,0,0)));

        yield return new WaitForSeconds(3f);

        StartCoroutine(FadeOutAnim());
    }

    IEnumerator EnergyBeamAttack()
    {
        StartCoroutine(FadeInAnim());
        yield return new WaitForSeconds(0.5f);

        //rotate energyBeam local euler angles on y axis pointing at Player
        Quaternion targetDir = Quaternion.LookRotation(playerManager.transform.position - transform.position);

        //setting spawn pos around Lich while pointing towards player
        Vector3 offset = transform.position + (targetDir * new Vector3(0f, 1f, 0f));

        GameObject energyBeam = Instantiate(energyBeamAttackPrefab, offset, targetDir);

        yield return new WaitForSeconds(3f);
        StartCoroutine(FadeOutAnim());
    }

    IEnumerator SkyBeamAttack()
    {
        StartCoroutine(FadeInAnim());
        yield return new WaitForSeconds(0.5f);
        
        StartCoroutine(SigilAnimRotate());

        yield return new WaitForSeconds(.5f);
        GameObject skyBeam = Instantiate(skyBeamAttackPrefab, new Vector3(playerManager.transform.position.x, 0.16f, playerManager.transform.position.z), Quaternion.Euler(new Vector3(-90f,0,0)));
        yield return new WaitForSeconds(3f);
        StartCoroutine(SigilAnimRotateBack());
        StartCoroutine(FadeOutAnim());
    }

    IEnumerator AoeAttack()
    {
        StartCoroutine(FadeInAnim());
        yield return new WaitForSeconds(1f);

        GameObject aoeAttack = Instantiate(aoeAttackPrefab, new Vector3(transform.position.x, 0.1f, transform.position.z), Quaternion.Euler(new Vector3(-90,0,0)));
        yield return new WaitForSeconds(2f);
        StartCoroutine(FadeOutAnim());
    }

    IEnumerator SigilAnimRotate()
    {
        //rotate sigil local euler angles on x axis by 90 degrees
        float time = 0f;
        while (time < 1f)
        {
            time += Time.deltaTime;
            sigilObject.transform.localEulerAngles = new Vector3(Mathf.SmoothStep(0f, 90f, time), 0f, 0f);
            yield return null;
        }
    }

    IEnumerator SigilAnimRotateBack()
    {
        //rotate sigil back to local euler angles on x axis by 0 degrees
        float time = 0f;
        while (time < 1f)
        {
            time += Time.deltaTime;
            sigilObject.transform.localEulerAngles = new Vector3(Mathf.SmoothStep(90f, 0f, time), 0f, 0f);
            yield return null;
        }
    }

    IEnumerator FadeOutAnim()
    {
        // fading out sigil and pentagon sprite renderer with lerping alpha
        float time = 0f;
        while (time < 1f)
        {
            time += Time.deltaTime;
            sigilObject.GetComponent<SpriteRenderer>().color = new Color(magicColor.r, magicColor.g, magicColor.b, Mathf.Lerp(1f, 0f, time));
            pentagonObject.GetComponent<SpriteRenderer>().color = new Color(magicColor.r, magicColor.g, magicColor.b, Mathf.Lerp(1f, 0f, time));

            //moving sigil and pentagon slightly down on y axis
            sigilObject.transform.localPosition = new Vector3(sigilObject.transform.localPosition.x, Mathf.Lerp(sigilObject.transform.localPosition.y, 2, time), sigilObject.transform.localPosition.z);
            pentagonObject.transform.localPosition = new Vector3(pentagonObject.transform.localPosition.x, Mathf.Lerp(pentagonObject.transform.localPosition.y, 2, time), pentagonObject.transform.localPosition.z);

            yield return null;
        }
    }

    IEnumerator FadeInAnim()
    {
        // fading in sigil and pentagon sprite renderer with lerping alpha
        float time = 0f;
        while (time < 1f)
        {
            time += Time.deltaTime;
            sigilObject.GetComponent<SpriteRenderer>().color = new Color(magicColor.r, magicColor.g, magicColor.b, Mathf.Lerp(0f, 1f, time));
            pentagonObject.GetComponent<SpriteRenderer>().color = new Color(magicColor.r, magicColor.g, magicColor.b, Mathf.Lerp(0f, 1f, time));
            sigilObject.transform.localPosition = new Vector3(sigilObject.transform.localPosition.x, Mathf.Lerp(sigilObject.transform.localPosition.y, 2.95f, time), sigilObject.transform.localPosition.z);
            pentagonObject.transform.localPosition = new Vector3(pentagonObject.transform.localPosition.x, Mathf.Lerp(pentagonObject.transform.localPosition.y, 2.756f, time), pentagonObject.transform.localPosition.z);
            yield return null;
        }
    }

    private void OnDestroy()
    {
        FindObjectOfType<GameManager>().ReplaceCorruptWithGreenland(2000);
    }
}
