using System.Collections;
using UnityEngine;
using UnityEditor;

public class TextureChange : AssetPostprocessor
{
    void OnPostprocessTexture(Texture2D texture)
    {
        TextureImporter importer = assetImporter as TextureImporter;
        Debug.Log(importer.assetPath);
        //Set mipmap disable
        importer.mipmapEnabled = false;
        importer.textureType = TextureImporterType.Sprite;
        //Save the changes
        EditorUtility.SetDirty(importer);
    }
}