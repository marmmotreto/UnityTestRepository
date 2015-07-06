using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;



public static class MaterialManager {
   
    private static List<MaskingMaterial> m_materialList = new List<MaskingMaterial>();
	
    public static Material GetMaskingMaterial(Material baseMaterial, int stencilID)
    {
        // Check if Material supports masking
        if (!baseMaterial.HasProperty(ShaderUtilities.ID_StencilID))
        {
            Debug.LogWarning("Selected Shader does not support Stencil Masking. Please select the Distance Field or Mobile Distance Field Shader.");
            return baseMaterial;
        }
        
        Material maskingMaterial = null;
        
        // Check if baseMaterial already has a masking material associated with it.
        int index = m_materialList.FindIndex(item => item.baseMaterial == baseMaterial && item.stencilID == stencilID);

        if (index == -1)
        {             
            //Create new Masking Material Instance for this Base Material 
            maskingMaterial = new Material(baseMaterial);
            //maskingMaterial.hideFlags = HideFlags.HideAndDontSave;
            maskingMaterial.name += " Masking";
            maskingMaterial.shaderKeywords = baseMaterial.shaderKeywords;

            // Set Stencil Properties
            ShaderUtilities.GetShaderPropertyIDs();
            maskingMaterial.SetFloat(ShaderUtilities.ID_StencilID, stencilID);
            maskingMaterial.SetFloat(ShaderUtilities.ID_StencilComp, 3);

            MaskingMaterial temp = new MaskingMaterial();
            temp.baseMaterial = baseMaterial;
            temp.maskingMaterial = maskingMaterial;
            temp.stencilID = stencilID;
            temp.count = 1;

            m_materialList.Add(temp);

            //Debug.Log("Masking material for " + baseMaterial.name + " DOES NOT exists. Creating new " + maskingMaterial.name + " with ID " + maskingMaterial.GetInstanceID() + " which is used " + temp.count + " time(s).");           

        }
        else
        {                     
            maskingMaterial = m_materialList[index].maskingMaterial;
            m_materialList[index].count += 1; 
            
            //Debug.Log("Masking material for " + baseMaterial.name + " already exists. Passing reference to " + maskingMaterial.name + " with ID " + maskingMaterial.GetInstanceID() + " which is used " + m_materialList[index].count + " time(s).");           
        }

        return maskingMaterial;
    }



    public static void AddMaskingMaterial(Material baseMaterial, Material maskingMaterial, int stencilID)
    {
        // Check if maskingMaterial already has a base material associated with it.
        int index = m_materialList.FindIndex(item => item.maskingMaterial == maskingMaterial);

        if (index == -1)
        {
            MaskingMaterial temp = new MaskingMaterial();
            temp.baseMaterial = baseMaterial;
            temp.maskingMaterial = maskingMaterial;
            temp.stencilID = stencilID;
            temp.count = 1;

            m_materialList.Add(temp);
        }
        else
        {
            maskingMaterial = m_materialList[index].maskingMaterial;
            m_materialList[index].count += 1; 
        }
    }



    public static void RemoveMaskingMaterial(Material maskingMaterial)
    {
        // Check if maskingMaterial is already on the list.
        int index = m_materialList.FindIndex(item => item.maskingMaterial == maskingMaterial);

        if (index != -1)
        {
            m_materialList.RemoveAt(index);
        }
    }



    public static void ReleaseBaseMaterial(Material baseMaterial)
    {
        // Check if baseMaterial already has a masking material associated with it.
        int index = m_materialList.FindIndex(item => item.baseMaterial == baseMaterial);

        if (index == -1)
        {
            Debug.Log("No Masking Material exists for " + baseMaterial.name);
        }
        else
        {
            if (m_materialList[index].count > 1)
            {
                m_materialList[index].count -= 1;
                Debug.Log("Removed (1) reference to " + m_materialList[index].maskingMaterial.name + ". There are " + m_materialList[index].count + " references left.");
            }
            else
            {
                Debug.Log("Removed last reference to " + m_materialList[index].maskingMaterial.name + " with ID " + m_materialList[index].maskingMaterial.GetInstanceID()); 
                Object.DestroyImmediate(m_materialList[index].maskingMaterial);
                m_materialList.RemoveAt(index);
            }
        }

        ListMaterials();
    }



    public static void ReleaseMaskingMaterial(Material maskingMaterial)
    {
        // Check if baseMaterial already has a masking material associated with it.
        int index = m_materialList.FindIndex(item => item.maskingMaterial == maskingMaterial);
   
        if (index == -1)
        {
            //Debug.Log("No Masking Material exists for " + maskingMaterial.name);
        }
        else
        {
            if (m_materialList[index].count > 1)
            {
                m_materialList[index].count -= 1;
                //Debug.Log("Removed (1) reference to " + m_materialList[index].maskingMaterial.name + ". There are " + m_materialList[index].count + " references left.");
            }
            else
            {
                //Debug.Log("Removed last reference to " + m_materialList[index].maskingMaterial.name + " with ID " + m_materialList[index].maskingMaterial.GetInstanceID());
                Object.DestroyImmediate(m_materialList[index].maskingMaterial);
                m_materialList.RemoveAt(index);           
            }
        }

        ListMaterials();
    }


    
    public static void ListMaterials()
    {
        if (m_materialList.Count() == 0)
        {
            //Debug.Log("Material List is empty.");
            return;
        }

        //Debug.Log("List contains " + m_materialList.Count() + " items.");
        
        for (int i = 0; i < m_materialList.Count(); i ++)
        {
            Material baseMaterial = m_materialList[i].baseMaterial;
            Material maskingMaterial = m_materialList[i].maskingMaterial;
            
            Debug.Log("Item #" + (i + 1) + " - Base Material is [" + baseMaterial.name + "] with ID " + baseMaterial.GetInstanceID() + " is associated with [" + (maskingMaterial != null ? maskingMaterial.name : "Null") + "] with ID " + (maskingMaterial != null ? maskingMaterial.GetInstanceID() : 0) + " and is referenced " + m_materialList[i].count + " time(s).");
        }
    }
    


    private class MaskingMaterial
    {
        public Material baseMaterial;
        public Material maskingMaterial;
        public int count;
        public int stencilID;      
    }

}
