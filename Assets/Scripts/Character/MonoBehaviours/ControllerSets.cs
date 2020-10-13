using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerSets : MonoBehaviour
{
    [Serializable]
    public class KeySet
    {
        public InputComponent.InputButton debugMenuOpen= new InputComponent.InputButton(KeyCode.F12);
        public InputComponent.InputButton restage = new InputComponent.InputButton(KeyCode.F5);
        public InputComponent.InputButton manualOpen = new InputComponent.InputButton(KeyCode.Tab);
        public InputComponent.InputButton pause = new InputComponent.InputButton(KeyCode.Escape);
        public InputComponent.InputButton jump = new InputComponent.InputButton(KeyCode.Z);
        public InputComponent.InputButton dash = new InputComponent.InputButton(KeyCode.X);
        public InputComponent.InputButton interact = new InputComponent.InputButton(KeyCode.V);
        public InputComponent.InputAxis horizontal = new InputComponent.InputAxis(KeyCode.RightArrow, KeyCode.LeftArrow);
        public InputComponent.InputAxis vertical = new InputComponent.InputAxis(KeyCode.UpArrow,KeyCode.DownArrow);
    }

    public KeySet[] IresKeySettings;
    public KeySet[] SeriKeySettings;

    
    public KeySetTypes iresKeySet;

    public KeySetTypes seriKeySet;

    public enum KeySetTypes
    {
        KeyBoard=0,
        
        Controller,
        TenKeyLess

    }
    
    public void SelectKeySetting(int key)
    {
        foreach(Observer obs in Publisher.Instance.Observers)
        {
            KeySet inputkeyset;
            if(obs.GetInput()[0].Dash.Enabled)
            {
                inputkeyset = SeriKeySettings[key];
            }
            else
            {
                inputkeyset = IresKeySettings[key];
            }

            obs.GetInput()[0].DebugMenuOpen = inputkeyset.debugMenuOpen;
            obs.GetInput()[0].Restage = inputkeyset.restage;
            obs.GetInput()[0].ManualOpen = inputkeyset.manualOpen;
            obs.GetInput()[0].Pause = inputkeyset.pause;
            obs.GetInput()[0].Jump = inputkeyset.jump;
            obs.GetInput()[0].Dash = inputkeyset.dash;
            obs.GetInput()[0].Interact = inputkeyset.interact;
            obs.GetInput()[0].Horizontal = inputkeyset.horizontal;
            obs.GetInput()[0].Vertical = inputkeyset.vertical;

            obs.GetInput()[0].GainControl();
        }
    }

    public void InitializeKeySet()
    {
        foreach (Observer obs in Publisher.Instance.Observers)
        {
            KeySet inputkeyset;
            if (obs.GetInput()[0].Dash.Enabled)
            {
                inputkeyset = SeriKeySettings[(int)seriKeySet];
                if (seriKeySet != KeySetTypes.Controller && iresKeySet == KeySetTypes.Controller)
                    inputkeyset = SeriKeySettings[4];
                else if (seriKeySet == KeySetTypes.Controller && iresKeySet == KeySetTypes.Controller)
                    inputkeyset = SeriKeySettings[3];

                if(seriKeySet !=KeySetTypes.Controller && iresKeySet !=KeySetTypes.Controller)
                {
                    inputkeyset.restage.Disable();
                    inputkeyset.manualOpen.Disable();
                    inputkeyset.pause.Disable();
                }
            }
            else
            {
                inputkeyset = IresKeySettings[(int)iresKeySet];
                if (iresKeySet != KeySetTypes.Controller && seriKeySet == KeySetTypes.Controller)
                    inputkeyset = IresKeySettings[2];
            }

            obs.GetInput()[0].DebugMenuOpen = inputkeyset.debugMenuOpen;
            obs.GetInput()[0].Restage = inputkeyset.restage;
            obs.GetInput()[0].ManualOpen = inputkeyset.manualOpen;
            obs.GetInput()[0].Pause = inputkeyset.pause;
            obs.GetInput()[0].Jump = inputkeyset.jump;
            obs.GetInput()[0].Dash = inputkeyset.dash;
            obs.GetInput()[0].Interact = inputkeyset.interact;
            obs.GetInput()[0].Horizontal = inputkeyset.horizontal;
            obs.GetInput()[0].Vertical = inputkeyset.vertical;

           // obs.GetInput()[0].GainControl();
        }
    }

    public void SetIres(int types)
    {
        iresKeySet = (KeySetTypes)types;
    }

    public void SetSeri(int types)
    {
        seriKeySet = (KeySetTypes)types;
    }
    //public void 
}
