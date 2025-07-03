import { useFormik } from "formik";
import "./EntradaForm.css";
import { Box, Typography } from "@mui/material";
import TextFieldUno from "../../TextField/TextFieldUno/TextFieldUno";
import { useEffect, useState } from "react";
import useSentido from "../../../hooks/auth/useSentido/useSentido";
import SelectMui from "../../Select/SelectMui/SelectMui";
import SwitchMui from "../../Switch/SwitchMui/SwitchMui";
import useZonas from "../../../hooks/useZonas/useZonas";
import useLugaresEstacionamiento from "../../../hooks/UseLugarEstacionamiento/useLugarEstacionamiento";
import useTiposVisita from "../../../hooks/useTipoVisita/useTipoVisita";
import { ValidationVisitaForm } from "./ValidationVisitaForm";
import ButtonTypeOne from "../../Buttons/ButtonTypeOne/ButtonTypeOne";
import useVisita from "../../../hooks/useVisita/useVisita";
import { useConfirmDialog } from "../../../hooks/useConfirmDialog/useConfirmDialog.jsx";
import ModalLoadingMasRespuesta from "../../Modal/ModalLoadingMasRespuesta/ModalLoadingMasRespuesta.jsx";
import { generarRango } from "../../../utils/funciones.js";
import { cantidadHorasMaximasUsoEstacionamiento, cantidadHorasMinimasUsoEstacionamiento, idSentidoVisitaEntrada, idSentidoVisitaSalida } from "../../../utils/constantes.js";

