using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Farm.Input
{
    public class InputManager : MonoSingleton<InputManager>
    {
        #region Fields

        private Vector2 movement;
        private bool leftShift;
        private bool spaceTalk;
        private bool bPack;
        private bool canInput;
        private bool mouseRight;
        private bool esc;
        private bool keyI;
        private bool keyG;
        private bool mouseLeftDown;

        #endregion

        #region SetsAndGets

        public Vector2 GetMovement() => movement;
        public bool GetLeftShift() => leftShift;
        public bool GetSpaceTalk() => spaceTalk;
        public bool GetCanInput() => canInput;
        public bool GetMouseRight() => mouseRight;
        public bool GetEscDown() => esc;
        public bool GetbPack() => bPack;
        public bool GetI() => keyI;
        public bool GetG() => keyG;
        public bool GetMouseLeftDown() => mouseLeftDown;

        #endregion

        protected override void Awake()
        {
            base.Awake();
            canInput = true;
        }

        private void Update()
        {
            if (canInput)
                UpdateInput();
            else
                InputInit();
        }

        private void OnEnable()
        {
            
        }
        private void OnDisable()
        {

        }

        private void UpdateInput()
        {
            movement = new Vector2(UnityEngine.Input.GetAxisRaw("Horizontal"), UnityEngine.Input.GetAxisRaw("Vertical"));
            leftShift = UnityEngine.Input.GetKey(KeyCode.LeftShift);
            bPack = UnityEngine.Input.GetKeyDown(KeyCode.B);
            spaceTalk = UnityEngine.Input.GetKeyDown(KeyCode.Space);
            mouseRight = UnityEngine.Input.GetMouseButtonDown(1);
            esc = UnityEngine.Input.GetKeyDown(KeyCode.Escape);
            keyI = UnityEngine.Input.GetKeyDown(KeyCode.I);
            keyG = UnityEngine.Input.GetKeyDown(KeyCode.G);
            mouseLeftDown = UnityEngine.Input.GetMouseButtonDown(0);
        }

        private void InputInit()
        {
            movement = Vector2.zero;
            leftShift = false;
            bPack = false;
            spaceTalk = false;
            keyI = false;
            keyG = false;
        }
    }
}
