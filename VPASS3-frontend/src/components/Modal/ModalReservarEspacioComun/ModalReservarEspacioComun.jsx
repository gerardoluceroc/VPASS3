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
import { cambiarFormatoHoraFecha, formatoLegibleDesdeHoraString, generarRango, transformarAFormatoDateTime, transformarAFormatoTimeSpan } from "../../../utils/funciones";
import ValidationReservarEspacioComun from "./ValidationReservarEspacioComun";
import ProgressStepperMui from "../../StepperMui/ProgressStepperMui/ProgressStepperMui";
import TextFieldReadOnlyUno from "../../TextField/TextFieldReadOnlyUno/TextFieldReadOnlyUno";
import useReservarEspacioComun from "../../../hooks/useReservarEspacioComun/useReservarEspacioComun";
import ModalLoadingMasRespuesta from "../ModalLoadingMasRespuesta/ModalLoadingMasRespuesta";

const ModalReservarEspacioComun = ({ open, onClose, setEspaciosComunes, espaciosComunes }) => {

    useEffect(() => {console.log("📌 - [ModalReservarEspacioComun.jsx] - Line [25] - espaciosComunes => ", espaciosComunes)}, [espaciosComunes]);

    const {crearReservaExclusivaEspacioComun, crearReservaUsoCompartido, loading} = useReservarEspacioComun();

    // Con esta función se evita que el modal se cierre al presionar fuera de él
    const handleClose = (event, reason) => {
        if (reason !== 'backdropClick') {
            onClose();
        }
    };

    const { idEstablishment } = useSelector((state) => state.user);

    // Se invoca la función para consultarle al usuario si desea enviar el formulario
    const { confirm, ConfirmDialogComponent } = useConfirmDialog();

    // Estados y funciones para manejar el componente ModalLoadingMasRespuesta
    const [openLoadingRespuesta, setOpenLoadingRespuesta] = useState(false);
    const [messageLoadingRespuesta, setMessageLoadingRespuesta] = useState('');
    const [operacionExitosa, setOperacionExitosa] = useState(false);
    const accionPostCierreLoadingRespuesta = () => {
        setOpenLoadingRespuesta(false);
        setMessageLoadingRespuesta('');
        onClose();
    }

    // Estados y funciones para manejar los pasos llevados en el formulario
    const [pasoActualFormulario, setPasoActualFormulario] = useState(0);
    const cantidadPasosFormulario = 6;
    // const handleNextClick = () => {setPasoActualFormulario(prev => prev + 1)}

    // Se define una función para avanzar al siguiente paso del formulario.
    // Esta función incluirá la lógica de validación para el paso actual.
    const handleNextClick = async () => {
        // Se inicializa un arreglo para almacenar los nombres de los campos que deben ser validados en el paso actual.
        let fieldsToValidate = [];

        // Se usa un switch para determinar qué campos corresponden al 'pasoActualFormulario'.
        // Esto asegura que solo los campos relevantes para la vista actual sean considerados para la validación.
        switch (pasoActualFormulario) {
            case 0:
                fieldsToValidate = ['idEspacioComunSeleccionado', 'idTipoReservacion'];
                break;
            case 1:
                fieldsToValidate = ['nombres', 'apellidos', 'numeroIdentificacion'];
                break;
            case 2:
                // Los campos de fecha y hora son condicionales, solo se validan si el usuario eligió "Seleccionar fecha".
                fieldsToValidate = ['idOpcionRadioFechaReserva']; // Este siempre se valida en este paso
                if (formik.values.idOpcionRadioFechaReserva === 2) {
                    fieldsToValidate.push('fechaReserva', 'horaReserva', 'minutosHoraReserva');
                }
                break;
            case 3:
                // Los campos de duración son condicionales, solo se validan si el usuario eligió "Seleccionar horas".
                fieldsToValidate = ['idOpcionRadioHorasReserva']; // Este siempre se valida en este paso
                if (formik.values.idOpcionRadioHorasReserva === 2) {
                    fieldsToValidate.push('cantidadHorasReserva', 'cantidadMinutosReserva');
                }
                break;
            case 4:
                // El campo de cantidad de invitados es condicional, solo se valida si el usuario eligió "Sí".
                fieldsToValidate = ['idOpcionRadioIncluyeInvitados']; // Este siempre se valida en este paso
                if (formik.values.idOpcionRadioIncluyeInvitados === 1) {
                    fieldsToValidate.push('cantidadInvitados');
                }
                break;
            default:
                break;
        }

        // Se crea un objeto para marcar los campos del paso actual como 'touched' (visitados).
        // Esto es crucial para que Formik muestre visualmente los mensajes de error y los estados de error
        // (como bordes rojos) en los componentes de Material-UI. Si un campo no está 'touched', Formik
        // no mostrará sus errores aunque existan.
        const touchedFields = {};
        fieldsToValidate.forEach(field => {
            touchedFields[field] = true;
        });
        // Se actualiza el estado 'touched' de Formik. El segundo argumento 'false' evita que Formik
        // ejecute una validación automática inmediatamente después de setear los 'touched',
        // ya que la validación se ejecutará de forma explícita a continuación.
        formik.setTouched(touchedFields, false);

        // Se ejecuta la validación de todo el formulario. `validateForm()` devuelve un objeto con todos los errores.
        // Aunque valida todo, solo nos interesan los errores de los campos del paso actual.
        const errors = await formik.validateForm();

        // Se verifica si hay errores en los campos que pertenecen al paso actual.
        // `some()` devuelve true si al menos uno de los campos en 'fieldsToValidate' tiene un error.
        const currentStepErrors = fieldsToValidate.some(field => errors[field]);

        // Si no hay errores en los campos del paso actual, se permite al usuario avanzar al siguiente paso.
        if (!currentStepErrors) {
            setPasoActualFormulario(prev => prev + 1);
        }
    };
    
    // Funcion para retroceder en el formulario con el boton "volver"
    const handleBackClick = () => {setPasoActualFormulario(prev => prev - 1)}

    //Arreglo con las opciones de reserva, se encuentra en la carpeta utils en el archivo constantes.js
    const opcionesReserva = opcionesReservacionEspacioComun;

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
            fechaReserva: null,
            horaReserva: null,
            minutosHoraReserva: null,
            fechaFinalReserva: null, // Este será la mezcla de lo que hay en fechaReserva, horaReserva y minutosHoraReserva todo junto y en formato DateTime

            idOpcionRadioHorasReserva: '',
            cantidadHorasReserva: null,
            cantidadMinutosReserva: null,
            cantidadTiempoReserva: null, // Este será la mezcla de lo que hay en cantidadHorasReserva y cantidadMinutos reserva en formato timespan

            idOpcionRadioIncluyeInvitados: '',
            cantidadInvitados: null
        },
        validationSchema: ValidationReservarEspacioComun,
        onSubmit: async (values) => {
            const confirmed = await confirm({
                title: "¿Reservar espacio común?",
                message: "¿Deseas reservar este espacio común para la persona ingresada?"
            });

            if(confirmed){
                const idAreaComun = values.idEspacioComunSeleccionado;
                const fechaReserva = values.fechaFinalReserva;
                const duracionReserva = values.cantidadTiempoReserva;
                const cantidadInvitados = values.cantidadInvitados;
                const nombresPersonaReservada = values.nombres;
                const apellidosPersonaReservada = values.apellidos;
                const rutPersonaReserva = values.numeroIdentificacion;

                setOpenLoadingRespuesta(true);

                if(values.idTipoReservacion === idReservacionTipoReserva){
                    const respuestaCrearReservaExclusiva = await crearReservaExclusivaEspacioComun(nombresPersonaReservada, apellidosPersonaReservada, rutPersonaReserva, idAreaComun, fechaReserva, duracionReserva, cantidadInvitados);

                    const {statusCode: statusCodeCrearReservaExclusiva, data: dataCrearReservaExclusiva, message: messageCrearReservaExclusiva} = respuestaCrearReservaExclusiva;

                    // Si el servidor responde con el Response dto que tiene configurado
                    if(statusCodeCrearReservaExclusiva != null && statusCodeCrearReservaExclusiva != undefined){
        
                    if (statusCodeCrearReservaExclusiva === 200 || statusCodeCrearReservaExclusiva === 201) {
                        setOperacionExitosa(true);
                        setMessageLoadingRespuesta(messageCrearReservaExclusiva);
                        // setRows(prevRows => [...prevRows, dataCrearReservaExclusiva]); // Agrega la nueva reserva a las filas de la tabla de registros de reservas
                        // formik.resetForm(); // Resetea el formulario después de crear la reserva exclusiva
                        // onClose();
                    }
                    else if (statusCodeCrearReservaExclusiva === 500) {
                        //En caso de error 500, se muestra un mensaje de error genérico, en vez del mensaje de error del backend
                        setOperacionExitosa(false);
                        setMessageLoadingRespuesta("Error desconocido, por favor intente nuevamente más tarde.");
                    }
                    else{
                        //En caso de cualquier otro error, se muestra el mensaje de error del backend
                        setOperacionExitosa(false);
                        setMessageLoadingRespuesta(messageCrearReservaExclusiva);
                    }
                    }
                    else{
                        //Esto es para los casos que el servidor no responda el ResponseDto tipico
                        setOperacionExitosa(false);
                        setMessageLoadingRespuesta("Error desconocido, por favor intente nuevamente más tarde.");
                    }
                }

                else if(values.idTipoReservacion === idReservacionTipoUso){
                    const respuestaCrearReservaUsoCompartido = await crearReservaUsoCompartido(nombresPersonaReservada, apellidosPersonaReservada, rutPersonaReserva, idAreaComun, fechaReserva, duracionReserva, cantidadInvitados);

                    const {statusCode: statusCodeCrearReservaUsoCompartido, data: dataCrearReservaUsoCompartido, message: messageCrearReservaUsoCompartido} = respuestaCrearReservaUsoCompartido;

                    // Si el servidor responde con el Response dto que tiene configurado
                    if(statusCodeCrearReservaUsoCompartido != null && statusCodeCrearReservaUsoCompartido != undefined){
        
                    if (statusCodeCrearReservaUsoCompartido === 200 || statusCodeCrearReservaUsoCompartido === 201) {
                        setOperacionExitosa(true);
                        setMessageLoadingRespuesta(messageCrearReservaUsoCompartido);
                    }
                    else if (statusCodeCrearReservaUsoCompartido === 500) {
                        //En caso de error 500, se muestra un mensaje de error genérico, en vez del mensaje de error del backend
                        setOperacionExitosa(false);
                        setMessageLoadingRespuesta("Error desconocido, por favor intente nuevamente más tarde.");
                    }
                    else{
                        //En caso de cualquier otro error, se muestra el mensaje de error del backend
                        setOperacionExitosa(false);
                        setMessageLoadingRespuesta(messageCrearReservaUsoCompartido);
                    }
                    }
                    else{
                        //Esto es para los casos que el servidor no responda el ResponseDto tipico
                        setOperacionExitosa(false);
                        setMessageLoadingRespuesta("Error desconocido, por favor intente nuevamente más tarde.");
                    }
                }
            }
        }
    });
    useEffect(() => {console.log("📌 - formik values => ",formik.values)}, [formik.values]);
    useEffect(() => {console.log("📌 - formik errors => ",formik.errors)}, [formik.errors]);

    //Cada vez que se abra el modal se reseteará el formulario y se muestra en el primer paso
    useEffect(() => {
      if(open){
        formik.resetForm();
        setPasoActualFormulario(0);
      }
    }, [open])

    // Si el usuario en la parte de la duracion de la reserva, selecciona el valor "No indicar"
    // Se debe resetear lo que habia puesto anteriormente en las horas seleccionadas
    useEffect(() => {
        if(formik.values.idOpcionRadioHorasReserva === 1){
            formik.setFieldValue("cantidadHorasReserva", null);
            formik.setFieldValue("cantidadMinutosReserva", null);
        }
    }, [formik.values.idOpcionRadioHorasReserva]);

    // Si se setean las horas y minutos de la reserva, se transoforma a formato timespan y se guarda en el atributo cantidadTiempoReserva del formik.
    useEffect(() => {
        if(formik.values.cantidadHorasReserva !== null && formik.values.cantidadMinutosReserva !== null){
            const formatoTimeSpan = transformarAFormatoTimeSpan(formik.values.cantidadHorasReserva, formik.values.cantidadMinutosReserva);
            formik.setFieldValue("cantidadTiempoReserva", formatoTimeSpan);
        }
      
    }, [formik.values.cantidadHorasReserva, formik.values.cantidadMinutosReserva]);

    // Si el usuario en la parte de la fecha de la reserva, selecciona el valor "Ahora"
    // Se debe resetear lo que habia puesto anteriormente en la informacion de la fecha
    useEffect(() => {
        // Si selecciona la opcion de "ahora", se resetean los otros valores del formik relacionados
        if(formik.values.idOpcionRadioFechaReserva === 1){
            formik.setFieldValue("fechaReserva", null);
            formik.setFieldValue("horaReserva", null);
            formik.setFieldValue("minutosHoraReserva", null);
            formik.setFieldValue("fechaFinalReserva", null);
        }
    }, [formik.values.idOpcionRadioFechaReserva]);

    // Si se setean la fecha, hora y minuto de la reserva, se transoforma a formato datetime y se guarda en el atributo fechaFinalReserva del formik.
    useEffect(() => {
        if(formik.values.fechaReserva !== null && formik.values.horaReserva !== null && formik.values.minutosHoraReserva !== null){
            const formatoFechaFinal = transformarAFormatoDateTime(formik.values.fechaReserva, formik.values.horaReserva, formik.values.minutosHoraReserva);
            formik.setFieldValue("fechaFinalReserva", formatoFechaFinal);
        }
    }, [formik.values.fechaReserva, formik.values.horaReserva, formik.values.minutosHoraReserva]);

    // Si el usuario en la parte de indicar los invitados, selecciona el valor "No"
    // Se debe resetear lo que habia puesto en la cantidad de invitados
    useEffect(() => {
        if(formik.values.idOpcionRadioIncluyeInvitados === 2){
            formik.setFieldValue("cantidadInvitados", null);
        }
    }, [formik.values.idOpcionRadioIncluyeInvitados]);
    
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

                {pasoActualFormulario === 5 &&
                    <Fade in={pasoActualFormulario === 5} timeout={300}>
                        <Box id="ItemCuerpoModalReservarEspacioComun">
                            <Box id="DosItemsModalReservarEspacioComun">
                                <TextFieldReadOnlyUno
                                    label={"Nombres"}
                                    value={`${formik.values.nombres}`}
                                />
                                <TextFieldReadOnlyUno
                                    label={"Apellidos"}
                                    value={`${formik.values.apellidos}`}
                                />
                                <TextFieldReadOnlyUno
                                    label={"Rut/Pasaporte"}
                                    value={`${formik.values.numeroIdentificacion}`}
                                />
                            </Box>
                            <Box id="DosItemsModalReservarEspacioComun">
                                <TextFieldReadOnlyUno
                                    label={"Espacio reservado"}
                                    value={`${espaciosComunes?.find(espacioComun => espacioComun.id === formik.values.idEspacioComunSeleccionado).name || "Sin datos"}`}
                                />
                                <TextFieldReadOnlyUno
                                    label="Tipo de reserva"
                                    value={
                                        opcionesReserva?.find(opcion => opcion.id === formik.values.idTipoReservacion)?.name 
                                        || "Sin datos"
                                    }
                                />

                            </Box>

                            <Box id="DosItemsModalReservarEspacioComun">
                                <TextFieldReadOnlyUno
                                    label={"Fecha de reserva"}
                                    value={`${formik.values.fechaFinalReserva === null ? "Ahora" : cambiarFormatoHoraFecha(formik.values.fechaFinalReserva)}`}
                                />

                                <TextFieldReadOnlyUno
                                    label={"Tiempo de reserva"}
                                    value={`${formik.values.cantidadTiempoReserva === null ? "No indicado" : formatoLegibleDesdeHoraString(formik.values.cantidadTiempoReserva)}`}
                                />
                            </Box>

                            <Box id="DosItemsModalReservarEspacioComun">
                                <TextFieldReadOnlyUno
                                    label={"¿Incluye invitados?"}
                                    value={`${formik.values.cantidadInvitados === null ? "No" : "Sí"}`}
                                />

                                {/* Si la reserva incluye invitados, se muestra la cantidad */}
                                {formik.values.idOpcionRadioIncluyeInvitados === 1 && formik.values.cantidadInvitados !== null ? 
                                    <TextFieldReadOnlyUno
                                        label={"Cantidad de invitados"}
                                        value={`${formik.values.cantidadInvitados}` || "No informado"}
                                    />

                                    :

                                    null
                                }
                            </Box>
                            <Box id="BoxButtonSubmitModalReservarEspacioComun">
                                <ButtonTypeOne
                                    defaultText="Confirmar reserva"
                                    loadingText="Reservando..."
                                    handleClick={formik.handleSubmit}
                                    disabled={formik.isSubmitting}
                                />
                            </Box>
                            {ConfirmDialogComponent}
                            <ModalLoadingMasRespuesta
                                open={openLoadingRespuesta}
                                loading={loading}
                                message={messageLoadingRespuesta}
                                loadingMessage="Creando reserva..."
                                successfulProcess={operacionExitosa}
                                accionPostCierre={accionPostCierreLoadingRespuesta}
                            />
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
        </Box>
    </Modal>
  )
}

export default ModalReservarEspacioComun;