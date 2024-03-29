// Copyright (C) 2014 Stephan Bouchard - All Rights Reserved
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

#if UNITY_4_6 || UNITY_5_0

using UnityEngine;
using UnityEditor;
using System.Collections;


namespace TMPro.EditorUtilities
{

    [CustomEditor(typeof(TextMeshProUGUI)), CanEditMultipleObjects]
    public class TMPro_uiEditorPanel : Editor
    {


        private struct m_foldout
        { // Track Inspector foldout panel states, globally.
            public static bool textInput = true;
            public static bool fontSettings = true;
            public static bool extraSettings = false;
            public static bool shadowSetting = false;
            public static bool materialEditor = true;
        }

        private static int m_eventID;

        private static string[] uiStateLabel = new string[] { "\t- <i>Click to expand</i> -", "\t- <i>Click to collapse</i> -" };

        private const string k_UndoRedo = "UndoRedoPerformed";

        private GUISkin mySkin;
        //private GUIStyle Group_Label;
        private GUIStyle textAreaBox;
        private GUIStyle Section_Label;


        // Alignment Button Textures
        private Texture2D alignLeft;
        private Texture2D alignCenter;
        private Texture2D alignRight;
        private Texture2D alignJustified;
        private Texture2D alignTop;
        private Texture2D alignMiddle;
        private Texture2D alignBottom;
        private GUIContent[] alignContent_A;
        private GUIContent[] alignContent_B;

        public int selAlignGrid_A = 0;
        public int selAlignGrid_B = 0;


        // Serialized Properties
        private SerializedProperty text_prop;
        private SerializedProperty fontAsset_prop;
        private SerializedProperty fontSharedMaterial_prop;
        //private SerializedProperty fontBaseMaterial_prop;
        private SerializedProperty isNewBaseMaterial_prop;

        private SerializedProperty fontStyle_prop; 

		// Color Properties
        private SerializedProperty fontColor_prop;
		private SerializedProperty enableVertexGradient_prop;
		private SerializedProperty fontColorGradient_prop;
        private SerializedProperty overrideHtmlColor_prop;
        
        private SerializedProperty fontSize_prop;
        private SerializedProperty fontSizeBase_prop;

        private SerializedProperty autoSizing_prop;
        private SerializedProperty fontSizeMin_prop;
        private SerializedProperty fontSizeMax_prop;
        private SerializedProperty charSpacingMax_prop;
        private SerializedProperty lineSpacingMax_prop;

        private SerializedProperty characterSpacing_prop;
        private SerializedProperty lineSpacing_prop;
        private SerializedProperty paragraphSpacing_prop;

        private SerializedProperty textAlignment_prop;
        //private SerializedProperty textAlignment_prop;

        private SerializedProperty horizontalMapping_prop;
        private SerializedProperty verticalMapping_prop;
        private SerializedProperty uvOffset_prop;
        private SerializedProperty uvLineOffset_prop;

        private SerializedProperty enableWordWrapping_prop;
        private SerializedProperty wordWrappingRatios_prop;
        private SerializedProperty textOverflowMode_prop;

        private SerializedProperty enableKerning_prop;

       

        private SerializedProperty inputSource_prop;
        private SerializedProperty havePropertiesChanged_prop;
        private SerializedProperty isInputPasingRequired_prop;
        //private SerializedProperty isAffectingWordWrapping_prop;
        private SerializedProperty isRichText_prop;

        private SerializedProperty hasFontAssetChanged_prop;

        private SerializedProperty enableExtraPadding_prop;
        private SerializedProperty checkPaddingRequired_prop;

        //private SerializedProperty isOrthographic_prop;

        //private SerializedProperty textRectangle_prop;
        private SerializedProperty margin_prop;

        //private SerializedProperty isMaskUpdateRequired_prop;
        //private SerializedProperty mask_prop;
        private SerializedProperty maskOffset_prop;
        //private SerializedProperty maskOffsetMode_prop;
        //private SerializedProperty maskSoftness_prop;

        private SerializedProperty vertexOffset_prop;


        //private SerializedProperty sortingLayerID_prop;
        //private SerializedProperty sortingOrder_prop;
    
        private bool havePropertiesChanged = false;


        private TextMeshProUGUI m_textMeshProScript;
        private RectTransform m_rectTransform;
        private CanvasRenderer m_uiRenderer;
        private Editor m_materialEditor;		
        private Material m_targetMaterial;

        private Rect m_inspectorStartRegion;
        private Rect m_inspectorEndRegion;

        //private TMPro_UpdateManager m_updateManager;

        private Vector3[] m_rectCorners = new Vector3[4];
        private Vector3[] handlePoints = new Vector3[4]; // { new Vector3(-10, -10, 0), new Vector3(-10, 10, 0), new Vector3(10, 10, 0), new Vector3(10, -10, 0) };
        private float prev_lineLenght;

        private bool m_isUndoSet;

