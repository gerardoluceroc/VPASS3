import React from 'react';
import { Modal, Box, Typography, Button, Stack } from '@mui/material';
import './ConfirmDialog.css'; // Asegúrate de importar el archivo CSS
import ButtonTypeOne from '../../Buttons/ButtonTypeOne/ButtonTypeOne';

const ConfirmDialog = ({ open, message, onConfirm, onCancel }) => {
  return (
    <Modal open={open} onClose={onCancel}>
      <Box id="ContainerConfirmDialog">
        <Typography variant="body1" mb={3} sx={{ wordWrap: 'break-word', whiteSpace: 'normal' }}>
          {message}
        </Typography>
        <Stack direction="row" justifyContent="center" spacing={2}>
            <ButtonTypeOne
                defaultText="Aceptar"
                handleClick={onConfirm}
            />
            <ButtonTypeOne
                defaultText="Cancelar"
                handleClick={onCancel}
            />
        </Stack>
      </Box>
    </Modal>
  );
};

export default ConfirmDialog;


