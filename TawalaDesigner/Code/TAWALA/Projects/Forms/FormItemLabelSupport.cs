// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using System.Text.RegularExpressions;

namespace Tawala.Projects.Forms
{
    public static class FormItemLabelSupport
    {
        /// <summary>
        /// Indicates whether the specified label is a legal label for the specified target form item.
        /// </summary>
        public static bool IsLegalItemLabel(string label, IFormItem targetFormItem)
        {
            return isLegalLabel(label) && !ItemLabelDuplicatesOtherItemLabel(label, targetFormItem) &&
                   !ItemLabelDuplicatesAnyBlankLabel(label, targetFormItem) && !ItemLabelDuplicatesHiddenFieldName(label, targetFormItem);
        }

        /// <summary>
        /// Indicates whether the specified label is a legal label for the specified target blank.
        /// </summary>
        public static bool IsLegalBlankLabel(string label, IBlank targetBlank)
        {
            return isLegalLabel(label) && !BlankLabelDuplicatesAnyItemLabel(label, targetBlank) &&
                   !BlankLabelDuplicatesOtherBlankLabel(label, targetBlank) && !BlankLabelDuplicatesHiddenFieldName(label, targetBlank);
        }

        private static bool isLegalLabel(string label)
        {
            return !resemblesDefaultFormItemLabel(label) && !hasLeadingDoubleUnderscores(label) && !containsColon(label) &&
                   !isNumeric(label) && containsValidLabelCharacters(label);
        }

        private static bool resemblesDefaultFormItemLabel(string label)
        {
            return Regex.IsMatch(label, @"^[QqTtHh]\d+$");
        }

        private static bool hasLeadingDoubleUnderscores(string label)
        {
            return Regex.IsMatch(label, @"^__.*$");
        }

        private static bool containsColon(string label)
        {
            return Regex.IsMatch(label, @":+");
        }

        private static bool containsValidLabelCharacters(string label)
        {
            return !Regex.IsMatch(label, @"[<>]+");
        }

        private static bool isNumeric(string label)
        {
            return Regex.IsMatch(label, @"^[+-]?\d+\.?\d*$");
        }

        /// <summary>
        /// Indicates whether the specified label duplicates any form item label in the target blank's Form.
        /// </summary>
        public static bool BlankLabelDuplicatesAnyItemLabel(string label, IBlank targetBlank)
        {
            IForm form = getContainingFormForBlank(targetBlank);

            foreach (IFormItem item in form.ItemList)
            {
                if (item.AlternateLabel == label && label != string.Empty)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Indicates whether the specified item label duplicates any hidden field name in the target item's Form.
        /// </summary>
        public static bool ItemLabelDuplicatesHiddenFieldName(string label, IFormItem targetFormItem)
        {
            IForm form = Project.Current.GetFormContaining(targetFormItem);

            return labelDuplicatesHiddenFieldNameInForm(label, form);
        }

        /// <summary>
        /// Indicates whether the specified blank label duplicates any hidden field name in the target blank's Form.
        /// </summary>
        public static bool BlankLabelDuplicatesHiddenFieldName(string label, IBlank targetBlank)
        {
            IForm form = getContainingFormForBlank(targetBlank);

            return labelDuplicatesHiddenFieldNameInForm(label, form);
        }

        private static bool labelDuplicatesHiddenFieldNameInForm(string label, IForm form)
        {
            if (form != null)
            {
                foreach (IFormItem item in form.ItemList)
                {
                    var hiddenField = item as IHiddenField;

                    if (hiddenField != null)
                    {
                        if (hiddenField.Name == label && label != string.Empty)
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        private static IForm getContainingFormForBlank(IBlank targetBlank)
        {
            foreach (IForm form in Project.Current.FormList)
            {
                foreach (IFormItem item in form.ItemList)
                {
                    var fibItem = item as IFibItem;

                    if (fibItem != null)
                    {
                        foreach (IBlank blank in fibItem.BlankList)
                        {
                            if (blank == targetBlank)
                            {
                                return form;
                            }
                        }
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Indicates whether the specified label duplicates any other form item label in the target item's Form.
        /// </summary>
        public static bool ItemLabelDuplicatesOtherItemLabel(string label, IFormItem targetFormItem)
        {
            IForm form = Project.Current.GetFormContaining(targetFormItem);
            foreach (IFormItem item in form.ItemList)
            {
                if (item != targetFormItem)
                {
                    if (item.AlternateLabel == label && label != string.Empty)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Indicates whether the specified label duplicates any blank label in the target item's form.
        /// </summary>
        public static bool ItemLabelDuplicatesAnyBlankLabel(string label, IFormItem targetFormItem)
        {
            IForm form = Project.Current.GetFormContaining(targetFormItem);

            foreach (IFormItem item in form.ItemList)
            {
                var fibItem = item as IFibItem;

                if (fibItem != null)
                {
                    foreach (IBlank blank in fibItem.BlankList)
                    {
                        if (blank.AlternateLabel == label && label != string.Empty)
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Indicates whether the specified label duplicates any blank label in the target blank's Form.
        /// </summary>
        public static bool BlankLabelDuplicatesOtherBlankLabel(string label, IBlank targetBlank)
        {
            IForm form = getContainingFormForBlank(targetBlank);

            foreach (IFormItem item in form.ItemList)
            {
                var fibItem = item as IFibItem;

                if (fibItem != null)
                {
                    foreach (IBlank blank in fibItem.BlankList)
                    {
                        if (blank != targetBlank)
                        {
                            if (blank.AlternateLabel == label && label != string.Empty)
                            {
                                return true;
                            }
                        }
                    }
                }
            }

            return false;
        }
    }
}