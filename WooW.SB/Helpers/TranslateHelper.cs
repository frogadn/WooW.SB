using System;
using System.Net;
using System.Web;

namespace WooW.SB.Helpers
{
    public static class TranslateHelper
    {
        public static string[] TraduccirAccion = new string[]
        {
            "Renglón Actual",
            "Siguientes 10",
            "Siguientes 50"
        };

        public static int Renglones(string Accion)
        {
            if (Accion == TraduccirAccion[0])
                return 1;
            else if (Accion == TraduccirAccion[1])
                return 10;
            else if (Accion == TraduccirAccion[2])
                return 50;
            else
                return 0;
        }

        public static string Do(string word, string fromLanguage, string toLanguage)
        {
            if (string.IsNullOrEmpty(word))
                return string.Empty;

            var url =
                $"https://translate.googleapis.com/translate_a/single?client=gtx&sl={fromLanguage}&tl={toLanguage}&dt=t&q={HttpUtility.UrlEncode(word)}";
            var webClient = new WebClient { Encoding = System.Text.Encoding.UTF8 };
            var result = webClient.DownloadString(url);
            try
            {
                result = result.Substring(4, result.IndexOf("\"", 4, StringComparison.Ordinal) - 4);

                if (word.Substring(0, 1) == word.Substring(0, 1).ToUpper())
                    if (result.Length > 1)
                        result = result.Substring(0, 1).ToUpper() + result.Substring(1);
                    else
                        result = result.Substring(0, 1).ToUpper();

                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