const EntradaForm = () => {

    const { sentidos, getAllSentidos} = useSentido();
    const { zonas, getAllZonas } = useZonas();
    const { lugaresEstacionamiento, getAllLugaresEstacionamiento } = useLugaresEstacionamiento();
    const { tiposVisita, getAllTiposVisita } = useTiposVisita();
    const { loading: loadingVisitas, crearVisita } = useVisita();

    useEffect(() => {
        getAllSentidos();
        getAllZonas();
        getAllLugaresEstacionamiento();
        getAllTiposVisita();
    }, [])

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
            rut: '',
            idTipoVisita: null,
            idZona: null,
            idSubZona: null,
            idSentido: null,
            incluyeVehiculo: false,
            patenteVehiculo: '',
            horasUsoEstacionamiento: "",
            minutosUsoEstacionamiento: "",
            idEstacionamiento: null,
        },
        validationSchema: ValidationVisitaForm,
        onSubmit: async (values) => {
            const confirmed = await confirm({
                title: "Registrar visita",
                message: "¿Deseas registar esta nueva visita?"
            });
        
            if (confirmed) {
                setOpenLoadingRespuesta(true);

                // Se envía la información al backend para crear la visita
                const {statusCode: statusCodeCrearVisita, data: dataVisitaCreada, message: messageCrearVisita} = await crearVisita({
                    nombres: values.nombres,
                    apellidos: values.apellidos,
                    numeroIdentificacion: values.rut,
                    idTipoVisita: values.idTipoVisita,
                    idZona: values.idZona,
                    idSubZona: values.idSubZona,
                    idSentido: values.idSentido,
                    incluyeVehiculo: values.incluyeVehiculo,
                    patenteVehiculo: values.patenteVehiculo,
                    idEstacionamiento: values.idEstacionamiento,
                    horasUsoEstacionamiento: values.horasUsoEstacionamiento,
                    minutosUsoEstacionamiento: values.minutosUsoEstacionamiento
                });

                // Si el servidor responde con el Response dto que tiene configurado
                if(statusCodeCrearVisita != null && statusCodeCrearVisita != undefined){
                    if (statusCodeCrearVisita === 200 || statusCodeCrearVisita === 201 && (statusCodeCrearVisita != null && statusCodeCrearVisita != undefined)) {
                        setOperacionExitosa(true);
                        setMessageLoadingRespuesta(messageCrearVisita);
                        formik.resetForm(); // Resetea el formulario después de crear la visita
                    }
                    else if (statusCodeCrearVisita === 500) {
                        //En caso de error 500, se muestra un mensaje de error genérico, en vez del mensaje de error del backend
                        setOperacionExitosa(false);
                        setMessageLoadingRespuesta("Error desconocido, por favor intente nuevamente más tarde.");
                    }
                    else{
                        //En caso de cualquier otro error, se muestra el mensaje de error del backend
                        setOperacionExitosa(false);
                        setMessageLoadingRespuesta(messageCrearVisita);
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

    // Si se cambia el valor de la zona escogida, se debe resetear lo que habia en idSubZona
    useEffect(() => {
      return () => {
        formik.setFieldValue("idSubZona", null);
      }
    }, [formik.values.idZona])

    const [subZonasDisponibles, setSubZonasDisponibles] = useState([]);
    useEffect(() => {
        //Cada vez que se cambia la zona seleccionada, se setea el estado de subZonasDisponibles
        const subZona = zonas?.find((zona) => zona.id === formik.values.idZona)?.zoneSections || [];
        setSubZonasDisponibles(subZona);
    }, [formik.values.idZona])

    return (
        <Box id= "ContainerEntradaForm">
            <Typography variant="h3" id="TituloEntradaForm">Nueva Visita</Typography>
            <Box className ="DosItemsEntradaForm">
                <TextFieldUno 
                    name="nombres" 
                    type="text" 
                    label="Nombres"
                    value={formik.values.nombres} 
                    placeholder="Ingrese los nombres del visitante" 
                    onChange={formik.handleChange}
                    error={formik.touched.nombres && Boolean(formik.errors.nombres)}
                    helperText={formik.touched.nombres && formik.errors.nombres}
                    />
                <TextFieldUno 
                    name="apellidos" 
                    type="text" 
                    label="Apellidos" 
                    value={formik.values.apellidos}
                    placeholder="Ingrese los apellidos del visitante" 
                    onChange={formik.handleChange}
                    error={formik.touched.apellidos && Boolean(formik.errors.apellidos)}
                    helperText={formik.touched.apellidos && formik.errors.apellidos}
                    />
            </Box>

                <TextFieldUno 
                    name="rut" 
                    type="text" 
                    label="RUT" 
                    value={formik.values.rut}
                    placeholder="12345678-9" 
                    onChange={formik.handleChange}
                    error={formik.touched.rut && Boolean(formik.errors.rut)}
                    helperText={formik.touched.rut && formik.errors.rut}
                />

            <Box className="DosItemsEntradaForm">
                <SelectMui
                    label = "Sentido"
                    name="idSentido"
                    width={"100%"}
                    listadoElementos={sentidos || []}
                    keyListadoElementos={"id"}
                    mostrarElemento={(option)=> option["visitDirection"]}
                    handleChange = {formik.handleChange}
                    elementoSeleccionado = {formik.values.idSentido}
                    atributoValue={"id"}
                    helperText={formik.touched.idSentido && formik.errors.idSentido}
                    error={formik.touched.idSentido && Boolean(formik.errors.idSentido)}
                />

                <SelectMui
                    label = "Tipo de visita"
                    name="idTipoVisita"
                    width={"100%"}
                    listadoElementos={tiposVisita || []}
                    keyListadoElementos={"id"}
                    mostrarElemento={(option)=> option["name"]}
                    handleChange = {formik.handleChange}
                    elementoSeleccionado = {formik.values.idTipoVisita}
                    atributoValue={"id"}
                    helperText={formik.touched.idTipoVisita && formik.errors.idTipoVisita}
                    error={formik.touched.idTipoVisita && Boolean(formik.errors.idTipoVisita)}
                />
            </Box>

            <Box className="DosItemsEntradaForm">
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
                    name="idSubZona"
                    width={"100%"}
                    listadoElementos={subZonasDisponibles || []}
                    keyListadoElementos={"id"}
                    mostrarElemento={(option)=> option["name"]}
                    handleChange = {formik.handleChange}
                    elementoSeleccionado = {formik.values.idSubZona}
                    atributoValue={"id"}
                    helperText={formik.touched.idSubZona && formik.errors.idSubZona}
                    error={formik.touched.idSubZona && Boolean(formik.errors.idSubZona)}
                />
            </Box>
            <Box id="SwitchIncluyeVehiculoEntradaForm">
                <SwitchMui
                    name="incluyeVehiculo"
                    primaryLabel="¿Incluye vehículo?"
                    secondaryLabel=""
                    handleChange={formik.handleChange}
                    checked={formik.values.incluyeVehiculo}
                    helperText=""
                />
            </Box>

            {formik.values.incluyeVehiculo &&
                <>
                    <Box className="DosItemsEntradaForm">
                        <TextFieldUno 
                            name="patenteVehiculo"                         
                            label="Patente" 
                            value={formik.values.patenteVehiculo}
                            placeholder="Ingrese la patente del vehículo" 
                            onChange={formik.handleChange}
                            error={formik.touched.patenteVehiculo && Boolean(formik.errors.patenteVehiculo)}
                            helperText={formik.touched.patenteVehiculo && formik.errors.patenteVehiculo}
                            />   
                        <SelectMui
                            label = "Estacionamiento"
                            name="idEstacionamiento"
                            width={"100%"}
                            listadoElementos={lugaresEstacionamiento || []}
                            keyListadoElementos={"id"}
                            mostrarElemento={(option)=> option["name"]}
                            handleChange = {formik.handleChange}

                            // La condicion && formik.values.idSentido === idSentidoVisitaEntrada se agrega ya que si es salida, obviamente el estacionamiento usado estará en ocupado, 
                            // por lo que esta comprobación extra permite que el usuario en caso de ser una visita de salida, pueda seleccionar el estacionamiento que el visitante utilizó
                            disabledOptionCondition={(opcion) => opcion["isAvailable"] === false && formik.values.idSentido === idSentidoVisitaEntrada}
                            elementoSeleccionado = {formik.values.idEstacionamiento}
                            atributoValue={"id"}
                            helperText={formik.touched.idEstacionamiento && formik.errors.idEstacionamiento}
                            error={formik.touched.idEstacionamiento && Boolean(formik.errors.idEstacionamiento)}
                        />     
                    </Box> 

                {formik.values.idSentido === idSentidoVisitaEntrada &&
                    // Si el sentido es entrada, se muestran los selects para las horas y minutos de uso del estacionamiento
                    <Box className="DosItemsEntradaForm">
                        <SelectMui
                            label = "Horas de uso de estacionamiento autorizadas"
                            name="horasUsoEstacionamiento"
                            width={"100%"}
                            listadoElementos={generarRango(cantidadHorasMinimasUsoEstacionamiento, cantidadHorasMaximasUsoEstacionamiento)}
                            keyListadoElementos={"id"}
                            mostrarElemento={(option)=> `${option["valor"]} hora(s)`}
                            handleChange = {formik.handleChange}
                            elementoSeleccionado = {formik.values.horasUsoEstacionamiento}
                            atributoValue={"valor"}
                            helperText={formik.touched.horasUsoEstacionamiento && formik.errors.horasUsoEstacionamiento}
                            error={formik.touched.horasUsoEstacionamiento && Boolean(formik.errors.horasUsoEstacionamiento)}
                        />

                        <SelectMui
                            label = "Minutos de uso de estacionamiento autorizados"
                            name="minutosUsoEstacionamiento"
                            width={"100%"}
                            listadoElementos={generarRango(0, 59)}
                            keyListadoElementos={"id"}
                            mostrarElemento={(option)=> `${option["valor"]} minuto(s)`}
                            handleChange = {formik.handleChange}
                            elementoSeleccionado = {formik.values.minutosUsoEstacionamiento}
                            atributoValue={"valor"}
                            helperText={formik.touched.minutosUsoEstacionamiento && formik.errors.minutosUsoEstacionamiento}
                            error={formik.touched.minutosUsoEstacionamiento && Boolean(formik.errors.minutosUsoEstacionamiento)}
                        />
                    </Box>
                }
                </>
            }

            <Box id="BoxButtonSubmitEntradaForm">
                <ButtonTypeOne
                    defaultText="Registrar visita"
                    loadingText="Registrando visita..."
                    handleClick={formik.handleSubmit}
                    // handleClick={simularPeticion}
                    disabled={formik.isSubmitting}
                />
            </Box>

            {ConfirmDialogComponent}

            <ModalLoadingMasRespuesta
                open={openLoadingRespuesta}
                loading={loadingVisitas}
                message={messageLoadingRespuesta}
                loadingMessage="Registrando visita..."
                successfulProcess={operacionExitosa}
                accionPostCierre={accionPostCierreLoadingRespuesta}
            />
        </Box>
    )
}
export default EntradaForm;