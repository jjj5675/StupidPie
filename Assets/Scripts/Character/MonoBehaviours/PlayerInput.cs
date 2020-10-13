using UnityEngine;

public class PlayerInput : InputComponent
{
    public InputButton DebugMenuOpen = new InputButton(KeyCode.F12);
    public InputButton Restage = new InputButton(KeyCode.F5);
    public InputButton ManualOpen = new InputButton(KeyCode.Tab);
    public InputButton Pause = new InputButton(KeyCode.Escape);
    public InputButton Jump = new InputButton(KeyCode.Z);
    public InputButton Dash = new InputButton(KeyCode.X);
    public InputButton Interact = new InputButton(KeyCode.V);
    public InputAxis Horizontal = new InputAxis(KeyCode.RightArrow, KeyCode.LeftArrow);
    public InputAxis Vertical = new InputAxis(KeyCode.UpArrow, KeyCode.DownArrow);

    protected bool m_HaveControl = true;

    public bool HaveControl { get { return m_HaveControl; } }

    protected override void GetInputs(bool fixedUpdateHappened)
    {
        Pause.Get(fixedUpdateHappened);
        ManualOpen.Get(fixedUpdateHappened);
        Jump.Get(fixedUpdateHappened);
        Dash.Get(fixedUpdateHappened);
        Interact.Get(fixedUpdateHappened);
        Horizontal.Get();
        Vertical.Get();
        Restage.Get(fixedUpdateHappened);
        DebugMenuOpen.Get(fixedUpdateHappened);
    }

    public override void GainControl()
    {
        m_HaveControl = true;

        GainControl(Pause);
        GainControl(Restage);
        GainControl(ManualOpen);
        GainControl(Jump);
        GainControl(Dash);
        GainControl(Interact);
        GainControl(Horizontal);
        GainControl(Vertical);

        GainControl(DebugMenuOpen);
    }

    public override void ReleaseControl(bool resetValuse = true)
    {
        m_HaveControl = false;

        ReleaseControl(Pause, resetValuse);
        ReleaseControl(Restage, resetValuse);
        ReleaseControl(ManualOpen, resetValuse);
        ReleaseControl(Jump, resetValuse);
        ReleaseControl(Dash, resetValuse);
        ReleaseControl(Interact, resetValuse);
        ReleaseControl(Horizontal, resetValuse);
        ReleaseControl(Vertical, resetValuse);

        ReleaseControl(DebugMenuOpen, resetValuse);
    }

    public bool CheckAxisInputsNone()
    {
        return !Horizontal.ReceivingInput && !Vertical.ReceivingInput ? true : false;
    }

    public Vector2 AxisInputsValue()
    {
        return new Vector2(Horizontal.Value, Vertical.Value);
    }
}
