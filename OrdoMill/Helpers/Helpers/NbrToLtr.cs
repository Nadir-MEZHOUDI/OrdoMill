namespace OrdoMill.Helpers
{
    public static class NbrToLtr
    {
        private static Language _l;
        private static string[] _g0T9;
        private static string[] _g10T19;
        private static string[] _g10Jump;
        private static string _hundred = string.Empty;
        private static string _thousand = string.Empty;
        private static string _million = string.Empty;
        private static string _billion = string.Empty;
        private static string _join = string.Empty;
        private static string _bigNumError = string.Empty;

        private static readonly string[] Ar0T9 =
        {
            "���", "����", "�����", "�����", "�����", "����", "���", "����", "������", "����"
        };

        private static readonly string[] En0T9 =
        {
            "Zero", "One", "Two", "Three", "Four", "Five", "Six", "Seven", "Eight", "Nine"
        };

        private static readonly string[] Fr0T9 =
        {
            "z�ro", "Un", "deux", "trois", "quatre", "cinq", "six", "sept", "huit", "neuf"
        };

        private static readonly string[] Ar10T19 =
        {
            "", "����", "���� ���", "���� ���", "����� ���", "����� ���", "���� ���", "��� ���", "���� ���", "������ ���", "���� ���"
        };

        private static readonly string[] En10T19 =
        {
            "", "Ten", "Eleven", "Twelve", "Thirteen", "Fouteen", "Fifteen", "Sixteen", "Seventeen", "Eighteen", "Nineteen"
        };

        private static readonly string[] Fr10T19 =
        {
            "", "dix", "onze", "douze", "treize", "quatorze", "quinze", "Seize", "dix-sept", "dix-huit", "dix-neuf"
        };

        private static readonly string[] Ar10Jump =
        {
            "", "", "�����", "������", "������", "�����", "����", "�����", "������", "�����"
        };

        private static readonly string[] En10Jump =
        {
            "", "", "Twenty", "Thirty", "Forty", "Fifty", "Sixty", "Seventy", "Eighty", "Ninety"
        };

        private static readonly string[] Fr10Jump =
        {
            "", "", "Vingt", "trente", "quarante", "cinquante", "soixante", "soixante-dix", "quatre-vingts", "quatre-vingt dix"
        };

        private static readonly string ArHundred = "����";
        private static readonly string EnHundred = "Hundred";
        private static readonly string FrHundred = "cent";
        private static readonly string ArThousand = "���";
        private static readonly string EnThousand = "Thousand";
        private static readonly string FrThousand = "mille";
        private static readonly string ArMillion = "�����";
        private static readonly string EnMillion = "Million";
        private static readonly string FrMillion = "million";
        private static readonly string ArBillion = "�����";
        private static readonly string EnBillion = "Billion";
        private static readonly string FrBillion = "milliard";
        private static readonly string ArJoin = " � ";
        private static readonly string EnJoin = " and ";
        private static readonly string FrJoin = " - ";
        private static readonly string ArBigNumError = "����� ���� ���";
        private static readonly string EnBigNumError = "The number is very big";
        private static readonly string FrBigNumError = "Le nombre est tr�s grand";

        private static string Get0T9(string s)
        {
            return _g0T9[int.Parse(s)];
        }

        private static string Get10T19(string s)
        {
            return _g10T19[int.Parse(s) - 9];
        }

        private static string Get10Jmp(string s)
        {
            return _g10Jump[int.Parse(s[0].ToString())];
        }

        private static string GetArHtmb(string s, char htmb)
        {
            switch (htmb)
            {
                case 'H':
                    if (s == "1") return " ���� ";
                    if (s == "2") return " ������ ";
                    return Get0T9(s).Replace("�", string.Empty) + " ���� ";
                case 'T':
                    if (s == "1") return " ��� ";
                    if (s == "2") return " ����� ";
                    if (s == "10" || s.Length == 1) return Get0T99(s) + " ���� ";
                    if (s.Length == 3 && (s[2].ToString() == "1" || s[2].ToString() == "2"))
                        return GetArHtmb(s[0].ToString(), 'H') + " ��� � " + GetArHtmb(s[2].ToString(), 'T');
                    if (s.Length == 3 && int.Parse(s.Substring(1)) <= 10 && int.Parse(s.Substring(1)) > 2)
                        return Get0T999(s) + " ���� ";
                    return Get0T999(s) + " ���";
                case 'M':
                    if (s == "1") return " ����� ";
                    if (s == "2") return " ������ ";
                    if (s == "10" || s.Length == 1) return Get0T99(s) + " ������ ";
                    if (s.Length == 3 && (s[2].ToString() == "1" || s[2].ToString() == "2"))
                        return GetArHtmb(s[0].ToString(), 'H') + " ����� � " + GetArHtmb(s[2].ToString(), 'M');
                    if (s.Length == 3 && int.Parse(s.Substring(1)) <= 10 && int.Parse(s.Substring(1)) > 2)
                        return Get0T999(s) + " ������ ";
                    return Get0T999(s) + " �����";
                case 'B':
                    if (s == "1") return " ����� ";
                    if (s == "2") return " ������ ";
                    if (s == "10" || s.Length == 1) return Get0T99(s) + " ������ ";
                    if (s.Length == 3 && (s[2].ToString() == "1" || s[2].ToString() == "2"))
                        return GetArHtmb(s[0].ToString(), 'H') + " ����� � " + GetArHtmb(s[2].ToString(), 'B');
                    if (s.Length == 3 && int.Parse(s.Substring(1)) <= 10 && int.Parse(s.Substring(1)) > 2)
                        return Get0T999(s) + " ������ ";
                    return Get0T999(s) + " �����";
                default:
                    return string.Empty;
            }
        }

        private static string GetLength2Nbr(string s)
        {
            if (_l == Language.French)
            {
                switch (s)
                {
                    case "21":
                        return "vingt et un";
                    case "31":
                        return " trente et un";
                    case "41":
                        return "quarante et un";
                    case "51":
                        return "cinquante et un";
                    case "61":
                        return "soixante et un";
                    case "81":
                        return "quatre-vingt un";
                }

                if (s[0] == '7')
                {
                    return Get10Jmp("60") + _join + Get10T19((int.Parse(s) - 60).ToString());
                }

                if (s[0] == '9')
                {
                    return Get10Jmp("80") + _join + Get10T19((int.Parse(s) - 80).ToString());
                }
            }

            if (int.Parse(s) % 10 == 0)
            {
                return Get10Jmp(s);
            }

            if (_l == Language.Arabic)
            {
                return Get0T9(s[1].ToString()) + _join + Get10Jmp(s[0] + "0");
            }

            return Get10Jmp(s[0] + "0") + " " + Get0T9(s[1].ToString());
        }

        private static string Get0T99(string s)
        {
            string number = int.Parse(s).ToString();
            if (number.Length == 2)
            {
                int x = int.Parse(number);
                if (x >= 10 && x < 20)
                {
                    return Get10T19(number);
                }

                return GetLength2Nbr(number);
            }

            return Get0T9(number);
        }

        private static string Get0T999(string s)
        {
            string number = int.Parse(s).ToString();
            if (number.Length <= 2)
            {
                return Get0T99(number);
            }

            if (_l == Language.French && number[0] == '1')
            {
                return $" {_hundred} " + Get0T99(number.Substring(1));
            }

            string ss = Get0T99(number.Substring(1));
            if (_l == Language.Arabic)
            {
                if (ss == string.Empty) return GetArHtmb(number[0].ToString(), 'H');
                return GetArHtmb(number[0].ToString(), 'H') + _join + ss;
            }

            if (ss == string.Empty) return Get0T9(number[0].ToString()) + $" {_hundred}";
            return Get0T9(number[0].ToString()) + $" {_hundred}{_join}" + ss;
        }

        private static string Get0T999999(string s)
        {
            string number = int.Parse(s).ToString();
            if (number.Length <= 3)
            {
                return Get0T999(number);
            }

            string hundred = number.Substring(number.Length - 3);
            string thousand = number.Remove(number.Length - 3);
            string ss = Get0T999(hundred);
            if (_l == Language.Arabic)
            {
                if (ss == string.Empty) return GetArHtmb(thousand, 'T');
                return GetArHtmb(thousand, 'T') + " � " + ss;
            }

            if (ss == string.Empty) return Get0T999(thousand) + $" {_thousand}";
            return Get0T999(thousand) + $" {_thousand}{_join}" + ss;
        }

        private static string Get0T999999999(string s)
        {
            if (s.Length <= 6)
            {
                return Get0T999999(s);
            }

            string thousand = s.Substring(s.Length - 6);
            string million = s.Remove(s.Length - 6);
            string ss = Get0T999999(thousand);
            if (_l == Language.Arabic)
            {
                if (ss == string.Empty) return GetArHtmb(million, 'M');
                return GetArHtmb(million, 'M') + _join + ss;
            }

            if (ss == string.Empty) return Get0T999(million) + $" {_million} ";
            return Get0T999(million) + $" {_million}{_join}" + ss;
        }

        public static string Convert(ulong number, Language language)
        {
            ConfugLanguageElements(language);
            if (number > 999999999999)
            {
                return _bigNumError;
            }

            string s = number.ToString();
            if (s.Length <= 9)
            {
                return Get0T999999999(s);
            }

            string million = s.Substring(s.Length - 9);
            string billion = s.Remove(s.Length - 9);
            string ss = Get0T999999999(million);
            if (_l == Language.Arabic)
            {
                if (ss == string.Empty) return GetArHtmb(billion, 'B');
                return GetArHtmb(billion, 'B') + _join + ss;
            }

            if (ss == string.Empty) return Get0T999(billion) + $" {_billion} ";
            return Get0T999(billion) + $" {_billion}{_join}" + ss;
        }

        public static string ConvertAr(ulong number)
        {
            return Convert(number, Language.Arabic);
        }

        public static string ConvertEn(ulong number)
        {
            return Convert(number, Language.English);
        }

        public static string ConvertFr(decimal number)
        {
            return Convert((ulong)number, Language.French);
        }

        private static void ConfugLanguageElements(Language language)
        {
            _l = language;
            switch (language)
            {
                case Language.Arabic:
                    _g0T9 = Ar0T9;
                    _g10T19 = Ar10T19;
                    _g10Jump = Ar10Jump;
                    _hundred = ArHundred;
                    _thousand = ArThousand;
                    _million = ArMillion;
                    _billion = ArBillion;
                    _join = ArJoin;
                    _bigNumError = ArBigNumError;
                    break;
                case Language.French:
                    _g0T9 = Fr0T9;
                    _g10T19 = Fr10T19;
                    _g10Jump = Fr10Jump;
                    _hundred = FrHundred;
                    _thousand = FrThousand;
                    _million = FrMillion;
                    _billion = FrBillion;
                    _join = FrJoin;
                    _bigNumError = FrBigNumError;
                    break;
                case Language.English:
                    _g0T9 = En0T9;
                    _g10T19 = En10T19;
                    _g10Jump = En10Jump;
                    _hundred = EnHundred;
                    _thousand = EnThousand;
                    _million = EnMillion;
                    _billion = EnBillion;
                    _join = EnJoin;
                    _bigNumError = EnBigNumError;
                    break;
            }
        }
    }
}
