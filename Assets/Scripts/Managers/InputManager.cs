using UnityEngine;
using Zenject;

namespace QuickMafs
{
    public delegate void InputManagerSelectEventHandler(TileView tileView);
    public delegate void InputManagerEventHandler();
    public class InputManager : IInitializable, ITickable
    {
        public event InputManagerSelectEventHandler FirstTileSelected;
        public event InputManagerSelectEventHandler TileSelected;
        public event InputManagerEventHandler MouseUp;

        private const int LEFT_MOUSE_BUTTON = 0;
        private Camera _camera;
        private RaycastHit2D[] _raycastOut;

        private State _state = State.Waiting;

        public enum State
        {
            MouseDown, Dragging, MouseUp, Waiting
        }

        public void Initialize()
        {
            _camera = Camera.main;
            _raycastOut = new RaycastHit2D[1];
        }

        public void Tick()
        {
            if (Input.GetMouseButtonDown(LEFT_MOUSE_BUTTON))
            {
                _state = State.MouseDown;
            }
            else if (Input.GetMouseButtonUp(LEFT_MOUSE_BUTTON) && _state == State.Dragging)
            {
                _state = State.MouseUp;
            }

            ProcessState();
        }

        private void ProcessState()
        {
            switch (_state)
            {
                case State.MouseDown:
                    ProcessMouseDown();
                    break;
                case State.Dragging:
                    ProcessDragging();
                    break;
                case State.MouseUp:
                    ProcessMouseUp();
                    break;
                default:
                    break;
            }
        }

        private void ProcessMouseUp()
        {
            if(MouseUp != null)
            {
                MouseUp();
            }
        }

        private void ProcessDragging()
        {
            CheckIfTileSelected();
        }

        private void ProcessMouseDown()
        {
            CheckIfFirstTileSelected();
            _state = State.Dragging;
        }

        private void CheckIfTileSelected()
        {
            var tileView = CastRayOnMousePosition();
            if (tileView && TileSelected != null)
            {
                TileSelected(tileView);
            }
        }

        private void CheckIfFirstTileSelected()
        {
            var tileView = CastRayOnMousePosition();
            if (tileView && FirstTileSelected != null)
            {
                FirstTileSelected(tileView);
            }
        }

        private TileView CastRayOnMousePosition()
        {
            Ray ray = _camera.ScreenPointToRay(Input.mousePosition);
            // TODO: optimize
            //Physics2D.RaycastNonAlloc(ray.origin, ray.direction, _raycastOut);
            RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, Mathf.Infinity);
            return hit ? hit.collider.gameObject.GetComponent<TileView>() : null;
        }
    }
}
