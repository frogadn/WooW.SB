using DevExpress.Data.Filtering;

namespace WooW.SB.Config.Helpers
{
    public static class ODataHelpers
    {
        public static string FormatODataQueryString(CriteriaOperator _filter)
        {
            bool firstParam = true;
            string sResult = "";

            GroupOperator opGroup = _filter as GroupOperator;
            if (ReferenceEquals(opGroup, null))
            {
                sResult += FormatODataQueryStringParameter(_filter);
            }
            else
            {
                foreach (var opn in opGroup.Operands)
                {
                    if (!firstParam)
                    {
                        sResult += $" {opGroup.OperatorType} ";
                    }
                    firstParam = false;
                    sResult += FormatODataQueryStringParameter(opn);
                }
            }

            return sResult;
        }

        public static string FormatODataQueryStringParameter(CriteriaOperator _op)
        {
            string _path = "";
            BinaryOperator opBinary = _op as BinaryOperator;
            if (ReferenceEquals(opBinary, null))
            {
                FunctionOperator opFunc = _op as FunctionOperator;
                if (ReferenceEquals(opFunc, null))
                {
                    UnaryOperator opUnary = _op as UnaryOperator;
                    if (!ReferenceEquals(opUnary, null) && _op.ToString().Contains("#") && opUnary.OperatorType == UnaryOperatorType.Not)
                    {
                        _path += $"{opUnary.OperatorType}({FormatODataQueryString(opUnary.Operand)})";
                    }
                    else
                    {
                        _path += _op.ToString().Replace("[", "").Replace("]", "");
                    }
                }
                else
                {
                    _path += $"{opFunc.OperatorType.ToString().ToLower()}({opFunc.Operands[0].ToString().Replace("[", "").Replace("]", "")}, {opFunc.Operands[1]})";
                }
            }
            else
            {
                switch (opBinary.OperatorType)
                {
                    case BinaryOperatorType.Equal:
                        if (opBinary.RightOperand.ToString().Contains("##ToString#"))
                        {
                            OperandValue opValue = opBinary.RightOperand as OperandValue;

                            //Forma estándar de OData v4.0 (no funciona)
                            //_path += $"{opBinary.LeftOperand.ToString().Replace("[", "").Replace("]", "")} eq {opValue.Value.GetType()}'{opValue.Value}'"; //
                            _path += $"{opBinary.LeftOperand.ToString().Replace("[", "").Replace("]", "")} eq {(int)opValue.Value}";
                        }
                        else if (opBinary.RightOperand.ToString().Contains("#"))
                        {
                            _path += $"{opBinary.LeftOperand.ToString().Replace("[", "").Replace("]", "")} eq '{opBinary.RightOperand.ToString().Replace("#", "")}'";
                        }
                        else
                        {
                            if (opBinary.RightOperand.ToString() == "True" || opBinary.RightOperand.ToString() == "False")
                                _path += $"{opBinary.LeftOperand.ToString().Replace("[", "").Replace("]", "")} eq {opBinary.RightOperand.ToString().ToLower()}";
                            else
                                _path += $"{opBinary.LeftOperand.ToString().Replace("[", "").Replace("]", "")} eq {opBinary.RightOperand}";
                        }
                        break;

                    case BinaryOperatorType.NotEqual:
                        if (opBinary.RightOperand.ToString().Contains("##ToString#"))
                        {
                            OperandValue opValue = opBinary.RightOperand as OperandValue;
                            _path += $"{opBinary.LeftOperand.ToString().Replace("[", "").Replace("]", "")} ne {opValue.Value.GetType()}'{opValue.Value}'";
                        }
                        else if (opBinary.RightOperand.ToString().Contains("#"))
                        {
                            _path += $"{opBinary.LeftOperand.ToString().Replace("[", "").Replace("]", "")} ne '{opBinary.RightOperand.ToString().Replace("#", "")}'";
                        }
                        else
                        {
                            _path += $"{opBinary.LeftOperand.ToString().Replace("[", "").Replace("]", "")} ne {opBinary.RightOperand}";
                        }
                        break;

                    case BinaryOperatorType.Greater:
                        if (opBinary.RightOperand.ToString().Contains("#"))
                        {
                            _path += $"{opBinary.LeftOperand.ToString().Replace("[", "").Replace("]", "")} gt '{opBinary.RightOperand.ToString().Replace("#", "")}'";
                        }
                        else
                        {
                            _path += $"{opBinary.LeftOperand.ToString().Replace("[", "").Replace("]", "")} gt {opBinary.RightOperand}";
                        }
                        break;

                    case BinaryOperatorType.GreaterOrEqual:
                        if (opBinary.RightOperand.ToString().Contains("#"))
                        {
                            _path += $"{opBinary.LeftOperand.ToString().Replace("[", "").Replace("]", "")} ge '{opBinary.RightOperand.ToString().Replace("#", "")}'";
                        }
                        else
                        {
                            _path += $"{opBinary.LeftOperand.ToString().Replace("[", "").Replace("]", "")} ge {opBinary.RightOperand}";
                        }
                        break;

                    case BinaryOperatorType.Less:
                        if (opBinary.RightOperand.ToString().Contains("#"))
                        {
                            _path += $"{opBinary.LeftOperand.ToString().Replace("[", "").Replace("]", "")} lt '{opBinary.RightOperand.ToString().Replace("#", "")}'";
                        }
                        else
                        {
                            _path += $"{opBinary.LeftOperand.ToString().Replace("[", "").Replace("]", "")} lt {opBinary.RightOperand}";
                        }
                        break;

                    case BinaryOperatorType.LessOrEqual:
                        if (opBinary.RightOperand.ToString().Contains("#"))
                        {
                            _path += $"{opBinary.LeftOperand.ToString().Replace("[", "").Replace("]", "")} le '{opBinary.RightOperand.ToString().Replace("#", "")}'";
                        }
                        else
                        {
                            _path += $"{opBinary.LeftOperand.ToString().Replace("[", "").Replace("]", "")} le {opBinary.RightOperand}";
                        }
                        break;

                    default:
                        _path += $"{opBinary.ToString().Replace("[", "").Replace("]", "")}";
                        break;
                }
            }
            return _path;
        }


    }
}
