import { useFormik } from "formik";
import CloseIcon from '@mui/icons-material/Close';
import { Box, Fade, IconButton, Modal, Typography } from "@mui/material";
import TextFieldUno from "../../TextField/TextFieldUno/TextFieldUno";
import ButtonTypeOne from "../../Buttons/ButtonTypeOne/ButtonTypeOne";
import "./ModalRegistrarEncomienda.css";
import useZonas from "../../../hooks/useZonas/useZonas";
import { useEffect, useState } from "react";
import SelectMui from "../../Select/SelectMui/SelectMui";
import RadioGroupMui from "../../RadioGroupMui/RadioGroupMui";
import SwitchMui from "../../Switch/SwitchMui/SwitchMui";
import TextFieldReadOnlyUno from "../../TextField/TextFieldReadOnlyUno/TextFieldReadOnlyUno";
import ProgressStepperMui from "../../StepperMui/ProgressStepperMui/ProgressStepperMui";
import ValidationRegistrarEncomienda from "./ValidationRegistrarEncomienda";
import { useConfirmDialog } from "../../../hooks/useConfirmDialog/useConfirmDialog";
import UseEncomienda from "../../../hooks/useEncomienda/useEncomienda";
import ModalLoadingMasRespuesta from "../ModalLoadingMasRespuesta/ModalLoadingMasRespuesta";