        public void OnEnable()
        {
            //Debug.Log("New Instance of TMPRO UGUI Editor with ID " + this.GetInstanceID());
            
            // Initialize the Event Listener for Undo Events.
            Undo.undoRedoPerformed += OnUndoRedo;
            //Undo.postprocessModifications += OnUndoRedoEvent;   

            text_prop = serializedObject.FindProperty("m_text");
            fontAsset_prop = serializedObject.FindProperty("m_fontAsset");
            fontSharedMaterial_prop = serializedObject.FindProperty("m_sharedMaterial");
            //fontBaseMaterial_prop = serializedObject.FindProperty("m_baseMaterial");
            isNewBaseMaterial_prop = serializedObject.FindProperty("m_isNewBaseMaterial");

            fontStyle_prop = serializedObject.FindProperty("m_fontStyle");

            fontSize_prop = serializedObject.FindProperty("m_fontSize");
            fontSizeBase_prop = serializedObject.FindProperty("m_fontSizeBase");

            autoSizing_prop = serializedObject.FindProperty("m_enableAutoSizing");
            fontSizeMin_prop = serializedObject.FindProperty("m_fontSizeMin");
            fontSizeMax_prop = serializedObject.FindProperty("m_fontSizeMax");
            charSpacingMax_prop = serializedObject.FindProperty("m_charSpacingMax");
            lineSpacingMax_prop = serializedObject.FindProperty("m_lineSpacingMax");

			// Colors & Gradient
			fontColor_prop = serializedObject.FindProperty("m_fontColor");
			enableVertexGradient_prop = serializedObject.FindProperty ("m_enableVertexGradient");
			fontColorGradient_prop = serializedObject.FindProperty ("m_fontColorGradient");    
            overrideHtmlColor_prop = serializedObject.FindProperty("m_overrideHtmlColors");

            characterSpacing_prop = serializedObject.FindProperty("m_characterSpacing");
            lineSpacing_prop = serializedObject.FindProperty("m_lineSpacing");
            paragraphSpacing_prop = serializedObject.FindProperty("m_paragraphSpacing");

            textAlignment_prop = serializedObject.FindProperty("m_textAlignment");

            enableWordWrapping_prop = serializedObject.FindProperty("m_enableWordWrapping");
            wordWrappingRatios_prop = serializedObject.FindProperty("m_wordWrappingRatios");
            textOverflowMode_prop = serializedObject.FindProperty("m_overflowMode");

            horizontalMapping_prop = serializedObject.FindProperty("m_horizontalMapping");
            verticalMapping_prop = serializedObject.FindProperty("m_verticalMapping");
            uvOffset_prop = serializedObject.FindProperty("m_uvOffset");
            uvLineOffset_prop = serializedObject.FindProperty("m_uvLineOffset");
            
            enableKerning_prop = serializedObject.FindProperty("m_enableKerning");
        
 
            //isOrthographic_prop = serializedObject.FindProperty("m_isOrthographic");

            havePropertiesChanged_prop = serializedObject.FindProperty("havePropertiesChanged");
            inputSource_prop = serializedObject.FindProperty("m_inputSource");
            isInputPasingRequired_prop = serializedObject.FindProperty("isInputParsingRequired");
            //isAffectingWordWrapping_prop = serializedObject.FindProperty("isAffectingWordWrapping");
            enableExtraPadding_prop = serializedObject.FindProperty("m_enableExtraPadding");
            isRichText_prop = serializedObject.FindProperty("m_isRichText");
            checkPaddingRequired_prop = serializedObject.FindProperty("checkPaddingRequired");


            margin_prop = serializedObject.FindProperty("m_margin");
            
            //isMaskUpdateRequired_prop = serializedObject.FindProperty("isMaskUpdateRequired");
            //mask_prop = serializedObject.FindProperty("m_mask");
            maskOffset_prop= serializedObject.FindProperty("m_maskOffset");
            //maskOffsetMode_prop = serializedObject.FindProperty("m_maskOffsetMode");
            //maskSoftness_prop = serializedObject.FindProperty("m_maskSoftness");
            //vertexOffset_prop = serializedObject.FindProperty("m_vertexOffset");

            //sortingLayerID_prop = serializedObject.FindProperty("m_sortingLayerID");
            //sortingOrder_prop = serializedObject.FindProperty("m_sortingOrder");

            hasFontAssetChanged_prop = serializedObject.FindProperty("hasFontAssetChanged");
          
            // Find to location of the TextMesh Pro Asset Folder (as users may have moved it)
            string tmproAssetFolderPath = TMPro_EditorUtility.GetAssetLocation();

            if (EditorGUIUtility.isProSkin)
            {
                mySkin = AssetDatabase.LoadAssetAtPath(tmproAssetFolderPath + "/GUISkins/TMPro_DarkSkin.guiskin", typeof(GUISkin)) as GUISkin;

                alignLeft = AssetDatabase.LoadAssetAtPath(tmproAssetFolderPath + "/GUISkins/Textures/btn_AlignLeft.psd", typeof(Texture2D)) as Texture2D;
                alignCenter = AssetDatabase.LoadAssetAtPath(tmproAssetFolderPath + "/GUISkins/Textures/btn_AlignCenter.psd", typeof(Texture2D)) as Texture2D;
                alignRight = AssetDatabase.LoadAssetAtPath(tmproAssetFolderPath + "/GUISkins/Textures/btn_AlignRight.psd", typeof(Texture2D)) as Texture2D;
                alignJustified = AssetDatabase.LoadAssetAtPath(tmproAssetFolderPath + "/GUISkins/Textures/btn_AlignJustified.psd", typeof(Texture2D)) as Texture2D;
                alignTop = AssetDatabase.LoadAssetAtPath(tmproAssetFolderPath + "/GUISkins/Textures/btn_AlignTop.psd", typeof(Texture2D)) as Texture2D;
                alignMiddle = AssetDatabase.LoadAssetAtPath(tmproAssetFolderPath + "/GUISkins/Textures/btn_AlignMiddle.psd", typeof(Texture2D)) as Texture2D;
                alignBottom = AssetDatabase.LoadAssetAtPath(tmproAssetFolderPath + "/GUISkins/Textures/btn_AlignBottom.psd", typeof(Texture2D)) as Texture2D;
            }
            else
            {
                mySkin = AssetDatabase.LoadAssetAtPath(tmproAssetFolderPath + "/GUISkins/TMPro_LightSkin.guiskin", typeof(GUISkin)) as GUISkin;

                alignLeft = AssetDatabase.LoadAssetAtPath(tmproAssetFolderPath + "/GUISkins/Textures/btn_AlignLeft_Light.psd", typeof(Texture2D)) as Texture2D;
                alignCenter = AssetDatabase.LoadAssetAtPath(tmproAssetFolderPath + "/GUISkins/Textures/btn_AlignCenter_Light.psd", typeof(Texture2D)) as Texture2D;
                alignRight = AssetDatabase.LoadAssetAtPath(tmproAssetFolderPath + "/GUISkins/Textures/btn_AlignRight_Light.psd", typeof(Texture2D)) as Texture2D;
                alignJustified = AssetDatabase.LoadAssetAtPath(tmproAssetFolderPath + "/GUISkins/Textures/btn_AlignJustified_Light.psd", typeof(Texture2D)) as Texture2D;
                alignTop = AssetDatabase.LoadAssetAtPath(tmproAssetFolderPath + "/GUISkins/Textures/btn_AlignTop_Light.psd", typeof(Texture2D)) as Texture2D;
                alignMiddle = AssetDatabase.LoadAssetAtPath(tmproAssetFolderPath + "/GUISkins/Textures/btn_AlignMiddle_Light.psd", typeof(Texture2D)) as Texture2D;
                alignBottom = AssetDatabase.LoadAssetAtPath(tmproAssetFolderPath + "/GUISkins/Textures/btn_AlignBottom_Light.psd", typeof(Texture2D)) as Texture2D;
            }

            if (mySkin != null)
            {
                Section_Label = mySkin.FindStyle("Section Label");
                //Group_Label = mySkin.FindStyle("Group Label");
                textAreaBox = mySkin.FindStyle("Text Area Box (Editor)");

                alignContent_A = new GUIContent[] { 
                    new GUIContent(alignLeft, "Left"), 
                    new GUIContent(alignCenter, "Center"), 
                    new GUIContent(alignRight, "Right"), 
                    new GUIContent(alignJustified, "Justified") };

                alignContent_B = new GUIContent[] { 
                    new GUIContent(alignTop, "Top"), 
                    new GUIContent(alignMiddle, "Middle"), 
                    new GUIContent(alignBottom, "Bottom") };

            }

            m_textMeshProScript = (TextMeshProUGUI)target;
            m_rectTransform = Selection.activeGameObject.GetComponent<RectTransform>();
            m_uiRenderer = Selection.activeGameObject.GetComponent<CanvasRenderer>();

            // Add a Material Component if one does not exists
            /*
            m_materialComponent = Selection.activeGameObject.GetComponent<MaterialComponent> ();
			if (m_materialComponent == null) 
			{
				m_materialComponent = Selection.activeGameObject.AddComponent<MaterialComponent> ();
			}
            */


            // Create new Material Editor if one does not exists                       
            if (m_uiRenderer != null && m_uiRenderer.GetMaterial() != null)
            {               
                m_materialEditor = Editor.CreateEditor(m_uiRenderer.GetMaterial());
                m_targetMaterial = m_uiRenderer.GetMaterial();
                //Debug.Log("Currently Assigned Material is " + m_targetMaterial + ".  Font Material is " + m_textMeshProScript.fontSharedMaterial);
            }
            
            //m_updateManager = Camera.main.gameObject.GetComponent<TMPro_UpdateManager>();
        }


