import { Box, TextField } from "@mui/material";
import Dashboard from "../../Dashboard/Dashboard";
import DrawerResponsive from "../../Drawer/DrawerResponsive/DrawerResponsive";
import "./HomePageComponent.css";
import { useState } from "react";
import { opcionNuevaVisita, opcionPanelDeControl } from "./constantesHome";
import EntradaForm from "../../Forms/EntradaForm/EntradaForm";

function HomePageComponent() {

  const [opcionSeleccionada, setOpcionSeleccionada] = useState(opcionPanelDeControl);

  const handleOpcionSeleccionada = (opcion) => {
    setOpcionSeleccionada(opcion);
  }

  return (
    <Box id="ContainerHomePageComponent">
      <Dashboard/> 
    </Box>
  );
}

export default HomePageComponent;