// Copyright (C) 2014 Stephan Bouchard - All Rights Reserved
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms


using UnityEngine;
using System;
using System.Collections.Generic;


namespace TMPro
{
    
  
    public struct TMPro_MeshInfo
    {           
        public Vector3[] vertices;
        public Vector2[] uv0s;
        public Vector2[] uv2s;
        public Color32[] vertexColors;
        public Vector3[] normals;
        public Vector4[] tangents;
#if UNITY_4_6 || UNITY_5_0
        public UIVertex[] uiVertices;
#endif
    }
       

    // Structure containing information about each Character & releated Mesh info for the text object.   
    public struct TMPro_CharacterInfo 
    {     
        public char character;
        public short lineNumber;
        public short charNumber;
        public short index;
        public short vertexIndex;
        public Vector3 topLeft;
        public Vector3 bottomLeft;
        public Vector3 topRight;
        public Vector3 bottomRight;
        public float topLine;      
        public float baseLine;
        public float bottomLine;     
        public float aspectRatio;
        public float padding;
        public float scale;
        public Color32 color;
        public FontStyles style;       
        public bool isVisible;
    }


    public struct TMPro_TextMetrics
    {
        public int characterCount;
        public int wordCount;
        public int spaceCount;
        public int lineCount;
        public Rect textRect;
    }


    [Serializable]
    public struct VertexGradient
    {
        public Color topLeft;
        public Color topRight;
        public Color bottomLeft;
        public Color bottomRight;

        public VertexGradient (Color color)
        {
            this.topLeft = color;
            this.topRight = color;
            this.bottomLeft = color;
            this.bottomRight = color;
        }

        public VertexGradient(Color color0, Color color1, Color color2, Color color3)
        {
            this.topLeft = color0;
            this.topRight = color1;
            this.bottomLeft = color2;
            this.bottomRight = color3;
        }
    }


    [Serializable]
    public class TextInfo
    {
        // These first 3 fields could be replaced by the TextMetrics      
        public int characterCount;
        public int spaceCount;
        public int wordCount;
        public int lineCount;
        public float minWidth;

        public TMPro_CharacterInfo[] characterInfo;
        public List<WordInfo> wordInfo;
        public LineInfo[] lineInfo;
        public TMPro_MeshInfo meshInfo;

        // Might was to add bounds in here.
    }


    public struct WordInfo
    {
        public int firstCharacterIndex;
        public int lastCharacterIndex;
        public int characterCount;
        public float length;
        public string word;
    }


    public struct LineInfo
    {
        public int characterCount;
        public int spaceCount;
        public int wordCount;
        public int firstCharacterIndex;
        public int lastCharacterIndex;
        public float lineLength;
        public float lineHeight;
        public float ascender;
        public float descender;

        public TextAlignmentOptions alignment;
        public Extents lineExtents;

    }


    public struct SpriteInfo
    {
        
    }


    public struct Extents
    {
        public Vector2 min;
        public Vector2 max;

        public Extents(Vector2 min, Vector2 max)
        {
            this.min = min;
            this.max = max;
        }

        public override string ToString()
        {
            string s = "Min (" + min.x.ToString("f2") + ", " + min.y.ToString("f2") + ")   Max (" + max.x.ToString("f2") + ", " + max.y.ToString("f2") + ")";           
            return s;
        }
    }


    [Serializable]
    public struct Mesh_Extents
    {
        public Vector2 min;
        public Vector2 max;
      
     
        public Mesh_Extents(Vector2 min, Vector2 max)
        {
            this.min = min;
            this.max = max;           
        }

        public override string ToString()
        {
            string s = "Min (" + min.x.ToString("f2") + ", " + min.y.ToString("f2") + ")   Max (" + max.x.ToString("f2") + ", " + max.y.ToString("f2") + ")";
            //string s = "Center: (" + ")" + "  Extents: (" + ((max.x - min.x) / 2).ToString("f2") + "," + ((max.y - min.y) / 2).ToString("f2") + ").";
            return s;
        }
    }


    // Structure used for Word Wrapping which tracks the state of execution when the last space or carriage return character was encountered. 
    public struct WordWrapState
    {
        public int previous_WordBreak;     
        public int total_CharacterCount;
        public int visible_CharacterCount;
        public float maxAscender;
        public float maxDescender;
        public float maxFontScale;
      
        public int wordCount;
        public FontStyles fontStyle;
        public float fontScale;
        public float xAdvance;
        public float currentFontSize;
        public float baselineOffset;
        public float lineOffset;

        public TextInfo textInfo;
        //public TMPro_CharacterInfo[] characterInfo;
        public LineInfo lineInfo;
        
        public Color32 vertexColor;
        public Extents meshExtents;
        //public Mesh_Extents lineExtents;    
    }


    // Structure used to track & restore state of previous line which is used to adjust linespacing.
    public struct LineWrapState
    {
        public int previous_LineBreak;
        public int total_CharacterCount;
        public int visible_CharacterCount;
        public float maxAscender;
        public float maxDescender;
        public float maxFontScale;

        //public float maxLineLength;
        public int wordCount;
        public FontStyles fontStyle;
        public float fontScale;
        public float xAdvance;
        public float currentFontSize;
        public float baselineOffset;
        public float lineOffset;

        public TextInfo textInfo;
        public LineInfo lineInfo;

        public Color32 vertexColor;
        public Extents meshExtents;
        //public Mesh_Extents lineExtents;    
    }

}
