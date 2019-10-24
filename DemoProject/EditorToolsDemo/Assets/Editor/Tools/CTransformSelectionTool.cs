//If it running in the unity editor
#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

/// <summary>
/// Tool to modify the transform of all the objects selected by an specific amount
/// </summary>
/// <Creator>Alvaro Chavez Mixco</Creator>
/// <CreationDate>Sunday, January 29th, 2017</CreationDate>
public class CTransformSelectionTool : EditorWindow
{
    #region Constants
    //Constants
    private const string M_LABEL_WARNING_SELECT_OBJECTS = "Please select object to transform";

    private const string M_VECTOR3_ADD_POSITION = "Position Change";
    private const string M_VECTOR3_ADD_ROTATION = "Rotation Change";
    private const string M_VECTOR3_ADD_SCALE = "Scale Change ";

    private const string M_BUTTON_SRT = "Affect Transform";
    #endregion

    //Values to add
    private Vector3 m_scaleToAdd = Vector3.zero;
    private Vector3 m_rotationDegreesToAdd = Vector3.zero;
    private Vector3 m_positionToAdd = Vector3.zero;

    #region UI
    /// <summary>
    /// Show the tool option in the Unity toolbar
    /// </summary>
    /// <Creator>Alvaro Chavez Mixco</Creator>
    /// <CreationDate>Sunday, January 29th, 2017</CreationDate>
    [MenuItem("Tools/Transform Selection")]
    private static void Init()
    {
        //Create a new editor window, or get an existing one
        CTransformSelectionTool window = (CTransformSelectionTool)EditorWindow.GetWindow(typeof(CTransformSelectionTool));

        //Show window
        window.Show();
    }

    /// <summary>
    /// Render all the field that user can set, and handles the logic for them
    /// </summary>
    /// <Creator>Alvaro Chavez Mixco</Creator>
    /// <CreationDate>Sunday, January 29th, 2017</CreationDate>
    private void OnGUI()
    {
        //The position we want to add to object
        m_positionToAdd = EditorGUILayout.Vector3Field(M_VECTOR3_ADD_POSITION, m_positionToAdd);

        //The rotation we want to add to object
        m_rotationDegreesToAdd = EditorGUILayout.Vector3Field(M_VECTOR3_ADD_ROTATION, m_rotationDegreesToAdd);

        //The scale to add to object
        m_scaleToAdd = EditorGUILayout.Vector3Field(M_VECTOR3_ADD_SCALE, m_scaleToAdd);

        //Add space to window
        EditorGUILayout.Space();

        //If the button is pressed
        if (GUILayout.Button(M_BUTTON_SRT))
        {
            //Transform the objects currently selected
            ScaleRotateTranslate(Selection.gameObjects, m_positionToAdd, m_rotationDegreesToAdd, m_scaleToAdd);
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
    #endregion

    /// <summary>
    /// Scale, rotate, translate the game objects currently selected.
    /// </summary>
    /// <Creator>Alvaro Chavez Mixco</Creator>
    /// <CreationDate>Sunday, January 29th, 2017</CreationDate>
    /// <param name="aObjectsToTransform" type="GameObject[]">The objects that will be transformed</param>
    /// <param name="aPositionChange" type="Vector3">The displacement that will be added to all the objects transform</param>
    /// <param name="aRotationChange" type="Vector3">The rotation in degree that will be added to all the objects transform
    /// (local rotaiton)</param>
    /// <param name="aScaleChange" type="Vector3">The scale that will be added to all the objects transform
    /// (local scale)</param>
    private void ScaleRotateTranslate(GameObject[] aObjectsToTransform,
        Vector3 aPositionChange, Vector3 aRotationChange, Vector3 aScaleChange)
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
                    aObjectsToTransform[i].transform.localRotation *= Quaternion.Euler(aRotationChange.x, aRotationChange.y, aRotationChange.z);
                    
                    //Add to position
                    aObjectsToTransform[i].transform.localPosition += aPositionChange;
                }
            }
        }
    }
}
#endif