using UnityEngine;

namespace QuickMafs
{
    [CreateAssetMenu(menuName = "QuickMafs/FontSprites")]
    public class FontSprites : ScriptableObject
    {
        public LetterToSprite[] FontLetterMap;
    }

    [System.Serializable]
    public class LetterToSprite
    {
        public Letters Letter;
        public Sprite Sprite;
    }

    public enum Letters
    {
        L_0,
        L_1,
        L_2,
        L_3,
        L_4,
        L_5,
        L_6,
        L_7,
        L_8,
        L_9,
        L_plus,
        L_minus
    }
}