        public void OnDisable()
        {
            //Debug.Log("OnDisable() for GUIEditor Panel called.");
            Undo.undoRedoPerformed -= OnUndoRedo;
            
			// Destroy material editor if one exists
            if (m_materialEditor != null)
            {
                //Debug.Log("Destroying Inline Material Editor.");
                DestroyImmediate(m_materialEditor);
            }
            
			//Undo.postprocessModifications -= OnUndoRedoEvent;  
        }


        public override void OnInspectorGUI()
        {           
            serializedObject.Update();

            //EditorGUIUtility.LookLikeControls(150, 30);
            Rect rect;
            float labelWidth = EditorGUIUtility.labelWidth = 130f;
            float fieldWidth = EditorGUIUtility.fieldWidth;
          
            // TEXT INPUT BOX SECTION
            if (GUILayout.Button("<b>TEXT INPUT BOX</b>" + (m_foldout.textInput ? uiStateLabel[1] : uiStateLabel[0]), Section_Label))
                m_foldout.textInput = !m_foldout.textInput;

            if (m_foldout.textInput)
            {
                EditorGUI.BeginChangeCheck();
                text_prop.stringValue = EditorGUILayout.TextArea(text_prop.stringValue, textAreaBox, GUILayout.Height(125), GUILayout.ExpandWidth(true));
                if (EditorGUI.EndChangeCheck())
                {
                    inputSource_prop.enumValueIndex = 0;
                    isInputPasingRequired_prop.boolValue = true;
                    //isAffectingWordWrapping_prop.boolValue = true;
                    havePropertiesChanged = true;
                }
            }


            // FONT SETTINGS SECTION
            if (GUILayout.Button("<b>FONT SETTINGS</b>" + (m_foldout.fontSettings ? uiStateLabel[1] : uiStateLabel[0]), Section_Label))
                m_foldout.fontSettings = !m_foldout.fontSettings;

            if (m_foldout.fontSettings)
            {
                // FONT ASSET
                EditorGUI.BeginChangeCheck();
                EditorGUILayout.PropertyField(fontAsset_prop);
                if (EditorGUI.EndChangeCheck())
                {
                    //Undo.RecordObject(m_textMeshProScript, "Material Change");
                    havePropertiesChanged = true;
                    hasFontAssetChanged_prop.boolValue = true;                                
                }


                // FONT STYLE
                EditorGUI.BeginChangeCheck();

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.PrefixLabel("Font Style");
                int styleValue = fontStyle_prop.intValue;

                int v1 = GUILayout.Toggle((styleValue & 1) == 1, "B", GUI.skin.button) ? 1 : 0; // Bold
                int v2 = GUILayout.Toggle((styleValue & 2) == 2, "I", GUI.skin.button) ? 2 : 0; // Italics
                int v3 = GUILayout.Toggle((styleValue & 4) == 4, "U", GUI.skin.button) ? 4 : 0; // Underline
                int v4 = GUILayout.Toggle((styleValue & 8) == 8, "ab", GUI.skin.button) ? 8 : 0; // Lowercase
                int v5 = GUILayout.Toggle((styleValue & 16) == 16, "AB", GUI.skin.button) ? 16 : 0; // Uppercase
                int v6 = GUILayout.Toggle((styleValue & 32) == 32, "S", GUI.skin.button) ? 32 : 0; // Smallcaps
                EditorGUILayout.EndHorizontal();
                                      
                if (EditorGUI.EndChangeCheck())
                {                                    
                    fontStyle_prop.intValue = v1 + v2 + v3 + v4 + v5 + v6;                                    
                    havePropertiesChanged = true;
                }


                // FACE VERTEX COLOR
                EditorGUI.BeginChangeCheck();
                EditorGUILayout.PropertyField(fontColor_prop, new GUIContent("Face Color"));

                // VERTEX COLOR GRADIENT
                EditorGUILayout.BeginHorizontal();
                //EditorGUILayout.PrefixLabel("Color Gradient");               
				EditorGUILayout.PropertyField(enableVertexGradient_prop, new GUIContent("Color Gradient"), GUILayout.MinWidth(140), GUILayout.MaxWidth(200));
                EditorGUIUtility.labelWidth = 95;
                EditorGUILayout.PropertyField(overrideHtmlColor_prop, new GUIContent("Override Tags"));
                EditorGUIUtility.labelWidth = labelWidth;
                EditorGUILayout.EndHorizontal();

				if (enableVertexGradient_prop.boolValue)
				{
                    EditorGUILayout.PropertyField(fontColorGradient_prop.FindPropertyRelative("topLeft"), new GUIContent("Top Left"));
                    EditorGUILayout.PropertyField(fontColorGradient_prop.FindPropertyRelative("topRight"), new GUIContent("Top Right"));
                    EditorGUILayout.PropertyField(fontColorGradient_prop.FindPropertyRelative("bottomLeft"), new GUIContent("Bottom Left"));
                    EditorGUILayout.PropertyField(fontColorGradient_prop.FindPropertyRelative("bottomRight"), new GUIContent("Bottom Right"));
				}
                if (EditorGUI.EndChangeCheck())
                {
                    havePropertiesChanged = true;
                }


                // FONT SIZE
                EditorGUI.BeginChangeCheck();
                EditorGUILayout.BeginHorizontal();               
                EditorGUILayout.PropertyField(fontSize_prop, new GUIContent("Font Size"), GUILayout.MinWidth(168), GUILayout.MaxWidth(200));
                EditorGUIUtility.fieldWidth = fieldWidth;
                if (EditorGUI.EndChangeCheck())
                {
                    fontSizeBase_prop.floatValue = fontSize_prop.floatValue;
                    havePropertiesChanged = true;
                    //isAffectingWordWrapping_prop.boolValue = true;
                }

                EditorGUI.BeginChangeCheck();
                EditorGUIUtility.labelWidth = 70;
                EditorGUILayout.PropertyField(autoSizing_prop, new GUIContent("Auto Size"));
                EditorGUILayout.EndHorizontal();

                EditorGUIUtility.labelWidth = labelWidth;
                if (EditorGUI.EndChangeCheck())
                {
                    if (autoSizing_prop.boolValue == false)
                        fontSize_prop.floatValue = fontSizeBase_prop.floatValue;

                    havePropertiesChanged = true;
                    //isAffectingWordWrapping_prop.boolValue = true;
                }


                // Show auto sizing options
                if (autoSizing_prop.boolValue)
                {
                    EditorGUI.BeginChangeCheck();
                    
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.PrefixLabel("Auto Size Options");
                    EditorGUIUtility.labelWidth = 30;
                   
                    EditorGUILayout.PropertyField(fontSizeMin_prop, new GUIContent("Min"), GUILayout.MinWidth(50)); 
                    EditorGUILayout.PropertyField(fontSizeMax_prop, new GUIContent("Max"), GUILayout.MinWidth(50));                                 
                    //EditorGUILayout.PropertyField(charSpacingMax_prop, new GUIContent("Char"), GUILayout.MinWidth(50));
                    EditorGUILayout.PropertyField(lineSpacingMax_prop, new GUIContent("Line"), GUILayout.MinWidth(50)); 

                    EditorGUIUtility.labelWidth = labelWidth;                   
                    EditorGUILayout.EndHorizontal();

                    if (EditorGUI.EndChangeCheck())
                    {
                        charSpacingMax_prop.floatValue = Mathf.Min(0, charSpacingMax_prop.floatValue);
                        lineSpacingMax_prop.floatValue = Mathf.Min(0, lineSpacingMax_prop.floatValue);
                        havePropertiesChanged = true;
                        //isAffectingWordWrapping_prop.boolValue = true;
                    }
                }

                // CHARACTER, LINE & PARAGRAPH SPACING
                EditorGUI.BeginChangeCheck();

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.PrefixLabel("Spacing Options");
                EditorGUIUtility.labelWidth = 30;
                EditorGUILayout.PropertyField(characterSpacing_prop, new GUIContent("Char"), GUILayout.MinWidth(50)); //, GUILayout.MaxWidth(100));               
                EditorGUILayout.PropertyField(lineSpacing_prop, new GUIContent("Line"), GUILayout.MinWidth(50)); //, GUILayout.MaxWidth(100));                           
                EditorGUILayout.PropertyField(paragraphSpacing_prop, new GUIContent(" �"), GUILayout.MinWidth(50)); //, GUILayout.MaxWidth(100));

                EditorGUIUtility.labelWidth = labelWidth;
                EditorGUILayout.EndHorizontal();
                
                if (EditorGUI.EndChangeCheck())
                {
                    havePropertiesChanged = true;
                    //isAffectingWordWrapping_prop.boolValue = true;
                }


                // TEXT ALIGNMENT
                EditorGUI.BeginChangeCheck();
                rect = EditorGUILayout.GetControlRect(false, 17);
                GUIStyle btn = new GUIStyle(GUI.skin.button);
                btn.margin = new RectOffset(1, 1, 1, 1);
                btn.padding = new RectOffset(1, 1, 1, 0);

                selAlignGrid_A = textAlignment_prop.enumValueIndex & ~12;
                selAlignGrid_B = (textAlignment_prop.enumValueIndex & ~3) / 4;

                GUI.Label(new Rect(rect.x, rect.y, 100, rect.height), "Alignment");
                float columnB = EditorGUIUtility.labelWidth + 15;
                selAlignGrid_A = GUI.SelectionGrid(new Rect(columnB, rect.y, 23 * 4, rect.height), selAlignGrid_A, alignContent_A, 4, btn);
                selAlignGrid_B = GUI.SelectionGrid(new Rect(columnB + 23 * 4 + 10, rect.y, 23 * 3, rect.height), selAlignGrid_B, alignContent_B, 3, btn);

                textAlignment_prop.enumValueIndex = selAlignGrid_A + selAlignGrid_B * 4;

                // WRAPPING RATIOS shown if Justified mode is selected.
                if (textAlignment_prop.enumValueIndex == 3 || textAlignment_prop.enumValueIndex == 7 || textAlignment_prop.enumValueIndex == 11)
                    DrawPropertySlider("Wrap Mix (W <-> C)", wordWrappingRatios_prop);

                if (EditorGUI.EndChangeCheck())
                {
                    havePropertiesChanged = true;
                }


                // TEXT WRAPPING & OVERFLOW        
                EditorGUI.BeginChangeCheck();
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.PrefixLabel("Wrapping & Overflow");                
                int wrapSelection = EditorGUILayout.Popup(enableWordWrapping_prop.boolValue ? 1 : 0, new string[] { "Disabled", "Enabled" }, GUILayout.MinWidth(70f));
                enableWordWrapping_prop.boolValue = wrapSelection == 1 ? true : false;

                EditorGUILayout.PropertyField(textOverflowMode_prop, GUIContent.none, GUILayout.MinWidth(70f));
                EditorGUILayout.EndHorizontal();              
                if (EditorGUI.EndChangeCheck())
                {
                    havePropertiesChanged = true;
                    //isAffectingWordWrapping_prop.boolValue = true;
                    isInputPasingRequired_prop.boolValue = true;
                }

                        
                // TEXTURE MAPPING OPTIONS   
                EditorGUI.BeginChangeCheck();
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.PrefixLabel("UV Mapping Options");
                EditorGUILayout.PropertyField(horizontalMapping_prop, GUIContent.none, GUILayout.MinWidth(70f));
                EditorGUILayout.PropertyField(verticalMapping_prop, GUIContent.none, GUILayout.MinWidth(70f));
                EditorGUILayout.EndHorizontal();
                if (EditorGUI.EndChangeCheck())
                {
                    havePropertiesChanged = true;
                }

                // UV OPTIONS
                EditorGUI.BeginChangeCheck();
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.PrefixLabel("UV Offset");                
                EditorGUILayout.PropertyField(uvOffset_prop, GUIContent.none, GUILayout.MinWidth(70f));
                EditorGUIUtility.labelWidth = 30;
                EditorGUILayout.PropertyField(uvLineOffset_prop, new GUIContent("Line"), GUILayout.MinWidth(70f));
                EditorGUIUtility.labelWidth = labelWidth;              
                EditorGUILayout.EndHorizontal();        
                if (EditorGUI.EndChangeCheck())
                {
                    havePropertiesChanged = true;
                }
                

                // KERNING
                EditorGUI.BeginChangeCheck();
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.PropertyField(enableKerning_prop, new GUIContent("Enable Kerning?"));
                if (EditorGUI.EndChangeCheck())
                {
                    //isAffectingWordWrapping_prop.boolValue = true;
                    havePropertiesChanged = true;
                }

                // EXTRA PADDING
                EditorGUI.BeginChangeCheck();
                EditorGUILayout.PropertyField(enableExtraPadding_prop, new GUIContent("Extra Padding?"));
                if (EditorGUI.EndChangeCheck())
                {
                    havePropertiesChanged = true;
                    checkPaddingRequired_prop.boolValue = true;
                }
                EditorGUILayout.EndHorizontal();
            }



            if (GUILayout.Button("<b>EXTRA SETTINGS</b>" + (m_foldout.extraSettings ? uiStateLabel[1] : uiStateLabel[0]), Section_Label))
                m_foldout.extraSettings = !m_foldout.extraSettings;

            if (m_foldout.extraSettings)
            {
                EditorGUI.indentLevel = 0;

                DrawMaginProperty(margin_prop, "Margins");
                DrawMaginProperty(maskOffset_prop, "Mask Offset");

                //EditorGUILayout.BeginHorizontal();
                EditorGUI.BeginChangeCheck();
                //EditorGUILayout.PropertyField(sortingLayerID_prop);
                //EditorGUILayout.PropertyField(sortingOrder_prop);

                //EditorGUILayout.EndHorizontal();

                //EditorGUILayout.PropertyField(isOrthographic_prop, new GUIContent("Orthographic Mode?"));
                EditorGUILayout.PropertyField(isRichText_prop, new GUIContent("Enable Rich Text?"));
                //EditorGUILayout.PropertyField(textRectangle_prop, true);

                if (EditorGUI.EndChangeCheck())
                    havePropertiesChanged = true;


                // EditorGUI.BeginChangeCheck();
                //EditorGUILayout.PropertyField(mask_prop);
                //EditorGUILayout.PropertyField(maskOffset_prop, true);
                //EditorGUILayout.PropertyField(maskSoftness_prop);
                //if (EditorGUI.EndChangeCheck())
                //{
                //    isMaskUpdateRequired_prop.boolValue = true;
                //    havePropertiesChanged = true;
                //}

                //EditorGUILayout.PropertyField(sortingLayerID_prop);
                //EditorGUILayout.PropertyField(sortingOrder_prop);

                // Mask Selection
            }

            EditorGUILayout.Space();

       
            // If a Custom Material Editor exists, we use it.
            if (m_uiRenderer != null && m_uiRenderer.GetMaterial() != null)
            {
                Material mat = m_uiRenderer.GetMaterial();

                //Debug.Log(mat + "  " + m_targetMaterial);
                
                if (mat != m_targetMaterial)
                {
                    // Destroy previous Material Instance
                    //Debug.Log("New Material has been assigned.");
                    m_targetMaterial = mat;
                    DestroyImmediate(m_materialEditor);
                }
           

                if (m_materialEditor == null)
                {
                    m_materialEditor = Editor.CreateEditor(mat);
                }

                // Define the Drag-n-Drop Region (Start)
                m_inspectorStartRegion = GUILayoutUtility.GetRect(0f, 0f, GUILayout.ExpandWidth(true));
               
                m_materialEditor.DrawHeader();
                //EditorGUILayout.PropertyField(fontSharedMaterial_prop);

                m_materialEditor.OnInspectorGUI();
                
                // Define the Drag-n-Drop Region (End)
                m_inspectorEndRegion = GUILayoutUtility.GetRect(0f, 0f, GUILayout.ExpandWidth(true));

            }

                       
            //DragAndDropGUI();


            if (havePropertiesChanged)
            {
                //Debug.Log("Properties have changed.");                
                havePropertiesChanged_prop.boolValue = true;
                havePropertiesChanged = false;
                //EditorUtility.SetDirty(target);
                //m_updateManager.ScheduleObjectForUpdate(m_textMeshProScript);

            }
            
            serializedObject.ApplyModifiedProperties();

            //m_targetMaterial = m_uiRenderer.GetMaterial();
        }


         
        private void DragAndDropGUI()
        {
            Event evt = Event.current;

            Rect dropArea = new Rect(m_inspectorStartRegion.x, m_inspectorStartRegion.y, m_inspectorEndRegion.width, m_inspectorEndRegion.y - m_inspectorStartRegion.y);
           
            switch (evt.type)
            {
                case EventType.dragUpdated:
                case EventType.DragPerform:
                    if (!dropArea.Contains(evt.mousePosition))
                        break;

                    DragAndDrop.visualMode = DragAndDropVisualMode.Generic;

                    if (evt.type == EventType.DragPerform)
                    {                      
                        DragAndDrop.AcceptDrag();
                  
                        // Do something                   
                        Material mat = DragAndDrop.objectReferences[0] as Material;
                        //Debug.Log("Drag-n-Drop Material is " + mat + ". Target Material is " + m_targetMaterial + ".  Canvas Material is " + m_uiRenderer.GetMaterial()  );
                        
                        // Check to make sure we have a valid material and that the font atlases match.
                        if (!mat || mat == m_uiRenderer.GetMaterial() || mat.GetTexture(ShaderUtilities.ID_MainTex).GetInstanceID() != m_textMeshProScript.font.atlas.GetInstanceID())
                        {
                            if (mat && mat.GetTexture(ShaderUtilities.ID_MainTex).GetInstanceID() != m_textMeshProScript.font.atlas.GetInstanceID())
                                Debug.LogWarning("Drag-n-Drop Material [" + mat.name + "]'s Atlas does not match the assigned Font Asset [" + m_textMeshProScript.font.name + "]'s Atlas."); 
                            break;
                        }
                        
                        fontSharedMaterial_prop.objectReferenceValue = mat;
                        //fontBaseMaterial_prop.objectReferenceValue = mat;
                        isNewBaseMaterial_prop.boolValue = true;
                        //TMPro_EventManager.ON_DRAG_AND_DROP_MATERIAL_CHANGED(m_textMeshProScript, mat);                        
                        EditorUtility.SetDirty(target);
                                                                
                        //havePropertiesChanged = true;                     
                    }
            
                    evt.Use();
                break;
            }
        }


        

