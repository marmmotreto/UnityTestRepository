// Copyright (C) 2014 Stephan Bouchard - All Rights Reserved
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

#if UNITY_4_6 || UNITY_5_0

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine.UI;
using UnityEngine.EventSystems;


namespace TMPro
{  

    public partial class TextMeshProUGUI
    {
                            
        [SerializeField]
        private string m_text;

        [SerializeField]
        private TextMeshProFont m_fontAsset;

        [SerializeField]
        private Material m_fontMaterial;

        [SerializeField]
        private Material m_sharedMaterial;

        [SerializeField]
        private FontStyles m_fontStyle = FontStyles.Normal;
        private FontStyles m_style = FontStyles.Normal;

        private bool m_isOverlay = false;

        [SerializeField]
        private Color32 m_fontColor32 = Color.white;

        [SerializeField]
        private Color m_fontColor = Color.white;

		[SerializeField]
		private bool m_enableVertexGradient;

		[SerializeField]
		private VertexGradient m_fontColorGradient = new VertexGradient(Color.white);

        [SerializeField]
        private Color32 m_faceColor = Color.white;

        [SerializeField]
        private Color32 m_outlineColor = Color.black;

        private float m_outlineWidth = 0.0f;

        [SerializeField]
        private float m_fontSize = 36; // Font Size
        [SerializeField]
        private float m_fontSizeMin = 0; // Text Auto Sizing Min Font Size.
        [SerializeField]
        private float m_fontSizeMax = 0; // Text Auto Sizing Max Font Size.
        [SerializeField]
        private float m_fontSizeBase = 36;
        //[SerializeField]
        //private float m_charSpacingMax = 0; // Text Auto Sizing Max Character spacing reduction.
        [SerializeField]
        private float m_lineSpacingMax = 0; // Text Auto Sizing Max Line spacing reduction.


        private float m_currentFontSize; // Temporary Font Size affected by tags

        [SerializeField]
        private float m_characterSpacing = 0;
		private float m_cSpacing = 0;

        //[SerializeField]
        //private float m_lineLength = 72;

        //[SerializeField]
        //private Vector4 m_textRectangle;

        [SerializeField]
        private float m_lineSpacing = 0;
        private float m_lineSpacingDelta = 0;

        [SerializeField]
        private float m_paragraphSpacing = 0;
        
        //[SerializeField]
        //private AnchorPositions m_anchor = AnchorPositions.TopLeft;

        [SerializeField]
        private TextAlignmentOptions m_textAlignment = TextAlignmentOptions.TopLeft;
        private TextAlignmentOptions m_lineJustification;

        [SerializeField]
        private bool m_enableKerning = false;

        private bool m_anchorDampening = false;
        private float m_baseDampeningWidth;

        [SerializeField]
        private bool m_overrideHtmlColors = false;

        [SerializeField]
        private bool m_enableExtraPadding = false;
        [SerializeField]
        private bool checkPaddingRequired;

        [SerializeField]
        private bool m_enableWordWrapping = false;
        private bool m_isCharacterWrappingEnabled = false;

        [SerializeField]
        private TextOverflowModes m_overflowMode = TextOverflowModes.Overflow;

        [SerializeField]
        private float m_wordWrappingRatios = 0.4f; // Controls word wrapping ratios between word or characters.

        [SerializeField]
        private TextureMappingOptions m_horizontalMapping = TextureMappingOptions.Character;

        [SerializeField]
        private TextureMappingOptions m_verticalMapping = TextureMappingOptions.Character;

        [SerializeField]
        private Vector2 m_uvOffset = Vector2.zero; // Used to offset UV on Texturing

        [SerializeField]
        private float m_uvLineOffset = 0.3f; // Used for UV line offset per line


        //[SerializeField]
        //private bool isAffectingWordWrapping = false; // Used internally to control which properties affect word wrapping.

        [SerializeField]
        private bool isInputParsingRequired = false; // Used to determine if the input text needs to be reparsed.
        //private bool m_inputHasBeenReparsed = false;

        [SerializeField]
        private bool havePropertiesChanged;  // Used to track when properties of the text object have changed.

        [SerializeField]
        private bool hasFontAssetChanged = false; // Used to track when font properties have changed.

        [SerializeField]
        private bool m_isRichText = true; // Used to enable or disable Rich Text.


        private enum TextInputSources { Text = 0, SetText = 1, SetCharArray = 2 };
        [SerializeField]
        private TextInputSources m_inputSource;
        private string old_text; // Used by SetText to determine if the text has changed.
        private float old_arg0, old_arg1, old_arg2; // Used by SetText to determine if the args have changed.

        private int m_fontIndex;

        private float m_fontScale; // Scaling of the font based on Atlas true Font Size and Rendered Font Size.  
        private bool m_isRecalculateScaleRequired = false;

        private Vector3 m_lossyScale; // Used for Tracking lossy scale changes in the transform;
        private float m_xAdvance; // Tracks x advancement from character to character.
        //private float m_totalxAdvance;
        //private bool m_isCheckingTextLength = false;
        //private float m_textLength;
        //private int[] m_text_buffer = new int[8];

        //private float max_LineWrapLength = 999;

        private Vector3 m_anchorOffset; // The amount of offset to be applied to the vertices. 


        private TextInfo m_textInfo; // Class which holds information about the Text object such as characters, lines, mesh data as well as metrics.
        //private TMPro_CharacterInfo[] m_characters_Info; // Data structre that contains information about the text object and characters contained in it.
        //private TMPro_TextMetrics m_textMetrics;
     

        private char[] m_htmlTag = new char[16]; // Maximum length of rich text tag. This is pre-allocated to avoid GC.


        [SerializeField]
        private CanvasRenderer m_uiRenderer;

        //private Canvas m_canvas;
        private RectTransform m_rectTransform;
        //private Mesh m_mesh;

        private Color32 m_htmlColor = new Color(255, 255, 255, 128);     
        private float m_tabSpacing = 0;
        private float m_spacing = 0;
        private Vector2[] m_spacePositions = new Vector2[8];

        private float m_baselineOffset; // Used for superscript and subscript.
        private float m_padding = 0; // Holds the amount of padding required to display the mesh correctly as a result of dilation, outline thickness, softness and similar material properties.
        private Vector4 m_alignmentPadding; // Holds the amount of padding required to account for Outline Width and Dilation with regards to text alignment.
        private bool m_isUsingBold = false; // Used to ensure GetPadding & Ratios take into consideration bold characters.

        private Vector2 k_InfinityVector = new Vector2(Mathf.Infinity, Mathf.Infinity);

        private bool m_isFirstAllocation; // Flag to determine if this is the first allocation of the buffers.
        private int m_max_characters = 8; // Determines the initial allocation and size of the character array / buffer.
        private int m_max_numberOfLines = 4; // Determines the initial allocation and maximum number of lines of text. 

        private int[] m_char_buffer; // This array holds the characters to be processed by GenerateMesh();
        private char[] m_input_CharArray = new char[256]; // This array hold the characters from the SetText();
        private int m_charArray_Length = 0;
        private List<char> m_VisibleCharacters = new List<char>();

        //private Mesh_Extents[] m_lineExtents; // Struct that holds information about each line which is used for UV Mapping.

        //private IFormatProvider NumberFormat = System.Globalization.NumberFormatInfo.CurrentInfo; // Speeds up accessing this interface.
        private readonly float[] k_Power = { 5e-1f, 5e-2f, 5e-3f, 5e-4f, 5e-5f, 5e-6f, 5e-7f, 5e-8f, 5e-9f, 5e-10f }; // Used by FormatText to enable rounding and avoid using Mathf.Pow.

        private GlyphInfo m_cached_GlyphInfo; // Glyph / Character information is cached into this variable which is faster than having to fetch from the Dictionary multiple times.
        private GlyphInfo m_cached_Underline_GlyphInfo; // Same as above but for the underline character which is used for Underline.

        private WordWrapState m_SavedWordWrapState = new WordWrapState(); // Struct which holds various states / variables used in conjunction with word wrapping.
        private WordWrapState m_SavedLineState = new WordWrapState();
        //private LineWrapState m_SavedLineState = new LineWrapState();
        private int m_characterCount;
        private int m_visibleCharacterCount;
        private int m_lineNumber;
        private float m_maxAscender;
        private float m_maxDescender;
        private float m_maxFontScale;
        private float m_lineOffset;
        private Extents m_meshExtents;


        // Properties related to the Auto Layout System
        private bool m_isCalculateSizeRequired = false;
        //private bool m_isCalculatingLayout = false;
        private ILayoutController m_layoutController;

        // Mesh Declaration
        [SerializeField]
        private UIVertex[] m_uiVertices;
        //private Vector3[] m_vertices;
        //private Vector3[] m_normals;
        //private Vector4[] m_tangents;
        //private Vector2[] m_uvs;
        //private Vector2[] m_uv2s;
        //private Color32[] m_vertColors;
        //private int[] m_triangles;
        private Bounds m_bounds;

        //private Camera m_sceneCamera;
        //private Bounds m_default_bounds = new Bounds(Vector3.zero, new Vector3(1000, 1000, 0));
       

        [SerializeField]
        private bool m_ignoreCulling = true; // Not implemented yet.
        [SerializeField]
        private bool m_isOrthographic = false;

        [SerializeField]
        private bool m_isCullingEnabled = false;

        [SerializeField]
        private int m_sortingLayerID;
        [SerializeField]
        private int m_sortingOrder;


        //Special Cases
        private int m_maxVisibleCharacters = -1;
        private int m_maxVisibleLines = -1;
        private bool m_isTextTruncated;
        //private int m_safetyCount = 0;

        // Multi Material & Font Handling
        // Forced to use a class until 4.5 as Structs do not serialize. 
        //private class TriangleList
        //{
        //    public int[] triangleIndex;
        //}

        //private TriangleList[] m_triangleListArray = new TriangleList[16];
        [SerializeField]
        private TextMeshProFont[] m_fontAssetArray;
        //[SerializeField]
        private List<Material> m_sharedMaterials = new List<Material>(16);
        private int m_selectedFontAsset;

        // MASKING RELATED PROPERTIES       
        private bool m_isMaskingEnabled;
        private bool m_isStencilUpdateRequired;

        [SerializeField]
        private Material m_baseMaterial;
        private Material m_lastBaseMaterial;
        [SerializeField]
        private bool m_isNewBaseMaterial;
        private Material m_maskingMaterial; // Instance of the material used for masking.

        private bool m_isScrollRegionSet;       
        private Mask m_mask;
        /*
        [SerializeField]
        
        [SerializeField]
        private MaskingTypes m_mask;
        [SerializeField]
        private MaskingOffsetMode m_maskOffsetMode;
        */
        [SerializeField]
        private Vector4 m_maskOffset;
        /*
        [SerializeField]
        private Vector2 m_maskSoftness;
        [SerializeField]
        private Vector2 m_vertexOffset;
        */

        //
        private Matrix4x4 m_EnvMapMatrix = new Matrix4x4();


        // FLAGS
        //private bool DONT_RENDER_MESH = false;
        private TextRenderFlags m_renderMode = TextRenderFlags.Render;

        // ADVANCED LAYOUT COMPONENT ** Still work in progress
        private float m_maxXAdvance;
        //private TMPro_AdvancedLayout m_advancedLayoutComponent;
        //private bool m_isAdvanceLayoutComponentPresent;
        //private TMPro_MeshInfo m_meshInfo;


        // Text Container / RectTransform Component
        [SerializeField]
        private Vector4 m_margin = new Vector4(0, 0, 0, 0);
        private float m_marginWidth;
        private float m_marginHeight;
        private bool m_marginsHaveChanged;
        private bool IsRectTransformDriven = false;

        [SerializeField]
        private bool m_isNewTextObject;
        private TextContainer m_textContainer;
        private bool m_rectTransformDimensionsChanged;
        private Vector3[] m_rectCorners = new Vector3[4];

        [SerializeField]
        private bool m_enableAutoSizing;
        private float m_maxFontSize;
        private float m_minFontSize;

        private bool m_isAwake;
      

        // ** Still needs to be implemented **
        //private Camera managerCamera;
        //private TMPro_UpdateManager m_updateManager;
        //private bool isAlreadyScheduled;

        // DEBUG Variables
        //private System.Diagnostics.Stopwatch m_StopWatch;
        //private int frame = 0;
        //private int loopCountA = 0;
        //private int loopCountB = 0;
        //private int loopCountC = 0;
        //private int loopCountD = 0;
        //private int loopCountE = 0;

       
        protected override void Awake()
        {
            //base.Awake();
            //Debug.Log("***** Awake() *****"); // on Object ID:" + GetInstanceID());      

            m_isAwake = true;      
            // Cache Reference to the Canvas
            //m_canvas = GetComponentInParent<Canvas>();
            //if (m_canvas == null)
            //    m_canvas = gameObject.AddComponent<Canvas>();

            // Cache Reference to RectTransform.
            m_rectTransform = gameObject.GetComponent<RectTransform>();
            if (m_rectTransform == null)
                m_rectTransform = gameObject.AddComponent<RectTransform>();

                          
            // Cache a reference to the UIRenderer.
            m_uiRenderer = GetComponent<CanvasRenderer>();
            if (m_uiRenderer == null) 
				m_uiRenderer = gameObject.AddComponent<CanvasRenderer> ();

			m_uiRenderer.hideFlags = HideFlags.HideInInspector;

            // Determine if the RectTransform is Driven         
            m_layoutController = GetComponent(typeof(ILayoutController)) as ILayoutController ?? (transform.parent != null ? transform.parent.GetComponent(typeof(ILayoutController)) as ILayoutController : null);           
            if (m_layoutController != null) IsRectTransformDriven = true;

            // Cache reference to Mask Component if one is present         
            m_mask = GetComponentInParent<Mask>(); 
                       

            // Load the font asset and assign material to renderer.
            LoadFontAsset();

            // Allocate our initial buffers.          
            m_char_buffer = new int[m_max_characters];           
            m_cached_GlyphInfo = new GlyphInfo();
            m_uiVertices = new UIVertex[0]; // 
            m_isFirstAllocation = true;          
            
            m_textInfo = new TextInfo();
            m_textInfo.wordInfo = new List<WordInfo>();
            m_textInfo.lineInfo = new LineInfo[m_max_numberOfLines];
            m_textInfo.meshInfo = new TMPro_MeshInfo();      
            
            m_fontAssetArray = new TextMeshProFont[16];
            
          
         
            // Check if we have a font asset assigned. Return if we don't because no one likes to see purple squares on screen.
            if (m_fontAsset == null)
            {
                Debug.LogWarning("Please assign a Font Asset to this " + transform.name + " gameobject.");
                return;
            }

            // Set Defaults for Font Auto-sizing
            if (m_fontSizeMin == 0) m_fontSizeMin = m_fontSize / 2;
            if (m_fontSizeMax == 0) m_fontSizeMax = m_fontSize * 2;

            //// Set flags to cause ensure our text is parsed and text redrawn. 
            isInputParsingRequired = true;
            havePropertiesChanged = true;
            m_rectTransformDimensionsChanged = true;
            m_isCalculateSizeRequired = true;

            
            //m_isNewTextObject = true;
            //LayoutRebuilder.MarkLayoutForRebuild(m_rectTransform);
            ForceMeshUpdate(); // Added to force OnWillRenderObject() to be called in case object is not visible so we get initial bounds for the mesh.
            ///* ScheduleUpdate();         
        }

           
        protected override void OnEnable()
        {
            //base.OnEnable();
            //Debug.Log("***** OnEnable() *****" + GetInstanceID());  // HavePropertiesChanged = " + havePropertiesChanged); // has been called on Object ID:" + gameObject.GetInstanceID());      
            
       
#if UNITY_EDITOR
            // Register Callbacks for various events.
            TMPro_EventManager.MATERIAL_PROPERTY_EVENT += ON_MATERIAL_PROPERTY_CHANGED;
            TMPro_EventManager.FONT_PROPERTY_EVENT += ON_FONT_PROPERTY_CHANGED;
            TMPro_EventManager.TEXTMESHPRO_UGUI_PROPERTY_EVENT += ON_TEXTMESHPRO_UGUI_PROPERTY_CHANGED;
            TMPro_EventManager.DRAG_AND_DROP_MATERIAL_EVENT += ON_DRAG_AND_DROP_MATERIAL;
            TMPro_EventManager.BASE_MATERIAL_EVENT += ON_BASE_MATERIAL_CHANGED;
#endif
            // Register to get callback before Canvas is Rendered.
            Canvas.willRenderCanvases += OnPreRenderCanvas;

            // Since Structures don't serialize in Unity 4.3 or below, we must re-allocate this array.          
            //m_textInfo.lineInfo = new LineInfo[m_max_numberOfLines];

            // Re-assign the Material to the Canvas Renderer
            if (m_uiRenderer.GetMaterial() == null)
            {
                if (m_sharedMaterial != null)
                {
                    m_uiRenderer.SetMaterial(m_sharedMaterial, null);
                }
                else
                {
                    // We likely had a masking material assigned
                    m_isNewBaseMaterial = true;
                    fontSharedMaterial = m_baseMaterial;
                    ParentMaskStateChanged();
                }

                havePropertiesChanged = true;                 
            }
   
            if (IsRectTransformDriven)
                LayoutRebuilder.MarkLayoutForRebuild(m_rectTransform);

        }

      

        protected override void OnDisable()
        {
            //base.OnDisable();
            //Debug.Log("***** OnDisable() *****"); //for " + this.name + " with ID: " + this.GetInstanceID() + " has been called.");

#if UNITY_EDITOR
            // Un-register the event this object was listening to
            TMPro_EventManager.MATERIAL_PROPERTY_EVENT -= ON_MATERIAL_PROPERTY_CHANGED;
            TMPro_EventManager.FONT_PROPERTY_EVENT -= ON_FONT_PROPERTY_CHANGED;
            TMPro_EventManager.TEXTMESHPRO_UGUI_PROPERTY_EVENT -= ON_TEXTMESHPRO_UGUI_PROPERTY_CHANGED;
            TMPro_EventManager.DRAG_AND_DROP_MATERIAL_EVENT -= ON_DRAG_AND_DROP_MATERIAL;
            TMPro_EventManager.BASE_MATERIAL_EVENT -= ON_BASE_MATERIAL_CHANGED;
#endif
            // Register to get callback before Canvas is Rendered.
            Canvas.willRenderCanvases -= OnPreRenderCanvas;
            
            m_uiRenderer.Clear();  // Might now want to clear since it wipes the Material in addition to the mesh geometry.

            if (IsRectTransformDriven)
                LayoutRebuilder.MarkLayoutForRebuild(m_rectTransform);

            // Clean up any material instances we may have created.
            if (m_fontMaterial != null)
            {
                //Debug.Log("Destroying " + m_fontMaterial.name);
                DestroyImmediate(m_fontMaterial);
            }


            //if (m_maskingMaterial != null)
            //{
            //    Debug.Log("Releasing " + m_maskingMaterial.name);
            //    MaterialManager.ReleaseMaskingMaterial(m_baseMaterial);
            //}      
        }


        protected override void OnDestroy()
        {
            //base.OnDestroy();
            //Debug.Log("***** OnDestroy() *****");
            
            if (m_maskingMaterial != null)
            {
                //Debug.Log("Trying to release Masking Material [" + m_maskingMaterial.name + "] with ID " + m_maskingMaterial.GetInstanceID());
                MaterialManager.ReleaseMaskingMaterial(m_maskingMaterial);           
            }  
        }


        protected override void Reset()
        {
            //base.Reset();
            //Debug.Log("***** Reset() *****"); //has been called.");  
            //LoadFontAsset();
            isInputParsingRequired = true;
            havePropertiesChanged = true;
        }


#if UNITY_EDITOR
        protected override void OnValidate()
        {                   
            //base.OnValidate(); 
            // Additional Properties could be added to sync up Serialized Properties & Properties.

            //Debug.Log("***** OnValidate() *****" + GetInstanceID()); // New Material [" + m_sharedMaterial.name + "] with ID " + m_sharedMaterial.GetInstanceID() + ". Base Material is [" + m_baseMaterial.name + "] with ID " + m_baseMaterial.GetInstanceID() + ". Previous Base Material is [" + (m_lastBaseMaterial == null ? "Null" : m_lastBaseMaterial.name) + "]."); 

            //if (!m_isAwake)
            //    return;

            if (hasFontAssetChanged) { LoadFontAsset(); m_isCalculateSizeRequired = true; hasFontAssetChanged = false; }
            font = m_fontAsset;      
            
            text = m_text;
           
            //color = m_fontColor32;  

            //if (m_fontMaterial != null)
            //    fontMaterial = m_fontMaterial;
            

            // Check if Base Material has changed
            //if (m_sharedMaterial == null)
            //{
            //    m_sharedMaterial = m_baseMaterial;
            //    m_isNewBaseMaterial = true;
            //}
            fontSharedMaterial = m_sharedMaterial;

            checkPaddingRequired = true;
            //if (m_sharedMaterial == null)
            //{
            //    Debug.Log(m_baseMaterial);
                //m_sharedMaterial = m_baseMaterial;
                //m_isNewBaseMaterial = true;
            //}

                                                                   
           
            
                      
            //maskOffset = m_maskOffset;
            //enableAutoSizing = m_enableAutoSizing;
            //fontSize = m_fontSize;
           
            margin = m_margin; // Getting called on assembly reloads.          
        }


