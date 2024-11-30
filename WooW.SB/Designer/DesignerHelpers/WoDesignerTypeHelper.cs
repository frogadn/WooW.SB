using WooW.Core;

namespace WooW.SB.Designer.DesignerHelpers
{
    public class WoDesignerTypeHelper
    {
        private string _modelName = string.Empty;

        public WoDesignerTypeHelper(string modelName)
        {
            _modelName = modelName;
        }

        public string DesignerTypeToCodeType(
            string designerType,
            bool isNullable,
            string attributeName
        )
        {
            string typeResult = string.Empty;

            if (
                designerType == "Timestamp"
                || designerType == "timestamp"
                || designerType == "Date"
                || designerType == "date"
                || designerType == "datetime"
            )
                typeResult = "DateTime";
            if (designerType == "Smallint" || designerType == "smallint")
                typeResult = "short";
            if (designerType == "Boolean" || designerType == "boolean")
                typeResult = "bool";
            if (
                designerType == "Reference"
                || designerType == "reference"
                || designerType == "Urn"
                || designerType == "urn"
                || designerType == "String"
            )
                typeResult = "string";
            if (designerType == "Integer" || designerType == "integer")
                typeResult = "int";
            if (designerType == "Clob" || designerType == "clob")
                typeResult = "string";
            if (designerType == "Blob" || designerType == "blob")
                typeResult = "byte[]";
            if (designerType == "Long" || designerType == "long")
                typeResult = designerType.ToLower();

            if (designerType == "Double")
                typeResult = "double";

            if (
                attributeName == "RowVersion"
                || attributeName == "rowVersion"
                || designerType == "Autoincrement"
                || designerType == "autoincrement"
            )
                typeResult = "ulong";

            if (
                designerType == "EnumInt"
                || designerType == "enumint"
                || designerType == "EnumString"
                || designerType == "enumstring"
                || designerType == "WoState"
                || designerType == "wostate"
            )
                typeResult = $@"e{_modelName}_{attributeName}";

            if (typeResult.IsNullOrStringEmpty())
                typeResult = designerType;

            if (isNullable)
                typeResult = $@"{typeResult}?";

            return typeResult;
        }
    }
}
