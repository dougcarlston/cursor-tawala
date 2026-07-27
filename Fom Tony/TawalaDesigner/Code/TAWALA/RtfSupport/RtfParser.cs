// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;
using Tawala.Common;

namespace Tawala.RtfSupport
{
    public delegate void ActionMethod(string anyText);

    public class RtfParser
    {
        private static readonly Dictionary<string, ActionMethod> commandActions = new Dictionary<string, ActionMethod>();

        /// <summary>
        /// Table of font colors
        /// </summary>
        private readonly RtfColorTable colorTable = new RtfColorTable();

        /// <summary>
        /// Table of fonts
        /// </summary>
        private readonly RtfFontTable fontTable = new RtfFontTable();

        private readonly RtfPictureData pictureData = new RtfPictureData();

        private readonly Stack stateStack = new Stack();

        private readonly StringCollection tokenStrings;

        private readonly StringBuilder xmlString;

        /// <summary>
        /// Count of {}-delimited groups
        /// </summary>
        private int groupCount;

        /// <summary>
        /// Index of current {}-delimited group
        /// </summary>
        private int groupIndex;

        protected RtfState rtfState = new RtfState();

        /// <summary>
        /// Collection of token objects
        /// </summary>
        private Collection<RtfToken> tokens;

        protected RtfParser()
        {
            commandActions.Clear();
            commandActions.Add("b", setBold);
            commandActions.Add("blipupi", setPictureUpi);
            commandActions.Add("blue", setColorTableBlue);
            commandActions.Add("cell", endTableCell);
            commandActions.Add("clwWidth", setCellWidth);
            commandActions.Add("cf", setColorIndex);
            commandActions.Add("colortbl", startColorTable);
            commandActions.Add("deftab", setDefaultTabPositions);
            commandActions.Add("f", setFontIndex);
            commandActions.Add("fmodern", setFontFamily);
            commandActions.Add("fonttbl", startFontTable);
            commandActions.Add("froman", setFontFamily);
            commandActions.Add("fs", setFontSize);
            commandActions.Add("fswiss", setFontFamily);
            commandActions.Add("fsymbol", setFontFamily);
            commandActions.Add("generator", setOptionalGroup);
            commandActions.Add("green", setColorTableGreen);
            commandActions.Add("i", setItalic);
            commandActions.Add("intbl", setInTable);
            commandActions.Add("itap", setParagraphNestingLevel);
            commandActions.Add("li", setIndent);
            commandActions.Add("par", endParagraph);
            commandActions.Add("pard", resetParagraphAttributes);
            commandActions.Add("pich", setPictureH);
            commandActions.Add("pichgoal", setPictureHGoal);
            commandActions.Add("picscalex", setPictureScaleX);
            commandActions.Add("picscaley", setPictureScaleY);
            commandActions.Add("pict", setInImage);
            commandActions.Add("picw", setPictureW);
            commandActions.Add("picwgoal", setPictureWGoal);
            commandActions.Add("plain", setPlain);
            commandActions.Add("qc", setCenterAlignment);
            commandActions.Add("qj", setJustifyAlignment);
            commandActions.Add("qr", setRightAlignment);
            commandActions.Add("red", setColorTableRed);
            commandActions.Add("row", endTableRow);
            commandActions.Add("stylesheet", startStyleSheet);
            commandActions.Add("tab", accumulateTab);
            commandActions.Add("trgaph", setTableCellGap);
            commandActions.Add("trleft", setTableLeftEdge);
            commandActions.Add("trowd", startTableRow);
            commandActions.Add("txfielddata", startFieldData);
            commandActions.Add("txfielddataval", setFieldId);
            commandActions.Add("txfieldend", endTextField);
            commandActions.Add("txfieldstart", startTextField);
            commandActions.Add("tx", setTabPosition);
            commandActions.Add("ul", setUnderline);

            xmlString = new StringBuilder();
        }

