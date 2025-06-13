import { useSelector } from "react-redux";
import "./ModalCrearZona.css";
import useZonas from "../../../hooks/useZonas/useZonas";
import ModalLoadingMasRespuesta from "../ModalLoadingMasRespuesta/ModalLoadingMasRespuesta";
import { useFormik } from "formik";
import ValidationCrearZona from "./ValidationCrearZona";
import { useConfirmDialog } from "../../../hooks/useConfirmDialog/useConfirmDialog";
import { useEffect, useState } from "react";
import { Box, IconButton, Modal, Typography } from "@mui/material";
import CloseIcon from '@mui/icons-material/Close';
import TextFieldUno from "../../TextField/TextFieldUno/TextFieldUno";
import ButtonTypeOne from "../../Buttons/ButtonTypeOne/ButtonTypeOne";


const ModalCrearZona = ({ open, onClose, setRows }) => {

    const { idEstablishment } = useSelector((state) => state.user);

    const {loading, crearZona} = useZonas();

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
            nombreZona: ''
        },
        validationSchema: ValidationCrearZona,
        onSubmit: async (values) => {
            const confirmed = await confirm({
                title: "¿Crear zona?",
                message: "¿Deseas crear una nueva zona en el establecimiento?"
            });
        
            if (confirmed) {
                setOpenLoadingRespuesta(true);

                // Se envía la información al backend para crear una nueva zona
                const {statusCode: statusCodeCrearZona, data: dataZonaAgregada, message: messageCrearZona} = await crearZona(idEstablishment, values.nombreZona);

                // Si el servidor responde con el Response dto que tiene configurado
                if(statusCodeCrearZona != null && statusCodeCrearZona != undefined){
    
                  if (statusCodeCrearZona === 200 || statusCodeCrearZona === 201) {
                    setOperacionExitosa(true);
                    setMessageLoadingRespuesta(messageCrearZona);
                    setRows(prevRows => [...prevRows, dataZonaAgregada]); // Agrega la nueva zona a las filas de la tabla
                    formik.resetForm(); // Resetea el formulario después de crear la zona
                  }
                  else if (statusCodeCrearZona === 500) {
                      //En caso de error 500, se muestra un mensaje de error genérico, en vez del mensaje de error del backend
                      setOperacionExitosa(false);
                      setMessageLoadingRespuesta("Error desconocido, por favor intente nuevamente más tarde.");
                  }
                  else{
                      //En caso de cualquier otro error, se muestra el mensaje de error del backend
                      setOperacionExitosa(false);
                      setMessageLoadingRespuesta(messageCrearZona);
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
        <Box id="ContainerModalCrearZona">
            <Box id="HeaderModalCrearZona">
                <Typography variant="h5" component="h5" gutterBottom>
                    {"Crear nueva zona"}
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

            <Box id="CuerpoModalCrearZona">
                <TextFieldUno 
                    name="nombreZona" 
                    label="Nombre de la zona" 
                    placeholder="Ej: Zona A" 
                    value={formik.values.nombreZona}
                    onChange={formik.handleChange}
                    error={formik.touched.nombreZona && Boolean(formik.errors.nombreZona)}
                    helperText={formik.touched.nombreZona && formik.errors.nombreZona}
                />
            </Box>

            <Box id="BoxButtonSubmitModalCrearZona">
                <ButtonTypeOne
                    defaultText="Crear zona"
                    loadingText="Creando zona..."
                    handleClick={formik.handleSubmit}
                    disabled={formik.isSubmitting}
                />
            </Box>

            {ConfirmDialogComponent}

            <ModalLoadingMasRespuesta
                open={openLoadingRespuesta}
                loading={loading}
                message={messageLoadingRespuesta}
                loadingMessage="Creando zona..."
                successfulProcess={operacionExitosa}
                accionPostCierre={accionPostCierreLoadingRespuesta}
            />

        </Box>
    </Modal>
  )
}

export default ModalCrearZona;