// ========================================================================
// 文件功能：玩家挤压死亡处理器
// 负责检测玩家是否被两个物体从相反方向夹住（挤压），在满足条件时触发死亡流程。
// 通过碰撞回调收集接触点信息，在物理更新中检查速度与接触法线，使用时间缩放冻结游戏，
// 并调用场景管理器重新加载当前关卡。
// ========================================================================

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

    /// <summary>
    /// Unity 碰撞持续回调 OnCollisionStay2D，在与其他 2D 碰撞体持续接触时每帧调用。
    /// 通过 Collision2D.GetContact(index) 获取接触点，将所有非玩家物体的接触点收集到 currentContacts 列表中，
    /// 供 FixedUpdate 中的挤压判定使用。
    /// 在组件面板中可配置的字段：crushSpeedThreshold（速度阈值）、crushDeathDelay（死亡延迟时间）。
    /// </summary>
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

    /// <summary>
    /// Unity 固定物理更新回调 FixedUpdate，按固定时间间隔调用。
    /// 如果未死亡且当前帧有接触点，则调用 CheckCrush 进行挤压判定；
    /// 判定结束后清空 currentContacts 列表，为下一帧重新收集做准备。
    /// </summary>
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

    /// <summary>
    /// 挤压死亡判定逻辑。
    /// 通过 GetComponent&lt;Rigidbody2D&gt;() 获取玩家刚体，检查其水平速度绝对值是否低于 crushSpeedThreshold；
    /// 满足速度条件后，遍历收集到的接触点，使用 Vector2.Dot 计算两两接触法线的点积，
    /// 若点积小于 -0.9 则说明存在方向几乎相反的接触法线（夹角接近180度），判定为被挤压，启动死亡协程。
    /// </summary>
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

    /// <summary>
    /// 挤压死亡序列协程 CrushDeathSequence。
    /// 设置死亡标志，通过 Time.timeScale = 0 冻结游戏时间；
    /// 使用 FindObjectOfType&lt;PauseManager&gt;() 查找并禁用暂停管理器，防止死亡期间弹出暂停菜单；
    /// 调用 WaitForSecondsRealtime 等待真实时间 crushDeathDelay 秒后，恢复时间缩放为 1f，
    /// 并通过 SceneTransitionManager.LoadScene 重新加载当前场景（传入 SceneManager.GetActiveScene().buildIndex）。
    /// </summary>
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
        SceneTransitionManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}