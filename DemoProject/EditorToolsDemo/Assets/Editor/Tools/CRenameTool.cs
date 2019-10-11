using UnityEngine;
using System.Collections;

using UnityEditor;
using System.Collections.Generic;
using System;

//If it running in the unity editor
#if UNITY_EDITOR
/*
Description: Tool to rename all the children gameobjects of a single parent game object.
Creator: Alvaro Chavez Mixco
Creation Date: Sunday, January 29, 2017
*/
public class CRenameTool : EditorWindow
{
    private const string M_LABEL_SELECT_OBJECTS = "Select objects to rename";

    private const string M_TOGGLE_RENAME_SELECTION = "Rename Selection: ";
    private const string M_TOGGLE_CHANGE_BASE_NAME = "Change Base Name: ";
    private const string M_TOGGLE_USE_PREFIX = "Use Prefix: ";
    private const string M_TOGGLE_USE_SUFFIX = "Use Suffix: ";
    private const string M_TOGGLE_NUMBER_OBJECTS = "Number Objects: ";
    private const string M_TOGGLE_SHOULD_REORDER_OBJECTS_ALPHANUMERICALLY = "Reorder Objects Alphanumerically";
    private const string M_TOGGLE_REORDER_SORT_METHOD_ALPHANUMERICALLY = "Sort Alphanumerically: ";

    private const string M_INT_BASE_NUMBER = "Base Number: ";
    private const string M_INT_STEP = "Step: ";

    private const string M_TEXT_BASE_NAME = "Base Name: ";
    private const string M_TEXT_PREFIX = "Prefix: ";
    private const string M_TEXT_SUFFIX = "Suffix: ";

    private const string M_OBJECT_PARENT = "Parent Object: ";

    private const string M_BUTTON_RENAME = "Rename Objects";

    //The final combination of prefix + base + suffix
    private string m_finalName = string.Empty;

    //Strings that make the name
    private string m_baseName = string.Empty;
    private string m_prefix = string.Empty;
    private string m_suffix = string.Empty;

    //Numbering variables
    private int m_startingNumber = 0;
    private int m_stepAmount = 1;

    //Conditions
    private bool m_renameSelection = true;
    private bool m_changeBaseName = true;
    private bool m_usePrefix = false;
    private bool m_useSuffix = false;
    private bool m_numberObjects = true;
    private bool m_shouldReorderObjectsAlphanumerically = true;
    private bool m_sortAlphanumerically = true;//If this is false the orders will be ordered inverse Alphanumerically 

    //Game object to rename
    private GameObject m_parentObjectToRename;

    private delegate int delegSorting(GameObject objectA, GameObject objectB);
    private delegSorting m_delegSorting;

    /*
    Description: Show the tab on the editor
    Creator: Alvaro Chavez Mixco
    Creation Date: Sunday, January 29, 2017
    */
    [MenuItem("Custom Tools/Rename")]
    private static void Init()
    {
        CRenameTool window = (CRenameTool)EditorWindow.GetWindow(typeof(CRenameTool));//Create window
        window.Show();//Show window
    }

