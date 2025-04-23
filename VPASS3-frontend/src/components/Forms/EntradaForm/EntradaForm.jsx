import { useFormik } from "formik";
import "./EntradaForm.css"
import { Box } from "@mui/material";
import TextFieldUno from "../../TextField/TextFieldUno/TextFieldUno";
import { useEffect, useState } from "react";
import useSentido from "../../../hooks/auth/useSentido/useSentido";
import SelectMui from "../../Select/SelectMui/SelectMui";
import SwitchMui from "../../Switch/SwitchMui/SwitchMui";
import useZonas from "../../../hooks/useZonas/useZonas";
import useLugaresEstacionamiento from "../../../hooks/UseLugarEstacionamiento/useLugarEstacionamiento";

const EntradaForm = () => {
    const {loading: loadingSentidos, sentidos, getAllSentidos} = useSentido();
    const {loading:loadingZonas, zonas, getAllZonas } = useZonas();
    const {loading:loadingLugaresEstacionamiento, lugaresEstacionamiento, getAllLugaresEstacionamiento } = useLugaresEstacionamiento();

    useEffect(() => {console.log("📌 sentidos => ",sentidos)}, [sentidos]);
    useEffect(() => {console.log("📌 zonas => ",zonas)}, [zonas]);
    useEffect(() => {console.log("📌 - lugaresEstacionamiento => ",lugaresEstacionamiento)}, [lugaresEstacionamiento]);


    useEffect(() => {
        getAllSentidos();
        getAllZonas();
        getAllLugaresEstacionamiento();
    }, [])
    
    const formik = useFormik({
        initialValues:{
            nombres: '',
            apellidos: '',
            rut: '',
            idTipoVisita: '',
            idZona: '',
            idSubZona: '',
            idSentido: '',
            incluyeVehiculo: false,
            patenteVehiculo: '',
            idEstacionamiento: '',
        },
        // validationSchema: ValidationNuevoFacial,
        onSubmit:
        async () => {
            
        }
    })
    useEffect(() => {console.log("📌 formik values => ",formik.values)}, [formik.values]);

    const [subZonasDisponibles, setSubZonasDisponibles] = useState([]);
    useEffect(() => {

        //Cada vez que se cambia la zona seleccionada, se setea el estado de subZonasDisponibles
        const subZona = zonas?.find((zona) => zona.id === formik.values.idZona)?.zoneSections || [];
        setSubZonasDisponibles(subZona);
    }, [formik.values.idZona])


  return (
    <Box id= "ContainerEntradaForm">
        <Box className ="DosItemsEntradaForm">
            <TextFieldUno name="nombres" type="text" label="Nombres" placeholder="Ingrese los nombres del visitante" onChange={formik.handleChange}/>
            <TextFieldUno name="apellidos" type="text" label="Apellidos" placeholder="Ingrese los apellidos del visitante" onChange={formik.handleChange}/>
        </Box>

        <TextFieldUno name="rut" type="text" label="RUT" placeholder="12345678-9" onChange={formik.handleChange}/>

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
            />

            <SelectMui
                label = "Tipo de visita"
                name="idTipoVisita"
                width={"100%"}
                listadoElementos={sentidos || []}
                keyListadoElementos={"id"}
                mostrarElemento={(option)=> option["visitDirection"]}
                handleChange = {formik.handleChange}
                elementoSeleccionado = {formik.values.idSentido}
                atributoValue={"id"}
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
            />

            <SelectMui
                label = "Número/SubZona"
                name="idSubZona"
                width={"100%"}
                listadoElementos={subZonasDisponibles || []}
                keyListadoElementos={"id"}
                mostrarElemento={(option)=> option["name"]}
                handleChange = {formik.handleChange}
                elementoSeleccionado = {formik.values.idSubZona}
                atributoValue={"id"}
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
            <Box className="DosItemsEntradaForm">
                <TextFieldUno name="patenteVehiculo" type="text" label="Patente" placeholder="Ingrese la patente del vehículo" onChange={formik.handleChange}/>   
                <SelectMui
                    label = "Estacionamiento"
                    name="idEstacionamiento"
                    width={"100%"}
                    listadoElementos={lugaresEstacionamiento || []}
                    keyListadoElementos={"id"}
                    mostrarElemento={(option)=> option["name"]}
                    handleChange = {formik.handleChange}
                    elementoSeleccionado = {formik.values.idEstacionamiento}
                    atributoValue={"id"}
                />     
            </Box> 
        }
    </Box>
  )
}

export default EntradaForm;