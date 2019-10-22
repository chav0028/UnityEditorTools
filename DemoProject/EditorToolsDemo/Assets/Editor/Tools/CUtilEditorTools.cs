//If the program is in the Unity editor
#if UNITY_EDITOR
using UnityEngine;

/// <summary>
/// A series of functions commonly used in the editor tools
/// </summary>
/// <Creator>Alvaro Chavez Mixco</Creator>
/// <CreationDate>Sunday, January 29th, 2017</CreationDate>
public class CUtilEditorTools
{
    /// <summary>
    /// Function to read a string and get the first (if any) number chunk from it (series
    /// of continous number characters)
    /// </summary>
    /// <Creator>Alvaro Chavez Mixco</Creator>
    /// <CreationDate>Wednesday, February 15th, 2017</CreationDate>
    /// <param name="aStringToRead" type="string">The string of the value that will be read</param>
    /// <param name="aStartingIndex" type="ref int">The index from which it will start reading the 
    /// string, this will be updated after the function executes</param>
    /// <param name="aStringToReadLength" type="int">The length (number of characters) of the string
    /// that will be read</param>
    /// <returns type="char[]">The continous chunk of number characters read from the string </returns>
    /// <remarks>For explanation of how it works check: https://www.dotnetperls.com/alphanumeric-sorting
    /// This function is used when sorting names alphanumerically, since it provides the continous chunk
    /// of number characters that can then be used for sorting.
    /// </remarks>
    private static char[] GetNumberCharsChunks(string aStringToRead, ref int aStartingIndex, int aStringToReadLength)
    {
        //Get the starting character to read
        char characterToRead = aStringToRead[aStartingIndex];

        // Create arrays of the values read
        char[] readValues = new char[aStringToReadLength];

        //Index of the current values read
        int indexReadValues = 0;

        //While the character read is number matches the intial character is number condition
        //So it will read the string by chunks, determining if the char read is a number or not
        do
        {
            //Read the current character value
            readValues[indexReadValues] = characterToRead;

            //Increase the index
            indexReadValues++;
            aStartingIndex++;

            //If the index is still valid
            if (aStartingIndex < aStringToReadLength)
            {
                //Read the next value
                characterToRead = aStringToRead[aStartingIndex];
            }
            //If the index is not valid
            else
            {
                //Quit the loop
                break;
            }
        } while (char.IsNumber(characterToRead) == char.IsNumber(readValues[0]));

        return readValues;
    }

    /// <summary>
    /// A helper function to get all the children gameobjects from a parent game object.
    /// </summary>
    /// <Creator>Alvaro Chavez Mixco</Creator>
    /// <CreationDate>Sunday, January 29th, 2017</CreationDate>
    /// <param name="aParentObject" type="GameObject">The parent game object from which the children 
    /// game objects will be obtained</param>
    /// <returns type="GameObject[]">All the children game objects found in the parent object</returns>
    /// <remarks>This function is not recursive, it only returns the first line/hierarcht of 
    /// children game objects</remarks>
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

    /// <summary>
    /// Divide all individual axis of a Vector3 by all the individual axis
    /// of another Vector3 (dividing x with x, y with y, and z with z)
    /// </summary>
    /// <Creator>Alvaro Chavez Mixco</Creator>
    /// <CreationDate>Sunday, January 29th, 2017</CreationDate>
    /// <param name="aDividend" type="Vector3">The Vector3 that will be divided</param>
    /// <param name="aDivisor" type="Vector3">The Vector3 that will divide</param>
    /// <returns type="Vector3">The vector3 that results from dividing each individual
    /// axis of the vector3</returns>
    public static Vector3 DivideVector3(Vector3 aDividend, Vector3 aDivisor)
    {
        //Get the value between each x,y and z element, according to the total value
        //and the total number of elements
        return new Vector3(
            aDividend.x / aDivisor.x,
            aDividend.y / aDivisor.y,
           aDividend.z / aDivisor.z);
    }

    /// <summary>
    /// Helper function to get the 3D (X,Y, Z) index of an object in a array according to its 1D intex
    /// and the array size.
    /// </summary>
    /// <Creator>Alvaro Chavez Mixco</Creator>
    /// <CreationDate>Sunday, January 29th, 2017</CreationDate>
    /// <param name="a1DIndex" type="int">The 1D array index that will be converted</param>
    /// <param name="aArrayWidth"type="int">The width, number of elements in X, of the array</param>
    /// <param name="aArrayHeight" type="int">The width, number of elements in Y, of the array</param>
    /// <returns type="Vector3Int">The 3D array index that equals
    /// the 1D array index <paramref name="a1DIndex"/></returns>
    public static Vector3Int Convert1DIndexTo3DArrayIndex(int a1DIndex, int aArrayWidth, int aArrayHeight)
    {
        Vector3Int index3D = Vector3Int.zero;

        //Get the Z index
        index3D.z = a1DIndex / (aArrayWidth * aArrayHeight);

        //Adjust the 1D index so that it only takes into account X and Y
        a1DIndex -= index3D.z * aArrayWidth * aArrayHeight;

        //Get the Y index
        index3D.y = a1DIndex / aArrayWidth;

        //Get the X index
        index3D.x = a1DIndex % aArrayWidth;

        return index3D;
    }

