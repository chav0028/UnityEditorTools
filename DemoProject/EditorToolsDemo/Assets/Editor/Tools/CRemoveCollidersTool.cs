using UnityEngine;

using UnityEditor;
using UnityEditorInternal;

/// <summary>
/// Tool to remove all the type of colliders (capsule, sphere, etc.) from gameobjects
/// </summary>
/// <Creator>Alvaro Chavez Mixco</Creator>
/// <CreationDate>Friday, October 11th, 2019</CreationDate>
public class CRemoveCollidersTool : EditorWindow
{
    private const string M_LABEL_WARNING_SELECT_OBJECTS = "Please select object to remove colliders from";

    private const string M_TOGGLE_REMOVE_RECURSIVELY = "Remove recursively";
    private const string M_TOGGLE_REMOVE_COLLIDERS = "Remove colliders";
    private const string M_TOGGLE_REMOVE_TRIGGERS = "Remove triggers";
    private const string M_TOGGLE_REMOVE_IN_INACTIVE_OBJECT = "Remove in inactive objects";

    private const string M_BUTTON_REMOVE = "Remove colliders";

    private bool m_removeRecursively = true;
    private bool m_removeColliders = true;
    private bool m_removeTriggers = true;
    private bool m_removeInInactiveObjects = false;

    /// <summary>
    /// Show editor tool label in Unity toolbar
    /// </summary>
    /// <Creator>Alvaro Chavez Mixco</Creator>
    /// <CreationDate>Friday, October 11th, 2019</CreationDate>
    [MenuItem("Custom Tools/Remove Colliders")]
    private static void Init()
    {
        //Create editor window
        CRemoveCollidersTool window = (CRemoveCollidersTool)EditorWindow.GetWindow(typeof(CRemoveCollidersTool));

        //Show editor window
        window.Show();
    }

    /// <summary>
    /// Show UI for the tool and handles all UI input, including button presses
    /// for removing the colliders
    /// </summary>
    /// <Creator>Alvaro Chavez Mixco</Creator>
    /// <CreationDate>Friday, October 11th, 2019</CreationDate>
    private void OnGUI()
    {
        //Display toggle for removing colliders recursively, and save its value
        m_removeRecursively = EditorGUILayout.ToggleLeft(M_TOGGLE_REMOVE_RECURSIVELY, m_removeRecursively);

        //Display toggle for removing colliders, and save its value
        m_removeColliders = EditorGUILayout.ToggleLeft(M_TOGGLE_REMOVE_COLLIDERS, m_removeColliders);

        //Display toggle for removing triggers, and save its value
        m_removeTriggers = EditorGUILayout.ToggleLeft(M_TOGGLE_REMOVE_TRIGGERS, m_removeTriggers);

        //If the tool will remove colliders reclusively
        if (m_removeRecursively == true)
        {
            //Add space to editor window
            EditorGUILayout.Space();

            //Show toggle whether inactive objects should be removed
            m_removeInInactiveObjects =
                EditorGUILayout.ToggleLeft(M_TOGGLE_REMOVE_IN_INACTIVE_OBJECT, m_removeInInactiveObjects);
        }

        //Add space to editor window
        EditorGUILayout.Space();

        //If the button for removing collide is pressed
        if (GUILayout.Button(M_BUTTON_REMOVE) == true)
        {
            //Get the objects currently selected
            GameObject[] selectedObjects = Selection.gameObjects;

            //Go through each selected game object
            for (int i = 0; i < selectedObjects.Length; i++)
            {
                Collider[] collidersToRemove;

                //If the colliders will be removed recurvisely
                if (m_removeRecursively == true)
                {
                    //Get the colliders in this object and his children, 
                    //it may also check for colliders in inactive objects
                    collidersToRemove = 
                        selectedObjects[i].GetComponentsInChildren<Collider>(m_removeInInactiveObjects);
                }
                //If the colliders won't be removed recursively
                else
                {
                    //Just remove the colliders in the selected object
                    collidersToRemove = selectedObjects[i].GetComponents<Collider>();
                }

                //Remove all colliders in the selected object
                RemoveColliders(collidersToRemove, m_removeColliders, m_removeTriggers);
            }
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
    /// Remove all the colliders passed from their gameobjects, checking
    /// if colliders and/or triggers should be removed
    /// </summary>
    /// <Creator>Alvaro Chavez Mixco</Creator>
    /// <CreationDate>Friday, October 11th, 2019</CreationDate>
    /// <param name="aColliders">The colliders that will be removed from the game object</param>
    /// <param name="aRemoveCollider">Indicates if colliders (non-triggers) should be removed</param>
    /// <param name="aRemoveTrigger">Inidcates if triggers (colliders set as trigger) should be removed</param>
    /// 
    private void RemoveColliders(Collider[] aColliders, bool aRemoveCollider, bool aRemoveTrigger)
    {
        //If the colliders are valid
        if (aColliders != null)
        {
            //Go through all the colliders
            for (int i = 0; i < aColliders.Length; i++)
            {
                //If the collider is a collider (non trigger) and colliders should be removed or
                //if the collider is a trigger and trigger should be removed
                if (aColliders[i].isTrigger == false && aRemoveCollider == true ||
                    aColliders[i].isTrigger == true && aRemoveTrigger == true)
                {
                    // Disable the collider first, in case there is an issue deleting the collider
                    // such as the collider being a required component
                    aColliders[i].enabled = false;

                    //Destroy the collider
                    DestroyImmediate(aColliders[i]);
                }
            }
        }
    }
}
