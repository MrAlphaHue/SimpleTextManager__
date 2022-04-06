
//Warn : Don't change this code.
//Generated By MrHue.SimpleDataConverter

using System;
using UnityEngine;
using System.Collections.Generic;


namespace MrHue.Text
{
    public enum ELANGUAGE
    {
        NULL,
		EN,
		KR,
		JP,
		CH_SIM,
		CH_TR,
		GE,
		RU,
		SP,
		FR,

    }

    public enum ETexTableType
    {
        NULL = 0,
        EMPTY = 99,
		SampleCommon = 100,

    }

    public static class TextManager
    {
        //
        public class TextStruct
        {
			public string en = null; 	 //English
			public string kr = null; 	 //Korean
			public string jp = null; 	 //Japnese
			public string ch_sim = null; 	 //Chinese Simplified
			public string ch_tr = null; 	 //Chinese Traditional
			public string ge = null; 	 //German
			public string ru = null; 	 //Russian
			public string sp = null; 	 //Spanish
			public string fr = null; 	 //French

        }

        private static Dictionary<int, TextStruct> _txDic_id = new Dictionary<int, TextStruct>();
        private static Dictionary<string, int> _txIdDic_strKey = new Dictionary<string, int>();

        private static ELANGUAGE _curLanguage;
        private static List<ELANGUAGE> _languagePriorities;
        private static bool _ifNoneGetOther = false;

        public static void SetLanguage(ELANGUAGE elanguage)
        {
            _curLanguage = elanguage;
        }

        //If getTex is Null , Then Check Other Languages
        public static void SetLanguagePriority(List<ELANGUAGE> elanguages, bool ifNoneGetOther)
        {
            _languagePriorities.Clear();
            for (int i = 0; i < elanguages.Count; i++)
            {
                _languagePriorities.Add(elanguages[i]);
            }

            _ifNoneGetOther = ifNoneGetOther;
        }


        private static bool IsNone(string str)
        {
            return str == null ||  str == "None" || str == "";
        }


        public static string GetText(ETexTableType type, int index)
        {
            return GetText(type, (int) type + index, _curLanguage);;
        }

        public static string GetText(ETexTableType type, string strKey)
        {
            return GetText(type, strKey, _curLanguage);
        }

        public static string GetText(ETexTableType type, int index, ELANGUAGE langType)
        {
            string errMsg = string.Format("Text/{0}/id_{1}/lang_{2}", type.ToString(), index.ToString(),
                langType.ToString());
            string _out = GetText((int) type + index, langType);

            if (IsNone(_out))
                return errMsg;

            return _out;
        }

        public static string GetText(ETexTableType type, string strKey, ELANGUAGE langType)
        {
            string errMsg = string.Format("Text/{0}/strKey_{1}/lang_{2}", type.ToString(), strKey, langType.ToString());
            strKey = string.Format("{0}_{1}_{2}", "ETEXTTYPE", type.ToString(), strKey);
            string _out = GetText(strKey, langType);
            
            if (IsNone(_out)) 
                return errMsg;

            return _out;
        }

        private static string GetText(string strKey, ELANGUAGE langType)
        {
            if (_txIdDic_strKey.ContainsKey(strKey) == false)
                return null;
            return GetText(_txIdDic_strKey[strKey], langType);
        }
        
        private static string GetText(int index, ELANGUAGE langType)
        {
            string _out = null;
            if (_txDic_id.ContainsKey(index) == false)
                return null;

            _out = Func_GetTextByLanguage(_txDic_id[index], langType);
            
            if (IsNone(_out) && _ifNoneGetOther == true)
            {
                _out = GetText_ByLanguagePriority(index, langType);
            }
            return _out;
        }

        private static string GetText_ByLanguagePriority(int index, ELANGUAGE baseLangType)
        {
            string _out = null;
            for (int i = 0; i < _languagePriorities.Count; i++)
            {
                if (_languagePriorities[i] == baseLangType) continue;
                _out = Func_GetTextByLanguage(_txDic_id[index], _languagePriorities[i]);
                if (IsNone(_out) == false)
                    break;
            }
            return _out;
        }

