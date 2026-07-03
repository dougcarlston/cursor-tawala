# Tawala Document Template System — Source Analysis
*From C# Designer (NewCode) + Java runtime (build-1700)*

---

## What a Document Is

A Document is a **named rich-text template** that can be rendered at runtime from inside a Process. It is the primary mechanism for:

- **Email bodies** — a `<send>` command references a document as its body
- **Printed/displayed output** — a `<show document="..."/>` command renders the document to the browser
- **Personalized content** — field references inside the template are resolved against the current `ExecutionContext` at render time

Documents live at the project level (alongside Forms and Processes) and are authored in the Designer's `DocumentDesigner` module.

---

## The XML Structure

A document in the `.tawala` file:

```xml
<documents>
  <document name="Confirmation Letter">
    <xmlData>
      <paragraph indent="0" align="left">
        Dear <field name="Registration:FirstName"/> <field name="Registration:LastName"/>,
      </paragraph>
      <paragraph indent="0" align="left">
        Thank you for registering <field name="Registration:PlayerName"/> in the
        <field name="Registration:Division"/> division.
      </paragraph>
      <table indent="0">
        <row>
          <cell width="3000"><paragraph indent="0" align="left">Jersey Number:</paragraph></cell>
          <cell width="3000"><paragraph indent="0" align="left"><field name="Registration:JerseyNumber"/></paragraph></cell>
        </row>
      </table>
      <paragraph indent="0" align="left">
        <font face="Arial" size="200" color="000000">
          <b>Registration closes on December 1st.</b>
        </font>
      </paragraph>
    </xmlData>
  </document>
</documents>
```

**Key structural rules:**
- `<document name="...">` — name is the identifier referenced in `<send>` and `<show document="..."/>`
- Content lives in a single `<xmlData>` wrapper child
- `<rawHtmlData>` is an older, legacy variant — the runtime marks it `used` and ignores it
- Multiple documents are children of the `<documents>` container

---

## Designer: `NewDocument` Class

```csharp
public class NewDocument : IDocument
{
    public string Name { get; set; }
    public IFormItemContents NewContents { get; set; }

    // Serializes to XML
    public string ToXml()
    {
        return string.Format(
            "<document name=\"{0}\">\r\n<xmlData>{1}</xmlData>\r\n</document>\r\n",
            Name, NewContents.ToXml());
    }
}
```

`NewContents` is a `FormItemContentsCollection` — the same rich-text content model used inside form items (FIB paragraphs, text blocks, etc.). **Documents share the exact same content model as form item bodies.** The `FormItemContentsFactory` is used to parse and serialize both.

---

## Runtime: `Document` Class

```java
public class Document implements FormRenderable {
    private static final Factory<FormRenderable> FACTORY = new Factory<FormRenderable>();

    static {
        FACTORY.setKeepWhitespace(false);
        FACTORY.register("paragraph", Paragraph.class);
        FACTORY.register("table", Table.class);
        Repository.registerWebComponentsWith(FACTORY);
        FunctionToWebAdapter.registerFunctionsWith(FACTORY);
    }

    public Html toHtml(ExecutionContext context) {
        Div div = new Div("class", "document");
        div.appendContents(contents, context);
        return div;
    }
}
```

At runtime, `Document.toHtml(context)` walks the content tree, resolves all `<field name="..."/>` references against the live `ExecutionContext`, and returns HTML wrapped in `<div class="document">`.

---

## The Rich-Text Content Model — Complete Node Type Registry

Both documents and form item bodies use the same `FormItemContentsFactory`. This is the complete set of XML elements that can appear in a document's `<xmlData>`:

### Structure nodes

| XML element | Class | Description |
|---|---|---|
| `<paragraph indent="N" align="left\|right\|center">` | `ParagraphXmlContents` | Block of inline content. `indent` in twips (1/20pt). Serializes to `<div style="margin-left: Npt">` at runtime. |
| `<table indent="N">` | `TableXmlContents` | A layout table. Children are `<row>` elements. |
| `<row>` | `TableRowXmlContents` | A table row. Children are `<cell>` elements. |
| `<cell width="N">` | `TableCellXmlContents` | A table cell. `width` in twips. Can contain paragraphs. |
| `<division>` | `DivisionContents` | A generic block container (less common). |

### Inline text-formatting nodes

| XML element | Class | Runtime output |
|---|---|---|
| `<font face="..." size="N" color="RRGGBB">` | `NewXmlFont` | `<span style="font-family:...; font-size:Npt; color:#RRGGBB;">` |
| `<b>` | `BoldContents` | `<strong>` |
| `<i>` | `ItalicContents` | `<em>` |
| `<u>` | `UnderlineContents` | `<u>` |
| `#text` (raw text node) | `TextContents` | Plain text, HTML-escaped |
| `<sp>` | `WhitespaceContents` | Whitespace/space character |

**Font size encoding:** The Designer stores font size in **twips** (1/20 point). `size="200"` = 200 twips = 10pt. The runtime divides by 20 to get CSS `pt` values.

**Font color encoding:** Stored as a 6-digit hex string without the `#` prefix (e.g. `color="FF0000"`). The runtime renders as `color: #FF0000`.

### Dynamic content nodes

