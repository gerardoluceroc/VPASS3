import { useFormik } from "formik";
import "./EntradaForm.css"
import { Box } from "@mui/material";
import TextFieldUno from "../../TextField/TextFieldUno/TextFieldUno";
import { useEffect } from "react";
import useSentido from "../../../hooks/auth/useSentido/useSentido";
import SelectMui from "../../Select/SelectMui/SelectMui";

const EntradaForm = () => {
    const {loading: loadingSentidos, sentidos, getAllSentidos} = useSentido();

    useEffect(() => {console.log("📌 sentidos => ",sentidos)}, [sentidos]);

    useEffect(() => {
      getAllSentidos();
    }, [])
    
    const formik = useFormik({
        initialValues:{
            nombres: '',
            apellidos: '',
            rut: '',
            zona: '',
            idSentido: '',
            incluyeVehiculo: false,
        },
        // validationSchema: ValidationNuevoFacial,
        onSubmit:
        async () => {
            
        }
    })
    useEffect(() => {console.log("📌 formik values => ",formik.values)}, [formik.values]);
  return (
    <Box id= "ContainerEntradaForm">
        <Box id="NombresApellidosEntradaForm">
            <TextFieldUno name="nombres" type="text" label="Nombres" placeholder="Ingrese los nombres del visitante" onChange={formik.handleChange}/>
            <TextFieldUno name="apellidos" type="text" label="Apellidos" placeholder="Ingrese los apellidos del visitante" onChange={formik.handleChange}/>
        </Box>

        <TextFieldUno name="rut" type="text" label="RUT" placeholder="12345678-9" onChange={formik.handleChange}/>
        <SelectMui
            label = "Sentido"
            name="idSentido"
            listadoElementos={sentidos || []}
            keyListadoElementos={"id"}
            mostrarElemento={(option)=> option["visitDirection"]}
            handleChange = {formik.handleChange}
            elementoSeleccionado = {formik.values.idSentido}
            atributoValue={"id"}
        />
    </Box>
  )
}

export default EntradaForm;