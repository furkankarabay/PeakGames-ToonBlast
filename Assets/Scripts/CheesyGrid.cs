using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class CheesyGrid : MonoBehaviour
{
    [Header("Cell values cycle from 0 to this-1")]
    public int MaxValue = 4;

    [Header("Actual saved payload. Use GetCell(x,y) to read!")]
    [Header("WARNING: changing this will nuke your data!")]
    public string data;

    [Range(3, 11)]
    public int height = 6;

    [Range(3, 9)]
    public int width = 4;

    public string GetCell(int x, int y)
    {
        int n = GetIndex(x, y);
        return data.Substring(n, 1);
    }


#if UNITY_EDITOR
    void OnValidate()
    {
        if (data == null || data.Length != (height * width))
        {
            Undo.RecordObject(this, "Resize");

            if (height < 1) height = 1;
            if (width < 1) width = 1;

            // make a default layout
            data = "";
            for (int y = 0; y < width; y++)
            {
                for (int x = 0; x < height; x++)
                {
                    string cell = "1";

                    data = data + cell;
                }
            }

            EditorUtility.SetDirty(this);
        }
    }
    void Reset()
    {
        OnValidate();
    }
#endif

    int GetIndex(int x, int y)
    {
        if (x < 0) return -1;
        if (y < 0) return -1;
        if (x >= height) return -1;
        if (y >= width) return -1;
        return x + y * height;
    }

    void ToggleCell(int x, int y)
    {
        int n = GetIndex(x, y);
        if (n >= 0)
        {
            var cell = data.Substring(n, 1);

            int c = 0;
            int.TryParse(cell, out c);
            c++;
            if (c >= MaxValue) c = 0;

            cell = c.ToString();

#if UNITY_EDITOR
            Undo.RecordObject(this, "Toggle Cell");
#endif
            // reassemble
            data = data.Substring(0, n) + cell + data.Substring(n + 1);
#if UNITY_EDITOR
            EditorUtility.SetDirty(this);
#endif
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(CheesyGrid))]
    public class CheesyGridEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            var grid = (CheesyGrid)target;

            GUILayout.Label("WARNING: Save and commit your prefab/scene OFTEN!");

            EditorGUILayout.BeginVertical();

            for (int x = grid.height - 1; x >= 0; x--)
            {
                GUILayout.BeginHorizontal();

                for (int y = 0; y < grid.width; y++)
                {
                    int n = grid.GetIndex(x, y);

                    var cell = grid.data.Substring(n, 1);

                    // hard-coded some cheesy color map - improve it by all means!
                    GUI.color = Color.gray;
                    if (cell == "0") GUI.color = Color.yellow;
                    if (cell == "1") GUI.color = Color.red;
                    if (cell == "2") GUI.color = Color.blue;
                    if (cell == "3") GUI.color = Color.green;
                    if (cell == "4") GUI.color = Color.magenta;

                    if (cell == "5") // BALON
                    {
                        cell = "B";
                        GUI.color = new Color(240f / 255f, 121f / 255f, 161f / 255f); //PINK
                    }

                    if (cell == "6") // ÖRDEK
                    {
                        cell = "D";
                        GUI.color = Color.yellow;
                    }

                    if (GUILayout.Button(cell, GUILayout.Width(20)))
                    {
                        grid.ToggleCell(x, y);
                    }
                }
                GUILayout.EndHorizontal();
            }


            EditorGUILayout.EndVertical();

            GUI.color = Color.yellow;

            GUILayout.Label("DANGER ZONE BELOW THIS AREA!");

            GUI.color = Color.white;

            DrawDefaultInspector();
        }
    }
#endif
}