        // Event to Track Material Changed resulting from Drag-n-drop.
        void ON_DRAG_AND_DROP_MATERIAL(Material currentMaterial, Material newMaterial)
        {                       
            
            //Debug.Log("Drag-n-Drop Event - Receiving Object ID " + GetInstanceID() + ". New Material is " + newMaterial.name + " with ID " + newMaterial.GetInstanceID() + ". Base Material is " + m_baseMaterial.name + " with ID " + m_baseMaterial.GetInstanceID());

            // Check if event applies to this current object
            if (currentMaterial.GetInstanceID() == m_baseMaterial.GetInstanceID())
            {
                Debug.Log("Assigning new Base Material " + newMaterial.name + " to replace " + currentMaterial.name);

                UnityEditor.Undo.RecordObject(this, "Material Assignment");
                fontSharedMaterial = newMaterial;
                m_baseMaterial = newMaterial;
            }
        }


        // Event received when custom material editor properties are changed.
        void ON_MATERIAL_PROPERTY_CHANGED(bool isChanged, Material mat)
        {
            //Debug.Log("ON_MATERIAL_PROPERTY_CHANGED event received by object with ID " + GetInstanceID()); // + " and Targeted Material is: " + mat.name + " with ID " + mat.GetInstanceID() + ". Base Material is " + m_baseMaterial.name + " with ID " + m_baseMaterial.GetInstanceID() + ". Masking Material is " + m_maskingMaterial.name + "  with ID " + m_maskingMaterial.GetInstanceID());         
            
            ShaderUtilities.GetShaderPropertyIDs(); // Initialize ShaderUtilities and get shader property IDs.

            int materialID = mat.GetInstanceID();

            if (m_uiRenderer.GetMaterial() == null)
            {
                if (m_fontAsset != null)
                {
                    m_uiRenderer.SetMaterial(m_fontAsset.material, null);
                    Debug.LogWarning("No Material was assigned to " + name + ". " + m_fontAsset.material.name + " was assigned.");
                }
                else
                    Debug.LogWarning("No Font Asset assigned to " + name + ". Please assign a Font Asset.");
            }
           
            /*
            if (m_fontAsset.atlas.GetInstanceID() != mat.GetTexture(ShaderUtilities.ID_MainTex).GetInstanceID())
            {
                m_uiRenderer.SetMaterial(m_sharedMaterial, null);
                //m_renderer.sharedMaterial = m_fontAsset.material;
                Debug.LogWarning("Font Asset Atlas doesn't match the Atlas in the newly assigned material. Select a matching material or a different font asset.");
            }
            */

            if (m_uiRenderer.GetMaterial() != m_sharedMaterial && m_fontAsset == null) //    || m_renderer.sharedMaterials.Contains(mat))
            {
                Debug.Log("ON_MATERIAL_PROPERTY_CHANGED Called on Target ID: " + GetInstanceID() + ". Previous Material:" + m_sharedMaterial + "  New Material:" + m_uiRenderer.GetMaterial()); // on Object ID:" + GetInstanceID() + ". m_sharedMaterial: " + m_sharedMaterial.name + "  m_renderer.sharedMaterial: " + m_renderer.sharedMaterial.name);         
                m_sharedMaterial = m_uiRenderer.GetMaterial();
            }


            
            // Is Material being modified my Base Material            
            if (m_mask != null && m_baseMaterial != null && m_maskingMaterial != null)
            {
                if (materialID == m_baseMaterial.GetInstanceID())
                {
                    //Debug.Log("Copying Material properties from " + mat + " to " + m_maskingMaterial);
                    float stencilID = m_maskingMaterial.GetFloat(ShaderUtilities.ID_StencilID);
                    float stencilComp = m_maskingMaterial.GetFloat(ShaderUtilities.ID_StencilComp);
                    m_maskingMaterial.CopyPropertiesFromMaterial(mat);
                    m_maskingMaterial.shaderKeywords = mat.shaderKeywords;

                    m_maskingMaterial.SetFloat(ShaderUtilities.ID_StencilID, stencilID);
                    m_maskingMaterial.SetFloat(ShaderUtilities.ID_StencilComp, stencilComp);
                }
                else if (materialID == m_maskingMaterial.GetInstanceID())
                {
                    //Debug.Log("Copying Material properties from " + mat + " to " + m_baseMaterial);
                    m_baseMaterial.CopyPropertiesFromMaterial(mat);
                    m_baseMaterial.shaderKeywords = mat.shaderKeywords;
                    m_baseMaterial.SetFloat(ShaderUtilities.ID_StencilID, 0);
                    m_baseMaterial.SetFloat(ShaderUtilities.ID_StencilComp, 8);
                }
            }
            
                       
            //Debug.Log("Assigned Material is " + m_sharedMaterial.name + " with ID " + m_sharedMaterial.GetInstanceID() +
            //          ". Target Mat is " + mat.name + " with ID " + mat.GetInstanceID() + 
            //          ". Base Material is " + m_baseMaterial.name + " with ID " + m_baseMaterial.GetInstanceID() + 
            //          ". Masking Material is " + m_maskingMaterial.name + " with ID " + m_maskingMaterial.GetInstanceID());

                                 
            m_padding = ShaderUtilities.GetPadding(m_uiRenderer.GetMaterial(), m_enableExtraPadding, m_isUsingBold);
            m_alignmentPadding = ShaderUtilities.GetFontExtent(m_uiRenderer.GetMaterial());
            havePropertiesChanged = true;
            /* ScheduleUpdate(); */
        }


        // Event received to handle base material changes related to masking
        void ON_BASE_MATERIAL_CHANGED(Material mat)
        {           
            /*
            Debug.Log("Assigning new Base Material " + mat.name + " to replace " + m_baseMaterial.name);
            
            
            // Remove reference to masking material for this base material if one exists.
            if (m_maskingMaterial != null)
                MaterialManager.ReleaseMaskingMaterial(m_baseMaterial);

            // Check if Masking is enabled and if so assign the masking material.
            if (m_mask != null && m_mask.MaskEnabled())
            {
                if (m_maskingMaterial == null)
                    m_maskingMaterial = MaterialManager.GetMaskingMaterial(mat, 1);

                //Debug.Log("Shared Material is " + fontSharedMaterial.name + " with ID " + fontSharedMaterial.GetInstanceID() + ". Masking Material is " + m_maskingMaterial.name + " with ID " + m_maskingMaterial.GetInstanceID());
                //Debug.Log("Masking Material is " + m_maskingMaterial.name + " with ID " + m_maskingMaterial.GetInstanceID());

                fontSharedMaterial = m_maskingMaterial;
            }



            
            if (m_baseMaterial.GetInstanceID() == mat.GetInstanceID())
            {
                // Update the Masking Material Properties
                float stencilID = m_maskingMaterial.GetFloat(ShaderUtilities.ID_StencilID);
                float stencilComp = m_maskingMaterial.GetFloat(ShaderUtilities.ID_StencilComp);
                m_maskingMaterial.CopyPropertiesFromMaterial(mat);
                m_maskingMaterial.shaderKeywords = mat.shaderKeywords;

                m_maskingMaterial.SetFloat(ShaderUtilities.ID_StencilID, stencilID);
                m_maskingMaterial.SetFloat(ShaderUtilities.ID_StencilComp, stencilComp);

                m_padding = ShaderUtilities.GetPadding(m_sharedMaterial, m_enableExtraPadding, m_isUsingBold);
                m_alignmentPadding = ShaderUtilities.GetFontExtent(m_sharedMaterial);
                havePropertiesChanged = true;
            }
            */
        }


        // Event received when font asset properties are changed in Font Inspector
        void ON_FONT_PROPERTY_CHANGED(bool isChanged, TextMeshProFont font)
        {
            if (font == m_fontAsset)
            {
                //Debug.Log("ON_FONT_PROPERTY_CHANGED event received.");
                havePropertiesChanged = true;
                hasFontAssetChanged = true;
                /* ScheduleUpdate(); */
            }
        }


        // Event received when UNDO / REDO Event alters the properties of the object.
        void ON_TEXTMESHPRO_UGUI_PROPERTY_CHANGED(bool isChanged, TextMeshProUGUI obj)
        {
            Debug.Log("Event Received by " + obj);
            
            if (obj == this)
            {
                //Debug.Log("Undo / Redo Event Received by Object ID:" + GetInstanceID());
                havePropertiesChanged = true;
                isInputParsingRequired = true;
                /* ScheduleUpdate(); */
            }
        }
#endif


        // Function which loads either the default font or a newly assigned font asset. This function also assigned the appropriate material to the renderer.
        void LoadFontAsset()
        {
            //Debug.Log("***** LoadFontAsset() *****"); //TextMeshPro LoadFontAsset() has been called."); // Current Font Asset is " + (font != null ? font.name: "Null") );
            ShaderUtilities.GetShaderPropertyIDs();

            if (m_fontAsset == null) 
            {
                
                //Debug.LogWarning("No Font Asset has been assigned. Loading Default Arial SDF Font.");
                
                m_fontAsset = Resources.Load("Fonts & Materials/ARIAL SDF", typeof(TextMeshProFont)) as TextMeshProFont;
                if (m_fontAsset == null)
                {
                    Debug.LogWarning("There is no Font Asset assigned to " + gameObject.name + ".");
                    return;
                }

                if (m_fontAsset.characterDictionary == null)
                {
                    Debug.Log("Dictionary is Null!");
                }

                               
                //m_uiRenderer.SetMaterial(m_fontAsset.material, null);
                m_sharedMaterial = m_fontAsset.material;
                m_baseMaterial = m_sharedMaterial;
                m_isNewBaseMaterial = true;
              
                //m_renderer.receiveShadows = false;
                //m_renderer.castShadows = false; // true;
                // Get a Reference to the Shader
            }
            else
            {
                if (m_fontAsset.characterDictionary == null)
                {
                    //Debug.Log("Reading Font Definition and Creating Character Dictionary.");
                    m_fontAsset.ReadFontDefinition();
                }


                // Force the use of the base material
                m_sharedMaterial = m_baseMaterial;
                m_isNewBaseMaterial = true;              


                // If font atlas texture doesn't match the assigned material font atlas, switch back to default material specified in the Font Asset.
                if (m_sharedMaterial == null || m_sharedMaterial.mainTexture == null || m_fontAsset.atlas.GetInstanceID() != m_sharedMaterial.mainTexture.GetInstanceID())
                {                                       
                    m_sharedMaterial = m_fontAsset.material;
                    m_baseMaterial = m_sharedMaterial;
                    m_isNewBaseMaterial = true;
                }
                else
                {                   
                    // Loading the same font asset.
                    //m_sharedMaterial = m_baseMaterial;                  
                }

                //m_sharedMaterial.SetFloat("_CullMode", 0);
                //m_sharedMaterial.SetFloat("_ZTestMode", 4);

                // Check if we are using the SDF Surface Shader
                if (m_sharedMaterial.passCount > 1)
                {
                    //m_renderer.receiveShadows = false;
                    //m_renderer.castShadows = true;
                }
                else
                {
                    //m_renderer.receiveShadows = false;
                    //m_renderer.castShadows = false;
                }
            }
            
            // Check & Assign Underline Character for use with the Underline tag.
            if (!m_fontAsset.characterDictionary.TryGetValue(95, out m_cached_Underline_GlyphInfo)) //95
                Debug.LogWarning("Underscore character wasn't found in the current Font Asset. No characters assigned for Underline.");
            
                                                 
            fontSharedMaterial = m_sharedMaterial;
            
            m_sharedMaterials.Add(m_sharedMaterial);
                                 
            // Might be Redundant
            m_padding = ShaderUtilities.GetPadding(m_sharedMaterial, m_enableExtraPadding, m_isUsingBold);
            m_alignmentPadding = ShaderUtilities.GetFontExtent(m_sharedMaterial);

                     

            //Debug.Log("Base Material is " + m_baseMaterial.name + " with ID " + m_baseMaterial.GetInstanceID() + ". Shared Material is " + m_sharedMaterial.name + " with ID " + m_sharedMaterial.GetInstanceID());
            //Debug.Log("Shared Material is " + m_sharedMaterial + ". Base Material is " + m_baseMaterial + ". Masking Material is " + m_maskingMaterial);
            
            //m_uiRenderer.SetMaterial(this.material, null);
            // Hide Material Editor Component
            //m_renderer.sharedMaterial.hideFlags = HideFlags.None;
        }


        /// <summary>
        /// Function under development to utilize an Update Manager instead of normal event functions like LateUpdate() or OnWillRenderObject().
        /// </summary>
        void ScheduleUpdate()
        {
            return;
            /*
            if (!isAlreadyScheduled)
            {
                m_updateManager.ScheduleObjectForUpdate(this);
                isAlreadyScheduled = true;
            }
            */
        }



        void UpdateEnvMapMatrix()
        {
            if (!m_sharedMaterial.HasProperty(ShaderUtilities.ID_EnvMap) || m_sharedMaterial.GetTexture(ShaderUtilities.ID_EnvMap) == null)
                return;

            Debug.Log("Updating Env Matrix...");
            Vector3 rotation = m_sharedMaterial.GetVector(ShaderUtilities.ID_EnvMatrixRotation);
            m_EnvMapMatrix = Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(rotation), Vector3.one);

            m_sharedMaterial.SetMatrix(ShaderUtilities.ID_EnvMatrix, m_EnvMapMatrix);
        }


        // Enable Masking in the Shader
        void EnableMasking()
        {
            Material mat = m_uiRenderer.GetMaterial();
            if (mat.HasProperty(ShaderUtilities.ID_MaskCoord))
            {
                mat.EnableKeyword("MASK_SOFT");
                mat.DisableKeyword("MASK_HARD");
                mat.DisableKeyword("MASK_OFF");

                m_isMaskingEnabled = true;
                UpdateMask();
            }
        }


        // Enable Masking in the Shader
        void DisableMasking()
        {
            Material mat = m_uiRenderer.GetMaterial();
            if (mat.HasProperty(ShaderUtilities.ID_MaskCoord))
            {
                mat.EnableKeyword("MASK_OFF");
                mat.DisableKeyword("MASK_HARD");
                mat.DisableKeyword("MASK_SOFT");

                m_isMaskingEnabled = false;
                UpdateMask();
            }
        }


        // Update & recompute Mask offset
        void UpdateMask()
        {

            if (m_rectTransform != null)
            {
                Material mat = m_uiRenderer.GetMaterial();
                if (mat == null || (m_overflowMode == TextOverflowModes.ScrollRect && m_isScrollRegionSet))
                    return;

                if (!ShaderUtilities.isInitialized)
                    ShaderUtilities.GetShaderPropertyIDs();
                
                //Debug.Log("Setting Mask for the first time.");

                m_isScrollRegionSet = true;

                float softnessX = Mathf.Min(Mathf.Min(m_margin.x, m_margin.z), mat.GetFloat(ShaderUtilities.ID_MaskSoftnessX));
                float softnessY = Mathf.Min(Mathf.Min(m_margin.y, m_margin.w), mat.GetFloat(ShaderUtilities.ID_MaskSoftnessY));

                softnessX = softnessX > 0 ? softnessX : 0;
                softnessY = softnessY > 0 ? softnessY : 0;

                float width = (m_rectTransform.rect.width - Mathf.Max(m_margin.x, 0) - Mathf.Max(m_margin.z, 0)) / 2 + softnessX;
                float height = (m_rectTransform.rect.height - Mathf.Max(m_margin.y, 0) - Mathf.Max(m_margin.w, 0)) / 2 + softnessY;

                
                Vector2 center = m_rectTransform.localPosition + new Vector3((0.5f - m_rectTransform.pivot.x) * m_rectTransform.rect.width + (Mathf.Max(m_margin.x, 0) - Mathf.Max(m_margin.z, 0)) / 2, (0.5f - m_rectTransform.pivot.y) * m_rectTransform.rect.height + (-Mathf.Max(m_margin.y, 0) + Mathf.Max(m_margin.w, 0)) / 2);                           
        
                //Vector2 center = m_rectTransform.localPosition + new Vector3((0.5f - m_rectTransform.pivot.x) * m_rectTransform.rect.width + (margin.x - margin.z) / 2, (0.5f - m_rectTransform.pivot.y) * m_rectTransform.rect.height + (-margin.y + margin.w) / 2);
                Vector4 mask = new Vector4(center.x, center.y, width, height);
                //Debug.Log(mask);



                //Rect rect = new Rect(0, 0, m_rectTransform.rect.width + margin.x + margin.z, m_rectTransform.rect.height + margin.y + margin.w);
                //int softness = (int)m_sharedMaterial.GetFloat(ShaderUtilities.ID_MaskSoftnessX) / 2;
                m_uiRenderer.GetMaterial().SetVector(ShaderUtilities.ID_MaskCoord, mask);
            }
        }


        //void UpdateMask()
        //{
            //Debug.Log("Changing property block.");
            
            //MaterialPropertyBlock block = new MaterialPropertyBlock();
            //Vector4 offsetDelta = new Vector4(m_mesh.bounds.center.x + m_maskOffset.x, m_mesh.bounds.center.y + m_maskOffset.y, m_mesh.bounds.extents.x + m_maskOffset.z, m_mesh.bounds.extents.y + m_maskOffset.w); 
            //block.AddVector(ShaderUtilities.ID_MaskCoord, m_maskOffset);
            //block.AddFloat(ShaderUtilities.ID_MaskSoftnessX, m_maskSoftness.x);
            //block.AddFloat(ShaderUtilities.ID_MaskSoftnessY, m_maskSoftness.y);
            //block.AddFloat(ShaderUtilities.ID_VertexOffsetX, m_vertexOffset.x);
            //block.AddFloat(ShaderUtilities.ID_VertexOffsetY, m_vertexOffset.y);

            //m_renderer.SetPropertyBlock(block);          
        //}


        // Function to allocate the necessary buffers to render the text. This function is called whenever the buffer size needs to be increased.
        void SetMeshArrays(int size)
        {
            // Should add a check to make sure we don't try to create a mesh that contains more than 65535 vertices.

            int sizeX4 = size * 4;        

            m_uiVertices = new UIVertex[sizeX4];
           
            // Setup Triangle Structure 
            for (int i = 0; i < size; i++)
            {
                int index_X4 = i * 4;
                //int index_X6 = i * 6;

                m_uiVertices[0 + index_X4].position = Vector3.zero;
                m_uiVertices[1 + index_X4].position = Vector3.zero;
                m_uiVertices[2 + index_X4].position = Vector3.zero;
                m_uiVertices[3 + index_X4].position = Vector3.zero;

                m_uiVertices[0 + index_X4].normal = new Vector3(0, 0, -1);
                m_uiVertices[1 + index_X4].normal = new Vector3(0, 0, -1);
                m_uiVertices[2 + index_X4].normal = new Vector3(0, 0, -1);
                m_uiVertices[3 + index_X4].normal = new Vector3(0, 0, -1);

                m_uiVertices[0 + index_X4].tangent = new Vector4(-1, 0, 0, 1);
                m_uiVertices[1 + index_X4].tangent = new Vector4(-1, 0, 0, 1);
                m_uiVertices[2 + index_X4].tangent = new Vector4(-1, 0, 0, 1);
                m_uiVertices[3 + index_X4].tangent = new Vector4(-1, 0, 0, 1);              
            }

            //Debug.Log("Size:" + size + "  Vertices:" + m_vertices.Length + "  Triangles:" + m_triangles.Length + " Mesh - Vertices:" + m_mesh.vertices.Length + "  Triangles:" + m_mesh.triangles.Length);

            m_uiRenderer.SetVertices(m_uiVertices, sizeX4);           
        }


        // Function called internally when a new material is assigned via the fontMaterial property.
        void SetFontMaterial(Material mat)
        {          
            // Check in case Object is disabled. If so, we don't have a valid reference to the Renderer.
            // This can occur when the Duplicate Material Context menu is used on an inactive object.
            if (m_uiRenderer == null)
                m_uiRenderer = GetComponent<CanvasRenderer>();

            // Create an instance of the material
            if (m_fontMaterial != null) DestroyImmediate(m_fontMaterial); // If an instance already exists, destroy it.
            m_fontMaterial = new Material(mat);
            m_fontMaterial.hideFlags = HideFlags.HideAndDontSave; // Make sure material isn't saved.
            m_fontMaterial.name += " Instance";
            m_fontMaterial.shaderKeywords = mat.shaderKeywords;
           
            m_sharedMaterial = m_fontMaterial; 
            m_uiRenderer.SetMaterial(m_sharedMaterial, null);          
           
            m_padding = ShaderUtilities.GetPadding(m_sharedMaterial, m_enableExtraPadding, m_isUsingBold);
            m_alignmentPadding = ShaderUtilities.GetFontExtent(m_sharedMaterial);
        }


