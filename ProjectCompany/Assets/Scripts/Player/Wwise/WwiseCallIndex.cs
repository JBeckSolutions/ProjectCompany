using System.Collections.Generic;
using UnityEngine;

public class WwiseCallIndex : MonoBehaviour
{
    public WwiseEventPost StepPost;
    public WwiseEventPost LiftPost;
    public void PostWwiseEvent(string key)
    {
        switch (key)
        {
            case "Lift":
                StepPost.PostWwiseEvent();
                break;
            case "Step":
                LiftPost.PostWwiseEvent();
                break;
        }
    }
}