    /// <summary>
    /// Convert a 1D array of game objects into a 3D array of game objects.
    /// </summary>
    /// <Creator>Alvaro Chavez Mixco</Creator>
    /// <CreationDate>Sunday, January 29th, 2017</CreationDate>
    /// <param name="aArrayGameObject" type="GameObject[]">The 1D array of game objects to convert to a 3D array</param>
    /// <param name="aArrayXYZDimensions" type="Vector3">The dimensions of the 3D array that will be created</param>
    /// <returns type="GameObject[,,]">The new 3D array of game objects</returns>
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

    /// <summary>
    /// Compares 2 strings alphanumerically, for sorting purposes
    /// </summary>
    /// <Creator>Alvaro Chavez Mixco</Creator>
    /// <CreationDate>Tuesday, February 14th, 2017</CreationDate>
    /// <param name="aTextA" type="string">The text to compare alphanumerically</param>
    /// <param name="aTextB" type="string">The other text to compare alphanumerically</param>
    /// <returns type="int">The int result from the comparison, this can be:
    /// Less than 0   : A is less than B
    /// Equals 0      : A equals B
    /// Greater than 0: A is greater than B
    /// </returns>
    /// <remarks>Function intented to be assigned to a delegate.
    /// For explanation on how it works check:
    /// https://www.dotnetperls.com/alphanumeric-sorting
    /// https://msdn.microsoft.com/en-us/library/tfakywbh(v=vs.110).aspx
    /// </remarks>
    /// <seealso cref="SortByInverseAlphanumerically(string, string)"/>
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

    /// <summary>
    /// Compares 2 strings inverse alhpanumerically. This is done by calling the normal 
    /// SortByAlphanumerically function, but switching the order of the parameters.
    /// </summary>
    /// <Creator>Alvaro Chavez Mixco</Creator>
    /// <CreationDate>Tuesday, February 14th, 2017</CreationDate>
    /// <param name="aTextA" type="string">The text to compare inverse alphanumerically</param>
    /// <param name="aTextB" type="string">The other text to compare inverse alphanumerically</param>
    /// <returns type="int">The int result from the comparison, this can be:
    /// Less than 0   : A is less than B
    /// Equals 0      : A equals B
    /// Greater than 0: A is greater than B
    /// </returns>
    /// <remarks>Function intented to be assigned to a delegate.
    /// </remarks>
    /// <seealso cref="SortByAlphanumerically(string, string)"/>
    public static int SortByInverseAlphanumerically(string aTextA, string aTextB)
    {
        //Call the normal SortByAlphanumerically function, but switching the order of the parameters.
        return SortByAlphanumerically(aTextB, aTextA);
    }

    /// <summary>
    /// Compares the name of 2 game objects alphanumerically, for sorting purposes
    /// </summary>
    /// <Creator>Alvaro Chavez Mixco</Creator>
    /// <CreationDate>Tuesday, February 14th, 2017</CreationDate>
    /// <param name="aObjectA" type="GameObject">The game object which name will be compared alphanumerically</param>
    /// <param name="aObjectB" type="GameObject">The other game object which name will be compared alphanumerically</param>
    /// <returns type="int">The int result from the comparison, this can be:
    /// Less than 0   : A is less than B
    /// Equals 0      : A equals B
    /// Greater than 0: A is greater than B
    /// </returns>
    /// <remarks>Function intented to be assigned to a delegate.
    /// </remarks>
    /// <seealso cref="SortByAlphanumerically(string, string)"/>
    /// <seealso cref="SortByInverseAlphanumerically(GameObject, GameObject)"/>
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

    /// <summary>
    /// Compares the name of 2 game objects inverse alphanumerically, for sorting purposes.
    /// This is done by calling the normal SortByAlphanumerically function, but switching 
    /// the order of the parameters.
    /// </summary>
    /// <Creator>Alvaro Chavez Mixco</Creator>
    /// <CreationDate>Tuesday, February 14th, 2017</CreationDate>
    /// <param name="aObjectA" type="GameObject">The game object which name will be compared inverse alphanumerically</param>
    /// <param name="aObjectB" type="GameObject">The other game object which name will be compared inverse alphanumerically</param>
    /// <returns type="int">The int result from the comparison, this can be:
    /// Less than 0   : A is less than B
    /// Equals 0      : A equals B
    /// Greater than 0: A is greater than B
    /// </returns>
    /// <remarks>Function intented to be assigned to a delegate.
    /// </remarks>
    /// <seealso cref="SortByInverseAlphanumerically(string, string)"/>
    /// <seealso cref="SortByAlphanumerically(GameObject, GameObject)"/>
    public static int SortByInverseAlphanumerically(GameObject aObjectA, GameObject aObjectB)
    {
        return SortByAlphanumerically(aObjectB, aObjectA);
    }
}
#endif