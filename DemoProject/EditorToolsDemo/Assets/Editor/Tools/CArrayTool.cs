using UnityEngine;
using System.Collections;

using UnityEditor;
using System.Collections.Generic;

//If the program is in the Unity editor
#if UNITY_EDITOR

/// <summary>
/// Unity Editor Tool to place objects in a 3D array. This works similar to 3DS Max array tool.
/// </summary>
/// <Creator>Alvaro Chavez Mixco</Creator>
/// <CreationDate>Sunday, January 29, 2017</CreationDate>

public class CArrayTool : EditorWindow
{
    private const string M_LABEL_COPY = "Copy Objects";
    private const string M_LABEL_PLACE = "Place Objects";
    private const string M_LABEL_ERROR_OBJECT_NOT_PREFAB = "The object you wish to copy is not a prefab.";

    private const string M_TOGGLE_IS_COPYING = "Is Copying Object: ";
    private const string M_TOGGLE_COPY_AS_INSTANCE = "Copy as Instance: ";
    private const string M_TOGGLE_RENAME_OBJECTS = "Rename Objects: ";
    private const string M_TOGGLE_GET_ELEMENTS_FROM_PARENT = "Get Elements from Parent Object: ";
    private const string M_TOGGLE_INCREMENTAL_ARRAY = "Is Incremental Array: ";

    private const string M_VECTOR3_ARRAY_DIMENSIONS = "Array Dimensions: ";
    private const string M_VECTOR3_INCREMENTAL_DISTANCE = "Incremental Distance: ";
    private const string M_VECTOR3_INCREMENTAL_ANGLE = "Incremental Angle Degrees: ";
    private const string M_VECTOR3_INCREMENTAL_SCALE = "Incremental Scale: ";
    private const string M_VECTOR3_TOTAL_DISTANCE = "Total Distance: ";
    private const string M_VECTOR3_TOTAL_ANGLE = "Total Angle Degrees: ";
    private const string M_VECTOR3_TOTAL_SCALE = "Total Scale: ";

    private const string M_OBJECT_TO_COPY = "Object to Copy: ";
    private const string M_OBJECT_PARENT_OBJECT = "Parent Object: ";
    private const string M_OBJECT_BASE_OBJECT_TO_PLACE = "Object to Place: ";

    private const string M_BUTTON_COPY = "Copy Object";
    private const string M_BUTTON_PLACE = "Place Objects";

    private const string M_FOLDOUT_ELEMENTS_TO_PLACE = "Elements to Place: ";
    private const string M_FOLDOUT_ARRAY_PROPERTIES = "Array Properties: ";

    //Used to optimize and not recalculate arrays each frame
    private Vector3 m_previousArrayDimensions = Vector3.zero;
    private GameObject m_previousParentObject = null;
    private int m_previousParentObjectchildCount = 0;

    //Copying elements properties
    private bool m_isCopyingElement = true;
    private bool m_isCopyingAsInstance = true;
    private bool m_renameObjects = true;

    //Place elements according to parent properties
    private bool m_getElementsFromParent = true;

    //Array properties
    private bool m_isIncrementalArray = true;
    private Vector3 m_arrayDimensions = new Vector3(1, 1, 1);

    //Incremental array properties
    private Vector3 m_incrementalDistance = Vector3.zero;
    private Vector3 m_incrementalAngle = Vector3.zero;
    private Vector3 m_incrementalScale = Vector3.zero;

    //Total array properties
    private Vector3 m_totalDistance = Vector3.zero;
    private Vector3 m_totalAngle = Vector3.zero;
    private Vector3 m_totalScale = Vector3.zero;

    //Foldouts
    private bool m_foldoutArrayProperties = true;
    private bool m_foldoutArrayElementsToPlace = true;

    //Objects to copy/obtain children from/ place
    private GameObject m_objectToCopy = null;
    private GameObject m_parentObject = null;
    private GameObject[,,] m_listObjectsToPlace = null;

    private delegate GameObject delegInstantiateObject(GameObject aObjectPrefab, string aText);

    /// <summary>
    /// Show the Array Tool tab on the edtior
    /// </summary>
    /// <Creator>Alvaro Chavez Mixco</Creator>
    /// <CreationDate>Sunday, January 29, 2017</CreationDate>
    [MenuItem("Custom Tools/Array Tool")]
    private static void Init()
    {
        //Create an editor window
        CArrayTool window = (CArrayTool)EditorWindow.GetWindow(typeof(CArrayTool));

        //Show the window
        window.Show();
    }