        // DRAW MARGIN PROPERTY
        private void DrawMaginProperty(SerializedProperty property, string label)
        {
            float old_LabelWidth = EditorGUIUtility.labelWidth;
            float old_FieldWidth = EditorGUIUtility.fieldWidth;

            Rect rect = EditorGUILayout.GetControlRect(false, 2 * 18);
            Rect pos0 = new Rect(rect.x, rect.y + 2, rect.width, 18);

            float width = rect.width + 3;
            pos0.width = old_LabelWidth;
            GUI.Label(pos0, label);

            Vector4 vec = property.vector4Value;

            float widthB = width - old_LabelWidth;
            float fieldWidth = widthB / 4;
            pos0.width = fieldWidth - 5;

            // Labels
            pos0.x = old_LabelWidth + 15;
            GUI.Label(pos0, "Left");

            pos0.x += fieldWidth;
            GUI.Label(pos0, "Top");

            pos0.x += fieldWidth;
            GUI.Label(pos0, "Right");

            pos0.x += fieldWidth;
            GUI.Label(pos0, "Bottom");

            pos0.y += 18;

            pos0.x = old_LabelWidth + 15;
            vec.x = EditorGUI.FloatField(pos0, GUIContent.none, vec.x);

            pos0.x += fieldWidth;
            vec.y = EditorGUI.FloatField(pos0, GUIContent.none, vec.y);

            pos0.x += fieldWidth;
            vec.z = EditorGUI.FloatField(pos0, GUIContent.none, vec.z);

            pos0.x += fieldWidth;
            vec.w = EditorGUI.FloatField(pos0, GUIContent.none, vec.w);

            property.vector4Value = vec;

            EditorGUIUtility.labelWidth = old_LabelWidth;
            EditorGUIUtility.fieldWidth = old_FieldWidth;
        }



