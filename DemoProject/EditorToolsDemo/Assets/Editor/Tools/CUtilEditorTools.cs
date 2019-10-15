//If the program is in the Unity editor
#if UNITY_EDITOR
using UnityEngine;

public class CUtilEditorTools
{
    /*
Description: Function to read a string and get numeric or char (not numeric characters) from a string.
             The function will return the values read.
Parameters: string aValueToRead - The string of the value that will be reaed
            ref int aStartingIndex - The index from which it will start reading the string, this will be updated after
                                     the function executes
            int aStringToReadLength - The length (number of characters) of the string that will be read
Creator: Alvaro Chavez Mixco
Creation Date:  Wednesday, February 15th, 2017
Extra Notes: Based on: https://www.dotnetperls.com/alphanumeric-sorting
             This function is inteded to work with the function SortByAlphanumerically, so it misses a lot
             of "if" checks.
*/
    private static char[] GetNumberCharsChunks(string aStringToRead, ref int aStartingIndex, int aStringToReadLength)
    {
        //Get the current character of both names
        char tempCharacter = aStringToRead[aStartingIndex];

        // Create arrays of the values read
        char[] readValues = new char[aStringToReadLength];

        //Index of the current values read
        int indexReadValues = 0;

        //While the character read is number matches the intial character is number condition
        //So it will read the string by chunks, determining if the char read is a number or not
        do
        {
            //Read the starting value
            readValues[indexReadValues] = tempCharacter;

            //Increase the index
            indexReadValues++;
            aStartingIndex++;

            //If the index is still valid
            if (aStartingIndex < aStringToReadLength)
            {
                //Read the next value
                tempCharacter = aStringToRead[aStartingIndex];
            }
            else//If the index is not valid
            {
                //Quit the loop
                break;
            }
        } while (char.IsNumber(tempCharacter) == char.IsNumber(readValues[0]));

        return readValues;
    }

    /*
    Description: A helper function to get all the children gameobjects from a parent game object.
    Parameters: GameObject aParentObject - The gameobject that is the parent of the desired children objects.
    Creator: Alvaro Chavez Mixco
    Creation Date: Sunday, January 29, 2017
    */
    public static GameObject[] GetChildrenGameObjectFromParent(GameObject aParentObject)
    {
        //If the parent object is valid
        if (aParentObject != null)
        {
            //Create an array of game objects, accordign to its number of children
            GameObject[] children = new GameObject[aParentObject.transform.childCount];

            //For each children the parent object has
            for (int i = 0; i < aParentObject.transform.childCount; i++)
            {
                //Get the children transform, and from it get its game object
                children[i] = (aParentObject.transform.GetChild(i)).gameObject;
            }

            //Return the children that were found
            return children;
        }

        return null;
    }

    /*
Description: Get the value between elements in each axis.
Parameters: Vector3 aTotalValues - The total value of the object
            Vector3 aTotalNumberOfElements - The number of object that will divide the total value.
Creator: Alvaro Chavez Mixco
Creation Date: Sunday, January 29, 20177
*/
    public static Vector3 GetValuesBetweenElements(Vector3 aTotalValues, Vector3 aTotalNumberOfElements)
    {
        Vector3 valuesBetweenElements = Vector3.zero;

        //Get the value between each x,y and z element, according to the total value
        //and the total number of elements
        valuesBetweenElements.x = aTotalValues.x / aTotalNumberOfElements.x;
        valuesBetweenElements.y = aTotalValues.y / aTotalNumberOfElements.y;
        valuesBetweenElements.z = aTotalValues.z / aTotalNumberOfElements.z;

        return valuesBetweenElements;
    }

    /*
Description: Helper function to get the 3D (X,Y, Z) index of an object in a array according to its 1D intex
             and the array size.
Parameters: int a1DIndex - The total or 1D index being converted to 3D.
            int aArrayWidth - The width, number fo columns, of the array
            int aArrayHeight - The height of the array
Creator: Alvaro Chavez Mixco
Creation Date: Sunday, January 29, 2017
*/
    public static Vector3 Convert1DIndexTo3DArrayIndex(int a1DIndex, int aArrayWidth, int aArrayHeight)
    {
        Vector3 index3D;
        index3D.z = a1DIndex / (aArrayWidth * aArrayHeight);
        a1DIndex -= ((int)index3D.z * aArrayWidth * aArrayHeight);
        index3D.y = a1DIndex / aArrayWidth;
        index3D.x = a1DIndex % aArrayWidth;

        return index3D;
    }

