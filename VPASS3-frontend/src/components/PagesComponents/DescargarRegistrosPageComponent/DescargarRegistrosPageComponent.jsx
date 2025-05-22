import { Box, Typography } from "@mui/material"
import "./DescargarRegistrosPageComponent.css"
import OpcionRangoFechasDescargarRegistros from "./OpcionRangoFechasDescargarRegistros/OpcionRangoFechasDescargarRegistros"
import OpcionRutDescargarRegistros from "./OpcionRutDescargarRegistros/OpcionRutDescargarRegistros"

const DescargarRegistrosPageComponent = () => {
  return (
    <Box id="ContainerDescagarRegistrosComponent">
        <Box id="CuerpoDescargarRegistrosComponent">
            <Typography id="CabeceraDescargarRegistrosComponent">Descargar registros</Typography>

            <Box id="CuerpoOpcionesDescargarRegistrosComponent">
                <Box id="BoxOpcionRangoFechasDescargarRegistros">
                    <OpcionRangoFechasDescargarRegistros/>
                </Box>

                <Box id="BoxOpcionRutDescargarRegistros">
                    <OpcionRutDescargarRegistros/>
                </Box>
            </Box>
        </Box>
    </Box>
  )
}

export default DescargarRegistrosPageComponent