        public void OnSceneGUI()
        {
            // Margin Frame & Handles               
            m_rectTransform.GetWorldCorners(m_rectCorners);
            Vector4 marginOffset = m_textMeshProScript.margin;

            handlePoints[0] = m_rectCorners[0] + m_rectTransform.TransformDirection(new Vector3(marginOffset.x, marginOffset.w, 0));
            handlePoints[1] = m_rectCorners[1] + m_rectTransform.TransformDirection(new Vector3(marginOffset.x, -marginOffset.y, 0));
            handlePoints[2] = m_rectCorners[2] + m_rectTransform.TransformDirection(new Vector3(-marginOffset.z, -marginOffset.y, 0));
            handlePoints[3] = m_rectCorners[3] + m_rectTransform.TransformDirection(new Vector3(-marginOffset.z, marginOffset.w, 0));

            Handles.DrawSolidRectangleWithOutline(handlePoints, new Color32(255, 255, 255, 0), new Color32(255, 255, 0, 255));

            // Draw & process FreeMoveHandles

            // LEFT HANDLE
            Vector3 old_left = (handlePoints[0] + handlePoints[1]) * 0.5f;
            Vector3 new_left = Handles.FreeMoveHandle(old_left, Quaternion.identity, HandleUtility.GetHandleSize(m_rectTransform.position) * 0.05f, Vector3.zero, Handles.DotCap);
            bool hasChanged = false;
            if (old_left != new_left)
            {
                float delta = old_left.x - new_left.x;
                marginOffset.x += -delta;
                //Debug.Log("Left Margin H0:" + handlePoints[0] + "  H1:" + handlePoints[1]);
                hasChanged = true;
            }

            // TOP HANDLE
            Vector3 old_top = (handlePoints[1] + handlePoints[2]) * 0.5f;
            Vector3 new_top = Handles.FreeMoveHandle(old_top, Quaternion.identity, HandleUtility.GetHandleSize(m_rectTransform.position) * 0.05f, Vector3.zero, Handles.DotCap);
            if (old_top != new_top)
            {
                float delta = old_top.y - new_top.y;
                marginOffset.y += delta;
                //Debug.Log("Top Margin H1:" + handlePoints[1] + "  H2:" + handlePoints[2]);   
                hasChanged = true;
            }

            // RIGHT HANDLE
            Vector3 old_right = (handlePoints[2] + handlePoints[3]) * 0.5f;
            Vector3 new_right = Handles.FreeMoveHandle(old_right, Quaternion.identity, HandleUtility.GetHandleSize(m_rectTransform.position) * 0.05f, Vector3.zero, Handles.DotCap);
            if (old_right != new_right)
            {
                float delta = old_right.x - new_right.x;
                marginOffset.z += delta;
                hasChanged = true;
                //Debug.Log("Right Margin H2:" + handlePoints[2] + "  H3:" + handlePoints[3]);
            }

            // BOTTOM HANDLE
            Vector3 old_bottom = (handlePoints[3] + handlePoints[0]) * 0.5f;
            Vector3 new_bottom = Handles.FreeMoveHandle(old_bottom, Quaternion.identity, HandleUtility.GetHandleSize(m_rectTransform.position) * 0.05f, Vector3.zero, Handles.DotCap);
            if (old_bottom != new_bottom)
            {
                float delta = old_bottom.y - new_bottom.y;
                marginOffset.w += -delta;
                hasChanged = true;
                //Debug.Log("Bottom Margin H0:" + handlePoints[0] + "  H3:" + handlePoints[3]);
            }

            if (hasChanged)
            {
                Undo.RecordObjects(new Object[] {m_rectTransform, m_textMeshProScript }, "Margin Changes");
                m_textMeshProScript.margin = marginOffset;
                EditorUtility.SetDirty(target);
                //m_textMeshProScript.ForceMeshUpdate();
            }
        }


