import { Box } from "@mui/material";
import DatagridResponsive from "../../Datagrid/DatagridResponsive/DatagridResponsive";
import "./UltimosRegistrosPageComponent.css";
import useVisita from "../../../hooks/useVisita/useVisita";
import { useEffect } from "react";

const UltimosRegistrosPageComponent = () => {
    const { getAllVisitas, visitas } = useVisita();
  
    useEffect(() => {
      getAllVisitas();
    }, []);

    useEffect(() => {console.log("visitas => ",visitas)}, [visitas]);
    // useEffect(() => {console.log("visitas => ",JSON.stringify(visitas))}, [visitas]);
    
    // const columns = [
    //     { name: "Name", options: { filterOptions: { fullWidth: true } } },
    //     "Title",
    //     "Location"
    //   ];
    
    //   const data = [
    //     ["Gabby George", "Business Analyst", "Minneapolis"],
    //     ["Aiden Lloyd", "Consultant", "Dallas"],
    //     ["Jaden Collins", "Attorney", "Santa Ana"],
    //     ["Franky Rees", "Analyst", "St. Petersburg"],
    //   ];

  
    if (!Array.isArray(visitas)) {
      return <div>Cargando visitas...</div>;
    }
  
    const columns = ["Nombre", "Rut", "Destino", "Sentido", "Hora"];
    const data = visitas.map(({ visitor, zone, zoneSection, direction, entryDate: horaEntrada }) => {
      const { names = "", lastNames = "", identificationNumber = "" } = visitor || {};
      const {name: nombreZona = ""} = zone || {};
      const {name: nombreSubzona = ""} = zoneSection || {}; 
      const {visitDirection: sentido = ""} = direction || {};
      return [`${names} ${lastNames}`.trim(), identificationNumber, `${nombreZona} ${nombreSubzona}`, sentido, horaEntrada || "Sin nombre"];
    });
  
    return (
      <Box id="ContainerUltimosRegistrosPageComponent">
        <DatagridResponsive title="Últimos Registros" columns={columns} data={data} />
      </Box>
    );
  };
  
  export default UltimosRegistrosPageComponent;