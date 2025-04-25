import { Box, ButtonBase, Typography } from "@mui/material";
import "./Dashboard.css";
import PersonPlus from "./../../assets/person-plus.png"
import ContactanosImage from "./../../assets/contactanos.png"
import DescargarRegistrosImage from "./../../assets/descargaRegistros.png"
import ConfiguracionImage from "./../../assets/configuracion.png"
import BlacklistImage from "./../../assets/blacklist.png"
import UltimosRegistrosImage from "./../../assets/ultimosregistros.png"
import { opcionAjustes, opcionBlacklist, opcionContactanos, opcionDescargarRegistros, opcionNuevaVisita, opcionUltimosRegistros } from "../Home/constantesHome";
const Dashboard = ({handleOpcionSeleccionada = ()=>{}}) => {
return (
    <Box id="ContainerDashboard">
            <ButtonBase className="ItemDashboard" onClick={() => handleOpcionSeleccionada(opcionNuevaVisita)}>
                <Box
                component="img"
                src={PersonPlus}
                alt="Nueva visita"
                className="ImagenNuevaVisitaDashboard"
                />
                <Typography className="TituloItemDashboard">Nueva Visita</Typography>
            </ButtonBase>

            <ButtonBase className="ItemDashboard" onClick={() => handleOpcionSeleccionada(opcionUltimosRegistros)}>
                <Box
                component="img"
                src={UltimosRegistrosImage}
                alt="Últimos Registros"
                className="ImagenNuevaVisitaDashboard"
                />
                <Typography className="TituloItemDashboard">Últimos Registros</Typography>
            </ButtonBase>

            <ButtonBase className="ItemDashboard" onClick={() => handleOpcionSeleccionada(opcionBlacklist)}>
                <Box
                component="img"
                src={BlacklistImage}
                alt="Lista Negra"
                className="ImagenNuevaVisitaDashboard"
                />
                <Typography className="TituloItemDashboard">Lista Negra</Typography>
            </ButtonBase>

            <ButtonBase className="ItemDashboard" onClick={() => handleOpcionSeleccionada(opcionDescargarRegistros)}>
                <Box
                component="img"
                src={DescargarRegistrosImage}
                alt="Descargar Registros"
                className="ImagenNuevaVisitaDashboard"
                />
                <Typography className="TituloItemDashboard">Descargar Registros</Typography>
            </ButtonBase>

            <ButtonBase className="ItemDashboard" onClick={() => handleOpcionSeleccionada(opcionContactanos)}>
                <Box
                component="img"
                src={ContactanosImage}
                alt="Contáctanos"
                className="ImagenNuevaVisitaDashboard"
                />
                <Typography className="TituloItemDashboard">Contáctanos</Typography>
            </ButtonBase>

            <ButtonBase className="ItemDashboard" onClick={() => handleOpcionSeleccionada(opcionAjustes)}>
                <Box
                component="img"
                src={ConfiguracionImage}
                alt="Ajustes"
                className="ImagenNuevaVisitaDashboard"
                />
                <Typography className="TituloItemDashboard">Ajustes</Typography>
            </ButtonBase>

    </Box>
)
}

export default Dashboard;