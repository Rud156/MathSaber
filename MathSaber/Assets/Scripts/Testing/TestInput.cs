using UnityEngine;
using Valve.VR;

namespace Testing
{
    public class TestInput : MonoBehaviour
    {
        public SteamVR_Action_Single squeezeAction;
        public SteamVR_Action_Vector2 touchPadAction;

        private void Update()
        {
            if (SteamVR_Actions.default_Teleport.GetStateDown(SteamVR_Input_Sources.Any))
            {
                Debug.Log("Teleport Button Clicked");
            }

            if (SteamVR_Actions.default_GrabPinch.GetStateUp(SteamVR_Input_Sources.Any))
            {
                Debug.Log("Grab Button Clicked");
            }

            float triggerValue = squeezeAction.GetAxis(SteamVR_Input_Sources.Any);
            if (triggerValue != 0)
            {
                Debug.Log($"Trigger Value: {triggerValue}");
            }

            Vector2 touchPadValue = touchPadAction.GetAxis(SteamVR_Input_Sources.Any);
            if (touchPadValue != Vector2.zero)
            {
                Debug.Log($"Touch Pad Value: {touchPadValue}");
            }
        }
    }
}