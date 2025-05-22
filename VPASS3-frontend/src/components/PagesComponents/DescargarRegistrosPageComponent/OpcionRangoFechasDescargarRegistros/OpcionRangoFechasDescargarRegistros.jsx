import { Alert, Box, Snackbar, Typography } from "@mui/material"
import ButtonTypeOne from "../../../Buttons/ButtonTypeOne/ButtonTypeOne";
import "./OpcionRangoFechasDescargarRegistros.css"
import TextFieldDate from "../../../TextField/TextFieldDate/TextFieldDate";
import { useFormik } from "formik";
import { ValidationOpcionRangoFechasDescargarRegistros } from "./ValidationOpcionRangoFechasDescargarRegistros";
import useVisita from "../../../../hooks/useVisita/useVisita";
import { useState } from "react";
import ModalLoadingMasRespuesta from "../../../Modal/ModalLoadingMasRespuesta/ModalLoadingMasRespuesta";

// const OpcionRangoFechasDescargarRegistros = ({width = "100%"}) => {

//     const {loading, getVisitasPorRangoDeFechas} = useVisita();
//     const formik = useFormik({
//         initialValues: {
//             fechaInicio: "",
//             fechaFinal: ""
//         },
//         validationSchema: ValidationOpcionRangoFechasDescargarRegistros,
//         onSubmit: async (values) => {
//             console.log("Submit de opción rango de fechas descargar registros");
//         }
//     });
//     return (
//         <Box id="ContainerRangoFechasDescargarRegistros" sx={{width: width }}>
//             <Typography id="CabeceraRangoFechasDescargarRegistros">Rango de fechas</Typography>
//             <TextFieldDate
//                 name="fechaInicio"
//                 label="Fecha de inicio del reporte"
//                 width="90%"
//                 onChange={formik.handleChange}
//                 error={formik.touched.fechaInicio && Boolean(formik.errors.fechaInicio)}
//                 helperText={formik.touched.fechaInicio && formik.errors.fechaInicio}
//             />
//             <TextFieldDate
//                 name="fechaFinal"
//                 width="90%"
//                 label="Fecha final del reporte"
//                 onChange={formik.handleChange}
//                 error={formik.touched.fechaFinal && Boolean(formik.errors.fechaFinal)}
//                 helperText={formik.touched.fechaFinal && formik.errors.fechaFinal}
//             />

//             <ButtonTypeOne
//                 defaultText="Descargar reporte"
//                 width="60%"
//                 handleClick={formik.handleSubmit}
//             />
//         </Box>
//     )
// }

// export default OpcionRangoFechasDescargarRegistros;
























const OpcionRangoFechasDescargarRegistros = ({width = "100%"}) => {
    const {loading, getVisitasPorRangoDeFechas} = useVisita();

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
            fechaInicio: "",
            fechaFinal: ""
        },
        validationSchema: ValidationOpcionRangoFechasDescargarRegistros,
        onSubmit: async (values) => {
            setOpenLoadingRespuesta(true);
            try {
                // Las fechas ya vienen en formato YYYY-MM-DD desde los inputs
                const result = await getVisitasPorRangoDeFechas(values.fechaInicio, values.fechaFinal);
                
                if (result.success) {
                    setOperacionExitosa(true);
                    setMessageLoadingRespuesta("Reporte descargado con éxito")
                    // setSnackbar({
                    //     open: true,
                    //     message: result.error?.message || 'Error al descargar el reporte',
                    //     severity: 'error'
                    // });
                }
                else{
                    setOperacionExitosa(false);
                    setMessageLoadingRespuesta("Error al descargar el reporte");
                }
            } catch (error) {
                // setSnackbar({
                //     open: true,
                //     message: 'Ocurrió un error inesperado al generar el reporte',
                //     severity: 'error'
                // });
                setOperacionExitosa(false);
                setMessageLoadingRespuesta("Error al descargar el reporte");
            }
        }
    });

    return (
        <Box id="ContainerRangoFechasDescargarRegistros" sx={{width: width }}>
            <Typography id="CabeceraRangoFechasDescargarRegistros">Rango de fechas</Typography>
            <TextFieldDate
                name="fechaInicio"
                label="Fecha de inicio del reporte"
                width="90%"
                onChange={formik.handleChange}
                error={formik.touched.fechaInicio && Boolean(formik.errors.fechaInicio)}
                helperText={formik.touched.fechaInicio && formik.errors.fechaInicio}
            />
            <TextFieldDate
                name="fechaFinal"
                width="90%"
                label="Fecha final del reporte"
                onChange={formik.handleChange}
                error={formik.touched.fechaFinal && Boolean(formik.errors.fechaFinal)}
                helperText={formik.touched.fechaFinal && formik.errors.fechaFinal}
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
                loadingMessage="Registrando visita..."
                successfulProcess={operacionExitosa}
                accionPostCierre={accionPostCierreLoadingRespuesta}
            />
        </Box>
    )
}

export default OpcionRangoFechasDescargarRegistros;