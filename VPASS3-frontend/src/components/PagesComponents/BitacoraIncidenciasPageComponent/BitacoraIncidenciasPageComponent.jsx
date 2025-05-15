import { Box, Fade } from "@mui/material";
import "./BitacoraIncidenciasPageComponent.css"
import useBitacoraIncidencias from "../../../hooks/useBitacoraIncidencias/useBitacoraIncidencias";
import { useEffect, useState } from "react";
import TableSkeleton from "../../Skeleton/TableSkeleton/TableSkeleton";
import DatagridResponsive from "../../Datagrid/DatagridResponsive/DatagridResponsive";
import dayjs from "dayjs";
import { cambiarFormatoHoraFecha } from "../../../utils/funciones";
const BitacoraIncidenciasPageComponent = () => {
    const {bitacoraIncidencias, getAllBitacoraIncidencias} = useBitacoraIncidencias();
    const [rows, setRows] = useState();

    useEffect(() => {
      getAllBitacoraIncidencias();
    }, [])

    // Cuando carguen las visitas desde el servidor, se proceden a ordenar por fecha.
    useEffect(() => {
        if (!Array.isArray(bitacoraIncidencias)) return;
    
        const incidenciasOrdenadasPorFecha = [...bitacoraIncidencias].sort((a, b) =>
        dayjs(b.timestamp).valueOf() - dayjs(a.timestamp).valueOf()
        );
        setRows(incidenciasOrdenadasPorFecha);
    }, [bitacoraIncidencias]);

    const columns = ["Tipo de acción", "Usuario", "Fecha"]

    const data = rows?.map((bitacora)=>{
        const {action, email, timestamp} = bitacora; 
        return[
            action || "",
            email || "",
            cambiarFormatoHoraFecha(timestamp) || ""
        ]
    })

    return (
        <Box id="ContainerBitacoraIncidenciasPageComponent">
            <Fade in={!(!Array.isArray(rows))} timeout={{ enter: 500, exit: 300 }} unmountOnExit>
                <div>
                    <DatagridResponsive
                        title="Bitacora de incidencias"
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
    );
}

export default BitacoraIncidenciasPageComponent;