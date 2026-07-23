/**
 * DYNAMIC MCQ Configure Function ↔ choice JSON unit tests.
 */
import { describe, expect, it } from "vitest";
import {
  configFromDynamicChoice,
  dynamicChoiceFromConfig,
  emptyDynamicChoice,
  findDynamicChoice,
  getDynamicMcqDef,
  normalizeDynamicMcqFieldRef,
} from "./mcDynamicConfig";
import { configMeetsRequirements } from "./functionCatalog";

describe("mcDynamicConfig", () => {
  it("catalog entry is form-only with Display/Value/Form required", () => {
    const def = getDynamicMcqDef();
    expect(def.formOnly).toBe(true);
    expect(def.parameters.map((p) => p.id)).toEqual([
      "form-name",
      "display-expression",
      "value-expression",
      "sort-expression",
      "conditions",
    ]);
  });

  it("normalizes palette Form:Field refs to Record:Form:Field", () => {
    expect(normalizeDynamicMcqFieldRef("<<Form 1:ChoiceName>>", "Form 1")).toBe(
      "<<Record:Form 1:ChoiceName>>",
    );
    expect(normalizeDynamicMcqFieldRef("Form 1:ChoiceName", "Form 1")).toBe(
      "<<Record:Form 1:ChoiceName>>",
    );
    expect(normalizeDynamicMcqFieldRef("ChoiceName", "Form 1")).toBe(
      "<<Record:Form 1:ChoiceName>>",
    );
    expect(normalizeDynamicMcqFieldRef("<<Record:Form 1:ChoiceName>>")).toBe(
      "<<Record:Form 1:ChoiceName>>",
    );
  });

  it("round-trips Configure config ↔ dynamic choice", () => {
    const config = configFromDynamicChoice({
      type: "dynamic",
      sourceForm: "Sheet",
      displayExpr: "<<Record:Sheet:SheetName>>",
      valueExpr: "<<Record:Sheet:SheetName>>",
      sortExpr: "<<Record:Sheet:Count>>",
      conditionsRows: [{ field: "Record:Sheet:Count", op: "isLessThan", value: "10" }],
      conditionsCombinator: "and",
    });
    expect(config["form-name"]).toBe("Sheet");
    expect(configMeetsRequirements(getDynamicMcqDef(), config)).toBe(true);

    const choice = dynamicChoiceFromConfig(config);
    expect(choice).toEqual({
      type: "dynamic",
      sourceForm: "Sheet",
      displayExpr: "<<Record:Sheet:SheetName>>",
      valueExpr: "<<Record:Sheet:SheetName>>",
      sortExpr: "<<Record:Sheet:Count>>",
      conditionsRows: [{ field: "Record:Sheet:Count", op: "isLessThan", value: "10" }],
      conditionsCombinator: "and",
    });
  });

  it("saves palette drops with Record: prefix", () => {
    const config = configFromDynamicChoice(emptyDynamicChoice());
    config["form-name"] = "Form 1";
    config["display-expression"] = "<<Form 1:ChoiceName>>";
    config["value-expression"] = "<<Form 1:ChoiceName>>";
    const choice = dynamicChoiceFromConfig(config);
    expect(choice.displayExpr).toBe("<<Record:Form 1:ChoiceName>>");
    expect(choice.valueExpr).toBe("<<Record:Form 1:ChoiceName>>");
  });

  it("omits empty conditions from the saved choice", () => {
    const config = configFromDynamicChoice(emptyDynamicChoice());
    config["form-name"] = "Sheet";
    config["display-expression"] = "<<Record:Sheet:SheetName>>";
    config["value-expression"] = "<<Record:Sheet:SheetName>>";
    const choice = dynamicChoiceFromConfig(config);
    expect(choice.conditionsRows).toBeUndefined();
    expect(findDynamicChoice([choice])?.sourceForm).toBe("Sheet");
  });

  it("opens Configure from imported where tree", () => {
    const config = configFromDynamicChoice({
      type: "dynamic",
      sourceForm: "Sheet",
      displayExpr: "<<Record:Sheet:Name>>",
      valueExpr: "<<Record:Sheet:Id>>",
      where: {
        and: [
          { field: "Record:Sheet:Count", op: "isLessThan", value: "10" },
          { field: "Record:Sheet:Active", op: "equals", value: "yes" },
        ],
      },
    });
    expect(config.conditionsCombinator).toBe("and");
    expect(config.conditionsRows).toEqual([
      { field: "Record:Sheet:Count", op: "isLessThan", value: "10" },
      { field: "Record:Sheet:Active", op: "equals", value: "yes" },
    ]);
  });

  it("requires form + display + value before OK", () => {
    const def = getDynamicMcqDef();
    const config = configFromDynamicChoice(null);
    expect(configMeetsRequirements(def, config)).toBe(false);
    config["form-name"] = "Sheet";
    config["display-expression"] = "<<Record:Sheet:SheetName>>";
    config["value-expression"] = "<<Record:Sheet:SheetName>>";
    expect(configMeetsRequirements(def, config)).toBe(true);
  });
});