        // Function called internally when a new shared material is assigned via the fontSharedMaterial property.
        void SetSharedFontMaterial(Material mat)
        {
            ShaderUtilities.GetShaderPropertyIDs();

            // Check in case Object is disabled. If so, we don't have a valid reference to the Renderer.
            // This can occur when the Duplicate Material Context menu is used on an inactive object. 
            if (m_uiRenderer == null)
                m_uiRenderer = GetComponent<CanvasRenderer>();

            if (mat == null) { mat = m_baseMaterial; m_isNewBaseMaterial = true; }


            // Check if Material has Stencil Support
            if (mat.HasProperty(ShaderUtilities.ID_StencilID))
            {
                // Check if Material is a Base Material
                if (mat.HasProperty(ShaderUtilities.ID_StencilID) && mat.GetFloat(ShaderUtilities.ID_StencilID) == 0)
                {
                    if (m_baseMaterial == null) m_baseMaterial = m_sharedMaterial; // Addition check in the transition to the use of a Base Material.
                 
                    // if new Base Material
                    if (mat.GetInstanceID() != m_baseMaterial.GetInstanceID())
                    {
                        m_baseMaterial = mat;
                        m_isNewBaseMaterial = true;
                    }

                }
                else
                {
                    // If new Masking Material
                    if (mat != m_maskingMaterial)
                    {
                        if (m_maskingMaterial != null)
                            MaterialManager.ReleaseMaskingMaterial(m_maskingMaterial);

                        //Debug.Log("A New Masking Material [" + mat + "]. Previous Masking Material [" + m_maskingMaterial);
                        m_maskingMaterial = mat;
                        MaterialManager.AddMaskingMaterial(m_baseMaterial, m_maskingMaterial, 1);
                    }
                    else
                    {
                        // Remove masking material from list
                        //MaterialManager.RemoveMaskingMaterial(m_maskingMaterial);

                        // Add Masking Material back
                        //MaterialManager.AddMaskingMaterial(m_baseMaterial, m_maskingMaterial, 1);
                        //Debug.Log("Masking Material [" + mat.name + "] with ID " + mat.GetInstanceID() + " is assigned.");
                    }
                }


                if (m_mask && m_mask.MaskEnabled())
                {
                    if (m_isNewBaseMaterial)
                    {
                        if (m_maskingMaterial != null)
                            MaterialManager.ReleaseMaskingMaterial(m_maskingMaterial);

                        m_maskingMaterial = MaterialManager.GetMaskingMaterial(m_baseMaterial, 1);
                        mat = m_maskingMaterial;
                    }
                }
            }

            m_isNewBaseMaterial = false;
            
            m_uiRenderer.SetMaterial(mat, null);
            m_sharedMaterial = mat;           
            m_padding = ShaderUtilities.GetPadding(m_sharedMaterial, m_enableExtraPadding, m_isUsingBold);
            m_alignmentPadding = ShaderUtilities.GetFontExtent(m_sharedMaterial);

            //Debug.Log("New Material [" + mat.name + "] with ID " + mat.GetInstanceID() + " has been assigned. Base Material is [" + m_baseMaterial.name + "] with ID " + m_baseMaterial.GetInstanceID());
            //MaterialManager.ListMaterials();
        }


        void SetFontBaseMaterial(Material mat)
        {
            Debug.Log("Changing Base Material from [" + (m_lastBaseMaterial == null ? "Null" : m_lastBaseMaterial.name) + "] to [" + mat.name + "].");

            
            // Remove reference to masking material for this base material if one exists.
            //if (m_maskingMaterial != null)           
            //    MaterialManager.ReleaseMaskingMaterial(m_lastBaseMaterial);
            

            // Assign new Base Material
            m_baseMaterial = mat;
            m_lastBaseMaterial = mat;

            // Check if Masking is enabled and if so assign the masking material.
            //if (m_mask != null && m_mask.enabled)
            //{
                //if (m_maskingMaterial == null)
            //    m_maskingMaterial = MaterialManager.GetMaskingMaterial(mat, 1);
            
            //    fontSharedMaterial = m_maskingMaterial;
            //}

            //m_isBaseMaterialChanged = false;
        }


        // Function to create an instance of the material used for Masking.
        void CreateMaskingMaterial(Material mat)
        {
            // Check in case Object is disabled. If so, we don't have a valid reference to the Renderer.
            // This can occur when the Duplicate Material Context menu is used on an inactive object.
            if (m_uiRenderer == null)
                m_uiRenderer = GetComponent<CanvasRenderer>();

            // Store the previous Material so we can re-assign it when needed. 
            //Material newMat = MaterialManager.Add(mat);

            // Create an instance of the material
            if (m_maskingMaterial != null) DestroyImmediate(m_maskingMaterial); // If an instance already exists, destroy it.
            m_maskingMaterial = new Material(mat);
            m_maskingMaterial.hideFlags = HideFlags.HideAndDontSave; // Make sure material isn't saved.
            m_maskingMaterial.name += " Masking";
            m_maskingMaterial.shaderKeywords = mat.shaderKeywords;

            // Set Stencil Properties
            ShaderUtilities.GetShaderPropertyIDs();
            m_maskingMaterial.SetFloat(ShaderUtilities.ID_StencilID, 1); // Should be changed to match stencil value in mask.
            m_maskingMaterial.SetFloat(ShaderUtilities.ID_StencilComp, 3);


            //m_sharedMaterial = m_maskingMaterial;
            //m_uiRenderer.SetMaterial(m_sharedMaterial, null);

            //m_padding = ShaderUtilities.GetPadding(m_sharedMaterial, m_enableExtraPadding, m_isUsingBold);
            //m_alignmentPadding = ShaderUtilities.GetFontExtent(m_sharedMaterial);
        } 


        // This function will create an instance of the Font Material.
        void SetOutlineThickness(float thickness)
        {
            // Check if we need to create an instance of the material
            if (m_fontMaterial == null)
                CreateMaterialInstance();

            // Check to make sure we still have a Material assigned to the CanvasRenderer
            if (m_uiRenderer.GetMaterial() == null)
                m_uiRenderer.SetMaterial(m_fontMaterial, null);
                       
            thickness = Mathf.Clamp01(thickness);
            m_uiRenderer.GetMaterial().SetFloat(ShaderUtilities.ID_OutlineWidth, thickness);                   
        }


        // This function will create an instance of the Font Material.
        void SetFaceColor(Color32 color)
        {           
            // Check if we need to create an instance of the material
            if (m_fontMaterial == null)
                CreateMaterialInstance();
            
            // Check to make sure we still have a Material assigned to the CanvasRenderer
            if (m_uiRenderer.GetMaterial() == null)
                m_uiRenderer.SetMaterial(m_fontMaterial, null);
            
            m_uiRenderer.GetMaterial().SetColor(ShaderUtilities.ID_FaceColor, color);                 
        }


        // This function will create an instance of the Font Material.
        void SetOutlineColor(Color32 color)
        {
            // Check if we need to create an instance of the material
            if (m_fontMaterial == null)
                CreateMaterialInstance();

            // Check to make sure we still have a Material assigned to the CanvasRenderer
            if (m_uiRenderer.GetMaterial() == null)
                m_uiRenderer.SetMaterial(m_fontMaterial, null);
                  
            m_uiRenderer.GetMaterial().SetColor(ShaderUtilities.ID_OutlineColor, color);          
        }


        // Function used to create an instance of the material
        void CreateMaterialInstance()
        {          
            Material mat = new Material(m_sharedMaterial);
            mat.shaderKeywords = m_sharedMaterial.shaderKeywords;
            
            //mat.hideFlags = HideFlags.DontSave;
            mat.name += " Instance";
            m_uiRenderer.SetMaterial(mat, null);
            m_fontMaterial = mat;
        }


        // Sets the Render Queue and Ztest mode 
        void SetShaderType()
        {
            if (m_isOverlay)
            {
                // Changing these properties results in an instance of the material           
                //m_renderer.material.SetFloat("_ZTestMode", 8);
                //m_renderer.material.renderQueue = 4000;

                //m_sharedMaterial = m_renderer.material;
            }
            else
            {   // TODO: This section needs to be tested.
                //m_renderer.material.SetFloat("_ZWriteMode", 0);
                //m_renderer.material.SetFloat("_ZTestMode", 4);
                //m_renderer.material.renderQueue = -1;
                //m_sharedMaterial = m_renderer.material;
            }

            //if (m_fontMaterial == null)
            //    m_fontMaterial = m_renderer.material;
        }


        // Sets the Culling mode of the material
        void SetCulling()
        {
            if (m_isCullingEnabled)
            {                        
                m_uiRenderer.GetMaterial().SetFloat("_CullMode", 2);
            }
            else
            {
                m_uiRenderer.GetMaterial().SetFloat("_CullMode", 0);
            }
        }


        // Set Perspective Correction Mode based on whether Camera is Orthographic or Perspective
        void SetPerspectiveCorrection()
        {
            if (m_isOrthographic)
                m_sharedMaterial.SetFloat(ShaderUtilities.ID_PerspectiveFilter, 0.0f);
            else
                m_sharedMaterial.SetFloat(ShaderUtilities.ID_PerspectiveFilter, 0.875f);
        }


        // Function used in conjunection with SetText()
        void AddIntToCharArray(int number, ref int index, int precision)
        {
            if (number < 0)
            {
                m_input_CharArray[index++] = '-';
                number = -number;
            }

            int i = index;
            do
            {
                m_input_CharArray[i++] = (char)(number % 10 + 48);
                number /= 10;
            } while (number > 0);

            int lastIndex = i;

            // Reverse string
            while (index + 1 < i)
            {
                i -= 1;
                char t = m_input_CharArray[index];
                m_input_CharArray[index] = m_input_CharArray[i];
                m_input_CharArray[i] = t;
                index += 1;
            }
            index = lastIndex;
        }


        // Functions used in conjunction with SetText()
        void AddFloatToCharArray(float number, ref int index, int precision)
        {
            if (number < 0)
            {
                m_input_CharArray[index++] = '-';
                number = -number;
            }

            number += k_Power[Mathf.Min(9, precision)];

            int integer = (int)number;
            AddIntToCharArray(integer, ref index, precision);

            if (precision > 0)
            {
                // Add the decimal point
                m_input_CharArray[index++] = '.';

                number -= integer;
                for (int p = 0; p < precision; p++)
                {
                    number *= 10;
                    int d = (int)(number);

                    m_input_CharArray[index++] = (char)(d + 48);
                    number -= d;
                }
            }
        }


        // Converts a string to a Char[]
        void StringToCharArray(string text, ref int[] chars)
        {
            if (text == null)
                return;

            // Check to make sure chars_buffer is large enough to hold the content of the string.
            if (chars.Length <= text.Length)
            {
                int newSize = text.Length > 1024 ? text.Length + 256 : Mathf.NextPowerOfTwo(text.Length + 1);
                //Debug.Log("Resizing the chars_buffer[" + chars.Length + "] to chars_buffer[" + newSize + "].");
                chars = new int[newSize];
            }

            int index = 0;

            for (int i = 0; i < text.Length; i++)
            {
                if (text[i] == 92 && i < text.Length - 1)
                {
                    switch ((int)text[i + 1])
                    {
                        case 116: // \t Tab
                            chars[index] = (char)9;
                            i += 1;
                            index += 1;
                            continue;
                        case 110: // \n LineFeed
                            chars[index] = (char)10;
                            i += 1;
                            index += 1;
                            continue;
                    }
                }

                chars[index] = text[i];
                index += 1;
            }
            chars[index] = (char)0;
        }


        // Copies Content of formatted SetText() to charBuffer.
        void SetTextArrayToCharArray(char[] charArray, ref int[] charBuffer)
        {
            //Debug.Log("SetText Array to Char called.");
            if (charArray == null || m_charArray_Length == 0)
                return;

            // Check to make sure chars_buffer is large enough to hold the content of the string.
            if (charBuffer.Length <= m_charArray_Length)
            {
                int newSize = m_charArray_Length > 1024 ? m_charArray_Length + 256 : Mathf.NextPowerOfTwo(m_charArray_Length + 1);
                charBuffer = new int[newSize];
            }

            int index = 0;

            for (int i = 0; i < m_charArray_Length; i++)
            {
                if (charArray[i] == 92 && i < m_charArray_Length - 1)
                {
                    switch ((int)charArray[i + 1])
                    {
                        case 116: // \t Tab
                            charBuffer[index] = 9;
                            i += 1;
                            index += 1;
                            continue;
                        case 110: // \n LineFeed
                            charBuffer[index] = 10;
                            i += 1;
                            index += 1;
                            continue;
                    }
                }

                charBuffer[index] = charArray[i];
                index += 1;
            }
            charBuffer[index] = 0;
        }


        /// <summary>
        /// Function used in conjunction with GetTextInfo to figure out Array allocations.
        /// </summary>
        /// <param name="chars"></param>
        /// <returns></returns>
        int GetArraySizes(int[] chars)
        {
            //Debug.Log("Set Array Size called.");

            int visibleCount = 0;
            int totalCount = 0;
            int tagEnd = 0;
            m_isUsingBold = false;

            m_VisibleCharacters.Clear();

            for (int i = 0; chars[i] != 0; i++)
            {
                int c = chars[i];

                if (m_isRichText && c == 60) // if Char '<'
                {
                    // Check if Tag is Valid
                    if (ValidateHtmlTag(chars, i + 1, out tagEnd))
                    {
                        i = tagEnd;
                        if ((m_style & FontStyles.Underline) == FontStyles.Underline) visibleCount += 3;

                        if ((m_style & FontStyles.Bold) == FontStyles.Bold) m_isUsingBold = true;

                        continue;
                    }
                }

                if (c != 32 && c != 9 && c != 10 && c != 13)
                {
                    visibleCount += 1;
                }

                m_VisibleCharacters.Add((char)c);  
                totalCount += 1;
            }
                   
            return totalCount;
        }




        // This function parses through the Char[] to determine how many characters will be visible. It then makes sure the arrays are large enough for all those characters.
        int SetArraySizes(int[] chars)
        {
            //Debug.Log("Set Array Size called.");

            int visibleCount = 0;
            int totalCount = 0;
            int tagEnd = 0;
            m_isUsingBold = false;

            m_VisibleCharacters.Clear();

            for (int i = 0; chars[i] != 0; i++)
            {
                int c = chars[i];

                if (m_isRichText && c == 60) // if Char '<'
                {
                    // Check if Tag is Valid
                    if (ValidateHtmlTag(chars, i + 1, out tagEnd))
                    {
                        i = tagEnd;
                        if ((m_style & FontStyles.Underline) == FontStyles.Underline) visibleCount += 3;

                        if ((m_style & FontStyles.Bold) == FontStyles.Bold) m_isUsingBold = true;
            
                        continue;
                    }
                }

                if (c != 32 && c != 9 && c != 10 && c != 13)
                {
                    visibleCount += 1;
                }

                m_VisibleCharacters.Add((char)c);   
                totalCount += 1;
            }


            if (m_textInfo.characterInfo == null || totalCount > m_textInfo.characterInfo.Length)
            {                
                m_textInfo.characterInfo = new TMPro_CharacterInfo[totalCount > 1024 ? totalCount + 256 : Mathf.NextPowerOfTwo(totalCount)];              
            }

            // Make sure our Mesh Buffer Allocations can hold these new Quads.
            if (m_uiVertices == null) m_uiVertices = new UIVertex[0];
            if (visibleCount * 4 > m_uiVertices.Length)
            {              
                // If this is the first allocation, we allocated exactly the number of Quads we need. Otherwise, we allocated more since this text object is dynamic.
                if (m_isFirstAllocation)
                {
                    SetMeshArrays(visibleCount);
                    m_isFirstAllocation = false;
                }
                else
                {
                    SetMeshArrays(visibleCount > 1024 ? visibleCount + 256 : Mathf.NextPowerOfTwo(visibleCount));
                }
            }

            return totalCount;
        }


        // Added to sort handle the potential issue with OnWillRenderObject() not getting called when objects are not visible by camera.
        //void OnBecameInvisible()
        //{
        //    if (m_mesh != null)
        //        m_mesh.bounds = new Bounds(transform.position, new Vector3(1000, 1000, 0));
        //}

      
        void ComputeMarginSize()
        {                      
                      
            if (m_rectTransform != null)
            {
                
                //Debug.Log("Computing new margins. Current RectTransform's Width is " + m_rectTransform.rect.width + " and Height is " + m_rectTransform.rect.height + "  Preferred Width: " + m_preferredWidth + " Height: " + m_preferredHeight);
                //Debug.Log("Preferred Width: " + LayoutUtility.GetPreferredSize(m_rectTransform, 0) + " Height: " + LayoutUtility.GetPreferredSize(m_rectTransform, 1));                

                if (m_rectTransform.rect.width == 0) m_marginWidth = Mathf.Infinity;
                else
                    m_marginWidth = m_rectTransform.rect.width - m_margin.x - m_margin.z;
                
                if (m_rectTransform.rect.height == 0) m_marginHeight = Mathf.Infinity;                 
                else                   
                    m_marginHeight = m_rectTransform.rect.height - m_margin.y - m_margin.w;
                            
            }
        }


        protected override void OnDidApplyAnimationProperties()
        {
            
            havePropertiesChanged = true;
            //Debug.Log("Animation Properties have changed.");
        }


        //protected override void OnTransformParentChanged()
        //{
        //    ParentMaskStateChanged(); 
        //}


        void OnRenderObject()
        {
            //frame += 1;
        }


