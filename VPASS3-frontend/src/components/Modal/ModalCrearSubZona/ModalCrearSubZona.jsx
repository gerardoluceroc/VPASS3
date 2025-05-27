import { useFormik } from "formik";
import ValidationCrearSubZona from "./ValidationCrearSubZona";
import { useConfirmDialog } from "../../../hooks/useConfirmDialog/useConfirmDialog";
import useSubZona from "../../../hooks/useSubZona/useSubZona";
import ModalLoadingMasRespuesta from "../ModalLoadingMasRespuesta/ModalLoadingMasRespuesta";
import ButtonTypeOne from "../../Buttons/ButtonTypeOne/ButtonTypeOne";
import { Box, IconButton, Modal, Typography } from "@mui/material";
import TextFieldUno from "../../TextField/TextFieldUno/TextFieldUno";
import CloseIcon from '@mui/icons-material/Close';
import { agregarSubZona } from "../../PagesComponents/GestionZonasPageComponent/funcionesGestionZonasPageComponent";
import { useEffect, useState } from "react";
import "./ModalCrearSubZona.css";


const ModalCrearSubZona = ({ open, onClose, setRows, idZona }) => {



    useEffect(() => {console.log("- [ModalCrearSubZona.jsx] idZona => ",idZona)}, [idZona]);

    const {loading, crearSubZona} = useSubZona();

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
            nombreSubZona: ''
        },
        validationSchema: ValidationCrearSubZona,
        onSubmit: async (values) => {
            const confirmed = await confirm({
                title: "¿Crear subzona?",
                message: "¿Deseas crear una nueva subzona en el establecimiento?"
            });
        
            if (confirmed) {
                setOpenLoadingRespuesta(true);

                // Se envía la información al backend para crear una nueva subzona
                const {statusCode: statusCodeCrearSubZona, data: dataSubZonaAgregada, message: messageCrearSubZona} = await crearSubZona(idZona, values.nombreSubZona);

                // Si el servidor responde con el Response dto que tiene configurado
                if(statusCodeCrearSubZona != null && statusCodeCrearSubZona != undefined){
    
                  if (statusCodeCrearSubZona === 200 || statusCodeCrearSubZona === 201) {
                    setOperacionExitosa(true);
                    setMessageLoadingRespuesta(messageCrearSubZona);

                    // Se actualizan las filas de la tabla con la nueva subzona agregada
                    setRows(prevRows => agregarSubZona(prevRows, idZona, dataSubZonaAgregada));
                  }
                  else if (statusCodeCrearSubZona === 500) {
                      //En caso de error 500, se muestra un mensaje de error genérico, en vez del mensaje de error del backend
                      setOperacionExitosa(false);
                      setMessageLoadingRespuesta("Error desconocido, por favor intente nuevamente más tarde.");
                  }
                  else{
                      //En caso de cualquier otro error, se muestra el mensaje de error del backend
                      setOperacionExitosa(false);
                      setMessageLoadingRespuesta(messageCrearSubZona);
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
        <Box id="ContainerModalCrearSubZona">
            <Box id="HeaderModalCrearSubZona">
                <Typography variant="h5" component="h5" gutterBottom>
                    {"Crear nueva subzona"}
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

            <Box id="CuerpoModalCrearSubZona">
                <TextFieldUno 
                    name="nombreSubZona" 
                    label="Nombre de la subzona" 
                    placeholder="Ej: SubZona A" 
                    onChange={formik.handleChange}
                    error={formik.touched.nombreSubZona && Boolean(formik.errors.nombreSubZona)}
                    helperText={formik.touched.nombreSubZona && formik.errors.nombreSubZona}
                />
            </Box>

            <Box id="BoxButtonSubmitModalCrearSubZona">
                <ButtonTypeOne
                    defaultText="Crear subzona"
                    loadingText="Creando subzona..."
                    handleClick={formik.handleSubmit}
                    disabled={formik.isSubmitting}
                />
            </Box>

            {ConfirmDialogComponent}

            <ModalLoadingMasRespuesta
                open={openLoadingRespuesta}
                loading={loading}
                message={messageLoadingRespuesta}
                loadingMessage="Creando subzona..."
                successfulProcess={operacionExitosa}
                accionPostCierre={accionPostCierreLoadingRespuesta}
            />

        </Box>
    </Modal>
  )
}

export default ModalCrearSubZona;