| XML element | Class | Description |
|---|---|---|
| `<field name="FormName:FieldName"/>` | `FieldReference` | Resolves field value from `ExecutionContext` at render time. Multi-value fields (MC checkboxes) are joined with ", ". In preview mode, renders as `<<FormName:FieldName>>`. |
| `<image id="..." width="N" height="N"/>` | `ImageXmlReference` | Inline image from the project's image library. `id` is the image asset key. |
| `<invitation form="..." project="...">text</invitation>` | `InvitationReference` | A link to another form (or another project's form). Public version uses `form`+`project` attributes; private version adds `<authenticationTokenValue>` child. |
| `<link>...</link>` | `HyperlinkReference` | External hyperlink. Contains `<description><string value="..."/></description>` and `<url>...</url>` children, plus optional `<new-window/>`. |
| `function` elements | `FunctionXmlReference` | Dynamic function output (computed values via the function plugin system). |

### Blank nodes (form items only, not typical in documents)

| XML element | Class | Description |
|---|---|---|
| `<blank label="a" length="N" height="N" required="true\|false" alternateLabel="..."/>` | `XmlBlank` | Input field within a FIB question. Not typically used in document bodies, but parseable since both use `FormItemContentsFactory`. |

### MC-specific nodes (form items only)

| XML element | Class | Description |
|---|---|---|
| `<question>` | `Question` | Wraps the question text in an MC item |
| `<choice label="a">text</choice>` | `NewXmlChoice` | A static MC choice option |
| `<data-provider><dynamic-mcq .../></data-provider>` | `DataXmlProvider` | Dynamic MC choices from a function/data source |

---

## The `<field>` Reference — Rendering Logic

`FieldReference.toHtml(context)`:

1. Looks up the field by qualified name (`FormName:FieldName`) in `ExecutionContext`
2. Calls `context.getValues(name)` — returns a `List<Value>` (multi-value for MC checkboxes)
3. Joins multiple values with `", "` separator
4. In **preview mode**: renders as `<<FormName:FieldName>>` (literal placeholder)
5. In **customization marker mode**: wraps in `<span id="tawalaFieldReference_FormName:FieldName">` for client-side dynamic replacement
6. Otherwise: plain HTML-escaped text

This is how a Confirmation Letter personalizes itself — every `<field>` is replaced with the live submission value.

---

## The `<table>` at Runtime

The runtime `Table.java` uses a different XML vocabulary than the Designer. **Important divergence:**

| Designer XML | Runtime XML |
|---|---|
| `<table>` contains `<row>` and `<cell>` | `<table>` contains `<row>` and `<cell>` |
| `<table indent="N">` | `<table indent="N">` |
| Cell width in twips | Cell width in twips |

The Designer `TableXmlContents.ToXml()` serializes to:
```xml
<table indent="0">
  <row><cell width="3000">...</cell><cell width="3000">...</cell></row>
</table>
```

The runtime `Table.java` parses `config.children("row")` → `config.children("cell")`. This is a native XML table, **not** HTML `<tr>/<td>`. The runtime renders it to an HTML `<table>` with calculated column widths.

**Note:** The `FormItemContentsFactory` also registers XHTML variants (`<tbody>`, `<tr>`, `<td>`) for loading documents that were pasted from HTML. These are normalized to the XML form on save.

---

## How Documents Are Used in Processes

### As email body

```xml
<send>
  <from addressField="Registration:Email"/>
  <to addressLiteral="admin@league.com"/>
  <subject>Registration confirmed for <string field="Registration:FirstName"/></subject>
  <body document="Confirmation Letter"/>
</send>
```

The `Send` command looks up the named document from `context.getProject().getDocument("Confirmation Letter")`, calls `document.toHtml(context)`, and uses the rendered HTML as the email body.

### As displayed output

```xml
<show document="Receipt"/>
```

`ShowDocument` navigates the user to a rendered view of the document — a full browser page showing the document HTML, without any form inputs.

---

## The `InvitationField` / `<invitation>` Link

`InvitationField` is a project-level link object (like `Hyperlink`) that creates a clickable link pointing to another form — either in the same project or a different project.

**Public invitation** (link to a form, no auth):
```xml
<invitation form="PlayerSignup" project="">Click here to register a player</invitation>
```
- `project=""` means the current project
- `project="YouthLeagueRegistration"` links to a different project by name

**Private invitation** (link with authentication token — limits who can follow the link):
```xml
<invitation form="PlayerSignup" project="" private="true">
  <authenticationTokenValue>
    <string field="Registration:InviteToken"/>
  </authenticationTokenValue>
  Click here to register
</invitation>
```

`authenticationTokenValue` is a `CompoundExpression` — same expression system used in `SetStatement`. The token value is computed from field references and embedded in the generated URL, allowing the runtime to authenticate the link recipient.

---

## The `Hyperlink` / `<link>` Element

```xml
<link>
  <new-window/>   <!-- optional: opens in new tab -->
  <description><string value="Visit our website"/></description>
  <url>
    <string value="https://example.com"/>
  </url>
</link>
```

- `<new-window/>` presence (no attributes needed) triggers `target="_blank"`
- `<url>` uses the `UrlExpression` system — can contain field references for dynamic URLs
- `<description>` is the display text

---

## Document System Summary for Reimplementation

| Concept | Key facts |
|---|---|
| Document identity | Named string, referenced by `<send>` and `<show document="..."/>` |
| Content model | Shared with form item bodies — same `FormItemContentsFactory` parses both |
| Rendering | `document.toHtml(context)` at runtime; field refs resolved live from execution context |
| Field references | `<field name="FormName:FieldName"/>` — resolved from current submission/variables |
| Output wrapper | `<div class="document">` in the rendered HTML |
| Tables | XML-native `<table>/<row>/<cell>` (not HTML `<tr>/<td>`) |
| Font sizes | Stored in twips (÷20 for CSS pt); colors as 6-char hex without `#` |
| Images | Referenced by asset ID; uploaded to project image library |
| Links | Two types: `<invitation>` (cross-form/project) and `<link>` (external URL) |
| Multi-value fields | MC checkbox values joined with `", "` when field is referenced in a document |
| Preview mode | Field refs render as `<<FieldName>>` placeholder text |