    /// <summary>
    /// Show all the field that user can fill to place the prefabs, and handles the logic
    /// for when the user press a button
    /// </summary>
    /// <Creator>Alvaro Chavez Mixco</Creator>
    /// <CreationDate>Sunday, January 29, 2017</CreationDate>
    private void OnGUI()
    {
        //If we want to copy objectts
        if (m_isCopyingElement == true)
        {
            //Display a copy label
            EditorGUILayout.LabelField(M_LABEL_COPY);
        }
        //If we want to place elements
        else
        {
            //Display a place label
            EditorGUILayout.LabelField(M_LABEL_PLACE);
        }

        //Add space in the editor
        EditorGUILayout.Space();

        //Dispaly copy elements toggle
        m_isCopyingElement = EditorGUILayout.Toggle(M_TOGGLE_IS_COPYING, m_isCopyingElement);

        //Display all the array properties
        DisplayArrayProperties();

        //If we want to copy elements
        if (m_isCopyingElement == true)
        {
            //Display the corresponding fields
            DisplayCopyElementFields();

            //If we are using a total array, calculate the incremental properties
            CalculateIncrementalProperties();

            //If there is a prefab
            if (m_objectToCopy != null)
            {
                //If it is copying object as instance of prefab
                if (m_isCopyingAsInstance == true)
                {
                    //If the object we want to copy is not a prefab, or its type can't be determined
                    if (PrefabUtility.GetPrefabAssetType(m_objectToCopy) == PrefabAssetType.NotAPrefab ||
                        PrefabUtility.GetPrefabAssetType(m_objectToCopy) == PrefabAssetType.MissingAsset)
                    {
                        //Display error message
                        GUILayout.Label(M_LABEL_ERROR_OBJECT_NOT_PREFAB);
                    }
                    //If the object we want to copy is a prefab
                    else
                    {
                        //If the button is press
                        if (GUILayout.Button(M_BUTTON_COPY))
                        {
                            //Copy the objects
                            CopyObject();
                        }
                    }
                }
                //If its not copying as instance of prefab
                else
                {
                    //If the button is press
                    if (GUILayout.Button(M_BUTTON_COPY))
                    {
                        //Copy the objects
                        CopyObject();
                    }
                }

            }
        }
        //If we want to place objects in the scene
        else
        {
            //Display the corresponding fields
            DisplayPlaceElementsFields();

            //If we are using a total array, calculate the incremental properties
            CalculateIncrementalProperties();

            //If there is a valid list of objects to place
            if (m_listObjectsToPlace != null)
            {
                //If the button is press
                if (GUILayout.Button(M_BUTTON_PLACE))
                {
                    //Place, rotate, and scale the desired objects
                    ScaleRotateTranslateObjects(ref m_listObjectsToPlace);
                }
            }
        }
    }

    /// <summary>
    /// Display in the editor window the options to copy an object.
    /// </summary>
    /// <Creator>Alvaro Chavez Mixco</Creator>
    /// <CreationDate>Sunday, January 29, 2017</CreationDate>
    private void DisplayCopyElementFields()
    {
        //Add space in window
        EditorGUILayout.Space();

        //Object field for the object we want to copy
        m_objectToCopy = (GameObject)EditorGUILayout.ObjectField(M_OBJECT_TO_COPY, m_objectToCopy, typeof(GameObject), true);

        //Toggle if the object should be copied as an instance, keep prefab connection
        m_isCopyingAsInstance = EditorGUILayout.Toggle(M_TOGGLE_COPY_AS_INSTANCE, m_isCopyingAsInstance);

        //Toggle if we should rename the new objects created
        m_renameObjects = EditorGUILayout.Toggle(M_TOGGLE_RENAME_OBJECTS, m_renameObjects);

        //Add space in window
        EditorGUILayout.Space();
    }

