using System.Collections.Generic;
using UnityEngine;

namespace QuickMafs
{
    [CreateAssetMenu(menuName = "QuickMafs/FontSprites")]
    public class FontSprites : ScriptableObject
    {
        [SerializeField]
        private LetterToSprite[] FontLettersMap;

        private Dictionary<Letters, Sprite> _map;

        public Dictionary<Letters, Sprite> Map
        {
            get
            {
                if(_map == null && FontLettersMap != null)
                {
                    _map = new Dictionary<Letters, Sprite>();
                    for(int i = 0; i < FontLettersMap.Length; i++)
                    {
                        _map.Add(FontLettersMap[i].Letter, FontLettersMap[i].Sprite);
                    }
                }
                return _map;
            }
        }

        public Sprite GetRandomLetterSprite()
        {
            return Map[GetRandomLetter()];
        }

        private Letters GetRandomLetter()
        {
            var array = System.Enum.GetValues(typeof(Letters));
            return (Letters)array.GetValue(Random.Range(0, array.Length));
        }
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