        protected override void OnRectTransformDimensionsChange()
        {
            //Debug.Log("OnRectTransformDimensionsChange() called on Object ID " + GetInstanceID());
            
            // Make sure object is active in Hierarachy
            if (!this.gameObject.activeInHierarchy)
                return;

            ComputeMarginSize();
         
            if (m_rectTransform != null)
                m_rectTransform.hasChanged = true;
            else
            {
                m_rectTransform = GetComponent<RectTransform>();
                m_rectTransform.hasChanged = true;
            }

            m_rectTransformDimensionsChanged = true;
            
                       
            //Debug.Log("OnRectTransformDimensionsChange() called. New Width: " + m_rectTransform.rect.width + "  Height: " + m_rectTransform.rect.height);
        } 

        
        // Called just before the Canvas is rendered.
        void OnPreRenderCanvas()
        {
            //loopCountA = 0;
            //loopCountB = 0;
            //loopCountC = 0;
            //loopCountD = 0;
            //loopCountE = 0;
            
            //Debug.Log("OnPreRenderCanvas() called."); // Assigned Material is " + m_uiRenderer.GetMaterial().name); // isInputParsingRequired = " + isInputParsingRequired);                    
                      
            // Check if Transform has changed since last update.          
            if (m_rectTransform.hasChanged || m_marginsHaveChanged)
            {
                //m_layoutGroup.SetLayoutHorizontal();
                //Debug.Log("RectTransform has changed. Current Width: " + m_rectTransform.rect.width + " and  Height: " + m_rectTransform.rect.height);

               
				// If Dimension Changed or Margin (Regenerate the Mesh)              
                if (m_rectTransformDimensionsChanged || m_marginsHaveChanged)
                {                    
                    //Debug.Log("RectTransform Dimensions or Margins have changed.");
                    ComputeMarginSize();

                    if (m_marginsHaveChanged)
                        m_isScrollRegionSet = false;

                    m_rectTransformDimensionsChanged = false;                 
                    m_marginsHaveChanged = false;

                    havePropertiesChanged = true;                  
                }

                // Update Mask
                if (m_isMaskingEnabled)
                {
                    UpdateMask();
                }
            
                m_rectTransform.hasChanged = false;
                
                // We need to regenerate the mesh if the lossy scale has changed.
                if (m_rectTransform.lossyScale != m_lossyScale)
                {
                    havePropertiesChanged = true;
                    m_lossyScale = m_rectTransform.lossyScale;
                }

                if (m_isTextTruncated)
                {
                    //Debug.Log("Text was truncated.");                   
                    isInputParsingRequired = true;
                    m_isTextTruncated = false;
                    havePropertiesChanged = true;
                }                       
            }
                     

            if (havePropertiesChanged || m_fontAsset.propertiesChanged)
            {
                //Debug.Log("Properties have changed!"); // Assigned Material is:" + m_sharedMaterial); // New Text is: " + m_text + ".");                

                if (hasFontAssetChanged || m_fontAsset.propertiesChanged)
                {
                    //Debug.Log("Font Asset has changed. Loading new font asset."); 
                    
                    LoadFontAsset();

                    hasFontAssetChanged = false;

                    if (m_fontAsset == null || m_uiRenderer.GetMaterial() == null)
                        return;

                    m_fontAsset.propertiesChanged = false;
                }

                
                //if (m_isMaskingEnabled)
                //{                                                      
                //    UpdateMask();                  
                //}
                

                // Reparse Input if modified properties affect word wrapping.
                if (isInputParsingRequired)
                {
                    //Debug.Log("Reparsing Text.");
             
                    switch (m_inputSource)
                    {
                        case TextInputSources.Text:
                            StringToCharArray(m_text, ref m_char_buffer);
                            //isTextChanged = false;
                            break;
                        case TextInputSources.SetText:
                            SetTextArrayToCharArray(m_input_CharArray, ref m_char_buffer);
                            //isSetTextChanged = false;
                            break;
                        case TextInputSources.SetCharArray:
                            break;
                    }
                              
                    isInputParsingRequired = false;
                                                  
                }


                // Defer Mesh Generation if Layout Component is present
                if (IsRectTransformDriven && m_isCalculateSizeRequired)
                {
                    if (m_layoutController as UIBehaviour != null && (m_layoutController as UIBehaviour).enabled)
                    {
                        LayoutRebuilder.MarkLayoutForRebuild(m_rectTransform);
                        return;
                    }
                }



                // Reset Font min / max used with Auto-sizing
                if (m_enableAutoSizing)
                    m_fontSize = Mathf.Clamp(m_fontSize, m_fontSizeMin, m_fontSizeMax);

                m_maxFontSize = m_fontSizeMax;
                m_minFontSize = m_fontSizeMin;               
                m_lineSpacingDelta = 0;

                // Reset Preferred Width & Height
                //m_preferredWidth = 0;
                //m_preferredHeight = 0;    

                GenerateTextMesh();
                havePropertiesChanged = false;
                //isAlreadyScheduled = false;
            }
        
  
        }

        


        
        /// <summary>
        /// This is the main function that is responsible for creating / displaying the text.
        /// </summary>
        void GenerateTextMesh()
        {           
            //Debug.Log("GenerateTextMesh() called."); // Assigned Material is " + m_uiRenderer.GetMaterial().name); // IncludeForMasking " + this.m_IncludeForMasking); // and text is " + m_text);
            //Debug.Log(this.defaultMaterial.GetInstanceID() + "  " + m_sharedMaterial.GetInstanceID() + "  " + m_uiRenderer.GetMaterial().GetInstanceID());
            // Early exit if no font asset was assigned. This should not be needed since Arial SDF will be assigned by default.
            if (m_fontAsset.characterDictionary == null)
            {
                Debug.Log("Can't Generate Mesh! No Font Asset has been assigned to Object ID: " + this.GetInstanceID());
                return;
            }

            // Reset TextInfo
            if (m_textInfo != null)
            {
                m_textInfo.characterCount = 0;
                m_textInfo.lineCount = 0;
                m_textInfo.spaceCount = 0;
                m_textInfo.wordCount = 0;
                m_textInfo.minWidth = 0;
            }


            // Early exit if we don't have any Text to generate.          
            if (m_char_buffer == null || m_char_buffer.Length == 0 || m_char_buffer[0] == (char)0)
            {
                //Debug.Log("Early Out! No Text has been set.");
                if (m_uiVertices != null)
                {
                    m_uiRenderer.SetVertices(m_uiVertices, 0);
                }

                m_preferredWidth = 0;
                m_preferredHeight = 0;

                // This should only be called if there is a layout component attached
                if (IsRectTransformDriven) LayoutRebuilder.MarkLayoutForRebuild(m_rectTransform);
                return;
            }


            // Determine how many characters will be visible and make the necessary allocations (if needed).
            int totalCharacterCount = SetArraySizes(m_char_buffer);

            m_fontIndex = 0; // Will be used when support for using different font assets or sprites withint the same object will be added.
            m_fontAssetArray[m_fontIndex] = m_fontAsset;

            // Scale the font to approximately match the point size           
            m_fontScale = (m_fontSize / m_fontAssetArray[m_fontIndex].fontInfo.PointSize);
            //float baseScale = m_fontScale; // BaseScale keeps the character aligned vertically since <size=+000> results in font of different scale.
            m_maxFontScale = 0;
            float previousFontScale = 0;
            m_currentFontSize = m_fontSize;
            float fontSizeDelta = 0;

            int charCode = 0; // Holds the character code of the currently being processed character.
            //int prev_charCode = 0;
            bool isMissingCharacter; // Used to handle missing characters in the Font Atlas / Definition.

            //bool isLineTruncated = false;

            m_style = m_fontStyle; // Set the default style.
            m_lineJustification = m_textAlignment; // Sets the line justification mode to match editor alignment.

            // GetPadding to adjust the size of the mesh due to border thickness, softness, glow, etc...
            if (checkPaddingRequired)
            {
                checkPaddingRequired = false;
                m_padding = ShaderUtilities.GetPadding(m_uiRenderer.GetMaterial(), m_enableExtraPadding, m_isUsingBold);
                m_alignmentPadding = ShaderUtilities.GetFontExtent(m_sharedMaterial);
                m_isMaskingEnabled = ShaderUtilities.IsMaskingEnabled(m_sharedMaterial);
            }

            float style_padding = 0; // Extra padding required to accomodate Bold style.
            float xadvance_multiplier = 1; // Used to increase spacing between character when style is bold.

            m_baselineOffset = 0; // Used by subscript characters.

            bool beginUnderline = false;
            Vector3 underline_start = Vector3.zero; // Used to track where underline starts & ends.
            Vector3 underline_end = Vector3.zero;

            m_fontColor32 = m_fontColor;
            Color32 vertexColor;
            m_htmlColor = m_fontColor32;


            m_lineOffset = 0; // Amount of space between lines (font line spacing + m_linespacing).
			m_cSpacing = 0; // Amount of space added between characters as a result of the use of the <cspace> tag.
            float lineOffsetDelta = 0;
            m_xAdvance = 0; // Used to track the position of each character.
            m_maxXAdvance = 0;

            m_lineNumber = 0;
            m_characterCount = 0; // Total characters in the char[]
            m_visibleCharacterCount = 0; // # of visible characters.

            // Limit Line Length to whatever size fits all characters on a single line.
            //m_lineLength = m_lineLength > max_LineWrapLength ? max_LineWrapLength : m_lineLength;
            int firstCharacterOfLine = 0;
            int lastCharacterOfLine = 0;

            int ellipsisIndex = 0;
            //int truncateIndex = -1;
            //bool isLineTruncated = false;
            //int truncatedLine = 0;
            //m_isTextTruncated = false;
          
            m_rectTransform.GetLocalCorners(m_rectCorners); // m_textContainer.corners;
            //Debug.Log (corners [0] + "  " + corners [2]);
            Vector4 margins = m_margin; // _textContainer.margins;
            float marginWidth = m_marginWidth; // m_rectTransform.rect.width - margins.z - margins.x;
            float marginHeight = m_marginHeight; // m_rectTransform.rect.height - margins.y - margins.w;  

            m_preferredWidth = 0;
            m_preferredHeight = 0;
            //m_layoutWidth = 0;
            //m_layoutHeight = 0;

            // Initialize struct to track states of word wrapping
            m_SavedWordWrapState = new WordWrapState();
            m_SavedLineState = new WordWrapState();           
            int wrappingIndex = 0;

            // Need to initialize these Extents Structs
            m_meshExtents = new Extents(k_InfinityVector, -k_InfinityVector);
            if (m_textInfo.wordInfo == null)
                m_textInfo.wordInfo = new List<WordInfo>();
            else
                m_textInfo.wordInfo.Clear();

            if (m_textInfo.lineInfo == null) m_textInfo.lineInfo = new LineInfo[2];
            for (int i = 0; i < m_textInfo.lineInfo.Length; i++)
            {
                m_textInfo.lineInfo[i] = new LineInfo();
                m_textInfo.lineInfo[i].lineExtents = new Extents(k_InfinityVector, -k_InfinityVector);
                m_textInfo.lineInfo[i].ascender = -k_InfinityVector.x;
                m_textInfo.lineInfo[i].descender = k_InfinityVector.x;
            }

            // Tracking of the highest Ascender
            m_maxAscender = 0;
            m_maxDescender = 0;

            bool isLineOffsetAdjusted = false;

            int endTagIndex = 0;
            // Parse through Character buffer to read html tags and begin creating mesh.
            for (int i = 0; m_char_buffer[i] != 0; i++)
            {
                m_tabSpacing = -999;
                m_spacing = -999;
                charCode = m_char_buffer[i];

                //Debug.Log("i:" + i + "  Character [" + (char)charCode + "]");
                //loopCountE += 1;

                if (m_isRichText && charCode == 60)  // '<'
                {
                    // Check if Tag is valid. If valid, skip to the end of the validated tag.
                    if (ValidateHtmlTag(m_char_buffer, i + 1, out endTagIndex))
                    {
                        i = endTagIndex;

                        if (m_isRecalculateScaleRequired)
                        {
                            m_fontScale = m_currentFontSize / m_fontAssetArray[m_fontIndex].fontInfo.PointSize;
                            //isAffectingWordWrapping = true;
                            m_isRecalculateScaleRequired = false;
                        }

                        if (m_tabSpacing != -999)
                        {
                            // Move character to a fix position. Position expresses in characters (approximation).
                            m_xAdvance = m_tabSpacing * m_cached_Underline_GlyphInfo.width * m_fontScale;
                        }

                        if (m_spacing != -999)
                        {
                            m_xAdvance += m_spacing * m_fontScale * m_cached_Underline_GlyphInfo.width;
                        }

                        continue;
                    }
                }

                isMissingCharacter = false;


                // Check if we should be using a different font asset
                //if (m_fontIndex != 0)
                //{
                //    // Check if we need to load the new font asset
                //    if (m_fontAssetArray[m_fontIndex] == null)
                //    {
                //        Debug.Log("Loading secondary font asset.");
                //        m_fontAssetArray[m_fontIndex] = Resources.Load("Fonts & Materials/Bangers SDF", typeof(TextMeshProFont)) as TextMeshProFont;
                //        //m_sharedMaterials.Add(m_fontAssetArray[m_fontIndex].material);
                //        //m_renderer.sharedMaterials = new Material[] { m_sharedMaterial, m_fontAssetArray[m_fontIndex].material }; // m_sharedMaterials.ToArray();
                //    }
                //}               
                //Debug.Log("Char [" + (char)charCode + "] is using FontIndex: " + m_fontIndex);

              
              
                // Handle Font Styles like LowerCase, UpperCase and SmallCaps.
                #region Handling of LowerCase, UpperCase and SmallCaps Font Styles
                if ((m_style & FontStyles.UpperCase) == FontStyles.UpperCase)
                {
                    // If this character is lowercase, switch to uppercase.
                    if (char.IsLower((char)charCode))
                        charCode -= 32;

                }
                else if ((m_style & FontStyles.LowerCase) == FontStyles.LowerCase)
                {
                    // If this character is uppercase, switch to lowercase.
                    if (char.IsUpper((char)charCode))
                        charCode += 32;
                }
                else if ((m_fontStyle & FontStyles.SmallCaps) == FontStyles.SmallCaps || (m_style & FontStyles.SmallCaps) == FontStyles.SmallCaps)
                {
                    if (char.IsLower((char)charCode))
                    {
                        m_fontScale = m_currentFontSize * 0.8f / m_fontAssetArray[m_fontIndex].fontInfo.PointSize;
                        charCode -= 32;
                    }
                    else
                        m_fontScale = m_currentFontSize / m_fontAssetArray[m_fontIndex].fontInfo.PointSize;

                }
                #endregion


                // Look up Character Data from Dictionary and cache it.
                #region Look up Character Data
                m_fontAssetArray[m_fontIndex].characterDictionary.TryGetValue(charCode, out m_cached_GlyphInfo);
                if (m_cached_GlyphInfo == null)
                {
                    // Character wasn't found in the Dictionary.

                    // Check if Lowercase & Replace by Uppercase if possible                                   
                    if (char.IsLower((char)charCode))
                    {
                        if (m_fontAssetArray[m_fontIndex].characterDictionary.TryGetValue(charCode - 32, out m_cached_GlyphInfo))
                            charCode -= 32;
                    }
                    else if (char.IsUpper((char)charCode))
                    {
                        if (m_fontAssetArray[m_fontIndex].characterDictionary.TryGetValue(charCode + 32, out m_cached_GlyphInfo))
                            charCode += 32;
                    }

                    // Still don't have a replacement?
                    if (m_cached_GlyphInfo == null)
                    {
                        m_fontAssetArray[m_fontIndex].characterDictionary.TryGetValue(88, out m_cached_GlyphInfo);
                        if (m_cached_GlyphInfo != null)
                        {
                            Debug.LogWarning("Character with ASCII value of " + charCode + " was not found in the Font Asset Glyph Table.");
                            // Replace the missing character by X (if it is found)
                            charCode = 88;
                            isMissingCharacter = true;
                        }
                        else
                        {  // At this point the character isn't in the Dictionary, the replacement X isn't either so ...                         
                            Debug.LogWarning("Character with ASCII value of " + charCode + " was not found in the Font Asset Glyph Table.");
                            continue;
                        }
                    }
                }
                #endregion

                // Store some of the text object's information
                m_textInfo.characterInfo[m_characterCount].character = (char)charCode;
                m_textInfo.characterInfo[m_characterCount].color = m_htmlColor;
                m_textInfo.characterInfo[m_characterCount].style = m_style;
                m_textInfo.characterInfo[m_characterCount].index = (short)i;


                // Handle Kerning if Enabled.                 
                #region Handle Kerning
                if (m_enableKerning && m_characterCount >= 1)
                {
                    int prev_charCode = m_textInfo.characterInfo[m_characterCount - 1].character;
                    KerningPairKey keyValue = new KerningPairKey(prev_charCode, charCode);

                    KerningPair pair;

                    m_fontAssetArray[m_fontIndex].kerningDictionary.TryGetValue(keyValue.key, out pair);
                    if (pair != null)
                    {
                        m_xAdvance += pair.XadvanceOffset * m_fontScale;
                    }
                }
                #endregion


                // Set Padding based on selected font style
                #region Handle Style Padding
                if ((m_style & FontStyles.Bold) == FontStyles.Bold || (m_fontStyle & FontStyles.Bold) == FontStyles.Bold) // Checks for any combination of Bold Style.
                {                   
                    style_padding = m_fontAssetArray[m_fontIndex].BoldStyle * 2;
                    xadvance_multiplier = 1.07f; // Increase xAdvance for bold characters.         
                }
                else
                {
                    style_padding = m_fontAssetArray[m_fontIndex].NormalStyle * 2;
                    xadvance_multiplier = 1.0f;
                }
                #endregion Handle Style Padding

               
                // Setup Vertices for each character               
                Vector3 top_left = new Vector3(0 + m_xAdvance + ((m_cached_GlyphInfo.xOffset - m_padding - style_padding) * m_fontScale), (m_cached_GlyphInfo.yOffset + m_baselineOffset + m_padding) * m_fontScale - m_lineOffset, 0);
                Vector3 bottom_left = new Vector3(top_left.x, top_left.y - ((m_cached_GlyphInfo.height + m_padding * 2) * m_fontScale), 0);
                Vector3 top_right = new Vector3(bottom_left.x + ((m_cached_GlyphInfo.width + m_padding * 2 + style_padding * 2) * m_fontScale), top_left.y, 0);
                Vector3 bottom_right = new Vector3(top_right.x, bottom_left.y, 0);

                
                // Check if we need to Shear the rectangles for Italic styles
                #region Handle Italic & Shearing
                if ((m_style & FontStyles.Italic) == FontStyles.Italic || (m_fontStyle & FontStyles.Italic) == FontStyles.Italic)
                {
                    // Shift Top vertices forward by half (Shear Value * height of character) and Bottom vertices back by same amount. 
                    float shear_value = m_fontAssetArray[m_fontIndex].ItalicStyle * 0.01f;
                    Vector3 topShear = new Vector3(shear_value * ((m_cached_GlyphInfo.yOffset + m_padding + style_padding) * m_fontScale), 0, 0);
                    Vector3 bottomShear = new Vector3(shear_value * (((m_cached_GlyphInfo.yOffset - m_cached_GlyphInfo.height - m_padding - style_padding)) * m_fontScale), 0, 0);

                    top_left = top_left + topShear;
                    bottom_left = bottom_left + bottomShear;
                    top_right = top_right + topShear;
                    bottom_right = bottom_right + bottomShear;
                }
                #endregion Handle Italics & Shearing


                // Store postion of vertices for each character
                m_textInfo.characterInfo[m_characterCount].topLeft = top_left;
                m_textInfo.characterInfo[m_characterCount].bottomLeft = bottom_left;
                m_textInfo.characterInfo[m_characterCount].topRight = top_right;
                m_textInfo.characterInfo[m_characterCount].bottomRight = bottom_right;

                // Compute MaxAscender & MaxDescender which is used for AutoScaling & other type layout options
                float ascender = (m_fontAsset.fontInfo.Ascender + m_baselineOffset + m_alignmentPadding.y) * m_fontScale;
                if (m_lineNumber == 0) m_maxAscender = m_maxAscender > ascender ? m_maxAscender : ascender;

                //float descender = (m_fontAsset.fontInfo.Descender + m_baselineOffset + m_alignmentPadding.w) * m_fontScale - m_lineOffset;                  
                //m_maxDescender = m_maxDescender < descender ? m_maxDescender : descender;  

                // Set Characters to not visible by default.
                m_textInfo.characterInfo[m_characterCount].isVisible = false;


                // Setup Mesh for visible characters. ie. not a SPACE / LINEFEED / CARRIAGE RETURN.
                #region Handle Visible Characters
                if (charCode != 32 && charCode != 9 && charCode != 10 && charCode != 13)
                {
                    int index_X4 = m_visibleCharacterCount * 4;

                    m_textInfo.characterInfo[m_characterCount].isVisible = true;
                    m_textInfo.characterInfo[m_characterCount].vertexIndex = (short)(0 + index_X4);

                    // Vertices
                    m_uiVertices[0 + index_X4].position = m_textInfo.characterInfo[m_characterCount].bottomLeft;
                    m_uiVertices[1 + index_X4].position = m_textInfo.characterInfo[m_characterCount].topLeft;
                    m_uiVertices[2 + index_X4].position = m_textInfo.characterInfo[m_characterCount].topRight;
                    m_uiVertices[3 + index_X4].position = m_textInfo.characterInfo[m_characterCount].bottomRight;


                    //m_textInfo.characterInfo[character_Count].charNumber = (short)m_lineExtents[lineNumber].characterCount;                                      

                    // Used to adjust line spacing when larger fonts or the size tag is used.
                    if (m_baselineOffset == 0)
                        m_maxFontScale = Mathf.Max(m_maxFontScale, m_fontScale);



                    //Debug.Log("Char [" + (char)charCode + "] Width is " + (m_textInfo.characterInfo[m_characterCount].topRight.x - (m_padding - m_alignmentPadding.z) * m_fontScale) + "  Margin Width is " + marginWidth +
                    //    "  Vertex Padding: " + (m_padding * m_fontScale) + "  Alignment Padding: " + (m_alignmentPadding.z * m_fontScale) + "  Alignment Padding: " + (m_alignmentPadding.x * m_fontScale));

                    // Check if Character exceeds the width of the Text Container               
                    #region Check for Characters Exceeding Width of Text Container
                    if (m_textInfo.characterInfo[m_characterCount].topRight.x - (m_padding - m_alignmentPadding.z) * m_fontScale > marginWidth)
                    {

                        // Word Wrapping
                        #region Handle Word Wrapping
                        if (enableWordWrapping)
                        {

                            ellipsisIndex = m_characterCount - 1;

                            if (wrappingIndex == m_SavedWordWrapState.previous_WordBreak) // && m_isCharacterWrappingEnabled == false)
                            {
                                // Word wrapping is no longer possible. Shrink size of text if auto-sizing is enabled.
                                if (m_enableAutoSizing && m_fontSize > m_fontSizeMin)
                                {
                                    //Debug.Log("Reducing Font Size.");
                                    m_maxFontSize = m_fontSize;

                                    float delta = Mathf.Max((m_fontSize - m_minFontSize) / 2, 0.01f);
                                    m_fontSize -= delta;
                                    //Debug.Log("Delta = " + delta);
                                    m_fontSize = Mathf.Max(m_fontSize, m_fontSizeMin);

                                    //loopCountC += 1;
                                    //Debug.Log("Count C: " + loopCountC + " Min: " + m_minFontSize + "  Max: " + m_maxFontSize + "  Size: " + m_fontSize + " Delta: " + fontSizeDelta);
                                    //m_fontSize -= 1f; // 0.25f;
                                    //m_maxFontSize = m_fontSize;
                                    //Debug.Log("Decreasing Width to " + m_fontSize);

                                   
                                    GenerateTextMesh();
                                    return;
                                }

                                // Word wrapping is no longer possible, now breaking up individual words.
                                if (m_isCharacterWrappingEnabled == false)
                                {
                                    m_isCharacterWrappingEnabled = true; // Should add a check to make sure this mode is available.
                                    //Debug.Log("Enabling Character Wrapping.");
                                    
                                    GenerateTextMesh();
                                    return;
                                }

                                /*
                                if (isAffectingWordWrapping)
                                {
                                    m_textContainer.size = new Vector2(Mathf.Round((top_right.x + margins.x + margins.z) * 100 + 0.5f) / 100f, m_textContainer.rect.height);                              
                                    GenerateTextMesh();
                                    isAffectingWordWrapping = false;                              
                                }
                            
                                else
                                {                                                                                                                        
                                    //Debug.Log("Text can no longer be reduced in size. Min font size reached.");
                                
                                    //m_textContainer.size = new Vector2(Mathf.Round((top_right.x + margins.x + margins.z) * 100 + 0.5f) / 100f, m_textContainer.rect.height);
                                    //GenerateTextMesh();
                                }
                                */
                                //m_isCharacterWrappingEnabled = true;
                                //Debug.Log("Line #" + lineNumber + " Character [" + (char)charCode + "] cannot be wrapped."); // WrappingIndex: " + wrappingIndex + "  Saved Index: " + m_SaveWordWrapState.previous_WordBreak);                                                                            
                                return;
                            }


                            // Restore to previously stored state of last valid (space character or linefeed)
                            i = RestoreWordWrappingState(ref m_SavedWordWrapState);
                            wrappingIndex = i;  // Used to dectect when line length can no longer be reduced.

                            // Check if we need to Adjust LineOffset & Restore State to the start of the line.                            
                            if (m_lineNumber > 0 && m_maxFontScale != 0 && m_maxFontScale != previousFontScale && !isLineOffsetAdjusted)
                            {
                                // Compute Offset
                                float gap = m_fontAssetArray[m_fontIndex].fontInfo.LineHeight - (m_fontAssetArray[m_fontIndex].fontInfo.Ascender - m_fontAssetArray[m_fontIndex].fontInfo.Descender);                             
                                float offsetDelta = (m_fontAssetArray[m_fontIndex].fontInfo.Ascender + m_lineSpacing + m_paragraphSpacing + gap + m_lineSpacingDelta) * m_maxFontScale - (m_fontAssetArray[m_fontIndex].fontInfo.Descender - gap) * previousFontScale;
                                m_lineOffset += offsetDelta - lineOffsetDelta;
                                AdjustLineOffset(firstCharacterOfLine, lastCharacterOfLine, offsetDelta - lineOffsetDelta);                                  
                            }


                            // Calculate lineAscender & make sure if last character is superscript or subscript that we check that as well.
                            float lineAscender = (m_fontAsset.fontInfo.Ascender + m_alignmentPadding.y) * m_maxFontScale - m_lineOffset;
                            float lineAscender2 = (m_fontAsset.fontInfo.Ascender + m_baselineOffset + m_alignmentPadding.y) * m_fontScale - m_lineOffset;
                            lineAscender = lineAscender > lineAscender2 ? lineAscender : lineAscender2;

                            // Calculate lineDescender & make sure if last character is superscript or subscript that we check that as well.
                            float lineDescender = (m_fontAsset.fontInfo.Descender + m_alignmentPadding.w) * m_maxFontScale - m_lineOffset;
                            float lineDescender2 = (m_fontAsset.fontInfo.Descender + m_baselineOffset + m_alignmentPadding.w) * m_fontScale - m_lineOffset;
                            lineDescender = lineDescender < lineDescender2 ? lineDescender : lineDescender2;

                            m_maxDescender = m_maxDescender < lineDescender ? m_maxDescender : lineDescender;


                            // Track & Store lineInfo for the new line                           
                            m_textInfo.lineInfo[m_lineNumber].firstCharacterIndex = firstCharacterOfLine; // Need new variable to track this
                            m_textInfo.lineInfo[m_lineNumber].lastCharacterIndex = m_characterCount - 1 > 0 ? m_characterCount - 1 : 1;  
                            firstCharacterOfLine = m_characterCount; // Store first character for the next line.

                            m_textInfo.lineInfo[m_lineNumber].lineExtents.min = new Vector2(m_textInfo.characterInfo[m_textInfo.lineInfo[m_lineNumber].firstCharacterIndex].bottomLeft.x, lineDescender);
                            m_textInfo.lineInfo[m_lineNumber].lineExtents.max = new Vector2(m_textInfo.characterInfo[m_textInfo.lineInfo[m_lineNumber].lastCharacterIndex - 1].topRight.x, lineAscender);
                            m_textInfo.lineInfo[m_lineNumber].lineLength = m_textInfo.lineInfo[m_lineNumber].lineExtents.max.x - m_padding * m_maxFontScale;

                            // Compute Preferred Width & Height                                                     
                            m_preferredWidth += m_xAdvance; // m_textInfo.characterInfo[m_textInfo.lineInfo[m_lineNumber].lastCharacterIndex].topRight.x - m_textInfo.characterInfo[m_textInfo.lineInfo[m_lineNumber].firstCharacterIndex].bottomLeft.x;
                            if (m_enableWordWrapping)
                                m_preferredHeight = m_maxAscender - m_maxDescender;
                            else
                                m_preferredHeight = Mathf.Max(m_preferredHeight, lineAscender - lineDescender);
                           
                            //Debug.Log("LineInfo for line # " + (m_lineNumber) + " First character [" + m_textInfo.characterInfo[m_textInfo.lineInfo[m_lineNumber].firstCharacterIndex].character + "] at Index: " + m_textInfo.lineInfo[m_lineNumber].firstCharacterIndex +
                            //                                                    " Last character [" + m_textInfo.characterInfo[m_textInfo.lineInfo[m_lineNumber].lastCharacterIndex].character + "] at index: " + m_textInfo.lineInfo[m_lineNumber].lastCharacterIndex +
                            //                                                    " Character Count of " + m_textInfo.lineInfo[m_lineNumber].characterCount + " Line Lenght of " + m_textInfo.lineInfo[m_lineNumber].lineLength +
                            //                                                    "  MinX: " + m_textInfo.lineInfo[m_lineNumber].lineExtents.min.x + "  MinY: " + m_textInfo.lineInfo[m_lineNumber].lineExtents.min.y +
                            //                                                    "  MaxX: " + m_textInfo.lineInfo[m_lineNumber].lineExtents.max.x + "  MaxY: " + m_textInfo.lineInfo[m_lineNumber].lineExtents.max.y +
                            //                                                    "  Line Ascender: " + lineAscender + "  Line Descender: " + lineDescender);

                            // Store the state of the line before starting on the new line.
                            SaveWordWrappingState(ref m_SavedLineState, i);
                            
                            m_lineNumber += 1;
                            // Check to make sure Array is large enough to hold a new line.
                            if (m_lineNumber >= m_textInfo.lineInfo.Length)
                                ResizeLineExtents(m_lineNumber);

                            // Apply Line Spacing based on scale of the last character of the line.
                            lineOffsetDelta = (m_fontAssetArray[m_fontIndex].fontInfo.LineHeight + m_lineSpacing + m_lineSpacingDelta) * m_fontScale;
                            m_lineOffset += lineOffsetDelta;

                            previousFontScale = m_fontScale;
                            m_xAdvance = 0;
                            m_maxFontScale = 0;

                            // Handle Page #
                            //if (m_lineNumber % 5 == 0)
                            //    m_lineOffset = 0;

                            continue;
                        }
                        #endregion End Word Wrapping


                        // Text Auto-Sizing (text exceeding Width of container. 
                        #region Handle Text Auto-Sizing
                        if (m_enableAutoSizing && m_fontSize > m_fontSizeMin)
                        {
                            m_maxFontSize = m_fontSize;

                            float delta = Mathf.Max((m_fontSize - m_minFontSize) / 2, 0.01f);
                            m_fontSize -= delta;

                            m_fontSize = Mathf.Max(m_fontSize, m_fontSizeMin);

                            //Debug.Log("Decreasing Width to " + m_fontSize);
                            //loopCountA += 1;
                            //Debug.Log("Count A: " + loopCountA + " Min: " + m_minFontSize + "  Max: " + m_maxFontSize + "  Size: " + m_fontSize + " Delta: " + fontSizeDelta);
                          
                            GenerateTextMesh();
                            return;
                        }
                        #endregion End Text Auto-Sizing


                        // Handle Text Overflow
                        #region Handle Text Overflow
                        switch (m_overflowMode)
                        {
                            case TextOverflowModes.Overflow:
                                if (m_isMaskingEnabled)
                                    DisableMasking();

                                break;
                            case TextOverflowModes.Ellipsis:
                                if (m_isMaskingEnabled)
                                    DisableMasking();

                                m_isTextTruncated = true;
                                
                                if (i < 1) break;

                                m_char_buffer[i - 1] = 8230;
                                m_char_buffer[i] = (char)0;

                                GenerateTextMesh();                           
                                return;
                            case TextOverflowModes.Masking:
                                if (!m_isMaskingEnabled)
                                    EnableMasking();
                                break;
                            case TextOverflowModes.ScrollRect:
                                if (!m_isMaskingEnabled)
                                    EnableMasking();
                                break;
                            case TextOverflowModes.Truncate:
                                if (m_isMaskingEnabled)
                                    DisableMasking();

                                m_textInfo.characterInfo[m_characterCount].isVisible = false;
                                m_visibleCharacterCount -= 1;
                                break;
                        }
                        #endregion End Text Overflow

                    }
                    #endregion End Check for Characters Exceeding Width of Text Container


                    // Determine what color gets assigned to vertex.     
                    #region Handle Vertex Colors
                    if (isMissingCharacter)
                        vertexColor = Color.red;
                    else if (m_overrideHtmlColors)
                        vertexColor = m_fontColor32;
                    else
                        vertexColor = m_htmlColor;


                    // Set Alpha for Shader to render font as normal or bold. (Alpha channel is being used to let the shader know if the character is bold or normal).
                    if ((m_style & FontStyles.Bold) == FontStyles.Bold || (m_fontStyle & FontStyles.Bold) == FontStyles.Bold)
                    {
                        vertexColor.a = m_fontColor32.a < vertexColor.a ? (byte)(m_fontColor32.a >> 1) : (byte)(vertexColor.a >> 1);
                        vertexColor.a += 128;
                    }
                    else
                    {
                        vertexColor.a = m_fontColor32.a < vertexColor.a ? (byte)(m_fontColor32.a >> 1) : (byte)(vertexColor.a >> 1);
                    }


                    // Vertex Colors
                    if (!m_enableVertexGradient)
                    {
                        m_uiVertices[0 + index_X4].color = vertexColor;
                        m_uiVertices[1 + index_X4].color = vertexColor;
                        m_uiVertices[2 + index_X4].color = vertexColor;
                        m_uiVertices[3 + index_X4].color = vertexColor;
                    }
                    else
                    {
                        if (!m_overrideHtmlColors && !m_htmlColor.CompareRGB(m_fontColor32))
                        {
                            m_uiVertices[0 + index_X4].color = vertexColor;
                            m_uiVertices[1 + index_X4].color = vertexColor;
                            m_uiVertices[2 + index_X4].color = vertexColor;
                            m_uiVertices[3 + index_X4].color = vertexColor;
                        }
                        else
                        {
                            m_uiVertices[0 + index_X4].color = m_fontColorGradient.bottomLeft;
                            m_uiVertices[1 + index_X4].color = m_fontColorGradient.topLeft;
                            m_uiVertices[2 + index_X4].color = m_fontColorGradient.topRight;
                            m_uiVertices[3 + index_X4].color = m_fontColorGradient.bottomRight;
                        }

                        m_uiVertices[0 + index_X4].color.a = vertexColor.a;
                        m_uiVertices[1 + index_X4].color.a = vertexColor.a;
                        m_uiVertices[2 + index_X4].color.a = vertexColor.a;
                        m_uiVertices[3 + index_X4].color.a = vertexColor.a;
                    }
                    #endregion Handle Vertex Colors


                    // Apply style_padding only if this is a SDF Shader.
                    if (!m_sharedMaterial.HasProperty(ShaderUtilities.ID_WeightNormal))
                        style_padding = 0;


                    // Setup UVs for the Mesh
                    #region Setup UVs
                    Vector2 uv0 = new Vector2((m_cached_GlyphInfo.x - m_padding - style_padding) / m_fontAssetArray[m_fontIndex].fontInfo.AtlasWidth, 1 - (m_cached_GlyphInfo.y + m_padding + style_padding + m_cached_GlyphInfo.height) / m_fontAssetArray[m_fontIndex].fontInfo.AtlasHeight);  // bottom left
                    Vector2 uv1 = new Vector2(uv0.x, 1 - (m_cached_GlyphInfo.y - m_padding - style_padding) / m_fontAssetArray[m_fontIndex].fontInfo.AtlasHeight);  // top left
                    Vector2 uv2 = new Vector2((m_cached_GlyphInfo.x + m_padding + style_padding + m_cached_GlyphInfo.width) / m_fontAssetArray[m_fontIndex].fontInfo.AtlasWidth, uv0.y); // bottom right
                    Vector2 uv3 = new Vector2(uv2.x, uv1.y); // top right

                    // UV
                    m_uiVertices[0 + index_X4].uv0 = uv0; // BL
                    m_uiVertices[1 + index_X4].uv0 = uv1; // TL
                    m_uiVertices[2 + index_X4].uv0 = uv3; // TR
                    m_uiVertices[3 + index_X4].uv0 = uv2; // BR
                    #endregion Setup UVs

                    // Normal
                    #region Setup Normals & Tangents
                    Vector3 normal = new Vector3(0, 0, -1);
                    m_uiVertices[0 + index_X4].normal = normal;
                    m_uiVertices[1 + index_X4].normal = normal;
                    m_uiVertices[2 + index_X4].normal = normal;
                    m_uiVertices[3 + index_X4].normal = normal;

                    // Tangents
                    Vector4 tangent = new Vector4(-1, 0, 0, 1);
                    m_uiVertices[0 + index_X4].tangent = tangent;
                    m_uiVertices[1 + index_X4].tangent = tangent;
                    m_uiVertices[2 + index_X4].tangent = tangent;
                    m_uiVertices[3 + index_X4].tangent = tangent;
                    #endregion end Normals & Tangents

                    // Determine the bounds of the Mesh.                       
                    m_meshExtents.min = new Vector2(Mathf.Min(m_meshExtents.min.x, m_textInfo.characterInfo[m_characterCount].bottomLeft.x), Mathf.Min(m_meshExtents.min.y, m_textInfo.characterInfo[m_characterCount].bottomLeft.y));
                    m_meshExtents.max = new Vector2(Mathf.Max(m_meshExtents.max.x, m_textInfo.characterInfo[m_characterCount].topRight.x), Mathf.Max(m_meshExtents.max.y, m_textInfo.characterInfo[m_characterCount].topLeft.y));

                    m_visibleCharacterCount += 1;
                    lastCharacterOfLine = m_characterCount;
                }
                else
                {   // This is a Space, Tab, LineFeed or Carriage Return              

                    // Track # of spaces per line which is used for line justification.
                    if (charCode == 9 || charCode == 32)
                    {
                        m_textInfo.lineInfo[m_lineNumber].spaceCount += 1;
                        m_textInfo.spaceCount += 1;
                    }
                }
                #endregion Handle Visible Characters


                // Store Rectangle positions for each Character.                
                #region Store Character Data
                m_textInfo.characterInfo[m_characterCount].lineNumber = (short)m_lineNumber;
                m_textInfo.lineInfo[m_lineNumber].characterCount += 1;
                if (charCode != 10 && charCode != 13)
                    m_textInfo.lineInfo[m_lineNumber].alignment = m_lineJustification;  
                #endregion Store Character Data


                // Handle Tabulation Stops. Tab stops at every 25% of Font Size.
                #region xAdvance & Tabulation
                if (charCode == 9)
                {
                    m_xAdvance = (int)(m_xAdvance / (m_fontSize * 0.25f) + 1) * (m_fontSize * 0.25f);
                }
                else
                    m_xAdvance += (m_cached_GlyphInfo.xAdvance * xadvance_multiplier * m_fontScale) + m_characterSpacing + m_cSpacing;                                                       
                #endregion Tabulation & Stops

               
                // Handle Carriage Return
                #region Carriage Return
                if (charCode == 13)
                {
                    m_maxXAdvance = Mathf.Max(m_maxXAdvance, m_preferredWidth + m_xAdvance + (m_alignmentPadding.z * m_fontScale));
                    m_preferredWidth = 0;
                    m_xAdvance = 0;                   
                }
                #endregion Carriage Return


                //Debug.Log("Char [" + (char)charCode + "] with ASCII (" + charCode + ") cummulative xAdvance: " + m_xAdvance);
                
                // Handle Line Spacing Adjustments + Word Wrapping & special case for last line.
                #region Check for Line Feed and Last Character
                if (charCode == 10 || m_characterCount == totalCharacterCount - 1)
                {
                    //Debug.Log("Line # " + m_lineNumber + "  Current Character is [" + (char)charCode + "] with ASC value of " + charCode);

                    // Handle Line Spacing Changes
                    if (m_lineNumber > 0 && m_maxFontScale != 0 && m_maxFontScale != previousFontScale && !isLineOffsetAdjusted)
                    {
                        float gap = m_fontAssetArray[m_fontIndex].fontInfo.LineHeight - (m_fontAssetArray[m_fontIndex].fontInfo.Ascender - m_fontAssetArray[m_fontIndex].fontInfo.Descender);                       
                        float offsetDelta = (m_fontAssetArray[m_fontIndex].fontInfo.Ascender + m_lineSpacing + m_paragraphSpacing + gap + m_lineSpacingDelta) * m_maxFontScale - (m_fontAssetArray[m_fontIndex].fontInfo.Descender - gap) * previousFontScale;
                        m_lineOffset += offsetDelta - lineOffsetDelta;
                        AdjustLineOffset(firstCharacterOfLine, lastCharacterOfLine, offsetDelta - lineOffsetDelta);    
                    }


                    // Calculate lineAscender & make sure if last character is superscript or subscript that we check that as well.
                    float lineAscender = (m_fontAsset.fontInfo.Ascender + m_alignmentPadding.y) * m_maxFontScale - m_lineOffset;
                    float lineAscender2 = (m_fontAsset.fontInfo.Ascender + m_baselineOffset + m_alignmentPadding.y) * m_fontScale - m_lineOffset;
                    lineAscender = lineAscender > lineAscender2 ? lineAscender : lineAscender2;

                    // Calculate lineDescender & make sure if last character is superscript or subscript that we check that as well.
                    float lineDescender = (m_fontAsset.fontInfo.Descender + m_alignmentPadding.w) * m_maxFontScale - m_lineOffset;
                    float lineDescender2 = (m_fontAsset.fontInfo.Descender + m_baselineOffset + m_alignmentPadding.w) * m_fontScale - m_lineOffset;
                    lineDescender = lineDescender < lineDescender2 ? lineDescender : lineDescender2;

                    m_maxDescender = m_maxDescender < lineDescender ? m_maxDescender : lineDescender;


                    // Save Line Information
                    m_textInfo.lineInfo[m_lineNumber].firstCharacterIndex = firstCharacterOfLine; // Need new variable to track this
                    m_textInfo.lineInfo[m_lineNumber].lastCharacterIndex = charCode == 10 ? lastCharacterOfLine + 1 : lastCharacterOfLine;  
                    firstCharacterOfLine = m_characterCount + 1;

                    m_textInfo.lineInfo[m_lineNumber].lineExtents.min = new Vector2(m_textInfo.characterInfo[m_textInfo.lineInfo[m_lineNumber].firstCharacterIndex].bottomLeft.x, lineDescender);
                    m_textInfo.lineInfo[m_lineNumber].lineExtents.max = new Vector2(m_textInfo.characterInfo[m_textInfo.lineInfo[m_lineNumber].lastCharacterIndex - (charCode == 10 ? 1 : 0)].topRight.x, lineAscender);                  
                    m_textInfo.lineInfo[m_lineNumber].lineLength = m_textInfo.lineInfo[m_lineNumber].lineExtents.max.x - (m_padding * m_maxFontScale);


                    // Store PreferredWidth paying attention to linefeed and last character of text.
                    if (charCode == 10 && m_characterCount != totalCharacterCount - 1)
                    {
                        m_maxXAdvance = Mathf.Max(m_maxXAdvance, m_preferredWidth + m_xAdvance + (m_alignmentPadding.z * m_fontScale));                        
                        m_preferredWidth = 0;
                    }
                    else
                        m_preferredWidth = Mathf.Max(m_maxXAdvance, m_preferredWidth + m_xAdvance + (m_alignmentPadding.z * m_fontScale));

                    //Debug.Log("Line # " + m_lineNumber + " XAdance is " +  (m_preferredWidth + m_xAdvance + (m_alignmentPadding.z * m_fontScale)) + "  Max XAdvance: " + m_maxXAdvance);

                    //m_preferredWidth += m_xAdvance + (m_alignmentPadding.z * m_fontScale); 
                    if (m_enableWordWrapping)
                        m_preferredHeight = m_maxAscender - m_maxDescender;
                    else
                        m_preferredHeight = Mathf.Max(m_preferredHeight, lineAscender - lineDescender);

                    //Debug.Log("LineInfo for line # " + (m_lineNumber) + " First character [" + m_textInfo.characterInfo[m_textInfo.lineInfo[m_lineNumber].firstCharacterIndex].character + "] at Index: " + m_textInfo.lineInfo[m_lineNumber].firstCharacterIndex +
                    //                                                    " Last character [" + m_textInfo.characterInfo[m_textInfo.lineInfo[m_lineNumber].lastCharacterIndex].character + "] at index: " + m_textInfo.lineInfo[m_lineNumber].lastCharacterIndex +
                    //                                                    " Character Count of " + m_textInfo.lineInfo[m_lineNumber].characterCount + " Line Lenght of " + m_textInfo.lineInfo[m_lineNumber].lineLength +
                    //                                                    "  MinX: " + m_textInfo.lineInfo[m_lineNumber].lineExtents.min.x + "  MinY: " + m_textInfo.lineInfo[m_lineNumber].lineExtents.min.y +
                    //                                                    "  MaxX: " + m_textInfo.lineInfo[m_lineNumber].lineExtents.max.x + "  MaxY: " + m_textInfo.lineInfo[m_lineNumber].lineExtents.max.y +
                    //                                                    "  Line Ascender: " + lineAscender + "  Line Descender: " + lineDescender);

                    // Add new line if not last lines or character.
                    if (charCode == 10)
                    {
                        // Store the state of the line before starting on the new line.
                        SaveWordWrappingState(ref m_SavedLineState, i);
                        
                        m_lineNumber += 1;
                        // Check to make sure Array is large enough to hold a new line.
                        if (m_lineNumber >= m_textInfo.lineInfo.Length)
                            ResizeLineExtents(m_lineNumber);

                        // Apply Line Spacing based on scale of the last character of the line.                  
                        lineOffsetDelta = (m_fontAssetArray[m_fontIndex].fontInfo.LineHeight + m_paragraphSpacing + m_lineSpacing + m_lineSpacingDelta) * m_fontScale;
                        m_lineOffset += lineOffsetDelta;

                        previousFontScale = m_fontScale;
                        m_maxFontScale = 0;
                        m_xAdvance = 0;

                        // Check for page #
                        //if (m_lineNumber % 5 == 0)
                        //    m_lineOffset = 0;
                    }
                }
                #endregion Check for Linefeed or Last Character


                // Check if text Exceeds the vertical bounds of the margin area.
                #region Check Vertical Bounds & Auto-Sizing
                if (m_maxAscender - m_maxDescender + (m_alignmentPadding.w * 2 * m_fontScale) > marginHeight)
                {
                    //Debug.Log((m_maxAscender - m_maxDescender) + "  " + marginHeight);
                    //Debug.Log("Character [" + (char)charCode + "] at Index: " + m_characterCount + " has exceeded the Height of the text container. Max Ascender: " + m_maxAscender + "  Max Descender: " + m_maxDescender + "  Margin Height: " + marginHeight + " Bottom Left: " + bottom_left.y);                                              

                    // Handle Linespacing adjustments
                    #region Line Spacing Adjustments                    
                    if (m_enableAutoSizing && m_lineSpacingDelta > m_lineSpacingMax)
                    {                       
                        m_lineSpacingDelta -= 1;
                        GenerateTextMesh();
                        return;
                    }
                    #endregion

                    // Handle Text Auto-sizing resulting from text exceeding vertical bounds.
                    #region Text Auto-Sizing (Text greater than verical bounds)
                    if (m_enableAutoSizing && m_fontSize > m_fontSizeMin)
                    {
                        m_maxFontSize = m_fontSize;

                        float delta = Mathf.Max((m_fontSize - m_minFontSize) / 2, 0.01f);
                        //if (delta == 0.01f) return;

                        m_fontSize -= delta;

                        m_fontSize = Mathf.Max(m_fontSize, m_fontSizeMin);
                        //Debug.Log("Decreasing Height to " + m_fontSize);
                        //loopCountB += 1;
                        //Debug.Log("Count B: " + loopCountB + " Min: " + m_minFontSize + "  Max: " + m_maxFontSize + "  Size: " + m_fontSize + " Delta: " + (m_maxFontSize - m_minFontSize).ToString("f4"));
                      
                        GenerateTextMesh();
                        return;
                    }
                    #endregion Text Auto-Sizing

                    // Handle Text Overflow
                    #region Text Overflow
                    switch (m_overflowMode)
                    {
                        case TextOverflowModes.Overflow:
                            if (m_isMaskingEnabled)
                                DisableMasking();

                            break;
                        case TextOverflowModes.Ellipsis:
                            if (m_isMaskingEnabled)
                                DisableMasking();

                            if (m_lineNumber > 0)
                            {
                                m_char_buffer[m_textInfo.characterInfo[ellipsisIndex].index] = 8230;
                                m_char_buffer[m_textInfo.characterInfo[ellipsisIndex].index + 1] = (char)0;
                                GenerateTextMesh();
                                m_isTextTruncated = true;
                                return;
                            }
                            else
                            {
                                m_char_buffer[0] = (char)0;
                                GenerateTextMesh();
                                m_isTextTruncated = true;
                                return;
                            }
                        case TextOverflowModes.Masking:
                            if (!m_isMaskingEnabled)
                                EnableMasking();
                            break;
                        case TextOverflowModes.ScrollRect:
                            if (!m_isMaskingEnabled)
                                EnableMasking();
                            break;
                        case TextOverflowModes.Truncate:
                            if (m_isMaskingEnabled)
                                DisableMasking();

                            if (m_lineNumber > 0)
                            {
                                m_char_buffer[m_textInfo.characterInfo[ellipsisIndex].index + 1] = (char)0;
                                GenerateTextMesh();
                                m_isTextTruncated = true;
                                return;
                            }
                            else
                            {
                                m_char_buffer[0] = (char)0;
                                GenerateTextMesh();
                                m_isTextTruncated = true;
                                return;
                            }
                        //case TextOverflowModes.Pages:
                        //    if (m_isMaskingEnabled)
                        //        DisableMasking();

                        //    // Go back to previous line and re-layout 
                        //    i = RestoreWordWrappingState(ref m_SavedLineState);
                        //    m_xAdvance = 0;
                        //    m_lineOffset = 0;
                        //    continue;

                        //    //break;

                    }
                    #endregion End Text Overflow

                }
                #endregion Check Vertical Bounds

                //// Store Rectangle positions for each Character. 
                #region Save CharacterInfo for the current character.
                m_textInfo.characterInfo[m_characterCount].baseLine = m_textInfo.characterInfo[m_characterCount].topRight.y - (m_cached_GlyphInfo.yOffset + m_padding) * m_fontScale;
                m_textInfo.characterInfo[m_characterCount].topLine = m_textInfo.characterInfo[m_characterCount].baseLine + (m_fontAssetArray[m_fontIndex].fontInfo.Ascender + m_alignmentPadding.y) * m_fontScale; // Ascender              
                m_textInfo.characterInfo[m_characterCount].bottomLine = m_textInfo.characterInfo[m_characterCount].baseLine + (m_fontAssetArray[m_fontIndex].fontInfo.Descender - m_alignmentPadding.w) * m_fontScale; // Descender          
                m_textInfo.characterInfo[m_characterCount].padding = m_padding * m_fontScale;
                m_textInfo.characterInfo[m_characterCount].aspectRatio = m_cached_GlyphInfo.width / m_cached_GlyphInfo.height;
                m_textInfo.characterInfo[m_characterCount].scale = m_fontScale;
                #endregion Saving CharacterInfo

                           
                // Save State of Mesh Creation forhandling of Word Wrapping
                #region Save Word Wrapping State
                if (m_enableWordWrapping)
                {
                    //char c;
                    //m_fontAsset.lineBreakingInfo.leadingCharacters.ContainsKey(charCode);

                    //Debug.Log((char)charCode + "  " +  m_fontAsset.lineBreakingInfo.leadingCharacters.ContainsKey(charCode));

                    if (m_isCharacterWrappingEnabled == false && (charCode == 9 || charCode == 32))
                    {
                        // We store the state of numerous variables for the most recent Space, LineFeed or Carriage Return to enable them to be restored 
                        // for Word Wrapping.
                        SaveWordWrappingState(ref m_SavedWordWrapState, i);
                        //Debug.Log("Storing Character [" + (char)charCode + "] at Index: " + i);  
                    }
                    else if (m_isCharacterWrappingEnabled == true && m_characterCount < totalCharacterCount - 1
                           && m_fontAsset.lineBreakingInfo.leadingCharacters.ContainsKey(charCode) == false
                           && m_fontAsset.lineBreakingInfo.followingCharacters.ContainsKey(m_VisibleCharacters[m_characterCount + 1]) == false
                           && (charCode < 48 || charCode > 57) && charCode != 44)
                    {
                        //Debug.Log("Storing Character [" + (char)charCode + "] at Index: " + i);
                        SaveWordWrappingState(ref m_SavedWordWrapState, i);
                    }
                }
                #endregion Save Word Wrapping State

                m_characterCount += 1;
            }


            // Check Auto Sizing and increase font size to fill text container.
            #region Check Auto-Sizing (Upper Font Size Bounds)
            fontSizeDelta = m_maxFontSize - m_minFontSize;
            if (m_enableAutoSizing && fontSizeDelta > 0.25f && m_fontSize < m_fontSizeMax)
            {
                m_minFontSize = m_fontSize;
                m_fontSize = m_fontSize + (m_maxFontSize - m_fontSize) / 2;

                //loopCountD += 1;
                //Debug.Log("Count D: " + loopCountD + " Min: " + m_minFontSize + "  Max: " + m_maxFontSize + "  Size: " + m_fontSize + " Delta: " + fontSizeDelta);               

                m_fontSize = Mathf.Min(m_fontSize, m_fontSizeMax);

                GenerateTextMesh();
                return;
            }
            #endregion End Auto-sizing Check



            // Add Termination Character to textMeshCharacterInfo which is used by the Advanced Layout Component.     
            if (m_characterCount < m_textInfo.characterInfo.Length)
                m_textInfo.characterInfo[m_characterCount].character = (char)0;


            if (m_renderMode == TextRenderFlags.GetPreferredSizes)
                return;

            // DEBUG & PERFORMANCE CHECKS (0.006ms)
            //m_StopWatch.Stop();          


            // If there are no visible characters... no need to continue
            if (m_visibleCharacterCount == 0)
            {
                if (m_uiVertices != null)
                {
                    m_uiRenderer.SetVertices(m_uiVertices, 0);
                }
                return;
            }


            int last_vert_index = m_visibleCharacterCount * 4;
            // Partial clear of the vertices array to mark unused vertices as degenerate.
            Array.Clear(m_uiVertices, last_vert_index, m_uiVertices.Length - last_vert_index);


            // Handle Text Alignment
            #region Text Alignment          
            switch (m_textAlignment)
            {
                // Top Vertically
                case TextAlignmentOptions.Top:
                case TextAlignmentOptions.TopLeft:
                case TextAlignmentOptions.TopJustified:
                case TextAlignmentOptions.TopRight:
                    m_anchorOffset = m_rectCorners[1] + new Vector3(0 + margins.x, 0 - m_maxAscender - margins.y, 0);
                    break;

                // Middle Vertically
                case TextAlignmentOptions.Left:
                case TextAlignmentOptions.Right:
                case TextAlignmentOptions.Center:
                case TextAlignmentOptions.Justified:
                    m_anchorOffset = (m_rectCorners[0] + m_rectCorners[1]) / 2 + new Vector3(0 + margins.x, 0 - (m_maxAscender + margins.y + m_maxDescender - margins.w) / 2, 0);
                    break;

                // Bottom Vertically
                case TextAlignmentOptions.Bottom:
                case TextAlignmentOptions.BottomLeft:
                case TextAlignmentOptions.BottomRight:
                case TextAlignmentOptions.BottomJustified:
                    m_anchorOffset = m_rectCorners[0] + new Vector3(0 + margins.x, 0 - m_maxDescender + margins.w, 0);
                    break;

                // Baseline Vertically 
                case TextAlignmentOptions.BaselineLeft:
                case TextAlignmentOptions.BaselineRight:
                case TextAlignmentOptions.Baseline:
                    m_anchorOffset = (m_rectCorners[0] + m_rectCorners[1]) / 2 + new Vector3(0 + margins.x, 0, 0);
                    break;       
            }
            #endregion Text Alignment


            // Handling of Anchor Dampening. If mesh width changes by more than 1/3 of the underline character's wdith then adjust it.           
            float currentMeshWidth = m_meshExtents.max.x - m_meshExtents.min.x;
            if (m_anchorDampening)
            {
                float delta = currentMeshWidth - m_baseDampeningWidth;
                if (m_baseDampeningWidth != 0 && Mathf.Abs(delta) < m_cached_Underline_GlyphInfo.width * m_fontScale * 0.6f)
                    m_anchorOffset.x += delta / 2;
                else
                    m_baseDampeningWidth = currentMeshWidth;
            }


            // Initialization for Second Pass
            Vector3 justificationOffset = Vector3.zero;
            Vector3 offset = Vector3.zero;
            int vert_index_X4 = 0;
            int underlineSegmentCount = 0;

            int wordCount = 0;
            int lineCount = 0;
            int lastLine = 0;

            bool isStartOfWord = false;
            int wordFirstChar = 0;
            int wordLastChar = 0;


            // Second Pass : Line Justification, UV Mapping, Character & Line Visibility & more.
            #region Handle Line Justification & UV Mapping & Character Visibility & More
            for (int i = 0; i < m_characterCount; i++)
            {
                int currentLine = m_textInfo.characterInfo[i].lineNumber;
                char currentCharacter = m_textInfo.characterInfo[i].character;
                LineInfo lineInfo = m_textInfo.lineInfo[currentLine];
                TextAlignmentOptions lineAlignment = lineInfo.alignment;
                lineCount = currentLine + 1;

                // Process Line Justification
                #region Handle Line Justification
                switch (lineAlignment)
                {
                    case TextAlignmentOptions.TopLeft:
                    case TextAlignmentOptions.Left:
                    case TextAlignmentOptions.BottomLeft:
                    case TextAlignmentOptions.BaselineLeft:
                        justificationOffset = Vector3.zero;
                        break;
                    
                    case TextAlignmentOptions.Top:
                    case TextAlignmentOptions.Center:
                    case TextAlignmentOptions.Bottom:
                    case TextAlignmentOptions.Baseline:
                        justificationOffset = new Vector3(marginWidth / 2 - (lineInfo.lineExtents.min.x + lineInfo.lineExtents.max.x) / 2, 0, 0);
                        break;
                    
                    case TextAlignmentOptions.TopRight:
                    case TextAlignmentOptions.Right:
                    case TextAlignmentOptions.BottomRight:
                    case TextAlignmentOptions.BaselineRight:
                        justificationOffset = new Vector3(marginWidth - lineInfo.lineExtents.max.x, 0, 0);
                        break;
                    
                    case TextAlignmentOptions.TopJustified:
                    case TextAlignmentOptions.Justified:
                    case TextAlignmentOptions.BottomJustified:
                        charCode = m_textInfo.characterInfo[i].character;
                        char lastCharOfCurrentLine = m_textInfo.characterInfo[lineInfo.lastCharacterIndex].character;

                        if (char.IsWhiteSpace(lastCharOfCurrentLine) && !char.IsControl(lastCharOfCurrentLine) && currentLine < m_lineNumber)
                        {   // All lines are justified accept the last one.
                            float gap = (m_rectCorners[3].x - margins.z) - (m_rectCorners[0].x + margins.x) - (lineInfo.lineExtents.max.x);
                            if (currentLine != lastLine || i == 0)
                                justificationOffset = Vector3.zero;
                            else
                            {
                                if (charCode == 9 || charCode == 32)
                                {
                                    justificationOffset += new Vector3(gap * (1 - m_wordWrappingRatios) / (lineInfo.spaceCount - 1), 0, 0);
                                }
                                else
                                {
                                    //Debug.Log("LineInfo Character Count: " + lineInfo.characterCount);
                                    justificationOffset += new Vector3(gap * m_wordWrappingRatios / (lineInfo.characterCount - lineInfo.spaceCount - 1), 0, 0);
                                }
                            }
                        }
                        else
                            justificationOffset = Vector3.zero; // Keep last line left justified.

                        //Debug.Log("Char [" + (char)charCode + "] Code:" + charCode + "  Offset:" + justificationOffset + "  # Spaces:" + m_lineExtents[currentLine].NumberOfSpaces + "  # Characters:" + m_lineExtents[currentLine].NumberOfChars);                       
                        break;
                }
                #endregion End Text Justification

                offset = m_anchorOffset + justificationOffset;

                if (m_textInfo.characterInfo[i].isVisible)
                {
                    Extents lineExtents = lineInfo.lineExtents;
                    float uvOffset = (m_uvLineOffset * currentLine) % 1 + m_uvOffset.x;                       


                    // Setup UV2 based on Character Mapping Options Selected
                    #region Handle UV Mapping Options
                    switch (m_horizontalMapping)
                    {
                        case TextureMappingOptions.Character:
                            m_uiVertices[vert_index_X4 + 0].uv1.x = 0 + m_uvOffset.x;
                            m_uiVertices[vert_index_X4 + 1].uv1.x = 0 + m_uvOffset.x;
                            m_uiVertices[vert_index_X4 + 2].uv1.x = 1 + m_uvOffset.x;
                            m_uiVertices[vert_index_X4 + 3].uv1.x = 1 + m_uvOffset.x;
                            break;

                        case TextureMappingOptions.Line:
                            if (m_textAlignment != TextAlignmentOptions.Justified)
                            {
                                m_uiVertices[vert_index_X4 + 0].uv1.x = (m_uiVertices[vert_index_X4 + 0].position.x - lineExtents.min.x) / (lineExtents.max.x - lineExtents.min.x) + uvOffset;
                                m_uiVertices[vert_index_X4 + 1].uv1.x = (m_uiVertices[vert_index_X4 + 1].position.x - lineExtents.min.x) / (lineExtents.max.x - lineExtents.min.x) + uvOffset;
                                m_uiVertices[vert_index_X4 + 2].uv1.x = (m_uiVertices[vert_index_X4 + 2].position.x - lineExtents.min.x) / (lineExtents.max.x - lineExtents.min.x) + uvOffset;
                                m_uiVertices[vert_index_X4 + 3].uv1.x = (m_uiVertices[vert_index_X4 + 3].position.x - lineExtents.min.x) / (lineExtents.max.x - lineExtents.min.x) + uvOffset;
                                break;
                            }
                            else // Special Case if Justified is used in Line Mode.
                            {
                                m_uiVertices[vert_index_X4 + 0].uv1.x = (m_uiVertices[vert_index_X4 + 0].position.x + justificationOffset.x - m_meshExtents.min.x) / (m_meshExtents.max.x - m_meshExtents.min.x) + uvOffset;
                                m_uiVertices[vert_index_X4 + 1].uv1.x = (m_uiVertices[vert_index_X4 + 1].position.x + justificationOffset.x - m_meshExtents.min.x) / (m_meshExtents.max.x - m_meshExtents.min.x) + uvOffset;
                                m_uiVertices[vert_index_X4 + 2].uv1.x = (m_uiVertices[vert_index_X4 + 2].position.x + justificationOffset.x - m_meshExtents.min.x) / (m_meshExtents.max.x - m_meshExtents.min.x) + uvOffset;
                                m_uiVertices[vert_index_X4 + 3].uv1.x = (m_uiVertices[vert_index_X4 + 3].position.x + justificationOffset.x - m_meshExtents.min.x) / (m_meshExtents.max.x - m_meshExtents.min.x) + uvOffset;
                                break;
                            }

                        case TextureMappingOptions.Paragraph:
                            m_uiVertices[vert_index_X4 + 0].uv1.x = (m_uiVertices[vert_index_X4 + 0].position.x + justificationOffset.x - m_meshExtents.min.x) / (m_meshExtents.max.x - m_meshExtents.min.x) + uvOffset;
                            m_uiVertices[vert_index_X4 + 1].uv1.x = (m_uiVertices[vert_index_X4 + 1].position.x + justificationOffset.x - m_meshExtents.min.x) / (m_meshExtents.max.x - m_meshExtents.min.x) + uvOffset;
                            m_uiVertices[vert_index_X4 + 2].uv1.x = (m_uiVertices[vert_index_X4 + 2].position.x + justificationOffset.x - m_meshExtents.min.x) / (m_meshExtents.max.x - m_meshExtents.min.x) + uvOffset;
                            m_uiVertices[vert_index_X4 + 3].uv1.x = (m_uiVertices[vert_index_X4 + 3].position.x + justificationOffset.x - m_meshExtents.min.x) / (m_meshExtents.max.x - m_meshExtents.min.x) + uvOffset;
                            break;

                        case TextureMappingOptions.MatchAspect:

                            switch (m_verticalMapping)
                            {
                                case TextureMappingOptions.Character:
                                    m_uiVertices[vert_index_X4 + 0].uv1.y = 0 + m_uvOffset.y;
                                    m_uiVertices[vert_index_X4 + 1].uv1.y = 1 + m_uvOffset.y;
                                    m_uiVertices[vert_index_X4 + 2].uv1.y = 0 + m_uvOffset.y;
                                    m_uiVertices[vert_index_X4 + 3].uv1.y = 1 + m_uvOffset.y;
                                    break;

                                case TextureMappingOptions.Line:
                                    m_uiVertices[vert_index_X4 + 0].uv1.y = (m_uiVertices[vert_index_X4].position.y - lineExtents.min.y) / (lineExtents.max.y - lineExtents.min.y) + uvOffset;
                                    m_uiVertices[vert_index_X4 + 1].uv1.y = (m_uiVertices[vert_index_X4 + 1].position.y - lineExtents.min.y) / (lineExtents.max.y - lineExtents.min.y) + uvOffset;
                                    m_uiVertices[vert_index_X4 + 2].uv1.y = m_uiVertices[vert_index_X4].uv1.y;
                                    m_uiVertices[vert_index_X4 + 3].uv1.y = m_uiVertices[vert_index_X4 + 1].uv1.y;
                                    break;

                                case TextureMappingOptions.Paragraph:
                                    m_uiVertices[vert_index_X4 + 0].uv1.y = (m_uiVertices[vert_index_X4].position.y - m_meshExtents.min.y) / (m_meshExtents.max.y - m_meshExtents.min.y) + uvOffset;
                                    m_uiVertices[vert_index_X4 + 1].uv1.y = (m_uiVertices[vert_index_X4 + 1].position.y - m_meshExtents.min.y) / (m_meshExtents.max.y - m_meshExtents.min.y) + uvOffset;
                                    m_uiVertices[vert_index_X4 + 2].uv1.y = m_uiVertices[vert_index_X4].uv1.y;
                                    m_uiVertices[vert_index_X4 + 3].uv1.y = m_uiVertices[vert_index_X4 + 1].uv1.y;
                                    break;

                                case TextureMappingOptions.MatchAspect:
                                    Debug.Log("ERROR: Cannot Match both Vertical & Horizontal.");
                                    break;
                            }

                            //float xDelta = 1 - (_uv2s[vert_index + 0].y * textMeshCharacterInfo[i].AspectRatio); // Left aligned
                            float xDelta = (1 - ((m_uiVertices[vert_index_X4 + 0].uv1.y + m_uiVertices[vert_index_X4 + 1].uv1.y) * m_textInfo.characterInfo[i].aspectRatio)) / 2; // Center of Rectangle
                            //float xDelta = 0;

                            m_uiVertices[vert_index_X4 + 0].uv1.x = (m_uiVertices[vert_index_X4 + 0].uv1.y * m_textInfo.characterInfo[i].aspectRatio) + xDelta + uvOffset;
                            m_uiVertices[vert_index_X4 + 1].uv1.x = m_uiVertices[vert_index_X4 + 0].uv1.x;
                            m_uiVertices[vert_index_X4 + 2].uv1.x = (m_uiVertices[vert_index_X4 + 1].uv1.y * m_textInfo.characterInfo[i].aspectRatio) + xDelta + uvOffset;
                            m_uiVertices[vert_index_X4 + 3].uv1.x = m_uiVertices[vert_index_X4 + 2].uv1.x;
                            break;
                    }

                    switch (m_verticalMapping)
                    {
                        case TextureMappingOptions.Character:
                            m_uiVertices[vert_index_X4 + 0].uv1.y = 0 + m_uvOffset.y;
                            m_uiVertices[vert_index_X4 + 1].uv1.y = 1 + m_uvOffset.y;
                            m_uiVertices[vert_index_X4 + 2].uv1.y = 1 + m_uvOffset.y;
                            m_uiVertices[vert_index_X4 + 3].uv1.y = 0 + m_uvOffset.y;
                            break;

                        case TextureMappingOptions.Line:
                            m_uiVertices[vert_index_X4 + 0].uv1.y = (m_uiVertices[vert_index_X4].position.y - lineExtents.min.y) / (lineExtents.max.y - lineExtents.min.y) + m_uvOffset.y;
                            m_uiVertices[vert_index_X4 + 1].uv1.y = (m_uiVertices[vert_index_X4 + 1].position.y - lineExtents.min.y) / (lineExtents.max.y - lineExtents.min.y) + m_uvOffset.y;
                            m_uiVertices[vert_index_X4 + 3].uv1.y = m_uiVertices[vert_index_X4].uv1.y;
                            m_uiVertices[vert_index_X4 + 2].uv1.y = m_uiVertices[vert_index_X4 + 1].uv1.y;
                            break;

                        case TextureMappingOptions.Paragraph:
                            m_uiVertices[vert_index_X4 + 0].uv1.y = (m_uiVertices[vert_index_X4].position.y - m_meshExtents.min.y) / (m_meshExtents.max.y - m_meshExtents.min.y) + m_uvOffset.y;
                            m_uiVertices[vert_index_X4 + 1].uv1.y = (m_uiVertices[vert_index_X4 + 1].position.y - m_meshExtents.min.y) / (m_meshExtents.max.y - m_meshExtents.min.y) + m_uvOffset.y;
                            m_uiVertices[vert_index_X4 + 3].uv1.y = m_uiVertices[vert_index_X4].uv1.y;
                            m_uiVertices[vert_index_X4 + 2].uv1.y = m_uiVertices[vert_index_X4 + 1].uv1.y;
                            break;

                        case TextureMappingOptions.MatchAspect:
                            //float yDelta = 1 - (_uv2s[vert_index + 2].x / textMeshCharacterInfo[i].AspectRatio); // Top Corner                       
                            float yDelta = (1 - ((m_uiVertices[vert_index_X4 + 0].uv1.x + m_uiVertices[vert_index_X4 + 2].uv1.x) / m_textInfo.characterInfo[i].aspectRatio)) / 2; // Center of Rectangle
                            //float yDelta = 0;

                            m_uiVertices[vert_index_X4 + 0].uv1.y = yDelta + (m_uiVertices[vert_index_X4 + 0].uv1.x / m_textInfo.characterInfo[i].aspectRatio) + m_uvOffset.y;
                            m_uiVertices[vert_index_X4 + 1].uv1.y = yDelta + (m_uiVertices[vert_index_X4 + 2].uv1.x / m_textInfo.characterInfo[i].aspectRatio) + m_uvOffset.y;
                            m_uiVertices[vert_index_X4 + 2].uv1.y = m_uiVertices[vert_index_X4 + 0].uv1.y;
                            m_uiVertices[vert_index_X4 + 3].uv1.y = m_uiVertices[vert_index_X4 + 1].uv1.y;
                            break;
                    }
                    #endregion End UV Mapping Options


                    // Pack UV's so that we can pass Xscale needed for Shader to maintain 1:1 ratio.
                    float xScale = m_textInfo.characterInfo[i].scale * m_rectTransform.lossyScale.z;

                    float x0 = m_uiVertices[vert_index_X4 + 0].uv1.x;
                    float y0 = m_uiVertices[vert_index_X4 + 0].uv1.y;
                    float x1 = m_uiVertices[vert_index_X4 + 2].uv1.x;
                    float y1 = m_uiVertices[vert_index_X4 + 2].uv1.y;

                    float dx = Mathf.Floor(x0);
                    float dy = Mathf.Floor(y0);

                    x0 = x0 - dx;
                    x1 = x1 - dx;
                    y0 = y0 - dy;
                    y1 = y1 - dy;

                    m_uiVertices[vert_index_X4 + 0].uv1 = PackUV(x0, y0, xScale);
                    m_uiVertices[vert_index_X4 + 1].uv1 = PackUV(x0, y1, xScale);
                    m_uiVertices[vert_index_X4 + 2].uv1 = PackUV(x1, y1, xScale);
                    m_uiVertices[vert_index_X4 + 3].uv1 = PackUV(x1, y0, xScale);


                    // Enables control of the visibility of characters && lines.
                    if (m_maxVisibleCharacters != -1 && i >= m_maxVisibleCharacters || m_maxVisibleLines != -1 && currentLine >= m_maxVisibleLines) // || pages != AllPages && 
                    {
                        m_uiVertices[vert_index_X4 + 0].position *= 0;
                        m_uiVertices[vert_index_X4 + 1].position *= 0;
                        m_uiVertices[vert_index_X4 + 2].position *= 0;
                        m_uiVertices[vert_index_X4 + 3].position *= 0;
                    }
                    else
                    {
                        m_uiVertices[vert_index_X4 + 0].position += offset;
                        m_uiVertices[vert_index_X4 + 1].position += offset;
                        m_uiVertices[vert_index_X4 + 2].position += offset;
                        m_uiVertices[vert_index_X4 + 3].position += offset;
                    }

                    vert_index_X4 += 4;
                }

                m_textInfo.characterInfo[i].bottomLeft += offset;
                m_textInfo.characterInfo[i].topRight += offset;
                m_textInfo.characterInfo[i].topLine += offset.y;
                m_textInfo.characterInfo[i].bottomLine += offset.y;
                m_textInfo.characterInfo[i].baseLine += offset.y;


                // Store Max Ascender & Descender
                m_textInfo.lineInfo[currentLine].ascender = m_textInfo.characterInfo[i].topLine > m_textInfo.lineInfo[currentLine].ascender ? m_textInfo.characterInfo[i].topLine : m_textInfo.lineInfo[currentLine].ascender;
                m_textInfo.lineInfo[currentLine].descender = m_textInfo.characterInfo[i].bottomLine < m_textInfo.lineInfo[currentLine].descender ? m_textInfo.characterInfo[i].bottomLine : m_textInfo.lineInfo[currentLine].descender;


                // Need to recompute lineExtent to account for the offset from justification.
                if (currentLine != lastLine || i == m_characterCount - 1)
                {
                    m_textInfo.lineInfo[lastLine].lineExtents.min = new Vector2(m_textInfo.characterInfo[m_textInfo.lineInfo[lastLine].firstCharacterIndex].bottomLeft.x, m_textInfo.lineInfo[lastLine].descender);
                    m_textInfo.lineInfo[lastLine].lineExtents.max = new Vector2(m_textInfo.characterInfo[m_textInfo.lineInfo[lastLine].lastCharacterIndex].topRight.x, m_textInfo.lineInfo[lastLine].ascender);
                }


                // Track Word Count per line and for the object
                if (char.IsLetterOrDigit(currentCharacter) && i < m_characterCount - 1)
                {
                    if (isStartOfWord == false)
                    {
                        isStartOfWord = true;
                        wordFirstChar = i;
                    }
                }
                else if ((char.IsPunctuation(currentCharacter) || char.IsWhiteSpace(currentCharacter) || i == m_characterCount - 1) && isStartOfWord || i == 0)
                {
                    wordLastChar = i == m_characterCount - 1 && char.IsLetterOrDigit(currentCharacter) ? i : i - 1;
                    isStartOfWord = false;

                    wordCount += 1;
                    m_textInfo.lineInfo[currentLine].wordCount += 1;

                    WordInfo wordInfo = new WordInfo();
                    wordInfo.firstCharacterIndex = wordFirstChar;
                    wordInfo.lastCharacterIndex = wordLastChar;
                    wordInfo.characterCount = wordLastChar - wordFirstChar + 1;
                    m_textInfo.wordInfo.Add(wordInfo);
                    //Debug.Log("Word #" + wordCount + " is [" + wordInfo.word + "] Start Index: " + wordInfo.firstCharacterIndex + "  End Index: " + wordInfo.lastCharacterIndex);
                }


                // Handle Underline
                #region Underline Tracking
                // TODO : Address underline and which font to use in the list
                if ((m_textInfo.characterInfo[i].style & FontStyles.Underline) == FontStyles.Underline && i != m_textInfo.lineInfo[currentLine].lastCharacterIndex) // m_textInfo.characterInfo[i].character != 10 && m_textInfo.characterInfo[i].character != 13)
                {
                    if (beginUnderline == false)
                    {
                        beginUnderline = true;
                        underline_start = new Vector3(m_textInfo.characterInfo[i].bottomLeft.x, m_textInfo.characterInfo[i].baseLine + font.fontInfo.Underline * m_fontScale, 0);
                        //Debug.Log("Underline Start Char [" + m_textInfo.characterInfo[i].character + "].");
                    }
                }
                else
                {
                    if (beginUnderline == true)
                    {
                        beginUnderline = false;
                        if (i != m_characterCount - 1 && (m_textInfo.characterInfo[i].character == 32 || m_textInfo.characterInfo[i].character == 10))
                        {
                            underline_end = new Vector3(m_textInfo.characterInfo[i - 1].topRight.x, m_textInfo.characterInfo[i - 1].baseLine + font.fontInfo.Underline * m_fontScale, 0);
                            //Debug.Log("Underline End Char [" + m_textInfo.characterInfo[i - 1].character + "].");
                        }
                        else
                        {
                            underline_end = new Vector3(m_textInfo.characterInfo[i].topRight.x, m_textInfo.characterInfo[i].baseLine + font.fontInfo.Underline * m_fontScale, 0);
                            //Debug.Log("Underline End Char [" + m_textInfo.characterInfo[i].character + "].");
                        }

                        
                        DrawUnderlineMesh(underline_start, underline_end, ref last_vert_index);
                        underlineSegmentCount += 1;
                    }
                }
                #endregion


                lastLine = currentLine;
            }
            #endregion

            // METRICS ABOUT THE TEXT OBJECT            
            m_textInfo.characterCount = (short)m_characterCount;
            m_textInfo.lineCount = (short)lineCount;
            m_textInfo.wordCount = wordCount != 0 && m_characterCount > 0 ? (short)wordCount : (short)1;

            // Need to store UI Vertex
            m_textInfo.meshInfo.uiVertices = m_uiVertices;



            // If Advanced Layout Component is present, don't upload the mesh.
            if (m_renderMode == TextRenderFlags.Render) // m_isAdvanceLayoutComponentPresent == false || m_advancedLayoutComponent.isEnabled == false)
            {
                //Debug.Log("Uploading Mesh normally.");
                // Upload Mesh Data 
                m_uiRenderer.SetVertices(m_uiVertices, vert_index_X4 + underlineSegmentCount * 12);

                // Setting Mesh Bounds manually is more efficient.               
                m_bounds = new Bounds(new Vector3((m_meshExtents.max.x + m_meshExtents.min.x) / 2, (m_meshExtents.max.y + m_meshExtents.min.y) / 2, 0) + m_anchorOffset, new Vector3(m_meshExtents.max.x - m_meshExtents.min.x, m_meshExtents.max.y - m_meshExtents.min.y, 0));

                //m_maskOffset = new Vector4(m_mesh.bounds.center.x, m_mesh.bounds.center.y, m_mesh.bounds.size.x, m_mesh.bounds.size.y);
            }
           
              
            m_bounds = new Bounds(new Vector3((m_meshExtents.max.x + m_meshExtents.min.x) / 2, (m_meshExtents.max.y + m_meshExtents.min.y) / 2, 0) + m_anchorOffset, new Vector3(m_meshExtents.max.x - m_meshExtents.min.x, m_meshExtents.max.y - m_meshExtents.min.y, 0));
                                             
            m_isCharacterWrappingEnabled = false;
           


            // Has Text Container's Width or Height been specified by the user?
            /*
            if (m_rectTransform.sizeDelta.x == 0 || m_rectTransform.sizeDelta.y == 0)
            {
                //Debug.Log("Auto-fitting Text. Default Width:" + m_textContainer.isDefaultWidth + "  Default Height:" + m_textContainer.isDefaultHeight);
                if (marginWidth == 0)
                    m_rectTransform.sizeDelta = new Vector2(m_preferredWidth + margins.x + margins.z, m_rectTransform.sizeDelta.y);

                if (marginHeight == 0)
                    m_rectTransform.sizeDelta = new Vector2(m_rectTransform.sizeDelta.x,  m_preferredHeight + margins.y + margins.w);

                
                Debug.Log("Auto-fitting Text. Default Width:" + m_preferredWidth + "  Default Height:" + m_preferredHeight);
                GenerateTextMesh();
                return;
            }
            */
                  
            //Debug.Log("Done rendering text. Margin Width was " + marginWidth +  " and Margin Height was " + marginHeight + ". Preferred Width: " + m_preferredWidth + " and Height: " + m_preferredHeight);
            
            //Debug.Log(m_minWidth);
            //Profiler.EndSample();
            //m_StopWatch.Stop();           
            //Debug.Log("Done Rendering Text.");
            //Debug.Log("TimeElapsed is:" + (m_StopWatch.ElapsedTicks / 10000f).ToString("f4"));
            //m_StopWatch.Reset();     
        }





