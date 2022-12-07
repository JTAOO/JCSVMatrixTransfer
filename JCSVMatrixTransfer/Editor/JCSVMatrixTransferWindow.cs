using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class JCSVMatrixTransferWindow : EditorWindow
{
    GUIStyle AlphaStyle;
    GUIStyle boxStyle;
    GUIStyle richTextStyle_Mid;
    GUIStyle richTextStyle_Left;
    GUIStyle headerStyle;
    GUIStyle smallHeaderStyle;
    GUIStyle lineStyle;
    GUIStyle foldoutWithRichText;

    [MenuItem("JTools/J CSV Matrix Transfer")]
    static void Open()
    {
        var win = EditorWindow.GetWindow<JCSVMatrixTransferWindow>(false, "J CSV Matrix Transfer", true);
        win.GenerateStyles();
        var icon = Resources.Load("Textures/jicon2") as Texture;
        win.titleContent = new GUIContent("J CSV Matrix Transfer", icon);
    }

    string worldMatrix_line0 = "hlslcc_mtx4x4unity_ObjectToWorld[0] 0.83626, 0.0098, 0.54824, 0.00 float4";
    string worldMatrix_line1 = "hlslcc_mtx4x4unity_ObjectToWorld[1] -0.05881, 0.99568, 0.07191, 0.00 float4";
    string worldMatrix_line2 = "hlslcc_mtx4x4unity_ObjectToWorld[2] -0.54517, -0.09238, 0.83322, 0.00 float4";
    string worldMatrix_line3 = "hlslcc_mtx4x4unity_ObjectToWorld[3] 0.00353, 0.87572, -0.0069, 1.00 float4";
    Transform targetObject;

    Vector2 scrollPos_Inputs = Vector2.zero;
    private void OnGUI()
    {
        GUILayout.Box(" J CSV Matrix Transfer", boxStyle, GUILayout.Height(60), GUILayout.ExpandWidth(true));
        GUILayout.Box("使用这个工具， 利用世界空间转换矩阵", richTextStyle_Mid, GUILayout.ExpandWidth(true));

        GUILayout.Space(10);
        DrawALine(3);
        GUILayout.Space(10);
        GUILayout.Label(" Inputs", headerStyle);
        targetObject = EditorGUILayout.ObjectField("Target", targetObject, typeof(Transform), true, GUILayout.Height(30)) as Transform;
        worldMatrix_line0 = EditorGUILayout.TextField("worldMatrix_line0", worldMatrix_line0);
        worldMatrix_line1 = EditorGUILayout.TextField("worldMatrix_line1", worldMatrix_line1);
        worldMatrix_line2 = EditorGUILayout.TextField("worldMatrix_line2", worldMatrix_line2);
        worldMatrix_line3 = EditorGUILayout.TextField("worldMatrix_line3", worldMatrix_line3);





        GUILayout.Space(10);
        DrawALine(3);
        GUILayout.Space(10);

        if(GUILayout.Button("转换", GUILayout.Height(60)))
        {
            Trans();
        }
    }

    void Trans()
    {
        if (targetObject == null) return;
        if (string.IsNullOrEmpty(worldMatrix_line0)) return;
        if (string.IsNullOrEmpty(worldMatrix_line1)) return;
        if (string.IsNullOrEmpty(worldMatrix_line2)) return;
        if (string.IsNullOrEmpty(worldMatrix_line3)) return;
        if (MatrixLineAvailable(worldMatrix_line0) == false) return;
        if (MatrixLineAvailable(worldMatrix_line1) == false) return;
        if (MatrixLineAvailable(worldMatrix_line2) == false) return;
        if (MatrixLineAvailable(worldMatrix_line3) == false) return;

        Vector4 line0 = AnalysisLine(worldMatrix_line0);
        Vector4 line1 = AnalysisLine(worldMatrix_line1);
        Vector4 line2 = AnalysisLine(worldMatrix_line2);
        Vector4 line3 = AnalysisLine(worldMatrix_line3);

        Matrix4x4 matrix1 = new Matrix4x4(line0, line1, line2, line3);
        //matrix1 = matrix1.transpose;

        Vector3 newForward = matrix1.MultiplyVector(Vector3.forward);
        Vector3 newUp = matrix1.MultiplyVector(Vector3.up);
        targetObject.localRotation = Quaternion.LookRotation(newForward, newUp);
        targetObject.localPosition = matrix1.MultiplyPoint(Vector3.zero);
        //Debug.Log(matrix1);
    }

    Vector4 AnalysisLine(string _line)
    {
        Vector4 out1 = Vector4.zero;

        var parts = _line.Split(" ");
        List<string> availableParts = new List<string>();
        foreach(string part1 in parts)
        {
            if(part1.Contains('.'))
            {
                availableParts.Add(part1);
            }
        }

        for(int i=0; i< availableParts.Count; i++)
        {
            string part1 = availableParts[i];
            //Debug.Log("[" + part1 + "]");
            if (part1[part1.Length - 1] == ',')
            {
                availableParts[i] = part1.Substring(0, part1.Length - 1);
            }
        }

        out1.x = float.Parse(availableParts[0]);
        out1.y = float.Parse(availableParts[1]);
        out1.z = float.Parse(availableParts[2]);
        out1.w = float.Parse(availableParts[3]);

        return out1;
    }


    bool MatrixLineAvailable(string _line)
    {
        if(_line.Split(',').Length >= 4)
        {
            return true;
        }
        return false;
    }

    #region HelpDrawFunctions
    void DrawALine(int _height)
    {
        GUILayout.Box("", lineStyle, GUILayout.ExpandWidth(true), GUILayout.Height(_height));
    }
    void ShowProcessBar(float _jindu)
    {
        if (Event.current.type == EventType.Repaint)
        {
            var lastRect = GUILayoutUtility.GetLastRect();
            EditorGUI.ProgressBar(new Rect(lastRect.x, lastRect.y + lastRect.height, lastRect.width, 20), Mathf.Clamp01(_jindu), "");
        }
        GUILayout.Space(20);
    }
    #endregion



    void GenerateStyles()
    {
        AlphaStyle = new GUIStyle();
        AlphaStyle.normal.background = Resources.Load("Textures/touming") as Texture2D;

        boxStyle = new GUIStyle();
        boxStyle.normal.background = Resources.Load("Textures/d_box") as Texture2D;
        boxStyle.normal.textColor = Color.white;
        boxStyle.border = new RectOffset(3, 3, 3, 3);
        boxStyle.margin = new RectOffset(5, 5, 5, 5);
        boxStyle.fontSize = 25;
        boxStyle.fontStyle = FontStyle.Bold;
        boxStyle.font = Resources.Load("Fonts/GAU_Root_Nomal") as Font;
        boxStyle.alignment = TextAnchor.MiddleCenter;

        richTextStyle_Mid = new GUIStyle();
        richTextStyle_Mid.richText = true;
        richTextStyle_Mid.normal.textColor = Color.white;
        richTextStyle_Mid.alignment = TextAnchor.MiddleCenter;

        richTextStyle_Left = new GUIStyle();
        richTextStyle_Left.richText = true;

        headerStyle = new GUIStyle();
        headerStyle.border = new RectOffset(3, 3, 3, 3);
        headerStyle.fontSize = 17;
        headerStyle.fontStyle = FontStyle.Bold;

        smallHeaderStyle = new GUIStyle();
        smallHeaderStyle.border = new RectOffset(3, 3, 3, 3);
        smallHeaderStyle.fontSize = 14;
        smallHeaderStyle.fontStyle = FontStyle.Bold;

        lineStyle = new GUIStyle();
        lineStyle.normal.background = boxStyle.normal.background;
        lineStyle.alignment = TextAnchor.MiddleCenter;

        foldoutWithRichText = new GUIStyle();
        foldoutWithRichText = EditorStyles.foldout;
        foldoutWithRichText.richText = true;

    }

}