        void DrawPropertySlider(string label, SerializedProperty property)
        {
            float old_LabelWidth = EditorGUIUtility.labelWidth;
            float old_FieldWidth = EditorGUIUtility.fieldWidth;

            Rect rect = EditorGUILayout.GetControlRect(false, 17);

            //EditorGUIUtility.labelWidth = m_labelWidth;

            GUIContent content = label == "" ? GUIContent.none : new GUIContent(label);
            EditorGUI.Slider(new Rect(rect.x, rect.y, rect.width, rect.height), property, 0.0f, 1.0f, content);

            EditorGUIUtility.labelWidth = old_LabelWidth;
            EditorGUIUtility.fieldWidth = old_FieldWidth;
        }


        private void DrawDimensionProperty(SerializedProperty property, string label)
        {
            float old_LabelWidth = EditorGUIUtility.labelWidth;
            float old_FieldWidth = EditorGUIUtility.fieldWidth;

            Rect rect = EditorGUILayout.GetControlRect(false, 18);
            Rect pos0 = new Rect(rect.x, rect.y + 2, rect.width, 18);

            float width = rect.width + 3;
            pos0.width = old_LabelWidth;
            GUI.Label(pos0, label);

            Rect rectangle = property.rectValue;

            float width_B = width - old_LabelWidth;
            float fieldWidth = width_B / 4;
            pos0.width = fieldWidth - 5;

            pos0.x = old_LabelWidth + 15;
            GUI.Label(pos0, "Width");

            pos0.x += fieldWidth;
            rectangle.width = EditorGUI.FloatField(pos0, GUIContent.none, rectangle.width);

            pos0.x += fieldWidth;
            GUI.Label(pos0, "Height");

            pos0.x += fieldWidth;
            rectangle.height = EditorGUI.FloatField(pos0, GUIContent.none, rectangle.height);

            property.rectValue = rectangle;
            EditorGUIUtility.labelWidth = old_LabelWidth;
            EditorGUIUtility.fieldWidth = old_FieldWidth;
        }



