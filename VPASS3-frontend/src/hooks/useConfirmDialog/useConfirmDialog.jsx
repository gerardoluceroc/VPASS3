import { useState, useCallback } from "react";
import ConfirmDialog from "../../components/Modal/ConfirmDialog/ConfirmDialog";

export const useConfirmDialog = () => {
  const [isOpen, setIsOpen] = useState(false);
  const [resolver, setResolver] = useState(null);
  const [dialogProps, setDialogProps] = useState({
    title: '',
    message: '',
  });

  const confirm = useCallback(({ title = "Confirmación", message = "¿Estás seguro?" }) => {
    setDialogProps({ title, message });
    setIsOpen(true);

    return new Promise((resolve) => {
      setResolver(() => resolve);
    });
  }, []);

  const handleConfirm = () => {
    setIsOpen(false);
    resolver?.(true);
  };

  const handleCancel = () => {
    setIsOpen(false);
    resolver?.(false);
  };

  // Este componente debes renderizarlo UNA VEZ en tu app (idealmente en App.jsx o Layout)
  const ConfirmDialogComponent = (
    <ConfirmDialog
      open={isOpen}
      message={dialogProps.message}
      onConfirm={handleConfirm}
      onCancel={handleCancel}
    />
  );

  return { confirm, ConfirmDialogComponent };
};
