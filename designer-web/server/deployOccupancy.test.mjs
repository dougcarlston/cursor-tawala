import { describe, expect, it } from "vitest";
import {
  claimedAuthorId,
  decideNameOccupancy,
  findOccupantInDeploymentsXml,
  findOccupantInNodeIndex,
  incomingOwnsOccupant,
  isMintedDeployName,
  occupancyHatchAllowed,
} from "./deployOccupancy.mjs";

const LIBRARY_XML = `<?xml version="1.0" encoding="UTF-8"?>
<response status="success">
  <deployments user="dev">
    <deployment project="Simple Survey Template">
      <startpoint form="Survey" url="http://localhost:8080/p/gy1zssbrwm4fgfm/npwtqlg.Survey"/>
    </deployment>
    <deployment project="Smoke26 Survey 54ef790c">
      <startpoint form="Survey" url="http://localhost:8080/p/oxixxno90gttkyp/cth0mod.Survey"/>
    </deployment>
  </deployments>
</response>
`;

describe("isMintedDeployName", () => {
  it("matches File→New / clone suffix", () => {
    expect(isMintedDeployName("Online Exam Builder 54ef790c")).toBe(true);
  });

  it("does not match Library catalog titles", () => {
    expect(isMintedDeployName("Simple Survey Template")).toBe(false);
    expect(isMintedDeployName("Online Exam Builder")).toBe(false);
  });
});

describe("claimedAuthorId", () => {
  it("extracts authorId, author, or userId", () => {
    expect(claimedAuthorId({ authorId: "alice" })).toBe("alice");
    expect(claimedAuthorId({ author: "bob" })).toBe("bob");
    expect(claimedAuthorId({ userId: "charlie" })).toBe("charlie");
    expect(claimedAuthorId({})).toBe("");
    expect(claimedAuthorId(null)).toBe("");
  });
});

describe("findOccupantInDeploymentsXml", () => {
  it("finds Library Simple Survey by Tomcat name", () => {
    const o = findOccupantInDeploymentsXml(LIBRARY_XML, "Simple Survey Template");
    expect(o).toEqual({ name: "Simple Survey Template", uniqueId: "gy1zssbrwm4fgfm" });
  });

  it("returns null when the name is free", () => {
    expect(findOccupantInDeploymentsXml(LIBRARY_XML, "Brand New Picnic aabbccdd")).toBeNull();
  });

  it("does not treat a minted sibling name as the catalog title", () => {
    const xml = `<?xml version="1.0"?>
<response status="success">
  <deployments user="dev">
    <deployment project="Online Exam Builder a1b2c3d4">
      <startpoint form="Exam" url="http://localhost:8080/p/privclone0001/e.Exam"/>
    </deployment>
  </deployments>
</response>`;
    expect(findOccupantInDeploymentsXml(xml, "Online Exam Builder")).toBeNull();
  });
});

describe("findOccupantInNodeIndex", () => {
  it("matches case-insensitively", () => {
    const o = findOccupantInNodeIndex(
      [{ name: "Simple Survey Template", uniqueId: "gy1zssbrwm4fgfm" }],
      "simple survey template",
    );
    expect(o.uniqueId).toBe("gy1zssbrwm4fgfm");
  });
});

describe("incomingOwnsOccupant", () => {
  const occupant = { name: "Smoke26 Survey 54ef790c", uniqueId: "oxixxno90gttkyp" };

  it("allows matching deployUniqueId", () => {
    expect(
      incomingOwnsOccupant({ name: "Smoke26 Survey", deployUniqueId: "oxixxno90gttkyp" }, occupant),
    ).toBe(true);
  });

  it("allows minted deployIdentityName even without uniqueId", () => {
    expect(
      incomingOwnsOccupant(
        { name: "Smoke26 Survey", deployIdentityName: "Smoke26 Survey 54ef790c" },
        occupant,
      ),
    ).toBe(true);
  });

  it("allows matching author between incoming project and occupant", () => {
    const owned = { name: "My Math Quiz", uniqueId: "mathquiz123", author: "alice" };
    expect(
      incomingOwnsOccupant({ name: "My Math Quiz", author: "alice" }, owned),
    ).toBe(true);
  });

  it("allows matching authenticated user when occupant has author", () => {
    const owned = { name: "My Math Quiz", uniqueId: "mathquiz123", author: "alice" };
    expect(
      incomingOwnsOccupant({ name: "My Math Quiz" }, owned, { user: "alice" }),
    ).toBe(true);
  });

  it("refuses different author", () => {
    const owned = { name: "My Math Quiz", uniqueId: "mathquiz123", author: "alice" };
    expect(
      incomingOwnsOccupant({ name: "My Math Quiz", author: "bob" }, owned, { user: "bob" }),
    ).toBe(false);
  });

  it("refuses catalog JSON (display name only)", () => {
    const lib = { name: "Simple Survey Template", uniqueId: "gy1zssbrwm4fgfm" };
    expect(incomingOwnsOccupant({ name: "Simple Survey Template" }, lib)).toBe(false);
    expect(
      incomingOwnsOccupant(
        { name: "Simple Survey Template", deployIdentityName: "Simple Survey Template" },
        lib,
      ),
    ).toBe(false);
  });
});

