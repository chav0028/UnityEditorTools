using UnityEngine;
using System.Collections;

using UnityEditor;

//If it running in the unity editor
#if UNITY_EDITOR
/*
Description: Tool to modify the transform of all the objects selected by an specific amount
Creator: Alvaro Chavez Mixco
Creation Date: Sunday, January 29, 2017
*/
public class CTransformSelectionTool : EditorWindow
{
    //Constants
    private const string M_TOGGLE_ADD_LOCAL_POSITION = "Change Local Position: ";

    private const string M_VECTOR3_ADD_ROTATION = "Rotation Degrees to Add: ";
    private const string M_VECTOR3_ADD_POSITION = "Translation: ";
    private const string M_VECTOR3_ADD_SCALE = "Scale to Add: ";

    private const string M_BUTTON_SRT = "Affect Transform";

    //Conditions
    private bool m_changeLocalPosition = true;

    //Values to add
    private Vector3 m_scaleToAdd = Vector3.zero;
    private Vector3 m_rotationDegreesToAdd = Vector3.zero;
    private Vector3 m_positionToAdd = Vector3.zero;

    /*
    Description: Show the tab on the editor
    Creator: Alvaro Chavez Mixco
    Creation Date: Sunday, January 29, 2017
    */
    [MenuItem("Custom Tools/Transform Selection")]
    private static void Init()
    {
        CTransformSelectionTool window = (CTransformSelectionTool)EditorWindow.GetWindow(typeof(CTransformSelectionTool));//Create window
        window.Show();//Show window
    }

    /*
    Description: Show all the field that user can fill to place the prefabs
    Creator: Alvaro Chavez Mixco
    Creation Date: Sunday, January 29, 2017
    */
    private void OnGUI()
    {
        //Show toggle whether it should affect local or world space position
        m_changeLocalPosition = EditorGUILayout.Toggle(M_TOGGLE_ADD_LOCAL_POSITION, m_changeLocalPosition);

        //Add space to window
        EditorGUILayout.Space();

        //The position we want to add to object
        m_positionToAdd = EditorGUILayout.Vector3Field(M_VECTOR3_ADD_POSITION, m_positionToAdd);

        //Add space to window
        EditorGUILayout.Space();

        //The rotation we want to add to object
        m_rotationDegreesToAdd = EditorGUILayout.Vector3Field(M_VECTOR3_ADD_ROTATION, m_rotationDegreesToAdd);

        //Add space to window
        EditorGUILayout.Space();

        //The scale to add to object
        m_scaleToAdd = EditorGUILayout.Vector3Field(M_VECTOR3_ADD_SCALE, m_scaleToAdd);

        //If the button is pressed
        if (GUILayout.Button(M_BUTTON_SRT))
        {
            //Add to the values of the object
            ScaleRotateTranslate();
        }
    }

    /*
    Description: Scale, rotate, translate the game objects currently selected.
    Creator: Alvaro Chavez Mixco
    Creation Date: Sunday, January 29, 2017
    */
    private void ScaleRotateTranslate()
    {
        //If slected game objects are valid
        if (Selection.gameObjects != null)
        {
            //Go through all selected objects
            for (int i = 0; i < Selection.gameObjects.Length; i++)
            {
                //If the selection object is valid
                if (Selection.gameObjects[i] != null)
                {
                    //Add to scale
                    Selection.gameObjects[i].transform.localScale = Selection.gameObjects[i].transform.localScale + m_scaleToAdd;

                    //Add to rotation
                    Selection.gameObjects[i].transform.localEulerAngles = Selection.gameObjects[i].transform.localEulerAngles + m_rotationDegreesToAdd;

                    //Add to position
                    //If we want to change local position
                    if (m_changeLocalPosition == true)
                    {
                        Selection.gameObjects[i].transform.localPosition = Selection.gameObjects[i].transform.localPosition + m_positionToAdd;
                    }
                    else//If we want to change world position
                    {
                        Selection.gameObjects[i].transform.position = Selection.gameObjects[i].transform.position + m_positionToAdd;
                    }
                }
            }
        }
    }
}
#endif