using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.InputSystem.Layouts;

////TODO: custom icon for OnScreenStick component

namespace UnityEngine.InputSystem.OnScreen
{
    /// <summary>
    /// A stick control displayed on screen and moved around by touch or other pointer
    /// input.
    /// </summary>
    [AddComponentMenu("Input/On-Screen Stick")]
    public class AppearingOnScreenJoystick : OnScreenControl, IPointerDownHandler, IPointerUpHandler, IDragHandler
    {
        [SerializeField]
        private GameObject back;
        [SerializeField]
        private GameObject stick;

        public void OnPointerDown(PointerEventData eventData)
        {
            if (eventData == null)
                throw new System.ArgumentNullException(nameof(eventData));

            //RectTransformUtility.ScreenPointToLocalPointInRectangle(transform.parent.GetComponentInParent<RectTransform>(), eventData.position, eventData.pressEventCamera, out m_PointerDownPos);
            m_PointerDownPos = eventData.position;
            m_StartPos = eventData.position;

            back.SetActive(true);
            stick.SetActive(true);
            back.transform.position = eventData.position;
            stick.transform.position = eventData.position;
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (eventData == null)
                throw new System.ArgumentNullException(nameof(eventData));

            //RectTransformUtility.ScreenPointToLocalPointInRectangle(transform.parent.GetComponentInParent<RectTransform>(), eventData.position, eventData.pressEventCamera, out var position);
            Vector2 position = eventData.position;
            var delta = position - m_PointerDownPos;

            float movementRange = ((RectTransform)back.transform).rect.width / 2 * movementRangeMultiplier;

            delta = Vector2.ClampMagnitude(delta, movementRange);
            stick.transform.position = m_StartPos + (Vector3)delta;

            var newPos = new Vector2(delta.x / movementRange, delta.y / movementRange);
            SendValueToControl(newPos);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            SendValueToControl(Vector2.zero);

            back.SetActive(false);
            stick.SetActive(false);
        }

        private void Start()
        {
            m_StartPos = ((RectTransform)transform).anchoredPosition;
        }

        public float movementRangeMultiplier
        {
            get => m_MovementRangeMultiplier;
            set => m_MovementRangeMultiplier = value;
        }

        [FormerlySerializedAs("movementRange")]
        [SerializeField]
        private float m_MovementRangeMultiplier = 1;

        [InputControl(layout = "Vector2")]
        [SerializeField]
        private string m_ControlPath;

        private Vector3 m_StartPos;
        private Vector2 m_PointerDownPos;

        protected override string controlPathInternal
        {
            get => m_ControlPath;
            set => m_ControlPath = value;
        }
    }
}