        public RtfParser(string rtfString) : this()
        {
            tokenStrings = RtfTokenizer.Tokenize(rtfString);
            createTokens();
        }

        public RtfParser(string rtfString, RtfFontTable fontTable, RtfColorTable colorTable) : this()
        {
            this.fontTable = fontTable;
            this.colorTable = colorTable;

            tokenStrings = RtfTokenizer.Tokenize(rtfString);
            createTokens();
        }

        public Collection<RtfToken> Tokens
        {
            get
            {
                return tokens;
            }
        }

        public RtfFontTable FontTable
        {
            get
            {
                return fontTable;
            }
        }

        public RtfColorTable ColorTable
        {
            get
            {
                return colorTable;
            }
        }

        public int GroupCount
        {
            get
            {
                return groupCount;
            }
        }

        public int GroupIndex
        {
            get
            {
                return groupIndex;
            }
        }

        #region Parsing Utility methods

        /// <summary>
        /// Turns the list of token strings into a list of token objects.
        /// </summary>
        private void createTokens()
        {
            tokens = new Collection<RtfToken>();

            foreach (string tokenString in tokenStrings)
            {
                RtfToken token = getToken(tokenString);

                if (token != RtfToken.NULL)
                {
                    tokens.Add(token);
                }
            }
        }

