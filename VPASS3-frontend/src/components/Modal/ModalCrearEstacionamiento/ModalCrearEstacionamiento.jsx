// ModalCrearEstacionamiento.jsx
import { Modal, Box, Typography, IconButton } from '@mui/material';
import CloseIcon from '@mui/icons-material/Close';
import "./ModalCrearEstacionamiento.css";
import useEstacionamiento from '../../../hooks/useEstacionamiento/useEstacionamiento';
import TextFieldUno from '../../TextField/TextFieldUno/TextFieldUno';
import { useState } from 'react';
import ButtonTypeOne from '../../Buttons/ButtonTypeOne/ButtonTypeOne';
import ModalLoadingMasRespuesta from '../ModalLoadingMasRespuesta/ModalLoadingMasRespuesta';
import { useConfirmDialog } from '../../../hooks/useConfirmDialog/useConfirmDialog';
import { useFormik } from 'formik';
import { useSelector } from 'react-redux';

const ModalCrearEstacionamiento = ({ open, onClose, setRows }) => {

  const { idEstablishment } = useSelector((state) => state.user);

  // Se obtienen las funciones y estados a utilizar del hook
  const { loading: loadingEstacionamientos, crearEstacionamiento } = useEstacionamiento();

  // Se invoca la función para consultarle al usuario si desea enviar el formulario
  const { confirm, ConfirmDialogComponent } = useConfirmDialog();

  // Estados y funciones para manejar el componente ModalLoadingMasRespuesta
  const [openLoadingRespuesta, setOpenLoadingRespuesta] = useState(false);
  const [messageLoadingRespuesta, setMessageLoadingRespuesta] = useState('');
  const [operacionExitosa, setOperacionExitosa] = useState(false);
  const accionPostCierreLoadingRespuesta = () => {
      setOpenLoadingRespuesta(false);
      setMessageLoadingRespuesta('');
  }

  const formik = useFormik({
    initialValues: {
        nombreNuevoEstacionamiento: '',
    },
    onSubmit: async (values) => {
        const confirmed = await confirm({
            title: "Crear estacionamiento",
            message: "¿Deseas crear un nuevo estacionamiento?"
        });
    
        if (confirmed) {
            setOpenLoadingRespuesta(true);

            const cuerpoPeticion = {
              name: values.nombreNuevoEstacionamiento,
              idEstablishment: idEstablishment
            }

            // Se envía la información al backend para crear el estacionamiento
            const {statusCode: statusCodeCrearEstacionamiento, data: dataEstacionamientoCreado, message: messageCrearEstacionamiento} = await crearEstacionamiento(cuerpoPeticion);

            // Si el servidor responde con el Response dto que tiene configurado
            if(statusCodeCrearEstacionamiento != null && statusCodeCrearEstacionamiento != undefined){

              if (statusCodeCrearEstacionamiento === 200 || statusCodeCrearEstacionamiento === 201) {
                setOperacionExitosa(true);
                setMessageLoadingRespuesta(messageCrearEstacionamiento);
                setRows(prevRows => [...prevRows, dataEstacionamientoCreado]);
              }
              else if (statusCodeCrearEstacionamiento === 500) {
                  //En caso de error 500, se muestra un mensaje de error genérico, en vez del mensaje de error del backend
                  setOperacionExitosa(false);
                  setMessageLoadingRespuesta("Error desconocido, por favor intente nuevamente más tarde.");
              }
              else{
                  //En caso de cualquier otro error, se muestra el mensaje de error del backend
                  setOperacionExitosa(false);
                  setMessageLoadingRespuesta(messageCrearEstacionamiento);
              }
            }
            else{
              //Esto es para los casos que el servidor no responda el ResponseDto tipico
              setOperacionExitosa(false);
              setMessageLoadingRespuesta("Error desconocido, por favor intente nuevamente más tarde.");
            }
        } 
    }
  });

  return (
    <Modal open={open} onClose={onClose}>
      <Box id="ContainerModalCrearEstacionamiento">

        <Box id="HeaderModalCrearEstacionamiento">
          <Typography variant="h5" component="h5" gutterBottom>
            {"Nuevo Estacionamiento"}
          </Typography>

          <IconButton
            aria-label="close"
            onClick={onClose}
            sx={{
              top: -8,
              color: "black",
            }}
          >
            <CloseIcon sx={{fontSize: "30px"}} />
          </IconButton>
        </Box>

        <Box id="CuerpoModalCrearEstacionamiento">
            <TextFieldUno 
            name='nombreNuevoEstacionamiento'
            placeholder='Ingrese el nombre del nuevo estacionamiento'
            label='Nombre de estacionamiento'
            onChange={formik.handleChange}
            />

            <Box>
              <ButtonTypeOne
              defaultText='Crear nuevo estacionamiento'
              handleClick={formik.handleSubmit}
              />
            </Box>

            {ConfirmDialogComponent}

            <ModalLoadingMasRespuesta
                open={openLoadingRespuesta}
                loading={loadingEstacionamientos}
                message={messageLoadingRespuesta}
                loadingMessage="Creando estacionamiento..."
                successfulProcess={operacionExitosa}
                accionPostCierre={accionPostCierreLoadingRespuesta}
            />
        </Box>
      </Box>
    </Modal>
  );
};

export default ModalCrearEstacionamiento;