        void DrawPropertyBlock(string[] labels, SerializedProperty[] properties)
        {
            float old_LabelWidth = EditorGUIUtility.labelWidth;
            float old_FieldWidth = EditorGUIUtility.fieldWidth;

            Rect rect = EditorGUILayout.GetControlRect(false, 17);
            GUI.Label(new Rect(rect.x, rect.y, old_LabelWidth, rect.height), labels[0]);

            rect.x = old_LabelWidth + 15;
            rect.width = (rect.width + 20 - rect.x) / labels.Length;

            for (int i = 0; i < labels.Length; i++)
            {
                if (i == 0)
                {
                    EditorGUIUtility.labelWidth = 20;
                    EditorGUI.PropertyField(new Rect(rect.x - 20, rect.y, 75, rect.height), properties[i], new GUIContent("  "));
                    rect.x += rect.width;
                }
                else
                {
                    EditorGUIUtility.labelWidth = GUI.skin.textArea.CalcSize(new GUIContent(labels[i])).x;
                    EditorGUI.PropertyField(new Rect(rect.x, rect.y, rect.width - 5, rect.height), properties[i], new GUIContent(labels[i]));
                    rect.x += rect.width;
                }

            }

            EditorGUIUtility.labelWidth = old_LabelWidth;
            EditorGUIUtility.fieldWidth = old_FieldWidth;
        }



