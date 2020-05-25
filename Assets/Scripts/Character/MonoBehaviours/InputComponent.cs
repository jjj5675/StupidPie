using System;
using System.Collections;
using UnityEngine;

public abstract class InputComponent : MonoBehaviour
{
    [Serializable]
    public class InputButton
    {
        public KeyCode key;
        public bool Down { get; protected set; }
        public bool Held { get; protected set; }
        public bool Up { get; protected set; }

        public bool Enabled
        {
            get { return m_Enabled; }
        }

        [SerializeField]
        protected bool m_Enabled = true;
        protected bool m_GettingInput = true;

        bool m_AfterFixedUpdateDown;
        bool m_AfterFixedUpdateHeld;
        bool m_AfterFixedUpdateUp;

        public InputButton(KeyCode key)
        {
            this.key = key;
        }

        public void Get(bool fixedUpdateHappened)
        {
            if(!m_Enabled)
            {
                Down = false;
                Held = false;
                Up = false;
                return;
            }

            if (!m_GettingInput)
            {
                return;
            }

            if (fixedUpdateHappened)
            {
                Down = Input.GetKeyDown(key);
                Held = Input.GetKey(key);
                Up = Input.GetKeyUp(key);

                m_AfterFixedUpdateDown = Down;
                m_AfterFixedUpdateHeld = Held;
                m_AfterFixedUpdateUp = Up;
            }
            else
            {
                Down = Input.GetKeyDown(key) || m_AfterFixedUpdateDown;
                Held = Input.GetKey(key) || m_AfterFixedUpdateHeld;
                Up = Input.GetKeyUp(key) || m_AfterFixedUpdateUp;

                m_AfterFixedUpdateDown |= Down;
                m_AfterFixedUpdateHeld |= Held;
                m_AfterFixedUpdateUp |= Up;
            }
        }

        public void Enable()
        {
            m_Enabled = true;
        }

        public void Disable()
        {
            m_Enabled = false;
        }

        public void GainControl()
        {
            m_GettingInput = true;
        }

        public IEnumerator ReleaseControl(bool resetValue)
        {
            m_GettingInput = false;

            if (!resetValue)
            {
                yield break;
            }

            if (Down)
            {
                Up = true;
            }
            Down = false;
            Held = false;

            m_AfterFixedUpdateDown = false;
            m_AfterFixedUpdateHeld = false;
            m_AfterFixedUpdateUp = false;

            yield return null;

            Up = false;
        }
    }

    [Serializable]
    public class InputAxis
    {
        public KeyCode positive;
        public KeyCode negative;
        public float Value { get; protected set; }
        public bool ReceivingInput { get; protected set; }

        public bool Enabled
        {
            get { return m_Enabled; }
        }

        public bool GettingInput { get { return m_GettingInput; } }

        protected bool m_Enabled = true;
        protected bool m_GettingInput = true;

        public InputAxis(KeyCode positive, KeyCode negative)
        {
            this.positive = positive;
            this.negative = negative;
        }

        public void Get()
        {
            if(!m_Enabled)
            {
                Value = 0f;
                return;
            }

            if (!m_GettingInput)
            {
                return;
            }

            bool positiveHeld = false;
            bool negativeHeld = false;

            positiveHeld = Input.GetKey(positive);
            negativeHeld = Input.GetKey(negative);

            if (positiveHeld == negativeHeld)
            {
                Value = 0;
            }
            else if (positiveHeld)
            {
                Value = 1f;
            }
            else
            {
                Value = -1f;
            }

            ReceivingInput = positiveHeld || negativeHeld;
        }

        public void Enable()
        {
            m_Enabled = true;
        }

        public void Disable()
        {
            m_Enabled = false;
        }

        public void GainControl()
        {
            m_GettingInput = true;
        }

        public void ReleaseControl(bool resetValue)
        {
            m_GettingInput = false;

            if (resetValue)
            {
                Value = 0f;
                ReceivingInput = false;
            }
        }
    }

    bool m_FixedUpdateHappened;

    // Update is called once per frame
    void Update()
    {
        GetInputs(m_FixedUpdateHappened);

        m_FixedUpdateHappened = false;
    }

    void FixedUpdate()
    {
        m_FixedUpdateHappened = true;
    }

    protected abstract void GetInputs(bool fixedUpdateHappened);

    public abstract void GainControl();

    public abstract void ReleaseControl(bool resetValuse = true);

    protected void GainControl(InputButton inputButton)
    {
        inputButton.GainControl();
    }

    protected void GainControl(InputAxis inputAxis)
    {
        inputAxis.GainControl();
    }

    protected void ReleaseControl(InputButton inputButton, bool resetValue)
    {
        StartCoroutine(inputButton.ReleaseControl(resetValue));
    }

    protected void ReleaseControl(InputAxis inputAxis, bool resetValue)
    {
        inputAxis.ReleaseControl(resetValue);
    }
}