    /*
Description: Convert a 1D array of game objects into a 3D array of game objects. The function will 
             return the new 3D array of game objects.
Parameters: GameObject[] aArrayGameObject - The 1D array of game objects to convert
            Vector3 aArrayXYZDimensions - The dimensions for the 3D array
Creator: Alvaro Chavez Mixco
Creation Date: Sunday, January 29, 20177
*/
    public static GameObject[,,] Convert1DArrayGameObjectsTo3DArray(GameObject[] aArrayGameObject, Vector3 aArrayXYZDimensions)
    {
        //Create the 3D array of game objects
        GameObject[,,] gameObjects3DArray = new GameObject[(int)aArrayXYZDimensions.x, (int)aArrayXYZDimensions.y, (int)aArrayXYZDimensions.z];

        Vector3 temp3DIndex;

        //If the 1D array matches in size with the desired 3D array
        if (aArrayXYZDimensions.x * aArrayXYZDimensions.y * aArrayXYZDimensions.z == aArrayGameObject.Length)
        {
            //Go through all the gameobjects
            for (int i = 0; i < aArrayGameObject.Length; i++)
            {
                //Get the 3D coordinates according to the 1D invex
                temp3DIndex = Convert1DIndexTo3DArrayIndex(i, (int)aArrayXYZDimensions.x, (int)aArrayXYZDimensions.y);

                //Set the game object at the desired 3D intex
                gameObjects3DArray[(int)temp3DIndex.x, (int)temp3DIndex.y, (int)temp3DIndex.z] = aArrayGameObject[i];
            }
        }

        return gameObjects3DArray;
    }


    /*
Description: Function intented to be assigned to a delegate. 
             Function to compare 2 strings alhpanumerically according to their.
    Parameters: string aTextA - The text to compare
                string aTextB - The other text to compare
Creator: Alvaro Chavez Mixco
Creation Date:  Tuesday, February 14th, 2017
Extra Notes: Based on: https://www.dotnetperls.com/alphanumeric-sorting
             https://msdn.microsoft.com/en-us/library/tfakywbh(v=vs.110).aspx 
             Value           Meaning          
             Less than 0     x is less than y.
             0               x equals y.  
             Greater than 0  x is greater than y.                        
*/
    public static int SortByAlphanumerically(string aTextA, string aTextB)
    {
        //If both objects are valid
        if (string.IsNullOrEmpty(aTextA) == false && string.IsNullOrEmpty(aTextB) == false)
        {
            //Create an index to iterate over both objects
            int indexA = 0;
            int indexB = 0;

            string nameA = aTextA;
            string nameB = aTextB;

            //Go through each character in both of the objects name
            while (indexA < nameA.Length && indexB < nameB.Length)
            {
                // Create arrays of the values read, separate the numbers and characters in each array
                char[] readValuesA = GetNumberCharsChunks(nameA, ref indexA, nameA.Length);
                char[] readValuesB = GetNumberCharsChunks(nameB, ref indexB, nameB.Length);

                //Convert the chars arrays back into strings
                string stringReadA = new string(readValuesA);
                string stringReadB = new string(readValuesB);

                int result;

                //Check if the starting value of each string read is a number
                if (char.IsNumber(readValuesA[0]) == true && char.IsNumber(readValuesB[0]) == true)
                {
                    //Parse both chunks for int 
                    int numericChunkA = int.Parse(stringReadA);
                    int numericChunkB = int.Parse(stringReadB);

                    //Compare the ints normally
                    result = numericChunkA.CompareTo(numericChunkB);
                }
                else//If the strings start with a char
                {
                    //Compare strings normally
                    result = stringReadA.CompareTo(stringReadB);
                }

                //If we determine which string is bigger
                if (result != 0)
                {
                    //Return the result
                    return result;
                }
            }

            //Sort according to the actual length of the string
            return nameA.Length - nameB.Length;
        }

        //Return both values are equal
        return 0;
    }

    /*
    Description: Function intented to be assigned to a delegate. 
                 Function to compare 2 objects  inverse alhpanumerically according to their name. This is done
                by calling the normal SortByAlphanumerically function, but switching the order of the parameters.
    Parameters: string aTextA - The text to compare
                string aTextB - The other text to compare
    Creator: Alvaro Chavez Mixco
    Creation Date:  Tuesday, February 14th, 2017
    */
    public static int SortByInverseAlphanumerically(string aTextA, string aTextB)
    {
        //Call the normal SortByAlphanumerically function, but switching the order of the parameters.
        return SortByAlphanumerically(aTextB, aTextA);
    }

    /*
Description: Function intented to be assigned to a delegate. 
             Function to compare 2 gameobjects alhpanumerically according to their name.
Parameters: GameObject aObjectA - A gameobject
            GameObject aObjectB - The gameobject that will be compared to aObjectB
Creator: Alvaro Chavez Mixco
Creation Date:  Tuesday, March 20th, 2017                   
*/
    public static int SortByAlphanumerically(GameObject aObjectA, GameObject aObjectB)
    {
        //If both game objects are valid
        if (aObjectA != null && aObjectB != null)
        {
            //Sort them according to their name
            return SortByAlphanumerically(aObjectA.name, aObjectB.name);
        }

        //Return both values are equal
        return 0;
    }

    /*
    Description: Function intented to be assigned to a delegate. This is done
                    by calling the normal SortByAlphanumerically function, but switching the order of the parameters.
                 Function to compare 2 gameobjects alhpanumerically according to their name.
    Parameters: GameObject aObjectA - A gameobject
                GameObject aObjectB - The gameobject that will be compared to aObjectB
    Creator: Alvaro Chavez Mixco
    Creation Date:  Tuesday, March 20th, 2017                   
    */
    public static int SortByInverseAlphanumerically(GameObject aObjectA, GameObject aObjectB)
    {
        return SortByAlphanumerically(aObjectB, aObjectA);
    }
}
#endif