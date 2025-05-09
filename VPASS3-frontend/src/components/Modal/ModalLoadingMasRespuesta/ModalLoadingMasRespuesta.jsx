import { Modal, Box, Typography, Button, CircularProgress, Stack } from '@mui/material';
import './ModalLoadingMasRespuesta.css';
import ButtonTypeOne from '../../Buttons/ButtonTypeOne/ButtonTypeOne';
import { IconoExito, IconoFallo } from '../../../icons/iconos';
import { useEffect, useState } from 'react';


// Componente tipo modal que se muestra cuando se está cargando algo o cuando se ha completado un proceso
// y se muestra un mensaje de éxito o error.
// Se utiliza para mostrar mensajes de carga y resultados de procesos, como éxito o error.
// Recibe props para controlar su apertura, el estado de carga, mensajes y acciones post-cierre.

// Props:
// - open: booleano que indica si el modal está abierto o cerrado.
// - loading: booleano que indica si se está cargando algo (true) o si se ha completado un proceso (false).
// - loadingMessage: mensaje que se muestra mientras se está cargando (opcional, por defecto "cargando...").
// - message: mensaje que se muestra al completar el proceso (opcional).
// - accionPostCierre: función que se ejecuta al cerrar el modal (opcional, por defecto una función vacía).
// - successfulProcess: booleano que indica si el proceso fue exitoso (true) o fallido (false).

// Ejemplo de uso:

/*const componenteEjemplo = () => {

  // Estados y funciones para manejar el componente ModalLoadingMasRespuesta
    const [loading, setLoading] = useState(false);
    const [openLoadingRespuesta, setOpenLoadingRespuesta] = useState(false);
    const [operacionExitosa, setOperacionExitosa] = useState(false);
    const accionPostCierreLoadingRespuesta = () => {
        setOpenLoadingRespuesta(false);
        setMessageLoadingRespuesta('');
  }

  return (
  <>
    // ... Código del formulario)
    <Button
        variant="contained"
        color="primary"
        onClick={async () => {
          setOpenLoadingRespuesta(true);
          setloading(true);
          try {
            // Simulación de una operación asincrónica (ejemplo: envío de formulario)
            await new Promise((resolve) => setTimeout(resolve, 2000));
            setOperacionExitosa(true);
            setMessageLoadingRespuesta('Operación exitosa!');
          } catch (error) {
            setOperacionExitosa(false);
            setMessageLoadingRespuesta('Error en la operación!');
          }
          finally {
            setLoading(false);
          }
        }}
        disabled={isSubmitting}
    > 
      Enviar
    </Button>

    <ModalLoadingMasRespuesta
        open={openLoadingRespuesta}
        loading={loading}
        loadingMessage="Cargando..."
        message={messageLoadingRespuesta}
        accionPostCierre={accionPostCierreLoadingRespuesta}
        successfulProcess={operacionExitosa}
   </>
    }
*/
const ModalLoadingMasRespuesta = ({ 
  open,
  loading, 
  loadingMessage = "cargando...", 
  message, 
  accionPostCierre = () => {}, 
  successfulProcess = false
}) => {

  const handleClose = (event, reason) => {
    if (reason === 'backdropClick' || reason === 'escapeKeyDown') return;
  };

  const ejecutarAccionPostCierre = () => {
    if (typeof accionPostCierre === 'function') {
      accionPostCierre();
    }
  }

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
            <Typography id="TituloModalLoadingMasRespuesta" variant="h6">
              {successfulProcess ? "Proceso exitoso" : "Error en el proceso"}
            </Typography>

            {successfulProcess ? (
              <IconoExito 
                sx={{ fontSize: "8rem" }}
              />
            ) : (
              <IconoFallo 
                sx={{ fontSize: "8rem" }}
              />
            )}
            <Typography id="DescripcionModalLoadingMasRespuesta" variant="body" mt={2}>
              {message}
            </Typography>
            <ButtonTypeOne
                defaultText="Aceptar"
                handleClick={ejecutarAccionPostCierre}
            />
          </>
        )}
      </Box>
    </Modal>
  );
};

export default ModalLoadingMasRespuesta;
