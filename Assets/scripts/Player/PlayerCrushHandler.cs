using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class PlayerCrushHandler : MonoBehaviour
{
    [Header("挤压死亡设置")]
    [Tooltip("触发挤压死亡的最大速度阈值（低于此值且被夹住则判定死亡）")]
    [SerializeField] private float crushSpeedThreshold = 0.5f;
    [Tooltip("死亡后等待多少秒重新加载关卡")]
    [SerializeField] private float crushDeathDelay = 1.5f;

    private bool isDead = false;
    private List<ContactPoint2D> currentContacts = new List<ContactPoint2D>();

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (isDead)
            return;

        // 忽略玩家自身
        if (collision.gameObject.CompareTag("Player"))
            return;

        // 只累加，不清空，FixedUpdate 会统一处理并清空
        for (int i = 0; i < collision.contactCount; i++)
        {
            currentContacts.Add(collision.GetContact(i));
        }
    }

    private void FixedUpdate()
    {
        if (isDead)
        {
            currentContacts.Clear();
            return;
        }

        if (currentContacts.Count > 0)
        {
            CheckCrush();
        }

        // 清空，准备下一帧收集
        currentContacts.Clear();
    }

    private void CheckCrush()
    {
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb == null) return;

        // 条件1：玩家水平速度很小（被卡住不能自主移动，允许一定的被动滑动）
        if (Mathf.Abs(rb.linearVelocity.x) > crushSpeedThreshold)
            return;

        // 条件2：检查是否存在两个方向相反的接触法线，表明被左右（或上下）夹住
        bool hasOppositeContacts = false;
        for (int i = 0; i < currentContacts.Count; i++)
        {
            for (int j = i + 1; j < currentContacts.Count; j++)
            {
                float dot = Vector2.Dot(currentContacts[i].normal, currentContacts[j].normal);
                // 点积小于 -0.9 表示法线几乎相反（夹角接近180度）
                if (dot < -0.9f)
                {
                    hasOppositeContacts = true;
                    break;
                }
            }
            if (hasOppositeContacts) break;
        }

        if (hasOppositeContacts)
        {
            StartCoroutine(CrushDeathSequence());
        }
    }

    private IEnumerator CrushDeathSequence()
    {
        isDead = true;

        // 冻结时间
        Time.timeScale = 0f;

        // 禁用暂停管理器，避免死亡期间按 ESC 弹出暂停界面
        PauseManager pauseManager = FindObjectOfType<PauseManager>();
        if (pauseManager != null)
            pauseManager.enabled = false;

        // 等待指定真实时间
        yield return new WaitForSecondsRealtime(crushDeathDelay);

        // 恢复时间并重新加载当前关卡
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
//using UnityEngine;
//using UnityEngine.SceneManagement;
//using System.Collections;
//using System.Collections.Generic;

//public class PlayerCrushHandler : MonoBehaviour
//{
//    [Header("挤压死亡设置 (法线相反)")]
//    [Tooltip("启用基于法线相反的传统挤压死亡判定")]
//    [SerializeField] private bool useNormalOppositeCrush = true;
//    [Tooltip("触发挤压死亡需要的最小相对速度阈值（低于此速度且被夹住则判定死亡）")]
//    [SerializeField] private float crushSpeedThreshold = 0.1f;

//    [Header("挤压死亡设置 (重叠百分比)")]
//    [Tooltip("启用基于碰撞箱重叠百分比的挤压死亡判定")]
//    [SerializeField] private bool useOverlapCrush = true;
//    [Tooltip("玩家碰撞箱与其它碰撞箱重叠面积占玩家碰撞箱面积的百分比阈值 (0~1)，建议 0.2~0.5")]
//    [SerializeField] private float overlapPercentageThreshold = 0.3f;

//    [Header("死亡后行为")]
//    [Tooltip("死亡后等待多少秒重新加载关卡")]
//    [SerializeField] private float crushDeathDelay = 1.5f;

//    private bool isDead = false;
//    private List<ContactPoint2D> currentContacts = new List<ContactPoint2D>();
//    private List<Collider2D> currentColliders = new List<Collider2D>();
//    private Collider2D playerCollider;

//    private void Awake()
//    {
//        playerCollider = GetComponent<Collider2D>();
//    }

//    private void OnCollisionStay2D(Collision2D collision)
//    {
//        if (isDead)
//            return;

//        // 只检测非玩家物体
//        if (collision.gameObject.CompareTag("Player"))
//            return;

//        // 收集接触点
//        for (int i = 0; i < collision.contactCount; i++)
//        {
//            currentContacts.Add(collision.GetContact(i));
//        }

//        // 收集碰撞体（用于重叠百分比检测）
//        if (useOverlapCrush && !currentColliders.Contains(collision.collider))
//        {
//            currentColliders.Add(collision.collider);
//        }
//    }

//    private void FixedUpdate()
//    {
//        if (isDead)
//            return;

//        // 法线相反挤压检测
//        if (useNormalOppositeCrush && currentContacts.Count > 0)
//        {
//            CheckNormalOppositeCrush();
//        }

//        // 重叠百分比挤压检测
//        if (useOverlapCrush && currentColliders.Count > 0)
//        {
//            CheckOverlapCrush();
//        }

//        // 清空列表，准备下一帧重新收集
//        currentContacts.Clear();
//        currentColliders.Clear();
//    }

//    private void CheckNormalOppositeCrush()
//    {
//        Rigidbody2D rb = GetComponent<Rigidbody2D>();
//        if (rb == null) return;

//        // 条件1：玩家速度极小（几乎被卡住）
//        if (rb.linearVelocity.magnitude > crushSpeedThreshold)
//            return;

//        // 条件2：检查是否存在两个方向相反的接触法线
//        bool hasOppositeContacts = false;
//        for (int i = 0; i < currentContacts.Count; i++)
//        {
//            for (int j = i + 1; j < currentContacts.Count; j++)
//            {
//                float dot = Vector2.Dot(currentContacts[i].normal, currentContacts[j].normal);
//                if (dot < -0.9f)
//                {
//                    hasOppositeContacts = true;
//                    break;
//                }
//            }
//            if (hasOppositeContacts) break;
//        }

//        if (hasOppositeContacts)
//        {
//            StartCoroutine(CrushDeathSequence());
//        }
//    }

//    private void CheckOverlapCrush()
//    {
//        if (playerCollider == null) return;

//        Bounds playerBounds = playerCollider.bounds;
//        float playerArea = playerBounds.size.x * playerBounds.size.y;
//        if (playerArea <= 0f) return;

//        foreach (Collider2D other in currentColliders)
//        {
//            Bounds otherBounds = other.bounds;
//            if (!playerBounds.Intersects(otherBounds))
//                continue;

//            // 计算两个轴对齐包围盒的交集面积
//            float xMin = Mathf.Max(playerBounds.min.x, otherBounds.min.x);
//            float xMax = Mathf.Min(playerBounds.max.x, otherBounds.max.x);
//            float yMin = Mathf.Max(playerBounds.min.y, otherBounds.min.y);
//            float yMax = Mathf.Min(playerBounds.max.y, otherBounds.max.y);

//            float overlapArea = (xMax - xMin) * (yMax - yMin);
//            if (overlapArea <= 0f) continue;

//            float percentage = overlapArea / playerArea;
//            if (percentage >= overlapPercentageThreshold)
//            {
//                StartCoroutine(CrushDeathSequence());
//                return;
//            }
//        }
//    }

//    private IEnumerator CrushDeathSequence()
//    {
//        isDead = true;

//        // 冻结时间
//        Time.timeScale = 0f;

//        // 禁用暂停管理器，避免死亡期间按 ESC 弹出暂停界面
//        PauseManager pauseManager = FindObjectOfType<PauseManager>();
//        if (pauseManager != null)
//            pauseManager.enabled = false;

//        // 等待指定真实时间
//        yield return new WaitForSecondsRealtime(crushDeathDelay);

//        // 恢复时间并重新加载当前关卡
//        Time.timeScale = 1f;
//        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
//    }
//}