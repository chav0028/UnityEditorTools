//If it running in the unity editor
#if UNITY_EDITOR
using UnityEditorInternal;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Tool used to render the content being seen to a cubemap (legaccy 6 textures cubemap, or render
/// texture cubemap)
/// </summary>
/// <Creator>Alvaro Chavez Mixco</Creator>
/// <CreationDate>Tuesday, October 15th, 2019</CreationDate>
public class CRenderCubemapTool : EditorWindow
{
    //The minimum distance that there must be between the rendering camera near and far clipping planes
    private const float M_CAMERA_MIN_DISTANCE_NEAR_FAR_CLIPPING_PLANES = float.Epsilon;
    private const float M_MIN_VALUE_NEAR_CLIPPING_PLANE = 0.1f;

    private const string M_OBJECT_FIELD_RENDERING_POSITION = "Render from Position";
    private const string M_OBJECT_FIELD_RENDER_TEXTURE_CUBEMAP = "Render Texture Cubemap";
    private const string M_OBJECT_FIELD_LEGACY_CUBEMAP = "Legacy Cubemap";

    //Advanced camera settings
    private const string M_FOLDOUT_CAMERA_ADVANCED_SETTINGS = "Camera Advanced Settings";
    private const string M_MASK_FIELD_CAMERA_CULLING_MASK = "Culling Mask";
    private const string M_ENUM_FIELD_CAMERA_CLEAR_FLAGS = "Clear Flags";
    private const string M_COLOR_FIELD_CAMERA_BACKGROUND_COLOR = "Background Color";
    private const string M_TOGGLE_CAMERA_ALLOW_HDR = "Allow HDR";
    private const string M_FLOAT_CAMERA_FIELD_NEAR_CLIP = "Distance Near Clipping";
    private const string M_FLOAT_CAMERA_FIELD_FAR_CLIP = "Distance Far Clipping";

    private const string M_TOGGLE_USE_LEGACY_CUBEMAP = "Use Legacy Cubemap";

    private const string M_BUTTON_RENDER_CUBEMAP = "Render Cubemap";

    //The position from where the cubemap will be rendered
    private Transform m_renderingPosition;

    private Cubemap m_legacyCubemap;
    private RenderTexture m_renderTextureCubemap;

    private bool m_useLegacyCubemap = false;

    private bool m_displayCameraAdvancedSettings = false;

    //Camera settings
    private LayerMask m_cameraCullingMask = ~0;//Default layer mask to everything    
    private CameraClearFlags m_cameraClearFlags = CameraClearFlags.Skybox;
    private Color m_cameraBackgroundColor = new Color(0.192f, 0.302f, 0.475f, 0.0f);//Default background color for Unity Camera
    private float m_cameraNearClippingPlane = 0.3f;
    private float m_cameraFarClippingPlane = 1000.0f;
    private bool m_cameraAllowHDR = true;

    /// <summary>
    /// Show the tab on the editor
    /// </summary>
    /// <Creator>Alvaro Chavez Mixco</Creator>
    /// <CreationDate>Tuesday, October 15th, 2019</CreationDate>
    [MenuItem("Tools/Render Cubemap")]
    private static void Init()
    {
        //Create the editor window
        CRenderCubemapTool window = (CRenderCubemapTool)EditorWindow.GetWindow(typeof(CRenderCubemapTool));

        //Show the editor window option in the Unity toolbar
        window.Show();
    }

    /// <summary>
    /// Show all the different options the user has for renaming the object/s, and
    /// handles the interaction/functionality of the UI shown
    /// </summary>
    /// <Creator>Alvaro Chavez Mixco</Creator>
    /// <CreationDate>Sunday, January 29, 2017</CreationDate>
    private void OnGUI()
    {
        //Display the field object where the user can specify from which position (transform) the
        //cubemap will be rendered from
        m_renderingPosition = (Transform)EditorGUILayout.ObjectField(M_OBJECT_FIELD_RENDERING_POSITION,
            m_renderingPosition, typeof(Transform), true);

        //Display toggle for uising legacy cubemap
        m_useLegacyCubemap = EditorGUILayout.Toggle(M_TOGGLE_USE_LEGACY_CUBEMAP, m_useLegacyCubemap);

        //If the user doesn't want to use the legacy cubemap, use a render texture
        if (m_useLegacyCubemap == false)
        {
            //Display field for the render texture where the rendered texture will be saved
            m_renderTextureCubemap = (RenderTexture)EditorGUILayout.ObjectField(
                M_OBJECT_FIELD_RENDER_TEXTURE_CUBEMAP, m_renderTextureCubemap, typeof(RenderTexture), false);
        }
        //If the legacy cubemap will be used
        else
        {
            //Display field for the legacy cubemap where the rendered texture will be saved
            m_legacyCubemap = (Cubemap)EditorGUILayout.ObjectField(
                M_OBJECT_FIELD_LEGACY_CUBEMAP, m_legacyCubemap, typeof(Cubemap), false);
        }

        //Display a foldout for the advanced camera settings
        m_displayCameraAdvancedSettings =
            EditorGUILayout.Foldout(m_displayCameraAdvancedSettings, M_FOLDOUT_CAMERA_ADVANCED_SETTINGS);

        //If the camera advanced settings foldout is open
        if (m_displayCameraAdvancedSettings == true)
        {
            //Show the extra settings
            ShowAdvancedCameraSettings();
        }

        //If the render texture cubemap is being used
        if (m_useLegacyCubemap == false)
        {
            //If the legacy cubemap is valid, and there is a rendering position defined
            if (m_renderTextureCubemap != null && m_renderingPosition != null)
            {
                if (m_renderTextureCubemap.dimension == UnityEngine.Rendering.TextureDimension.Cube)
                {

                    //If the render button is pressed
                    if (GUILayout.Button(M_BUTTON_RENDER_CUBEMAP) == true)
                    {
                        //Render into the render texture
                        RenderRenderTextureCubemap(ref m_renderTextureCubemap, m_renderingPosition,
                            m_cameraClearFlags, m_cameraBackgroundColor, m_cameraCullingMask,
                            m_cameraNearClippingPlane, m_cameraFarClippingPlane, m_cameraAllowHDR);
                    }
                }
            }
        }
        //If the legacy cubemap is being used
        else
        {
            //If the legacy cubemap is valid, and there is a rendering position defined
            if (m_legacyCubemap != null && m_renderingPosition != null)
            {
                //If the render button is pressed
                if (GUILayout.Button(M_BUTTON_RENDER_CUBEMAP) == true)
                {
                    //Render into the legacy cubemap
                    RenderLegacyCubemap(ref m_legacyCubemap, m_renderingPosition,
                        m_cameraClearFlags,m_cameraBackgroundColor,m_cameraCullingMask,
                        m_cameraNearClippingPlane,m_cameraFarClippingPlane,m_cameraAllowHDR);
                }
            }
        }
    }

    private void ShowAdvancedCameraSettings()
    {
        //Display an enum field for the camera clear flags
        m_cameraClearFlags =
            (CameraClearFlags)EditorGUILayout.EnumPopup(M_ENUM_FIELD_CAMERA_CLEAR_FLAGS, m_cameraClearFlags);

        //If the camera clear flags are set to skybox or solid color
        if (m_cameraClearFlags == CameraClearFlags.Skybox || m_cameraClearFlags == CameraClearFlags.SolidColor)
        {
            //Display the background color field
            m_cameraBackgroundColor =
                EditorGUILayout.ColorField(M_COLOR_FIELD_CAMERA_BACKGROUND_COLOR, m_cameraBackgroundColor);
        }

        //Show layer mask field
        //From http://answers.unity3d.com/questions/42996/how-to-create-layermask-field-in-a-custom-editorwi.html
        LayerMask tempMask = EditorGUILayout.MaskField(M_MASK_FIELD_CAMERA_CULLING_MASK,
            InternalEditorUtility.LayerMaskToConcatenatedLayersMask(m_cameraCullingMask), InternalEditorUtility.layers);

        //Convert value set layer value set by user to mask
        m_cameraCullingMask = InternalEditorUtility.ConcatenatedLayersMaskToLayerMask(tempMask);

        //Display near clipping plane field
        m_cameraNearClippingPlane = EditorGUILayout.FloatField(M_FLOAT_CAMERA_FIELD_NEAR_CLIP,
            m_cameraNearClippingPlane);

        //Display far clipping plane field
        m_cameraFarClippingPlane = EditorGUILayout.FloatField(M_FLOAT_CAMERA_FIELD_FAR_CLIP,
            m_cameraFarClippingPlane);

        //Ensure the near clip plane is not negative, or too small of a value
        m_cameraNearClippingPlane = Mathf.Max(M_MIN_VALUE_NEAR_CLIPPING_PLANE,
            m_cameraNearClippingPlane);

        //If the far clipping distance is smaller than the near clip distance
        if (m_cameraFarClippingPlane < m_cameraNearClippingPlane)
        {
            //Ensure the far clipping plane disntace is always at least
            //slightly bigger than the near clipping plane distance
            m_cameraFarClippingPlane = m_cameraNearClippingPlane + M_MIN_VALUE_NEAR_CLIPPING_PLANE;
        }

        //Display the allow HDR field
        m_cameraAllowHDR = EditorGUILayout.Toggle(M_TOGGLE_CAMERA_ALLOW_HDR, m_cameraAllowHDR);
    }

    private void SetRenderingCamera(ref Camera aCameraToSet, CameraClearFlags aCameraClearFlags,
        Color aCameraBackgroundColor, LayerMask aCameraCullingMask, float aDistanceNearClippingPlane,
        float aDistanceFarClippingPlane, bool aCameraAllowHDR)
    {
        //Set the camera clear flags, background color , and culling mask
        aCameraToSet.clearFlags = aCameraClearFlags;
        aCameraToSet.backgroundColor = aCameraBackgroundColor;
        aCameraToSet.cullingMask = aCameraCullingMask;

        //Set the camera near and far clip planes
        aCameraToSet.nearClipPlane = aDistanceNearClippingPlane;
        aCameraToSet.farClipPlane = aDistanceFarClippingPlane;

        //Set the use of HDR in the camera
        aCameraToSet.allowHDR = aCameraAllowHDR;
    }

    /*
    Description: Create a camera at the specified location, configure its setttings,
                 and render a cubemap into the specified cubemap asset
    Creator: Alvaro Chavez Mixco
    Creation Date: Tuesday, October 24th, 2017
    */
    private void RenderLegacyCubemap(ref Cubemap aCubemap, Transform aRenderingPosition, 
        CameraClearFlags aCameraClearFlags, Color aCameraBackgroundColor,
        LayerMask aCameraCullingMask, float aDistanceNearClippingPlane,
        float aDistanceFarClippingPlane, bool aCameraAllowHDR)
    {
        //Create temporary camera for rendering
        //Create an object to place the temporary camera on
        GameObject cubemapObject = new GameObject("LegacyCubemapCamera");

        //Create a camera to take the render from
        Camera cubemapCameraRenderer = cubemapObject.AddComponent<Camera>();

        SetRenderingCamera(ref cubemapCameraRenderer, aCameraClearFlags, aCameraBackgroundColor,
            aCameraCullingMask, aDistanceNearClippingPlane, aDistanceFarClippingPlane, aCameraAllowHDR);

        //Move the cube map camera object to the render position
        cubemapCameraRenderer.transform.position = aRenderingPosition.position;
        cubemapCameraRenderer.transform.rotation = Quaternion.identity;

        //Takes 6 pictures for the cubemap and save it to the cubemap texture
        cubemapObject.GetComponent<Camera>().RenderToCubemap(aCubemap);

        //Destroy the camera object
        DestroyImmediate(cubemapObject);
    }

    private void RenderRenderTextureCubemap(ref RenderTexture aCubemap, Transform aRenderingPosition,
    CameraClearFlags aCameraClearFlags, Color aCameraBackgroundColor,
    LayerMask aCameraCullingMask, float aDistanceNearClippingPlane,
    float aDistanceFarClippingPlane, bool aCameraAllowHDR)
    {
        //Verifies that the render texture is a cubemap
        if (m_renderTextureCubemap.dimension == UnityEngine.Rendering.TextureDimension.Cube)
        {
            //Create temporary camera for rendering
            //Create an object to place the temporary camera on
            GameObject cubemapObject = new GameObject("RenderTextureCubemapCamera");

            //Create a camera to take the render from
            Camera cubemapCameraRenderer = cubemapObject.AddComponent<Camera>();

            SetRenderingCamera(ref cubemapCameraRenderer, aCameraClearFlags, aCameraBackgroundColor,
                aCameraCullingMask, aDistanceNearClippingPlane, aDistanceFarClippingPlane, aCameraAllowHDR);

            //Move the cube map camera object to the render position
            cubemapCameraRenderer.transform.position = aRenderingPosition.position;
            cubemapCameraRenderer.transform.rotation = Quaternion.identity;

            //Takes 6 pictures for the cubemap and save it to the cubemap texture
            cubemapObject.GetComponent<Camera>().RenderToCubemap(aCubemap);

            //Destroy the camera object
            DestroyImmediate(cubemapObject);
        }
    }
}
#endif