using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using WooW.Core;
using WooW.SB.UI;

namespace WooW.SB.Config
{
    public class Etiqueta
    {
        private string id;

        public Etiqueta()
        {
            Id = String.Empty;
            Idiomas = new List<EtiquetaIdioma>();
        }

        public Etiqueta(string parId, string Idioma, string Texto)
        {
            Id = parId;
            Idiomas = new List<EtiquetaIdioma>();
            Idiomas.Add(new EtiquetaIdioma(Idioma, Texto));
        }


        [WoRegEx(@"[A-Z][a-zA-Z0-9]*")]
        public string Id { get => id; set => id = (value == null ? string.Empty : value.Trim()); }

        public List<EtiquetaIdioma> Idiomas { get; set; }

        public override string ToString()
        {
            if (Idiomas.Count == 0)
                return $"{Id}";
            else
                return $"{Id}-{Idiomas[0]}";
        }

        public static string ToId(string Texto)
        {
            var normalizedString = Texto.Normalize(NormalizationForm.FormD);
            var sb2 = new StringBuilder();
            for (int i = 0; i < normalizedString.Length; i++)
            {
                var uc = System.Globalization.CharUnicodeInfo.GetUnicodeCategory(normalizedString[i]);
                if (uc != System.Globalization.UnicodeCategory.NonSpacingMark)
                {
                    sb2.Append(normalizedString[i]);
                }
            }

            var TextoNormalizado = (sb2.ToString().Normalize(NormalizationForm.FormC));

            StringBuilder sb = new StringBuilder();

            bool bMayuscula = true;
            foreach (char c in TextoNormalizado)
            {
                if (char.IsWhiteSpace(c))
                {
                    bMayuscula = true;
                    continue;
                }

                if (char.IsLetterOrDigit(c))
                {
                    char C;

                    if (bMayuscula)
                        C = Char.ToUpper(c);
                    else
                        C = Char.ToLower(c);

                    bMayuscula = false;

                    sb.Append(C);
                }
            }

            return sb.ToString();
        }

        public string ToJson()
        {
            return JsonConvert.SerializeObject(this);
        }

        public static Etiqueta FromJson(string Json)
        {
            if (Json.IsNullOrStringEmpty())
                return new Etiqueta();
            else
                return JsonConvert.DeserializeObject<Etiqueta>(Json);
        }

        public Etiqueta Clone()
        {
            return JsonConvert.DeserializeObject<Etiqueta>(JsonConvert.SerializeObject(this));
        }


    }
}