    /// <summary>
    /// Display in the editor window the options to place an object
    /// </summary>
    /// <Creator>Alvaro Chavez Mixco</Creator>
    /// <CreationDate>Sunday, January 29, 2017</CreationDate>
    private void DisplayPlaceElementsFields()
    {
        //Add space in window
        EditorGUILayout.Space();

        //If the array has changed size, this will clear the preivous entries in the array
        //This is to avoid recalculating the array each frame
        if (m_arrayDimensions != m_previousArrayDimensions)
        {
            //Create the array  with desired dimensions
            m_listObjectsToPlace = new GameObject[(int)m_arrayDimensions.x, (int)m_arrayDimensions.y, (int)m_arrayDimensions.z];

            //Reset the previous data
            ClearPreviousOperationData();

            //Save the new size
            m_previousArrayDimensions = m_arrayDimensions;
        }

        //Toggle if we want to get all the objects from the parent object
        m_getElementsFromParent = EditorGUILayout.Toggle(M_TOGGLE_GET_ELEMENTS_FROM_PARENT, m_getElementsFromParent);

        //If we want to get the elements from the parent object
        if (m_getElementsFromParent == true)
        {
            //Display an object field where to set the parent object
            m_parentObject = (GameObject)EditorGUILayout.ObjectField(M_OBJECT_PARENT_OBJECT, m_parentObject, typeof(GameObject), true);

            //If the parent object is valid
            if (m_parentObject != null && (m_parentObject != m_previousParentObject))
            {
                //Optimization, if the parent object is different from the previos one, or if their 
                //child count differ
                if (m_parentObject != m_previousParentObject ||
                    m_parentObject.transform.childCount != m_previousParentObjectchildCount)
                {
                    //Get all the children game object from the parent
                    GameObject[] childrenObjects = CUtilEditorTools.GetChildrenGameObjectFromParent(m_parentObject);

                    //Convert the array of children into a 3D array so that it can be used with the editor
                    m_listObjectsToPlace = CUtilEditorTools.Convert1DArrayGameObjectsTo3DArray(childrenObjects, m_arrayDimensions);

                    //Save the current parent object properties 
                    m_previousParentObject = m_parentObject;
                    m_previousParentObjectchildCount = m_parentObject.transform.childCount;
                }
            }
        }
        //If we are going to manually set all the objects that will be placed
        else
        {
            //Display a fold out for the elements to place
            m_foldoutArrayElementsToPlace = EditorGUILayout.Foldout(m_foldoutArrayElementsToPlace, M_FOLDOUT_ELEMENTS_TO_PLACE);

            //If we want to show the elements
            if (m_foldoutArrayElementsToPlace == true)
            {
                //Create an object field for each element in the array
                m_listObjectsToPlace = Instantiate3DArray(InstantiateObjectField, m_listObjectsToPlace);
            }
        }

        //Add space in window
        EditorGUILayout.Space();
    }

    /// <summary>
    /// Display in the editor window the options to set the properties of an array.
    /// </summary>
    /// <Creator>Alvaro Chavez Mixco</Creator>
    /// <CreationDate>Sunday, January 29, 2017</CreationDate>
    private void DisplayArrayProperties()
    {
        //Add space in window
        EditorGUILayout.Space();

        //Create a field to set the dimensions of the array
        m_arrayDimensions = EditorGUILayout.Vector3Field(M_VECTOR3_ARRAY_DIMENSIONS, m_arrayDimensions);

        //Create togle for whether the array should use incremental or total values
        m_isIncrementalArray = EditorGUILayout.Toggle(M_TOGGLE_INCREMENTAL_ARRAY, m_isIncrementalArray);

        //Display a fold out for the elements to place
        m_foldoutArrayProperties = EditorGUILayout.Foldout(m_foldoutArrayProperties, M_FOLDOUT_ARRAY_PROPERTIES);

        //If we want to show the array properties
        if (m_foldoutArrayProperties == true)
        {
            //If it is an incremental array
            if (m_isIncrementalArray == true)
            {
                //The incremental properties fields
                //Distance 
                m_incrementalDistance = EditorGUILayout.Vector3Field(M_VECTOR3_INCREMENTAL_DISTANCE, m_incrementalDistance);
                //Angle Degrees
                m_incrementalAngle = EditorGUILayout.Vector3Field(M_VECTOR3_INCREMENTAL_ANGLE, m_incrementalAngle);
                //Scale
                m_incrementalScale = EditorGUILayout.Vector3Field(M_VECTOR3_INCREMENTAL_SCALE, m_incrementalScale);
            }
            else//If it is a total array
            {
                //The total properties  fields
                //Distance
                m_totalDistance = EditorGUILayout.Vector3Field(M_VECTOR3_TOTAL_DISTANCE, m_totalDistance);
                //Angle Degrees
                m_totalAngle = EditorGUILayout.Vector3Field(M_VECTOR3_TOTAL_ANGLE, m_totalAngle);
                //Scale
                m_totalScale = EditorGUILayout.Vector3Field(M_VECTOR3_TOTAL_SCALE, m_totalScale);
            }
        }

        //Add space to window
        EditorGUILayout.Space();
    }