const ModalRegistarEncomienda = ({ open, onClose, setRows, departamentos }) => {

    const {crearRegistroEncomienda, loading: loadingEncomiendas} = UseEncomienda();
    const {zonas, getAllZonas} = useZonas();

    useEffect(() => {
      getAllZonas();
    }, []);

    // Se invoca la función para consultarle al usuario si desea enviar el formulario
    const { confirm, ConfirmDialogComponent } = useConfirmDialog();

    // Estados y funciones para manejar el componente ModalLoadingMasRespuesta
    const [openLoadingRespuesta, setOpenLoadingRespuesta] = useState(false);
    const [messageLoadingRespuesta, setMessageLoadingRespuesta] = useState('');
    const [operacionExitosa, setOperacionExitosa] = useState(false);
    const accionPostCierreLoadingRespuesta = () => {
        setOpenLoadingRespuesta(false);
        setMessageLoadingRespuesta('');
        // onClose();
    }

    const formik = useFormik({
        initialValues: {
            nombreDestinatario: null,
            codigoEncomienda: null,

            idZona: null,
            idDepartamento: null,

            encomiendaFueRetirada: false,
            nombrePersonaQueRetira: null,
            apellidoPersonaQueRetira: null,
            rutPersonaQueRetira: null,

            retiraPropietario: false, // Este campo se puede usar para indicar si el propietario es el que retira la encomienda

            idInquilinoDepartamento: null, // Este campo se puede usar para indicar el id del inquilino del departamento seleccionado
        },
        validationSchema: ValidationRegistrarEncomienda,
        onSubmit: async (values) => {
            const confirmed = await confirm({
                title: "¿Registrar encomienda?",
                message: "¿Deseas registrar esta encomienda?"
            });
        
            if (confirmed) {
                setOpenLoadingRespuesta(true);
                const nombreDestinatario = values.nombreDestinatario;
                const codigoEncomienda = values.codigoEncomienda;
                const idDepartamento = values.idDepartamento;
                const nombrePersonaQueRetira = values.nombrePersonaQueRetira;
                const apellidoPersonaQueRetira = values.apellidoPersonaQueRetira;
                const rutPersonaQueRetira = values.rutPersonaQueRetira;
                const encomiendaFueRetirada = values.encomiendaFueRetirada;
                const idInquilinoDepartamento = values.idInquilinoDepartamento;

                const respuestaCrearRegistroEncomienda = await crearRegistroEncomienda(
                    nombreDestinatario,
                    codigoEncomienda,
                    idDepartamento,
                    nombrePersonaQueRetira,
                    apellidoPersonaQueRetira,
                    rutPersonaQueRetira,
                    encomiendaFueRetirada,
                    idInquilinoDepartamento
                )

                const {statusCode: statusCodeCrearRegistroEncomienda, data: dataCrearRegistroEncomienda, message: messageCrearRegistroEncomienda} = respuestaCrearRegistroEncomienda;

                    // Si el servidor responde con el Response dto que tiene configurado
                    if(statusCodeCrearRegistroEncomienda != null && statusCodeCrearRegistroEncomienda != undefined){
                        if (statusCodeCrearRegistroEncomienda === 200 || statusCodeCrearRegistroEncomienda === 201) {
                            setOperacionExitosa(true);
                            setMessageLoadingRespuesta(messageCrearRegistroEncomienda);
                            // handleAgregarNuevaReservaExclusiva(dataCrearRegistroEncomienda);
                            setRows(prevRows => [...prevRows, dataCrearRegistroEncomienda]); // Agrega la nueva reserva a las filas de la tabla de registros de reservas
                            // formik.resetForm(); // Resetea el formulario después de crear la reserva exclusiva
                            // onClose();
                        }
                        else{
                            setOperacionExitosa(false);
                            setMessageLoadingRespuesta(messageCrearRegistroEncomienda);
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
    const opcionesRadioEncomiendaFueRetirada = [
        {
            id: 0,
            opcion: "No",
            value: false
        },
        {
            id: 1,
            opcion: "Sí",
            value: true
        }
    ]

    // Con esta función se evita que el modal se cierre al presionar fuera de él
    const handleClose = (event, reason) => {
        if (reason !== 'backdropClick') {
            onClose();
        }
    };

    // Estados y funciones para manejar los pasos llevados en el formulario
    const [pasoActualFormulario, setPasoActualFormulario] = useState(0);
    const cantidadPasosFormulario = 4;
    // const handleNextClick = () => {setPasoActualFormulario(prev => prev + 1)}
    const handleResetClick = () => {setPasoActualFormulario(0)}
    // Funcion para retroceder en el formulario con el boton "volver"
    const handleBackClick = () => {setPasoActualFormulario(prev => prev - 1)}

    const handleNextClick = async () => {
        let fieldsToValidate = [];

        switch (pasoActualFormulario) {
            case 0:
            fieldsToValidate = ['nombreDestinatario', 'codigoEncomienda'];
            break;
            case 1:
            fieldsToValidate = ['idZona', 'idDepartamento'];
            break;
            case 2:
            fieldsToValidate = ['encomiendaFueRetirada'];
            if (formik.values.encomiendaFueRetirada && !formik.values.retiraPropietario) {
                fieldsToValidate.push(
                'nombrePersonaQueRetira',
                'apellidoPersonaQueRetira',
                'rutPersonaQueRetira'
                );
            }
            break;
            default:
            break;
        }

        let erroresDetectados = false;

        // Validamos campo por campo con Yup.validateAt
        for (const campo of fieldsToValidate) {
            try {
            await ValidationRegistrarEncomienda.validateAt(campo, formik.values);
            } catch (error) {
            erroresDetectados = true;
            formik.setFieldError(campo, error.message);
            formik.setFieldTouched(campo, true, false);
            }
        }

        // Si no se detectaron errores, se avanza al siguiente paso
        if (!erroresDetectados) {
            setPasoActualFormulario((prev) => prev + 1);
        }
    };

    //Cada vez que se abra el modal se reseteará el formulario y se muestra en el primer paso
    useEffect(() => {
      if(open){
        formik.resetForm();
        setPasoActualFormulario(0);
      }
    }, [open])


    // Si se cambia el valor de codigoEncomienda, se debe resetear el campo a null si el valor es un string vacío.
    // Esto es para el caso en que el usuario borre el valor del campo codigoEncomienda, ya que si no se hace esto, al enviar el formulario, se enviaría un string vacío en vez de null.
    useEffect(() => {
        if(formik.values.codigoEncomienda === ""){
            formik.setFieldValue("codigoEncomienda", null);
        }
    }, [formik.values.codigoEncomienda])
    

    // Si se cambia el valor de la zona escogida, se debe resetear lo que habia en idDepartamento, ya que si no se hace esto, al cambiar de zona, el departamento seleccionado anteriormente se queda guardado en el formulario
    // y al enviar el formulario, se enviaría un idDepartamento que no corresponde a la zona seleccionada.
    // Por lo tanto, se debe resetear el idDepartamento a null cada vez que se cambie la zona seleccionada.
    // Esto se hace con un useEffect que se ejecuta cada vez que cambia el valor de idZona en el formulario.
    useEffect(() => {
        return () => {
        formik.setFieldValue("idDepartamento", null);
        }
    }, [formik.values.idZona])


    const [departamentosDisponibles, setDepartamentosDisponibles] = useState([]);
    useEffect(() => {
        //Cada vez que se cambia la zona seleccionada, se setea el estado de departamentosDisponibles
        if(formik.values.idZona !== null && formik.values.idZona !== undefined){
            const departamentosZona = departamentos?.filter((depto) => depto.idZone === formik.values.idZona) || [];
            setDepartamentosDisponibles(departamentosZona);
        }
    }, [formik.values.idZona, departamentos]);


    const [departamentoSeleccionado, setDepartamentoSeleccionado] = useState(null);
    useEffect(() => {
        // Cada vez que se cambia el idDepartamento, se setea el estado de departamentoSeleccionado
        if(formik.values.idDepartamento !== null && formik.values.idDepartamento !== undefined){
            const departamento = departamentos?.find((departamento) => departamento.id === formik.values.idDepartamento) || "";
            setDepartamentoSeleccionado(departamento);
            formik.setFieldValue("idInquilinoDepartamento", departamento?.activeOwnership?.id || null);
        }
        else{
            // Si el idDepartamento es null o undefined, se setea el estado de departamentoSeleccionado a un string vacío
            setDepartamentoSeleccionado(null);
            formik.setFieldValue("idInquilinoDepartamento", null);
        }
    }, [formik.values.idDepartamento, departamentos]);


    // Si se cambia el valor de encomiendaFueRetirada, se debe resetear los campos de persona que retira, ya que si no se hace esto, al cambiar de valor de encomiendaFueRetirada, los campos de persona que retira se quedan con los valores anteriores.
    // Por lo tanto, se debe resetear los campos de persona que retira a null
    useEffect(() => {
      if (!formik.values.encomiendaFueRetirada) {
        formik.setFieldValue("nombrePersonaQueRetira", null);
        formik.setFieldValue("apellidoPersonaQueRetira", null);
        formik.setFieldValue("rutPersonaQueRetira", null);
        formik.setFieldValue("retiraPropietario", false); // Resetea el switch de retiraPropietario
      }
    }, [formik.values.encomiendaFueRetirada])


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

    return (
        <Modal open={open} onClose={handleClose}>
            <Box id="ContainerModalRegistrarEncomienda">
                <Box id="HeaderModalRegistrarEncomienda">
                    <Typography variant="h5" component="h5" gutterBottom>
                        {"Registrar encomienda"}
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

                <Box id="CuerpoModalRegistrarEncomienda">
                    {pasoActualFormulario === 0 && 
                        <Fade in={pasoActualFormulario === 0} timeout={300}>
                            <Box id="ItemCuerpoModalRegistrarEncomienda">
                                <Typography variant="h6" gutterBottom>
                                    {"Información de la encomienda"}
                                </Typography>

                                <Box id="DosItemsCuerpoModalRegistrarEncomienda">
                                    <TextFieldUno 
                                        name="nombreDestinatario" 
                                        label="Nombre del destinatario" 
                                        placeholder="Ej: Juan Pérez" 
                                        value={formik.values.nombreDestinatario}
                                        onChange={formik.handleChange}
                                        error={formik.touched.nombreDestinatario && Boolean(formik.errors.nombreDestinatario)}
                                        helperText={formik.touched.nombreDestinatario && formik.errors.nombreDestinatario}
                                    />

                                    <TextFieldUno 
                                        name="codigoEncomienda" 
                                        label="Código de la encomienda (opcional)" 
                                        placeholder="" 
                                        value={formik.values.codigoEncomienda}
                                        onChange={formik.handleChange}
                                        error={formik.touched.codigoEncomienda && Boolean(formik.errors.codigoEncomienda)}
                                        helperText={formik.touched.codigoEncomienda && formik.errors.codigoEncomienda}
                                    />
                                </Box>
                            </Box>
                        </Fade>
                    }

                    {pasoActualFormulario === 1 &&
                        <Fade in={pasoActualFormulario === 1} timeout={300}>
                            <Box id="ItemCuerpoModalRegistrarEncomienda">
                                <Typography variant="h6" component="h6" gutterBottom>
                                    {"Destino"}
                                </Typography>
                                <Box id="DosItemsCuerpoModalRegistrarEncomienda">
                                    <SelectMui
                                        label = "Sector/Calle/Piso"
                                        name="idZona"
                                        width={"100%"}
                                        listadoElementos={zonas || []}
                                        keyListadoElementos={"id"}
                                        mostrarElemento={(option)=> option["name"]}
                                        handleChange = {formik.handleChange}
                                        elementoSeleccionado = {formik.values.idZona}
                                        atributoValue={"id"}
                                        helperText={formik.touched.idZona && formik.errors.idZona}
                                        error={formik.touched.idZona && Boolean(formik.errors.idZona)}
                                    />

                                    <SelectMui
                                        label = "Departamento"
                                        name="idDepartamento"
                                        width={"100%"}
                                        listadoElementos={departamentosDisponibles || []}
                                        keyListadoElementos={"id"}
                                        mostrarElemento={(option)=> option["name"]}
                                        disabledOptionCondition={(option) => {
                                            // Deshabilita los departamentos que no tienen propietario activo
                                            return (option.activeOwnership === null || option.activeOwnership === undefined)
                                        }}
                                        handleChange = {formik.handleChange}
                                        elementoSeleccionado = {formik.values.idDepartamento}
                                        atributoValue={"id"}
                                        helperText={formik.touched.idDepartamento && formik.errors.idDepartamento}
                                        error={formik.touched.idDepartamento && Boolean(formik.errors.idDepartamento)}
                                    />
                                </Box>
                            </Box>
                        </Fade>
                    }

                    {pasoActualFormulario === 2 &&
                        <Fade in={pasoActualFormulario === 2} timeout={300}>
                            <Box id="ItemCuerpoModalRegistrarEncomienda">
                                <RadioGroupMui
                                    label="¿La encomienda ya fue retirada por su destinatario?"
                                    name="encomiendaFueRetirada"
                                    listadoElementos={opcionesRadioEncomiendaFueRetirada}
                                    keyListadoElementos="id"
                                    atributoValue="value"
                                    mostrarElemento={(option) => option.opcion}
                                    handleChange={(e) => {
                                        formik.setFieldValue('encomiendaFueRetirada', JSON.parse(e.target.value)); //El JSON.parse es para convertir el string "true" o "false" a booleano
                                    }}
                                    elementoSeleccionado={formik.values.encomiendaFueRetirada}
                                    helperText={formik.touched.encomiendaFueRetirada && formik.errors.encomiendaFueRetirada}
                                    error={formik.touched.encomiendaFueRetirada && Boolean(formik.errors.encomiendaFueRetirada)}
                                    row={true} // Para mostrar los radios en horizontal
                                />

                                {formik.values.encomiendaFueRetirada && (
                                    <Box id="ItemCuerpoModalRegistrarEncomienda">
                                        <Typography variant="h6" component="h6" gutterBottom>
                                            {"Información de la persona que retira la encomienda"}
                                        </Typography>

                                        <Box>
                                            <SwitchMui
                                                name="retiraPropietario"
                                                primaryLabel="¿Es el inquilino quien retiró la encomienda?"
                                                secondaryLabel=""
                                                handleChange={formik.handleChange}
                                                checked={formik.values.retiraPropietario}
                                                helperText=""
                                            />
                                        </Box>

                                        {
                                        
                                            formik.values.retiraPropietario === true ? 
                                                <>
                                                    <Box id="DosItemsCuerpoModalRegistrarEncomienda">
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
                                                    <Box id="DosItemsCuerpoModalRegistrarEncomienda">
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
                                    </Box>
                                )}
                            </Box>
                        </Fade>
                    }

                    {pasoActualFormulario === 3 &&
                        <Fade in={pasoActualFormulario === 3} timeout={300}>
                            <Box id="ItemCuerpoModalRegistrarEncomienda">
                                <Box id="DosItemsCuerpoModalRegistrarEncomienda">
                                    <TextFieldReadOnlyUno
                                        label={"Destinatario"}
                                        value={ (formik.values.nombreDestinatario !== null && formik.values.nombreDestinatario !== undefined && formik.values.nombreDestinatario !== "" ) ? formik.values.nombreDestinatario : "Sin datos"}
                                    /> 
                                    <TextFieldReadOnlyUno
                                        label={"Código de encomienda"}
                                        value={(formik.values.codigoEncomienda !== null && formik.values.codigoEncomienda !== undefined && formik.values.codigoEncomienda !== "" ) ? formik.values.codigoEncomienda : "No asignado"}
                                    />
                                </Box>

                                <TextFieldReadOnlyUno
                                    label={"Destino"}
                                    value={(departamentoSeleccionado !== null && departamentoSeleccionado !== undefined) ? `${departamentoSeleccionado?.zoneName} - ${departamentoSeleccionado?.name}` : "Sin datos"}
                                /> 

                                
                                <TextFieldReadOnlyUno
                                    label={"¿La encomienda ya fue retirada?"}
                                    value={formik.values.encomiendaFueRetirada ? "Sí" : "No"}
                                /> 

                                {formik.values.encomiendaFueRetirada && (
                                    <TextFieldReadOnlyUno
                                        label="Persona que retira"
                                        value={formik.values.nombreDestinatario !== null && formik.values.nombreDestinatario !== undefined ? `${formik.values.nombrePersonaQueRetira} ${formik.values.apellidoPersonaQueRetira} - Rut: ${formik.values.rutPersonaQueRetira}` : "Sin datos"}
                                    />
                                )}

                                <Box id="BoxButtonSubmitModalRegistrarEncomienda">
                                    <ButtonTypeOne
                                        defaultText="Confirmar"
                                        loadingText="Registrando encomienda..."
                                        handleClick={formik.handleSubmit}
                                        disabled={formik.isSubmitting}
                                    />
                                </Box>
                            </Box>
                        </Fade>
                    }
                </Box>

                <ProgressStepperMui
                    activeStep={pasoActualFormulario}
                    handleNext={handleNextClick}
                    handleBack={handleBackClick}
                    steps={cantidadPasosFormulario}
                    width="100%"
                />

                {ConfirmDialogComponent}

                <ModalLoadingMasRespuesta
                    open={openLoadingRespuesta}
                    loading={loadingEncomiendas}
                    message={messageLoadingRespuesta}
                    loadingMessage="Registrando encomienda..."
                    successfulProcess={operacionExitosa}
                    accionPostCierre={accionPostCierreLoadingRespuesta}
                />
            </Box>
        </Modal>
    );
};

export default ModalRegistarEncomienda;