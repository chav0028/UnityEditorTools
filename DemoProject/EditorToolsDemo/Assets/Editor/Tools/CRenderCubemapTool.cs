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
    #region Constants
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
    private const string M_TOGGLE_CAMERA_IS_ORTOGRAPHIC = "Is Ortographic";
    private const string M_FLOAT_FIELD_CAMERA_ORTOGRAPHIC_SIZE = "Ortographic Size";
    private const string M_FLOAT_FIELD_CAMERA_PERSPECTIVE_FOV = "Vertical FOV";
    private const string M_FLOAT_FIELD_CAMERA_NEAR_CLIP = "Distance Near Clipping";
    private const string M_FLOAT_FIELD_CAMERA_FAR_CLIP = "Distance Far Clipping";

    private const string M_TOGGLE_USE_LEGACY_CUBEMAP = "Use Legacy Cubemap";

    private const string M_BUTTON_RENDER_CUBEMAP = "Render Cubemap";
    #endregion

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
    private bool m_cameraIsOrtographic = false;
    private float m_cameraOrtographicSize = 5.0f;
    private float m_cameraVerticalFOV = 60.0f;

    #region UI
    /// <summary>
    /// Show the tab on the editor
    /// </summary>
    /// <Creator>Alvaro Chavez Mixco</Creator>
    /// <CreationDate>Tuesday, October 15th, 2019</CreationDate>
    [MenuItem("Tools/Render Cubemap")]
    private static void Init()
    {
        //Create a new editor window, or get an existing one
        CRenderCubemapTool window = (CRenderCubemapTool)EditorWindow.GetWindow(typeof(CRenderCubemapTool));

        //Show the editor window option in the Unity toolbar
        window.Show();
    }

    /// <summary>
    /// Show all the different options the user has for renaming the object/s, and
    /// handles the interaction/functionality of the UI shown
    /// </summary>
    /// <Creator>Alvaro Chavez Mixco</Creator>
    /// <CreationDate>Tuesday, October 15th, 2019</CreationDate>
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
            DisplayAdvancedCameraSettings();
        }

        //If the render texture cubemap is being used
        if (m_useLegacyCubemap == false)
        {
            //If the legacy cubemap is valid, and there is a rendering position defined
            if (m_renderTextureCubemap != null && m_renderingPosition != null)
            {
                //If the render button is pressed
                if (GUILayout.Button(M_BUTTON_RENDER_CUBEMAP) == true)
                {
                    //Creates and setups the camera that will be used to render the cubemap
                    Camera renderCamera = CreateRenderCamera(m_cameraClearFlags, m_cameraBackgroundColor,
                        m_cameraCullingMask, m_cameraIsOrtographic, m_cameraOrtographicSize, m_cameraVerticalFOV,
                        m_cameraNearClippingPlane, m_cameraFarClippingPlane, m_cameraAllowHDR);

                    //Render into the render texture
                    RenderCubemap(m_renderTextureCubemap, m_renderingPosition, renderCamera);

                    //Destroy the camera object
                    DestroyImmediate(renderCamera.gameObject);
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
                    //Creates and setups the camera that will be used to render the cubemap
                    Camera renderCamera = CreateRenderCamera(m_cameraClearFlags, m_cameraBackgroundColor,
                        m_cameraCullingMask, m_cameraIsOrtographic, m_cameraOrtographicSize, m_cameraVerticalFOV,
                        m_cameraNearClippingPlane, m_cameraFarClippingPlane, m_cameraAllowHDR);

                    //Render into the legacy cubemap
                    RenderCubemap(m_legacyCubemap, m_renderingPosition, renderCamera);

                    //Destroy the camera object
                    DestroyImmediate(renderCamera.gameObject);
                }

            }
        }
    }

    /// <summary>
    /// Show the UI for configuring the advacned camera (used for rendering the cubemap) settings
    /// </summary>
    /// <Creator>Alvaro Chavez Mixco</Creator>
    /// <CreationDate>Tuesday, October 15th, 2019</CreationDate>
    private void DisplayAdvancedCameraSettings()
    {
        //Display an enum field for the camera clear flags
        m_cameraClearFlags =
            (CameraClearFlags)EditorGUILayout.EnumPopup(M_ENUM_FIELD_CAMERA_CLEAR_FLAGS, m_cameraClearFlags);

        //If the camera clear flags are set to skybox or solid color
        if (m_cameraClearFlags == CameraClearFlags.SolidColor ||
            (m_cameraClearFlags == CameraClearFlags.Skybox && RenderSettings.skybox == null))
        {
            //Display the background color field
            //Only used if clearFlags are set to CameraClearFlags.SolidColor
            //(or CameraClearFlags.Skybox but the skybox is not set up).
            m_cameraBackgroundColor =
                EditorGUILayout.ColorField(M_COLOR_FIELD_CAMERA_BACKGROUND_COLOR, m_cameraBackgroundColor);
        }

        //Show layer mask field
        //From http://answers.unity3d.com/questions/42996/how-to-create-layermask-field-in-a-custom-editorwi.html
        LayerMask tempMask = EditorGUILayout.MaskField(M_MASK_FIELD_CAMERA_CULLING_MASK,
            InternalEditorUtility.LayerMaskToConcatenatedLayersMask(m_cameraCullingMask), InternalEditorUtility.layers);

        //Convert value set layer value set by user to mask
        m_cameraCullingMask = InternalEditorUtility.ConcatenatedLayersMaskToLayerMask(tempMask);

        //Toggle whether the camera is ortographic (true) or perspective (false)
        m_cameraIsOrtographic = EditorGUILayout.Toggle(M_TOGGLE_CAMERA_IS_ORTOGRAPHIC, m_cameraIsOrtographic);

        //If camera is perspective
        if (m_cameraIsOrtographic == false)
        {
            //Show vertical field of view field
            m_cameraVerticalFOV = EditorGUILayout.FloatField(M_FLOAT_FIELD_CAMERA_PERSPECTIVE_FOV, m_cameraVerticalFOV);
        }
        //If camera is ortographic
        else
        {
            //Show ortographic size field
            m_cameraOrtographicSize = EditorGUILayout.FloatField(M_FLOAT_FIELD_CAMERA_ORTOGRAPHIC_SIZE, m_cameraOrtographicSize);
        }

        //Display near clipping plane field
        m_cameraNearClippingPlane = EditorGUILayout.FloatField(M_FLOAT_FIELD_CAMERA_NEAR_CLIP,
            m_cameraNearClippingPlane);

        //Display far clipping plane field
        m_cameraFarClippingPlane = EditorGUILayout.FloatField(M_FLOAT_FIELD_CAMERA_FAR_CLIP,
            m_cameraFarClippingPlane);

        //Ensure the near clip plane is not negative, or too small of a value
        m_cameraNearClippingPlane = Mathf.Max(M_MIN_VALUE_NEAR_CLIPPING_PLANE,
            m_cameraNearClippingPlane);

        //If the far clipping distance is smaller than the near clip distance
        if (m_cameraFarClippingPlane < m_cameraNearClippingPlane)
        {
            //Ensure the far clipping plane disntace is always at least
            //slightly bigger than the near clipping plane distance
            m_cameraFarClippingPlane = m_cameraNearClippingPlane + M_CAMERA_MIN_DISTANCE_NEAR_FAR_CLIPPING_PLANES;
        }

        //Display the allow HDR field
        m_cameraAllowHDR = EditorGUILayout.Toggle(M_TOGGLE_CAMERA_ALLOW_HDR, m_cameraAllowHDR);
    }
    #endregion

    /// <summary>
    /// Creates and setups the camera that will be used for rendering the cubemap
    /// </summary>
    /// <Creator>Alvaro Chavez Mixco</Creator>
    /// <CreationDate>Tuesday, October 15th, 2019</CreationDate>
    /// <param name="aCameraClearFlags" type="CameraClearFlags">How the camera clears the background</param>
    /// <param name="aCameraBackgroundColor" type="Color">The color with which the screen will be cleared.
    /// Only used if clearFlags are set to CameraClearFlags.SolidColor (or CameraClearFlags.Skybox
    /// but the skybox is not set up).</param>
    /// <param name="aCameraCullingMask" type="LayerMask">This is used to render parts of the Scene selectively</param>
    /// <param name="aIsOrtographic">Indicates whether the camera is ortographic (true) or perspective (false)</param>
    /// <param name="aOrtographicSize">The ortographic size of the camera (value will be set, 
    /// but ignored, if the camera is perspective)</param>
    /// <param name="aVerticalFOV">The vertical field of view (FOV), in degree, of the camera (value will be set, 
    /// but ignored, if the camera is ortographic)</param>
    /// <param name="aDistanceNearClippingPlane" type="float">The near clipping plane distance</param>
    /// <param name="aDistanceFarClippingPlane" type="float">The far clipping plane distance</param>
    /// <param name="aCameraAllowHDR" type="bool">Indicates if High dynamic range (HDR) rendering
    /// should be enabled</param>
    /// <returns>Returns the camera component that was created with the specified settings</returns>
    private Camera CreateRenderCamera(CameraClearFlags aCameraClearFlags,
        Color aCameraBackgroundColor, LayerMask aCameraCullingMask,
        bool aIsOrtographic, float aOrtographicSize, float aVerticalFOV,
        float aDistanceNearClippingPlane, float aDistanceFarClippingPlane, bool aCameraAllowHDR)
    {
        //Create temporary camera for rendering
        //Create an object to place the temporary camera on
        GameObject cameraGameObject = new GameObject("RenderCubemapCamera");

        //Create a camera to take the render from
        Camera renderCamera = cameraGameObject.AddComponent<Camera>();

        //Set the camera clear flags, background color , and culling mask
        renderCamera.clearFlags = aCameraClearFlags;
        renderCamera.backgroundColor = aCameraBackgroundColor;
        renderCamera.cullingMask = aCameraCullingMask;

        //Set the ortographic/perspective properties
        renderCamera.orthographic = aIsOrtographic;
        renderCamera.orthographicSize = aOrtographicSize;
        renderCamera.fieldOfView = aVerticalFOV;

        //Set the camera near and far clip planes
        renderCamera.nearClipPlane = aDistanceNearClippingPlane;
        renderCamera.farClipPlane = aDistanceFarClippingPlane;

        //Set the use of HDR in the camera
        renderCamera.allowHDR = aCameraAllowHDR;

        return renderCamera;
    }

    /// <param name="aCubemapAsset"></param>
    /// <summary>
    /// Renders the cubemap, using the camera properties being passed, to
    /// <paramref name="aCubemap"/> a Render Texture Cube
    /// </summary>
    /// <Creator>Alvaro Chavez Mixco</Creator>
    /// <CreationDate>Tuesday, October 15th, 2019</CreationDate>
    /// <typeparam name="T">The type of texture asset where the cubemap will be rendered, this must be
    /// either a render texture (with dimension set to cube) or a legacy cubemap (set to redable)</typeparam>
    /// <param name="aCubemapAsset">The asset where the cubemap will be rendered to, this must be
    /// either a render texture (with dimension set to cube) or a legacy cubemap (set to redable)</param>
    /// <param name="aRenderingPosition">The position from where the cubemap will be rendered from</param>
    /// <param name="aRenderCamera">The camera that will be used for rendering the cubemap</param>
    private void RenderCubemap<T>(T aCubemapAsset, Transform aRenderingPosition,
        Camera aRenderCamera)
        where T : Texture
    {
        //Move the render camera object to the render position
        aRenderCamera.transform.position = aRenderingPosition.position;
        aRenderCamera.transform.rotation = Quaternion.identity;

        //Checks if the cubemap asset is a render texture
        RenderTexture renderTextureAsset = aCubemapAsset as RenderTexture;
        if (renderTextureAsset != null)
        {
            //Verifies that the render texture is a cubemap
            if (m_renderTextureCubemap.dimension == UnityEngine.Rendering.TextureDimension.Cube)
            {
                //Render into the legacy cubemap
                aRenderCamera.RenderToCubemap(renderTextureAsset);

                //Display success message
                Debug.Log("Rendered cubemap to render texture: " + aCubemapAsset.name);
            }
            //If the render texture is not a cubemap
            else
            {
                //Display error message
                Debug.LogError(aCubemapAsset.name + " - Render texture dimension is not set to Cube");
            }
        }

        //Checks if the cubemap asset is a legacy cubemap
        Cubemap legacyCubemapAsset = aCubemapAsset as Cubemap;
        if (legacyCubemapAsset != null)
        {
            //Verifies that the legacy cubemap is readable
            if (legacyCubemapAsset.isReadable == true)
            {
                //Render into the legacy cubemap
                aRenderCamera.RenderToCubemap(legacyCubemapAsset);

                //Display 
                Debug.Log("Rendered cubemap to legacy cubemap: " + aCubemapAsset.name);
            }
            //If the cumepa is not readable
            else
            {
                //Display error message
                Debug.LogError(aCubemapAsset.name + " - Legacy Cubemap is not readable");
            }
        }
    }
}
#endif