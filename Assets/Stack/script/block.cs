using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class block : MonoBehaviour
{
    private const float BoundSize = 3.5f;
    private const float MovingBoundsSize = 3f;
    private const float StackMovingSpeed = 5.0f;
    private const float BlockMovingSpeed = 3.5f;
    private const float ErrorMargin = 0.1f;

    public GameObject originBlock = null;

    private Vector3 prevBlockPosition;
    private Vector3 desiredPosition;
    private Vector3 stackBounds = new Vector2(BoundSize, BoundSize);

    Transform lastBlock = null;
    float blockTransition = 0.0f;
    float secondaryPosition = 0.0f;


    int stackCount = -1;
    public int Score { get { return stackCount; } }

    int comboCount = 0;
    public int Combo { get { return comboCount; } }

    private int maxCombo = 0;
    public int MaxCombo { get => maxCombo; }

    int bestScore = 0;
    public int BestScore { get => bestScore; }

    int bestCombo = 0;
    public int BestCombo { get => bestCombo; }

    private const string BestScoreKey = "BestScore";
    private const string BestComboKey = "BestCombo";

    bool isMovingX = true;
    private bool isGameOver = true;


    // Start is called before the first frame update
    void Start()
    {
        if(originBlock == null)
        {
            Debug.Log("originblock = null");
            return;
        }
        bestScore = PlayerPrefs.GetInt(BestScoreKey, 0);
        bestCombo = PlayerPrefs.GetInt(BestComboKey, 0);

        prevBlockPosition = Vector3.down;
        Spawn_Block();
    }

    // Update is called once per frame
    void Update()
    {
        if (isGameOver)
            return;
        if (Input.GetMouseButtonDown(0))
        {
            if (PlaceBlock())
            {
                Spawn_Block();
            }
            else
            {
                // 게임 오버
                Debug.Log("GameOver");
                UpdateScore();
                isGameOver = true;
                GameOverEffect();
                UIManager.Instance.SetScoreUI();
            }
        }
        MoveBlock();
        transform.position = Vector3.Lerp(transform.position, desiredPosition, StackMovingSpeed * Time.deltaTime);
    }

    bool Spawn_Block()
    {
        if (lastBlock != null)
        {
            prevBlockPosition = lastBlock.localPosition;
        }
        GameObject newBlock = null;
        Transform newTrans = null;

        newBlock = Instantiate(originBlock);

        if(newBlock == null)
        {
            Debug.Log("newBlock = null");
            return false;
        }

        newTrans = newBlock.transform;
        newTrans.parent = this.transform;
        newTrans.localPosition = prevBlockPosition + Vector3.up;
        newTrans.localRotation = Quaternion.identity;
        newTrans.localScale = new Vector3(stackBounds.x, 1, stackBounds.y);

        stackCount++;

        desiredPosition = Vector3.down * stackCount;
        blockTransition = 0.0f;

        lastBlock = newTrans;
        isMovingX = !isMovingX;
        UIManager.Instance.UpdateScore();
        return true;
    }

    void MoveBlock()
    {
        blockTransition += Time.deltaTime * BlockMovingSpeed;

        float movePosition = Mathf.PingPong(blockTransition, BoundSize) - BoundSize / 2;

        if (isMovingX)
        {
            lastBlock.localPosition = new Vector3(movePosition * MovingBoundsSize, stackCount, secondaryPosition);
        }
        else
        {
            lastBlock.localPosition = new Vector3(secondaryPosition, stackCount, -movePosition * MovingBoundsSize);
        }
    }

    bool PlaceBlock()
    {
        Vector3 lastPosition = lastBlock.transform.localPosition;

        if (isMovingX)
        {
            float deltaX = prevBlockPosition.x - lastPosition.x;
            bool isNegativeNum = (deltaX < 0) ? true : false;

            deltaX = Mathf.Abs(deltaX);
            if (deltaX > ErrorMargin)
            {
                stackBounds.x -= deltaX;

                if (stackBounds.x <= 0)
                {
                    return false;
                }

                float middle = (prevBlockPosition.x + lastPosition.x) / 2;
                lastBlock.localScale = new Vector3(stackBounds.x, 1, stackBounds.y);

                Vector3 tempPosition = lastBlock.localPosition;
                tempPosition.x = middle;
                lastBlock.localPosition = lastPosition = tempPosition;

                float rubbleHalfScale = deltaX / 2f;
                CreateRubble(
                    new Vector3(isNegativeNum
                            ? lastPosition.x + stackBounds.x / 2 + rubbleHalfScale
                            : lastPosition.x - stackBounds.x / 2 - rubbleHalfScale
                        , lastPosition.y
                        , lastPosition.z),
                    new Vector3(deltaX, 1, stackBounds.y)
                );
                comboCount = 0;
            }
            else
            {
                ComboCheck();
                lastBlock.localPosition = prevBlockPosition + Vector3.up;
            }
        }
        else
        {
            float deltaZ = prevBlockPosition.z - lastPosition.z;
            bool isNegativeNum = (deltaZ < 0) ? true : false;

            deltaZ = Mathf.Abs(deltaZ);
            if (deltaZ > ErrorMargin)
            {
                stackBounds.y -= deltaZ;
                if (stackBounds.y <= 0)
                {
                    return false;
                }

                float middle = (prevBlockPosition.z + lastPosition.z) / 2;
                lastBlock.localScale = new Vector3(stackBounds.x, 1, stackBounds.y);

                Vector3 tempPosition = lastBlock.localPosition;
                tempPosition.z = middle;
                lastBlock.localPosition = lastPosition = tempPosition;

                float rubbleHalfScale = deltaZ / 2f;
                CreateRubble(
                    new Vector3(
                        lastPosition.x
                        , lastPosition.y
                        , isNegativeNum
                            ? lastPosition.z + stackBounds.y / 2 + rubbleHalfScale
                            : lastPosition.z - stackBounds.y / 2 - rubbleHalfScale),
                    new Vector3(stackBounds.x, 1, deltaZ)
                );
                comboCount = 0;
            }
            else
            {
                ComboCheck();
                lastBlock.localPosition = prevBlockPosition + Vector3.up;
            }
        }

        secondaryPosition = (isMovingX) ? lastBlock.localPosition.x : lastBlock.localPosition.z;

        return true;
    }

    void CreateRubble(Vector3 pos, Vector3 scale)
    {
        GameObject go = Instantiate(lastBlock.gameObject);
        go.transform.parent = this.transform;

        go.transform.localPosition = pos;
        go.transform.localScale = scale;
        go.transform.localRotation = Quaternion.identity;

        go.AddComponent<Rigidbody>();
        go.name = "Rubble";
    }

    void ComboCheck()
    {
        comboCount++;

        if (comboCount > maxCombo)
            maxCombo = comboCount;

        if ((comboCount % 5) == 0)
        {
            Debug.Log("5Combo Success!");
            stackBounds += new Vector3(0.5f, 0.5f);
            stackBounds.x =
                (stackBounds.x > BoundSize) ? BoundSize : stackBounds.x;
            stackBounds.y =
                (stackBounds.y > BoundSize) ? BoundSize : stackBounds.y;
        }
    }
    void UpdateScore()
    {
        if (bestScore < stackCount)
        {
            Debug.Log("최고 점수 갱신");
            bestScore = stackCount;
            bestCombo = maxCombo;

            PlayerPrefs.SetInt(BestScoreKey, bestScore);
            PlayerPrefs.SetInt(BestComboKey, bestCombo);
        }
    }

    void GameOverEffect()
    {
        int childCount = this.transform.childCount;

        for (int i = 1; i < 20; i++)
        {
            if (childCount < i)
                break;

            GameObject go =
                this.transform.GetChild(childCount - i).gameObject;

            if (go.name.Equals("Rubble"))
                continue;

            Rigidbody rigid = go.AddComponent<Rigidbody>();

            rigid.AddForce(
                (Vector3.up * Random.Range(0, 10f)
                 + Vector3.right * (Random.Range(0, 10f) - 5f))
                * 100f
            );
        }
    }

    public void Restart()
    {
        int childCount = transform.childCount;

        for (int i = 0; i < childCount; i++)
        {
            Destroy(transform.GetChild(i).gameObject);
        }

        isGameOver = false;

        lastBlock = null;
        desiredPosition = Vector3.zero;
        stackBounds = new Vector3(BoundSize, BoundSize);

        stackCount = -1;
        isMovingX = true;
        blockTransition = 0f;
        secondaryPosition = 0f;

        comboCount = 0;
        maxCombo = 0;

        prevBlockPosition = Vector3.down;

        Spawn_Block();
        Spawn_Block();
    }
}