describe("occupancyHatchAllowed", () => {
  const env = { TAWALA_OCCUPANCY_HATCH: "cleanup-secret" };

  it("is off when env is unset", () => {
    expect(
      occupancyHatchAllowed(
        { allowOccupiedNameRedeploy: true, occupancyHatch: "cleanup-secret" },
        {},
      ),
    ).toBe(false);
  });

  it("requires flag + matching token", () => {
    expect(occupancyHatchAllowed({ allowOccupiedNameRedeploy: true }, env)).toBe(false);
    expect(
      occupancyHatchAllowed(
        { allowOccupiedNameRedeploy: true, occupancyHatch: "wrong" },
        env,
      ),
    ).toBe(false);
    expect(
      occupancyHatchAllowed(
        { allowOccupiedNameRedeploy: true, occupancyHatch: "cleanup-secret" },
        env,
      ),
    ).toBe(true);
  });
});

describe("decideNameOccupancy", () => {
  const lib = { name: "Simple Survey Template", uniqueId: "gy1zssbrwm4fgfm" };

  it("allows a free Tomcat name", () => {
    const d = decideNameOccupancy({
      tomcatName: "Picnic aabbccdd",
      incomingProject: { name: "Picnic", _freshFromTemplate: true },
      occupant: null,
    });
    expect(d.allow).toBe(true);
  });

  it("refuses catalog Push onto Library uniqueId", () => {
    const d = decideNameOccupancy({
      tomcatName: "Simple Survey Template",
      incomingProject: { name: "Simple Survey Template" },
      occupant: lib,
    });
    expect(d.allow).toBe(false);
    expect(d.code).toBe("name-occupied");
    expect(d.message).toMatch(/gy1zssbrwm4fgfm/);
  });

  it("allows own clone Redeploy", () => {
    const d = decideNameOccupancy({
      tomcatName: "Smoke26 Survey 54ef790c",
      incomingProject: {
        name: "Smoke26 Survey",
        deployIdentityName: "Smoke26 Survey 54ef790c",
        deployUniqueId: "oxixxno90gttkyp",
      },
      occupant: { name: "Smoke26 Survey 54ef790c", uniqueId: "oxixxno90gttkyp" },
    });
    expect(d.allow).toBe(true);
    expect(d.code).toBe("owns-occupant");
  });

  it("allows authenticated author update of own deployment", () => {
    const owned = { name: "Weekly Status Poll", uniqueId: "poll998877", author: "alice" };
    const d = decideNameOccupancy({
      tomcatName: "Weekly Status Poll",
      incomingProject: { name: "Weekly Status Poll" },
      occupant: owned,
      user: "alice",
    });
    expect(d.allow).toBe(true);
    expect(d.code).toBe("owns-occupant");
  });

  it("refuses another user updating occupied deployment", () => {
    const owned = { name: "Weekly Status Poll", uniqueId: "poll998877", author: "alice" };
    const d = decideNameOccupancy({
      tomcatName: "Weekly Status Poll",
      incomingProject: { name: "Weekly Status Poll" },
      occupant: owned,
      user: "bob",
    });
    expect(d.allow).toBe(false);
    expect(d.code).toBe("name-occupied");
  });

  it("allows hatch Redeploy of Library", () => {
    const d = decideNameOccupancy({
      tomcatName: "Simple Survey Template",
      incomingProject: { name: "Simple Survey Template" },
      occupant: lib,
      hatch: true,
    });
    expect(d.allow).toBe(true);
    expect(d.code).toBe("hatch");
  });

  it("fails closed when occupancy cannot be queried", () => {
    const d = decideNameOccupancy({
      tomcatName: "Simple Survey Template",
      incomingProject: { name: "Simple Survey Template" },
      lookupFailed: true,
    });
    expect(d.allow).toBe(false);
    expect(d.code).toBe("occupancy-lookup-failed");
  });
});
