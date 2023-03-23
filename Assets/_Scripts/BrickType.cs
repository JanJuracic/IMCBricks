using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SO / BrickType")]
public class BrickType : ScriptableObject
{
    [SerializeField] private List<Sprite> sprites = new();
    [SerializeField] private int spriteIndex = 0;

    public Sprite GetNextSprite()
    {
        var sprite = sprites[spriteIndex];
        spriteIndex = Utilities.IntWrapAround(spriteIndex + 1, sprites.Count - 1);
        return sprite;
    }
}
