using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class TextureChange : AssetPostprocessor {
    /*纹理图片导入的过程中，钩子函数调用顺序为：
     * 进入导入阶段前：OnPreporcessTexture
     * 导入阶段后：OnPostprocessTexture
    */
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
