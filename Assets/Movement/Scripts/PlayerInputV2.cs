// GENERATED AUTOMATICALLY FROM 'Assets/Movement/Scripts/PlayerInputV2.inputactions'

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public class @PlayerInputV2 : IInputActionCollection, IDisposable
{
    public InputActionAsset asset { get; }
    public @PlayerInputV2()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""PlayerInputV2"",
    ""maps"": [
        {
            ""name"": ""CharacterControlls"",
            ""id"": ""6793b76a-eb1f-4781-bd21-b29c4a1ae732"",
            ""actions"": [
                {
                    ""name"": ""Move"",
                    ""type"": ""Value"",
                    ""id"": ""51698f4f-9550-49c2-8c94-aab1a4be04ad"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": ""NormalizeVector2"",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Dodge"",
                    ""type"": ""Button"",
                    ""id"": ""3afc821b-39a9-44dc-9763-1bb2609a0ca2"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": ""NormalizeVector2"",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Ultimate"",
                    ""type"": ""Button"",
                    ""id"": ""a81a749e-c0fc-4f5a-8c9a-01295cdce810"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": ""NormalizeVector2"",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Attack"",
                    ""type"": ""Button"",
                    ""id"": ""f8671606-ba7a-4e45-ab29-4f96aeb9b677"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": ""NormalizeVector2"",
                    ""interactions"": ""Press""
                }
            ],
            ""bindings"": [
                {
                    ""name"": ""Keyboard"",
                    ""id"": ""984f2d9b-ec65-47fc-af1f-872a9ad697d5"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""02815d2b-13cc-4ebb-8181-2cb2897ee24b"",
                    ""path"": ""<Keyboard>/w"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""8de876dd-eef6-44c0-8b11-c074f9387c6e"",
                    ""path"": ""<Keyboard>/s"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""bdd0d842-86ee-40fa-a50d-2fe429479e61"",
                    ""path"": ""<Keyboard>/a"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""35e6e2e2-6115-4b36-a0cf-650a125c5393"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""8a6ab5ef-2d77-4232-98ef-a9c4bb0a8477"",
                    ""path"": ""<Keyboard>/shift"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Dodge"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""9bd310da-7c11-47a8-aac5-f476de1588aa"",
                    ""path"": ""<Keyboard>/r"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Ultimate"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""2055075a-781e-4096-b412-8c66905ca9a5"",
                    ""path"": ""<Keyboard>/q"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Attack"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": []
}");
        // CharacterControlls
        m_CharacterControlls = asset.FindActionMap("CharacterControlls", throwIfNotFound: true);
        m_CharacterControlls_Move = m_CharacterControlls.FindAction("Move", throwIfNotFound: true);
        m_CharacterControlls_Dodge = m_CharacterControlls.FindAction("Dodge", throwIfNotFound: true);
        m_CharacterControlls_Ultimate = m_CharacterControlls.FindAction("Ultimate", throwIfNotFound: true);
        m_CharacterControlls_Attack = m_CharacterControlls.FindAction("Attack", throwIfNotFound: true);
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

    // CharacterControlls
    private readonly InputActionMap m_CharacterControlls;
    private ICharacterControllsActions m_CharacterControllsActionsCallbackInterface;
    private readonly InputAction m_CharacterControlls_Move;
    private readonly InputAction m_CharacterControlls_Dodge;
    private readonly InputAction m_CharacterControlls_Ultimate;
    private readonly InputAction m_CharacterControlls_Attack;
    public struct CharacterControllsActions
    {
        private @PlayerInputV2 m_Wrapper;
        public CharacterControllsActions(@PlayerInputV2 wrapper) { m_Wrapper = wrapper; }
        public InputAction @Move => m_Wrapper.m_CharacterControlls_Move;
        public InputAction @Dodge => m_Wrapper.m_CharacterControlls_Dodge;
        public InputAction @Ultimate => m_Wrapper.m_CharacterControlls_Ultimate;
        public InputAction @Attack => m_Wrapper.m_CharacterControlls_Attack;
        public InputActionMap Get() { return m_Wrapper.m_CharacterControlls; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(CharacterControllsActions set) { return set.Get(); }
        public void SetCallbacks(ICharacterControllsActions instance)
        {
            if (m_Wrapper.m_CharacterControllsActionsCallbackInterface != null)
            {
                @Move.started -= m_Wrapper.m_CharacterControllsActionsCallbackInterface.OnMove;
                @Move.performed -= m_Wrapper.m_CharacterControllsActionsCallbackInterface.OnMove;
                @Move.canceled -= m_Wrapper.m_CharacterControllsActionsCallbackInterface.OnMove;
                @Dodge.started -= m_Wrapper.m_CharacterControllsActionsCallbackInterface.OnDodge;
                @Dodge.performed -= m_Wrapper.m_CharacterControllsActionsCallbackInterface.OnDodge;
                @Dodge.canceled -= m_Wrapper.m_CharacterControllsActionsCallbackInterface.OnDodge;
                @Ultimate.started -= m_Wrapper.m_CharacterControllsActionsCallbackInterface.OnUltimate;
                @Ultimate.performed -= m_Wrapper.m_CharacterControllsActionsCallbackInterface.OnUltimate;
                @Ultimate.canceled -= m_Wrapper.m_CharacterControllsActionsCallbackInterface.OnUltimate;
                @Attack.started -= m_Wrapper.m_CharacterControllsActionsCallbackInterface.OnAttack;
                @Attack.performed -= m_Wrapper.m_CharacterControllsActionsCallbackInterface.OnAttack;
                @Attack.canceled -= m_Wrapper.m_CharacterControllsActionsCallbackInterface.OnAttack;
            }
            m_Wrapper.m_CharacterControllsActionsCallbackInterface = instance;
            if (instance != null)
            {
                @Move.started += instance.OnMove;
                @Move.performed += instance.OnMove;
                @Move.canceled += instance.OnMove;
                @Dodge.started += instance.OnDodge;
                @Dodge.performed += instance.OnDodge;
                @Dodge.canceled += instance.OnDodge;
                @Ultimate.started += instance.OnUltimate;
                @Ultimate.performed += instance.OnUltimate;
                @Ultimate.canceled += instance.OnUltimate;
                @Attack.started += instance.OnAttack;
                @Attack.performed += instance.OnAttack;
                @Attack.canceled += instance.OnAttack;
            }
        }
    }
    public CharacterControllsActions @CharacterControlls => new CharacterControllsActions(this);
    public interface ICharacterControllsActions
    {
        void OnMove(InputAction.CallbackContext context);
        void OnDodge(InputAction.CallbackContext context);
        void OnUltimate(InputAction.CallbackContext context);
        void OnAttack(InputAction.CallbackContext context);
    }
}
