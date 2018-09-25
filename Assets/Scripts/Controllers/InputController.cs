using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace QuickMafs
{
    public class InputController : IInitializable, ITickable
    {
        private const int LEFT_MOUSE_BUTTON = 0;
        private Camera _camera;

        private State _state = State.Waiting;

        List<GameObject> _objectList = new List<GameObject>();

        public void Initialize()
        {
            _camera = Camera.main;
        }

        public void Tick()
        {
            if (Input.GetMouseButtonDown(LEFT_MOUSE_BUTTON))
            {
                _state = State.MouseDown;
            }
            else if (Input.GetMouseButtonUp(LEFT_MOUSE_BUTTON))
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
            Debug.Log("Mouse is up");
            for (int i = 0; i < _objectList.Count; i++)
            {
                Debug.Log(_objectList[i].name);
            }
            _objectList.Clear();
            _state = State.Waiting;
        }

        private void ProcessDragging()
        {
            var go = CastRayOnMousePosition();
            if (go && !_objectList.Contains(go))
            {
                _objectList.Add(go);
            }
        }

        private void ProcessMouseDown()
        {
            Debug.Log("Mouse is down");

            var go = CastRayOnMousePosition();
            if (go)
            {
                _objectList.Add(go);
            }

            _state = State.Dragging;
        }

        private GameObject CastRayOnMousePosition()
        {
            Ray ray = _camera.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, Mathf.Infinity);
            return hit ? hit.collider.gameObject : null;
        }

        public enum State
        {
            MouseDown, Dragging, MouseUp, Waiting
        }
    }
}
