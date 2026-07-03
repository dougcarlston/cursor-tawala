/** Registration FIB blocks for Java — layout aligned with 5173 registrationLayout.mjs */

const TAB_LEFT =
  '<tabPositions><tabStop position="4031"/><tabStop position="6192"/></tabPositions>';
const TAB_HINT =
  '<tabPositions><tabStop position="4031"/><tabStop position="6192"/></tabPositions>';
/** Label | P1+input | P2+input | P3+input — matches Designer tabbed columns */
const TAB_PHONE =
  '<tabPositions><tabStop position="4031"/><tabStop position="7331"/><tabStop position="10631"/></tabPositions>';

function paragraph(inner, tabs = TAB_LEFT) {
  return `<paragraph indent="0" align="left">${tabs}${inner}</paragraph>`;
}

function fontXml(text, escText, { bold = false, italic = false, size = 200 } = {}) {
  let inner = escText(text);
  if (italic) inner = `<i>${inner}</i>`;
  if (bold) inner = `<b>${inner}</b>`;
  return `<font face="Arial" size="${size}" color="000000">${inner}</font>`;
}

function blankXml(blank, letter, escAttr) {
  const alt = blank.alternateLabel ?? blank.name;
  const req = blank.required ? "true" : "false";
  const len = blank.length ?? 20;
  const altAttr = alt && alt !== letter ? ` alternateLabel="${escAttr(alt)}"` : "";
  return `<blank label="${escAttr(letter)}" length="${len}" required="${req}"${altAttr}/>`;
}

function blankLetter(i) {
  return String.fromCharCode(97 + (i % 26));
}

function findBlank(item, name) {
  return (
    item.blanks?.find((b) => b.name === name || b.alternateLabel === name) ?? {
      name,
      length: 18,
    }
  );
}

function hintRowXml(hints, escText) {
  let body = "";
  for (const h of hints) {
    body += "<tab/>";
    body += fontXml(h, escText, { italic: true });
  }
  return paragraph(body, TAB_HINT);
}

function namePairRowXml(label, first, last, letters, escAttr, escText) {
  let body = fontXml(`${label} `, escText, { bold: true });
  body += "<tab/>";
  body += blankXml(first, letters.get(first), escAttr);
  body += "<tab/>";
  body += blankXml(last, letters.get(last), escAttr);
  return paragraph(body);
}

function dobRowXml(mo, day, yr, letters, escAttr, escText) {
  let body = fontXml("Date of Birth: ", escText, { bold: true });
  body += fontXml("(mm/dd/yyyy)", escText, { italic: true });
  body += "<tab/>";
  body += blankXml(mo, letters.get(mo), escAttr);
  body += fontXml("/", escText);
  body += blankXml(day, letters.get(day), escAttr);
  body += fontXml("/", escText);
  body += blankXml(yr, letters.get(yr), escAttr);
  return paragraph(body);
}

function phoneRowXml(phone1, phone2, phone3, letters, escAttr, escText) {
  let body = fontXml("Parent Phone Numbers: ", escText, { bold: true });
  body += fontXml("(for emergencies)", escText, { italic: true });
  body += "<tab/>";
  body += fontXml("P1:", escText, { bold: true });
  body += blankXml(phone1, letters.get(phone1), escAttr);
  body += "<tab/>";
  body += fontXml("P2:", escText, { bold: true });
  body += blankXml(phone2, letters.get(phone2), escAttr);
  body += "<tab/>";
  body += fontXml("P3:", escText, { bold: true });
  body += blankXml(phone3, letters.get(phone3), escAttr);
  return paragraph(body, TAB_PHONE);
}

function registrationQ1Fib(item, escAttr, escText) {
  const first = findBlank(item, "FirstName");
  const last = findBlank(item, "LastName");
  const mo = findBlank(item, "RegAgeMo");
  const day = findBlank(item, "RegAgeDay");
  const yr = findBlank(item, "RegAgeYr");
  const letters = new Map(item.blanks.map((b, i) => [b, blankLetter(i)]));

  const body = [
    hintRowXml(["First", "Last"], escText),
    namePairRowXml("Name of Registrant:", first, last, letters, escAttr, escText),
    dobRowXml(mo, day, yr, letters, escAttr, escText),
  ].join("");

  return `<fib label="${escAttr(item.label)}">${body}</fib>`;
}

function registrationQ3Fib(item, escAttr, escText) {
  const blank = item.blanks?.[0] ?? { name: "a", length: 39 };
  const letters = new Map([[blank, "a"]]);
  let body = fontXml("Name of your School: ", escText, { bold: true });
  body += "<tab/>";
  body += blankXml(blank, "a", escAttr);
  return `<fib label="${escAttr(item.label)}">${paragraph(body)}</fib>`;
}

function registrationQ4Fib(item, escAttr, escText) {
  const first = findBlank(item, "ParentFirstName");
  const last = findBlank(item, "ParentLastName");
  const address = findBlank(item, "Address");
  const city = findBlank(item, "City");
  const zip = findBlank(item, "Zip");
  const email = findBlank(item, "ParentEmail");
  const emailAgain = findBlank(item, "g");
  const email2 = findBlank(item, "ParentEmail2");
  const email2Again = findBlank(item, "ParentEmail2Confirm");
  const phone1 = findBlank(item, "Phone1");
  const phone2 = findBlank(item, "Phone2");
  const phone3 = findBlank(item, "Phone3");
  const letters = new Map(item.blanks.map((b, i) => [b, blankLetter(i)]));

  let addressBody = fontXml("Address: ", escText, { bold: true });
  addressBody += "<tab/>";
  addressBody += blankXml(address, letters.get(address), escAttr);
  addressBody += fontXml("City:", escText, { bold: true });
  addressBody += blankXml(city, letters.get(city), escAttr);
  addressBody += fontXml("Zip:", escText, { bold: true });
  addressBody += blankXml(zip, letters.get(zip), escAttr);

  let emailBody = fontXml("Parent Email: ", escText, { bold: true });
  emailBody += "<tab/>";
  emailBody += blankXml(email, letters.get(email), escAttr);
  emailBody += fontXml("(again)", escText, { italic: true });
  emailBody += blankXml(emailAgain, letters.get(emailAgain), escAttr);

  let email2Body = fontXml("Additional Parent Email: ", escText, { bold: true });
  email2Body += fontXml("(optional)", escText, { italic: true });
  email2Body += "<tab/>";
  email2Body += blankXml(email2, letters.get(email2), escAttr);
  email2Body += fontXml("(again)", escText, { italic: true });
  email2Body += blankXml(email2Again, letters.get(email2Again), escAttr);

  const noteBody = fontXml("(All communications will be by email only)", escText, {
    italic: true,
    size: 180,
  });

  const body = [
    hintRowXml(["First", "Last"], escText),
    namePairRowXml("Parent Name:", first, last, letters, escAttr, escText),
    paragraph(addressBody),
    paragraph(emailBody),
    paragraph(email2Body),
    paragraph(noteBody),
    phoneRowXml(phone1, phone2, phone3, letters, escAttr, escText),
  ].join("");

  return `<fib label="${escAttr(item.label)}">${body}</fib>`;
}

/** Registration-only FIB export matching 5173 testbed layout. */
export function registrationFibToXml(item, escAttr, escText) {
  switch (item.label) {
    case "Q1":
      return registrationQ1Fib(item, escAttr, escText);
    case "Q3":
      return registrationQ3Fib(item, escAttr, escText);
    case "Q4":
      return registrationQ4Fib(item, escAttr, escText);
    default:
      return null;
  }
}
