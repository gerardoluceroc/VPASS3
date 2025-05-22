import { Box, IconButton, Modal, Typography } from "@mui/material";
import "./ModalAgregarAListaNegra.css";
import CloseIcon from '@mui/icons-material/Close';
import { useSelector } from "react-redux";
import useListaNegra from "../../../hooks/useListaNegra/useListaNegra";
import { useConfirmDialog } from "../../../hooks/useConfirmDialog/useConfirmDialog";
import { useState } from "react";
import { useFormik } from "formik";
import TextFieldUno from "../../TextField/TextFieldUno/TextFieldUno";
import ButtonTypeOne from "../../Buttons/ButtonTypeOne/ButtonTypeOne";
import { ValidationAgregarAListaNegra } from "./ValidationAgregarAListaNegra";
import ModalLoadingMasRespuesta from "../ModalLoadingMasRespuesta/ModalLoadingMasRespuesta";


const ModalAgregarAListaNegra = ({ open, onClose, setRows }) => {

    const { idEstablishment } = useSelector((state) => state.user);

    // Se obtienen las funciones y estados a utilizar del hook
    const {loading, crearListaNegra} = useListaNegra();

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
            nombres: '',
            apellidos: '',
            numeroIdentificacion: '',
            motivo: ''
        },
        validationSchema: ValidationAgregarAListaNegra,
        onSubmit: async (values) => {
            const confirmed = await confirm({
                title: "Agregar persona a lista negra",
                message: "¿Deseas agregar a esta persona a la lista negra del establecimiento?"
            });
        
            if (confirmed) {
                setOpenLoadingRespuesta(true);

                // Se envía la información al backend para agregar el visitante a la lista negra
                const {statusCode: statusCodeAgregarAListaNegra, data: dataVisitanteAgregadoAListaNegra, message: messageAgregarAListaNegra} = await crearListaNegra({
                    nombres: values.nombres,
                    apellidos: values.apellidos,
                    numeroIdentificacion: values.numeroIdentificacion,
                    motivo: values.motivo,
                    idEstablecimiento: idEstablishment
                });
    
                // Si el servidor responde con el Response dto que tiene configurado
                if(statusCodeAgregarAListaNegra != null && statusCodeAgregarAListaNegra != undefined){
    
                  if (statusCodeAgregarAListaNegra === 200 || statusCodeAgregarAListaNegra === 201) {
                    setOperacionExitosa(true);
                    setMessageLoadingRespuesta(messageAgregarAListaNegra);
                    setRows(prevRows => [...prevRows, dataVisitanteAgregadoAListaNegra]);
                  }
                  else if (statusCodeAgregarAListaNegra === 500) {
                      //En caso de error 500, se muestra un mensaje de error genérico, en vez del mensaje de error del backend
                      setOperacionExitosa(false);
                      setMessageLoadingRespuesta("Error desconocido, por favor intente nuevamente más tarde.");
                  }
                  else{
                      //En caso de cualquier otro error, se muestra el mensaje de error del backend
                      setOperacionExitosa(false);
                      setMessageLoadingRespuesta(messageAgregarAListaNegra);
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
        <Box id="ContainerModalAgregarAListaNegra">
            <Box id="HeaderModalCrearEstacionamiento">
                <Typography variant="h5" component="h5" gutterBottom>
                    {"Agregar a lista negra"}
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

            <Box id="CuerpoModalAgregarAListaNegra">
                <Box className ="DosItemsModalAgregarAListaNegra">
                    <TextFieldUno 
                    name="nombres" 
                    type="text" 
                    label="Nombres" 
                    placeholder="Ingrese los nombres de la persona" 
                    onChange={formik.handleChange}
                    error={formik.touched.nombres && Boolean(formik.errors.nombres)}
                    helperText={formik.touched.nombres && formik.errors.nombres}
                    />
                    <TextFieldUno 
                    name="apellidos" 
                    type="text" 
                    label="Apellidos" 
                    placeholder="Ingrese los apellidos de la persona" 
                    onChange={formik.handleChange}
                    error={formik.touched.apellidos && Boolean(formik.errors.apellidos)}
                    helperText={formik.touched.apellidos && formik.errors.apellidos}
                    />
                </Box>

                <TextFieldUno 
                    name="numeroIdentificacion" 
                    type="text" 
                    label="RUT/Pasaporte" 
                    placeholder="12345678-9" 
                    onChange={formik.handleChange}
                    error={formik.touched.numeroIdentificacion && Boolean(formik.errors.numeroIdentificacion)}
                    helperText={formik.touched.numeroIdentificacion && formik.errors.numeroIdentificacion}
                />

                <TextFieldUno 
                    name="motivo" 
                    type="text" 
                    label="Motivo (Opcional)" 
                    placeholder="" 
                    onChange={formik.handleChange}
                    error={formik.touched.rut && Boolean(formik.errors.rut)}
                    helperText={formik.touched.rut && formik.errors.rut}
                />
            </Box>

            <Box id="BoxButtonSubmitModalAgregarAListaNegra">
                <ButtonTypeOne
                    defaultText="Agregar a lista negra"
                    loadingText="Registrando en lista negra..."
                    handleClick={formik.handleSubmit}
                    // handleClick={simularPeticion}
                    disabled={formik.isSubmitting}
                />
            </Box>

            {ConfirmDialogComponent}

            <ModalLoadingMasRespuesta
                open={openLoadingRespuesta}
                loading={loading}
                message={messageLoadingRespuesta}
                loadingMessage="Registrando en lista negra..."
                successfulProcess={operacionExitosa}
                accionPostCierre={accionPostCierreLoadingRespuesta}
            />

        </Box>
    </Modal>
  )
}

export default ModalAgregarAListaNegra