        // Draws the Underline
        void DrawUnderlineMesh(Vector3 start, Vector3 end, ref int index)
        {

            int visibleCount = index + 12;          
            // Check to make sure our current mesh buffer allocations can hold these new Quads.  
            if (visibleCount > m_uiVertices.Length)
            {
                // Check to make sure we can fit underline segments.            
                SetMeshArrays(visibleCount / 4 + 16);
                // Arrays have been resized so we need to re-process the mesh
                GenerateTextMesh();
                return;
            }

            // Adjust the position of the underline based on the lowest character. This matters for subscript character.
            start.y = Mathf.Min(start.y, end.y);
            end.y = Mathf.Min(start.y, end.y);

            float segmentWidth = m_cached_Underline_GlyphInfo.width / 2 * m_fontScale;

            if (end.x - start.x < m_cached_Underline_GlyphInfo.width * m_fontScale)
            {
                segmentWidth = (end.x - start.x) / 2f;
            }
            //Debug.Log("Char H:" + cached_Underline_GlyphInfo.height);

            float underlineThickness = m_cached_Underline_GlyphInfo.height; // m_fontAsset.FontInfo.UnderlineThickness;
            // Front Part of the Underline
            m_uiVertices[index + 0].position = start + new Vector3(0, 0 - (underlineThickness + m_padding) * m_fontScale, 0); // BL
            m_uiVertices[index + 1].position = start + new Vector3(0, m_padding * m_fontScale, 0); // TL
            m_uiVertices[index + 2].position = start + new Vector3(segmentWidth, m_padding * m_fontScale, 0); // TR
            m_uiVertices[index + 3].position = m_uiVertices[index + 0].position + new Vector3(segmentWidth, 0, 0); // BR

            // Middle Part of the Underline
            m_uiVertices[index + 4].position = m_uiVertices[index + 3].position; // BL
            m_uiVertices[index + 5].position = m_uiVertices[index + 2].position; // TL
            m_uiVertices[index + 6].position = end + new Vector3(-segmentWidth, m_padding * m_fontScale, 0);  // TR
            m_uiVertices[index + 7].position = end + new Vector3(-segmentWidth, -(underlineThickness + m_padding) * m_fontScale, 0); // BR

            // End Part of the Underline
            m_uiVertices[index + 8].position = m_uiVertices[index + 7].position; // BL
            m_uiVertices[index + 9].position = m_uiVertices[index + 6].position; // TL
            m_uiVertices[index + 10].position = end + new Vector3(0, m_padding * m_fontScale, 0); // TR
            m_uiVertices[index + 11].position = end + new Vector3(0, -(underlineThickness + m_padding) * m_fontScale, 0); // BR


            // Calculate UV required to setup the 3 Quads for the Underline.
            Vector2 uv0 = new Vector2((m_cached_Underline_GlyphInfo.x - m_padding) / m_fontAsset.fontInfo.AtlasWidth, 1 - (m_cached_Underline_GlyphInfo.y + m_padding + m_cached_Underline_GlyphInfo.height) / m_fontAsset.fontInfo.AtlasHeight);  // bottom left
            Vector2 uv1 = new Vector2(uv0.x, 1 - (m_cached_Underline_GlyphInfo.y - m_padding) / m_fontAsset.fontInfo.AtlasHeight);  // top left
            Vector2 uv2 = new Vector2((m_cached_Underline_GlyphInfo.x + m_padding + m_cached_Underline_GlyphInfo.width / 2) / m_fontAsset.fontInfo.AtlasWidth, uv1.y); // Top Right
            Vector2 uv3 = new Vector2(uv2.x, uv0.y); // Bottom right
            Vector2 uv4 = new Vector2((m_cached_Underline_GlyphInfo.x + m_padding + m_cached_Underline_GlyphInfo.width) / m_fontAsset.fontInfo.AtlasWidth, uv1.y); // End Part - Bottom Right
            Vector2 uv5 = new Vector2(uv4.x, uv0.y); // End Part - Top Right

            // Left Part of the Underline
            m_uiVertices[0 + index].uv0 = uv0; // BL
            m_uiVertices[1 + index].uv0 = uv1; // TL   
            m_uiVertices[2 + index].uv0 = uv2; // TR   
            m_uiVertices[3 + index].uv0 = uv3; // BR

            // Middle Part of the Underline
            m_uiVertices[4 + index].uv0 = new Vector2(uv2.x - uv2.x * 0.001f, uv0.y);
            m_uiVertices[5 + index].uv0 = new Vector2(uv2.x - uv2.x * 0.001f, uv1.y);
            m_uiVertices[6 + index].uv0 = new Vector2(uv2.x + uv2.x * 0.001f, uv1.y);
            m_uiVertices[7 + index].uv0 = new Vector2(uv2.x + uv2.x * 0.001f, uv0.y);

            // Right Part of the Underline
            m_uiVertices[8 + index].uv0 = uv3;
            m_uiVertices[9 + index].uv0 = uv2;
            m_uiVertices[10 + index].uv0 = uv4;
            m_uiVertices[11 + index].uv0 = uv5;

          
            // UV1 contains Face / Border UV layout.
            float min_UvX = 0;
            float max_UvX = (m_uiVertices[index + 2].position.x - start.x) / (end.x - start.x);

            //Calculate the xScale or how much the UV's are getting stretched on the X axis for the middle section of the underline.
            float xScale = m_fontScale * m_rectTransform.lossyScale.z;
            float xScale2 = xScale;

            m_uiVertices[0 + index].uv1 = PackUV(0, 0, xScale);
            m_uiVertices[1 + index].uv1 = PackUV(0, 1, xScale);
            m_uiVertices[2 + index].uv1 = PackUV(max_UvX, 1, xScale);
            m_uiVertices[3 + index].uv1 = PackUV(max_UvX, 0, xScale);

            min_UvX = (m_uiVertices[index + 4].position.x - start.x) / (end.x - start.x);
            max_UvX = (m_uiVertices[index + 6].position.x - start.x) / (end.x - start.x);

            m_uiVertices[4 + index].uv1 = PackUV(min_UvX, 0, xScale2);
            m_uiVertices[5 + index].uv1 = PackUV(min_UvX, 1, xScale2);
            m_uiVertices[6 + index].uv1 = PackUV(max_UvX, 1, xScale2);
            m_uiVertices[7 + index].uv1 = PackUV(max_UvX, 0, xScale2);

            min_UvX = (m_uiVertices[index + 8].position.x - start.x) / (end.x - start.x);
            max_UvX = (m_uiVertices[index + 6].position.x - start.x) / (end.x - start.x);

            m_uiVertices[8 + index].uv1 = PackUV(min_UvX, 0, xScale);
            m_uiVertices[9 + index].uv1 = PackUV(min_UvX, 1, xScale);
            m_uiVertices[10 + index].uv1 = PackUV(1, 1, xScale);
            m_uiVertices[11 + index].uv1 = PackUV(1, 0, xScale);


            Color32 underlineColor = new Color32(m_fontColor32.r, m_fontColor32.g, m_fontColor32.b, (byte)(m_fontColor32.a / 2));

            m_uiVertices[0 + index].color = underlineColor;
            m_uiVertices[1 + index].color = underlineColor;
            m_uiVertices[2 + index].color = underlineColor;
            m_uiVertices[3 + index].color = underlineColor;

            m_uiVertices[4 + index].color = underlineColor;
            m_uiVertices[5 + index].color = underlineColor;
            m_uiVertices[6 + index].color = underlineColor;
            m_uiVertices[7 + index].color = underlineColor;

            m_uiVertices[8 + index].color = underlineColor;
            m_uiVertices[9 + index].color = underlineColor;
            m_uiVertices[10 + index].color = underlineColor;
            m_uiVertices[11 + index].color = underlineColor;

            index += 12;
        }


