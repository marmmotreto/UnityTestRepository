﻿// Copyright (C) 2014 Stephan Bouchard - All Rights Reserved
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms


using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections;
using System.Collections.Generic;


namespace TMPro.EditorUtilities
{

    public static class TMPro_EditorUtility
    {
        // Static Fields Related to locating the TextMesh Pro Asset
        private static bool isTMProFolderLocated;
        private static string folderPath = "Not Found";
        
        private static EditorWindow Gameview;
        private static bool isInitialized = false;

        private static void GetGameview()
        {
            System.Reflection.Assembly assembly = typeof(UnityEditor.EditorWindow).Assembly;
            System.Type type = assembly.GetType("UnityEditor.GameView");
            Gameview = EditorWindow.GetWindow(type);
        }


        public static void RepaintAll()
        {
            if (isInitialized == false)
            {
                GetGameview();
                isInitialized = true;
            }

            SceneView.RepaintAll();
            Gameview.Repaint();
        }


        // Function used to find all materials which reference a font atlas so we can update all their references.
        public static Material[] FindMaterialReferences(Material mat)
        {
            // Find all Materials used in the project.
            Material[] mats = Resources.FindObjectsOfTypeAll<Material>();
            List<Material> refs = new List<Material>();

            for (int i = 0; i < mats.Length; i++)
            {
                if (mats[i].HasProperty(ShaderUtilities.ID_MainTex) && mats[i].GetTexture(ShaderUtilities.ID_MainTex) == mat.GetTexture(ShaderUtilities.ID_MainTex))
                {
                    refs.Add(mats[i]);
                    //Debug.Log(mats[i].name);
                }
            }

            return refs.ToArray();
        }


        public static string GetAssetLocation()
        {

            if (isTMProFolderLocated == false)
            {
                isTMProFolderLocated = true;               
                string projectPath = Directory.GetCurrentDirectory();               
                folderPath = Directory.GetDirectories(projectPath + "/Assets", "TextMesh Pro", SearchOption.AllDirectories)[0];
                folderPath = "Assets" + folderPath.Split(new string[] { "/Assets" }, System.StringSplitOptions.None)[1];
            }
            return folderPath;
        }

    }
}
