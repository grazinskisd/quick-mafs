using System.Collections.Generic;
using UnityEngine;

namespace QuickMafs
{
    [CreateAssetMenu(menuName = "QuickMafs/FontSettings")]
    public class FontSettings : ScriptableObject
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

        public static Letter RandomLetter()
        {
            var array = System.Enum.GetValues(typeof(Letter));
            return (Letter)array.GetValue(Random.Range(1, array.Length));
        }

        public static Letter RandomNumber()
        {
            return (Letter)Random.Range((int)Letter.L_1, (int)Letter.L_plus);
        }

        public static Letter RandomSymbol()
        {
            return (Letter)Random.Range((int)Letter.L_plus, (int)Letter.L_minus + 1);
        }

        [System.Serializable]
        public class LetterToSprite
        {
            public Letter Letter;
            public Sprite Sprite;
        }
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
