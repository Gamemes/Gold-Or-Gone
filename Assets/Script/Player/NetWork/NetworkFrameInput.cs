using System;
using UnityEngine;
using Mirror;

namespace Player.Network
{
    [RequireComponent(typeof(FrameInput))]
    public class NetworkFrameInput : NetworkBehaviour
    {
        public struct InputSyncValue
        {
            public bool Jump;
            public float Horizontal;
            public float Vertical;
            public InputSyncValue(FrameInput val)
            {
                this.Jump = val.Jump;
                this.Horizontal = val.Horizontal;
                this.Vertical = val.Vertical;
            }
            public void update(FrameInput val)
            {
                this.Jump = val.Jump;
                this.Horizontal = val.Horizontal;
                this.Vertical = val.Vertical;
            }
        }
        private FrameInput frameInput;
        private InputSyncValue input;
        private void Start()
        {
            frameInput = GetComponent<FrameInput>();
            input = new InputSyncValue(frameInput);
            Debug.Assert(frameInput != null);
        }
        bool needSync()
        {
            bool need = false;
            if (input.Jump != frameInput.Jump)
                need = true;
            if (input.Horizontal != frameInput.Horizontal)
                need = true;
            if (input.Vertical != frameInput.Vertical)
                need = true;
            if (need)
                input.update(frameInput);
            return need;
        }
        private void Update()
        {
            if (isLocalPlayer)
            {
                input.update(frameInput);
                CmdSyncInput(input);
            }
        }

        [Command]
        void CmdSyncInput(InputSyncValue val)
        {
            RpcSyncInput(val);
        }
        [ClientRpc]
        void RpcSyncInput(InputSyncValue val)
        {
            if (frameInput && !isLocalPlayer)
            {
                frameInput.Jump = val.Jump;
                frameInput.Horizontal = val.Horizontal;
                frameInput.Vertical = val.Vertical;
            }
        }
    }
}