    /*
    Description: Calculate the incremental properties (distance, angle, scale) of the array 
                 according to the total values.
    Creator: Alvaro Chavez Mixco
    Creation Date: Sunday, January 29, 2017
    */
    private void CalculateIncrementalProperties()
    {
        //If we are usign a total array
        if (m_isIncrementalArray == false)
        {
            //Get the distance between objects according to size of array
            m_incrementalDistance = CUtilEditorTools.GetValuesBetweenElements(m_totalDistance, m_arrayDimensions);

            //Get the angle between objects according to size of array
            m_incrementalAngle = CUtilEditorTools.GetValuesBetweenElements(m_totalAngle, m_arrayDimensions);

            //Get the scale between objects according to size of array
            m_incrementalScale = CUtilEditorTools.GetValuesBetweenElements(m_totalScale, m_arrayDimensions);
        }
    }

    /*
    Description: Clear the data from te previous operation
    Creator: Alvaro Chavez Mixco
    Creation Date: Sunday, January 29, 2017
    */
    private void ClearPreviousOperationData()
    {
        m_previousParentObject = null;
        m_previousParentObjectchildCount = 0;

        m_previousArrayDimensions = Vector3.zero;
    }

    /*
    Description: Go through all the indices in the 3D array, and using the desired instantiation function create
                 the desired prefab. The function returns the created objects.
    Parameters: delegInstantiateObject aInstantiationFunction - The function that will be used to instantiate the prefab.
                GameObject aPrefab - The prefab that will be instantiated.
    Creator: Alvaro Chavez Mixco
    Creation Date: Sunday, January 29, 2017
    */
    private GameObject[,,] Instantiate3DArray(delegInstantiateObject aInstantiationFunction, GameObject aPrefab)
    {
        //Create an array with the desired dimensions
        GameObject[,,] createdObjects = new GameObject[(int)m_arrayDimensions.x, (int)m_arrayDimensions.y, (int)m_arrayDimensions.z];

        //If the instantiation function is valid
        if (aInstantiationFunction != null)
        {
            string indexName = string.Empty;

            //Go through all the X rows in the array
            for (int rowX = 0; rowX < (int)m_arrayDimensions.x; rowX++)
            {
                //Go through all the Y rows in the array
                for (int rowY = 0; rowY < (int)m_arrayDimensions.y; rowY++)
                {
                    //Go through all the Z rows in the array
                    for (int rowZ = 0; rowZ < (int)m_arrayDimensions.z; rowZ++)
                    {
                        //Save the current index as a string
                        indexName = " " + rowX + ", " + rowY + ", " + rowZ;

                        //Create an object and stored in the prefab
                        createdObjects[rowX, rowY, rowZ] = aInstantiationFunction(aPrefab, indexName);

                        //If the object at current idnex is valid and we want to rename the object
                        if (createdObjects[rowX, rowY, rowZ] != null && m_renameObjects == true)
                        {
                            //Change the name of the object according to the index
                            createdObjects[rowX, rowY, rowZ].name = createdObjects[rowX, rowY, rowZ].name + indexName;
                        }

                    }
                }
            }
        }

        //Return the array of the objects created
        return createdObjects;
    }

