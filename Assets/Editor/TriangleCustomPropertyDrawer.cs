using UnityEngine;
using UnityEditor;
using System.Collections;


[CustomPropertyDrawer(typeof(TriangleLayout))]
public class TriangleCustomPropertyDrawer : PropertyDrawer {

	int n;

	bool inited = false;

	public static string[] labels;

	static TriangleCustomPropertyDrawer()
	{
		var enumVals = System.Enum.GetValues(typeof(ETeamEnum));
		
		labels = new string[enumVals.Length];
		for(int i = 0; i < enumVals.Length; i++)
		{
			labels[i] = enumVals.GetValue(i).ToString();
		}
	}




	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
	{


		EditorGUI.PrefixLabel(position, label);



		SerializedProperty tri = property.FindPropertyRelative("tri");
		SerializedProperty arr = tri.FindPropertyRelative("e");
		SerializedProperty ns = tri.FindPropertyRelative("n");
		//SerializedProperty lab = tri.FindPropertyRelative("labels");
		n = ns.intValue;

		Vector2 checkBoxTopLeft = new Vector2(position.x + 100.0f, position.y + 50.0f);

		Vector2 columnTopLeft = new Vector2(position.x + 100.0f, position.y + 30.0f);

		Vector2 rowTopLeft = new Vector2(position.x + 70.0f, position.y + 50.0f);

		Rect checkBoxPosition = new Rect(checkBoxTopLeft, new Vector2(10.0f, 10.0f));


		Rect colPos = new Rect(columnTopLeft, new Vector2(60.0f, 20.0f));
		Rect rowPos = new Rect(rowTopLeft, new Vector2(20.0f, 60.0f));

		var mx = GUI.matrix;

		//rotating is not really working..
		//GUIUtility.RotateAroundPivot(90.0f, columnTopLeft);

		var labelStyle = GUI.skin.GetStyle("Label");
		var defAlignment = labelStyle.alignment;
		labelStyle.alignment = TextAnchor.UpperLeft;


		//upper labels
		for(int i = 0; i < n; i++)
		{
			string text = labels[n - i - 1];

			GUI.Label(colPos, new GUIContent(text), labelStyle);
			colPos.x += 15.0f;
		}

		GUI.matrix = mx;
		

		//left labels
		labelStyle.alignment = TextAnchor.UpperRight;
		for(int i = 0; i < n; i++)
		{
			string text = labels[i];
			
			GUI.Label(rowPos, new GUIContent(text), labelStyle);
			rowPos.y += 15.0f;
		}
		labelStyle.alignment = defAlignment;

		//checkboxes
		int k = 0;
		for(int i = 0; i < n; i++)
		{
			for(int j = 0; j < n - i; j++)
			{
				EditorGUI.PropertyField(checkBoxPosition, arr.GetArrayElementAtIndex(k), GUIContent.none);
				checkBoxPosition.x += 15.0f;
				k++;
			}
			checkBoxPosition.y += 15.0f;
			checkBoxPosition.x = checkBoxTopLeft.x;
		}


	

	}

	public override float GetPropertyHeight (SerializedProperty property, GUIContent label)
	{
		return 120.0f + 15.0f * n;
	}
}