        // Special Handling of Undo / Redo Events.
        private void OnUndoRedo()
        {
            //int undoEventID = Undo.GetCurrentGroup();
            //int LastUndoEventID = m_eventID;

            //Debug.Log(m_textMeshProScript.fontMaterial);
            /*
            if (undoEventID != LastUndoEventID)
            {
                for (int i = 0; i < targets.Length; i++)
                {
                    //Debug.Log("Undo & Redo Performed detected in Editor Panel. Event ID:" + Undo.GetCurrentGroup());
                    TMPro_EventManager.ON_TEXTMESHPRO_PROPERTY_CHANGED(true, targets[i] as TextMeshPro);
                    m_eventID = undoEventID;
                }
            }
            */
        }

        /*
        private UndoPropertyModification[] OnUndoRedoEvent(UndoPropertyModification[] modifications)
        {
            int eventID = Undo.GetCurrentGroup();
            PropertyModification modifiedProp = modifications[0].propertyModification;      
            System.Type targetType = modifiedProp.target.GetType();
              
            if (targetType == typeof(Material))
            {
                //Debug.Log("Undo / Redo Event Registered in Editor Panel on Target: " + targetObject);
           
                //TMPro_EventManager.ON_MATERIAL_PROPERTY_CHANGED(true, targetObject as Material);
                //EditorUtility.SetDirty(targetObject);        
            }
  
            //string propertyPath = modifications[0].propertyModification.propertyPath;  
            //if (propertyPath == "m_fontAsset")
            //{
                //int currentEvent = Undo.GetCurrentGroup();
                //Undo.RecordObject(Selection.activeGameObject.renderer.sharedMaterial, "Font Asset Changed");
                //Undo.CollapseUndoOperations(currentEvent);
                //Debug.Log("Undo / Redo Event: Font Asset changed. Event ID:" + Undo.GetCurrentGroup());
            
            //}

            //Debug.Log("Undo / Redo Event Registered in Editor Panel on Target: " + modifiedProp.propertyPath + "  Undo Event ID:" + eventID + "  Stored ID:" + TMPro_EditorUtility.UndoEventID);
            //TextMeshPro_EventManager.ON_TEXTMESHPRO_PROPERTY_CHANGED(true, target as TextMeshPro);
            return modifications;
        }
        */
    }
}

#endif