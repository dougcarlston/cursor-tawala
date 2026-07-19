/**
 * @vitest-environment happy-dom
 */
import { afterEach, describe, expect, it } from "vitest";
import {
  clearEmailDeliveryDialog,
  getEmailDeliveryOpen,
  openEmailDeliveryDialog,
} from "./emailDelivery";

describe("emailDelivery dialog request", () => {
  afterEach(() => {
    clearEmailDeliveryDialog();
  });

  it("opens and clears without storing secrets", () => {
    expect(getEmailDeliveryOpen()).toBe(false);
    openEmailDeliveryDialog();
    expect(getEmailDeliveryOpen()).toBe(true);
    clearEmailDeliveryDialog();
    expect(getEmailDeliveryOpen()).toBe(false);
  });
});