        // Used with Advanced Layout Component.
        void UpdateMeshData(TMPro_CharacterInfo[] characterInfo, int characterCount, Mesh mesh, Vector3[] vertices, Vector2[] uv0s, Vector2[] uv2s, Color32[] vertexColors, Vector3[] normals, Vector4[] tangents)
        {
            m_textInfo.characterInfo = characterInfo;
            m_textInfo.characterCount = (short)characterCount;
            //m_meshInfo.mesh = mesh;
            //m_meshInfo.vertices = vertices;
            //m_meshInfo.uv0s = uv0s;
            //m_meshInfo.uv2s = uv2s;
            //m_meshInfo.vertexColors = m_vertColors;
            //m_meshInfo.normals = normals;
            //m_meshInfo.tangents = tangents;
        }


        // Function to offset vertices position to account for line spacing changes.
        void AdjustLineOffset(int startIndex, int endIndex, float offset)
        {
            Vector3 vertexOffset = new Vector3(0, offset, 0);

            for (int i = startIndex; i < endIndex + 1; i++)
            {
                if (m_textInfo.characterInfo[i].isVisible)
                {
                    int vertexIndex = m_textInfo.characterInfo[i].vertexIndex;
                    m_textInfo.characterInfo[i].bottomLeft -= vertexOffset;
                    m_textInfo.characterInfo[i].topRight -= vertexOffset;
                    m_textInfo.characterInfo[i].bottomLine -= vertexOffset.y;
                    m_textInfo.characterInfo[i].topLine -= vertexOffset.y;

                    m_uiVertices[0 + vertexIndex].position -= vertexOffset;
                    m_uiVertices[1 + vertexIndex].position -= vertexOffset;
                    m_uiVertices[2 + vertexIndex].position -= vertexOffset;
                    m_uiVertices[3 + vertexIndex].position -= vertexOffset;
                }

            }
        }