        private static string Func_GetTextByLanguage(TextStruct textStruct, ELANGUAGE langType)
        {
            string _out = null;
            switch (langType)
            {
				case ELANGUAGE.EN:
				_out = textStruct.en;
				break;
				case ELANGUAGE.KR:
				_out = textStruct.kr;
				break;
				case ELANGUAGE.JP:
				_out = textStruct.jp;
				break;
				case ELANGUAGE.CH_SIM:
				_out = textStruct.ch_sim;
				break;
				case ELANGUAGE.CH_TR:
				_out = textStruct.ch_tr;
				break;
				case ELANGUAGE.GE:
				_out = textStruct.ge;
				break;
				case ELANGUAGE.RU:
				_out = textStruct.ru;
				break;
				case ELANGUAGE.SP:
				_out = textStruct.sp;
				break;
				case ELANGUAGE.FR:
				_out = textStruct.fr;
				break;

            }

            return _out;
        }
        
        
        // [RuntimeInitializeOnLoadMethod]
        // Must Be Static If You want "RuntimeInitializeOnLoadMethod"
        public static void InIt()
        {
            _txDic_id.Clear();
            _txIdDic_strKey.Clear();

			InIt(ETexTableType.SampleCommon);

        }

        private static void InIt(ETexTableType type)
        {
            //
            var text_asset = Resources.Load("BinaryTextData/TexTable" + type.ToString()) as TextAsset;
            string getBytestring = System.Text.Encoding.UTF8.GetString(text_asset.bytes); //U2gi
            byte[] byte1 = Convert.FromBase64String(getBytestring);
            string final_str = System.Text.Encoding.UTF8.GetString(byte1);

            //strs0 is id,strkey,en,kr
            string[] strs = final_str.Split(new string[] {"\t\t"}, StringSplitOptions.None);

            //
            string firstKey = strs[0];
            string[] firstKeys = firstKey.Split('\t');

            //
            int start_index = (int) type;
            string base_str = string.Format("{0}_{1}_", "ETEXTTYPE", type.ToString());

            //
            for (int i = 1; i < strs.Length; i++)
            {
                string element = strs[i];
                string[] elements = element.Split('\t');

                if (elements.Length != firstKeys.Length)
                {
                    Debug.LogError(elements.Length + "   ,   " + firstKeys.Length);
                    throw new ArgumentException("strs");
                }

                //
                TextStruct textStruct = new TextStruct();
                int index = start_index + int.Parse(elements[0]);
                string strKey = base_str + elements[1];

                //
                for (int idx_el = 2; idx_el < elements.Length; idx_el++)
                {
                    string keyValue = firstKeys[idx_el].ToUpper();
                    string strValue = elements[idx_el];
                    // Debug.LogError("strValue : " + strValue);
                    switch (keyValue)
                    {
						case "EN":
						textStruct.en = strValue;
						break;
						case "KR":
						textStruct.kr = strValue;
						break;
						case "JP":
						textStruct.jp = strValue;
						break;
						case "CH_SIM":
						textStruct.ch_sim = strValue;
						break;
						case "CH_TR":
						textStruct.ch_tr = strValue;
						break;
						case "GE":
						textStruct.ge = strValue;
						break;
						case "RU":
						textStruct.ru = strValue;
						break;
						case "SP":
						textStruct.sp = strValue;
						break;
						case "FR":
						textStruct.fr = strValue;
						break;

                        default:
                            Debug.LogError("KeyValue Error : " + keyValue);
                            return;
                    }
                }

                //
                if (_txDic_id.ContainsKey(index))
                {
                    Debug.LogError("TextGenerateError_Multiple Key Index");
                    return;
                }

                _txDic_id.Add(index, textStruct);

                if (elements[1] != "None")
                {
                    if (_txIdDic_strKey.ContainsKey(strKey))
                    {
                        Debug.LogError("TextGenerateError_Multiple StrKey Index");
                        return;
                    }
                    _txIdDic_strKey.Add(strKey, index);
                }
            }
        }
    }
}