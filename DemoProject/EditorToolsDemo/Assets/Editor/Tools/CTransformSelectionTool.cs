//If it running in the unity editor
#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

/// <summary>
/// Tool to modify the transform of all the objects selected by an specific amount
/// </summary>
/// <Creator>Alvaro Chavez Mixco</Creator>
/// <CreationDate>Sunday, January 29, 2017</CreationDate>
public class CTransformSelectionTool : EditorWindow
{
    //Constants
    private const string M_LABEL_WARNING_SELECT_OBJECTS = "Please select object to transform";

    private const string M_TOGGLE_ADD_LOCAL_POSITION = "Change Local Position";
    private const string M_TOGGLE_ADD_LOCAL_ROTATION = "Change Local Rotation";


    private const string M_VECTOR3_ADD_ROTATION = "Rotation Degrees to Add: ";
    private const string M_VECTOR3_ADD_POSITION = "Translation: ";
    private const string M_VECTOR3_ADD_SCALE = "Scale to Add: ";

    private const string M_BUTTON_SRT = "Affect Transform";

    //Conditions
    private bool m_changeLocalPosition = true;
    private bool m_changeLocalRotation = true;

    //Values to add
    private Vector3 m_scaleToAdd = Vector3.zero;
    private Vector3 m_rotationDegreesToAdd = Vector3.zero;
    private Vector3 m_positionToAdd = Vector3.zero;

    /// <summary>
    /// Show the tool option in the Unity toolbar
    /// </summary>
    /// <Creator>Alvaro Chavez Mixco</Creator>
    /// <CreationDate>Sunday, January 29, 2017</CreationDate>
    [MenuItem("Tools/Transform Selection")]
    private static void Init()
    {
        //Create window
        CTransformSelectionTool window = (CTransformSelectionTool)EditorWindow.GetWindow(typeof(CTransformSelectionTool));

        //Show window
        window.Show();
    }

    /// <summary>
    /// Render all the field that user can set, and handles the logic for them
    /// </summary>
    /// <Creator>Alvaro Chavez Mixco</Creator>
    /// <CreationDate>Sunday, January 29, 2017</CreationDate>
    private void OnGUI()
    {
        //Show toggle whether it should affect local or world space position
        m_changeLocalPosition = EditorGUILayout.ToggleLeft(M_TOGGLE_ADD_LOCAL_POSITION, m_changeLocalPosition);

        //The position we want to add to object
        m_positionToAdd = EditorGUILayout.Vector3Field(M_VECTOR3_ADD_POSITION, m_positionToAdd);

        //Add space to window
        EditorGUILayout.Space();

        //Show toggle whether it should affect local or world space rotation
        m_changeLocalRotation = EditorGUILayout.ToggleLeft(M_TOGGLE_ADD_LOCAL_ROTATION, m_changeLocalRotation);

        //The rotation we want to add to object
        m_rotationDegreesToAdd = EditorGUILayout.Vector3Field(M_VECTOR3_ADD_ROTATION, m_rotationDegreesToAdd);

        //Add space to window
        EditorGUILayout.Space();

        //The scale to add to object
        m_scaleToAdd = EditorGUILayout.Vector3Field(M_VECTOR3_ADD_SCALE, m_scaleToAdd);

        //Add space to window
        EditorGUILayout.Space();

        //If the button is pressed
        if (GUILayout.Button(M_BUTTON_SRT))
        {
            //Transform the objects currently selected
            ScaleRotateTranslate(Selection.gameObjects, m_positionToAdd, m_rotationDegreesToAdd, m_scaleToAdd,
                m_changeLocalPosition);
        }

        //If there are no object selected (the button will always be shown
        //because the current selection value is not updated until the window is focused,
        // (mouse hover over the editow window)
        if (Selection.gameObjects.Length <= 0)
        {
            //Indicate to the user that he has to select objects
            EditorGUILayout.LabelField(M_LABEL_WARNING_SELECT_OBJECTS);
        }
    }

    /// <summary>
    /// Scale, rotate, translate the game objects currently selected.
    /// </summary>
    /// <Creator>Alvaro Chavez Mixco</Creator>
    /// <CreationDate>Sunday, January 29, 2017</CreationDate>
    /// <param name="aObjectsToTransform">The objects that will be transformed</param>
    /// <param name="aPositionChange">The displacement that will be added to all the objects transform</param>
    /// <param name="aRotationChange">The rotation in degree that will be added to all the objects transform
    /// (local rotaiton)</param>
    /// <param name="aScaleChange">The scale that will be added to all the objects transform
    /// (local scale)</param>
    /// <param name="aChangeLocalPosition">Indicates whether the change in position will be to the objects
    /// local position or world position</param>
    private void ScaleRotateTranslate(GameObject[] aObjectsToTransform,
        Vector3 aPositionChange, Vector3 aRotationChange, Vector3 aScaleChange,
        bool aChangeLocalPosition)
    {
        //If the objects being changed are valid
        if (aObjectsToTransform != null)
        {
            //Go through all the objects
            for (int i = 0; i < aObjectsToTransform.Length; i++)
            {
                //If the object is valid
                if (aObjectsToTransform[i] != null)
                {
                    //Add to scale
                    aObjectsToTransform[i].transform.localScale += aScaleChange;

                    //Add to rotation
                    aObjectsToTransform[i].transform.localRotation *= Quaternion.Euler(aRotationChange.x,aRotationChange.y,aRotationChange.z);

                    //Add to position
                    //If we want to change local position
                    if (aChangeLocalPosition == true)
                    {
                        //Add to the local position
                        aObjectsToTransform[i].transform.localPosition += aPositionChange;
                    }
                    //If we want to change world position
                    else
                    {
                        //Add to the world position
                        aObjectsToTransform[i].transform.position += aPositionChange;
                    }
                }
            }
        }
    }
}
#endif