        // Save the State of various variables used in the mesh creation loop in conjunction with Word Wrapping 
        void SaveWordWrappingState(ref WordWrapState state, int index)
        {
            state.previous_WordBreak = index;
            state.total_CharacterCount = m_characterCount;
            state.visible_CharacterCount = m_visibleCharacterCount;
            state.xAdvance = m_xAdvance;
            state.maxAscender = m_maxAscender;
            state.maxDescender = m_maxDescender;
            state.fontScale = m_fontScale;
            state.maxFontScale = m_maxFontScale;
            state.lineOffset = m_lineOffset;
            state.currentFontSize = m_currentFontSize;
            state.baselineOffset = m_baselineOffset;
            state.fontStyle = m_style;
            state.vertexColor = m_htmlColor;
            state.meshExtents = m_meshExtents;
            state.lineInfo = m_textInfo.lineInfo[m_lineNumber];
            state.textInfo = m_textInfo;
        }

        // Restore the State of various variables used in the mesh creation loop.
        int RestoreWordWrappingState(ref WordWrapState state)
        {
            m_textInfo.lineInfo[m_lineNumber] = state.lineInfo;
            m_textInfo = state.textInfo;
            m_currentFontSize = state.currentFontSize;
            m_fontScale = state.fontScale;
            m_baselineOffset = state.baselineOffset;
            m_style = state.fontStyle;
            m_htmlColor = state.vertexColor;

            m_characterCount = state.total_CharacterCount + 1;
            m_visibleCharacterCount = state.visible_CharacterCount;
            m_meshExtents = state.meshExtents;
            m_xAdvance = state.xAdvance;
            m_maxAscender = state.maxAscender;
            m_maxDescender = state.maxDescender;
            m_lineOffset = state.lineOffset;
            m_maxFontScale = state.maxFontScale;

            int index = state.previous_WordBreak;

            return index;
        }

