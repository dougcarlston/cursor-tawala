import { describe, expect, it } from "vitest";
import {
  commandUnknownMessage,
  isVacatedTomcatName,
  parseRetireDeploymentXml,
  retireDeploymentRequestXml,
  vacatedTomcatName,
} from "./retireDeployment.mjs";
import { decideNameOccupancy, findOccupantInDeploymentsXml } from "./deployOccupancy.mjs";

describe("vacatedTomcatName", () => {
  it("keeps uniqueId in the suffix and stays under 100 chars", () => {
    const n = vacatedTomcatName("Simple Survey Template", "gy1zssbrwm4fgfm");
    expect(n).toBe("Simple Survey Template (retired gy1zssbrwm4fgfm)");
    expect(n.length).toBeLessThanOrEqual(100);
    expect(isVacatedTomcatName(n, "gy1zssbrwm4fgfm")).toBe(true);
  });

  it("is a no-op when already vacated for this uniqueId", () => {
    const once = vacatedTomcatName("Picnic", "abc123");
    expect(vacatedTomcatName(once, "abc123")).toBe(once);
  });

  it("does not treat a sibling uniqueId as this occupant", () => {
    expect(isVacatedTomcatName("Online Exam Builder", "u3hkqgwtrepjlur")).toBe(false);
    expect(isVacatedTomcatName("Online Exam Builder-8-3-26", "u3hkqgwtrepjlur")).toBe(false);
  });
});

describe("parseRetireDeploymentXml", () => {
  it("reads a Hibernate rename receipt", () => {
    const xml = `<?xml version="1.0"?>
<response status="success">
  <retired uniqueId="gy1zssbrwm4fgfm" previousName="Simple Survey Template" name="Simple Survey Template (retired gy1zssbrwm4fgfm)" alreadyVacated="false"/>
</response>`;
    const p = parseRetireDeploymentXml(xml);
    expect(p.status).toBe("success");
    expect(p.uniqueId).toBe("gy1zssbrwm4fgfm");
    expect(p.previousName).toBe("Simple Survey Template");
    expect(p.alreadyVacated).toBe(false);
  });

  it("fails closed on command.unknown (old WAR)", () => {
    const xml = `<?xml version="1.0"?>
<response status="failure">
  <error id="command.unknown" message="Unknown command 'retireDeployment'."/>
</response>`;
    const p = parseRetireDeploymentXml(xml);
    expect(p.status).toBe("failure");
    expect(p.code).toBe("command.unknown");
  });
});

describe("retireDeploymentRequestXml", () => {
  it("sends uniqueId not display name", () => {
    const xml = retireDeploymentRequestXml("dev", "dev", "u3hkqgwtrepjlur");
    expect(xml).toMatch(/type="retireDeployment"/);
    expect(xml).toMatch(/uniqueId="u3hkqgwtrepjlur"/);
    expect(xml).not.toMatch(/Online Exam Builder/);
  });
});

describe("occupancy after vacated name", () => {
  it("queryDeployments no longer blocks the old display name", () => {
    const xml = `<?xml version="1.0"?>
<response status="success">
  <deployments user="dev">
    <deployment project="Simple Survey Template (retired gy1zssbrwm4fgfm)">
      <startpoint form="Survey" url="http://localhost:8080/p/gy1zssbrwm4fgfm/npwtqlg.Survey"/>
    </deployment>
  </deployments>
</response>`;
    expect(findOccupantInDeploymentsXml(xml, "Simple Survey Template")).toBeNull();
    const d = decideNameOccupancy({
      tomcatName: "Simple Survey Template",
      incomingProject: { name: "Simple Survey Template" },
      occupant: findOccupantInDeploymentsXml(xml, "Simple Survey Template"),
    });
    expect(d.allow).toBe(true);
    expect(d.code).toBe("unoccupied");
  });

  it("does not free a sibling that still holds the bare catalog title", () => {
    const xml = `<?xml version="1.0"?>
<response status="success">
  <deployments user="dev">
    <deployment project="Online Exam Builder-8-3-26 (retired u3hkqgwtrepjlur)">
      <startpoint form="Exam" url="http://localhost:8080/p/u3hkqgwtrepjlur/sto3lpi.Exam"/>
    </deployment>
    <deployment project="Online Exam Builder">
      <startpoint form="Exam" url="http://localhost:8080/p/455sem0swhcswu5/e.Exam"/>
    </deployment>
  </deployments>
</response>`;
    const occupant = findOccupantInDeploymentsXml(xml, "Online Exam Builder");
    expect(occupant.uniqueId).toBe("455sem0swhcswu5");
    expect(commandUnknownMessage()).toMatch(/ROOT\.war/);
  });
});
