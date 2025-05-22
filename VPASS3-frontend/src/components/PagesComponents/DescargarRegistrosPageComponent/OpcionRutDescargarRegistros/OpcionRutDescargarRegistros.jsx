import { Box, Typography } from "@mui/material"
import "./OpcionRutDescargarRegistros.css"
import TextFieldUno from "../../../TextField/TextFieldUno/TextFieldUno"
import ButtonTypeOne from "../../../Buttons/ButtonTypeOne/ButtonTypeOne"
import { useFormik } from "formik"
import { ValidationOpcionRutDescargarRegistros } from "./ValidationOpcionRutDescargarRegistros"

const OpcionRutDescargarRegistros = ({width = "100%"}) => {
    const formik = useFormik({
        initialValues: {
            numeroIdentificacion: ""
        },
        validationSchema: ValidationOpcionRutDescargarRegistros,
        onSubmit: async (values) => {
            console.log("Submit de opción rut descargar registros");
        }
    });
  return (
    <Box id="ContainerOpcionRutDescargarRegistros" sx={{width: width }}>
        <Typography id="CabeceraOpcionRutDescargarRegistros">Rut</Typography>
            <TextFieldUno
                name="numeroIdentificacion"
                label="Ingrese el rut que desea consultar"
                placeholder="12345678-9"
                width="90%"
                onChange={formik.handleChange}
                error={formik.touched.numeroIdentificacion && Boolean(formik.errors.numeroIdentificacion)}
                helperText={formik.touched.numeroIdentificacion && formik.errors.numeroIdentificacion}
            />

            <ButtonTypeOne
                defaultText="Descargar reporte"
                width="60%"
                handleClick={formik.handleSubmit}
            />
    </Box>
  )
}

export default OpcionRutDescargarRegistros;
