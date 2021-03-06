// GENERATED AUTOMATICALLY FROM 'Assets/Unity Generated Control Data/DefaultControls.inputactions'

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public class @DefaultControls : IInputActionCollection, IDisposable
{
    public InputActionAsset asset { get; }
    public @DefaultControls()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""DefaultControls"",
    ""maps"": [
        {
            ""name"": ""FPSCharacter"",
            ""id"": ""e13f0c68-812b-445b-8ce8-c4d5a3e797b7"",
            ""actions"": [
                {
                    ""name"": ""Walk"",
                    ""type"": ""Value"",
                    ""id"": ""ee345128-11f0-4e94-ac14-15e224b603f7"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Look"",
                    ""type"": ""Value"",
                    ""id"": ""f8470140-94bb-430d-bd62-330d667ee5b1"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Jump"",
                    ""type"": ""Button"",
                    ""id"": ""ab42c502-0dc8-45b3-9eac-0e51e2bc13a3"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Sprint"",
                    ""type"": ""Value"",
                    ""id"": ""d0f66fd4-84d7-4898-94d8-ca0f8a92d6be"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Interact"",
                    ""type"": ""Button"",
                    ""id"": ""0d725f9a-d0e2-4f87-9618-2778ba42c464"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""f2056263-50f0-4b0a-822a-125423194d3c"",
                    ""path"": ""<Gamepad>/leftStick"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""Walk"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""2D Vector"",
                    ""id"": ""68fb446a-7f25-45e6-9750-73a34f2a2f05"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Walk"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""a6c385a5-45e0-433d-9634-d0000da55d01"",
                    ""path"": ""<Keyboard>/w"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard and Mouse"",
                    ""action"": ""Walk"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""1aae9d78-fc24-481e-8826-d7358b1d4b1d"",
                    ""path"": ""<Keyboard>/s"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard and Mouse"",
                    ""action"": ""Walk"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""ffe869f8-6dfc-4fb1-af43-24a03abc1a27"",
                    ""path"": ""<Keyboard>/a"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard and Mouse"",
                    ""action"": ""Walk"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""5f5a26c6-f22b-4b7b-ac72-e81d70ab1eb3"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard and Mouse"",
                    ""action"": ""Walk"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""50e440ca-474e-4961-a709-cfd3bede31fd"",
                    ""path"": ""<Mouse>/delta"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard and Mouse"",
                    ""action"": ""Look"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""2e720d7e-63e0-4005-a95c-c02be0cf30c2"",
                    ""path"": ""<Gamepad>/rightStick"",
                    ""interactions"": """",
                    ""processors"": ""ScaleVector2(x=13,y=13)"",
                    ""groups"": ""Gamepad"",
                    ""action"": ""Look"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""d3b497ef-2ec5-47c0-b32b-614f43470778"",
                    ""path"": ""<Keyboard>/space"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard and Mouse"",
                    ""action"": ""Jump"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""442119d9-0260-4c49-af09-62c76d2f3914"",
                    ""path"": ""<Gamepad>/buttonSouth"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""Jump"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""55d7c76c-5aae-4f79-90e3-4f8dfbab337d"",
                    ""path"": ""<Keyboard>/leftShift"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard and Mouse"",
                    ""action"": ""Sprint"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""eaea9a4b-fd61-4cee-b5c6-9381176a6159"",
                    ""path"": ""<Gamepad>/rightTrigger"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""Sprint"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""0e2976cd-9bfa-49f8-9e4a-9312a467ed49"",
                    ""path"": ""<Mouse>/leftButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard and Mouse"",
                    ""action"": ""Interact"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""402a7016-5c65-40b1-99de-35d5c60b9b84"",
                    ""path"": ""<Gamepad>/buttonWest"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""Interact"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        },
        {
            ""name"": ""Global"",
            ""id"": ""fe1db6d0-986c-483a-81e1-6b46acc676aa"",
            ""actions"": [
                {
                    ""name"": ""Pause"",
                    ""type"": ""Button"",
                    ""id"": ""8292b56e-ef93-4f8b-9e3a-b6b4646b92e0"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""af039f94-7924-4a0b-a5b4-866da0fc4260"",
                    ""path"": ""<Gamepad>/start"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""Pause"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""9e1641d0-459f-4574-892d-b51e4a8de693"",
                    ""path"": ""<Keyboard>/escape"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard and Mouse"",
                    ""action"": ""Pause"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        },
        {
            ""name"": ""NoClipCharacter"",
            ""id"": ""8546e017-48d5-446d-9c4d-53446b05474a"",
            ""actions"": [
                {
                    ""name"": ""Look"",
                    ""type"": ""Value"",
                    ""id"": ""562cc132-389a-4a19-85a0-a32b8234de1c"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""MoveHorizontal"",
                    ""type"": ""Value"",
                    ""id"": ""72ded832-2b69-4aa2-a71f-96423f15ab26"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""MoveVertical"",
                    ""type"": ""Value"",
                    ""id"": ""ce0c5d0d-e3f3-4fb5-a46a-03ff3331c59b"",
                    ""expectedControlType"": ""Axis"",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""e1c59baf-9b6c-4189-bd0d-6d7645b12316"",
                    ""path"": ""<Mouse>/delta"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard and Mouse"",
                    ""action"": ""Look"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""b375b790-ba9e-45c6-b2c7-fee94f052786"",
                    ""path"": ""<Gamepad>/rightStick"",
                    ""interactions"": """",
                    ""processors"": ""ScaleVector2(x=40,y=40)"",
                    ""groups"": ""Gamepad"",
                    ""action"": ""Look"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""829aa7df-78f7-461a-84e4-ddb274ca98fb"",
                    ""path"": ""<Gamepad>/leftStick"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""MoveHorizontal"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""2D Vector"",
                    ""id"": ""8574f58b-773e-481c-a587-591352ca450d"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""MoveHorizontal"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""4e78096e-e313-4b10-94f0-acee1b52b633"",
                    ""path"": ""<Keyboard>/w"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard and Mouse"",
                    ""action"": ""MoveHorizontal"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""5799471b-7b54-4581-ac1e-c8e411d5b2d0"",
                    ""path"": ""<Keyboard>/s"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard and Mouse"",
                    ""action"": ""MoveHorizontal"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""31048556-f954-43a2-a0de-07d4c841831f"",
                    ""path"": ""<Keyboard>/a"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard and Mouse"",
                    ""action"": ""MoveHorizontal"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""4a910e0b-5a14-40c4-85f7-2c1210fa7d64"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard and Mouse"",
                    ""action"": ""MoveHorizontal"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""1D Axis"",
                    ""id"": ""f5b5d07d-fdd7-492a-affa-43378ae8083a"",
                    ""path"": ""1DAxis"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""MoveVertical"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""negative"",
                    ""id"": ""0153c601-184e-4962-b02f-d0ea8f5c161a"",
                    ""path"": ""<Gamepad>/leftTrigger"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""MoveVertical"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""positive"",
                    ""id"": ""9eab95cf-933c-4cd6-9ab8-a236119e1669"",
                    ""path"": ""<Gamepad>/rightTrigger"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Gamepad"",
                    ""action"": ""MoveVertical"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""1D Axis"",
                    ""id"": ""7dc05ed5-c707-4449-9280-7ed3a4185f1b"",
                    ""path"": ""1DAxis"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""MoveVertical"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""negative"",
                    ""id"": ""6b09ddba-d5a9-450a-a078-fb8dec8a61bd"",
                    ""path"": ""<Keyboard>/q"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard and Mouse"",
                    ""action"": ""MoveVertical"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""positive"",
                    ""id"": ""d413fb4b-3241-4e60-8424-bc31ff4cd995"",
                    ""path"": ""<Keyboard>/e"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard and Mouse"",
                    ""action"": ""MoveVertical"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                }
            ]
        }
    ],
    ""controlSchemes"": [
        {
            ""name"": ""Keyboard and Mouse"",
            ""bindingGroup"": ""Keyboard and Mouse"",
            ""devices"": [
                {
                    ""devicePath"": ""<Keyboard>"",
                    ""isOptional"": false,
                    ""isOR"": false
                },
                {
                    ""devicePath"": ""<Mouse>"",
                    ""isOptional"": false,
                    ""isOR"": false
                }
            ]
        },
        {
            ""name"": ""Gamepad"",
            ""bindingGroup"": ""Gamepad"",
            ""devices"": [
                {
                    ""devicePath"": ""<Gamepad>"",
                    ""isOptional"": false,
                    ""isOR"": false
                }
            ]
        }
    ]
}");
        // FPSCharacter
        m_FPSCharacter = asset.FindActionMap("FPSCharacter", throwIfNotFound: true);
        m_FPSCharacter_Walk = m_FPSCharacter.FindAction("Walk", throwIfNotFound: true);
        m_FPSCharacter_Look = m_FPSCharacter.FindAction("Look", throwIfNotFound: true);
        m_FPSCharacter_Jump = m_FPSCharacter.FindAction("Jump", throwIfNotFound: true);
        m_FPSCharacter_Sprint = m_FPSCharacter.FindAction("Sprint", throwIfNotFound: true);
        m_FPSCharacter_Interact = m_FPSCharacter.FindAction("Interact", throwIfNotFound: true);
        // Global
        m_Global = asset.FindActionMap("Global", throwIfNotFound: true);
        m_Global_Pause = m_Global.FindAction("Pause", throwIfNotFound: true);
        // NoClipCharacter
        m_NoClipCharacter = asset.FindActionMap("NoClipCharacter", throwIfNotFound: true);
        m_NoClipCharacter_Look = m_NoClipCharacter.FindAction("Look", throwIfNotFound: true);
        m_NoClipCharacter_MoveHorizontal = m_NoClipCharacter.FindAction("MoveHorizontal", throwIfNotFound: true);
        m_NoClipCharacter_MoveVertical = m_NoClipCharacter.FindAction("MoveVertical", throwIfNotFound: true);
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

    // FPSCharacter
    private readonly InputActionMap m_FPSCharacter;
    private IFPSCharacterActions m_FPSCharacterActionsCallbackInterface;
    private readonly InputAction m_FPSCharacter_Walk;
    private readonly InputAction m_FPSCharacter_Look;
    private readonly InputAction m_FPSCharacter_Jump;
    private readonly InputAction m_FPSCharacter_Sprint;
    private readonly InputAction m_FPSCharacter_Interact;
    public struct FPSCharacterActions
    {
        private @DefaultControls m_Wrapper;
        public FPSCharacterActions(@DefaultControls wrapper) { m_Wrapper = wrapper; }
        public InputAction @Walk => m_Wrapper.m_FPSCharacter_Walk;
        public InputAction @Look => m_Wrapper.m_FPSCharacter_Look;
        public InputAction @Jump => m_Wrapper.m_FPSCharacter_Jump;
        public InputAction @Sprint => m_Wrapper.m_FPSCharacter_Sprint;
        public InputAction @Interact => m_Wrapper.m_FPSCharacter_Interact;
        public InputActionMap Get() { return m_Wrapper.m_FPSCharacter; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(FPSCharacterActions set) { return set.Get(); }
        public void SetCallbacks(IFPSCharacterActions instance)
        {
            if (m_Wrapper.m_FPSCharacterActionsCallbackInterface != null)
            {
                @Walk.started -= m_Wrapper.m_FPSCharacterActionsCallbackInterface.OnWalk;
                @Walk.performed -= m_Wrapper.m_FPSCharacterActionsCallbackInterface.OnWalk;
                @Walk.canceled -= m_Wrapper.m_FPSCharacterActionsCallbackInterface.OnWalk;
                @Look.started -= m_Wrapper.m_FPSCharacterActionsCallbackInterface.OnLook;
                @Look.performed -= m_Wrapper.m_FPSCharacterActionsCallbackInterface.OnLook;
                @Look.canceled -= m_Wrapper.m_FPSCharacterActionsCallbackInterface.OnLook;
                @Jump.started -= m_Wrapper.m_FPSCharacterActionsCallbackInterface.OnJump;
                @Jump.performed -= m_Wrapper.m_FPSCharacterActionsCallbackInterface.OnJump;
                @Jump.canceled -= m_Wrapper.m_FPSCharacterActionsCallbackInterface.OnJump;
                @Sprint.started -= m_Wrapper.m_FPSCharacterActionsCallbackInterface.OnSprint;
                @Sprint.performed -= m_Wrapper.m_FPSCharacterActionsCallbackInterface.OnSprint;
                @Sprint.canceled -= m_Wrapper.m_FPSCharacterActionsCallbackInterface.OnSprint;
                @Interact.started -= m_Wrapper.m_FPSCharacterActionsCallbackInterface.OnInteract;
                @Interact.performed -= m_Wrapper.m_FPSCharacterActionsCallbackInterface.OnInteract;
                @Interact.canceled -= m_Wrapper.m_FPSCharacterActionsCallbackInterface.OnInteract;
            }
            m_Wrapper.m_FPSCharacterActionsCallbackInterface = instance;
            if (instance != null)
            {
                @Walk.started += instance.OnWalk;
                @Walk.performed += instance.OnWalk;
                @Walk.canceled += instance.OnWalk;
                @Look.started += instance.OnLook;
                @Look.performed += instance.OnLook;
                @Look.canceled += instance.OnLook;
                @Jump.started += instance.OnJump;
                @Jump.performed += instance.OnJump;
                @Jump.canceled += instance.OnJump;
                @Sprint.started += instance.OnSprint;
                @Sprint.performed += instance.OnSprint;
                @Sprint.canceled += instance.OnSprint;
                @Interact.started += instance.OnInteract;
                @Interact.performed += instance.OnInteract;
                @Interact.canceled += instance.OnInteract;
            }
        }
    }
    public FPSCharacterActions @FPSCharacter => new FPSCharacterActions(this);

    // Global
    private readonly InputActionMap m_Global;
    private IGlobalActions m_GlobalActionsCallbackInterface;
    private readonly InputAction m_Global_Pause;
    public struct GlobalActions
    {
        private @DefaultControls m_Wrapper;
        public GlobalActions(@DefaultControls wrapper) { m_Wrapper = wrapper; }
        public InputAction @Pause => m_Wrapper.m_Global_Pause;
        public InputActionMap Get() { return m_Wrapper.m_Global; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(GlobalActions set) { return set.Get(); }
        public void SetCallbacks(IGlobalActions instance)
        {
            if (m_Wrapper.m_GlobalActionsCallbackInterface != null)
            {
                @Pause.started -= m_Wrapper.m_GlobalActionsCallbackInterface.OnPause;
                @Pause.performed -= m_Wrapper.m_GlobalActionsCallbackInterface.OnPause;
                @Pause.canceled -= m_Wrapper.m_GlobalActionsCallbackInterface.OnPause;
            }
            m_Wrapper.m_GlobalActionsCallbackInterface = instance;
            if (instance != null)
            {
                @Pause.started += instance.OnPause;
                @Pause.performed += instance.OnPause;
                @Pause.canceled += instance.OnPause;
            }
        }
    }
    public GlobalActions @Global => new GlobalActions(this);

    // NoClipCharacter
    private readonly InputActionMap m_NoClipCharacter;
    private INoClipCharacterActions m_NoClipCharacterActionsCallbackInterface;
    private readonly InputAction m_NoClipCharacter_Look;
    private readonly InputAction m_NoClipCharacter_MoveHorizontal;
    private readonly InputAction m_NoClipCharacter_MoveVertical;
    public struct NoClipCharacterActions
    {
        private @DefaultControls m_Wrapper;
        public NoClipCharacterActions(@DefaultControls wrapper) { m_Wrapper = wrapper; }
        public InputAction @Look => m_Wrapper.m_NoClipCharacter_Look;
        public InputAction @MoveHorizontal => m_Wrapper.m_NoClipCharacter_MoveHorizontal;
        public InputAction @MoveVertical => m_Wrapper.m_NoClipCharacter_MoveVertical;
        public InputActionMap Get() { return m_Wrapper.m_NoClipCharacter; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(NoClipCharacterActions set) { return set.Get(); }
        public void SetCallbacks(INoClipCharacterActions instance)
        {
            if (m_Wrapper.m_NoClipCharacterActionsCallbackInterface != null)
            {
                @Look.started -= m_Wrapper.m_NoClipCharacterActionsCallbackInterface.OnLook;
                @Look.performed -= m_Wrapper.m_NoClipCharacterActionsCallbackInterface.OnLook;
                @Look.canceled -= m_Wrapper.m_NoClipCharacterActionsCallbackInterface.OnLook;
                @MoveHorizontal.started -= m_Wrapper.m_NoClipCharacterActionsCallbackInterface.OnMoveHorizontal;
                @MoveHorizontal.performed -= m_Wrapper.m_NoClipCharacterActionsCallbackInterface.OnMoveHorizontal;
                @MoveHorizontal.canceled -= m_Wrapper.m_NoClipCharacterActionsCallbackInterface.OnMoveHorizontal;
                @MoveVertical.started -= m_Wrapper.m_NoClipCharacterActionsCallbackInterface.OnMoveVertical;
                @MoveVertical.performed -= m_Wrapper.m_NoClipCharacterActionsCallbackInterface.OnMoveVertical;
                @MoveVertical.canceled -= m_Wrapper.m_NoClipCharacterActionsCallbackInterface.OnMoveVertical;
            }
            m_Wrapper.m_NoClipCharacterActionsCallbackInterface = instance;
            if (instance != null)
            {
                @Look.started += instance.OnLook;
                @Look.performed += instance.OnLook;
                @Look.canceled += instance.OnLook;
                @MoveHorizontal.started += instance.OnMoveHorizontal;
                @MoveHorizontal.performed += instance.OnMoveHorizontal;
                @MoveHorizontal.canceled += instance.OnMoveHorizontal;
                @MoveVertical.started += instance.OnMoveVertical;
                @MoveVertical.performed += instance.OnMoveVertical;
                @MoveVertical.canceled += instance.OnMoveVertical;
            }
        }
    }
    public NoClipCharacterActions @NoClipCharacter => new NoClipCharacterActions(this);
    private int m_KeyboardandMouseSchemeIndex = -1;
    public InputControlScheme KeyboardandMouseScheme
    {
        get
        {
            if (m_KeyboardandMouseSchemeIndex == -1) m_KeyboardandMouseSchemeIndex = asset.FindControlSchemeIndex("Keyboard and Mouse");
            return asset.controlSchemes[m_KeyboardandMouseSchemeIndex];
        }
    }
    private int m_GamepadSchemeIndex = -1;
    public InputControlScheme GamepadScheme
    {
        get
        {
            if (m_GamepadSchemeIndex == -1) m_GamepadSchemeIndex = asset.FindControlSchemeIndex("Gamepad");
            return asset.controlSchemes[m_GamepadSchemeIndex];
        }
    }
    public interface IFPSCharacterActions
    {
        void OnWalk(InputAction.CallbackContext context);
        void OnLook(InputAction.CallbackContext context);
        void OnJump(InputAction.CallbackContext context);
        void OnSprint(InputAction.CallbackContext context);
        void OnInteract(InputAction.CallbackContext context);
    }
    public interface IGlobalActions
    {
        void OnPause(InputAction.CallbackContext context);
    }
    public interface INoClipCharacterActions
    {
        void OnLook(InputAction.CallbackContext context);
        void OnMoveHorizontal(InputAction.CallbackContext context);
        void OnMoveVertical(InputAction.CallbackContext context);
    }
}