        /*
        // Save the State of a line
        void SaveLineState(int i)
        {
            m_SavedLineState.previous_LineBreak = i;
            m_SavedLineState.total_CharacterCount = m_characterCount;
            m_SavedLineState.visible_CharacterCount = m_visibleCharacterCount;
            m_SavedLineState.maxAscender = m_maxAscender;
            m_SavedLineState.fontScale = m_fontScale;
            
            m_SavedLineState.lineOffset = m_lineOffset;
            m_SavedLineState.currentFontSize = m_currentFontSize;
            m_SavedLineState.baselineOffset = m_baselineOffset;
            m_SavedLineState.fontStyle = m_style;
            m_SavedLineState.vertexColor = m_htmlColor;
            m_SavedLineState.meshExtents = m_meshExtents;
            
            m_SavedLineState.textInfo = m_textInfo;
         
        }


        // Restore the State of several variables 
        int RestorePreviousLineState()
        {
            int i = m_SavedLineState.previous_LineBreak;
            m_characterCount = m_SavedLineState.total_CharacterCount + 1;
            m_visibleCharacterCount = m_SavedLineState.visible_CharacterCount;
            m_maxAscender = m_SavedLineState.maxAscender;
            m_fontScale = m_SavedLineState.fontScale;
            m_currentFontSize = m_SavedLineState.currentFontSize;
            m_baselineOffset = m_SavedLineState.baselineOffset;
            m_style = m_SavedLineState.fontStyle;
            m_htmlColor = m_SavedLineState.vertexColor;
            m_meshExtents = m_SavedLineState.meshExtents;
            m_textInfo = m_SavedLineState.textInfo;
            m_lineOffset = m_SavedLineState.lineOffset;

            return i; 
        }
        */


        // Function to pack scale information in the UV2 Channel.
        Vector2 PackUV(float x, float y, float scale)
        {
            x = (x % 5) / 5;
            y = (y % 5) / 5;

            //return new Vector2((x * 4096) + y, scale);
            return new Vector2(Mathf.Round(x * 4096) + y, scale);
        }


        // Function to increase the size of the Line Extents Array.
        void ResizeLineExtents(int size)
        {
            size = size > 1024 ? size + 256 : Mathf.NextPowerOfTwo(size + 1);

            LineInfo[] temp_lineInfo = new LineInfo[size];
            for (int i = 0; i < size; i++)
            {
                if (i < m_textInfo.lineInfo.Length)
                    temp_lineInfo[i] = m_textInfo.lineInfo[i];
                else
                {
                    temp_lineInfo[i].lineExtents = new Extents(k_InfinityVector, -k_InfinityVector);
                    temp_lineInfo[i].ascender = -k_InfinityVector.x;
                    temp_lineInfo[i].descender = k_InfinityVector.x;
                }
            }

            m_textInfo.lineInfo = temp_lineInfo;
        }


        // Convert HEX to INT
        int HexToInt(char hex)
        {
            switch (hex)
            {
                case '0': return 0;
                case '1': return 1;
                case '2': return 2;
                case '3': return 3;
                case '4': return 4;
                case '5': return 5;
                case '6': return 6;
                case '7': return 7;
                case '8': return 8;
                case '9': return 9;
                case 'A': return 10;
                case 'B': return 11;
                case 'C': return 12;
                case 'D': return 13;
                case 'E': return 14;
                case 'F': return 15;
                case 'a': return 10;
                case 'b': return 11;
                case 'c': return 12;
                case 'd': return 13;
                case 'e': return 14;
                case 'f': return 15;
            }
            return 15;
        }


        Color32 HexCharsToColor(char[] hexChars)
        {
            if (hexChars.Length == 7)
            {
                byte r = (byte)(HexToInt(hexChars[1]) * 16 + HexToInt(hexChars[2]));
                byte g = (byte)(HexToInt(hexChars[3]) * 16 + HexToInt(hexChars[4]));
                byte b = (byte)(HexToInt(hexChars[5]) * 16 + HexToInt(hexChars[6]));

                return new Color32(r, g, b, 255);
            }
            else
            {
                byte r = (byte)(HexToInt(hexChars[1]) * 16 + HexToInt(hexChars[2]));
                byte g = (byte)(HexToInt(hexChars[3]) * 16 + HexToInt(hexChars[4]));
                byte b = (byte)(HexToInt(hexChars[5]) * 16 + HexToInt(hexChars[6]));
                byte a = (byte)(HexToInt(hexChars[7]) * 16 + HexToInt(hexChars[8]));

                return new Color32(r, g, b, a);
            }
        }


        /// <summary>
        /// Extracts a float value from char[] assuming we know the position of the start, end and decimal point.
        /// </summary>
        /// <param name="chars"></param> The Char[] containing the numerical sequence.
        /// <param name="startIndex"></param> The index of the start of the numerical sequence.
        /// <param name="endIndex"></param> The index of the last number in the numerical sequence.
        /// <param name="decimalPointIndex"></param> The index of the decimal point if any.
        /// <returns></returns>
        float ConvertToFloat(char[] chars, int startIndex, int endIndex, int decimalPointIndex)
        {
            float v = 0;
            float sign = 1;
            decimalPointIndex = decimalPointIndex > 0 ? decimalPointIndex : endIndex + 1; // Check in case we don't have any decimal point

            // Check if negative value
            if (chars[startIndex] == 45) // '-'
            {
                startIndex += 1;
                sign = -1;
            }

            if (chars[startIndex] == 43 || chars[startIndex] == 37) startIndex += 1; // '+'
              
              
            for (int i = startIndex; i < endIndex + 1; i++)
            {
                switch (decimalPointIndex - i)
                {
                    case 4:
                        v += (chars[i] - 48) * 1000;
                        break;
                    case 3:
                        v += (chars[i] - 48) * 100;
                        break;
                    case 2:
                        v += (chars[i] - 48) * 10;
                        break;
                    case 1:
                        v += (chars[i] - 48);
                        break;
                    case -1:
                        v += (chars[i] - 48) * 0.1f;
                        break;
                    case -2:
                        v += (chars[i] - 48) * 0.01f;
                        break;
                    case -3:
                        v += (chars[i] - 48) * 0.001f;
                        break;
                }
            }
            return v * sign;
        }


        // Function to identify and validate the rich tag. Returns the position of the > if the tag was valid.
        bool ValidateHtmlTag(int[] chars, int startIndex, out int endIndex)
        {
            Array.Clear(m_htmlTag, 0, 16);
            int tagCharCount = 0;
            int tagCode = 0;
            int colorCode = 0;
            int numSequenceStart = 0;
            int numSequenceEnd = 0;
            int numSequenceDecimalPos = 0;

            endIndex = startIndex;

            bool isValidHtmlTag = false;
            int equalSignValue = 1;

            for (int i = startIndex; chars[i] != 0 && tagCharCount < 16 && chars[i] != 60; i++)
            {
                if (chars[i] == 62) // ASC Code of End Html tag '>'
                {
                    isValidHtmlTag = true;
                    endIndex = i;
                    m_htmlTag[tagCharCount] = (char)0;
                    break;
                }

                m_htmlTag[tagCharCount] = (char)chars[i];
                tagCharCount += 1;

                if (chars[i] == 61) equalSignValue = 0; // Once we encounter the equal sign, we stop adding the tagCode.

                tagCode += chars[i] * tagCharCount * equalSignValue;
                colorCode += chars[i] * tagCharCount * (1 - equalSignValue);
                // Get possible positions of numerical values 
                switch ((int)chars[i])
                {
                    case 61: // '='
                        numSequenceStart = tagCharCount;
                        break;
                    case 46: // '.'
                        numSequenceDecimalPos = tagCharCount - 1;
                        break;
                }
            }

            if (!isValidHtmlTag)
            {
                return false;
            }

            //Debug.Log("Tag Code:" + tagCode + "  Color Code: " + colorCode);

            if (m_htmlTag[0] == 35 && tagCharCount == 7) // if Tag begins with # and contains 7 characters. 
            {
                m_htmlColor = HexCharsToColor(m_htmlTag);
                return true;
            }
            else if (m_htmlTag[0] == 35 && tagCharCount == 9) // if Tag begins with # and contains 9 characters. 
            {
                m_htmlColor = HexCharsToColor(m_htmlTag);
                return true;
            }
            else
            {
                switch (tagCode)
                {
                    case 98: // <b>
                        m_style |= FontStyles.Bold;
                        return true;
                    case 105: // <i>
                        m_style |= FontStyles.Italic;
                        return true;
                    case 117: // <u>
                        m_style |= FontStyles.Underline;
                        return true;
                    case 243: // </b>
                        m_style &= ~FontStyles.Bold;
                        return true;
                    case 257: // </i>
                        m_style &= ~FontStyles.Italic;
                        return true;
                    case 281: // </u>
                        m_style &= ~FontStyles.Underline;
                        return true;
                    case 643: // <sub>
                        m_currentFontSize *= m_fontAsset.fontInfo.SubSize > 0 ? m_fontAsset.fontInfo.SubSize : 1; // Subscript characters are half size.
                        //m_xAdvance += 10 * m_fontScale;
                        m_baselineOffset = m_fontAsset.fontInfo.SubscriptOffset;
                        m_isRecalculateScaleRequired = true;
                        return true;
                    case 679: // <pos=000.00>
                        m_tabSpacing = ConvertToFloat(m_htmlTag, numSequenceStart, tagCharCount - 1, numSequenceDecimalPos);
                        return true;
                    case 685: // <sup>
                        m_currentFontSize *= m_fontAsset.fontInfo.SubSize > 0 ? m_fontAsset.fontInfo.SubSize : 1;
                        //m_xAdvance += 10 * m_fontScale;
                        m_baselineOffset = m_fontAsset.fontInfo.SuperscriptOffset;
                        m_isRecalculateScaleRequired = true;
                        return true;
                    case 1020: // </sub>
                        m_currentFontSize /= m_fontAsset.fontInfo.SubSize > 0 ? m_fontAsset.fontInfo.SubSize : 1; //m_fontSize / m_fontAsset.FontInfo.PointSize * .1f;
                        m_baselineOffset = 0;
                        m_isRecalculateScaleRequired = true;
                        return true;
                    case 1076: // </sup>
                        m_currentFontSize /= m_fontAsset.fontInfo.SubSize > 0 ? m_fontAsset.fontInfo.SubSize : 1; //m_fontSize / m_fontAsset.FontInfo.PointSize * .1f;
                        m_baselineOffset = 0;
                        m_isRecalculateScaleRequired = true;
                        return true;
                    case 1095: // <size=>
                        numSequenceEnd = tagCharCount - 1;
                        float val = 0;

                        if (m_htmlTag[5] == 37) // <size=%00>
                        {
                            val = ConvertToFloat(m_htmlTag, numSequenceStart, numSequenceEnd, numSequenceDecimalPos);
                            m_currentFontSize = m_fontSize * val / 100;
                            m_isRecalculateScaleRequired = true;
                            return true;
                        }
                        else if (m_htmlTag[5] == 43) // <size=+00>
                        {
                            val = ConvertToFloat(m_htmlTag, numSequenceStart, numSequenceEnd, numSequenceDecimalPos);
                            m_currentFontSize = m_fontSize + val;
                            m_isRecalculateScaleRequired = true;
                            return true;
                        }
                        else if (m_htmlTag[5] == 45) // <size=-00>
                        {
                            val = ConvertToFloat(m_htmlTag, numSequenceStart, numSequenceEnd, numSequenceDecimalPos);
                            m_currentFontSize = m_fontSize + val;
                            m_isRecalculateScaleRequired = true;
                            return true;
                        }
                        else // <size=0000.00>
                        {
                            val = ConvertToFloat(m_htmlTag, numSequenceStart, numSequenceEnd, numSequenceDecimalPos);
                            if (val == 73493) return false; // if tag is <size> with no values.
                            m_currentFontSize = val;
                            m_isRecalculateScaleRequired = true;
                            return true;
                        }
                    case 1118: // <font=xx>
                        Debug.Log("Font Tag used.");
                        //m_fontIndex = (int)ConvertToFloat(m_htmlTag, numSequenceStart, tagCharCount - 1, numSequenceDecimalPos);

                        //if(m_fontAssetArray[m_fontIndex] == null)
                        //{
                        //    // Load new font asset into index
                        //    m_fontAssetArray[m_fontIndex] = Resources.Load("Fonts & Materials/Bangers SDF", typeof(TextMeshProFont)) as TextMeshProFont; // Hard coded right now to a specific font
                        //}

                        //m_fontScale = (m_fontSize / m_fontAssetArray[m_fontIndex].fontInfo.PointSize * (m_isOrthographic ? 1 : 0.1f));
                        //Debug.Log("Font Index = " + m_fontIndex);
                        return true;
                    case 1531: // <space=000.00>
                        m_spacing = ConvertToFloat(m_htmlTag, numSequenceStart, tagCharCount - 1, numSequenceDecimalPos);
                        return true;
                    case 1585: // </size>
                        m_currentFontSize = m_fontSize;
                        m_isRecalculateScaleRequired = true;
                        //m_fontScale = m_fontSize / m_fontAsset.fontInfo.PointSize * .1f;
                        return true;
                    case 1590: // <align=>
                        //Debug.Log("Align " + colorCode);
                        switch (colorCode)
                        {
                            case 4008: // <align=left>
                                m_lineJustification = TextAlignmentOptions.Left;
                                return true;
                            case 5247: // <align=right>
                                m_lineJustification = TextAlignmentOptions.Right;
                                return true;
                            case 6496: // <align=center>
                                m_lineJustification = TextAlignmentOptions.Center;
                                return true;
                            case 10897: // <align=justified>
                                m_lineJustification = TextAlignmentOptions.Justified;
                                return true;
                        }
                        return false;
                    case 1659: // <color=>
                        switch (colorCode)
                        {
                            case 2872: // <color=red>                            
                                m_htmlColor = Color.red;
                                return true;
                            case 3979: // <color=blue>
                                m_htmlColor = Color.blue;
                                return true;
                            case 4956: // <color=black>
                                m_htmlColor = Color.black;
                                return true;
                            case 5128: // <color=green>
                                m_htmlColor = Color.green;
                                return true;
                            case 5247: // <color=white>
                                m_htmlColor = Color.white;
                                return true;
                            case 6373: // <color=orange>
                                m_htmlColor = new Color32(255, 128, 0, 255);
                                return true;
                            case 6632: // <color=purple>
                                m_htmlColor = new Color32(160, 32, 240, 255);
                                return true;
                            case 6722: // <color=yellow>
                                m_htmlColor = Color.yellow;
                                return true;
                        }
                        return false;
					case 2154: // <cspace=xx.x>
						m_cSpacing = ConvertToFloat(m_htmlTag, numSequenceStart, tagCharCount - 1, numSequenceDecimalPos);
						return true;
                    case 2160: // </align>
                        m_lineJustification = m_textAlignment;
                        return true;
					case 2824: // </cspace>
						m_cSpacing = 0;
						return true;
                    case 2249: // </color>
                        m_htmlColor = m_fontColor32;
                        return true;
                    case 2287: // <sprite=x>
                        Debug.Log("Sprite Tag used.");
                        return true;
                    case 2995: // <allcaps>
                        m_style |= FontStyles.UpperCase;
                        return true;
                    case 3778: // </allcaps>
                        m_style &= ~FontStyles.UpperCase;
                        return true;
                    case 4800: // <smallcaps>                      
                        m_style |= FontStyles.SmallCaps;                 
                        return true;
                    case 5807: // </smallcaps>
                        m_currentFontSize = m_fontSize;
                        m_style &= ~FontStyles.SmallCaps;
                        m_isRecalculateScaleRequired = true;
                        return true;
                }
            }

            return false;
        }
    }
}

#endif