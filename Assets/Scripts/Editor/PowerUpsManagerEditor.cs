using UnityEditor;
using FantasyErrand;

namespace FantasyErrand
{
    [CustomEditor(typeof(PowerUpsManager))]
    public class PowerUpsManagerEditor : Editor
    {
        public override bool RequiresConstantRepaint()
        {
            return true;
        }

        public override void OnInspectorGUI()
        {
            PowerUpsManager manager = (PowerUpsManager)target;
            EditorGUILayout.LabelField("Magnet", $"{manager.MagnetEndTime.TotalSeconds.ToString("n2")}");
            EditorGUILayout.LabelField("Boost", $"{manager.BoostEndTime.TotalSeconds.ToString("n2")}");
            EditorGUILayout.LabelField("Golden Coin", $"{manager.GoldenCoinEndTime.TotalSeconds.ToString("n2")}");
            EditorGUILayout.LabelField("Phase", $"{manager.PhaseEndTime.TotalSeconds.ToString("n2")}");
            base.OnInspectorGUI();
        }
    }
}
