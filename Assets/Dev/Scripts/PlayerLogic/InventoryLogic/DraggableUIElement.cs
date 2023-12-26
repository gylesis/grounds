using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Dev.Scripts.PlayerLogic.InventoryLogic
{
    public class DraggableUIElement : MonoBehaviour, IDragHandler, IPointerUpHandler, IPointerDownHandler, IPointerClickHandler
    {
        public Subject<PointerEventData> Drag { get; } = new Subject<PointerEventData>();
        public Subject<PointerEventData> PointerUp { get; } = new Subject<PointerEventData>();
        public Subject<PointerEventData> PointerDown { get; } = new Subject<PointerEventData>();
        public Subject<PointerEventData> PointerClick{ get; } = new Subject<PointerEventData>();
        
        public void OnDrag(PointerEventData eventData)
        {
            Drag.OnNext(eventData);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            PointerUp.OnNext(eventData);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            PointerDown.OnNext(eventData);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            PointerClick.OnNext(eventData);
        }
    }
}