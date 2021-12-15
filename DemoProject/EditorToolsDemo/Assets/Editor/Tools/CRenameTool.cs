//If it running in the unity editor
#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System;

/// <summary>
/// Tool to rename all the children gameobjects of a single parent game object.
/// </summary>
/// <Creator>Alvaro Chavez Mixco</Creator>
/// <CreationDate>Sunday, January 29th, 2017</CreationDate>
public class CRenameTool : EditorWindow
{
    #region Constants
    //Constants
    private const string M_LABEL_SELECT_OBJECTS = "Select objects to rename";

    private const string M_TOGGLE_RENAME_SELECTION = "Rename Selection";
    private const string M_TOGGLE_CHANGE_BASE_NAME = "Change Base Name";
    private const string M_TOGGLE_USE_PREFIX = "Use Prefix";
    private const string M_TOGGLE_USE_SUFFIX = "Use Suffix";
    private const string M_TOGGLE_NUMBER_OBJECTS = "Number Objects";
    private const string M_TOGGLE_SHOULD_REORDER_OBJECTS_ALPHANUMERICALLY = "Reorder Objects Alphanumerically";
    private const string M_TOGGLE_REORDER_SORT_METHOD_ALPHANUMERICALLY = "Sort Alphanumerically (On) or inverse Alphanumerically (Off)";

    private const string M_INT_BASE_NUMBER = "Base Number: ";
    private const string M_INT_STEP = "Step: ";

    private const string M_TEXT_BASE_NAME = "Base Name: ";
    private const string M_TEXT_PREFIX = "Prefix: ";
    private const string M_TEXT_SUFFIX = "Suffix: ";

    private const string M_OBJECT_PARENT = "Parent Object: ";

    private const string M_BUTTON_RENAME = "Rename Objects";
    #endregion

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

    /// <summary>
    /// Delegate for function used to compare 2 objects when making an
    /// arbitrary sort
    /// </summary>
    /// <param name="objectA" type="GameObject">The fist object that will be compared in the sort</param>
    /// <param name="objectB" type="GameObject">The second object that will be compared in the sort</param>
    /// <returns type="int"> Less than 0: A is less than b..
    /// Equals 0: A equals B.
    /// Greater than 0:  A is greater than B.</returns>
    private delegate int delegSorting(GameObject objectA, GameObject objectB);
    private delegSorting m_delegSorting;

    #region UI
    /// <summary>
    /// Show the tab on the editor
    /// </summary>
    /// <Creator>Alvaro Chavez Mixco</Creator>
    /// <CreationDate>Sunday, January 29th, 2017</CreationDate>
    [MenuItem("Tools/Rename")]
    private static void Init()
    {
        //Create a new editor window, or get an existing one
        CRenameTool window = (CRenameTool)EditorWindow.GetWindow(typeof(CRenameTool));

        //Show the editor window option in the Unity toolbar
        window.Show();
    }