    /*
    Description: Show all the field that user can fill to place the prefabs
    Creator: Alvaro Chavez Mixco
    Creation Date: Sunday, January 29, 2017
    */
    private void OnGUI()
    {
        //Show toggle whether it should rename the object selection
        m_renameSelection = EditorGUILayout.Toggle(M_TOGGLE_RENAME_SELECTION, m_renameSelection);

        //If we want to rename the objects selected
        if (m_renameSelection == true)
        {
            //Get the current game objects slected from editor
            GameObject[] objectsToRename = Selection.gameObjects;

            //Add space to window
            EditorGUILayout.Space();

            //Display the field to modify the text to set as name
            DisplayRenamingFields();

            //Display the field whether should objects be ordered alphanumerically 
            DisplayOrderingAlphanumerically();

            //If the objects we want to rename are valid
            if (objectsToRename != null)
            {
                    //If the button is pressed
                    if (GUILayout.Button(M_BUTTON_RENAME))
                    {
                        //Rename the selected objects
                        RenameObjects(objectsToRename);

                        //Reorder the selected objects
                        ReorderObjects(objectsToRename);
                    }

                //If there are  no objects to rename
                //Always show the button because the current selection objects don't seem to be updated
                //until you hover over this tool window
                if (objectsToRename.Length <= 0)
                {
                    //Display message
                    EditorGUILayout.LabelField(M_LABEL_SELECT_OBJECTS);
                }
            }
        }
        else//If it will rename the children of an object
        {
            //Add space to window
            EditorGUILayout.Space();

            //Object field of the parent object containing everything
            m_parentObjectToRename = (GameObject)EditorGUILayout.ObjectField(M_OBJECT_PARENT, m_parentObjectToRename, typeof(GameObject), true);

            //Add space to window
            EditorGUILayout.Space();

            //Display the field to modify the text to set as name
            DisplayRenamingFields();

            //Display the field whether should objects be ordered alphanumerically
            DisplayOrderingAlphanumerically();

            //If there is a prefab
            if (m_parentObjectToRename != null)
            {
                //If the button is pressed
                if (GUILayout.Button(M_BUTTON_RENAME))
                {
                    //Rename children object
                    RenameChildrenObjects();

                    //Reorderchildren objects
                    ReorderObjects(CUtilEditorTools.GetChildrenGameObjectFromParent(m_parentObjectToRename));
                }
            }
        }
    }

    /*
    Description: Display the editor fields corresponding to changing the name of objects
    Creator: Alvaro Chavez Mixco
    Creation Date: Sunday, January 29, 2017
    */
    private void DisplayRenamingFields()
    {
        //Toggle should it change the base name
        m_changeBaseName = EditorGUILayout.Toggle(M_TOGGLE_CHANGE_BASE_NAME, m_changeBaseName);

        //If we want to change the base name
        if (m_changeBaseName == true)
        {
            //Text field to write the base name
            m_baseName = EditorGUILayout.TextField(M_TEXT_BASE_NAME, m_baseName);
        }

        //Add space to window
        EditorGUILayout.Space();

        //Toggle should it use the prefix
        m_usePrefix = EditorGUILayout.Toggle(M_TOGGLE_USE_PREFIX, m_usePrefix);

        //If it would allow prefixes
        if (m_usePrefix == true)
        {
            //Text field for prefix
            m_prefix = EditorGUILayout.TextField(M_TEXT_PREFIX, m_prefix);
        }

        //Add space to window
        EditorGUILayout.Space();

        //Toggle should it use the suffix
        m_useSuffix = EditorGUILayout.Toggle(M_TOGGLE_USE_SUFFIX, m_useSuffix);

        //If it would allow suffixes
        if (m_useSuffix == true)
        {
            //Text field for suffix
            m_suffix = EditorGUILayout.TextField(M_TEXT_SUFFIX, m_suffix);
        }

        //Add space to window
        EditorGUILayout.Space();

        //Toggle for it should number objects
        m_numberObjects = EditorGUILayout.Toggle(M_TOGGLE_NUMBER_OBJECTS, m_numberObjects);

        //If it would allow to number objects
        if (m_numberObjects == true)
        {
            //Int field for the base number to show
            m_startingNumber = EditorGUILayout.IntField(M_INT_BASE_NUMBER, m_startingNumber);

            //Int field for the step amount
            m_stepAmount = EditorGUILayout.IntField(M_INT_STEP, m_stepAmount);
        }

        //Add space
        EditorGUILayout.Space();
    }

    /*
    Description: Display the editor fields corresponding to ordering the objects alphanumerically
    Creator: Alvaro Chavez Mixco
    Creation Date: Sunday, January 29, 2017
    */
    private void DisplayOrderingAlphanumerically()
    {
        //Add space
        EditorGUILayout.Space();

        //Toggle should it reorder objects Alphanumerically 
        m_shouldReorderObjectsAlphanumerically = EditorGUILayout.Toggle(M_TOGGLE_SHOULD_REORDER_OBJECTS_ALPHANUMERICALLY,
            m_shouldReorderObjectsAlphanumerically);

        //If we are reordering objects alphanumerically
        if (m_shouldReorderObjectsAlphanumerically == true)
        {
            //Toggle should it reorder objects Alphanumerically normally or the inverse of it
            m_sortAlphanumerically = EditorGUILayout.Toggle(M_TOGGLE_REORDER_SORT_METHOD_ALPHANUMERICALLY, m_sortAlphanumerically);

            //If it will reorders objects alphanumerically
            if (m_sortAlphanumerically == true)
            {
                //Assign delegate
                m_delegSorting = CUtilEditorTools.SortByAlphanumerically;
            }
            else//If it will reorder objects inverse alphanumerically
            {
                //Assign delegate
                m_delegSorting = CUtilEditorTools.SortByInverseAlphanumerically;
            }
        }

        //Add space
        EditorGUILayout.Space();
    }

