using UnityEngine;
using UnityEditor;
using Wastepunk.Cooking;
using Wastepunk.Ingredients;
using System;

namespace Wastepunk.EditorScripts
{
    public static class CactusBuilderMenu
    {
        [MenuItem("Wastepunk/Generate Cactus Prototype")]
        public static void GenerateCactusPrefab()
        {
            // 0. Ensure IngredientData exists
            string dataPath = "Assets/_ScriptableObjects/Ingredients";
            if (!AssetDatabase.IsValidFolder("Assets/_ScriptableObjects"))
                AssetDatabase.CreateFolder("Assets", "_ScriptableObjects");
            if (!AssetDatabase.IsValidFolder("Assets/_ScriptableObjects/Ingredients"))
                AssetDatabase.CreateFolder("Assets/_ScriptableObjects", "Ingredients");

            string assetPath = dataPath + "/CactusData.asset";
            IngredientData cactusData = AssetDatabase.LoadAssetAtPath<IngredientData>(assetPath);
            if (cactusData == null)
            {
                cactusData = ScriptableObject.CreateInstance<IngredientData>();
                
                SerializedObject soData = new SerializedObject(cactusData);
                soData.FindProperty("ingredientId").stringValue = "cactus";
                soData.FindProperty("ingredientName").stringValue = "Cactus";
                soData.FindProperty("defaultState").intValue = (int)IngredientState.Raw;
                soData.ApplyModifiedProperties();

                AssetDatabase.CreateAsset(cactusData, assetPath);
                AssetDatabase.SaveAssets();
            }

            // 1. Create root Cactus object (uniform scale)
            GameObject cactusRoot = new GameObject("Cactus_Prototype");
            
            // Add Rigidbody to root
            Rigidbody rbCactus = cactusRoot.AddComponent<Rigidbody>();
            rbCactus.mass = 2f;

            // Add IngredientBase and assign data
            IngredientBase ingredientBase = cactusRoot.AddComponent<IngredientBase>();
            SerializedObject soIng = new SerializedObject(ingredientBase);
            soIng.FindProperty("ingredientData").objectReferenceValue = cactusData;
            soIng.ApplyModifiedProperties();
            
            // Add Cactus logic
            Cactus cactusScript = cactusRoot.AddComponent<Cactus>();
            
            // Add Capsule Collider to root
            CapsuleCollider rootCollider = cactusRoot.AddComponent<CapsuleCollider>();
            rootCollider.height = 2f;
            rootCollider.radius = 0.25f;

            // Try Add XRGrabInteractable
            Type xriGrabType = Type.GetType("UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable, Unity.XR.Interaction.Toolkit") 
                            ?? Type.GetType("UnityEngine.XR.Interaction.Toolkit.XRGrabInteractable, Unity.XR.Interaction.Toolkit");
                           
            if (xriGrabType != null)
                cactusRoot.AddComponent(xriGrabType);
            else
                Debug.LogError("Could not find XRGrabInteractable type! Make sure XR Interaction Toolkit is installed.");

            // 2. Create Visuals
            GameObject cactusVisual = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            cactusVisual.name = "Visuals";
            cactusVisual.transform.SetParent(cactusRoot.transform);
            cactusVisual.transform.localPosition = Vector3.zero;
            cactusVisual.transform.localScale = new Vector3(0.5f, 1f, 0.5f);
            GameObject.DestroyImmediate(cactusVisual.GetComponent<Collider>());

            // 3. Create Needles
            int needleCount = 5;
            SerializedObject soCactus = new SerializedObject(cactusScript);
            soCactus.FindProperty("totalNeedles").intValue = needleCount;
            soCactus.ApplyModifiedProperties();

            for (int i = 0; i < needleCount; i++)
            {
                GameObject needle = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                needle.name = $"Needle_{i}";
                needle.transform.localScale = new Vector3(0.05f, 0.3f, 0.05f);
                
                // Position around the cactus
                float angle = i * Mathf.PI * 2f / needleCount;
                Vector3 offset = new Vector3(Mathf.Sin(angle) * 0.25f, 0, Mathf.Cos(angle) * 0.25f);
                needle.transform.position = cactusRoot.transform.position + offset;
                
                // Orient needle outward
                needle.transform.rotation = Quaternion.LookRotation(offset) * Quaternion.Euler(90, 0, 0);
                
                // Parent to root, not visual, to avoid skewing
                needle.transform.SetParent(cactusRoot.transform);
                
                Rigidbody rbNeedle = needle.AddComponent<Rigidbody>();
                rbNeedle.mass = 0.1f;
                
                // Add FixedJoint
                FixedJoint joint = needle.AddComponent<FixedJoint>();
                joint.connectedBody = rbCactus;
                joint.breakForce = 150f; 
                joint.breakTorque = 150f;
                
                // Add CactusNeedle logic
                CactusNeedle needleScript = needle.AddComponent<CactusNeedle>();
                SerializedObject soNeedle = new SerializedObject(needleScript);
                soNeedle.FindProperty("parentCactus").objectReferenceValue = cactusScript;
                soNeedle.ApplyModifiedProperties();
                
                // Add XRGrabInteractable to needle
                if (xriGrabType != null)
                    needle.AddComponent(xriGrabType);
            }
            
            // 4. Save as Prefab
            string prefabPath = "Assets/_Prefabs";
            if (!AssetDatabase.IsValidFolder("Assets/_Prefabs"))
            {
                AssetDatabase.CreateFolder("Assets", "_Prefabs");
            }
            
            PrefabUtility.SaveAsPrefabAsset(cactusRoot, prefabPath + "/Cactus_Prototype.prefab");
            
            // Cleanup scene object
            GameObject.DestroyImmediate(cactusRoot);
            Debug.Log("Cactus Prefab generated successfully at " + prefabPath + "/Cactus_Prototype.prefab");
        }
    }
}