    /// <summary>
    /// Show all the different options the user has for renaming the object/s, and
    /// handles the interaction/functionality of the UI shown
    /// </summary>
    /// <Creator>Alvaro Chavez Mixco</Creator>
    /// <CreationDate>Sunday, January 29th, 2017</CreationDate>
    private void OnGUI()
    {
        //Show toggle whether it should rename the object selection
        m_renameSelection = EditorGUILayout.ToggleLeft(M_TOGGLE_RENAME_SELECTION, m_renameSelection);

        //Add space to window
        EditorGUILayout.Space();

        //Display the field to modify the text to set as name
        DisplayRenamingFields();

        //Display the field whether should objects be ordered alphanumerically
        DisplayOrderingAlphanumerically();

        //If we want to rename the objects selected
        if (m_renameSelection == true)
        {
            //Get the current game objects slected from editor
            GameObject[] objectsToRename = Selection.gameObjects;

            //If the objects we want to rename are valid
            if (objectsToRename != null)
            {
                //If the button is pressed and there are 
                //objects selected
                if (GUILayout.Button(M_BUTTON_RENAME) == true &&
                    objectsToRename.Length > 0)
                {
                    //Rename and reorder the objects
                    RenameReorderObjects(objectsToRename);
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
        //If it will rename the children of an object
        else
        {
            //Add space to window
            EditorGUILayout.Space();

            //Object field of the parent object containing everything
            m_parentObjectToRename = (GameObject)EditorGUILayout.ObjectField(
                M_OBJECT_PARENT, m_parentObjectToRename, typeof(GameObject), true);

            //If there is a prefab
            if (m_parentObjectToRename != null)
            {
                //If the button is pressed
                if (GUILayout.Button(M_BUTTON_RENAME))
                {
                    //Rename and reorder the objects
                    RenameReorderObjects(
                        CUtilEditorTools.GetChildrenGameObjectFromParent(m_parentObjectToRename));
                }
            }
        }
    }

    /// <summary>
    /// Display the editor fields corresponding to changing the name of objects
    /// </summary>
    /// <Creator>Alvaro Chavez Mixco</Creator>
    /// <CreationDate>Sunday, January 29th, 2017</CreationDate>
    private void DisplayRenamingFields()
    {
        //Toggle should it change the base name
        m_changeBaseName = EditorGUILayout.ToggleLeft(M_TOGGLE_CHANGE_BASE_NAME, m_changeBaseName);

        //If we want to change the base name
        if (m_changeBaseName == true)
        {
            //Text field to write the base name
            m_baseName = EditorGUILayout.TextField(M_TEXT_BASE_NAME, m_baseName);
        }

        //Add space to window
        EditorGUILayout.Space();

        //Toggle should it use the prefix
        m_usePrefix = EditorGUILayout.ToggleLeft(M_TOGGLE_USE_PREFIX, m_usePrefix);

        //If it would allow prefixes
        if (m_usePrefix == true)
        {
            //Text field for prefix
            m_prefix = EditorGUILayout.TextField(M_TEXT_PREFIX, m_prefix);
        }

        //Add space to window
        EditorGUILayout.Space();

        //Toggle should it use the suffix
        m_useSuffix = EditorGUILayout.ToggleLeft(M_TOGGLE_USE_SUFFIX, m_useSuffix);

        //If it would allow suffixes
        if (m_useSuffix == true)
        {
            //Text field for suffix
            m_suffix = EditorGUILayout.TextField(M_TEXT_SUFFIX, m_suffix);
        }

        //Add space to window
        EditorGUILayout.Space();

        //Toggle for it should number objects
        m_numberObjects = EditorGUILayout.ToggleLeft(M_TOGGLE_NUMBER_OBJECTS, m_numberObjects);

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

    /// <summary>
    /// Display the editor fields corresponding to ordering the objects alphanumerically
    /// </summary>
    /// <Creator>Alvaro Chavez Mixco</Creator>
    /// <CreationDate>Sunday, January 29th, 2017</CreationDate>
    private void DisplayOrderingAlphanumerically()
    {
        //Add space
        EditorGUILayout.Space();

        //Toggle should it reorder objects Alphanumerically 
        m_shouldReorderObjectsAlphanumerically = EditorGUILayout.ToggleLeft(M_TOGGLE_SHOULD_REORDER_OBJECTS_ALPHANUMERICALLY,
            m_shouldReorderObjectsAlphanumerically);

        //If we are reordering objects alphanumerically
        if (m_shouldReorderObjectsAlphanumerically == true)
        {
            //Toggle should it reorder objects Alphanumerically normally or the inverse of it
            m_sortAlphanumerically = EditorGUILayout.ToggleLeft(M_TOGGLE_REORDER_SORT_METHOD_ALPHANUMERICALLY, m_sortAlphanumerically);

            //If it will reorders objects alphanumerically
            if (m_sortAlphanumerically == true)
            {
                //Assign delegate
                m_delegSorting = CUtilEditorTools.SortByAlphanumerically;
            }
            //If it will reorder objects inverse alphanumerically
            else
            {
                //Assign delegate
                m_delegSorting = CUtilEditorTools.SortByInverseAlphanumerically;
            }
        }

        //Add space
        EditorGUILayout.Space();
    }
    #endregion

    /// <summary>
    /// Function to rename and reorders the objects passed as parameter
    /// </summary>
    /// <Creator>Alvaro Chavez Mixco</Creator>
    /// <CreationDate>Monday, October 14th, 2019</CreationDate>
    /// <param name="aObjectsToRename" type="GameObject[]">The GameObjects that will be renamed and reordered</param>
    /// <remarks>This function is intended to be used when the rename button is pressed, and therefore
    /// makes use of the local variables of this class</remarks>
    private void RenameReorderObjects(GameObject[] aObjectsToRename)
    {
        //Rename the objects
        RenameObjects(aObjectsToRename, m_baseName, m_prefix, m_suffix,
            m_changeBaseName, m_usePrefix, m_useSuffix,
            m_numberObjects, m_startingNumber, m_stepAmount);

        //If the objects should be sorted
        if (m_shouldReorderObjectsAlphanumerically == true)
        {
            //Reorder the objects
            ReorderObjects(aObjectsToRename, m_delegSorting);
        }
    }

    /// <summary>
    /// Calculate the final name that the object will have, according to the base name, and 
    /// the prefixes and suffixes.This doesn't include the number.
    /// </summary>
    /// <Creator>Alvaro Chavez Mixco</Creator>
    /// <CreationDate>Sunday, January 29th, 2017</CreationDate>
    /// <param name="aOriginalName" type="string">The original name, the name that the object being
    /// renamed curretly has </param>
    /// <param name="aBaseName" type="string">The base name of the object</param>
    /// <param name="aPrefix" type="string">The prefix that will be added before the name of the obejct</param>
    /// <param name="aSuffix" type="string">The sufix that will be added after the name of the object</param>
    /// <param name="aChangeBaseName" type="bool">Indicates whether the original name of the object
    /// should be changed by the base name</param>
    /// <param name="aUsePrefix" type="bool">Indicates whether a prefix should be added to the name</param>
    /// <param name="aUseSuffix" type="bool">Indicates whether a sufix should be added to the name</param>
    /// <returns type="string">Returns the final name of the object, with the changed base name, the
    /// prefix, and the sufix (if these options are desired)</returns>
    private string GetFinalName(string aOriginalName, string aBaseName, string aPrefix, string aSuffix,
        bool aChangeBaseName, bool aUsePrefix, bool aUseSuffix)
    {
        //Save the orignal name of the object
        string finalName = aOriginalName;

        //If we want to change the base namae
        if (aChangeBaseName == true)
        {
            //Set the new base name
            finalName = aBaseName;
        }

        //If prefixes are being used
        if (aUsePrefix == true)
        {
            //Add the prefix to the base name
            finalName = aPrefix + finalName;
        }

        //If suffixes name are being used
        if (aUseSuffix == true)
        {
            //Add the suffix, to the current final name
            finalName += aSuffix;
        }

        return finalName;
    }

    /// 
    /// <summary>
    /// Go through all the objects in the array, and change their name. This includes numbering
    /// the objects if necessary.
    /// </summary>
    /// <Creator>Alvaro Chavez Mixco</Creator>
    /// <CreationDate>Sunday, January 29th, 2017</CreationDate>
    /// <param name="aArrayObjects" type ="GameObject[]">The array of  all the GameObjects that will be renamed</param>
    /// <param name="aBaseName" type="string">The base name of the object</param>
    /// <param name="aPrefix" type="string">The prefix that will be added before the name of the obejct</param>
    /// <param name="aSuffix" type="string">The sufix that will be added after the name of the object</param>
    /// <param name="aChangeBaseName" type="bool">Indicates whether the original name of the object
    /// should be changed by the base name</param>
    /// <param name="aUsePrefix" type="bool">Indicates whether a prefix should be added to the name</param>
    /// <param name="aUseSuffix" type="bool">Indicates whether a sufix should be added to the name</param>
    /// <param name="aNumberObjects" type="bool" >Indicates whether the objects should be numbered</param>
    /// <param name="aStartingNumber" type="int">The starting number used for the numbering</param>
    /// <param name="aNumberingStepAmount" type="int">The amount that will increase for each object when numbering</param> 
    private void RenameObjects(GameObject[] aArrayObjects, string aBaseName, string aPrefix, string aSuffix,
        bool aChangeBaseName, bool aUsePrefix, bool aUseSuffix,
        bool aNumberObjects, int aStartingNumber, int aNumberingStepAmount)
    {
        //If the array is valid
        if (aArrayObjects != null)
        {
            //Record the change of all objects
            Undo.RecordObjects(aArrayObjects, "Rename objects");

            //If the objects will be numbered
            if (aNumberObjects == true)
            {
                //Go through all the objects
                for (int i = 0; i < aArrayObjects.Length; i++)
                {
                    //If the object is valid
                    if (aArrayObjects[i] != null)
                    {
                        //Get the final name of the object, not numbered, according to prefixes and suffixes                  
                        aArrayObjects[i].name = GetFinalName(aArrayObjects[i].name, aBaseName, aPrefix, aSuffix,
                            aChangeBaseName, aUsePrefix, aUseSuffix);

                        //Number the objects
                        aArrayObjects[i].name += (aStartingNumber + (i * aNumberingStepAmount)).ToString();
                    }
                }
            }
            //If the objects will not be numbereD
            else
            {
                //Go through all the objects
                for (int i = 0; i < aArrayObjects.Length; i++)
                {
                    //If the object is valid
                    if (aArrayObjects[i] != null)
                    {
                        //Get the name of the object, according to prefixes and suffixes, without numbering it              
                        aArrayObjects[i].name = GetFinalName(aArrayObjects[i].name, aBaseName, aPrefix, aSuffix,
                            aChangeBaseName, aUsePrefix, aUseSuffix);
                    }
                }
            }
        }
    }

    /// <summary>
    /// Function to sort, according to the assigned sorting delegate, an array
    /// of gameobjects in the Unity hierarchy.
    /// </summary>
    /// <Creator>Alvaro Chavez Mixco</Creator>
    /// <CreationDate>Tuesday, February 14th, 2017</CreationDate> 
    /// <param name="aArrayObjects" type="GameObject[]">The GameObjects that will be sorted</param>
    /// <param name="aSortingDelegate" type="delegSorting">The sorting function used to the objects</param>
    private void ReorderObjects(GameObject[] aArrayObjects, delegSorting aSortingDelegate)
    {
        //If the objects to rename are valid
        if (aArrayObjects != null)
        {
            //Copy the arrays of objects into a list for easier sorting
            List<GameObject> objectsToOrder = new List<GameObject>(aArrayObjects);

            //Sort the objects according to the determined delegate
            objectsToOrder.Sort(new Comparison<GameObject>(aSortingDelegate));

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