using Pixelplacement;
using UnityEngine;

namespace Core.Character
{
    public class CameraController : MonoBehaviour
    {
        public static CameraController Instance;

        public Transform cameraTarget;
        public float scrollSpeed = 8.0f;
        public float verticalOffset = 3.0f;

        private Transform borderContainerTRBL;
        private Vector2 borderMin;
        private Vector2 borderMax;

        private float scrollLeftPos = -0.1f;
        private float scrollRightPos = 0.1f;

        public bool IsFollowingPlayer { get; set; }

        private Player player;
        private Vector2 targetPosition;

        private void Awake()
        {
            // ? 完整单例：防重复
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;

            IsFollowingPlayer = true;
        }

        private void Start()
        {
            // ? Start 里拿 Player，更稳（保证 Player.Awake 已执行）
            player = Player.instance;
            if (player == null)
                player = FindAnyObjectByType<Player>();

            // 你也可以选择：在 borders 未设置前不跟随，避免 borderMax/min 未初始化
            // IsFollowingPlayer = (player != null && cameraTarget != null && borderContainerTRBL != null);
        }

        private void OnDestroy()
        {
            if (Instance == this) Instance = null;
        }

        void Update()
        {
            if (!IsFollowingPlayer) return;

            // ? 缺引用直接 return，别只 Debug
            if (player == null)
            {
                // 尝试补救（可选）
                player = Player.instance ?? FindAnyObjectByType<Player>();
                if (player == null) return;
            }

            if (cameraTarget == null) return;

            // 如果你依赖边界，borderContainerTRBL 没设时也不要跑 clamp
            if (borderContainerTRBL == null) return;

            Vector2 playerScreenPos = (Vector2)player.transform.position - (Vector2)cameraTarget.position;

            // Scrolling in X
            if (playerScreenPos.x > scrollRightPos)
                targetPosition.x = Mathf.Min(player.transform.position.x, borderMax.x - 9.0f);
            else if (playerScreenPos.x < scrollLeftPos)
                targetPosition.x = Mathf.Max(player.transform.position.x, borderMin.x + 9.0f);

            // ? 建议也更新 y（否则 y 会停留在旧值）
            targetPosition.y = Mathf.Min(
                Mathf.Max(player.transform.position.y + verticalOffset, borderMin.y + 10.0f),
                borderMax.y - 5.0f);

            cameraTarget.position = new Vector3(
                Mathf.SmoothStep(cameraTarget.position.x, targetPosition.x, Time.deltaTime * scrollSpeed),
                Mathf.SmoothStep(cameraTarget.position.y, targetPosition.y, Time.deltaTime * scrollSpeed),
                cameraTarget.position.z);
        }

        public void SetBorders(Transform borderContainerTRBL)
        {
            this.borderContainerTRBL = borderContainerTRBL;
            UpdateBorders();
        }

        public void UpdateBorders()
        {
            if (borderContainerTRBL == null) return;

            borderMax = new Vector2(borderContainerTRBL.GetChild(1).position.x,
                                    borderContainerTRBL.GetChild(0).position.y);
            borderMin = new Vector2(borderContainerTRBL.GetChild(3).position.x,
                                    borderContainerTRBL.GetChild(2).position.y);

            MoveToTarget();
        }

        public void MoveToTarget()
        {
            if (player == null || cameraTarget == null || borderContainerTRBL == null) return;

            targetPosition.x = Mathf.Clamp(player.transform.position.x, borderMin.x + 9.0f, borderMax.x - 9.0f);
            targetPosition.y = Mathf.Clamp(player.transform.position.y + verticalOffset, borderMin.y + 10.0f, borderMax.y - 5.0f);

            cameraTarget.position = new Vector3(targetPosition.x, targetPosition.y, cameraTarget.position.z);
        }

        public void ShakeCamera(float strength, float duration = 1.0f)
        {
            // ? 防止 Instance 已被销毁但外部还在调用
            if (!this) return;

            Tween.Shake(transform, transform.localPosition, new Vector3(strength, strength, 0), duration, 0);
        }
    }
}
