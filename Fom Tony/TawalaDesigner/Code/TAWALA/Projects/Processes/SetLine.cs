// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using System.Text.RegularExpressions;
using Tawala.Parsing;
using Tawala.Projects.Fields;
using Tawala.Projects.Forms;

namespace Tawala.Projects.Processes
{
    /// <summary>
    /// Implements single SET line for display in Process window.
    /// </summary>
    [Serializable]
    public class SetLine : ProcessLine
    {
        public SetLine()
        {
        }

        public SetLine(SetStatement statement) : base(statement, statement.ToString())
        {
            SelectsGroup = false;
            IsDeletable = true;
            IsSelectable = true;
            CanInsertBefore = true;
        }

        /// <summary>
        /// Boolean indicating whether this line is valid.
        /// </summary>
        public override bool IsValidLine(FieldList fieldResolver)
        {
            var setStatement = Statement as SetStatement;

            // variableString and expressionString are always required
            if (setStatement.Variable.FieldName == null || setStatement.Variable.FieldName.Equals("") ||
                setStatement.Expression.ToString() == null || setStatement.Expression.ToString().Equals(""))
            {
                return false;
            }

#if KM_20070607
    //// check for valid variable name
			Process parentProcess = Project.Current.GetProcessOrSkipInstructions(setStatement);
			int lineIndex = parentProcess.Lines.GetIndex(setStatement);
			if (fieldResolver[setStatement.Variable.FieldName] == null)
			{
				FieldList variableFieldResolver = parentProcess.GetValidVariables(lineIndex);
				if (variableFieldResolver[setStatement.Variable.FieldName] == null)
				{
					if (!FieldUtil.IsRecordAndFormQualifiedField(setStatement.Variable.FieldName))
					{
						return false;
					}
				}
			}
#endif
            Boolean isValid = true;

            // see if the entered expression is a valid mathematical expression
            if (setStatement.IsArithmetic() && !setStatement.TreatArithmeticAsText)
            {
                var parser = new MathParser(setStatement.Expression.ToString());
                if (parser.Balance == 0)
                {
                    string parsed = parser.ToXml();
                    if (parsed.Contains("invalidExpression"))
                    {
                        isValid = false;
                    }
                }
                else
                {
                    // unbalanced parentheses
                    isValid = false;
                }
            }

            // get collection of field names
            MatchCollection matches = Regex.Matches(setStatement.Expression.ToString(), @"<<([^>]+)>>");

            foreach (Match m in matches)
            {
                Group g = m.Groups[1];

                if (fieldResolver[g.Value] == null)
                {
                    IField fieldFromNameMap = Project.FieldMapByName[g.Value];
                    // REVISIT: This should really test for the existence of a field corresponding to g.Value.
                    //			(Deferred until "global field mapping" is addressed.) -SB 09/24/06
                    if (fieldFromNameMap is IHiddenField || fieldFromNameMap is IFileUploadItem)
                    {
                        isValid = true;
                    }
                    else if (FieldUtil.IsRecordField(g.Value))
                    {
                        isValid = true;
                    }
                    else
                    {
                        isValid = false;
                        break;
                    }
                }
            }

            return isValid;
        }
    }
}