using System.Collections.Generic;
using UnityEngine;

namespace QuickMafs
{
    [CreateAssetMenu(menuName = "QuickMafs/FontSprites")]
    public class FontSprites : ScriptableObject
    {
        [SerializeField]
        private LetterToSprite[] FontLettersMap;

        private Dictionary<Letter, Sprite> _map;

        public Dictionary<Letter, Sprite> Map
        {
            get
            {
                if(_map == null && FontLettersMap != null)
                {
                    _map = new Dictionary<Letter, Sprite>();
                    for(int i = 0; i < FontLettersMap.Length; i++)
                    {
                        _map.Add(FontLettersMap[i].Letter, FontLettersMap[i].Sprite);
                    }
                }
                return _map;
            }
        }

        public Sprite GetSpriteForLetter(Letter letter)
        {
            return Map[letter];
        }

        public static Letter GetRandomLetter()
        {
            var array = System.Enum.GetValues(typeof(Letter));
            return (Letter)array.GetValue(Random.Range(0, array.Length));
        }
    }

    [System.Serializable]
    public class LetterToSprite
    {
        public Letter Letter;
        public Sprite Sprite;
    }

    public enum Letter
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
