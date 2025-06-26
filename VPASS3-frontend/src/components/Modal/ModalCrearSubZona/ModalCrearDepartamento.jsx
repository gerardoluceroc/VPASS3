import { useFormik } from "formik";
import { useConfirmDialog } from "../../../hooks/useConfirmDialog/useConfirmDialog";
import ModalLoadingMasRespuesta from "../ModalLoadingMasRespuesta/ModalLoadingMasRespuesta";
import ButtonTypeOne from "../../Buttons/ButtonTypeOne/ButtonTypeOne";
import { Box, IconButton, Modal, Typography } from "@mui/material";
import TextFieldUno from "../../TextField/TextFieldUno/TextFieldUno";
import CloseIcon from '@mui/icons-material/Close';
import { useState } from "react";
import "./ModalCrearDepartamento.css";
import useDepartamento from "../../../hooks/useDepartamento/useDepartamento";
import ValidationCrearDepartamento from "./ValidationCrearDepartamento";
import { agregarDepartamento } from "../../PagesComponents/GestionZonasPageComponent/funcionesGestionZonasPageComponent";

const ModalCrearDepartamento = ({ open, onClose, setRows, idZona }) => {

    // const {loading, crearSubZona} = useSubZona();
    const { loading, crearDepartamento } = useDepartamento();

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
            nombreDepartamento: ''
        },
        validationSchema: ValidationCrearDepartamento,
        onSubmit: async (values) => {
            const confirmed = await confirm({
                title: "¿Crear departamento?",
                message: "¿Deseas crear un nuevo departamento en el establecimiento?"
            });
        
            if (confirmed) {
                setOpenLoadingRespuesta(true);

                // Se envía la información al backend para crear un nuevo departamento
                const {statusCode: statusCodeCrearDepartamento, data: dataDepartamentoAgregado, message: messageCrearDepartamento} = await crearDepartamento(idZona, values.nombreDepartamento);

                // Si el servidor responde con el Response dto que tiene configurado
                if(statusCodeCrearDepartamento != null && statusCodeCrearDepartamento != undefined){
    
                  if (statusCodeCrearDepartamento === 200 || statusCodeCrearDepartamento === 201) {
                    setOperacionExitosa(true);
                    setMessageLoadingRespuesta(messageCrearDepartamento);
                    formik.resetForm(); // Resetea el formulario después de crear el departamento

                    // Se actualizan las filas de la tabla con el nuevo departamento agregado
                    setRows(prevRows => agregarDepartamento(prevRows, idZona, dataDepartamentoAgregado));
                  }
                  else if (statusCodeCrearDepartamento === 500) {
                      //En caso de error 500, se muestra un mensaje de error genérico, en vez del mensaje de error del backend
                      setOperacionExitosa(false);
                      setMessageLoadingRespuesta("Error desconocido, por favor intente nuevamente más tarde.");
                  }
                  else{
                      //En caso de cualquier otro error, se muestra el mensaje de error del backend
                      setOperacionExitosa(false);
                      setMessageLoadingRespuesta(messageCrearDepartamento);
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
        <Box id="ContainerModalCrearDepartamento">
            <Box id="HeaderModalCrearDepartamento">
                <Typography variant="h5" component="h5" gutterBottom>
                    {"Crear nuevo departamento"}
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

            <Box id="CuerpoModalCrearDepartamento">
                <TextFieldUno 
                    name="nombreDepartamento" 
                    value={formik.values.nombreDepartamento}
                    label="Nombre del departamento" 
                    placeholder="Ej: Departamento 201" 
                    onChange={formik.handleChange}
                    error={formik.touched.nombreDepartamento && Boolean(formik.errors.nombreDepartamento)}
                    helperText={formik.touched.nombreDepartamento && formik.errors.nombreDepartamento}
                />
            </Box>

            <Box id="BoxButtonSubmitModalCrearDepartamento">
                <ButtonTypeOne
                    defaultText="Crear departamento"
                    loadingText="Creando departamento..."
                    handleClick={formik.handleSubmit}
                    disabled={formik.isSubmitting}
                />
            </Box>

            {ConfirmDialogComponent}

            <ModalLoadingMasRespuesta
                open={openLoadingRespuesta}
                loading={loading}
                message={messageLoadingRespuesta}
                loadingMessage="Creando departamento..."
                successfulProcess={operacionExitosa}
                accionPostCierre={accionPostCierreLoadingRespuesta}
            />
        </Box>
    </Modal>
  )
}

export default ModalCrearDepartamento;