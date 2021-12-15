# UnityEditorTools
Different editor tools to use with Unity.

The tools included in this project are:
- Array: Inspired by the Array tool of 3ds Max, this custom tool was made to either place/create objects spread out organized in a grid structure. In this grid the objects are evenly spaced, and can also have variations in their rotation and scale.
- Remove Colliders: This simple tool removes all colliders of an object, and possibly its children, regardless of the specific collider type (box, sphere, etc.).
- Rename: Based on 3ds Max Rename Objects Tools, this tool allows you to rename all objects you have selected. The rename can be to add prefix, a suffix, or to number them. Besides renaming them, this tool can also be used to sort the objects alphanumerically by name in the scene hierarchy.
- Render Cubemap: The tool creates a camera, which can be customized, at the specified position and takes a screenshot of its surrounding, saving this screenshot to a render texture cube map. This allows creating custom cube maps of different positions of the scene view.
- Transform Selection: The tools allows applying a change in position, rotation or scale to all objects selected in the scene. This can be used to change all object selected by a specific amount, like for example moving all objects 5 units to the right (which can not be currently done in Unity).

Note: Currently only the Rename and the Transform Selection tools support undo/redo operations.