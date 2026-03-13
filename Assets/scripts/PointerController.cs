using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.Extras;
using TMPro;
using Assets.LSL4Unity.Scripts;

public class PointerController : SteamVR_LaserPointer
{
    public int score_yellow = 0;
    public TextMeshPro counterTextYellow;

    private LSLMarkerStream Destroy_marker;

  

    public override void OnPointerClick(PointerEventArgs e)
    {
        Destroy_marker = FindObjectOfType<LSLMarkerStream>();
        base.OnPointerClick(e);

        if (e.target.gameObject.tag == "yellow")
        {
            Destroy(e.target.gameObject);
            score_yellow += 1;
            counterTextYellow.SetText(score_yellow.ToString());
            Destroy_marker.Write("yellow destroyed");
        }

    }
    public void ResetScore()
    {
        score_yellow = 0;
        counterTextYellow.SetText(score_yellow.ToString());
    }

}
