using DevExpress.Data.Filtering;

namespace WooW.SB.Reportes.Helpers
{
    internal class WoCriteriaOperatorHelper
    {
        /// <summary>
        /// The FormatODataQueryString.
        /// </summary>
        /// <param name="_filter">The _filter<see cref="CriteriaOperator"/>.</param>
        /// <returns>The <see cref="string"/>.</returns>
        public static string FormatODataQueryString(CriteriaOperator _filter)
        {
            bool firstParam = true;
            string sResult = string.Empty;

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

        /// <summary>
        /// The FormatODataQueryStringParameter.
        /// </summary>
        /// <param name="_op">The _op<see cref="CriteriaOperator"/>.</param>
        /// <returns>The <see cref="string"/>.</returns>
        private static string FormatODataQueryStringParameter(CriteriaOperator _op)
        {
            string _path = string.Empty;
            BinaryOperator opBinary = _op as BinaryOperator;
            if (ReferenceEquals(opBinary, null))
            {
                FunctionOperator opFunc = _op as FunctionOperator;
                if (ReferenceEquals(opFunc, null))
                {
                    UnaryOperator opUnary = _op as UnaryOperator;
                    if (
                        !ReferenceEquals(opUnary, null)
                        && _op.ToString().Contains("#")
                        && opUnary.OperatorType == UnaryOperatorType.Not
                    )
                    {
                        _path +=
                            $"{opUnary.OperatorType}({WoCriteriaOperatorHelper.FormatODataQueryString(opUnary.Operand)})";
                    }
                    else
                    {
                        _path += _op.ToString()
                            .Replace("[", string.Empty)
                            .Replace("]", string.Empty);
                    }
                }
                else
                {
                    _path +=
                        $"{opFunc.OperatorType.ToString().ToLower()}({opFunc.Operands[0].ToString().Replace("[", string.Empty).Replace("]", string.Empty)}, {opFunc.Operands[1]})";
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
                            //_path += $"{opBinary.LeftOperand.ToString().Replace("[", string.Empty).Replace("]", string.Empty)} eq {opValue.Value.GetType()}'{opValue.Value}'"; //
                            _path +=
                                $"{opBinary.LeftOperand.ToString().Replace("[", string.Empty).Replace("]", string.Empty)} eq {(int)opValue.Value}";
                        }
                        else if (opBinary.RightOperand.ToString().Contains("#"))
                        {
                            _path +=
                                $"{opBinary.LeftOperand.ToString().Replace("[", string.Empty).Replace("]", string.Empty)} eq '{opBinary.RightOperand.ToString().Replace("#", string.Empty)}'";
                        }
                        else
                        {
                            if (
                                opBinary.RightOperand.ToString() == "True"
                                || opBinary.RightOperand.ToString() == "False"
                            )
                                _path +=
                                    $"{opBinary.LeftOperand.ToString().Replace("[", string.Empty).Replace("]", string.Empty)} eq {opBinary.RightOperand.ToString().ToLower()}";
                            else
                                _path +=
                                    $"{opBinary.LeftOperand.ToString().Replace("[", string.Empty).Replace("]", string.Empty)} eq {opBinary.RightOperand}";
                        }
                        break;

                    case BinaryOperatorType.NotEqual:
                        if (opBinary.RightOperand.ToString().Contains("##ToString#"))
                        {
                            OperandValue opValue = opBinary.RightOperand as OperandValue;
                            _path +=
                                $"{opBinary.LeftOperand.ToString().Replace("[", string.Empty).Replace("]", string.Empty)} ne {opValue.Value.GetType()}'{opValue.Value}'";
                        }
                        else if (opBinary.RightOperand.ToString().Contains("#"))
                        {
                            _path +=
                                $"{opBinary.LeftOperand.ToString().Replace("[", string.Empty).Replace("]", string.Empty)} ne '{opBinary.RightOperand.ToString().Replace("#", string.Empty)}'";
                        }
                        else
                        {
                            _path +=
                                $"{opBinary.LeftOperand.ToString().Replace("[", string.Empty).Replace("]", string.Empty)} ne {opBinary.RightOperand}";
                        }
                        break;

                    case BinaryOperatorType.Greater:
                        if (opBinary.RightOperand.ToString().Contains("#"))
                        {
                            _path +=
                                $"{opBinary.LeftOperand.ToString().Replace("[", string.Empty).Replace("]", string.Empty)} gt '{opBinary.RightOperand.ToString().Replace("#", string.Empty)}'";
                        }
                        else
                        {
                            _path +=
                                $"{opBinary.LeftOperand.ToString().Replace("[", string.Empty).Replace("]", string.Empty)} gt {opBinary.RightOperand}";
                        }
                        break;

                    case BinaryOperatorType.GreaterOrEqual:
                        if (opBinary.RightOperand.ToString().Contains("#"))
                        {
                            _path +=
                                $"{opBinary.LeftOperand.ToString().Replace("[", string.Empty).Replace("]", string.Empty)} ge '{opBinary.RightOperand.ToString().Replace("#", string.Empty)}'";
                        }
                        else
                        {
                            _path +=
                                $"{opBinary.LeftOperand.ToString().Replace("[", string.Empty).Replace("]", string.Empty)} ge {opBinary.RightOperand}";
                        }
                        break;

                    case BinaryOperatorType.Less:
                        if (opBinary.RightOperand.ToString().Contains("#"))
                        {
                            _path +=
                                $"{opBinary.LeftOperand.ToString().Replace("[", string.Empty).Replace("]", string.Empty)} lt '{opBinary.RightOperand.ToString().Replace("#", string.Empty)}'";
                        }
                        else
                        {
                            _path +=
                                $"{opBinary.LeftOperand.ToString().Replace("[", string.Empty).Replace("]", string.Empty)} lt {opBinary.RightOperand}";
                        }
                        break;

                    case BinaryOperatorType.LessOrEqual:
                        if (opBinary.RightOperand.ToString().Contains("#"))
                        {
                            _path +=
                                $"{opBinary.LeftOperand.ToString().Replace("[", string.Empty).Replace("]", string.Empty)} le '{opBinary.RightOperand.ToString().Replace("#", string.Empty)}'";
                        }
                        else
                        {
                            _path +=
                                $"{opBinary.LeftOperand.ToString().Replace("[", string.Empty).Replace("]", string.Empty)} le {opBinary.RightOperand}";
                        }
                        break;

                    default:
                        _path +=
                            $"{opBinary.ToString().Replace("[", string.Empty).Replace("]", string.Empty)}";
                        break;
                }
            }
            return _path;
        }
    }
}
