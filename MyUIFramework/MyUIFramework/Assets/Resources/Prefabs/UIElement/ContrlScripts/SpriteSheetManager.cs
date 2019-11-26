using System.Collections.Generic;
using UnityEngine;

public class SpriteSheetManager
{
    //将精灵列表中包含的精灵缓存的字典
    private static Dictionary<string, Dictionary<string, Sprite>> spriteSheets = new Dictionary<string, Dictionary<string, Sprite>>();

    //将精灵列表中包含的精灵读取出来并缓存的方法
    public static void Load(string path)
    {
        if (!spriteSheets.ContainsKey(path))
        {
            spriteSheets.Add(path, new Dictionary<string, Sprite>());
        }

        //读取精灵,缓存名称与路径
        Sprite[] sprites = Resources.LoadAll<Sprite>(path);
        foreach (Sprite sprite in sprites)
        {
            if (!spriteSheets[path].ContainsKey(sprite.name))
            {
                spriteSheets[path].Add(sprite.name, sprite);
            }
        }
    }

    //由精灵名称返回精灵列表中包含的精灵的方法
    public static Sprite GetSpriteByName(string path,string name)
    {
        if (spriteSheets.ContainsKey(path) && spriteSheets[path].ContainsKey(name))
        {
            return spriteSheets[path][name];
        }
        return null;
    }
}
