import React from 'react';
import { Modal, Box, Typography, Button, CircularProgress, Stack } from '@mui/material';
import './ModalLoadingMasRespuesta.css';
import ButtonTypeOne from '../../Buttons/ButtonTypeOne/ButtonTypeOne';

const ModalLoadingMasRespuesta = ({ open, loading, loadingMessage = "cargando...", message, accionPostCierre = () => {} }) => {

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
            <Typography mt={2}>{loadingMessage}</Typography>
          </>
        ) : (
          <>
            <Typography variant="h6">
              {message}
            </Typography>
            <ButtonTypeOne
                defaultText="Aceptar"
                handleClick={accionPostCierre}
            />
          </>
        )}
      </Box>
    </Modal>
  );
};

export default ModalLoadingMasRespuesta;
