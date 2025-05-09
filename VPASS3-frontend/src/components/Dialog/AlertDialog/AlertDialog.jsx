import React from 'react';
import Button from '@mui/material/Button';
import Dialog from '@mui/material/Dialog';
import DialogActions from '@mui/material/DialogActions';
import DialogContent from '@mui/material/DialogContent';
import DialogContentText from '@mui/material/DialogContentText';
import DialogTitle from '@mui/material/DialogTitle';
import "./AlertDialog.css";
import ButtonTypeOne from '../../Buttons/ButtonTypeOne/ButtonTypeOne';
import { Box } from '@mui/material';

const AlertDialog = ({ 
  open, 
  onClose, 
  title = "Alerta", 
  message, 
  actionPostCierre 
}) => {
  const handleAccept = () => {
    if (typeof actionPostCierre === 'function') {
      actionPostCierre(); // Ejecuta la acción adicional
    }
    onClose();
  };

  return (
    <Dialog
        id="ContainerAlertDialog"
        open={open}
        onClose={(event, reason) => {
            // Evita el cierre al hacer clic fuera o presionar ESC
            if (reason === 'backdropClick' || reason === 'escapeKeyDown') {
                return;
            }
        }}
        disableEscapeKeyDown // Bloquea el cierre con la tecla ESC
    >
        <DialogTitle id="alert-dialog-title">{title}</DialogTitle>
        <DialogContent>
            <DialogContentText id="alert-dialog-description">
                {message}
            </DialogContentText>
        </DialogContent>
        <DialogActions id="DialogActionsAlertDialog">
            <Box id="BotonAceptarAlertDialog">
                <ButtonTypeOne
                    defaultText="Aceptar"
                    handleClick={handleAccept}
                />
            </Box>
        </DialogActions>
    </Dialog>
  );
};

export default AlertDialog;