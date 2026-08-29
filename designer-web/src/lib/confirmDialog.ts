export interface ConfirmDialogOptions {
  title?: string;
  message: string;
  okText?: string;
  cancelText?: string;
  destructive?: boolean;
}

type ConfirmResolver = (value: boolean) => void;

let currentConfirm: {
  options: ConfirmDialogOptions;
  resolve: ConfirmResolver;
} | null = null;

const listeners = new Set<() => void>();

function notify(): void {
  listeners.forEach((l) => l());
}

export function showDesignerConfirm(options: ConfirmDialogOptions | string): Promise<boolean> {
  const normalizedOptions: ConfirmDialogOptions =
    typeof options === "string" ? { message: options } : options;

  return new Promise<boolean>((resolve) => {
    // If a dialog is already showing, resolve the previous one as false
    if (currentConfirm) {
      currentConfirm.resolve(false);
    }
    currentConfirm = {
      options: normalizedOptions,
      resolve: (result: boolean) => {
        currentConfirm = null;
        notify();
        resolve(result);
      },
    };
    notify();
  });
}

export function getActiveConfirm(): {
  options: ConfirmDialogOptions;
  resolve: ConfirmResolver;
} | null {
  return currentConfirm;
}

export function subscribeConfirmDialog(listener: () => void): () => void {
  listeners.add(listener);
  return () => listeners.delete(listener);
}