    /*
    Description: Go through all the indices in the 3D array, and using the desired instantiation function create
                 the desired prefab for that specific coordinate.  The function returns the created objects.
    Parameters: delegInstantiateObject aInstantiationFunction - The function that will be used to instantiate the prefab.
                GameObject[,,] aArrayOfObjects) - The array of prefabs/objects that will be used when sampling which objects to
                                                  create in the 3D array.
    Creator: Alvaro Chavez Mixco
    Creation Date: Thursday, February 16th, 2017
    */
    private GameObject[,,] Instantiate3DArray(delegInstantiateObject aInstantiationFunction, GameObject[,,] aArrayOfObjects)
    {
        //Create an array with the desired dimensions
        GameObject[,,] createdObjects = new GameObject[(int)m_arrayDimensions.x, (int)m_arrayDimensions.y, (int)m_arrayDimensions.z];
                  
        //If the array of objects being passed is null
        if (aArrayOfObjects == null)
        {
            //Set it as an empty, but valid, array
            aArrayOfObjects = createdObjects;
        }

        //If the arrays don't match size
        if (createdObjects.Length != aArrayOfObjects.Length)
        {
            //Return the original array
            return aArrayOfObjects;
        }

        //If the instantiation function is invalid
        if (aInstantiationFunction == null)
        {
            //Return the original array
            return aArrayOfObjects;
        }

        string indexName = string.Empty;

        //Go through all the X rows in the array
        for (int rowX = 0; rowX < (int)m_arrayDimensions.x; rowX++)
        {
            //Go through all the Y rows in the array
            for (int rowY = 0; rowY < (int)m_arrayDimensions.y; rowY++)
            {
                //Go through all the Z rows in the array
                for (int rowZ = 0; rowZ < (int)m_arrayDimensions.z; rowZ++)
                {
                    //Save the current index as a string
                    indexName = " " + rowX + ", " + rowY + ", " + rowZ;

                    //Create an object and stored in the prefab
                    createdObjects[rowX, rowY, rowZ] = aInstantiationFunction(aArrayOfObjects[rowX, rowY, rowZ], indexName);

                    //If the object at current index is valid and we want to rename the object
                    if (createdObjects[rowX, rowY, rowZ] != null && m_renameObjects == true)
                    {
                        //Change the name of the object according to the index
                        createdObjects[rowX, rowY, rowZ].name = createdObjects[rowX, rowY, rowZ].name + indexName;
                    }

                }
            }
        }

        //Return the array of the objects created
        return createdObjects;
    }

    /*
    Description: Instantiate a gameobject from a prefab, but conserving the connection to the prefab
    Parameters: GameObject aPrefab - The gameobject prefab that will be created.
                string aText - Not used in this function
    Creator: Alvaro Chavez Mixco
    Creation Date: Sunday, January 29, 2017
    */
    private GameObject InstantiantePrefab(GameObject aPrefab, string aText)
    {
        //If the prefab is valid
        if (aPrefab != null)
        {
            //Instantiate the object as reference to prefab
            GameObject referenceObject = (GameObject)PrefabUtility.InstantiatePrefab(aPrefab);

            //Because it is a prefab, ensure the new object transform matches the prefab
            referenceObject.transform.position = aPrefab.transform.position;

            return referenceObject;
        }

        return null;
    }

    /*
    Description: Instantiate a gameobject from a prefab, as a clone, no connection to prefab kept.
    Parameters: GameObject aPrefab - The gameobject prefab that will be created.
                string aText - Not used in this function
    Creator: Alvaro Chavez Mixco
    Creation Date: Sunday, January 29, 2017
    */
    private GameObject InstantianteClone(GameObject aPrefab, string aText)
    {
        //If the prefab being passed is valid
        if (aPrefab != null)
        {
            //Create a clone
            GameObject clone = (GameObject)Instantiate(m_objectToCopy);

            //delete "(clone)" on the name because it's annoying
            clone.name = aPrefab.name.Replace("(Clone)", "");

            //Return the cloned object
            return clone;
        }

        return null;
    }

    /*
    Description: Instantiate an object field in the game object window
    Parameters: GameObject aPrefab - The gameobject prefab that will be created.
                string aText - The 3D coords of the object
    Creator: Alvaro Chavez Mixco
    Creation Date: Sunday, January 29, 2017
    */
    private GameObject InstantiateObjectField(GameObject aGameObject, string aText)
    {
        //If the text is valid
        if (aText == null)
        {
            //Set it as empty
            aText = string.Empty;
        }

        //Create an object field
        return (GameObject)EditorGUILayout.ObjectField(M_OBJECT_BASE_OBJECT_TO_PLACE + aText, aGameObject, typeof(GameObject), true);
    }

