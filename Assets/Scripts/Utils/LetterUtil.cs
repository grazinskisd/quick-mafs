using UnityEngine;

namespace QuickMafs
{
    public class LetterUtil
    {
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

        public static string LetterToString(Letter letter)
        {
            switch (letter)
            {
                case Letter.L_0:
                    return "0";
                case Letter.L_1:
                    return "1";
                case Letter.L_2:
                    return "2";
                case Letter.L_3:
                    return "3";
                case Letter.L_4:
                    return "4";
                case Letter.L_5:
                    return "5";
                case Letter.L_6:
                    return "6";
                case Letter.L_7:
                    return "7";
                case Letter.L_8:
                    return "8";
                case Letter.L_9:
                    return "9";
                case Letter.L_plus:
                    return "+";
                case Letter.L_minus:
                    return "-";
                default:
                    return "0";
            }
        }
    }
}
