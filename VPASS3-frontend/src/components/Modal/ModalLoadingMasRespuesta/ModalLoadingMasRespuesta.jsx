import React from 'react';
import { Modal, Box, Typography, Button, CircularProgress, Stack } from '@mui/material';
import './ModalLoadingMasRespuesta.css';

const ModalLoadingMasRespuesta = ({ open, loading, message, accionPostCierre = () => {} }) => {

    const handleClose = (event, reason) => {
        if (reason === 'backdropClick' || reason === 'escapeKeyDown') {
          return; // No cerrar en estos casos
        }
        accionPostCierre(); // Solo cerrar si el usuario presiona el botón
    };
  return (
    <Modal open={open} onClose={handleClose}>
      <Box id="ContainerModalLoadingMasRespuesta">
        {loading ? (
          <>
            <CircularProgress />
            <Typography mt={2}>Cargando...</Typography>
          </>
        ) : (
          <>
            <Typography variant="h6">
              {message}
            </Typography>
            <Button variant="contained" onClick={accionPostCierre} sx={{ mt: 2 }}>
              Aceptar
            </Button>
          </>
        )}
      </Box>
    </Modal>
  );
};

export default ModalLoadingMasRespuesta;