        /// <summary>
        /// Factory method that returns one of the objects derived from RtfToken.
        /// </summary>
        protected virtual RtfToken getToken(string tokenString)
        {
            const string commandPattern = @"\\([a-zA-Z*]+)(-?[0-9]+)? ?";

            switch (tokenString)
            {
                case "{":
                    return new RtfToken(tokenString, startGroup);
                case "}":
                    return new RtfToken(tokenString, endGroup);
                case @"\{":
                    return new RtfToken("{", accumulateText);
                case @"\}":
                    return new RtfToken("}", accumulateText);
                case @"\\":
                    return new RtfToken(@"\", accumulateText);
                default:
                    if (Regex.IsMatch(tokenString, commandPattern))
                    {
                        string commandString = Regex.Match(tokenString, commandPattern).Groups[1].Value;
                        return (commandActions.ContainsKey(commandString)
                                    ? new RtfToken(tokenString, commandActions[commandString])
                                    : RtfToken.NULL);
                    }
                    return new RtfToken(tokenString, accumulateText);
            }
        }

        #endregion

        #region Action Methods

        private readonly Collection<int> cellWidths = new Collection<int>();
        private readonly Collection<int> tabPositions = new Collection<int>();
        private int explicitTabStopCount;

        /// <summary>
        /// Table indent in TWIPS
        /// </summary>
        private int tableIndent;

        public Collection<int> CellWidths
        {
            get
            {
                return cellWidths;
            }
        }

        public Collection<int> TabPositions
        {
            get
            {
                return tabPositions;
            }
        }

        /// <summary>
        /// Accumulates text that belongs either to a paragraph or to a table cell
        /// </summary>
        private void accumulateText(string text)
        {
            if (rtfState.InTable)
            {
                startCellAndDivision();
            }

            if (rtfState.InFieldData)
            {
                accumulateFieldText(text);

                rtfState.InFieldData = false;
            }
            else if (rtfState.InImage)
            {
                accumulateImageData(text);

                rtfState.InImage = false;
            }
            else
            {
                if (!inTextField)
                {
                    if (isAllSpaces(text))
                    {
                        accumulateLiteralText(text.Replace(" ", "<sp/>"));
                    }
                    else
                    {
                        accumulateStandardText(text);
                    }
                }
            }
        }

        private static bool isAllSpaces(string text)
        {
            return (Regex.Match(text, "[ ]+").Length == text.Length);
        }

        private void resetParagraphAttributes(string text)
        {
            rtfState.ParagraphAlignment = RtfState.Alignment.Left;
            rtfState.ParagraphIndent = 0;

            rtfState.FontIndex = 0;
            rtfState.FontSize = RtfState.DefaultFontSize;

            explicitTabStopCount = 0;

            rtfState.InTable = false;
        }

        private void endParagraph(string text)
        {
            if (rtfState.InTable)
            {
                endDivision();
            }
            else
            {
                emitParagraph();
            }
        }

        private void startGroup(string text)
        {
            stateStack.Push(rtfState.Copy());
            groupCount++;
            groupIndex++;
        }

        private void endGroup(string text)
        {
            //			accumulateLiteralText(getCurrentAttributeEndTags());
            groupIndex--;
            rtfState = (RtfState)stateStack.Pop();
        }

        private void startFontTable(string text)
        {
            rtfState.InFontTable = true;
        }

        private void startColorTable(string text)
        {
            rtfState.InColorTable = true;
        }

        private void startStyleSheet(string text)
        {
            rtfState.InStyleSheet = true;
        }

        private void setFontIndex(string text)
        {
            int fontIndex = Convert.ToInt32(Regex.Match(text, @"\\f(\d+)").Groups[1].Value);

            if (rtfState.InFontTable)
            {
                fontTable.Add(new RtfFontTableEntry());
                rtfState.FontIndex = fontIndex;
            }
            else if (rtfState.InContent)
            {
                accumulateFontIndex(fontIndex);
            }
        }

        private void setFontSize(string text)
        {
            if (rtfState.InContent)
            {
                // preserve font size in half-points
                accumulateFontSize(Convert.ToInt32(Regex.Match(text, @"\\fs(\d+)").Groups[1].Value));
            }
        }

        private void setFontFamily(string text)
        {
            fontTable[rtfState.FontIndex].FontFamily = Regex.Match(text, @"\\f([a-z]+)").Groups[1].Value;
        }

        private void setFontName(string text)
        {
            fontTable[rtfState.FontIndex].FontName = text;
        }

        private void setOptionalGroup(string text)
        {
            rtfState.InOptionalGroup = true;
        }

        private void setBold(string text)
        {
            rtfState.Bold = true;
        }

        private void setItalic(string text)
        {
            rtfState.Italic = true;
        }

        private void setUnderline(string text)
        {
            rtfState.Underline = true;
        }

        private void setPlain(string text)
        {
            rtfState.Bold = false;
            rtfState.Italic = false;
            rtfState.Underline = false;

            newFontIndex = 0;
            newFontSize = RtfState.DefaultFontSize;
            newColorIndex = 0;
            synchronizeFontState();
        }

        private void setLeftAlignment(string text)
        {
            rtfState.ParagraphAlignment = RtfState.Alignment.Left;
        }

        private void setCenterAlignment(string text)
        {
            rtfState.ParagraphAlignment = RtfState.Alignment.Center;
        }

        private void setRightAlignment(string text)
        {
            rtfState.ParagraphAlignment = RtfState.Alignment.Right;
        }

        private void setJustifyAlignment(string text)
        {
            rtfState.ParagraphAlignment = RtfState.Alignment.Justify;
        }

        private void setIndent(string text)
        {
            rtfState.ParagraphIndent = Convert.ToInt32(Regex.Match(text, @"\\li(\d+)").Groups[1].Value);
        }

        private void setColorTableRed(string text)
        {
            if (rtfState.InColorTable)
            {
                int redValue = Convert.ToInt32(Regex.Match(text, @"\\red(\d+)").Groups[1].Value);

                colorTable.Add(new RtfColorTableEntry());
                colorTable[colorTable.Count - 1].Red = redValue;
            }
        }

        private void setColorTableGreen(string text)
        {
            if (rtfState.InColorTable)
            {
                int greenValue = Convert.ToInt32(Regex.Match(text, @"\\green(\d+)").Groups[1].Value);

                colorTable[colorTable.Count - 1].Green = greenValue;
            }
        }

        private void setColorTableBlue(string text)
        {
            if (rtfState.InColorTable)
            {
                int blueValue = Convert.ToInt32(Regex.Match(text, @"\\blue(\d+)").Groups[1].Value);

                colorTable[colorTable.Count - 1].Blue = blueValue;
            }
        }

        private void setColorIndex(string text)
        {
            int colorIndex = zeroBasedIndex(Convert.ToInt32(Regex.Match(text, @"\\cf(\d+)").Groups[1].Value));

            if (rtfState.InContent)
            {
                accumulateColorIndex(colorIndex);
            }
        }

        private void setTableCellGap(string text)
        {
        }

        private void setTableLeftEdge(string text)
        {
            tableIndent = Convert.ToInt32(Regex.Match(text, @"\\trleft(\d+)").Groups[1].Value);

            if (!tableStarted)
            {
                startTable();
            }

            accumulateLiteralText("<row>");
        }

        private void startTableRow(string text)
        {
            clearCellWidths();
        }

        private void endTableRow(string text)
        {
            accumulateLiteralText("</row>");
        }

        private void setInTable(string text)
        {
            rtfState.InTable = true;
        }

        private void setInImage(string text)
        {
            rtfState.InImage = true;
        }

        private void setParagraphNestingLevel(string text)
        {
            rtfState.ParagraphNestingLevel = Convert.ToInt32(Regex.Match(text, @"\\itap(\d+)").Groups[1].Value);

            if (tableStarted)
            {
                if (rtfState.ParagraphNestingLevel == 0)
                {
                    emitTable();
                }
            }
        }

        private void clearCellWidths()
        {
            cellWidths.Clear();
            cellIndex = 0;
        }

        private void setCellWidth(string text)
        {
            int cellWidth = Convert.ToInt32(Regex.Match(text, @"\\clwWidth(\d+)").Groups[1].Value);

            cellWidths.Add(cellWidth);
        }

        private void endTableCell(string text)
        {
            endDivision();
            endCell();
        }

        private string tabPositionsXml()
        {
            var xmlString = new StringBuilder();

            xmlString.Append("<tabPositions>");

            foreach (int position in tabPositions)
            {
                xmlString.AppendFormat("<tabStop position=\"{0}\"/>", position);
            }

            xmlString.Append("</tabPositions>");

            return xmlString.ToString();
        }

        private void setTabPosition(string text)
        {
            if (explicitTabStopCount == 0)
            {
                tabPositions.Clear();
            }

            explicitTabStopCount++;

            int tabPosition = Convert.ToInt32(Regex.Match(text, @"\\tx(\d+)").Groups[1].Value);

            tabPositions.Add(tabPosition);
        }

        private void setDefaultTabPositions(string text)
        {
            //  Note:   We don't want to set any default tabs, because TX always returns 14 positions,
            //			regardless of what the designer specified  -  jf 8/24/07
            //
            //int tabSpacing = Convert.ToInt32(Regex.Match(text, @"\\deftab(\d+)").Groups[1].Value);
            //int tabPosition = 0;

            //tabPositions.Clear();

            //for (int i = 0; i < 14; i++)
            //{
            //    tabPosition += tabSpacing;
            //    tabPositions.Add(tabPosition);
            //}

            tabPositions.Clear();
        }

        private void startTextField(string text)
        {
            inTextField = true;
            textFieldId = 0;
        }

        private void endTextField(string text)
        {
            inTextField = false;
        }

        private void startFieldData(string text)
        {
            rtfState.InFieldData = true;
        }

        private void setFieldId(string text)
        {
            textFieldId = Convert.ToInt32(Regex.Match(text, @"\\txfielddataval(\d+)").Groups[1].Value);
        }

        #endregion

        #region Action Utility Methods

        /// <summary>
        /// XML font element end tag
        /// </summary>
        protected const string fontEndTag = "</font>";

        private const string fieldFormat = "<field name=\"{0}\" id=\"{1}\"/>";
        private static readonly string functionFieldFormat = "<functionField instanceId=\"{0}\"/>";
        private static readonly string hyperlinkFieldFormat = "<hyperlink id=\"{0}\"/>";
        private static readonly string invitationFieldFormat = "<invitation id=\"{0}\"/>";

        /// <summary>
        /// String to hold XML text accumulated during parsing process.
        /// </summary>
        private string accumulatedText = "";

        private int cellIndex;

        /// <summary>
        /// Indicates whether a starting cell tag has been accumulated
        /// </summary>
        private bool cellStarted;

        /// <summary>
        /// Indicates whether a starting division tag has been accumulated
        /// </summary>
        private bool divisionStarted;

        /// <summary>
        /// Indicates whether a TX Text Control text field is being processed
        /// </summary>
        private bool inTextField;

        /// <summary>
        /// Index into colorTable
        /// </summary>
        protected int newColorIndex;

        /// <summary>
        /// Index into fontTable
        /// </summary>
        protected int newFontIndex;

        /// <summary>
        /// font size in half points
        /// </summary>
        protected int newFontSize = RtfState.DefaultFontSize;

        /// <summary>
        /// Indicates whether a starting table tag has been accumulated
        /// </summary>
        private bool tableStarted;

        private int textFieldId;

        protected void accumulateStandardText(string text)
        {
            if (rtfState.InFontTable)
            {
                setFontName(text);
            }
            else if (rtfState.InContent)
            {
                if (fontStateChanged() || !isDefaultFontState())
                {
                    accumulateLiteralText(getCurrentFontStartTag(newFontIndex, newFontSize, newColorIndex));
                }

                accumulateLiteralText(getCurrentAttributeStartTags());
                accumulateLiteralText(XMLStringFormatter.EscapeElementText(text));
                accumulateLiteralText(getCurrentAttributeEndTags());

                if (fontStateChanged() || !isDefaultFontState())
                {
                    accumulateLiteralText(fontEndTag);
                    synchronizeFontState();
                }
            }
        }

        private void accumulateFieldText(string text)
        {
            string decodedText = RtfUtility.DecodeHexString(text);
            string fieldString = XMLStringFormatter.EscapeAttributeText(decodedText.Substring(3));

            if (fontStateChanged() || !isDefaultFontState())
            {
                accumulateLiteralText(getCurrentFontStartTag(newFontIndex, newFontSize, newColorIndex));
            }

            accumulateLiteralText(getCurrentAttributeStartTags());
            if (isInvitationField(decodedText))
            {
                accumulateInvitationFieldText(fieldString);
            }
            else if (isHyperlinkField(decodedText))
            {
                accumulateHyperlinkFieldText(fieldString);
            }
            else if (isFunctionField(decodedText))
            {
                accumulateFunctionFieldText();
            }
            else
            {
                accumulateLiteralText(String.Format(fieldFormat, fieldString, textFieldId));
            }

            accumulateLiteralText(getCurrentAttributeEndTags());

            if (fontStateChanged() || !isDefaultFontState())
            {
                accumulateLiteralText(fontEndTag);
                synchronizeFontState();
            }
        }

        private static bool isInvitationField(string decodedText)
        {
            return decodedText.Substring(0, 3).Equals("IF$");
        }

        private static bool isHyperlinkField(string decodedText)
        {
            return decodedText.Substring(0, 3).Equals("HF$");
        }

        private static bool isFunctionField(string decodedText)
        {
            return decodedText.Substring(0, 3).Equals("FF$");
        }

        private void accumulateInvitationFieldText(string fieldString)
        {
            accumulateLiteralText(String.Format(invitationFieldFormat, textFieldId));
        }

        private void accumulateHyperlinkFieldText(string fieldString)
        {
            accumulateLiteralText(String.Format(hyperlinkFieldFormat, textFieldId));
        }

        private void accumulateFunctionFieldText()
        {
            accumulateLiteralText(String.Format(functionFieldFormat, textFieldId));
        }

        private void startCellAndDivision()
        {
            if (!cellStarted)
            {
                startCell();
            }

            if (!divisionStarted)
            {
                startDivision();
            }
        }

        protected void accumulateLiteralText(string text)
        {
            accumulatedText += text;
        }

        private void accumulateTab(string text)
        {
            accumulateLiteralText("<tab/>");
        }

        private void setPictureH(string text)
        {
            Debug.Assert(rtfState.InImage);

            pictureData.H = Convert.ToInt32(Regex.Match(text, @"\\pich(\d+)").Groups[1].Value);
        }

        private void setPictureHGoal(string text)
        {
            Debug.Assert(rtfState.InImage);

            pictureData.HGoal = Convert.ToInt32(Regex.Match(text, @"\\pichgoal(\d+)").Groups[1].Value);
        }

        private void setPictureScaleX(string text)
        {
            Debug.Assert(rtfState.InImage);

            pictureData.ScaleX = Convert.ToInt32(Regex.Match(text, @"\\picscalex(\d+)").Groups[1].Value);
        }

        private void setPictureScaleY(string text)
        {
            Debug.Assert(rtfState.InImage);

            pictureData.ScaleY = Convert.ToInt32(Regex.Match(text, @"\\picscaley(\d+)").Groups[1].Value);
        }

        private void setPictureW(string text)
        {
            Debug.Assert(rtfState.InImage);

            pictureData.W = Convert.ToInt32(Regex.Match(text, @"\\picw(\d+)").Groups[1].Value);
        }

        private void setPictureWGoal(string text)
        {
            Debug.Assert(rtfState.InImage);

            pictureData.WGoal = Convert.ToInt32(Regex.Match(text, @"\\picwgoal(\d+)").Groups[1].Value);
        }

        private void setPictureUpi(string text)
        {
            Debug.Assert(rtfState.InImage);

            pictureData.Upi = Convert.ToInt32(Regex.Match(text, @"\\blipupi(\d+)").Groups[1].Value);
        }

        private void accumulateImageData(string text)
        {
            //RtfMetafile image = new RtfMetafile(text);

            pictureData.MetafileHexString = text;

            var image = new RtfMetafile(pictureData);

            accumulateLiteralText("<font face=\"Arial\" size=\"200\" color=\"000000\">");

            accumulateLiteralText(image.ToXml());

            accumulateLiteralText("</font>");
        }

        private void emptyAccumulatedText()
        {
            accumulatedText = "";
        }

        /// <summary>
        /// Creates paragraph text in the output xmlString
        /// </summary>
        private void emitParagraph()
        {
            string paragraphStartTag = "<paragraph indent=\"{0}\" align=\"{1}\">";

            xmlString.AppendFormat(paragraphStartTag, rtfState.ParagraphIndent, paragraphAlignmentString());

            if (tabPositions.Count > 0)
            {
                xmlString.Append(tabPositionsXml());
            }

            xmlString.Append(accumulatedText);
            xmlString.Append("</paragraph>");

            emptyAccumulatedText();
        }

        /// <summary>
        /// Creates table text in the output xmlString
        /// </summary>
        private void emitTable()
        {
            xmlString.Append(accumulatedText);
            xmlString.Append("</table>");

            emptyAccumulatedText();

            tableStarted = false;
        }

        protected bool fontStateChanged()
        {
            return (rtfState.FontIndex != newFontIndex || rtfState.FontSize != newFontSize || rtfState.ColorIndex != newColorIndex);
        }

        protected bool isDefaultFontState()
        {
            return (newFontIndex == 0 && newFontSize == RtfState.DefaultFontSize);
        }

        private void accumulateFontIndex(int index)
        {
            newFontIndex = index;
        }

        private void accumulateFontSize(int fontSize)
        {
            newFontSize = fontSize;
        }

        private void synchronizeFontState()
        {
            rtfState.FontIndex = newFontIndex;
            rtfState.FontSize = newFontSize;
            rtfState.ColorIndex = newColorIndex;
        }

        private void accumulateColorIndex(int index)
        {
            newColorIndex = index;
        }

        private int zeroBasedIndex(int oneBasedIndex)
        {
            return oneBasedIndex - 1;
        }

        private void startTable()
        {
            string tableStartTag = "<table indent=\"{0}\">";

            accumulateLiteralText(String.Format(tableStartTag, tableIndent));

            tableStarted = true;
        }

        private void startCell()
        {
            accumulateLiteralText(String.Format("<cell width=\"{0}\">", cellWidths[cellIndex++]));
            cellStarted = true;
        }

        private void endCell()
        {
            if (!cellStarted)
            {
                // avoid unmatched end tags when cell contains no text
                startCell();
            }

            accumulateLiteralText("</cell>");
            cellStarted = false;
        }

        private void startDivision()
        {
            string divisionStartTag = "<division indent=\"{0}\" align=\"{1}\">";
            accumulateLiteralText(String.Format(divisionStartTag, rtfState.ParagraphIndent, paragraphAlignmentString()));

            divisionStarted = true;
        }

        private void endDivision()
        {
            if (divisionStarted)
            {
                accumulateLiteralText(("</division>"));
                divisionStarted = false;
            }
        }

        /// <summary>
        /// Returns zero or more attribute start tag strings based on the current bold, italic and underline states
        /// </summary>
        private string getCurrentAttributeStartTags()
        {
            string tagString = "";

            if (rtfState.Bold)
            {
                tagString += RtfTextAttribute.Bold.StartTag;
            }

            if (rtfState.Italic)
            {
                tagString += RtfTextAttribute.Italic.StartTag;
            }

            if (rtfState.Underline)
            {
                tagString += RtfTextAttribute.Underline.StartTag;
            }

            return tagString;
        }

        /// <summary>
        /// Returns zero or more attribute end tag strings based on the current bold, italic and underline states
        /// </summary>
        private string getCurrentAttributeEndTags()
        {
            string tagString = "";

            if (rtfState.Underline)
            {
                tagString += RtfTextAttribute.Underline.EndTag;
            }

            if (rtfState.Italic)
            {
                tagString += RtfTextAttribute.Italic.EndTag;
            }

            if (rtfState.Bold)
            {
                tagString += RtfTextAttribute.Bold.EndTag;
            }

            return tagString;
        }

        /// <summary>
        /// Returns a string corresponding to the current paragraph alignment state
        /// </summary>
        private string paragraphAlignmentString()
        {
            string align = "left";

            switch (rtfState.ParagraphAlignment)
            {
                case RtfState.Alignment.Center:
                    align = "center";
                    break;
                case RtfState.Alignment.Right:
                    align = "right";
                    break;
                case RtfState.Alignment.Justify:
                    align = "justify";
                    break;
                default:
                    break;
            }

            return align;
        }

        private int halfPointsToTwips(int sizeinHalfPoints)
        {
            return sizeinHalfPoints*10;
        }

        protected string getCurrentFontStartTag(int fontIndex, int fontSize, int colorIndex)
        {
            const string fontStartTag = "<font face=\"{0}\" size=\"{1}\" color=\"{2}\">";
            const string defaultFontStartTag = "<font face=\"Arial\" size=\"200\" color=\"000000\">";

            return (fontTable.Count > 0
                        ? String.Format(fontStartTag, fontTable[fontIndex].FontName, halfPointsToTwips(fontSize),
                                        colorTable[colorIndex].ToHexString())
                        : defaultFontStartTag);
        }

        #endregion

        public void Parse()
        {
            colorTable.Clear();

            foreach (RtfToken token in tokens)
            {
                token.Execute();
            }
        }

        public string ToXml()
        {
            return xmlString.ToString();
        }
    }
}