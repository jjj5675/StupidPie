// GENERATED AUTOMATICALLY FROM 'Assets/Scripts/PlayerControl.inputactions'

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public class @PlayerControl : IInputActionCollection, IDisposable
{
    public InputActionAsset asset { get; }
    public @PlayerControl()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""PlayerControl"",
    ""maps"": [
        {
            ""name"": ""GamePad"",
            ""id"": ""0439c8ce-6507-45f3-bcc1-a4ef83f2927a"",
            ""actions"": [
                {
                    ""name"": ""PadDown"",
                    ""type"": ""Button"",
                    ""id"": ""abe3669c-570b-4b59-8eb2-6e3577dc684c"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""PadUp"",
                    ""type"": ""Button"",
                    ""id"": ""c56ddca5-69c0-4720-8979-d05e9f8e30b6"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""PadLeft"",
                    ""type"": ""Button"",
                    ""id"": ""7f762380-1a22-457c-9cd7-a979f3508375"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""PadRight"",
                    ""type"": ""Button"",
                    ""id"": ""68bf43aa-f0e6-40fe-877c-6a3996736054"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""PadBack"",
                    ""type"": ""Button"",
                    ""id"": ""f8d5fe50-24b3-426f-8960-ef046953c220"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""PadStart"",
                    ""type"": ""Button"",
                    ""id"": ""b8371887-ae89-46ad-a615-d0de551fe27e"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""PadA"",
                    ""type"": ""Button"",
                    ""id"": ""84abcae5-1407-49ac-ad65-3d7c87934ceb"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""PadB"",
                    ""type"": ""Button"",
                    ""id"": ""000ae063-71ea-4e42-91ce-83042dfed6d4"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""d1425433-49ae-4a83-a8a7-998d44ad86b8"",
                    ""path"": ""<Gamepad>/dpad/down"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""PadDown"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""b8664b68-7b9b-4bb1-983d-af764a7d37ab"",
                    ""path"": ""<Gamepad>/dpad/up"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""PadUp"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""5313fc42-d77a-4ee5-a896-359dc9b7cd9c"",
                    ""path"": ""<Gamepad>/dpad/left"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""PadLeft"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""7a8fcaf2-c2e3-47fb-a504-c8ea08b4bfaa"",
                    ""path"": ""<Gamepad>/dpad/right"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""PadRight"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""8bfd8425-8cc7-4fa8-9f5e-1c86e8a290b8"",
                    ""path"": ""<Gamepad>/select"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""PadBack"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""3763d800-0b78-48ac-85a5-8271176531f6"",
                    ""path"": ""<Gamepad>/start"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""PadStart"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""92067c17-35f1-4b65-9738-0e51a8772f12"",
                    ""path"": ""<Gamepad>/buttonSouth"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""PadA"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""30deee78-f761-4f1e-afb2-71ccb8b4676a"",
                    ""path"": ""<Gamepad>/buttonEast"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""PadB"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": []
}");
        // GamePad
        m_GamePad = asset.FindActionMap("GamePad", throwIfNotFound: true);
        m_GamePad_PadDown = m_GamePad.FindAction("PadDown", throwIfNotFound: true);
        m_GamePad_PadUp = m_GamePad.FindAction("PadUp", throwIfNotFound: true);
        m_GamePad_PadLeft = m_GamePad.FindAction("PadLeft", throwIfNotFound: true);
        m_GamePad_PadRight = m_GamePad.FindAction("PadRight", throwIfNotFound: true);
        m_GamePad_PadBack = m_GamePad.FindAction("PadBack", throwIfNotFound: true);
        m_GamePad_PadStart = m_GamePad.FindAction("PadStart", throwIfNotFound: true);
        m_GamePad_PadA = m_GamePad.FindAction("PadA", throwIfNotFound: true);
        m_GamePad_PadB = m_GamePad.FindAction("PadB", throwIfNotFound: true);
    }

    public void Dispose()
    {
        UnityEngine.Object.Destroy(asset);
    }

    public InputBinding? bindingMask
    {
        get => asset.bindingMask;
        set => asset.bindingMask = value;
    }

    public ReadOnlyArray<InputDevice>? devices
    {
        get => asset.devices;
        set => asset.devices = value;
    }

    public ReadOnlyArray<InputControlScheme> controlSchemes => asset.controlSchemes;

    public bool Contains(InputAction action)
    {
        return asset.Contains(action);
    }

    public IEnumerator<InputAction> GetEnumerator()
    {
        return asset.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void Enable()
    {
        asset.Enable();
    }

    public void Disable()
    {
        asset.Disable();
    }

    // GamePad
    private readonly InputActionMap m_GamePad;
    private IGamePadActions m_GamePadActionsCallbackInterface;
    private readonly InputAction m_GamePad_PadDown;
    private readonly InputAction m_GamePad_PadUp;
    private readonly InputAction m_GamePad_PadLeft;
    private readonly InputAction m_GamePad_PadRight;
    private readonly InputAction m_GamePad_PadBack;
    private readonly InputAction m_GamePad_PadStart;
    private readonly InputAction m_GamePad_PadA;
    private readonly InputAction m_GamePad_PadB;
    public struct GamePadActions
    {
        private @PlayerControl m_Wrapper;
        public GamePadActions(@PlayerControl wrapper) { m_Wrapper = wrapper; }
        public InputAction @PadDown => m_Wrapper.m_GamePad_PadDown;
        public InputAction @PadUp => m_Wrapper.m_GamePad_PadUp;
        public InputAction @PadLeft => m_Wrapper.m_GamePad_PadLeft;
        public InputAction @PadRight => m_Wrapper.m_GamePad_PadRight;
        public InputAction @PadBack => m_Wrapper.m_GamePad_PadBack;
        public InputAction @PadStart => m_Wrapper.m_GamePad_PadStart;
        public InputAction @PadA => m_Wrapper.m_GamePad_PadA;
        public InputAction @PadB => m_Wrapper.m_GamePad_PadB;
        public InputActionMap Get() { return m_Wrapper.m_GamePad; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(GamePadActions set) { return set.Get(); }
        public void SetCallbacks(IGamePadActions instance)
        {
            if (m_Wrapper.m_GamePadActionsCallbackInterface != null)
            {
                @PadDown.started -= m_Wrapper.m_GamePadActionsCallbackInterface.OnPadDown;
                @PadDown.performed -= m_Wrapper.m_GamePadActionsCallbackInterface.OnPadDown;
                @PadDown.canceled -= m_Wrapper.m_GamePadActionsCallbackInterface.OnPadDown;
                @PadUp.started -= m_Wrapper.m_GamePadActionsCallbackInterface.OnPadUp;
                @PadUp.performed -= m_Wrapper.m_GamePadActionsCallbackInterface.OnPadUp;
                @PadUp.canceled -= m_Wrapper.m_GamePadActionsCallbackInterface.OnPadUp;
                @PadLeft.started -= m_Wrapper.m_GamePadActionsCallbackInterface.OnPadLeft;
                @PadLeft.performed -= m_Wrapper.m_GamePadActionsCallbackInterface.OnPadLeft;
                @PadLeft.canceled -= m_Wrapper.m_GamePadActionsCallbackInterface.OnPadLeft;
                @PadRight.started -= m_Wrapper.m_GamePadActionsCallbackInterface.OnPadRight;
                @PadRight.performed -= m_Wrapper.m_GamePadActionsCallbackInterface.OnPadRight;
                @PadRight.canceled -= m_Wrapper.m_GamePadActionsCallbackInterface.OnPadRight;
                @PadBack.started -= m_Wrapper.m_GamePadActionsCallbackInterface.OnPadBack;
                @PadBack.performed -= m_Wrapper.m_GamePadActionsCallbackInterface.OnPadBack;
                @PadBack.canceled -= m_Wrapper.m_GamePadActionsCallbackInterface.OnPadBack;
                @PadStart.started -= m_Wrapper.m_GamePadActionsCallbackInterface.OnPadStart;
                @PadStart.performed -= m_Wrapper.m_GamePadActionsCallbackInterface.OnPadStart;
                @PadStart.canceled -= m_Wrapper.m_GamePadActionsCallbackInterface.OnPadStart;
                @PadA.started -= m_Wrapper.m_GamePadActionsCallbackInterface.OnPadA;
                @PadA.performed -= m_Wrapper.m_GamePadActionsCallbackInterface.OnPadA;
                @PadA.canceled -= m_Wrapper.m_GamePadActionsCallbackInterface.OnPadA;
                @PadB.started -= m_Wrapper.m_GamePadActionsCallbackInterface.OnPadB;
                @PadB.performed -= m_Wrapper.m_GamePadActionsCallbackInterface.OnPadB;
                @PadB.canceled -= m_Wrapper.m_GamePadActionsCallbackInterface.OnPadB;
            }
            m_Wrapper.m_GamePadActionsCallbackInterface = instance;
            if (instance != null)
            {
                @PadDown.started += instance.OnPadDown;
                @PadDown.performed += instance.OnPadDown;
                @PadDown.canceled += instance.OnPadDown;
                @PadUp.started += instance.OnPadUp;
                @PadUp.performed += instance.OnPadUp;
                @PadUp.canceled += instance.OnPadUp;
                @PadLeft.started += instance.OnPadLeft;
                @PadLeft.performed += instance.OnPadLeft;
                @PadLeft.canceled += instance.OnPadLeft;
                @PadRight.started += instance.OnPadRight;
                @PadRight.performed += instance.OnPadRight;
                @PadRight.canceled += instance.OnPadRight;
                @PadBack.started += instance.OnPadBack;
                @PadBack.performed += instance.OnPadBack;
                @PadBack.canceled += instance.OnPadBack;
                @PadStart.started += instance.OnPadStart;
                @PadStart.performed += instance.OnPadStart;
                @PadStart.canceled += instance.OnPadStart;
                @PadA.started += instance.OnPadA;
                @PadA.performed += instance.OnPadA;
                @PadA.canceled += instance.OnPadA;
                @PadB.started += instance.OnPadB;
                @PadB.performed += instance.OnPadB;
                @PadB.canceled += instance.OnPadB;
            }
        }
    }
    public GamePadActions @GamePad => new GamePadActions(this);
    public interface IGamePadActions
    {
        void OnPadDown(InputAction.CallbackContext context);
        void OnPadUp(InputAction.CallbackContext context);
        void OnPadLeft(InputAction.CallbackContext context);
        void OnPadRight(InputAction.CallbackContext context);
        void OnPadBack(InputAction.CallbackContext context);
        void OnPadStart(InputAction.CallbackContext context);
        void OnPadA(InputAction.CallbackContext context);
        void OnPadB(InputAction.CallbackContext context);
    }
}
