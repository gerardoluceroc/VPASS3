import CloseIcon from '@mui/icons-material/Close';
import { Box, IconButton, Modal, Typography } from '@mui/material';
import TextFieldReadOnlyUno from '../../TextField/TextFieldReadOnlyUno/TextFieldReadOnlyUno';
import { useFormik } from 'formik';
import SwitchMui from '../../Switch/SwitchMui/SwitchMui';
import TextFieldUno from '../../TextField/TextFieldUno/TextFieldUno';
import "./ModalRetirarEncomienda.css";
import { useEffect, useState } from 'react';
import ButtonTypeOne from '../../Buttons/ButtonTypeOne/ButtonTypeOne';
import { useConfirmDialog } from '../../../hooks/useConfirmDialog/useConfirmDialog';
import ValidationRetirarEncomienda from './ValidationRetirarEncomienda';
import UseEncomienda from '../../../hooks/useEncomienda/useEncomienda';
import ModalLoadingMasRespuesta from '../ModalLoadingMasRespuesta/ModalLoadingMasRespuesta';

const ModalRetirarEncomienda = ({
    open,
    onClose,
    setRows,
    departamentoSeleccionado,
    encomiendaSeleccionada,
    setEncomiendaSeleccionada
}) => {
    const {retirarEncomienda, loading: loadingEncomiendas} = UseEncomienda();

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

    // Funcion para actualizar la tabla de encomiendas reemplazando la encomienda que fue actualizada y marcada como retirada
    const actualizarTablaDeEncomiendas = (setEncomiendas, encomiendaActualizada) => {
        setEncomiendas((prevEncomiendas) => {
            return prevEncomiendas.map((encomienda) =>
                encomienda.id === encomiendaActualizada.id ? encomiendaActualizada : encomienda
            );
        });
    };

    const formik = useFormik({
        initialValues: {
            nombrePersonaQueRetira: null,
            apellidoPersonaQueRetira: null,
            rutPersonaQueRetira: null,
            idEncomienda: null,

            retiraPropietario: false, // Este campo se puede usar para indicar si el propietario es el que retira la encomienda
        },
        validationSchema: ValidationRetirarEncomienda,
        onSubmit: async (values) => {
            const confirmed = await confirm({
                title: "¿Marcar encomienda como retirada?",
                message: "¿Deseas marcar esta encomienda como retirada?"
            });
        
            if (confirmed) {
                setOpenLoadingRespuesta(true);
                const nombrePersonaQueRetira = values.nombrePersonaQueRetira;
                const apellidoPersonaQueRetira = values.apellidoPersonaQueRetira;
                const rutPersonaQueRetira = values.rutPersonaQueRetira;
                const idEncomienda = values.idEncomienda;

                const respuestaRetirarEncomienda = await retirarEncomienda(
                    nombrePersonaQueRetira,
                    apellidoPersonaQueRetira,
                    rutPersonaQueRetira,
                    idEncomienda
                )
                
                const {statusCode: statusCodeRetirarEncomienda, data: dataEncomiendaRetirada, message: messageRetirarEncomienda} = respuestaRetirarEncomienda;

                    // Si el servidor responde con el Response dto que tiene configurado
                    if(statusCodeRetirarEncomienda != null && statusCodeRetirarEncomienda != undefined){
                        if (statusCodeRetirarEncomienda === 200 || statusCodeRetirarEncomienda === 201) {
                            setOperacionExitosa(true);
                            setMessageLoadingRespuesta(messageRetirarEncomienda);
                            actualizarTablaDeEncomiendas(setRows, dataEncomiendaRetirada);
                            // handleAgregarNuevaReservaExclusiva(dataEncomiendaRetirada);
                            // setRows(prevRows => [...prevRows, dataEncomiendaRetirada]); // Agrega la nueva reserva a las filas de la tabla de registros de reservas
                            // formik.resetForm(); // Resetea el formulario después de crear la reserva exclusiva
                            // onClose();
                        }
                        else{
                            setOperacionExitosa(false);
                            setMessageLoadingRespuesta(messageRetirarEncomienda);
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

    // Con esta función se evita que el modal se cierre al presionar fuera de él
    const handleClose = (event, reason) => {
        if (reason !== 'backdropClick') {
            setEncomiendaSeleccionada({}); // Esto permite reiniciar el estado, para que cuando se abra el modal se detecte el cambio en el estado y se aplique en este codigo
            onClose();
        }
    };

    //Cada vez que se abra el modal se reseteará el formulario
    useEffect(() => {
      if(open){
        formik.resetForm();
      }
    }, [open])

    useEffect(() => {
        // Si se cambia el valor de retiraPropietario, se debe resetear los campos de persona que retira, ya que si no se hace esto, al cambiar de valor de retiraPropietario, los campos de persona que retira se quedan con los valores anteriores.
        // Por lo tanto, se debe resetear los campos de persona que retira a null
        if (!formik.values.retiraPropietario) {
            formik.setFieldValue("nombrePersonaQueRetira", null);
            formik.setFieldValue("apellidoPersonaQueRetira", null);
            formik.setFieldValue("rutPersonaQueRetira", null);
        }
        else {
            // Si retiraPropietario es true, se setea los campos de persona que retira con los datos del propietario activo del departamento seleccionado
            if (departamentoSeleccionado !== null && departamentoSeleccionado !== undefined) { 
                formik.setFieldValue("nombrePersonaQueRetira", departamentoSeleccionado?.activeOwnership?.person?.names || "");
                formik.setFieldValue("apellidoPersonaQueRetira", departamentoSeleccionado?.activeOwnership?.person?.lastNames || "");
                formik.setFieldValue("rutPersonaQueRetira", departamentoSeleccionado?.activeOwnership?.person?.identificationNumber || "");
            }
        }
    }, [formik.values.retiraPropietario, departamentoSeleccionado])

    useEffect(() => {
        formik.setFieldValue("idEncomienda", encomiendaSeleccionada.id ?? null);
    }, [encomiendaSeleccionada]);
    
  return (
    <Modal open={open} onClose={handleClose}>
        <Box id="ContainerModalRetirarEncomienda">
            <Box id="HeaderModalRetirarEncomienda">
                <Typography variant="h5" component="h5" gutterBottom>
                    {"Marcar encomienda como retirada"}
                </Typography>

                <IconButton
                    aria-label="close"
                    onClick={handleClose}
                    sx={{
                    top: -8,
                    color: "black",
                    }}
                >
                    <CloseIcon sx={{fontSize: "30px"}} />
                </IconButton>
            </Box>
            <Box id="CuerpoModalRetirarEncomienda">
                <Box id="ItemCuerpoModalRetirarEncomienda">
                    <Typography variant="h6" component="h6" gutterBottom>
                        {"Información de la persona que retira la encomienda"}
                    </Typography>

                    <Box>
                        <SwitchMui
                            name="retiraPropietario"
                            primaryLabel="¿Es el inquilino quien retiró la encomienda? No / Sí"
                            secondaryLabel=""
                            handleChange={formik.handleChange}
                            checked={formik.values.retiraPropietario}
                            helperText=""
                        />
                    </Box>

                    {
                        formik.values.retiraPropietario === true ? 
                            <>
                                <Box id="DosItemsCuerpoModalRetirarEncomienda">
                                    <TextFieldReadOnlyUno
                                        label={"Nombres"}
                                        value={ (departamentoSeleccionado !== null && departamentoSeleccionado !== undefined) ? `${departamentoSeleccionado?.activeOwnership?.person?.names}` : "Sin datos"}
                                    /> 
                                    <TextFieldReadOnlyUno
                                        label={"Apellidos"}
                                        value={(departamentoSeleccionado !== null && departamentoSeleccionado !== undefined) ? `${departamentoSeleccionado?.activeOwnership?.person?.lastNames}` : "Sin datos"}
                                    /> 
                                </Box>
                                <TextFieldReadOnlyUno
                                    label={"Rut/Pasaporte"}
                                    value={(departamentoSeleccionado !== null && departamentoSeleccionado !== undefined) ? `${departamentoSeleccionado?.activeOwnership?.person?.identificationNumber}` : "Sin datos"}
                                />
                            </> 
                            : 
                            <>
                                <Box id="DosItemsCuerpoModalRetirarEncomienda">
                                    <TextFieldUno
                                        name="nombrePersonaQueRetira" 
                                        label="Nombre de la persona que retira" 
                                        placeholder="Ej: Juan Antonio" 
                                        value={formik.values.nombrePersonaQueRetira ?? ""}
                                        onChange={formik.handleChange}
                                        error={formik.touched.nombrePersonaQueRetira && Boolean(formik.errors.nombrePersonaQueRetira)}
                                        helperText={formik.touched.nombrePersonaQueRetira && formik.errors.nombrePersonaQueRetira}
                                    />

                                    <TextFieldUno 
                                        name="apellidoPersonaQueRetira" 
                                        label="Apellido de la persona que retira" 
                                        placeholder="Ej: Pérez González" 
                                        value={formik.values.apellidoPersonaQueRetira ?? ""}
                                        onChange={formik.handleChange}
                                        error={formik.touched.apellidoPersonaQueRetira && Boolean(formik.errors.apellidoPersonaQueRetira)}
                                        helperText={formik.touched.apellidoPersonaQueRetira && formik.errors.apellidoPersonaQueRetira}
                                    />
                                </Box>

                                <TextFieldUno 
                                    name="rutPersonaQueRetira" 
                                    label="RUT de la persona que retira (sin puntos con guión)" 
                                    placeholder="Ej: 12345678-9" 
                                    value={formik.values.rutPersonaQueRetira ?? ""}
                                    onChange={formik.handleChange}
                                    error={formik.touched.rutPersonaQueRetira && Boolean(formik.errors.rutPersonaQueRetira)}
                                    helperText={formik.touched.rutPersonaQueRetira && formik.errors.rutPersonaQueRetira}
                                />
                            </>
                    }
                    <Box id="BoxButtonSubmitModalRetirarEncomienda">
                        <ButtonTypeOne
                            defaultText="Confirmar"
                            loadingText="Registrando retiro..."
                            handleClick={formik.handleSubmit}
                            disabled={formik.isSubmitting}
                        />
                    </Box>

                    {ConfirmDialogComponent}
                    <ModalLoadingMasRespuesta
                        open={openLoadingRespuesta}
                        loading={loadingEncomiendas}
                        message={messageLoadingRespuesta}
                        loadingMessage="Actualizando información de encomienda..."
                        successfulProcess={operacionExitosa}
                        accionPostCierre={accionPostCierreLoadingRespuesta}
                    />
                </Box>
            </Box>
        </Box>
    </Modal>
  )
}

export default ModalRetirarEncomienda;
