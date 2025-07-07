import { Box, Typography } from "@mui/material"
import "./OpcionRutDescargarRegistros.css"
import TextFieldUno from "../../../TextField/TextFieldUno/TextFieldUno"
import ButtonTypeOne from "../../../Buttons/ButtonTypeOne/ButtonTypeOne"
import { useFormik } from "formik"
import { ValidationOpcionRutDescargarRegistros } from "./ValidationOpcionRutDescargarRegistros"
import useVisita from "../../../../hooks/useVisita/useVisita"
import ModalLoadingMasRespuesta from "../../../Modal/ModalLoadingMasRespuesta/ModalLoadingMasRespuesta"
import { useEffect, useState } from "react"

const OpcionRutDescargarRegistros = ({width = "100%"}) => {

    const { loading, getReporteVisitasPorRut } = useVisita();

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
            numeroIdentificacion: ""
        },
        validationSchema: ValidationOpcionRutDescargarRegistros,
        onSubmit: async (values) => {
            setOpenLoadingRespuesta(true);
            try {
                // Las fechas ya vienen en formato YYYY-MM-DD desde los inputs
                const result = await getReporteVisitasPorRut(values.numeroIdentificacion);
                
                if (result.success) {
                    setOperacionExitosa(true);
                    setMessageLoadingRespuesta("Reporte descargado con éxito")
                }
                else{
                    setOperacionExitosa(false);
                    setMessageLoadingRespuesta("Error al descargar el reporte");
                }
            } catch (error) {
                setOperacionExitosa(false);
                setMessageLoadingRespuesta("Error al descargar el reporte");
            }
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
                value={formik.values.numeroIdentificacion ?? ""}
                onChange={formik.handleChange}
                error={formik.touched.numeroIdentificacion && Boolean(formik.errors.numeroIdentificacion)}
                helperText={formik.touched.numeroIdentificacion && formik.errors.numeroIdentificacion}
            />

            <ButtonTypeOne
                defaultText="Descargar reporte"
                width="60%"
                handleClick={formik.handleSubmit}
                loading={loading}
                loadingText="Descargando reporte..."
            />

            <ModalLoadingMasRespuesta
                open={openLoadingRespuesta}
                loading={loading}
                message={messageLoadingRespuesta}
                loadingMessage="Descargando reporte..."
                successfulProcess={operacionExitosa}
                accionPostCierre={accionPostCierreLoadingRespuesta}
            />
    </Box>
  )
}

export default OpcionRutDescargarRegistros;
