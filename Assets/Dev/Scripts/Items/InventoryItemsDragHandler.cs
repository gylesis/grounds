using UniRx;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Dev.Scripts.Items
{
    public class InventoryItemsDragHandler : MonoBehaviour
    {
        [SerializeField] private Camera _camera;
        [SerializeField] private float _spherecastRadius = 0.05f;
        [SerializeField] private LayerMask _draggableObjectsLayerMask;
        [SerializeField] private float _dragObjHeight = 2f;
        [SerializeField] private LayerMask _groundLayer;
        [SerializeField] private float _itemFollowCursorSpeed = 1.5f;
        [SerializeField] private Transform _ground;
        [SerializeField] private float _maxThrowSpeed = 15f;
        [SerializeField] private float _minThrowSpeed = 5f;

        private DraggableObject _tagetDragObj;
        private float _dragHeight;
        private bool _isDragging;
        private Vector2 _lastMouseDelta;
        private bool _isActive;

        private Vector3 _raycastHitPoint;

        public bool IsDragging => _isDragging;
        public DraggableObject FocusedObject => _tagetDragObj;
        
        public Subject<DraggableObject> DraggableObjectUp { get; } = new Subject<DraggableObject>();
        public Subject<DraggableObject> DraggableObjectDown { get; } = new Subject<DraggableObject>();

        private void Awake()
        {
            SetActive(false);
        }

        public void SetActive(bool isActive)
        {
            _isActive = isActive;
            _camera.gameObject.SetActive(isActive);

            _isDragging = false;
            
            if (isActive == false)
            {
                _tagetDragObj = null;
            }
        }
        
        private void Update()
        {
            if(_isActive == false) return;
            
            DragHandle();

            if(_isDragging) return;
            
            Ray ray = _camera.ScreenPointToRay(Input.mousePosition);

            bool sphereCast = Physics.SphereCast(ray, _spherecastRadius, out var hit, float.MaxValue,_draggableObjectsLayerMask);

            if (sphereCast)
            {
                bool isDraggableObject = hit.collider.TryGetComponent<DraggableObject>(out var draggableObject);
                
                if (isDraggableObject)
                {
                    _raycastHitPoint = hit.point;
                    _tagetDragObj = draggableObject;
                }
            }
        }

        private void DragHandle()
        {
            if(_tagetDragObj == null) return;
            
            bool toStartDrag = Input.GetMouseButtonDown(0); // TODO get input from input service

            if (toStartDrag)
            {
                _isDragging = true;
                
                _dragHeight = _ground.transform.position.y + _dragObjHeight;
                
                _tagetDragObj.PrepareToDrag();
                DraggableObjectDown.OnNext(_tagetDragObj);
            }

            bool toDragObj = Input.GetMouseButton(0);

            if (toDragObj)
            {
                Ray ray = _camera.ScreenPointToRay(Input.mousePosition);
                var raycast = Physics.Raycast(ray, out var hit, float.MaxValue, _groundLayer);
   
                _isDragging = true;

                Vector3 pos = hit.point;
                pos.y = _dragHeight;
                
                Vector3 position = Vector3.Lerp(_tagetDragObj.Rigidbody.position, pos, Time.deltaTime * _itemFollowCursorSpeed);
                _tagetDragObj.Rigidbody.MovePosition(position);

                _lastMouseDelta = Vector2.ClampMagnitude(Mouse.current.delta.value * _minThrowSpeed, _maxThrowSpeed);
            }

            if (Input.GetMouseButtonUp(0))
            {
                var throwVelocity = new Vector3(_lastMouseDelta.x, 0, _lastMouseDelta.y);
                _tagetDragObj.MakeFree(throwVelocity);

                DraggableObjectUp.OnNext(_tagetDragObj);

                _isDragging = false;
                _tagetDragObj = null;
            }
            
        }
        
    }
}