using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CompositeState : CompositeStateBase
{
    public List<CompositeStateBase> tokens = new List<CompositeStateBase>();

    /// <summary>
    /// True if any token is true
    /// </summary>
    public override bool IsOn
    {
        get
        {
            foreach (CompositeStateBase token in tokens)
            {
                if (token.IsOn)
                    return true;
            }
            return false;
        }
    }

    public void Add(CompositeStateBase token)
    {
        tokens.Add(token);
    }

    public void Remove(CompositeStateBase token)
    {
        tokens.Remove(token);
    }
}

public class CompositeStateToken : CompositeStateBase
{
    private bool isOn = false;

    public override bool IsOn
    {
        get
        {
            return isOn;
        }
    }

    public void SetOn(bool value)
    {
        isOn = value;
    }
}


public abstract class CompositeStateBase
{
    public abstract bool IsOn { get; }
}