    /*
    Description: Calculate the final name that the object will have, according to the base name, and 
                 the prefixes and suffixes. This doesn't include the number.
    Parameters: string aBaseName: The base name of the object.
    Creator: Alvaro Chavez Mixco
    Creation Date: Sunday, January 29, 2017
    */
    private void GetFinalName(string aBaseName)
    {
        //If we want to change the base namae
        if (m_changeBaseName == true)
        {
            //Set the new base name
            m_finalName = m_baseName;
        }
        else//If we dont want to change the base name
        {
            //Set this as the base name
            m_finalName = aBaseName;
        }

        //If prefixes are being used
        if (m_usePrefix == true)
        {
            //Add the prefix to the base name
            m_finalName = m_prefix + m_finalName;
        }

        //If suffixes name are being used
        if (m_useSuffix == true)
        {
            //Add the suffix, to the current final name
            m_finalName += m_suffix;
        }
    }

    /*
    Description: Get all the children of a parent object, and rename them.
    Creator: Alvaro Chavez Mixco
    Creation Date: Sunday, January 29, 2017
    */
    private void RenameChildrenObjects()
    {
        //Rename the game objects children from the parent object
        RenameObjects(CUtilEditorTools.GetChildrenGameObjectFromParent(m_parentObjectToRename));
    }

    /*
    Description: Go through all the objects in the array, and change their name. This includes numbering
                 the objects if necessary.
    Parameters: GameObject[] aArrayObjects - The objects that will be renamed.
    Creator: Alvaro Chavez Mixco
    Creation Date: Sunday, January 29, 2017
    */
    private void RenameObjects(GameObject[] aArrayObjects)
    {
        //If the arraay is valid
        if (aArrayObjects != null)
        {
            //Go through all the objects
            for (int i = 0; i < aArrayObjects.Length; i++)
            {
                //If the object is valid
                if (aArrayObjects[i] != null)
                {
                    //Get the final name of the object, not numbered, according to prefixes and suffixes
                    GetFinalName(aArrayObjects[i].name);

                    //Change the name 
                    aArrayObjects[i].name = m_finalName;

                    //For efficiency we could place this if statement outside of the loop, and have 2 different loops. But since its editor efficiency is not a major concern
                    //If we want to number objects
                    if (m_numberObjects == true)
                    {
                        //Add the number desired
                        aArrayObjects[i].name += (m_startingNumber + (i * m_stepAmount)).ToString();
                    }
                }
            }
        }
    }

    /*
    Description: Function to sort, according to the assigned sorting delegate, an array
                 of gameobjects in the Unity hierarchy.
    Parameters: GameObject[] aArrayObjects - The objects that will be sorted.
    Creator: Alvaro Chavez Mixco
    Creation Date:  Tuesday, February 14th, 2017                       
    */
    private void ReorderObjects(GameObject[] aArrayObjects)
    {
        //If it will reorder objects alphanumerically and the objects to rename are valid
        if (m_shouldReorderObjectsAlphanumerically == true && aArrayObjects != null)
        {
            //Copy the arrays of objects into a list for easier sorting
            List<GameObject> objectsToOrder = new List<GameObject>(aArrayObjects);

            //Sort the objects according to the determined delegate
            objectsToOrder.Sort(new Comparison<GameObject>(m_delegSorting));

            //Go through all the objects in the list
            for (int i = 0; i < objectsToOrder.Count; i++)
            {
                //If the object is valid
                if (objectsToOrder[i] != null)
                {
                    //Set its sibling index (order in Unity hierarchy) according to the
                    //already sorted list index.
                    objectsToOrder[i].transform.SetSiblingIndex(i);
                }
            }

        }
    }
}
#endif