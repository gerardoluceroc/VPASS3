import { Box, ButtonBase, Typography } from "@mui/material";
import "./Dashboard.css";
import PersonPlus from "./../../assets/person-plus.png"
import ContactanosImage from "./../../assets/contactanos.png"
import DescargarRegistrosImage from "./../../assets/descargaRegistros.png"
import ConfiguracionImage from "./../../assets/configuracion.png"
import BlacklistImage from "./../../assets/blacklist.png"
import UltimosRegistrosImage from "./../../assets/ultimosregistros.png"
import { useNavigate } from "react-router-dom";

const Dashboard = () => {

    const RUTA_NUEVA_VISITA = "/visitas";
    const RUTA_HOME =  "/";
    const navigate = useNavigate();

    const handleOpcionClick = (ruta) => {
        navigate(ruta);
    };

    return (
        <Box id="ContainerDashboard">
                <Box id="DashboardBox">
                    <ButtonBase className="ItemDashboard" onClick={() => handleOpcionClick(RUTA_NUEVA_VISITA)}> 
                        <Box
                        component="img"
                        src={PersonPlus}
                        alt="Nueva visita"
                        className="ImagenNuevaVisitaDashboard"
                        />
                        <Typography className="TituloItemDashboard">Nueva Visita</Typography>
                    </ButtonBase>

                    <ButtonBase className="ItemDashboard" onClick={() => {}}>
                        <Box
                        component="img"
                        src={UltimosRegistrosImage}
                        alt="Últimos Registros"
                        className="ImagenNuevaVisitaDashboard"
                        />
                        <Typography className="TituloItemDashboard">Últimos Registros</Typography>
                    </ButtonBase>

                    <ButtonBase className="ItemDashboard" onClick={() => {}}>
                        <Box
                        component="img"
                        src={BlacklistImage}
                        alt="Lista Negra"
                        className="ImagenNuevaVisitaDashboard"
                        />
                        <Typography className="TituloItemDashboard">Lista Negra</Typography>
                    </ButtonBase>

                    <ButtonBase className="ItemDashboard" onClick={() => {}}>
                        <Box
                        component="img"
                        src={DescargarRegistrosImage}
                        alt="Descargar Registros"
                        className="ImagenNuevaVisitaDashboard"
                        />
                        <Typography className="TituloItemDashboard">Descargar Registros</Typography>
                    </ButtonBase>

                    <ButtonBase className="ItemDashboard" onClick={() => {}}>
                        <Box
                        component="img"
                        src={ContactanosImage}
                        alt="Contáctanos"
                        className="ImagenNuevaVisitaDashboard"
                        />
                        <Typography className="TituloItemDashboard">Contáctanos</Typography>
                    </ButtonBase>

                    <ButtonBase className="ItemDashboard" onClick={() => {}}>
                        <Box
                        component="img"
                        src={ConfiguracionImage}
                        alt="Ajustes"
                        className="ImagenNuevaVisitaDashboard"
                        />
                        <Typography className="TituloItemDashboard">Ajustes</Typography>
                    </ButtonBase>
                </Box>

        </Box>
    )
}

export default Dashboard;