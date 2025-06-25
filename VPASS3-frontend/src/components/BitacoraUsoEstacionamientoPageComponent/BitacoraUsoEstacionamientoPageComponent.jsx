import { useEffect, useState } from "react";
import useUsoEstacionamiento from "../../hooks/useUsoEstacionamiento/useUsoEstacionamiento"
import "./BitacoraUsoEstacionamientoPageComponent.css"
import dayjs from "dayjs";
import { Box, Fade } from "@mui/material";
import DatagridResponsive from "../Datagrid/DatagridResponsive/DatagridResponsive";
import TableSkeleton from "../Skeleton/TableSkeleton/TableSkeleton";
import { cambiarFormatoHoraFecha, formatoLegibleDesdeHoraString } from "../../utils/funciones";

const BitacoraUsoEstacionamientoPageComponent = () => {

    // Se obtienen los datos del uso del estacionamiento desde el backend
    const {usoEstacionamiento,getAllUsoEstacionamiento, loading} = useUsoEstacionamiento();

    useEffect(() => {
        getAllUsoEstacionamiento();
    }, [])

    // Estado donde se guardará la informacion del uso del estacionamiento
    const [rows, setRows] = useState()

    // Cuando cargue la info del uso de los estacionamientos desde el servidor, se proceden a ordenar por fecha.
    useEffect(() => {
        if (!Array.isArray(usoEstacionamiento)) return;
    
        const usoEstacionamientoOrdenadosPorFecha = [...usoEstacionamiento].sort((a, b) =>
        dayjs(b.startTime).valueOf() - dayjs(a.startTime).valueOf()
        );
        setRows(usoEstacionamientoOrdenadosPorFecha);
    }, [usoEstacionamiento]);
    
    const columns = ["Nombre visita", "Estacionamiento", "Tiempo Autorizado", "Inicio de uso", "Fin de uso", "Tiempo de uso"];

    const data = rows?.map((usoEstacionamiento) => {
        const {entryVisit, authorizedTime, startTime, endTime, usageTime} = usoEstacionamiento;

        const {person, parkingSpot} = entryVisit;
        const {names = "", lastNames = ""} = person;
        const {name: parkingSpotName = ""} = parkingSpot;

        return [
            `${names} ${lastNames}`.trim() || "No disponible",
            `${parkingSpotName}` || "No disponible",
            `${formatoLegibleDesdeHoraString(authorizedTime)}` || "No disponible",
            `${cambiarFormatoHoraFecha(startTime)}` || "No disponible",
            `${cambiarFormatoHoraFecha(endTime)}` || "Sin reportar",
            `${formatoLegibleDesdeHoraString(usageTime)}` || "Sin reportar"
        ]
    })
  return (
        <Box id="ContainerBitacoraUsoEstacionamientoPageComponent">
            <Fade in={!(!Array.isArray(rows))} timeout={{ enter: 500, exit: 300 }} unmountOnExit>
                <div>
                    <DatagridResponsive
                        title="Registro de uso de estacionamientos"
                        columns={columns}
                        data={data}
                        selectableRows="none" 
                        downloadCsvButton={false}
                    />
                </div>
            </Fade>

            <Fade in={!Array.isArray(rows)} timeout={{ enter: 500, exit: 300 }} unmountOnExit>
                <div>
                    <TableSkeleton columnCount={3} rowCount={7} />
                </div>
            </Fade>
        </Box>
  )
}

export default BitacoraUsoEstacionamientoPageComponent;