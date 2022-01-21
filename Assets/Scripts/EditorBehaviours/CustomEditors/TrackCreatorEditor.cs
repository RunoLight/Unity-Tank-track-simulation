using System.Collections;
using System.Collections.Generic;
using EditorBehaviours;
using UnityEditor;

using UnityEngine;

namespace TestGame
{
    [CustomEditor(typeof(TrackCreator))]
    public class TrackCreatorEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            bool createTrack = GUILayout.Button("Create track");
            if (createTrack)
            {
                (target as TrackCreator).SpawnTrackChain();
            }
        }
    }
}