    /*
    Description: Create a 3D array copies of the object
    Creator: Alvaro Chavez Mixco
    Creation Date: Sunday, January 29, 2017
    */
    private void CopyObject()
    {
        //if user want to keep the connection to the prefab
        if (m_isCopyingAsInstance == true)
        {
            //Spawn objects using the instantiate prefab function
            SpawnObjects(InstantiantePrefab, m_objectToCopy);
        }
        else //if user don't want to keep the connection to the prefab
        {
            //Spawn objects using the instantiate clone function
            SpawnObjects(InstantianteClone, m_objectToCopy);
        }
    }

    /*
    Description: Scale, Rotate, and Translate the objects in the list according to their index coordinates
                in the 3D array.
    Parameters: ref GameObject[,,] aListOfGameObjects - The list of gameobjects that will be scaled.
    Creator: Alvaro Chavez Mixco
    Creation Date: Sunday, January 29, 2017
    */
    private void ScaleRotateTranslateObjects(ref GameObject[,,] aListOfGameObjects)
    {
        //If the array of object is valid
        if (aListOfGameObjects != null)
        {
            //If the list has at least 1 elements
            if (aListOfGameObjects.Length > 0)
            {
                Vector3 startingScale = Vector3.zero;
                Vector3 startingRotation = Vector3.zero;
                Vector3 startingPosition = Vector3.zero;

                Vector3 newRotation = Vector3.zero;

                //If the first element in the array is valid
                if (aListOfGameObjects[0, 0, 0] != null)
                {
                    //Set his scale as the starting scale
                    startingScale = aListOfGameObjects[0, 0, 0].transform.localScale;

                    //Set his rotation as the starting rotation
                    startingRotation = aListOfGameObjects[0, 0, 0].transform.localEulerAngles;

                    //Set his postion as the starting position
                    startingPosition = aListOfGameObjects[0, 0, 0].transform.localPosition;
                }


                //Go through all the X rows in the array
                for (int rowX = 0; rowX < (int)m_arrayDimensions.x; rowX++)
                {
                    //Go through all the Y rows in the array
                    for (int rowY = 0; rowY < (int)m_arrayDimensions.y; rowY++)
                    {
                        //Go through all the Z rows in the array
                        for (int rowZ = 0; rowZ < (int)m_arrayDimensions.z; rowZ++)
                        {
                            //If the object is valid
                            if (aListOfGameObjects[rowX, rowY, rowZ] != null)
                            {
                                //Scale the object, local scale
                                aListOfGameObjects[rowX, rowY, rowZ].transform.localScale = startingScale +
                                    new Vector3(m_incrementalScale.x * rowX, m_incrementalScale.y * rowY, m_incrementalScale.y * rowZ);

                                //Calculate the rotation of each object
                                newRotation = startingRotation +
                                    new Vector3(m_incrementalAngle.x * rowX, m_incrementalAngle.y * rowY, m_incrementalAngle.z * rowZ);

                                //Set the rotation of each object
                                aListOfGameObjects[rowX, rowY, rowZ].transform.localEulerAngles = newRotation;

                                //Place the object at the desired position
                                aListOfGameObjects[rowX, rowY, rowZ].transform.localPosition = startingPosition +
                                    new Vector3(m_incrementalDistance.x * rowX, m_incrementalDistance.y * rowY, m_incrementalDistance.z * rowZ);
                            }
                        }
                    }
                }
            }
        }

        //Clear the data from the previous operation
        ClearPreviousOperationData();
    }

    /*
    Description: Instantiate a 3D array of objects, and then place, rotate and scale them.
    Parameters: delegInstantiateObject aInstantiationFunction - The function used to instantiate the object
                GameObject aPrefab - The object prefab that will be created.
    Creator: Alvaro Chavez Mixco
    Creation Date: Sunday, January 29, 2017
    */
    private void SpawnObjects(delegInstantiateObject aInstantiationFunction, GameObject aPrefab)
    {
        //Create the objects with the desired instantiation function, using the desired prefab
        GameObject[,,] createdObjects = Instantiate3DArray(aInstantiationFunction, aPrefab);

        //Place, rotate, and scale the objects
        ScaleRotateTranslateObjects(ref createdObjects);

        //Clear the data from the previous operation
        ClearPreviousOperationData();
    }
}
#endif