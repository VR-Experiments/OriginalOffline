using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.Extras;
using TMPro;
using Assets.LSL4Unity.Scripts;

public class PointerControllerBlue : SteamVR_LaserPointer
{
    int score_blue = 0;
    public TextMeshPro counterTextBlue;

    private LSLMarkerStream Destroy_marker;


    public override void OnPointerClick(PointerEventArgs e)
    {
        Destroy_marker = FindObjectOfType<LSLMarkerStream>();
        base.OnPointerClick(e);

        if (e.target.gameObject.tag == "blue")
        {
            Destroy(e.target.gameObject);
            score_blue += 1;
            counterTextBlue.SetText(score_blue.ToString());
            Destroy_marker.Write("blue destroyed");
        }

    }
    public void ResetScore()
    {
        score_blue = 0;
        counterTextBlue.SetText(score_blue.ToString());
    }

}

