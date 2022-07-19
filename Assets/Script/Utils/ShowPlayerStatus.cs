using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Player.PlayerController))]
public class ShowPlayerStatus : Editor
{
    public override bool HasPreviewGUI()
    {
        return base.HasPreviewGUI() || (target as Player.PlayerController).enabled;
    }
    public override void OnPreviewGUI(Rect r, GUIStyle background)
    {
        base.OnPreviewGUI(r, background);
        Player.PlayerController tar = target as Player.PlayerController;
        if (tar == null)
            return;
        Vector2 size = new(r.size.x, 30);
        GUI.Label(new Rect(new Vector2(10, 30), size), $"colDown : {tar.colDow}");
        GUI.Label(new Rect(new Vector2(10, 45), size), $"colLef : {tar.colLef}");
        GUI.Label(new Rect(new Vector2(10, 60), size), $"colUp : {tar.colUp}");
        GUI.Label(new Rect(new Vector2(10, 75), size), $"colRig : {tar.colRig}");
    }
}
