import { Box, Fade, IconButton, Modal, Typography } from "@mui/material";
import { useFormik } from "formik";
import { useSelector } from "react-redux";
import { useConfirmDialog } from "../../../hooks/useConfirmDialog/useConfirmDialog";
import { useEffect, useState } from "react";
import CloseIcon from '@mui/icons-material/Close';
import ButtonTypeOne from "../../Buttons/ButtonTypeOne/ButtonTypeOne";
import "./ModalReservarEspacioComun.css";
import TextFieldUno from "../../TextField/TextFieldUno/TextFieldUno";
import SelectMui from "../../Select/SelectMui/SelectMui";
import RadioGroupMui from "../../RadioGroupMui/RadioGroupMui";
import TextFieldDate from "../../TextField/TextFieldDate/TextFieldDate";
import { cantidadHorasMaximasReserva, cantidadHorasMinimasReserva, idReservacionTipoReserva, idReservacionTipoUso, opcionesReservacionEspacioComun } from "../../../utils/constantes";
import { generarRango } from "../../../utils/funciones";
import ValidationReservarEspacioComun from "./ValidationReservarEspacioComun";
import ProgressStepperMui from "../../StepperMui/ProgressStepperMui/ProgressStepperMui";

const ModalReservarEspacioComun = ({ open, onClose, setEspaciosComunes, espaciosComunes }) => {

    // Con esta función se evita que el modal se cierre al presionar fuera de él
    const handleClose = (event, reason) => {
        if (reason !== 'backdropClick') {
            onClose();
        }
    };

    const { idEstablishment } = useSelector((state) => state.user);

    // const {loading, crearZona} = useZonas();

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

    // Estados y funciones para manejar los pasos llevados en el formulario
    const [pasoActualFormulario, setPasoActualFormulario] = useState(0);
    const cantidadPasosFormulario = 5;
    const handleNextClick = () => {setPasoActualFormulario(prev => prev + 1)}
    const handleBackClick = () => {setPasoActualFormulario(prev => prev - 1)}
    const opcionesReserva = opcionesReservacionEspacioComun;
    console.log("🚀 - opcionesReserva:", opcionesReserva);

    const opcionesRadioFechaReserva =       
    [
        {
            id: 1,
            opcion: "Ahora",     
        },
        {
            id: 2,
            opcion: "Seleccionar fecha"
        }
    ];

    const opcionesRadioHorasReserva = 
    [
        {
            id: 1,
            opcion: "No indicar"
        },
        {
            id: 2,
            opcion: "Seleccionar horas"
        }
    ]

    const opcionesRadioIncluyeInvitados = 
    [
        {
            id: 1,
            opcion: "Sí"
        },
        {
            id: 2,
            opcion: "No"
        }
    ]

    const formik = useFormik({
        initialValues: {
            idTipoReservacion: '',

            nombres: '',
            apellidos: '',
            numeroIdentificacion: '',
            idEspacioComunSeleccionado: '',

            idOpcionRadioFechaReserva: '',
            fechaReserva: '',
            horaReserva: '',
            minutosHoraReserva: '',

            idOpcionRadioHorasReserva: '',
            cantidadHorasReserva: '',
            cantidadMinutosReserva: '',

            idOpcionRadioIncluyeInvitados: '',
            cantidadInvitados: ''
        },
        validationSchema: ValidationReservarEspacioComun,
        onSubmit: async (values) => {
            console.log("es submit del formik");
            const confirmed = await confirm({
                title: "¿Reservar espacio común?",
                message: "¿Deseas reservar este espacio común para la persona ingresada?"
            });

            // if (confirmed) {
            //     setOpenLoadingRespuesta(true);

            //     // Se envía la información al backend para crear una nueva zona
            //     const {statusCode: statusCodeCrearZona, data: dataZonaAgregada, message: messageCrearZona} = await crearZona(idEstablishment, values.nombreZona);

            //     // Si el servidor responde con el Response dto que tiene configurado
            //     if(statusCodeCrearZona != null && statusCodeCrearZona != undefined){
    
            //       if (statusCodeCrearZona === 200 || statusCodeCrearZona === 201) {
            //         setOperacionExitosa(true);
            //         setMessageLoadingRespuesta(messageCrearZona);
            //         setRows(prevRows => [...prevRows, dataZonaAgregada]); // Agrega la nueva zona a las filas de la tabla
            //         formik.resetForm(); // Resetea el formulario después de crear la zona
            //       }
            //       else if (statusCodeCrearZona === 500) {
            //           //En caso de error 500, se muestra un mensaje de error genérico, en vez del mensaje de error del backend
            //           setOperacionExitosa(false);
            //           setMessageLoadingRespuesta("Error desconocido, por favor intente nuevamente más tarde.");
            //       }
            //       else{
            //           //En caso de cualquier otro error, se muestra el mensaje de error del backend
            //           setOperacionExitosa(false);
            //           setMessageLoadingRespuesta(messageCrearZona);
            //       }
            //     }
            //     else{
            //       //Esto es para los casos que el servidor no responda el ResponseDto tipico
            //       setOperacionExitosa(false);
            //       setMessageLoadingRespuesta("Error desconocido, por favor intente nuevamente más tarde.");
            //     }
            // } 
        }
    });
    // useEffect(() => {console.log("📌 - formik values => ",formik.values)}, [formik.values]);
    // useEffect(() => {console.log("📌 - formik errors => ",formik.errors)}, [formik.errors]);

    //Cada vez que se abra el modal se reseteará el formulario
    useEffect(() => {
      if(open){
        formik.resetForm();
        setPasoActualFormulario(0);
      }
    }, [open])
    

  return (
    <Modal open={open} onClose={handleClose}>
        <Box id="ContainerModalReservarEspacioComun">
            <Box id="HeaderModalReservarEspacioComun">
                <Typography variant="h5" component="h5" gutterBottom>
                    {"Reservar/Usar espacio común"}
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

            <Box id="CuerpoModalReservarEspacioComun">
                {pasoActualFormulario === 0 &&
                    <Fade in={pasoActualFormulario === 0} timeout={300}>
                        <Box id="ItemCuerpoModalReservarEspacioComun">
                            <SelectMui
                                label = "Seleccione el espacio común a reservar"
                                name="idEspacioComunSeleccionado"
                                width={"100%"}
                                listadoElementos={espaciosComunes || []}
                                keyListadoElementos={"id"}
                                mostrarElemento={(option)=> option["name"]}
                                handleChange = {formik.handleChange}
                                elementoSeleccionado = {formik.values.idEspacioComunSeleccionado}
                                atributoValue={"id"}
                                helperText={formik.touched.idEspacioComunSeleccionado && formik.errors.idEspacioComunSeleccionado}
                                error={formik.touched.idEspacioComunSeleccionado && Boolean(formik.errors.idEspacioComunSeleccionado)}
                            />

                            <SelectMui
                                label = "Seleccione el tipo de reservación"
                                name="idTipoReservacion"
                                width={"100%"}
                                listadoElementos={opcionesReserva || []}
                                keyListadoElementos={"id"}
                                mostrarElemento={(option)=> option["name"]}
                                handleChange = {formik.handleChange}
                                elementoSeleccionado = {formik.values.idTipoReservacion}
                                atributoValue={"id"}
                                helperText={
                                    formik.touched.idTipoReservacion && formik.errors.idTipoReservacion
                                        ? 
                                            formik.errors.idTipoReservacion
                                        : 
                                            formik.values.idTipoReservacion === idReservacionTipoUso ?

                                                "Este tipo de reservación permite que el espacio común sea utilizado por varias personas al mismo tiempo. El usuario indica que estará usando el espacio, pero otros también pueden acceder durante ese horario. Ejemplo: gimnasio, piscina, etc."
                                            :

                                            formik.values.idTipoReservacion === idReservacionTipoReserva ? 

                                                "Este tipo de reservación es exclusiva. Solo el usuario que realiza la reserva e invitados autorizados pueden usar el espacio durante ese horario; no se permite que otros lo reserven al mismo tiempo. Ej: Centro de eventos."
                                            :
                                                null
                                }

                                error={formik.touched.idTipoReservacion && Boolean(formik.errors.idTipoReservacion)}
                            />
                        </Box>
                    </Fade>
                }

                {pasoActualFormulario === 1 &&
                    <Fade in={pasoActualFormulario === 1} timeout={300}>
                        <Box id="ItemCuerpoModalReservarEspacioComun">
                            <Box id="DosItemsModalReservarEspacioComun">
                                <TextFieldUno 
                                    name="nombres" 
                                    label="Nombres" 
                                    placeholder="" 
                                    value={formik.values.nombres}
                                    onChange={formik.handleChange}
                                    error={formik.touched.nombres && Boolean(formik.errors.nombres)}
                                    helperText={formik.touched.nombres && formik.errors.nombres}
                                />

                                <TextFieldUno 
                                    name="apellidos" 
                                    label="Apellidos" 
                                    placeholder="" 
                                    value={formik.values.apellidos}
                                    onChange={formik.handleChange}
                                    error={formik.touched.apellidos && Boolean(formik.errors.apellidos)}
                                    helperText={formik.touched.apellidos && formik.errors.apellidos}
                                />
                            </Box>

                            <TextFieldUno 
                                name="numeroIdentificacion" 
                                label="Rut/Pasaporte" 
                                placeholder="Ej: 12345678-9" 
                                value={formik.values.numeroIdentificacion}
                                onChange={formik.handleChange}
                                error={formik.touched.numeroIdentificacion && Boolean(formik.errors.numeroIdentificacion)}
                                helperText={formik.touched.numeroIdentificacion && formik.errors.numeroIdentificacion}
                            />
                        </Box>
                    </Fade>
                }

                {pasoActualFormulario === 2 &&
                    <Fade in={pasoActualFormulario === 2} timeout={300}>
                        <Box id="ItemCuerpoModalReservarEspacioComun">
                            <Box id="ItemCuerpoModalReservarEspacioComun">
                                <RadioGroupMui
                                    label="¿Cuándo desea reservar el espacio?"
                                    name="idOpcionRadioFechaReserva"
                                    listadoElementos={opcionesRadioFechaReserva}
                                    keyListadoElementos="id"
                                    atributoValue="id"
                                    mostrarElemento={(option) => option.opcion}
                                    handleChange={(e) => {
                                        formik.setFieldValue('idOpcionRadioFechaReserva', parseInt(e.target.value));
                                    }}
                                    elementoSeleccionado={formik.values.idOpcionRadioFechaReserva}
                                    helperText={formik.touched.idOpcionRadioFechaReserva && formik.errors.idOpcionRadioFechaReserva}
                                    error={formik.touched.idOpcionRadioFechaReserva && Boolean(formik.errors.idOpcionRadioFechaReserva)}
                                    row={true} // Para mostrar los radios en horizontal
                                />

                                {formik.values.idOpcionRadioFechaReserva === 2 &&
                                    <Box id="ItemCuerpoModalReservarEspacioComun">
                                        <TextFieldDate
                                            name="fechaReserva"
                                            label="Fecha de la reserva"
                                            onChange={formik.handleChange}
                                            error={formik.touched.fechaReserva && Boolean(formik.errors.fechaReserva)}
                                            helperText={formik.touched.fechaReserva && formik.errors.fechaReserva}
                                        />

                                        <Typography color="#00000099">Hora de reserva</Typography>

                                        <Box id="DosItemsModalReservarEspacioComun">
                                            <SelectMui
                                                label = "Hora"
                                                name="horaReserva"
                                                width={"100%"}
                                                listadoElementos={generarRango(0,23)}
                                                keyListadoElementos={"id"}
                                                mostrarElemento={(option)=> `${option["valor"]}`}
                                                handleChange = {formik.handleChange}
                                                elementoSeleccionado = {formik.values.horaReserva}
                                                atributoValue={"valor"}
                                                helperText={(formik.touched.horaReserva && formik.errors.horaReserva)}
                                                error={formik.touched.horaReserva && Boolean(formik.errors.horaReserva)}
                                            />

                                            <SelectMui
                                                label = "Minutos"
                                                name="minutosHoraReserva"
                                                width={"100%"}
                                                listadoElementos={generarRango(0, 59)}
                                                keyListadoElementos={"id"}
                                                mostrarElemento={(option)=> `${option["valor"]}`}
                                                handleChange = {formik.handleChange}
                                                elementoSeleccionado = {formik.values.minutosHoraReserva}
                                                atributoValue={"valor"}
                                                helperText={formik.touched.minutosHoraReserva && formik.errors.minutosHoraReserva}
                                                error={formik.touched.minutosHoraReserva && Boolean(formik.errors.minutosHoraReserva)}
                                            />
                                        </Box>
                                    </Box>
                                }
                            </Box>
                        </Box>
                    </Fade>
                }

                {pasoActualFormulario === 3 &&
                    <Fade in={pasoActualFormulario === 3} timeout={300}>
                        <Box id="ItemCuerpoModalReservarEspacioComun">
                            <RadioGroupMui
                                label="Duración de la reserva"
                                name="idOpcionRadioHorasReserva"
                                listadoElementos={opcionesRadioHorasReserva}
                                keyListadoElementos="id"
                                atributoValue="id"
                                mostrarElemento={(option) => option.opcion}
                                handleChange={(e) => {
                                    formik.setFieldValue('idOpcionRadioHorasReserva', parseInt(e.target.value));
                                }}
                                elementoSeleccionado={formik.values.idOpcionRadioHorasReserva}
                                helperText={formik.touched.idOpcionRadioHorasReserva && formik.errors.idOpcionRadioHorasReserva}
                                error={formik.touched.idOpcionRadioHorasReserva && Boolean(formik.errors.idOpcionRadioHorasReserva)}
                                row={true} // Mostrar en horizontal
                            />

                            {formik.values.idOpcionRadioHorasReserva === 2 && 

                                <Box id="DosItemsModalReservarEspacioComun">
                                    <SelectMui
                                        label = "Horas"
                                        name="cantidadHorasReserva"
                                        width={"100%"}
                                        listadoElementos={generarRango(cantidadHorasMinimasReserva, cantidadHorasMaximasReserva)}
                                        keyListadoElementos={"id"}
                                        mostrarElemento={(option)=> `${option["valor"]} hora(s)`}
                                        handleChange = {formik.handleChange}
                                        elementoSeleccionado = {formik.values.cantidadHorasReserva}
                                        atributoValue={"valor"}
                                        helperText={formik.touched.cantidadHorasReserva && formik.errors.cantidadHorasReserva}
                                        error={formik.touched.cantidadHorasReserva && Boolean(formik.errors.cantidadHorasReserva)}
                                    />

                                    <SelectMui
                                        label = "Minutos"
                                        name="cantidadMinutosReserva"
                                        width={"100%"}
                                        listadoElementos={generarRango(0, 59)}
                                        keyListadoElementos={"id"}
                                        mostrarElemento={(option)=> `${option["valor"]} minuto(s)`}
                                        handleChange = {formik.handleChange}
                                        elementoSeleccionado = {formik.values.cantidadMinutosReserva}
                                        atributoValue={"valor"}
                                        helperText={formik.touched.cantidadMinutosReserva && formik.errors.cantidadMinutosReserva}
                                        error={formik.touched.cantidadMinutosReserva && Boolean(formik.errors.cantidadMinutosReserva)}
                                    />
                                </Box>
                            }
                        </Box>
                    </Fade>
                }

                {pasoActualFormulario === 4 &&
                    <Fade in={pasoActualFormulario === 4} timeout={300}>
                        <Box id="ItemCuerpoModalReservarEspacioComun">
                            <RadioGroupMui
                                label="¿La reserva incluye invitados?"
                                name="idOpcionRadioIncluyeInvitados"
                                listadoElementos={opcionesRadioIncluyeInvitados}
                                keyListadoElementos="id"
                                atributoValue="id"
                                mostrarElemento={(option) => option.opcion}
                                handleChange={(e) => {
                                    formik.setFieldValue('idOpcionRadioIncluyeInvitados', parseInt(e.target.value));
                                }}
                                elementoSeleccionado={formik.values.idOpcionRadioIncluyeInvitados}
                                helperText={formik.touched.idOpcionRadioIncluyeInvitados && formik.errors.idOpcionRadioIncluyeInvitados}
                                error={formik.touched.idOpcionRadioIncluyeInvitados && Boolean(formik.errors.idOpcionRadioIncluyeInvitados)}
                                row={true} // Mostrar en horizontal
                            />

                            {formik.values.idOpcionRadioIncluyeInvitados === 1 && 
                                <TextFieldUno 
                                    name="cantidadInvitados" 
                                    label="Indique la cantidad de invitados" 
                                    placeholder="Ej: 2" 
                                    width="100%"
                                    value={formik.values.cantidadInvitados}
                                    onChange={formik.handleChange}
                                    error={formik.touched.cantidadInvitados && Boolean(formik.errors.cantidadInvitados)}
                                    helperText={formik.touched.cantidadInvitados && formik.errors.cantidadInvitados}
                                />
                            }
                        </Box>
                    </Fade>
                }
            </Box>

            <ProgressStepperMui
                activeStep={pasoActualFormulario}
                handleNext={handleNextClick}
                handleBack={handleBackClick}
                steps={cantidadPasosFormulario}
                width="95%"
            />

            {/* <Box id="BoxButtonSubmitModalReservarEspacioComun">
                <ButtonTypeOne
                    defaultText="Reservar"
                    loadingText="Reservando..."
                    handleClick={formik.handleSubmit}
                    disabled={formik.isSubmitting}
                />
            </Box> */}

            {/* {ConfirmDialogComponent} */}

            {/* <ModalLoadingMasRespuesta
                open={openLoadingRespuesta}
                loading={loading}
                message={messageLoadingRespuesta}
                loadingMessage="Creando zona..."
                successfulProcess={operacionExitosa}
                accionPostCierre={accionPostCierreLoadingRespuesta}
            /> */}
        </Box>
    </Modal>
  )
}

export default ModalReservarEspacioComun;