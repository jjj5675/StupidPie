using UnityEngine;

public class PlayerInput : InputComponent
{
    public InputButton Jump = new InputButton(KeyCode.Z);
    public InputButton Dash = new InputButton(KeyCode.X);
    public InputAxis Horizontal = new InputAxis(KeyCode.RightArrow, KeyCode.LeftArrow);
    public InputAxis Vertical = new InputAxis(KeyCode.UpArrow, KeyCode.DownArrow);

    protected bool m_HaveControl = true;

    public bool HaveControl { get { return m_HaveControl; } }

    protected override void GetInputs(bool fixedUpdateHappened)
    {
        Jump.Get(fixedUpdateHappened);
        Dash.Get(fixedUpdateHappened);
        Horizontal.Get();
        Vertical.Get();
    }

    public override void GainControl()
    {
        m_HaveControl = true;

        GainControl(Jump);
        GainControl(Dash);
        GainControl(Horizontal);
        GainControl(Vertical);
    }

    public override void ReleaseControl(bool resetValuse = true)
    {
        m_HaveControl = true;

        ReleaseControl(Jump, resetValuse);
        ReleaseControl(Dash, resetValuse);
        ReleaseControl(Horizontal, resetValuse);
        ReleaseControl(Vertical, resetValuse);
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
